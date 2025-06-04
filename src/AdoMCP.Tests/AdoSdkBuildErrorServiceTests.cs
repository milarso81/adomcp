using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;
using Xunit;

namespace AdoMCP.Tests;

/// <summary>
/// Tests for the AdoSdkBuildErrorService to verify build error retrieval from Azure DevOps.
/// These tests focus on the service's ability to find builds in various states and extract errors.
/// </summary>
public class AdoSdkBuildErrorServiceTests
{
    public class WhenRetrievingBuildErrors
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public WhenRetrievingBuildErrors()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["Ado:Pat"]).Returns("fake-pat-token");
        }

        [Fact]
        public async Task WhenPatIsMissing_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["Ado:Pat"]).Returns((string?)null);
            var service = new AdoSdkBuildErrorService(_mockConfiguration.Object);

            // Act
            var action = () => service.GetBuildErrorsAsync("test-org", "test-project", 123);

            // Assert
            _ = await action.ShouldThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task WhenPatIsEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["Ado:Pat"]).Returns(string.Empty);
            var service = new AdoSdkBuildErrorService(_mockConfiguration.Object);

            // Act
            var action = () => service.GetBuildErrorsAsync("test-org", "test-project", 123);

            // Assert
            _ = await action.ShouldThrowAsync<InvalidOperationException>();
        }
    }
}
