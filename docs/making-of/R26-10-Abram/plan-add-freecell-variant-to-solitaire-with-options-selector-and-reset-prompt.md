# Add FreeCell Variant to Solitaire with Options Selector and Reset Prompt
## Understanding
The user wants to keep Klondike and add FreeCell as a playable variant. Variant selection must be done via the existing options panel, and switching variants should prompt before resetting the current deal.

## Assumptions
- Reuse the current Solitaire module and extend it to support two rule/deal modes.
- Keep existing Klondike behavior unchanged by default.
- Implement a lightweight in-panel confirmation prompt for variant changes rather than a modal OS dialog.
- Preserve architecture direction by extending extracted interaction/renderer controllers as needed.

## Approach
Introduce a `SolitaireVariant` concept and current-variant state in `SolitaireGame`. Extend deal setup, move validation, and rendering labels/hints to branch by variant while sharing card rendering and interaction infrastructure. Add FreeCell structures (cascades, free cells, foundations) and rule helpers in dedicated files.

Extend options panel metadata/layout to include a variant selector row and a reset confirmation flow: when user selects a different variant, stage a pending variant and show confirm/cancel controls in the options panel; applying confirm starts a new game in that variant, cancel keeps current state.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - core game state/rules branching and new variant state
- src/Games/Solitaire/SolitaireRenderer.cs - draw FreeCell board zones and options-panel prompt text/buttons
- src/Games/Solitaire/SolitaireInteractionController.cs - options selector interaction and confirmation handling
- src/Games/Solitaire/SolitaireAccessibilityOptions.cs - add variant option labels/count updates
- src/Games/Solitaire/SolitaireTextConstants.cs - variant and confirmation UI strings
- src/Games/Solitaire/SolitaireLayout.cs - FreeCell geometry constants/helpers
- src/Games/Solitaire/SolitaireMoveRules.cs - FreeCell move rules helpers
- src/Games/Solitaire/SolitaireDealBuilder.cs - FreeCell deal helper
- src/Games/Solitaire/SolitaireVariant.cs - new enum for variants

## Risks & Open Questions
- FreeCell drag/multi-card movement rules can be complex; implement canonical movable-run constraints carefully.
- Existing undo/snapshot/history format may require adaptation for FreeCell pile topology.
- Options panel row indexing changes can affect keyboard/pointer option navigation.

## Steps
1. Add variant model and state
2. Implement FreeCell deal and board structures
3. Extend move-rule helpers for FreeCell
4. Branch game move/select logic by variant
5. Add options-panel variant selector and pending-switch state
6. Implement confirm/cancel prompt flow for variant switching
7. Update renderer for FreeCell board and prompt UI text
8. Update accessibility labels/constants/layout for new option row
9. Build and fix compile errors
10. Validate core gameplay flows for both variants