# Continue Solitaire modularization with keyboard-description text extraction
## Understanding
The user asked to continue refactoring. A safe next step is extracting remaining keyboard-description string literals from `GetKeyboardDescription()` into `SolitaireTextConstants`.
## Assumptions
- Existing displayed text must remain exactly unchanged.
- `SolitaireGame.cs` already statically imports `SolitaireTextConstants`, so updates are low risk.
## Approach
Add constants for static keyboard-focus labels and unavailability/status messages to `SolitaireTextConstants.cs`. Replace matching string literals in `GetKeyboardDescription()` with those constants while preserving interpolated strings where numeric values are embedded.
## Key Files
- src/Games/Solitaire/SolitaireTextConstants.cs - add keyboard-description constants.
- src/Games/Solitaire/SolitaireGame.cs - replace literals in `GetKeyboardDescription()`.
## Risks & Open Questions
- Minimal risk of missed literal replacement; build will validate.
## Steps
1. Add keyboard-description constants to `SolitaireTextConstants.cs`.
2. Replace matching `GetKeyboardDescription()` literals in `SolitaireGame.cs` with constants.
3. Build the solution and resolve any compile issues.