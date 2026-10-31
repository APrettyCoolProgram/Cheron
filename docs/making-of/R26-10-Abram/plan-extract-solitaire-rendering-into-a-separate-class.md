# Extract Solitaire rendering into a separate class
## Understanding
The user wants rendering extracted from `SolitaireGame` into a separate class. I will do a safe first-stage extraction by moving the main draw pipeline and toolbar rendering methods into a dedicated renderer class while preserving behavior.
## Assumptions
- A nested renderer class in a separate file still satisfies “separate class”.
- Keeping the renderer nested avoids broad access-modifier churn because it can use `SolitaireGame` private members through an owner reference.
- Existing functionality and visuals must remain unchanged.
## Approach
I will convert `SolitaireGame` to a partial class, add a renderer field initialized in a constructor, and delegate `Draw` to the renderer. Then I will move `Draw`, `DrawToolbar`, and `DrawToolbarButton` into `SolitaireRenderer` (new file) using an owner reference (`_game`) so existing helper methods/state remain accessible without widespread refactors.
## Key Files
- src/Games/Solitaire/SolitaireGame.cs - make partial, add renderer field/constructor, delegate draw, remove moved methods.
- src/Games/Solitaire/SolitaireRenderer.cs - new separate renderer class containing draw + toolbar rendering.
## Risks & Open Questions
- Constructor addition must not affect existing module lifecycle assumptions.
- Method moves require precise replacement to avoid duplicate members.
## Steps
1. Update `SolitaireGame.cs` class declaration and fields to support a renderer instance.
2. Create `SolitaireRenderer.cs` with extracted rendering methods.
3. Remove moved rendering methods from `SolitaireGame.cs` and delegate `Draw` to renderer.
4. Build solution and fix any compile issues from extraction.