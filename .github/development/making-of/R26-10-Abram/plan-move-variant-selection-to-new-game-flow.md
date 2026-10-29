# 🎯 Move Variant Selection to New Game Flow

## Understanding
The user wants variant selection to happen when starting a new game, instead of choosing variant in the options panel. Starting a new game should prompt variant choice and then deal that variant.

## Assumptions
- We can reuse existing pending-variant confirmation UI pattern.
- Options panel should no longer contain variant row or variant-switch prompt.
- New game triggers should be intercepted in interaction flow and show a variant picker prompt.

## Approach
Add a dedicated pending state for "new game variant choice" and handle it in keyboard/pointer interaction paths that currently call `StartNewGame()`. Keep existing `StartNewGame()` dealing logic by setting `_variant` before calling it. Remove variant selector from options metadata/labels and update renderer to draw variant choice prompt (likely overlay) when pending.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - add/remove state and keep start flow.
- src/Games/Solitaire/SolitaireInteractionController.cs - route New Game actions to variant prompt and handle confirm choice.
- src/Games/Solitaire/SolitaireRenderer.cs - render variant picker prompt.
- src/Games/Solitaire/SolitaireAccessibilityOptions.cs - revert option row count/labels.
- src/Games/Solitaire/SolitaireTextConstants.cs - replace options-variant prompt strings with new-game picker strings.

## Risks & Open Questions
- Need to preserve existing keyboard shortcuts behavior while prompt is active.
- Must ensure prompt is reachable from both toolbar click and R key.

**Progress**: 16% [█░░░░░░░░░]

**Last Updated**: 2026-10-29 19:05:35

## 📝 Plan Steps
- 🔄 **Add new-game variant prompt state in SolitaireGame**
- ✅ **Route New Game actions to variant prompt in interaction controller**
-  **Implement prompt input handling for variant selection/confirm/cancel**
-  **Remove variant selector from options metadata and options rendering flow**
-  **Add/update renderer and text constants for new-game variant picker**
-  **Build and fix compile errors**

