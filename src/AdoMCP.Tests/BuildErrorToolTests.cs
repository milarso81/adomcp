using Moq;
using Shouldly;

namespace AdoMCP.Tests;

public class BuildErrorToolTests
{
    public class WhenBuildErrorsExist
    {
        private readonly Mock<IBuildErrorService> _mockBuildErrorService;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;

        public WhenBuildErrorsExist()
        {
            _mockBuildErrorService = new Mock<IBuildErrorService>();
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            _mockConfiguration.Setup(c => c["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(c => c["Ado:Project"]).Returns("test-proj");
        }

        private BuildErrorTool SystemUnderTest => new BuildErrorTool(
            _mockBuildErrorService.Object,
            _mockConfiguration.Object);

        [Fact]
        public async Task ShouldReturnBuildErrorsAsJsonList()
        {
            // Arrange
            int pullRequestId = 123;
            var expectedErrors = new List<AdoMCP.BuildErrorDetail>
            {
                new ("Build failed: error CS1001", null),
                new ("Build failed: error CS1002", "stack trace for error CS1002"),
            };

            _mockBuildErrorService
                .Setup(s => s.GetBuildErrorsAsync("test-org", "test-proj", pullRequestId))
                .ReturnsAsync(expectedErrors);

            // Act
            var result = await SystemUnderTest.GetBuildErrorsForPullRequestAsync(pullRequestId);

            // Assert
            // Use System.Text.Json to serialize expectedErrors for comparison
            var expectedJson = System.Text.Json.JsonSerializer.Serialize(expectedErrors);
            result.ShouldBe(expectedJson);
        }
    }

    public class WhenConfigurationIsMissing
    {
        private readonly Mock<IBuildErrorService> _mockBuildErrorService;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;

        public WhenConfigurationIsMissing()
        {
            _mockBuildErrorService = new Mock<IBuildErrorService>();
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        }

        private BuildErrorTool SystemUnderTest => new BuildErrorTool(
            _mockBuildErrorService.Object,
            _mockConfiguration.Object);

        [Fact]
        public async Task AndProjectMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            int pullRequestId = 123;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns((string?)null);

            // Act
            var action = () => SystemUnderTest.GetBuildErrorsForPullRequestAsync(pullRequestId);

            // Assert
            await action.ShouldThrowAsync<System.InvalidOperationException>();
        }

        [Fact]
        public async Task AndOrganizationMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            int pullRequestId = 123;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns((string?)null);

            // No need to setup _mockBuildErrorService for this test since it should throw before calling the service.

            // Act
            var action = () => SystemUnderTest.GetBuildErrorsForPullRequestAsync(pullRequestId);

            // Assert
            await action.ShouldThrowAsync<System.InvalidOperationException>();
        }
    }
}
