# Solitaire decomposition refactor
## Understanding
Implement the recommendation to break the large partial-based `SolitaireGame` into smaller units by extracting cohesive logic into smaller classes/methods while preserving gameplay behavior.

## Assumptions
- No functional behavior changes are desired.
- The safest first extraction is logic that is mostly pure and well-contained.
- Existing partial structure remains, but responsibilities are reduced inside core files.

## Approach
I will extract two focused nested helper classes inside `SolitaireGame` to improve separation of concerns without changing public surface: a `DealBuilder` for deck creation/shuffle/deal setup and a `MoveRules` class for tableau/foundation legality checks. Then I will simplify `StartNewGame` and move-check methods to delegate to those helpers. Finally, I will build to verify no regressions.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - start-new-game flow and nested helper classes.
- src/Games/Solitaire/SolitaireGame.Interaction.cs - move legality methods delegated to `MoveRules`.

## Risks & Open Questions
- Nested helper classes still live in the same type; this is an incremental decomposition, not full standalone service extraction.
- Any subtle dealing/order change would impact gameplay, so exact logic parity is required.

## Steps
1. Review targeted methods and capture exact extraction boundaries.
2. Extract deal setup into a focused helper class and simplify `StartNewGame`.
3. Extract move legality checks into a focused helper class and delegate from interaction methods.
4. Build and resolve any compile issues.
5. Summarize the decomposition changes and next extraction candidates.