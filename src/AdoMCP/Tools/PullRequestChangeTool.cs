using Microsoft.Extensions.Configuration;

namespace AdoMCP;

/// <summary>
/// Tool for retrieving file changes in a pull request.
/// </summary>
public class PullRequestChangeTool
{
    private readonly IPullRequestChangeService _service;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PullRequestChangeTool"/> class.
    /// </summary>
    /// <param name="service">The pull request change service.</param>
    /// <param name="configuration">The application configuration.</param>
    public PullRequestChangeTool(
        IPullRequestChangeService service,
        IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the file changes for a pull request as a JSON string.
    /// </summary>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <returns>A JSON string representing the file changes.</returns>
    public Task<string> GetPullRequestChangesAsync(int pullRequestId)
    {
        var organization = _configuration.GetSetting(
            "Ado:Organization",
            "Azure DevOps organization is not configured. Set 'Ado:Organization' in configuration.");
        var project = _configuration.GetSetting(
            "Ado:Project",
            "Azure DevOps project is not configured. Set 'Ado:Project' in configuration.");

        // Minimal implementation for TDD: still not implemented for other cases
        throw new NotImplementedException();
    }
}
