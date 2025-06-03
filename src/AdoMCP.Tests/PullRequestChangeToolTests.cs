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

        private AdoMCP.PullRequestChangeTool SystemUnderTest => new AdoMCP.PullRequestChangeTool(
            _mockChangeService.Object,
            _mockConfiguration.Object);

        [Fact]
        public async Task AndProjectMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            int pullRequestId = 123;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns((string?)null);
            _mockConfiguration.Setup(cfg => cfg["Ado:Repository"]).Returns("test-repo");

            // Act
            var action = () => SystemUnderTest.GetPullRequestChangesAsync(pullRequestId);

            // Assert
            await action.ShouldThrowAsync<System.InvalidOperationException>();
        }
    }
}
