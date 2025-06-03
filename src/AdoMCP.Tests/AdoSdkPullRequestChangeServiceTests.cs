using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;
using Xunit;

namespace AdoMCP.Tests;

public class AdoSdkPullRequestChangeServiceTests
{
    public class WhenConfigurationIsMissing
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public WhenConfigurationIsMissing()
        {
            _mockConfiguration = new Mock<IConfiguration>();
        }

        private AdoSdkPullRequestChangeService SystemUnderTest => new AdoSdkPullRequestChangeService(
            _mockConfiguration.Object);

        [Fact]
        public async Task AndPatMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _mockConfiguration.Setup(cfg => cfg["Ado:Pat"]).Returns((string?)null);

            // Act
            var action = () => SystemUnderTest.GetPullRequestChangesAsync(
                "test-org", "test-project", "test-repo", 123);

            // Assert
            var exception = await action.ShouldThrowAsync<InvalidOperationException>();
            exception.Message.ShouldContain("Azure DevOps PAT is not configured");
            exception.Message.ShouldContain("Ado:Pat");
        }

        [Fact]
        public async Task AndPatEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _mockConfiguration.Setup(cfg => cfg["Ado:Pat"]).Returns(string.Empty);

            // Act
            var action = () => SystemUnderTest.GetPullRequestChangesAsync(
                "test-org", "test-project", "test-repo", 123);

            // Assert
            var exception = await action.ShouldThrowAsync<InvalidOperationException>();
            exception.Message.ShouldContain("Azure DevOps PAT is not configured");
            exception.Message.ShouldContain("Ado:Pat");
        }
    }
}
