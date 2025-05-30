# Main Features Requirements - AdoMCP

## 1. Azure DevOps Authentication
- The system must accept an Azure DevOps (ADO) Personal Access Token (PAT) from the user.
- The system must use the PAT to authenticate with the Azure DevOps REST API.
- The PAT must not be stored or logged; it should only be used in memory for the session.

## 2. MCP Tool: Display Current Pull Requests
- The system must expose a Model Context Protocol (MCP) tool to display current pull requests (PRs) for the repository associated with the user's current branch.
- The system must accept the name of the current branch as input.
- The system must retrieve and display all open PRs for the repository and branch.

## 3. PR Selection and Review Comments
- The user must be able to select a PR from the list of open PRs.
- The system must retrieve and display all code review comments and discussions for the selected PR.
- The system must allow GitHub Copilot to assist the user in addressing code review feedback.

## 4. Build Errors on PRs
- The system must retrieve and display any build errors associated with the selected PR.
- The system must allow GitHub Copilot to assist the user in fixing build errors as a separate tool.

## 5. Security
- The system must not store or expose secrets, tokens, or sensitive information.
- All configuration for secrets must be handled via local, gitignored config files.

---

Additional requirements and refinements should be added as the project evolves.
