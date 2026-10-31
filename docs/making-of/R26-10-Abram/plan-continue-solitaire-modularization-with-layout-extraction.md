# Continue Solitaire modularization with layout extraction
## Understanding
The user asked to continue the modular refactor of `SolitaireGame.cs`. The next low-risk step is extracting shared board layout constants/bounds into a dedicated layout type while keeping behavior unchanged.
## Assumptions
- Keeping names identical and importing them statically will minimize call-site churn.
- A standalone layout class in the same namespace is acceptable for project conventions.
- No external code depends on these members being private fields on `SolitaireGame`.
## Approach
Create `SolitaireLayout.cs` with board/card size constants, toolbar button bounds, and foundation-bounds calculation currently embedded in `SolitaireGame`. Then remove those members from `SolitaireGame` and add a static import so existing references continue to compile with minimal edits.

Run a full build to verify behavior-preserving extraction.
## Key Files
- src/Games/Solitaire/SolitaireLayout.cs - new centralized layout constants and bounds helpers.
- src/Games/Solitaire/SolitaireGame.cs - remove inline layout members and consume shared layout definitions.
## Risks & Open Questions
- If any removed private member names collide or are shadowed, compile fixes may be required.
## Steps
1. Create `SolitaireLayout` with extracted constants, button bounds, and foundation bounds helper.
2. Update `SolitaireGame.cs` to consume `SolitaireLayout` members and remove duplicated layout definitions.
3. Build the solution and resolve any compile issues from the extraction.