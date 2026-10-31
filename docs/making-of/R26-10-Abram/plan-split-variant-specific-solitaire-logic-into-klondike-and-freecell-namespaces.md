# Split variant-specific Solitaire logic into Klondike and FreeCell namespaces
## Understanding
You want the existing single Solitaire implementation reorganized so variant-specific logic is housed under `Cheron.Games.Solitaire.Klondike` and `Cheron.Games.Solitaire.FreeCell`, while shared logic remains in `Cheron.Games.Solitaire`.

## Assumptions
- Behavior must remain unchanged; this is structural separation.
- A full type-per-variant rewrite is not required right now; extracting variant logic collaborators is acceptable.
- Existing tests should continue to pass after refactor.

## Approach
Introduce new folders/namespaces with focused helper classes for each variant and move all branch-specific computations/rules/deal/layout decisions there. Then update `SolitaireGame`, `SolitaireRenderer`, and `SolitaireInteractionController` to delegate variant-specific behavior to these helpers instead of embedding FreeCell/Klondike decision code inline.

Common orchestration, shared UI framework, and cross-variant state management remain in `Cheron.Games.Solitaire`.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - replace inline variant-specific branches with helper calls
- src/Games/Solitaire/SolitaireRenderer.cs - route variant tableau spacing/top-row rendering decisions via helper calls
- src/Games/Solitaire/SolitaireInteractionController.cs - route prompt toggle choice helpers if variant-specific behavior is embedded
- src/Games/Solitaire/Klondike/*.cs - new Klondike-specific helpers
- src/Games/Solitaire/FreeCell/*.cs - new FreeCell-specific helpers

## Risks & Open Questions
- Tight coupling to private game state may require small new internal helper methods in `SolitaireGame`.
- Need to avoid moving shared logic accidentally (undo, generic card rendering, common input scaffolding).

## Steps
1. Inventory all variant-specific branches in Solitaire source files
2. Create FreeCell namespace helper classes and move FreeCell-specific rule/deal/layout logic
3. Create Klondike namespace helper classes and move Klondike-specific deal/layout logic
4. Refactor SolitaireGame, Renderer, and InteractionController to consume new variant helpers
5. Build and run Solitaire.Tests to verify no behavior regressions