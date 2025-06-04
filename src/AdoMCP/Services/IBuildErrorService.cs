namespace AdoMCP;

/// <summary>
/// Provides an abstraction for retrieving build errors for a pull request.
/// Requires organization, project, pull request ID, and branch name for all operations.
/// </summary>
public interface IBuildErrorService
{
    /// <summary>
    /// Gets the build errors and stack traces for a given pull request.
    /// </summary>
    /// <param name="organization">The Azure DevOps organization.</param>
    /// <param name="project">The Azure DevOps project.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <returns>A list of build error details.</returns>
    Task<IReadOnlyList<BuildErrorDetail>> GetBuildErrorsAsync(
        string organization,
        string project,
        int pullRequestId);
}
