# Continue Solitaire modularization with undo snapshot extraction
## Understanding
The user asked to continue the refactor. A safe incremental step is extracting the undo snapshot record types from `SolitaireGame.cs` into a dedicated file while preserving all behavior.
## Assumptions
- Keeping the extracted types `internal` in the same namespace preserves access and semantics.
- `SolitaireGame` can keep existing usage without further logic changes.
## Approach
Create a new file containing `SavedCard` and `BoardSnapshot`, then remove their in-class declarations from `SolitaireGame.cs`. Since these are pure data carriers, this extraction is low-risk and reduces class size.
## Key Files
- src/Games/Solitaire/SolitaireUndoSnapshot.cs - new extracted undo snapshot types.
- src/Games/Solitaire/SolitaireGame.cs - remove nested snapshot type declarations.
## Risks & Open Questions
- None expected beyond compile-time reference visibility, validated by build.
## Steps
1. Create `SolitaireUndoSnapshot.cs` with `SavedCard` and `BoardSnapshot` definitions.
2. Remove nested `SavedCard` and `BoardSnapshot` declarations from `SolitaireGame.cs`.
3. Build the solution and fix any compile issues.