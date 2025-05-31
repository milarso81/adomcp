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
# AdoMCP - Azure DevOps Model Context Protocol (MCP) Server

AdoMCP is a Model Context Protocol (MCP) server designed to connect with Azure DevOps (ADO) and provide a unified interface for viewing pull requests, build failures, and code review comments.

## Features
- List and view Azure DevOps Pull Requests (PRs)
- Display build failures for PRs and branches
- Show code review comments and discussions
- Designed with Hexagonal Architecture for maintainability and testability
- Built using Test Driven Development (TDD) principles

## Requirements
- .NET 9.0 SDK or later
- Access to an Azure DevOps organization and repository
- (Optional) Azure DevOps Personal Access Token (PAT) for authentication

## Getting Started

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

## Usage
Once running, the MCP server will expose endpoints to:
- List pull requests
- View build failures
- Retrieve code review comments

(Endpoint documentation and usage examples will be added as implementation progresses.)

## Project Structure
- `src/AdoMCP/` - Main server implementation
- `docs/requirements/` - Project requirements
- `docs/adrs/` - Architecture Decision Records (ADRs)
- `docs/copilot-instructions.md` - Copilot instructions
- `CONTRIBUTING.md` - Development and collaboration rules

## Development Practices
- Test Driven Development (TDD)
- Hexagonal Architecture (Ports & Adapters)
- Rule of Three for refactoring
- Simplicity over generalization
- All design decisions documented as ADRs

See [CONTRIBUTING.md](./CONTRIBUTING.md) for full development guidelines.

## License
This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.
