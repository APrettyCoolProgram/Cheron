# Complete remaining follow-up steps for Solitaire variant work
## Understanding
The user asked to complete the remaining follow-up items after the existing prompt/variant tests. The highest-value remaining work is finishing the testability cleanup and adding regression coverage for input isolation while the new-game prompt is open.

## Assumptions
- "Rest of the steps" refers to the previously suggested engineering follow-ups that are automatable in-repo.
- Manual runtime smoke checks and git commit slicing are not directly automatable here, but build/test validation is.
- Existing tests and behavior should remain unchanged while improving structure.

## Approach
Move test-only helper methods out of the main `SolitaireGame.cs` body into a dedicated partial file so production code stays cleaner while preserving internal visibility for tests. Then add regression tests that assert accessibility shortcuts do not leak through when the new-game prompt is open.

Finally, run a full build and the test project to confirm everything still passes.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - remove inline test-hook block
- src/Games/Solitaire/SolitaireGame.Testing.cs - new partial file containing test-only internal helpers
- src/Solitaire.Tests/SolitaireVariantPromptTests.cs - add regression tests for prompt input isolation

## Risks & Open Questions
- Moving helpers must preserve exact semantics so existing tests keep passing.
- Added test hooks should remain minimal and limited to what tests need.

## Steps
1. Remove inline test-hook methods from SolitaireGame.cs
2. Create SolitaireGame.Testing.cs partial with test helpers
3. Extend SolitaireVariantPromptTests with prompt isolation regression tests
4. Build solution and run Solitaire.Tests