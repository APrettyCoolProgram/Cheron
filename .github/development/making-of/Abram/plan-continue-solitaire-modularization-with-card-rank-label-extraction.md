# Continue Solitaire modularization with card-rank label extraction
## Understanding
The user asked to continue the refactor. A low-risk next step is extracting rank label mapping logic from `SolitaireGame.cs` into a dedicated helper while preserving behavior.
## Assumptions
- Rank label mappings are stable utility logic suitable for a static helper.
- Keeping wrappers in `SolitaireGame` is unnecessary; direct helper calls are acceptable.
## Approach
Create a new helper file with short and long rank label methods, update call sites in `SolitaireGame.cs` to use the helper, and remove the duplicated private methods.
## Key Files
- src/Games/Solitaire/SolitaireCardLabels.cs - new helper for short/long rank labels.
- src/Games/Solitaire/SolitaireGame.cs - call helper and remove in-class label methods.
## Risks & Open Questions
- Minimal risk; compile-time checks will catch any missed references.
## Steps
1. Create `SolitaireCardLabels.cs` with extracted short/long rank label methods.
2. Update `SolitaireGame.cs` call sites and remove the in-class rank label methods.
3. Build the solution and fix any compile issues.