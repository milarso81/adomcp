namespace AdoMCP;

public interface IAdoPullRequestService
{
    Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string organization, string project, string repository, string branch);
}

public record PullRequest(
    int Id,
    string Title,
    string CreatedBy,
    string SourceBranch,
    string TargetBranch,
    string Status,
    DateTime CreatedDate
);
