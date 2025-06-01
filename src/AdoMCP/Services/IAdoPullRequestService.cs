namespace AdoMCP;

public interface IAdoPullRequestService
{
    Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string organization, string project, string repository, string branch);
}

public record PullRequest(
    int id,
    string title,
    string createdBy,
    string sourceBranch,
    string targetBranch,
    string status,
    DateTime createdDate);
