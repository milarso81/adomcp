
# AdoMCP: Azure DevOps Model Context Protocol (MCP) Server

AdoMCP is a Model Context Protocol (MCP) server that connects to Azure DevOps (ADO) and allows you to interact with your DevOps environment using the LLM tool of your choice (such as GitHub Copilot, ChatGPT, or other AI assistants). It provides a unified, language-agnostic interface for viewing and managing pull requests, code review comments, and build errors directly from your preferred AI or automation tool.

## Key Features

- **Secure Azure DevOps Authentication:** Accepts a Personal Access Token (PAT) at runtime (never stored or logged) to authenticate with the Azure DevOps REST API.
- **Pull Request Management:** Lists all open pull requests for the repository and branch you specify, and allows you to select and review PRs.
- **Code Review Comments:** Retrieves and displays all code review comments and discussions for a selected PR, enabling AI-assisted review and feedback.
- **Build Error Reporting:** Shows build errors associated with pull requests, and allows AI tools to help you address them.
- **Security by Design:** Secrets and tokens are never stored or exposed; all sensitive configuration is handled via local, gitignored config files or secure environment variables.

## Usage

Once running, the MCP server exposes endpoints to:
- List pull requests for the current repository and branch
- Select a PR and view all code review comments and discussions
- Display build errors associated with a PR

You can use any LLM-enabled tool (such as GitHub Copilot or ChatGPT) to interact with the MCP server and automate or streamline your DevOps workflows.

## Example Prompts for GitHub Copilot

Once the MCP server is running, you can use these example prompts with GitHub Copilot to leverage the Azure DevOps integration for various PR review scenarios:

### 🔍 **Check for Build Errors on PR**
```
"Can you check PR #123 for any build errors and help me understand what's failing? If there are build errors, please suggest specific fixes based on the error messages and code changes in the PR."
```

### 💬 **Get Suggestions to Fix PR Comments**
```
"Please review all the comments on PR #456 and help me address each one. For code review feedback, suggest specific code changes. For discussion items, help me craft appropriate responses."
```

### 📋 **Help Review PRs in a Branch**
```
"I want to provide feedback on the 'feature/user-authentication' branch PR. Can you analyze the code changes, check for potential issues, and suggest improvements? Please focus on security, performance, and code quality."
```

### 🔄 **Comprehensive PR Review**
```
"Please do a complete review of PR #789. Check the code changes using git diff, look for any build errors, review existing comments, and provide a summary with actionable feedback for the author."
```

### 🚀 **PR Approval Assessment**
```
"Help me determine if PR #321 is ready for approval. Check for unresolved comments, build status, and do a final code review. Summarize any blocking issues that need to be addressed before merging."
```

### 🔧 **Code Quality Analysis**
```
"Can you analyze the code changes in PR #555 and suggest improvements for code quality, maintainability, and adherence to best practices? Use git diff to see the specific changes."
```

**💡 Pro Tip:** The tool provides git command suggestions that GitHub Copilot can use locally. You can also ask Copilot to run specific git commands like `git diff main..feature-branch` to get detailed code change analysis.

## Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- Access to an Azure DevOps organization and repository
- (Optional) Azure DevOps Personal Access Token (PAT) for authentication

### 1. Clone the Repository
```powershell
git clone https://github.com/your-org/adomcp.git
cd adomcp
```

### 2. Build the Project
```powershell
dotnet build
```

### 3. Run Unit Tests
```powershell
dotnet test
```


## Running the MCP Server

### Using the terminal

To start the MCP server locally using the .NET CLI, run:

```powershell
dotnet run --project src/AdoMCP/AdoMCP.csproj
```

You must provide the required Azure DevOps environment variables (PAT, organization, and project) either in your shell or via a launch configuration. For example:

```powershell
$env:Ado__Pat = "<your-pat>"
$env:Ado__Organization = "your-org"
$env:Ado__Project = "your-project"
dotnet run --project src/AdoMCP/AdoMCP.csproj
```

Or set them inline for a single command:

```powershell
Ado__Pat="<your-pat>" Ado__Organization="your-org" Ado__Project="your-project" dotnet run --project src/AdoMCP/AdoMCP.csproj
```

> **Note:** Never store your PAT or other secrets in source control. Use secure prompts or secret managers whenever possible.

### Using Docker

You can build and run the MCP server as a Docker container for easy deployment and consistent environments.

#### Build the Docker Image
```sh
docker build -t adomcp-server .
```

#### Run the Container (with secure PAT injection)
```sh
docker run --rm -i \
  -e Ado__Pat="<your-pat>" \
  -e Ado__Organization="your-org" \
  -e Ado__Project="your-project" \
  adomcp-server
```

**Security Note:**
- Never bake secrets into the image. Always pass them at runtime using environment variables or secret managers.
- The container will not start if required secrets are missing.

#### Example: Using with VS Code MCP Server Registration

You can prompt for the PAT and inject it into the container using VS Code's `mcp-servers.json` or `devcontainer.json`:

```json
"inputs": [
  {
    "id": "ado_pat",
    "description": "Azure DevOps Personal Access Token",
    "type": "promptString",
    "password": true
  }
],
"servers": {
  "ado": {
    "type": "stdio",
    "command": "docker",
    "args": [
      "run",
      "-i",
      "--rm",
      "-e", "Ado__Pat=${input:ado_pat}",
      "-e", "Ado__Organization=your-org",
      "-e", "Ado__Project=your-project",
      "adomcp-server"
    ]
  }
}
```

## How to Use the MCP Server Securely

1. **Do not store your Azure DevOps PAT in any configuration file.**
2. Use secure prompting and environment variable injection, as supported by VS Code and other IDEs.

### Example: VS Code MCP Server Registration

Add the following to your `mcp-servers.json` or `devcontainer.json`:

```json
"inputs": [
  {
    "id": "ado_pat",
    "description": "Azure DevOps Personal Access Token",
    "type": "promptString",
    "password": true
  }
],
"servers": {
  "ado": {
    "type": "stdio",
    "command": "dotnet",
    "args": [
      "run",
      "--project",
      "src/AdoMCP/AdoMCP.csproj"
    ],
    "env": {
      "Ado__Pat": "${input:ado_pat}",
      "Ado__Organization": "your-org",
      "Ado__Project": "your-project"
    }
  }
}
```

This will prompt the user for the PAT at launch and inject it securely as an environment variable.

### Environment Variables

The MCP server expects the following environment variables:

- `Ado__Pat`: Azure DevOps Personal Access Token (PAT)
- `Ado__Organization`: Azure DevOps organization name
- `Ado__Project`: Azure DevOps project name

**Never store secrets in files or source control. Always use secure prompts or secret managers.**


## Project Structure
- `src/AdoMCP/` - Main server implementation
- `docs/requirements/` - Project requirements
- `docs/adrs/` - Architecture Decision Records (ADRs)

For development practices, contribution guidelines, and code standards, see [CONTRIBUTING.md](./CONTRIBUTING.md).

## License
This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.
