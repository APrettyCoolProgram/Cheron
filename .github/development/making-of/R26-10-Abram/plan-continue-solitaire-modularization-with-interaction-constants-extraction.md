# Continue Solitaire modularization with interaction constants extraction
## Understanding
The user asked to continue the next extraction pass. A low-risk next step is extracting pointer/hint interaction constants from `SolitaireGame.cs` into a dedicated constants file, preserving behavior.
## Assumptions
- Existing constant values must remain unchanged.
- `SolitaireGame.cs` can statically import another constants class as done for layout/text/timing.
## Approach
Create `SolitaireInteractionConstants.cs` for drag threshold, double-click interval, and hint display duration. Update `SolitaireGame.cs` to use these constants and remove local declarations/literals.
## Key Files
- src/Games/Solitaire/SolitaireInteractionConstants.cs - new shared interaction constants.
- src/Games/Solitaire/SolitaireGame.cs - import and consume shared constants.
## Risks & Open Questions
- Minimal risk; compile catches missed references.
## Steps
1. Create `SolitaireInteractionConstants.cs` with extracted interaction constants.
2. Update `SolitaireGame.cs` to use extracted constants and remove duplicated declarations/literals.
3. Build the solution and resolve any compile issues.