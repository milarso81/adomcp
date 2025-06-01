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
        private readonly Mock<IAdoPullRequestService> _mockService;
        private readonly Mock<IConfiguration> _mockConfig;

        public ListPullRequestTests()
        {
            _mockService = new Mock<IAdoPullRequestService>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Ado:Organization"]).Returns("org");
            _mockConfig.Setup(c => c["Ado:Project"]).Returns("proj");
        }

        [Fact]
        public async Task WhenPullRequestsExist_ShouldReturnPullRequestsAsJsonList()
        {
            // Arrange
            var prs = new List<PullRequest>
            {
                new(1, "Title1", "Alice", "feature/branch", "main", "active", new DateTime(2025, 5, 30)),
                new(2, "Title2", "Bob", "feature/branch", "main", "active", new DateTime(2025, 5, 29))
            };
            _mockService.Setup(s => s.GetPullRequestsAsync("org", "proj", "repo", "feature/branch"))
                .ReturnsAsync(prs);

            // Act
            var result = await PullRequestTool.ListPullRequests(
                "feature/branch", "repo", _mockService.Object, _mockConfig.Object);

            // Assert
            var expectedJson = System.Text.Json.JsonSerializer.Serialize(prs);
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task WhenNoPullRequestsExist_ShouldReturnEmptyArray()
        {
            // Arrange
            _mockService.Setup(s => s.GetPullRequestsAsync("org", "proj", "repo", "feature/none"))
                .ReturnsAsync(new List<PullRequest>());

            // Act
            var result = await PullRequestTool.ListPullRequests(
                "feature/none", "repo", _mockService.Object, _mockConfig.Object);

            // Assert
            Assert.Equal("[]", result);
        }

        [Fact]
        public async Task WhenConfigMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockConfigWithMissingOrg = new Mock<IConfiguration>();
            mockConfigWithMissingOrg.Setup(c => c["Ado:Organization"]).Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                PullRequestTool.ListPullRequests("feature/branch", "repo", _mockService.Object, mockConfigWithMissingOrg.Object));
        }
    }
    
}
