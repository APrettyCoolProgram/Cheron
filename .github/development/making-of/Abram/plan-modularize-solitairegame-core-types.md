# Modularize SolitaireGame core types
## Understanding
The user wants to start breaking up `SolitaireGame.cs` into smaller, more modular units. A safe first pass is to extract self-contained nested types (card model, suit enum, move rules, and deal builder) into dedicated files without changing behavior.
## Assumptions
- `SolitaireGame` is the only current consumer of these nested types.
- Keeping extracted types `internal` in the same namespace preserves accessibility and behavior.
- No API surface outside the assembly depends on nested type names.
## Approach
I will extract the nested `Suit`, `Card`, `MoveRules`, and `DealBuilder` definitions from `SolitaireGame` into separate files under `src/Games/Solitaire/`. Then I will remove their nested definitions from `SolitaireGame.cs` and update references only if required by compiler diagnostics. This keeps changes minimal while reducing file size and separating responsibilities.

After edits, I will run a full build to verify no regressions.
## Key Files
- src/Games/Solitaire/SolitaireGame.cs - remove nested helper/model definitions.
- src/Games/Solitaire/Suit.cs - new standalone suit enum.
- src/Games/Solitaire/Card.cs - new standalone card model.
- src/Games/Solitaire/MoveRules.cs - new standalone move validation helper.
- src/Games/Solitaire/DealBuilder.cs - new standalone deck/deal helper.
## Risks & Open Questions
- If other files rely on nested type qualification, extraction could require reference updates.
- File-local style and XML-doc conventions must match existing project patterns.
## Steps
1. Extract `Suit` into a new file.
2. Extract `Card` into a new file.
3. Extract `MoveRules` into a new file.
4. Extract `DealBuilder` into a new file.
5. Remove the extracted nested definitions from `SolitaireGame.cs`.
6. Build the solution and fix any compile issues caused by extraction.