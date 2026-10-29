# Continue Solitaire modularization with UI text constants extraction
## Understanding
The user asked to continue refactoring. A safe next step is extracting toolbar and hint text constants from `SolitaireGame.cs` into a dedicated constants type to reduce class size without behavior changes.
## Assumptions
- Text values should remain exactly unchanged.
- Static import in `SolitaireGame.cs` is acceptable and consistent with recent layout extraction.
## Approach
Create a new static class to store toolbar labels and hint messages, remove the duplicate constants from `SolitaireGame.cs`, and add a static using so existing usages keep compiling with minimal edits.
## Key Files
- src/Games/Solitaire/SolitaireTextConstants.cs - new extracted UI text constants.
- src/Games/Solitaire/SolitaireGame.cs - remove inline constants and import shared constants.
## Risks & Open Questions
- Compile errors if any constant reference is missed; validated by full build.
## Steps
1. Create `SolitaireTextConstants.cs` with extracted toolbar and hint text constants.
2. Update `SolitaireGame.cs` to use the extracted constants and remove in-class constant declarations.
3. Build the solution and resolve any compile issues.