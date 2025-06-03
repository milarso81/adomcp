using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;

namespace AdoMCP;

/// <summary>
/// Provides tools for interacting with Azure DevOps pull requests via MCP.
/// </summary>
[McpServerToolType]
public class PullRequestTool
{
    private readonly IAdoPullRequestService _adoPullRequestService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PullRequestTool"/> class.
    /// </summary>
    /// <param name="adoPullRequestService">The Azure DevOps pull request service.</param>
    /// <param name="configuration">The application configuration.</param>
    public PullRequestTool(IAdoPullRequestService adoPullRequestService, IConfiguration configuration)
    {
        _adoPullRequestService = adoPullRequestService;
        _configuration = configuration;
    }

    /// <summary>
    /// Lists pull requests for the given branch in the configured repository.
    /// </summary>
    /// <param name="branch">The branch to list pull requests for.</param>
    /// <param name="repository">The repository name.</param>
    /// <returns>A JSON string containing the list of pull requests.</returns>
    [McpServerTool]
    [Description("List pull requests for the given branch in the configured repository.")]
    public async Task<string> ListPullRequests(
        [Description("The branch to list pull requests for")] string branch,
        [Description("The repository name")] string repository)
    {
        var org = _configuration.GetSetting(
            "Ado:Organization",
            "Azure DevOps organization is not configured. Set 'Ado:Organization' in configuration.");
        var proj = _configuration.GetSetting(
            "Ado:Project",
            "Azure DevOps project is not configured. Set 'Ado:Project' in configuration.");
        if (string.IsNullOrWhiteSpace(repository))
        {
            throw new InvalidOperationException("Repository is not configured or provided.");
        }

        var prs = await _adoPullRequestService.GetPullRequestsAsync(
            org,
            proj,
            repository,
            branch);

        return System.Text.Json.JsonSerializer.Serialize(prs ?? new List<PullRequest>());
    }

    /// <summary>
    /// Gets comments for a specific pull request.
    /// </summary>
    /// <param name="repository">The repository name.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <returns>A JSON string containing the list of pull request comments.</returns>
    [McpServerTool]
    [Description("Get comments for a specific pull request.")]
    public Task<string> GetPullRequestComments(
        [Description("The repository name")] string repository,
        [Description("The pull request ID")] int pullRequestId)
    {
        var org = _configuration.GetSetting(
            "Ado:Organization",
            "Azure DevOps organization is not configured. Set 'Ado:Organization' in configuration.");
        var proj = _configuration.GetSetting(
            "Ado:Project",
            "Azure DevOps project is not configured. Set 'Ado:Project' in configuration.");

        return GetPullRequestCommentsInternal(
            org,
            proj,
            repository,
            pullRequestId);
    }

    private async Task<string> GetPullRequestCommentsInternal(
        string organization,
        string project,
        string repository,
        int pullRequestId)
    {
        var comments = await _adoPullRequestService.GetPullRequestCommentsAsync(
            organization,
            project,
            repository,
            pullRequestId);

        return System.Text.Json.JsonSerializer.Serialize(comments ?? new List<PullRequestComment>());
    }
}
