# ADR-0007: Enhanced Git Suggestions for Pull Request Analysis

## Status
Accepted

Supersedes ADR-0006

## Context
While ADR-0006 established that we would provide basic metadata and let GitHub Copilot handle git operations, we discovered that GitHub Copilot instances were not consistently understanding that they could use local git commands for diff operations. This led to suboptimal user experiences where Copilot would request more information from the MCP server rather than executing appropriate git commands locally.

The challenge was that other GitHub Copilot instances analyzing pull request data couldn't easily determine:
1. Whether a local git repository was available
2. What specific git commands would be most useful for the context
3. How to construct appropriate git commands for file-specific vs. branch-level analysis

## Decision
We will enhance the pull request change tool responses to include explicit git command suggestions, making it clear to GitHub Copilot that local git operations are available and recommended.

### Enhancement Details
- **Add `GitSuggestions` data structure** containing ready-to-execute git commands
- **Generate file-specific commands** for individual file analysis (e.g., `git diff HEAD -- file.cs`)
- **Generate branch-level commands** for comprehensive change analysis (e.g., `git diff main..feature/branch`)
- **Include commands in JSON responses** to provide clear guidance to AI agents

### Implementation Approach
Following TDD methodology:
1. Write failing tests for git command inclusion in responses
2. Enhance data structures (`PullRequestChangeInfo`, `GitSuggestions`)
3. Update service implementation to generate appropriate git commands
4. Ensure all tests pass and validate the enhancement

## Consequences

### Positive
- **Clear Intent**: GitHub Copilot can immediately see that git operations are available
- **Specific Guidance**: Provides exact commands rather than requiring AI to construct them
- **Context-Aware**: Commands are tailored to the specific PR context (branches, files)
- **Improved UX**: Users get better assistance as Copilot knows to use local git
- **Maintainable**: TDD approach ensures robust implementation with good test coverage

### Negative
- **Increased Response Size**: JSON responses are slightly larger due to git suggestions
- **Command Generation Logic**: Additional complexity in service layer to generate commands
- **Dependency Assumption**: Assumes local git repository availability (mitigated by clear error handling)

### Mitigation
- Generate commands efficiently without significant performance impact
- Provide meaningful error messages when git repository is not available
- Keep command suggestions focused and relevant to avoid response bloat

## Implementation Details

### Data Structure
```json
{
  "pullRequest": { "id": 123, "title": "...", "sourceBranch": "feature/branch", "targetBranch": "main" },
  "changes": [{ "filePath": "src/Service.cs", "changeType": "Modified" }],
  "gitSuggestions": {
    "fileSpecificCommands": [
      "git diff HEAD -- src/Service.cs",
      "git diff main..feature/branch -- src/Service.cs"
    ],
    "branchCommands": [
      "git diff main..feature/branch",
      "git log main..feature/branch --oneline",
      "git show-branch feature/branch main"
    ]
  }
}
```

### Service Enhancement
- `AdoSdkPullRequestChangeService.GenerateGitSuggestions()` method
- File-specific commands generated per changed file
- Branch commands generated when source and target branches are available
- Graceful handling when branch information is incomplete

### Test Coverage
- Unit tests verify git suggestions are included in responses
- Tests validate file-specific command generation
- Tests verify branch-level command generation
- Comprehensive test coverage following TDD principles

## Related Decisions
- Supersedes ADR-0006: Pull Request Diff Strategy
- Builds on ADR-0002: Use Azure DevOps SDK for API integration
- Follows ADR-0005: Use Shouldly for fluent assertions (used in TDD implementation)

## Future Considerations
- Monitor GitHub Copilot usage patterns to refine command suggestions
- Consider adding more specialized git commands based on user feedback
- Evaluate performance impact if suggestion generation becomes complex
