
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

### 4. Run the MCP Server
```powershell
dotnet run --project src/AdoMCP/AdoMCP.csproj
```

## Docker Support

You can build and run the MCP server as a Docker container for easy deployment and consistent environments.

### Build the Docker Image
```sh
docker build -t adomcp-server .
```

### Run the Container (with secure PAT injection)
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

### Example: Using with VS Code MCP Server Registration

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

## Secure Configuration and Usage

### How to Use the MCP Server Securely

1. **Do not store your Azure DevOps PAT in any configuration file.**
2. Use secure prompting and environment variable injection, as supported by VS Code and other IDEs.

#### Example: VS Code MCP Server Registration

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
