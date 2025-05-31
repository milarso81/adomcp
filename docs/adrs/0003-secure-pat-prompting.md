# ADR 0003: Secure Prompting for Azure DevOps PAT in MCP Server

## Status
Accepted

## Context
Storing secrets such as Azure DevOps Personal Access Tokens (PATs) in configuration files (e.g., appsettings.json) is insecure and risks accidental exposure. Modern IDEs and MCP server registration mechanisms (such as in VS Code) support secure prompting for secrets at launch, injecting them as environment variables.

## Decision
- The MCP server will not require the PAT to be present in any configuration file or source code.
- Instead, the PAT will be provided at runtime via a secure prompt (e.g., using VS Code's `inputs` and `env` mapping in `mcp-servers.json` or `devcontainer.json`).
- The server will read the PAT from the environment variable (e.g., `Ado__Pat`).
- Organization and project can also be provided as environment variables or via prompt if desired.

## Consequences
- No secrets are stored in files or source control.
- The developer experience is secure and user-friendly, with the IDE prompting for the PAT as needed.
- The codebase remains compatible with all secure secret injection mechanisms.

## Example (VS Code MCP Server Registration)

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

## Related Requirements
- See [docs/requirements/main-features.md](../requirements/main-features.md)
- See ADR 0001, 0002 for authentication and SDK decisions.
