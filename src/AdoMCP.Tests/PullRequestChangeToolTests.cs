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
            var gitSuggestions = new GitSuggestions(Array.Empty<string>(), Array.Empty<string>());
            var changeInfo = new PullRequestChangeInfo(metadata, changes, gitSuggestions);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", It.IsAny<string>(), pullRequestId))
                .ReturnsAsync(changeInfo);

            var expectedJson = System.Text.Json.JsonSerializer.Serialize(changeInfo);            // Act
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
        public async Task ShouldIncludeGitCommandsInResponse()
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
                new[] { "git diff main..feature/new-service", "git log main..feature/new-service --oneline" });
            var changeInfo = new PullRequestChangeInfo(metadata, changes, gitSuggestions);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", "test-repo", pullRequestId))
                .ReturnsAsync(changeInfo);

            // Act
            var result = await SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();

            // The result should contain git context/suggestions - these should fail initially
            result.ShouldContain("gitSuggestions");  // This should fail since we haven't implemented it yet
            result.ShouldContain("git diff");       // This should fail since we haven't implemented it yet
        }

        [Fact]
        public async Task ShouldProvideFileSpecificGitCommands()
        {
            // Arrange
            int pullRequestId = 456;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns("test-project");

            var changes = new[]
            {
                new PullRequestFileChange("src/Calculator.cs", "Modified"),
            };

            var metadata = new PullRequestMetadata(pullRequestId, "Fix calculation bug", "bugfix/calc", "main");
            var gitSuggestions = new GitSuggestions(Array.Empty<string>(), Array.Empty<string>());
            var changeInfo = new PullRequestChangeInfo(metadata, changes, gitSuggestions);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", "test-repo", pullRequestId))
                .ReturnsAsync(changeInfo);

            // Act
            var result = await SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            // Should suggest git commands that can help Copilot get diffs
            result.ShouldContain("src/Calculator.cs");
            result.ShouldContain("git");
        }

        [Fact]
        public async Task ShouldProvideBranchComparisonSuggestions()
        {
            // Arrange
            int pullRequestId = 789;
            _mockConfiguration.Setup(cfg => cfg["Ado:Organization"]).Returns("test-org");
            _mockConfiguration.Setup(cfg => cfg["Ado:Project"]).Returns("test-project");

            var changes = new[]
            {
                new PullRequestFileChange("README.md", "Modified"),
            };

            var metadata = new PullRequestMetadata(pullRequestId, "Update documentation", "docs/update", "develop");
            var gitSuggestions = new GitSuggestions(Array.Empty<string>(), Array.Empty<string>());
            var changeInfo = new PullRequestChangeInfo(metadata, changes, gitSuggestions);

            _mockChangeService.Setup(svc => svc.GetPullRequestChangesAsync(
                "test-org", "test-project", "test-repo", pullRequestId))
                .ReturnsAsync(changeInfo);

            // Act
            var result = await SystemUnderTest.GetPullRequestChangesAsync("test-repo", pullRequestId);

            // Assert
            // Should include both source and target branches for git operations
            result.ShouldContain("docs/update");
            result.ShouldContain("develop");
        }
    }
}
