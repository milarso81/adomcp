using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AdoMCP;

/// <summary>
/// Provides build error retrieval functionality for pull requests.
/// </summary>
[McpServerToolType]
public class BuildErrorTool
{
    private readonly IBuildErrorService _service;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildErrorTool"/> class.
    /// </summary>
    /// <param name="service">The build error service dependency.</param>
    /// <param name="configuration">The application configuration.</param>
    public BuildErrorTool(
        IBuildErrorService service,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    /// <summary>
    /// Gets build errors for a given pull request as a JSON string.
    /// </summary>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <returns>A JSON string representing the build errors.</returns>
    [McpServerTool]
    [Description("Get build errors for a pull request to help with troubleshooting and code review.")]
    public async Task<string> GetBuildErrorsForPullRequestAsync(
        [Description("The pull request ID")] int pullRequestId)
    {
        string organization = _configuration.GetSetting(
            "Ado:Organization",
            "Azure DevOps organization is not configured. Set 'Ado:Organization' in configuration.");
        string project = _configuration.GetSetting(
            "Ado:Project",
            "Azure DevOps project is not configured. Set 'Ado:Project' in configuration.");

        var errors = await _service.GetBuildErrorsAsync(
            organization,
            project,
            pullRequestId);
        return System.Text.Json.JsonSerializer.Serialize(errors ?? new List<BuildErrorDetail>());
    }
}
