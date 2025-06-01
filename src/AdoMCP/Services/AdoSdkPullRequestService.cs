using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AdoMCP;

/// <summary>
/// Implements pull request service using Azure DevOps SDK.
/// </summary>
public class AdoSdkPullRequestService : IAdoPullRequestService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdoSdkPullRequestService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public AdoSdkPullRequestService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    /// <summary>
    /// Gets pull requests for the specified branch in the given repository.
    /// </summary>
    /// <param name="organization">The Azure DevOps organization.</param>
    /// <param name="project">The project name.</param>
    /// <param name="repository">The repository name.</param>
    /// <param name="branch">The branch name.</param>
    /// <returns>A list of pull requests for the specified branch.</returns>
    public async Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string organization, string project, string repository, string branch)
    {
        var pat = this._configuration["Ado:Pat"];
        if (string.IsNullOrWhiteSpace(pat))
        {
            throw new InvalidOperationException("Azure DevOps PAT is not configured. Set 'Ado:Pat' in configuration.");
        }

        var orgUrl = $"https://dev.azure.com/{organization}";
        var creds = new VssBasicCredential(string.Empty, pat);
        using var connection = new VssConnection(new Uri(orgUrl), creds);
        var gitClient = await connection.GetClientAsync<GitHttpClient>();

        // Pass the project name to the SDK call
        var pullRequests = await gitClient.GetPullRequestsAsync(
            project: project,
            repositoryId: repository,
            searchCriteria: new GitPullRequestSearchCriteria
            {
                Status = PullRequestStatus.Active,
                SourceRefName = $"refs/heads/{branch}",
            },
            top: 100); // limit for demo

        return pullRequests.Select(pr => new PullRequest(
            pr.PullRequestId,
            pr.Title ?? string.Empty,
            pr.CreatedBy?.DisplayName ?? string.Empty,
            pr.SourceRefName ?? string.Empty,
            pr.TargetRefName ?? string.Empty,
            pr.Status.ToString(),
            pr.CreationDate)).ToList();
    }

    /// <summary>
    /// Gets comments for a specific pull request.
    /// </summary>
    /// <param name="organization">The Azure DevOps organization.</param>
    /// <param name="project">The project name.</param>
    /// <param name="repository">The repository name.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <returns>A list of pull request comments.</returns>
    public async Task<IReadOnlyList<PullRequestComment>> GetPullRequestCommentsAsync(
        string organization,
        string project,
        string repository,
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
        var gitClient = await connection.GetClientAsync<GitHttpClient>();

        // Get pull request comments (threads)
        var threads = await gitClient.GetThreadsAsync(
            project: project,
            repositoryId: repository,
            pullRequestId: pullRequestId);

        var comments = threads
            .Where(thread => thread.Comments != null)
            .SelectMany(thread => thread.Comments
                .Where(comment => !string.IsNullOrWhiteSpace(comment.Content))
                .Select(comment =>
                {
                    var commentType = thread.Properties?.ContainsKey("Microsoft.TeamFoundation.Discussion.SupportsMarkdown") == true
                        ? "markdown"
                        : "text";
                    return new PullRequestComment(
                        comment.Id,
                        comment.Content,
                        comment.Author?.DisplayName ?? "Unknown",
                        comment.PublishedDate,
                        commentType);
                }))
            .ToList();

        return comments;
    }
}
