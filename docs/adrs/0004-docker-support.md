# ADR 0004: Add Docker Support for MCP Server

## Status
Accepted

## Context
To simplify deployment and ensure consistent environments, the MCP server should be runnable as a Docker container. This allows developers and CI/CD systems to build and run the server without installing the .NET SDK or managing dependencies directly. Docker also enables secure secret injection at runtime.

## Decision
- Add a multi-stage Dockerfile to the project root for building and running the MCP server.
- Add a .dockerignore file to keep images clean and small.
- Document secure usage: secrets (PAT, etc.) must be passed as environment variables at runtime, never baked into the image or stored in files.
- Update the README with build and usage instructions for Docker.

## Consequences
- Developers and CI/CD can build and run the MCP server with a single Docker command.
- The image is portable and can be used in any environment supporting Docker.
- Security is maintained by requiring secrets to be injected at runtime.

## Example Usage

```sh
# Build the image
$ docker build -t adomcp-server .

# Run the container securely
$ docker run --rm -e Ado__Pat="<your-pat>" -e Ado__Organization="your-org" -e Ado__Project="your-project" adomcp-server
```

## Related Requirements
- See [docs/requirements/main-features.md](../requirements/main-features.md)
- See ADR 0003 for secure secret handling.
