using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AdoMCP;

public class AdoSdkPullRequestService : IAdoPullRequestService
{
    private readonly IConfiguration _configuration;

    public AdoSdkPullRequestService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string organization, string project, string repository, string branch)
    {
        var pat = _configuration["Ado:Pat"];
        if (string.IsNullOrWhiteSpace(pat))
        {
            throw new InvalidOperationException("Azure DevOps PAT is not configured. Set 'Ado:Pat' in configuration.");
        }

        var orgUrl = $"https://dev.azure.com/{organization}";
        var creds = new VssBasicCredential(string.Empty, pat);
        using var connection = new VssConnection(new Uri(orgUrl), creds);
        var gitClient = await connection.GetClientAsync<GitHttpClient>();        // Pass the project name to the SDK call
        var pullRequests = await gitClient.GetPullRequestsAsync(
            project: project,
            repositoryId: repository,
            searchCriteria: new GitPullRequestSearchCriteria
            {
                Status = PullRequestStatus.Active,
                SourceRefName = $"refs/heads/{branch}",
            },
            top: 100); // limit for demo

        return pullRequests.Select(pr => new PullRequest(
            pr.PullRequestId,
            pr.Title ?? string.Empty,
            pr.CreatedBy?.DisplayName ?? string.Empty,
            pr.SourceRefName ?? string.Empty,
            pr.TargetRefName ?? string.Empty,
            pr.Status.ToString(),
            pr.CreationDate)).ToList();
    }
}
