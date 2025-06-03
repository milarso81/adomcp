using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AdoMCP;

/// <inheritdoc />
public class AdoSdkPullRequestChangeService : IPullRequestChangeService
{
    /// <inheritdoc />
    public async Task<PullRequestChangeInfo> GetPullRequestChangesAsync(
        string organization,
        string project,
        string repository,
        int pullRequestId)
    {
        using var connection = CreateConnection(organization);
        var gitClient = await connection.GetClientAsync<GitHttpClient>();
        var iterations = await gitClient.GetPullRequestIterationsAsync(
            project,
            repository,
            pullRequestId);
        if (iterations == null || iterations.Count == 0)
        {
            return CreateEmptyPullRequestChangeInfo(pullRequestId);
        }

        var pullRequest = await gitClient.GetPullRequestAsync(
            project,
            repository,
            pullRequestId);

        var latestIteration = iterations?.LastOrDefault();
        var changes = await gitClient.GetPullRequestIterationChangesAsync(
            project,
            repository,
            pullRequestId,
            latestIteration?.Id ?? 1);

        var fileChanges = changes.ChangeEntries
            .Where(change => change.Item?.Path != null)
            .Select(change => new PullRequestFileChange(
                change.Item!.Path,
                change.ChangeType.ToString()))
            .ToArray(); // Use ToArray instead of ToList for IReadOnlyList

        var metadata = new PullRequestMetadata(
            pullRequestId,
            pullRequest.Title ?? "Unknown",
            ExtractBranchName(pullRequest.SourceRefName),
            ExtractBranchName(pullRequest.TargetRefName));

        return new PullRequestChangeInfo(metadata, fileChanges);
    }

    private static VssConnection CreateConnection(string organization)
    {
        string adoUrl = $"https://dev.azure.com/{organization}";
        string? pat = Environment.GetEnvironmentVariable("Ado:Pat");

        if (string.IsNullOrWhiteSpace(pat))
        {
            throw new InvalidOperationException("Azure DevOps PAT not set in environment variable 'Ado:Pat'.");
        }

        var credentials = new VssBasicCredential(string.Empty, pat);
        return new VssConnection(new Uri(adoUrl), credentials);
    }

    private static PullRequestChangeInfo CreateEmptyPullRequestChangeInfo(int pullRequestId)
    {
        return new PullRequestChangeInfo(
            new PullRequestMetadata(pullRequestId, "Unknown", null, null),
            Array.Empty<PullRequestFileChange>());
    }

    private static string? ExtractBranchName(string? refName)
    {
        return refName?.Replace("refs/heads/", string.Empty);
    }
}
