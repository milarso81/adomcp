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
        /// Gets the pull request metadata and file changes for GitHub Copilot review.
        /// </summary>
        /// <param name="organization">The Azure DevOps organization.</param>
        /// <param name="project">The Azure DevOps project.</param>
        /// <param name="repository">The Azure DevOps repository.</param>
        /// <param name="pullRequestId">The pull request ID.</param>
        /// <returns>Pull request change information including metadata and suggested git commands.</returns>
        Task<PullRequestChangeInfo> GetPullRequestChangesAsync(
            string organization,
            string project,
            string repository,
            int pullRequestId);
    }

    // Implementation will be provided in a separate class.
}
