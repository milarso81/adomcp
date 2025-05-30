# ADR 0001: Use REST API and Personal Access Token (PAT) for Azure DevOps Communication

## Status
Accepted

## Context
AdoMCP needs to interact with Azure DevOps (ADO) to retrieve pull requests, build statuses, and code review comments. Azure DevOps provides both REST and older SOAP APIs, and supports several authentication mechanisms, including OAuth and Personal Access Tokens (PATs).

## Decision
- The system will use the Azure DevOps REST API for all communication with ADO services.
- The system will require the user to provide a Personal Access Token (PAT) for authentication.
- The PAT will be used only in memory for the session and never stored or logged.

## Consequences
- Using the REST API ensures modern, well-documented, and widely supported integration with ADO.
- Using a PAT simplifies authentication and is compatible with most ADO setups.
- The system must ensure that the PAT is handled securely and never exposed or persisted.

## Alternatives Considered
- Using OAuth: More complex for CLI/server tools, requires app registration and user interaction.
- Using legacy APIs: Not recommended due to limited support and documentation.

## Related Requirements
- See [docs/requirements/main-features.md](../requirements/main-features.md)
