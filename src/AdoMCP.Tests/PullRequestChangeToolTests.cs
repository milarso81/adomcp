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
            var action = () => SystemUnderTest.GetPullRequestChangesAsync(pullRequestId);

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
            var action = () => SystemUnderTest.GetPullRequestChangesAsync(pullRequestId);

            // Assert
            _ = await action.ShouldThrowAsync<InvalidOperationException>();
        }
    }
}
