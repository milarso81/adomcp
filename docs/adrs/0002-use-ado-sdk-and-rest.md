# ADR 0002: Use Azure DevOps SDK (C# Client Library) and REST API for Azure DevOps Communication

## Status
Accepted

## Context
Initially, AdoMCP used only the Azure DevOps REST API for all communication. However, Microsoft provides an official C# SDK (Microsoft.TeamFoundationServer.Client and related packages) that offers a strongly-typed, higher-level API for interacting with Azure DevOps. This SDK can simplify development, improve maintainability, and reduce the need for manual HTTP handling.

## Decision
- The system will use the official Azure DevOps C# SDK (Microsoft.TeamFoundationServer.Client) for communication with ADO services where possible.
- The REST API will still be used for scenarios not covered or not easily supported by the SDK.
- The authentication mechanism (PAT) remains unchanged.
- The IAdoPullRequestService interface will have at least one implementation using the SDK, and may retain the REST-based implementation for fallback or comparison.

## Consequences
- Using the SDK provides a more robust, discoverable, and maintainable integration with Azure DevOps.
- Some advanced or preview features may still require direct REST API calls.
- The codebase will need to support both SDK and REST approaches, at least for a transition period.

## Alternatives Considered
- REST API only: More manual work, but universal and always up-to-date.
- SDK only: Simpler for common scenarios, but may lag behind REST for new features.

## Related Requirements
- See [docs/requirements/main-features.md](../requirements/main-features.md)
- See ADR 0001 for original REST-only decision.
