# ADR 0005: Use Shouldly for Fluent Assertions in Unit Tests

## Status
Accepted

## Context

We want our unit tests to be highly readable, expressive, and to provide clear failure messages. Traditional assertion libraries (such as xUnit's Assert) are less fluent and can be harder to read, especially for complex assertions. Shouldly is a popular .NET assertion library that provides fluent, human-readable assertions and better error messages.

## Decision

- We will use the [Shouldly](https://shouldly.readthedocs.io/) library for all unit test assertions in this project.
- All new and existing unit tests should use Shouldly's fluent assertion syntax (e.g., `result.ShouldBe(expected)` instead of `Assert.Equal(expected, result)`).
- Shouldly is included as a NuGet dependency in the test project.
- The CONTRIBUTING.md and other documentation have been updated to reflect this decision.

## Consequences

- Unit tests will be more readable and maintainable.
- Contributors must use Shouldly for all assertions in unit tests.
- Test failure messages will be clearer, aiding debugging.
- No impact on production code or runtime dependencies.

---

Date: 2025-06-01
