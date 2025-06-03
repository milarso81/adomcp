namespace AdoMCP;

/// <summary>
/// Represents a file change in a pull request.
/// </summary>
public record PullRequestFileChange(
    string filePath,
    string changeType,
    string? diff);
