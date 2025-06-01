using Microsoft.Extensions.Configuration;
using Moq;

namespace AdoMCP.Tests;

public class PullRequestToolTests
{
    /// <summary>
    /// Tests for listing pull requests functionality.
    /// Validates the MCP tool requirement from main-features.md: "Display Current Pull Requests"
    /// - System must retrieve and display all open PRs for the repository and branch.
    /// - System must accept the name of the current branch as input.
    /// </summary>
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

        private PullRequestTool SystemUnderTest => new PullRequestTool(
            _mockService.Object,
            _mockConfig.Object);

        [Fact]
        public async Task WhenPullRequestsExist_ShouldReturnPullRequestsAsJsonList()
        {
            // Arrange
            var prs = new List<PullRequest>
            {
                new (
                    id: 1,
                    title: "Title1",
                    createdBy: "Alice",
                    sourceBranch: "feature/branch",
                    targetBranch: "main",
                    status: "active",
                    createdDate: new DateTime(2025, 5, 30)),
                new (
                    id: 2,
                    title: "Title2",
                    createdBy: "Bob",
                    sourceBranch: "feature/branch",
                    targetBranch: "main",
                    status: "active",
                    createdDate: new DateTime(2025, 5, 29)),
            };

            _mockService.Setup(
                s => s.GetPullRequestsAsync(
                    "org",
                    "proj",
                    "repo",
                    "feature/branch"))
                .ReturnsAsync(
                    prs);

            // Act
            var result = await SystemUnderTest.ListPullRequests("feature/branch", "repo");

            // Assert
            var expectedJson = System.Text.Json.JsonSerializer.Serialize(
                prs);
            Assert.Equal(
                expectedJson,
                result);
        }

        [Fact]
        public async Task WhenNoPullRequestsExist_ShouldReturnEmptyArray()
        {
            // Arrange
            _mockService.Setup(s => s.GetPullRequestsAsync("org", "proj", "repo", "feature/none"))
                .ReturnsAsync(new List<PullRequest>());            // Act
            var result = await SystemUnderTest.ListPullRequests("feature/none", "repo");

            // Assert
            Assert.Equal(
                "[]",
                result);
        }

        [Fact]
        public async Task WhenConfigMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockConfigWithMissingOrg = new Mock<IConfiguration>();
            mockConfigWithMissingOrg.Setup(c => c["Ado:Organization"]).Returns((string?)null);
            var toolWithBadConfig = new PullRequestTool(_mockService.Object, mockConfigWithMissingOrg.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => toolWithBadConfig.ListPullRequests(
                    "feature/branch",
                    "repo"));
        }
    }

    /// <summary>
    /// Tests for listing pull requests comments functionality.
    /// Validates the MCP tool requirement from main-features.md: "PR Selection and Review Comments"
    /// - System must retrieve and display all comments for the specified pull request.
    /// - System must accept the ID of the pull request as input.
    /// </summary>
    public class PullRequestCommentTests
    {
        private readonly Mock<IAdoPullRequestService> _mockService;
        private readonly Mock<IConfiguration> _mockConfig;

        public PullRequestCommentTests()
        {
            _mockService = new Mock<IAdoPullRequestService>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Ado:Organization"]).Returns("org");
            _mockConfig.Setup(c => c["Ado:Project"]).Returns("proj");
        }

        private PullRequestTool SystemUnderTest => new PullRequestTool(
            _mockService.Object,
            _mockConfig.Object);

        [Fact]
        public async Task WhenThereAreNoComments_ShouldReturnEmptyList()
        {
            // Arrange
            _mockService.Setup(s => s.GetPullRequestCommentsAsync("org", "proj", "repo", 123))
                .ReturnsAsync(new List<PullRequestComment>());

            // Act
            var result = await SystemUnderTest.GetPullRequestComments("repo", 123);

            // Assert
            Assert.Equal(
                "[]",
                result);
        }

        [Fact]
        public async Task WhenInvalidOrgConfiguration_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockConfigWithMissingOrg = new Mock<IConfiguration>();
            mockConfigWithMissingOrg.Setup(c => c["Ado:Organization"]).Returns((string?)null);
            var toolWithBadConfig = new PullRequestTool(_mockService.Object, mockConfigWithMissingOrg.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => toolWithBadConfig.GetPullRequestComments(
                    "repo",
                    123));
        }

        [Fact]
        public async Task WhenInvalidProjectConfiguration_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockConfigWithMissingProject = new Mock<IConfiguration>();
            mockConfigWithMissingProject.Setup(c => c["Ado:Organization"]).Returns("org");
            mockConfigWithMissingProject.Setup(c => c["Ado:Project"]).Returns((string?)null);
            var toolWithBadConfig = new PullRequestTool(_mockService.Object, mockConfigWithMissingProject.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => toolWithBadConfig.GetPullRequestComments(
                    "repo",
                    123));
        }

        [Fact]
        public async Task WhenCommentsExist_ShouldReturnCommentsAsJsonList()
        {
            // Arrange
            var comments = new List<PullRequestComment>
            {
                new (id: 1, content: "Great work!", author: "Alice", createdDate: DateTime.Now, commentType: "text"),
                new (id: 2, content: "Needs changes.", author: "Bob", createdDate: DateTime.Now, commentType: "text"),
            };

            _mockService.Setup(s => s.GetPullRequestCommentsAsync("org", "proj", "repo", 123))
                .ReturnsAsync(comments);

            // Act
            var result = await SystemUnderTest.GetPullRequestComments("repo", 123);

            // Assert
            var expectedJson = System.Text.Json.JsonSerializer.Serialize(
                comments);
            Assert.Equal(
                expectedJson,
                result);
        }
    }
}
