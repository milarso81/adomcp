namespace AdoMCP;

/// <summary>
/// Service interface for retrieving pull request information from Azure DevOps.
/// </summary>
public interface IAdoPullRequestService
{
    /// <summary>
    /// Retrieves pull requests for a specific repository and branch.
    /// </summary>
    /// <param name="organization">The Azure DevOps organization name.</param>
    /// <param name="project">The project name.</param>
    /// <param name="repository">The repository name.</param>
    /// <param name="branch">The source branch to filter by.</param>
    /// <returns>A read-only list of pull requests.</returns>
    Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(
        string organization,
        string project,
        string repository,
        string branch);

    /// <summary>
    /// Retrieves comments for a specific pull request.
    /// </summary>
    /// <param name="organization">The Azure DevOps organization name.</param>
    /// <param name="project">The project name.</param>
    /// <param name="repository">The repository name.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <returns>A read-only list of pull request comments.</returns>
    Task<IReadOnlyList<PullRequestComment>> GetPullRequestCommentsAsync(
        string organization,
        string project,
        string repository,
        int pullRequestId);
}

/// <summary>
/// Represents a pull request from Azure DevOps.
/// </summary>
/// <param name="id">The unique identifier of the pull request.</param>
/// <param name="title">The title of the pull request.</param>
/// <param name="createdBy">The user who created the pull request.</param>
/// <param name="sourceBranch">The source branch of the pull request.</param>
/// <param name="targetBranch">The target branch of the pull request.</param>
/// <param name="status">The current status of the pull request.</param>
/// <param name="createdDate">The date and time when the pull request was created.</param>
public record PullRequest(
    int id,
    string title,
    string createdBy,
    string sourceBranch,
    string targetBranch,
    string status, DateTime createdDate);

/// <summary>
/// Represents a comment on a pull request from Azure DevOps.
/// </summary>
/// <param name="id">The unique identifier of the comment.</param>
/// <param name="content">The content of the comment.</param>
/// <param name="author">The author of the comment.</param>
/// <param name="createdDate">The date and time when the comment was created.</param>
/// <param name="commentType">The type of comment (general, suggestion, etc.).</param>
public record PullRequestComment(
    int id,
    string content,
    string author,
    DateTime createdDate,
    string commentType);
