using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdoMCP;

/// <inheritdoc/>
public class AdoSdkPullRequestChangeService : IPullRequestChangeService
{
    /// <inheritdoc/>
    public Task<IReadOnlyList<PullRequestFileChange>> GetPullRequestChangesAsync(
        string organization,
        string project,
        string repository,
        int pullRequestId)
    {
        throw new NotImplementedException();
    }
}
