# 🎯 Center the Memory board and add selectable board size (up to 12x12)

## Understanding
Two related changes to the Memory game: (1) visually center the tile grid within the play area (currently the grid sits closer to the toolbar than the footer because `GridTopPadding` (24) is much smaller than `BoardMargin` (60)), and (2) let the player choose a square board size (2x2 up to 12x12, even sizes only so tiles always pair evenly) via a "New Game" picker prompt, similar in spirit to Solitaire's variant picker.

## Assumptions
- Only square boards are offered (NxN), with N in {2,4,6,8,10,12}, since odd NxN boards can't be evenly paired.
- "New Game" (button, N shortcut) now opens a picker prompt (pre-selecting the current size) instead of instantly redealing, mirroring `SolitaireGame`'s `OpenNewGamePrompt`/`ConfirmNewGamePromptSelection` pattern.
- The board's logical pixel size (`MemoryLayout.BoardWidth/BoardHeight`) legitimately changes with board size; `GameViewport`/`GameHost` already re-letterbox each frame based on `IGameModule.PreferredWindowSize`, so no engine/host changes are required for resizing to work.
- Since `MemoryColors.TileFronts` only defines 8 colors but a 12x12 board needs 72 distinct pair colors, color generation must be extended procedurally (e.g., evenly spaced hues) while preserving the existing 8 colors for the default/small boards.
- Artwork (`MemoryArtwork`) must be rebuilt (disposed and recreated) whenever the board size changes, since tile counts, board dimensions, and button positions all depend on it. `MemoryGame` will retain the `GraphicsDevice` reference from `LoadContent` to support rebuilding.
- Centering fix: replace the separate `GridTopPadding` constant with `BoardMargin`, so the grid has equal top/bottom/left/right margins within the toolbar/footer frame.

## Approach
Convert `MemoryLayout`'s `Columns`/`Rows` (and derived `BoardWidth`/`BoardHeight`/`GridHeight`) from compile-time `const` to parameters threaded through its static methods, since the grid size is now a per-game runtime value instead of fixed. `MemoryGame` keeps `_columns`/`_rows` fields (default 4x4) and passes them into `MemoryLayout` calls and `PreferredWindowSize`. A new `MemorySizeConstants`/inline array defines the selectable sizes. `MemoryColors` gains a method to produce N pair colors, reusing the 8 curated colors first and generating additional hues procedurally beyond that. The New Game flow becomes a modal prompt (`_sizePromptOpen`, `_sizePromptIndex`) with keyboard (arrows + Enter/Escape) and mouse (click a size tile, Confirm/Cancel buttons) interaction, drawn via a new `DrawSizePrompt` overlay reusing the existing overlay panel/banner styling. Confirming rebuilds artwork for the new size and deals a fresh board.

## Key Files
- [MemoryLayout.cs](src/Games/Memory/MemoryLayout.cs) - convert Columns/Rows to parameters; fix centering by equalizing grid padding to BoardMargin; add size-prompt panel/option bounds.
- [MemoryGame.cs](src/Games/Memory/MemoryGame.cs) - add _columns/_rows state, size prompt state/handlers, artwork rebuild-on-resize, update all MemoryLayout call sites, update toolbar/footer/help text drawing for new flow.
- [MemoryColors.cs](src/Games/Memory/MemoryColors.cs) - add procedural color generation for board sizes beyond 8 pairs.
- [MemoryTextConstants.cs](src/Games/Memory/MemoryTextConstants.cs) - add size-prompt labels/help text updates.
- [MemoryArtwork.cs](src/Games/Memory/MemoryArtwork.cs) - no structural change expected (already parameterized by board dims/colors), verify it tolerates size changes and re-creation.
- Test files under src/Solitaire.Tests referencing MemoryLayout/MemoryGame - update to match new signatures and add size-prompt coverage.

## Risks & Open Questions
- Large boards (12x12 = 144 tiles) must remain readable; tile size/spacing stay fixed, so BoardWidth/BoardHeight will grow substantially for big boards - this is expected/acceptable since the viewport letterboxes/scales to fit the window.
- Rebuilding artwork at runtime requires retaining the GraphicsDevice; must ensure proper disposal ordering to avoid leaks.
- Many internal `Test*` hooks and existing tests reference fixed 4x4 assumptions (e.g., `TestTileCount == 16`); these need updating without breaking coverage intent.

**Progress**: 100% [██████████]

**Last Updated**: 2026-10-30 22:59:02

## 📝 Plan Steps
- ✅ **Update MemoryLayout.cs - parameterize Columns/Rows across all geometry methods and equalize grid padding with BoardMargin for centering.**
- ✅ **Add size-prompt panel/option bounds to MemoryLayout.cs for the new board-size picker UI.**
- ✅ **Extend MemoryColors.cs with a method generating N pair colors procedurally beyond the curated 8.**
- ✅ **Update MemoryTextConstants.cs with new size-prompt title/labels and revised help/footer text reflecting the new New Game flow.**
- ✅ **Update MemoryGame.cs state - add _columns/_rows fields, store GraphicsDevice, add size-prompt state fields.**
- ✅ **Update MemoryGame.cs - rework StartNewGame/toolbar New Game handling/N shortcut to open the size prompt instead of dealing immediately.**
- ✅ **Implement size prompt interaction (keyboard navigation, mouse clicks, confirm/cancel) and artwork rebuild-on-confirm in MemoryGame.cs.**
- ✅ **Update MemoryGame.cs Draw method to render the size prompt overlay and adjust all MemoryLayout call sites for the new signatures.**
- ✅ **Update Test*-prefixed internal hooks in MemoryGame.cs to expose size-prompt state for tests.**
- ✅ **Update existing Memory tests (MemoryLayoutTests.cs, MemoryGameTests.cs) for new method signatures and board-size behavior; add new tests for centering and size selection.**
- ✅ **Build the solution and run the full Solitaire.Tests suite, fixing any regressions.**

