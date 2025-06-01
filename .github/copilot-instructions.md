# GitHub Copilot Instructions for This Project

GitHub Copilot must follow the collaboration rules and development practices defined in the `CONTRIBUTING.md` file at the root of this repository.

- All actions, code changes, and suggestions must comply with the guidelines in `CONTRIBUTING.md`.
- If there is any ambiguity or conflict, the rules in `CONTRIBUTING.md` take precedence.
- Copilot should reference and update `CONTRIBUTING.md` as the source of truth for process and standards.
- **Always complete the full TDD cycle: Red → Green → Refactor. Do not skip the refactoring step after tests pass.**

## PowerShell Terminal Commands
- The user's default shell is PowerShell on Windows
- **Do NOT use `&&` for command chaining** (this causes parser errors in PowerShell)
- Use `;` (semicolon) for command chaining instead
- Example: `cd path; git status` not `cd path && git status`
- Or use separate `run_in_terminal` calls for better clarity

For details, see [CONTRIBUTING.md](../CONTRIBUTING.md).
