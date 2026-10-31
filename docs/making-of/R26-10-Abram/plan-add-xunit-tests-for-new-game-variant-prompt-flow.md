# Add xUnit tests for new-game variant prompt flow
## Understanding
The user wants automated tests added for the recently implemented Solitaire variant-selection flow that appears when starting a new game. The repository currently has no test project, so test infrastructure must be introduced first, then focused tests added.

## Assumptions
- The solution currently contains only the main app project and no existing test infrastructure.
- Internal access is needed to test private/internal Solitaire prompt and variant state transitions.
- Keeping production behavior unchanged is required; only test hooks/access are acceptable.

## Approach
Create a dedicated xUnit test project under `src/` targeting .NET 10, reference the main project, and add it to `Cheron.slnx`. To test prompt behavior without invasive refactoring, expose minimal internal test-only members in `SolitaireGame` (state inspection and trigger methods for opening/confirming/canceling/toggling prompt variant) and allow internals visibility to the test assembly.

Then add focused tests in a `Solitaire.Tests` project validating prompt lifecycle and variant transitions: opening prompt preselects active variant, toggling switches selection, confirm applies variant and closes prompt, cancel closes prompt without changing variant. Build and run tests for verification.

## Key Files
- src/Cheron.csproj - add InternalsVisibleTo for test assembly
- src/Cheron.slnx - include new test project
- src/Games/Solitaire/SolitaireGame.cs - add minimal internal test hooks for prompt/variant state transitions
- src/Solitaire.Tests/Solitaire.Tests.csproj - new xUnit test project
- src/Solitaire.Tests/SolitaireVariantPromptTests.cs - new unit tests

## Risks & Open Questions
- Running test methods that touch MonoGame-dependent paths must be avoided; tests should only exercise non-graphics state logic.
- If internal hooks are considered too permissive, they may need conditional compilation later.

## Steps
1. Create xUnit test project structure
2. Register test project in solution
3. Expose minimal internal Solitaire prompt test hooks
4. Add prompt/variant unit tests
5. Build solution and run new tests