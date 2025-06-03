namespace AdoMCP;

/// <summary>
/// Represents a file change in a pull request.
/// </summary>
public record PullRequestFileChange(
    string filePath,
    string changeType);

/// <summary>
/// Represents pull request metadata and changes for GitHub Copilot review.
/// </summary>
public record PullRequestChangeInfo(
    PullRequestMetadata pullRequest,
    IReadOnlyList<PullRequestFileChange> changes);

/// <summary>
/// Represents pull request metadata.
/// </summary>
public record PullRequestMetadata(
    int id,
    string title,
    string? sourceBranch,
    string? targetBranch);
