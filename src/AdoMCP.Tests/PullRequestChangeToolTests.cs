using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;

namespace AdoMCP.Tests;

public class PullRequestChangeToolTests
{
    public class WhenConfigurationIsMissing
    {
        private readonly Mock<IPullRequestChangeService> _mockChangeService;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;

        public WhenConfigurationIsMissing()
        {
            _mockChangeService = new Mock<IPullRequestChangeService>();
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        }

        private PullRequestChangeTool SystemUnderTest => new PullRequestChangeTool(
            _mockChangeService.Object,
            _mockConfiguration.Object);

        [Fact]
        public async Task AndProjectMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            int pullRequestId = 123;
            _ = _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _ = _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns((string?)null);

            // Act
            var action = () => SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            _ = await action.ShouldThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AndOrganizationMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            int pullRequestId = 123;
            _ = _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns((string?)null);
            _ = _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns("test-project");

            // Act
            var action = () => SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            _ = await action.ShouldThrowAsync<InvalidOperationException>();
        }
    }

    public class WhenPullRequestExists
    {
        private readonly Mock<IPullRequestChangeService> _mockChangeService;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;

        public WhenPullRequestExists()
        {
            _mockChangeService = new Mock<IPullRequestChangeService>();
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        }

        private PullRequestChangeTool SystemUnderTest => new PullRequestChangeTool(
            _mockChangeService.Object,
            _mockConfiguration.Object);

        [Fact]
        public async Task ShouldReturnFileChangesAsJsonList()
        {
            // Arrange
            int pullRequestId = 42;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns("test-project");
            var changes = new[]
            {
                new PullRequestFileChange("src/Service.cs", "Modified"),
                new PullRequestFileChange("README.md", "Added"),
            };

            var metadata = new PullRequestMetadata(pullRequestId, "Test PR", "feature", "main");
            var changeInfo = new PullRequestChangeInfo(metadata, changes);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", It.IsAny<string>(), pullRequestId))
                .ReturnsAsync(changeInfo);

            var expectedJson = System.Text.Json.JsonSerializer.Serialize(changeInfo);            // Act
            var result = await SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            result.ShouldBe(expectedJson);
        }
    }
}
