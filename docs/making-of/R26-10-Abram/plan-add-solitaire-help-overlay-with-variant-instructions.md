# 🎯 Add Solitaire Help overlay with variant instructions

## Understanding
The user wants a new Help button on the Solitaire screen that opens gameplay instructions for the currently active variant (Klondike or FreeCell). The instructions should be shown as an overlay/modal and support vertical scrolling, with optional simple animation.

## Assumptions
- Help behaves as a modal similar to existing options/prompt overlays and blocks board interactions while open.
- Scroll input can be handled using keyboard and mouse wheel/pointer interactions.
- Minimal animation can be a subtle header pulse/scroll hint without adding heavy new systems.

## Approach
Add a toolbar Help button and a dedicated Help overlay state in `SolitaireGame`, plus variant-specific instruction text constants. Update input routing in `SolitaireInteractionController` to prioritize help modal handling (open/close/scroll) and prevent board interactions while active. Update `SolitaireRenderer` to draw the help overlay panel, clip/scissor instruction content, and render only visible lines according to a scroll offset.

Add internal test hooks and focused tests in `Solitaire.Tests` to validate open/close behavior and variant-specific help content selection.

## Key Files
- src/Games/Solitaire/SolitaireLayout.cs - add Help button bounds and help panel geometry constants/helpers.
- src/Games/Solitaire/SolitaireTextConstants.cs - add Help button label and instruction text lines for Klondike/FreeCell.
- src/Games/Solitaire/SolitaireGame.cs - add help modal state, scrolling state, and internal test helpers.
- src/Games/Solitaire/SolitaireInteractionController.cs - add help button handling and modal input/scroll logic.
- src/Games/Solitaire/SolitaireRenderer.cs - draw Help button and help overlay with scroll rendering.
- src/Solitaire.Tests/SolitaireVariantPromptTests.cs - add tests for help overlay behavior and variant text selection.

## Risks & Open Questions
- Mouse-wheel support in MonoGame can vary by platform; keyboard scrolling fallback should always work.
- Need to keep focus/keyboard navigation stable with existing modal priority.

**Progress**: 16% [█░░░░░░░░░]

**Last Updated**: 2026-10-29 21:24:21

## 📝 Plan Steps
- ✅ **Add Help UI layout and text constants**
- 🔄 **Add help overlay state and test hooks in game state**
-  **Implement help modal input and scroll behavior in interaction controller**
-  **Render Help toolbar button and help overlay content with scrolling**
-  **Add tests for help modal behavior and variant-specific content**
-  **Build and run Solitaire tests**

