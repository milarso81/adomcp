using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdoMCP
{
    /// <summary>
    /// Provides an abstraction for retrieving file changes in a pull request.
    /// </summary>
    public interface IPullRequestChangeService
    {
        /// <summary>
        /// Gets the list of file changes for a given pull request.
        /// </summary>
        /// <param name="organization">The Azure DevOps organization.</param>
        /// <param name="project">The Azure DevOps project.</param>
        /// <param name="repository">The Azure DevOps repository.</param>
        /// <param name="pullRequestId">The pull request ID.</param>
        /// <returns>A list of file changes in the pull request.</returns>
        Task<IReadOnlyList<PullRequestFileChange>> GetPullRequestChangesAsync(
            string organization,
            string project,
            string repository,
            int pullRequestId);
    }

    // Implementation will be provided in a separate class.
}
