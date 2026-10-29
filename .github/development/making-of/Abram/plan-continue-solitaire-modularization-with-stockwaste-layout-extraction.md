# Continue Solitaire modularization with stock/waste layout extraction
## Understanding
The user asked to continue the next extraction pass. A safe incremental refactor is centralizing stock/waste pile geometry that is still duplicated in `SolitaireGame.cs`.
## Assumptions
- Existing coordinates and dimensions must remain unchanged.
- `SolitaireGame.cs` already statically imports `SolitaireLayout`, so replacing literals is low-risk.
## Approach
Extend `SolitaireLayout` with named stock/waste X/Y constants and helper methods for stock/waste bounds. Replace repeated `new Rectangle(32, 82, CardWidth, CardHeight)` and `new Rectangle(128, 82, CardWidth, CardHeight)` occurrences in `SolitaireGame.cs` with the helper methods.
## Key Files
- src/Games/Solitaire/SolitaireLayout.cs - add stock/waste coordinates and bounds helpers.
- src/Games/Solitaire/SolitaireGame.cs - replace duplicated stock/waste rectangle literals.
## Risks & Open Questions
- Minimal risk; compile will catch missed replacements.
## Steps
1. Extend `SolitaireLayout.cs` with stock/waste coordinate constants and bounds helper methods.
2. Update `SolitaireGame.cs` to use `GetStockBounds()` and `GetWasteBounds()` instead of duplicated rectangle literals.
3. Build the solution and resolve any compile issues.