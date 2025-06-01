using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AdoMCP.Tests;

public class PullRequestToolTests
{
    public class ListPullRequestTests
    {
        [Fact]
        public async Task WhenPullRequestsExist_ShouldReturnPullRequestsAsJsonList()
        {
            // Arrange
            var mockService = new Mock<IAdoPullRequestService>();
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Ado:Organization"]).Returns("org");
            mockConfig.Setup(c => c["Ado:Project"]).Returns("proj");
            var prs = new List<PullRequest>
            {
                new(1, "Title1", "Alice", "feature/branch", "main", "active", new DateTime(2025, 5, 30)),
                new(2, "Title2", "Bob", "feature/branch", "main", "active", new DateTime(2025, 5, 29))
            };
            mockService.Setup(s => s.GetPullRequestsAsync("org", "proj", "repo", "feature/branch"))
                .ReturnsAsync(prs);

            // Act
            var result = await PullRequestTool.ListPullRequests(
                "feature/branch", "repo", mockService.Object, mockConfig.Object);

            // Assert
            var expectedJson = System.Text.Json.JsonSerializer.Serialize(prs);
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task WhenNoPullRequestsExist_ShouldReturnEmptyArray()
        {
            var mockService = new Mock<IAdoPullRequestService>();
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Ado:Organization"]).Returns("org");
            mockConfig.Setup(c => c["Ado:Project"]).Returns("proj");
            mockService.Setup(s => s.GetPullRequestsAsync("org", "proj", "repo", "feature/none"))
                .ReturnsAsync(new List<PullRequest>());

            var result = await PullRequestTool.ListPullRequests(
                "feature/none", "repo", mockService.Object, mockConfig.Object);

            Assert.Equal("[]", result);
        }

        [Fact]
        public async Task WhenConfigMissing_ShouldThrowInvalidOperationException()
        {
            var mockService = new Mock<IAdoPullRequestService>();
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Ado:Organization"]).Returns((string?)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                PullRequestTool.ListPullRequests("feature/branch", "repo", mockService.Object, mockConfig.Object));
        }
    }
    
}
