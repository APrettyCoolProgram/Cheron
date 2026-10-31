# Continue Solitaire modularization with accessibility layout extraction
## Understanding
The user asked to continue refactoring. A safe next step is extracting accessibility/options panel layout geometry from `SolitaireGame.cs` into a dedicated layout helper to reduce magic numbers and class size without changing behavior.
## Assumptions
- Existing coordinates and sizes must remain exactly unchanged.
- Keeping helper members static and internal in same namespace preserves usage simplicity.
## Approach
Create `SolitaireOptionsLayout.cs` with constants for panel frame and option row bounds, plus `GetOptionBounds(int)` logic. Update `SolitaireGame.cs` to statically import this helper and replace inline option bounds/panel literals with named constants. Build to verify no regressions.
## Key Files
- src/Games/Solitaire/SolitaireOptionsLayout.cs - extracted accessibility panel geometry.
- src/Games/Solitaire/SolitaireGame.cs - consume extracted option layout members and remove duplicated method.
## Risks & Open Questions
- Minimal; risk is missed replacement causing compile errors.
## Steps
1. Create `SolitaireOptionsLayout.cs` with option/panel layout constants and `GetOptionBounds` helper.
2. Update `SolitaireGame.cs` to use `SolitaireOptionsLayout` members and remove the in-class `GetOptionBounds` method.
3. Build the solution and resolve any compile issues.