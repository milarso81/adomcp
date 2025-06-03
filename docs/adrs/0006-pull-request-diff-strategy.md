# ADR-0006: Pull Request Diff Strategy

## Status
Accepted

## Context
We need to provide file change information and diffs for pull requests to enable GitHub Copilot to assist with code reviews. The initial implementation attempted to retrieve actual file diffs using the Azure DevOps SDK, but this approach had several issues:

1. **Complexity**: Required multiple API calls to get PR metadata, commit information, and file content
2. **Performance**: Heavy processing to create diffs programmatically
3. **Testing Difficulty**: Complex mocking required for Azure DevOps SDK interactions
4. **Container Limitation**: MCP server runs in Docker without access to local filesystem/git repository
5. **Reliability**: Multiple points of failure in the diff retrieval process

## Decision
We will simplify the pull request change tool to only provide metadata and let GitHub Copilot handle diff retrieval using local git commands.

### MCP Tool Responsibilities
- Retrieve PR metadata from Azure DevOps (changed files, change types, branch information)
- Provide suggested git commands for getting diffs
- Return structured data that Copilot can use to run appropriate git commands

### GitHub Copilot Responsibilities  
- Execute git commands locally in the user's workspace
- Get actual file diffs using `git diff` commands
- Perform file analysis and review tasks

## Consequences

### Positive
- **Simplified Implementation**: Much simpler service code, easier to test and maintain
- **Better Performance**: Fewer API calls, no heavy diff processing
- **Reliability**: Leverages battle-tested git tooling for diff generation
- **Flexibility**: Copilot can run any git commands appropriate for the context
- **User Experience**: Users get familiar git diff output format

### Negative
- **Dependency on Local Git**: Requires user to have git repository available locally
- **Branch Synchronization**: User needs to have appropriate branches available locally
- **Two-Step Process**: Information retrieval + local git execution instead of one-step solution

### Mitigation
- Provide clear error messages when git repository is not available
- Include suggested git commands in tool output to guide users
- Document the requirement for local git repository access

## Implementation
The `PullRequestChangeTool` returns JSON containing:
- **PR Metadata**: Source/target branches, title, and ID - providing context for git operations
- **File Changes**: List of changed files with change types and per-file git commands
- **Simplified Structure**: No redundant general git commands since GitHub Copilot can infer them from branch information

This approach allows GitHub Copilot to:
- Execute file-specific diffs using the provided git commands
- Generate broader diffs using the branch information (e.g., `git diff {targetBranch}..{sourceBranch}`)
- Choose the most appropriate git commands for each specific context

## Related Decisions
- ADR-0001: Use Azure DevOps SDK for API integration
- ADR-0002: Hexagonal Architecture implementation
