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
