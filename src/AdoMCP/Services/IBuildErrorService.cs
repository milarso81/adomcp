using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdoMCP.Services
{
    /// <summary>
    /// Provides an abstraction for retrieving build errors for a pull request.
    /// Requires organization, project, and pull request ID for all operations.
    /// </summary>
    public interface IBuildErrorService
    {
        /// <summary>
        /// Gets the build errors for a given pull request.
        /// </summary>
        /// <param name="organization">The Azure DevOps organization.</param>
        /// <param name="project">The Azure DevOps project.</param>
        /// <param name="pullRequestId">The pull request ID.</param>
        /// <returns>A list of build error messages.</returns>
        Task<IReadOnlyList<string>> GetBuildErrorsAsync(
            string organization,
            string project,
            int pullRequestId);
    }
}
