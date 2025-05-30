using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using AdoMCP;
using System.Text.Json;
using System.Collections.Generic;

public class AdoRestPullRequestServiceTests
{
    [Fact]
    public async Task GetPullRequestsAsync_ReturnsPullRequests()
    {
        // Arrange
        var json = "{\"value\":[{\"pullRequestId\":1,\"title\":\"Test PR\",\"createdBy\":{\"displayName\":\"Alice\"},\"sourceRefName\":\"refs/heads/feature\",\"targetRefName\":\"refs/heads/main\",\"status\":\"active\",\"creationDate\":\"2024-05-30T12:00:00Z\"}]}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json),
            });
        var httpClient = new HttpClient(handlerMock.Object);

        var configMock = new Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(c => c["Ado:Pat"]).Returns("pat");
        var service = new AdoRestPullRequestService(configMock.Object, httpClient);

        // Act
        var result = await service.GetPullRequestsAsync("org", "proj", "repo", "feature");

        // Assert
        Assert.Single(result);
        var pr = result[0];
        Assert.Equal(1, pr.Id);
        Assert.Equal("Test PR", pr.Title);
        Assert.Equal("Alice", pr.CreatedBy);
        Assert.Equal("refs/heads/feature", pr.SourceBranch);
        Assert.Equal("refs/heads/main", pr.TargetBranch);
        Assert.Equal("active", pr.Status);
        Assert.Equal(new System.DateTime(2024, 5, 30, 12, 0, 0, System.DateTimeKind.Utc), pr.CreatedDate);
    }
}
