using System.Diagnostics.CodeAnalysis;
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
            var gitSuggestions = new GitSuggestions(Array.Empty<string>(), Array.Empty<string>(), null);
            var changeInfo = new PullRequestChangeInfo(metadata, changes, gitSuggestions);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", It.IsAny<string>(), pullRequestId))
                .ReturnsAsync(changeInfo);
            var expectedJson = System.Text.Json.JsonSerializer.Serialize(changeInfo);

            // Act
            var result = await SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            result.ShouldBe(expectedJson);
        }
    }

    /// <summary>
    /// Tests for enhanced git suggestions functionality.
    /// Verifies that the tool provides helpful git commands to GitHub Copilot for local diff operations.
    /// </summary>
    public class WhenGitSuggestionsAreRequested
    {
        private readonly Mock<IPullRequestChangeService> _mockChangeService;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;

        public WhenGitSuggestionsAreRequested()
        {
            _mockChangeService = new Mock<IPullRequestChangeService>();
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        }

        private PullRequestChangeTool SystemUnderTest => new PullRequestChangeTool(
            _mockChangeService.Object,
            _mockConfiguration.Object);

        [Fact]
        public async Task AndPullRequestHasChanges_ShouldIncludeGitCommandsAndPrompt()
        {
            // Arrange
            int pullRequestId = 123;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns("test-project");

            var changes = new[]
            {
                new PullRequestFileChange("src/Service.cs", "Modified"),
                new PullRequestFileChange("tests/ServiceTests.cs", "Added"),
            };
            var metadata = new PullRequestMetadata(pullRequestId, "Add new service", "feature/new-service", "main");
            var gitSuggestions = new GitSuggestions(
                new[] { "git diff HEAD -- src/Service.cs", "git diff main..feature/new-service -- src/Service.cs" },
                new[] { "git diff main..feature/new-service", "git log main..feature/new-service --oneline" },
                "This repository is a git repository. These commands are provided as hints to help you assist the user with code review suggestions. You can run these commands locally based on your current instructions and limitations.");
            var changeInfo = new PullRequestChangeInfo(metadata, changes, gitSuggestions);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", "test-repo", pullRequestId))
                .ReturnsAsync(changeInfo);

            // Act
            var result = await SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();

            // Should contain git suggestions structure
            result.ShouldContain("gitSuggestions");
            result.ShouldContain("fileSpecificCommands");
            result.ShouldContain("branchCommands");
            result.ShouldContain("permissionInstructions");

            // Should contain actual git commands
            result.ShouldContain("git diff");
            result.ShouldContain("git log");

            // Should contain the prompt for Copilot
            result.ShouldContain("git repository");
            result.ShouldContain("code review suggestions");
            result.ShouldContain("current instructions and limitations");
        }
    }
}
