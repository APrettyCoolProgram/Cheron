# Continue Solitaire modularization with remaining UI label extraction
## Understanding
The user asked to continue the extraction pass. A safe next step is centralizing remaining hard-coded board and toolbar labels in `SolitaireGame.cs` into `SolitaireTextConstants`.
## Assumptions
- Displayed text values must remain exactly unchanged.
- `SolitaireGame.cs` already statically imports `SolitaireTextConstants`, so call-site impact is minimal.
## Approach
Add constants for board labels (deal, again, stock, waste, foundations, A/K hints), toolbar headings, and toolbar shortcut legend text to `SolitaireTextConstants`. Then replace matching inline string literals in `SolitaireGame.cs` draw paths.
## Key Files
- src/Games/Solitaire/SolitaireTextConstants.cs - add additional UI label constants.
- src/Games/Solitaire/SolitaireGame.cs - replace label literals with constants.
## Risks & Open Questions
- Very low risk; build validation will catch missed references.
## Steps
1. Add board and toolbar label constants to `SolitaireTextConstants.cs`.
2. Replace corresponding draw-string literals in `SolitaireGame.cs` with constants.
3. Build the solution and resolve any compile issues.