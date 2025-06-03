
# GitHub Copilot Instructions for This Project

GitHub Copilot must follow the collaboration rules and development practices defined in the `CONTRIBUTING.md` file at the root of this repository.

- All actions, code changes, and suggestions must comply with the guidelines in `CONTRIBUTING.md`.
- If there is any ambiguity or conflict, the rules in `CONTRIBUTING.md` take precedence.
- Copilot should reference and update `CONTRIBUTING.md` as the source of truth for process and standards.

## Test Driven Development (TDD) - Stepwise Guidance

**TDD Process:**

If the user does not start the process with a unit test, you must guide the user through TDD one step at a time:

  1. **Start with error handling and edge cases:** Write failing unit tests for error handling, invalid input, and edge cases first.
  2. **Then write tests for the main/positive logic:** After covering edge cases, write tests for the standard/expected flows.
  3. **Run the unit tests** and confirm that the correct assertions are failing (show the failure and check with the user if needed).
  4. **Write the simplest code necessary** to make the tests pass.
  5. **Pause and allow the user to review the code** and suggest refactoring or improvements before proceeding.
  6. Only after user review and approval, continue with further refactoring or additional tests as needed.

If the user starts with a unit test, proceed with the standard TDD cycle as described in `CONTRIBUTING.md`.

**Never skip steps in the TDD cycle.**

**Goal:** Ensure the user can review and influence each step of the TDD process, especially when the process is initiated without a unit test.

## PowerShell Terminal Commands
- The user's default shell is PowerShell on Windows
- **Do NOT use `&&` for command chaining** (this causes parser errors in PowerShell)
- Use `;` (semicolon) for command chaining instead
- Example: `cd path; git status` not `cd path && git status`
- Or use separate `run_in_terminal` calls for better clarity

## Windows-Specific Considerations
- **Line Endings**: Always use Windows line endings (`\r\n`) when creating or editing files
- This is important for consistency with the Windows environment and Git configuration

## Common Issues
- **StyleCop SA1516 Error**: If you see `CSC : error SA1516: Elements should be separated by blank line`, this is typically a problem in `Program.cs` where elements need blank lines between them.

## Formatting
- After you finish writing code, always run `dotnet format` to ensure there are no formatting issues before considering the task complete.

For details, see [CONTRIBUTING.md](../CONTRIBUTING.md).
