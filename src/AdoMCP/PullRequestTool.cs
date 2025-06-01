using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;

namespace AdoMCP;

[McpServerToolType]
public static class PullRequestTool
{
    [McpServerTool]
    [Description("List pull requests for the given branch in the configured repository.")]
    public static async Task<string> ListPullRequests(
        [Description("The branch to list pull requests for")] string branch,
        [Description("The repository name")] string repository,
        IAdoPullRequestService adoPullRequestService,
        IConfiguration configuration)
    {
        var org = configuration["Ado:Organization"];
        var proj = configuration["Ado:Project"];
        if (string.IsNullOrWhiteSpace(org) || string.IsNullOrWhiteSpace(proj) || string.IsNullOrWhiteSpace(repository))
        {
            throw new InvalidOperationException("Ado:Organization, Project, or repository is not configured or provided.");
        }

        var prs = await adoPullRequestService.GetPullRequestsAsync(org, proj, repository, branch);
        return System.Text.Json.JsonSerializer.Serialize(prs ?? new List<PullRequest>());
    }
}
