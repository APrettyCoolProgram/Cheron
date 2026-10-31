# Extract Solitaire Input/Interaction Class
## Understanding
The user wants Solitaire input and interaction responsibilities split out of `SolitaireGame` into a dedicated class, similar to the renderer extraction, while keeping behavior unchanged.

## Assumptions
- We should keep the existing incremental/refactor-safe style by using a nested helper class that references the owning `SolitaireGame`.
- We should only move input/interaction orchestration methods first, not gameplay rules/state mutations beyond what those methods already call.
- `SolitaireRenderer` will continue to call keyboard-focus description/bounds helpers, so any moved methods used there need compatibility wrappers.

## Approach
Create a nested `SolitaireInteractionController` partial file under `src/Games/Solitaire` and move pointer/keyboard/options interaction methods into it. The controller will access game state through `_game` and expose public/internal methods consumed by `Update(...)` and viewport pointer mapping. In `SolitaireGame.cs`, replace moved method bodies with delegations and then remove obsolete methods/fields where safe.

We will preserve method signatures used by renderer (`GetKeyboardBounds`, `GetKeyboardDescription`) by forwarding these calls to the controller to minimize churn. After migration, run a full build and address any access/modifier gaps.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - current owner of input/interaction fields and methods; update call sites and remove moved logic.
- src/Games/Solitaire/SolitaireRenderer.cs - depends on keyboard bounds/description methods; may need call-site compatibility.
- src/Games/Solitaire/SolitaireInteractionController.cs - new extracted class containing interaction logic.

## Risks & Open Questions
- Access level mismatches are likely because moved methods currently call many private members.
- If too many helpers are moved at once, compile fallout increases; keep small, behavior-preserving delegation paths.

## Steps
1. Add interaction controller scaffold
2. Move pointer interaction methods
3. Move accessibility/options input methods
4. Move keyboard navigation methods
5. Wire game update and compatibility delegations
6. Build and fix compilation issues