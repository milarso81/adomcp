namespace AdoMCP;

/// <summary>
/// Represents a file change in a pull request.
/// </summary>
public record PullRequestFileChange(
    string filePath,
    string changeType);

/// <summary>
/// Git command suggestions to help GitHub Copilot work with local git operations.
/// </summary>
public record GitSuggestions(
    IReadOnlyList<string> fileSpecificCommands,
    IReadOnlyList<string> branchCommands,
    string? permissionInstructions);

/// <summary>
/// Represents pull request metadata and changes for GitHub Copilot review.
/// </summary>
public record PullRequestChangeInfo(
    PullRequestMetadata pullRequest,
    IReadOnlyList<PullRequestFileChange> changes,
    GitSuggestions gitSuggestions);

/// <summary>
/// Represents pull request metadata.
/// </summary>
public record PullRequestMetadata(
    int id,
    string title,
    string? sourceBranch,
    string? targetBranch);
