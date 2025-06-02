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
        var buildClient = await connection.GetClientAsync<BuildHttpClient>();

        // Find builds associated with the pull request
        var builds = await buildClient.GetBuildsAsync(
            project: project,
            reasonFilter: BuildReason.PullRequest,
            statusFilter: BuildStatus.Completed,
            queryOrder: BuildQueryOrder.FinishTimeDescending,
            top: 1); // Only get the most recent build

        // Suppress SA1305 this is not hungarian notation, just an abbreviation for pull request.
        // StyleCop warning SA1305: Field names must not use Hungarian notation.
#pragma warning disable SA1305
        var pullRequestBuilds = builds.Where(b => b.TriggerInfo != null &&
            b.TriggerInfo.TryGetValue("pr.number", out var prNum) &&
            int.TryParse(prNum, out var prId) && prId == pullRequestId).ToList();
#pragma warning restore SA1305

        var errorDetails = new List<BuildErrorDetail>();

        foreach (var build in pullRequestBuilds)
        {
            var logs = await buildClient.GetBuildLogsAsync(project, build.Id);
            foreach (var log in logs)
            {
                var lines = await buildClient.GetBuildLogLinesAsync(project, build.Id, log.Id, 0, 1000);
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
        }

        return errorDetails;
    }
}

/// <summary>
/// Represents a build error and its associated stack trace (if any).
/// </summary>
public record BuildErrorDetail(string errorMessage, string? stackTrace);
