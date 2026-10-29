# Continue Solitaire modularization with timing constants extraction
## Understanding
The user asked to continue refactoring. A safe next step is extracting feedback/animation timing constants from `SolitaireGame.cs` into a dedicated constants class without changing behavior.
## Assumptions
- Existing timing values must remain exactly unchanged.
- Static import pattern used for layout/text constants is acceptable for timing constants too.
## Approach
Create `SolitaireTimingConstants.cs` to hold the invalid-feedback duration, win celebration duration, and move duration constants. Update `SolitaireGame.cs` to import and use them, then remove duplicated inline declarations.
## Key Files
- src/Games/Solitaire/SolitaireTimingConstants.cs - new shared timing constants.
- src/Games/Solitaire/SolitaireGame.cs - remove in-class timing constants and import shared ones.
## Risks & Open Questions
- Minimal risk; compile catches missed references.
## Steps
1. Create `SolitaireTimingConstants.cs` with extracted timing constants.
2. Update `SolitaireGame.cs` to use the extracted timing constants and remove duplicated declarations.
3. Build the solution and resolve any compile issues.