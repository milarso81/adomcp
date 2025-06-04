using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AdoMCP;

/// <inheritdoc />
public class AdoSdkPullRequestChangeService : IPullRequestChangeService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdoSdkPullRequestChangeService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public AdoSdkPullRequestChangeService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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

        var gitSuggestions = GenerateGitSuggestions(metadata, fileChanges);

        return new PullRequestChangeInfo(metadata, fileChanges, gitSuggestions);
    }

    private VssConnection CreateConnection(string organization)
    {
        string adoUrl = $"https://dev.azure.com/{organization}";
        string pat = _configuration.GetSetting(
            "Ado:Pat",
            "Azure DevOps PAT is not configured. Set 'Ado:Pat' in configuration.");

        var credentials = new VssBasicCredential(string.Empty, pat);
        return new VssConnection(new Uri(adoUrl), credentials);
    }

    private PullRequestChangeInfo CreateEmptyPullRequestChangeInfo(int pullRequestId)
    {
        return new PullRequestChangeInfo(
            new PullRequestMetadata(pullRequestId, "Unknown", null, null),
            Array.Empty<PullRequestFileChange>(),
            new GitSuggestions(Array.Empty<string>(), Array.Empty<string>(), null));
    }

    private string? ExtractBranchName(string? refName)
    {
        return refName?.Replace("refs/heads/", string.Empty);
    }

    private GitSuggestions GenerateGitSuggestions(PullRequestMetadata metadata, PullRequestFileChange[] fileChanges)
    {
        var fileSpecificCommands = new List<string>();
        var branchCommands = new List<string>();

        // Generate file-specific git commands
        foreach (var change in fileChanges)
        {
            fileSpecificCommands.Add($"git diff HEAD -- {change.filePath}");
            if (metadata.sourceBranch != null && metadata.targetBranch != null)
            {
                fileSpecificCommands.Add($"git diff {metadata.targetBranch}..{metadata.sourceBranch} -- {change.filePath}");
            }
        }

        // Generate branch comparison commands
        if (metadata.sourceBranch != null && metadata.targetBranch != null)
        {
            branchCommands.Add($"git diff {metadata.targetBranch}..{metadata.sourceBranch}");
            branchCommands.Add($"git log {metadata.targetBranch}..{metadata.sourceBranch} --oneline");
            branchCommands.Add($"git show-branch {metadata.sourceBranch} {metadata.targetBranch}");
        }

        return new GitSuggestions(
            fileSpecificCommands.AsReadOnly(),
            branchCommands.AsReadOnly(),
            "This repository is a git repository. These commands are provided as hints to help you assist the user with code review suggestions. You can run these commands locally based on your current instructions and limitations.");
    }
}
