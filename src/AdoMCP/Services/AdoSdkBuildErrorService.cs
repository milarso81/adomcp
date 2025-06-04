using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AdoMCP;

/// <summary>
/// Implements build error retrieval using Azure DevOps SDK.
/// </summary>
public class AdoSdkBuildErrorService : IBuildErrorService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdoSdkBuildErrorService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public AdoSdkBuildErrorService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BuildErrorDetail>> GetBuildErrorsAsync(
        string organization,
        string project,
        int pullRequestId)
    {
        var pat = this._configuration["Ado:Pat"];
        if (string.IsNullOrWhiteSpace(pat))
        {
            throw new InvalidOperationException("Azure DevOps PAT is not configured. Set 'Ado:Pat' in configuration.");
        }

        var orgUrl = $"https://dev.azure.com/{organization}";
        var creds = new VssBasicCredential(string.Empty, pat);
        using var connection = new VssConnection(new Uri(orgUrl), creds);
        var buildClient = await connection.GetClientAsync<BuildHttpClient>();        // Find builds associated with the pull request
        var builds = await buildClient.GetBuildsAsync(
            project: project,
            branchName: $"refs/pull/{pullRequestId}/merge", // Use the PR merge branch
            reasonFilter: BuildReason.PullRequest,
            statusFilter: BuildStatus.Completed,
            minFinishTime: DateTime.UtcNow.AddDays(-30), // Limit to last 30 days
            queryOrder: BuildQueryOrder.FinishTimeDescending,
            top: 100);

        var errorDetails = new List<BuildErrorDetail>();
        var build = builds.FirstOrDefault();
        if (build == null)
        {
            return errorDetails;
        }

        var logs = await buildClient.GetBuildLogsAsync(project, build.Id);
        foreach (var log in logs)
        {
            var lines = await buildClient.GetBuildLogLinesAsync(project, build.Id, log.Id);
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    // Collect simple stack trace: consecutive indented or 'at ' lines after error
                    var stackTraceLines = new List<string>();
                    int j = i + 1;
                    while (j < lines.Count && (lines[j].StartsWith(" ") || lines[j].TrimStart().StartsWith("at ")))
                    {
                        stackTraceLines.Add(lines[j]);
                        j++;
                    }

                    errorDetails.Add(new BuildErrorDetail(line, stackTraceLines.Count > 0 ? string.Join("\n", stackTraceLines) : null));
                }
            }
        }

        return errorDetails;
    }
}

/// <summary>
/// Represents a build error and its associated stack trace (if any).
/// </summary>
public record BuildErrorDetail(string errorMessage, string? stackTrace);
