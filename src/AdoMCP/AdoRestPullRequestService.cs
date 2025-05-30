
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AdoMCP;

public class AdoRestPullRequestService : IAdoPullRequestService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AdoRestPullRequestService(IConfiguration configuration, HttpClient? httpClient = null)
    {
        _configuration = configuration;
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string organization, string project, string repository, string branch)
    {
        var pat = _configuration["Ado:Pat"];
        if (string.IsNullOrWhiteSpace(pat))
            throw new InvalidOperationException("Azure DevOps PAT is not configured. Set 'Ado:Pat' in configuration.");

        var url = $"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repository}/pullrequests?searchCriteria.sourceRefName=refs/heads/{branch}&api-version=7.1-preview.1";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        var patEncoded = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", patEncoded);
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        var prList = new List<PullRequest>();
        foreach (var pr in doc.RootElement.GetProperty("value").EnumerateArray())
        {
            prList.Add(new PullRequest(
                pr.GetProperty("pullRequestId").GetInt32(),
                pr.GetProperty("title").GetString() ?? string.Empty,
                pr.GetProperty("createdBy").GetProperty("displayName").GetString() ?? string.Empty,
                pr.GetProperty("sourceRefName").GetString() ?? string.Empty,
                pr.GetProperty("targetRefName").GetString() ?? string.Empty,
                pr.GetProperty("status").GetString() ?? string.Empty,
                pr.GetProperty("creationDate").GetDateTime()
            ));
        }
        return prList;
    }
}
