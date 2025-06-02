# Contribution Guidelines

These are the rules and practices for collaborating on this project with GitHub Copilot and humans:

## 1. Test Driven Development (TDD)
- For all code where TDD is requested, follow this process:
  1. Write a failing unit test for the new feature or bug fix.
  2. Run the test and confirm the correct assertion is failing.
  3. Write the simplest code necessary to make the test pass.
  4. Refactor the code for clarity and maintainability, ensuring all tests still pass.
- Only skip TDD if explicitly instructed.

## 2. Hexagonal Architecture (Ports & Adapters)
- Structure code into core domain logic (inside the hexagon) and adapters (outside the hexagon).
- Keep business logic independent from frameworks, databases, and external services.
- Use interfaces (ports) for all external dependencies.

## 3. Rule of Three for Refactoring
- Only refactor to remove duplication when there are three or more similar code instances.
- Prioritize clarity and simplicity over premature abstraction.

## 4. Simplicity Over Generalization
- Write code that is easy to read and understand.
- Avoid making code generic unless there is a clear, repeated need.

## 5. Architecture Decision Records (ADR)
- Every significant design or architectural decision must be documented as an ADR.
- Store ADRs in `docs/adrs/` using the standard ADR template.

## 6. Requirements Documentation
- All requirements should be documented in `docs/requirements/`.
- Update requirements as the project evolves.

## 7. README Maintenance
- Keep the `README.md` up to date with:
  - Project overview and purpose
  - How to build and run the MCP server
  - How to run tests
  - Any prerequisites or setup steps

## 8. Unit Testing Standards
- For now, only write and maintain unit tests (no integration or end-to-end tests).
- Place unit tests in a dedicated test project or folder, following .NET conventions.

### Test Organization and Naming
- **Group tests by action**: Create inner classes within test classes to group tests by the specific action or method being tested.
  - Example: `ListPullRequestTests`, `CreatePullRequestTests`, etc.
- **Test method naming convention**: Use the pattern `When<Condition>_Should<ExpectedResult>()`
  - `When<Condition>`: Describes the state or scenario being tested
  - `Should<ExpectedResult>`: Describes the expected outcome or behavior
  - Examples:
    - `WhenPullRequestsExist_ShouldReturnPullRequestsAsJsonList()`
    - `WhenNoPullRequestsExist_ShouldReturnEmptyArray()`
    - `WhenConfigMissing_ShouldThrowInvalidOperationException()`
- **Test structure**: Always follow the Arrange-Act-Assert pattern with clear comments marking each section.

### xUnit Best Practices
- **Constructor for setup**: Use the class constructor for common test setup (mocks, test data). xUnit creates a new instance for each test.
- **Member variables for shared resources**: Use private readonly fields for mocks and test data that are used across multiple tests.
- **System Under Test pattern**: Create the system under test using a property that returns a fresh instance:
  ```csharp
  private PullRequestTool SystemUnderTest => new PullRequestTool(
      _mockService.Object,
      _mockConfig.Object);
  ```
  This ensures each test gets a fresh instance while keeping the creation logic centralized and readable.
- **Dispose pattern**: Implement `IDisposable` if tests need cleanup (rare for unit tests).
- **Test isolation**: Each test should be independent and not rely on the state from other tests.

### Assertion Library
- **Use Shouldly for assertions**: All unit test assertions should use the [Shouldly](https://shouldly.readthedocs.io/) library for fluent, readable assertions.
  - Example:
    ```csharp
    result.ShouldBe(expectedJson);
    comments.Count.ShouldBe(2);
    await action.ShouldThrowAsync<InvalidOperationException>();
    ```
- Shouldly is already included as a NuGet dependency in the test project.

## 9. Code Quality and Standards
- Follow standard .NET coding conventions and best practices.
- **Method argument formatting**: When writing method signatures, list each argument on its own line, aligned vertically, as shown in `AdoSdkPullRequestService.GetPullRequestCommentsAsync`. This improves readability and makes it easier to see and document each parameter. Example:
  ```csharp
  public async Task<IReadOnlyList<PullRequestComment>> GetPullRequestCommentsAsync(
      string organization,
      string project,
      string repository,
      int pullRequestId)
  {
      // ...
  }
  ```
- **StyleCop Analyzers**: The project uses StyleCop to enforce consistent code style and quality.
  - StyleCop warnings are treated as errors and will prevent builds from succeeding.
  - Configuration is defined in `stylecop.json` and `stylecop.ruleset` files.
  - Key rules enforced:
    - Proper spacing, indentation, and formatting
    - Consistent naming conventions (PascalCase for public members, camelCase for parameters, underscore prefix for private fields)
    - Required documentation for public APIs (main project only - not required for unit tests)
    - Consistent ordering of using statements and class members
    - Proper file organization (one type per file, matching file names)
  - **Test Projects**: Unit test projects have a separate StyleCop configuration that disables documentation requirements while maintaining all other code quality rules.
  - **Common StyleCop Error**: If you see `CSC : error SA1516: Elements should be separated by blank line`, this is typically a problem in `Program.cs` where elements need blank lines between them.
- Use clear, descriptive commit messages.
- Keep pull requests small and focused.
- Use consistent code formatting and naming conventions.
- Document public APIs and important classes/methods with XML comments (main project only).

## 10. Security and Secrets Management
- Never check in secrets, passwords, API keys, or sensitive credentials to the repository.
- Use local configuration files for secrets and environment-specific settings. These files must be listed in `.gitignore` and not checked in.
- Review code and configuration for accidental exposure of sensitive information before committing.

## 11. Committing Code
- Once all tests are passing and StyleCop rules are satisfied, commit the code.
- Ensure the build succeeds without warnings before committing.
