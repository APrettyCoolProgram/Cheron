# Continue Solitaire modularization with bitmap font extraction
## Understanding
The user asked to continue the refactor. A safe next extraction is to move Solitaire's bitmap font data and glyph texture/text rendering helpers out of `SolitaireGame.cs` into a dedicated class, while keeping behavior unchanged.
## Assumptions
- Keeping method signatures behavior-compatible preserves all existing rendering output.
- `SolitaireGame` can keep its `DrawText` method as a thin wrapper to avoid widespread call-site edits.
- A new helper under the same namespace will be automatically included by SDK-style project defaults.
## Approach
Create `SolitaireBitmapFont.cs` to own glyph data, glyph-texture creation, and text drawing logic. Then update `SolitaireGame.cs` to call into this helper in `LoadContent`, remove inline glyph data/texture creation code, and keep a thin `DrawText` wrapper that delegates to the new class.
## Key Files
- src/Games/Solitaire/SolitaireBitmapFont.cs - new bitmap font helper.
- src/Games/Solitaire/SolitaireGame.cs - remove duplicated bitmap-font implementation and delegate.
## Risks & Open Questions
- Any subtle color/high-contrast behavior changes would be visible immediately; build will validate compile but not pixel output.
## Steps
1. Create `SolitaireBitmapFont` with glyph atlas data, glyph texture creation, and text drawing logic.
2. Update `SolitaireGame.cs` to use `SolitaireBitmapFont` and remove the in-class glyph/font implementation details.
3. Build the solution and fix any compile issues from the extraction.