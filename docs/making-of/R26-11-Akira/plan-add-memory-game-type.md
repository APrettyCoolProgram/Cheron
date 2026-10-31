
# Add Memory game type

## Understanding
Create a new game module "Memory" with a 4x4 board of 1:1 ratio tiles. Each tile has a back side (solid maroon color, beveled edges) and a front side (solid color, different per matching pair). Players click or use keyboard (arrow keys + Enter) to flip tiles, matching pairs of colors. Add it to the Start Screen cartridge picker alongside Solitaire.

## Assumptions
- Follows existing `IGameModule` pattern (Initialize, SetViewport, LoadContent, Update, Draw, Dispose) as seen in `SolitaireGame`.
- Namespace/folder: `Cheron.Games.Memory`, mirroring `Cheron.Games.Solitaire` structure.
- 4x4 board = 16 tiles = 8 color pairs, shuffled randomly each new game.
- Tile back: maroon solid color with beveled edge (drawn via lighter/darker border rects, consistent with existing beveled rendering patterns in the codebase, e.g., FreeCell/Klondike rendering).
- Tile front: solid distinct color per pair (8 distinct colors).
- Basic memory gameplay: click/keyboard-select a tile to flip it face up; when two are face-up, pause briefly then either keep both face-up (matched) or flip back down (mismatch).
- Window size reuses a similar board layout approach as Solitaire (logical pixel board with viewport scaling via `GameViewport`).
- Added as a new `StartScreenCartridge` entry in `StartScreenGame`, launching `MemoryGame` similar to `SolitaireGame`.

## Approach
Create a self-contained `MemoryGame : IGameModule` class plus supporting types (`MemoryTile`, `MemoryColors`, `MemoryLayout`) under `src/Games/Memory/`. Implement board generation (shuffle 8 colors x2), rendering (beveled tile backs/fronts, 4x4 grid, 1:1 tiles with margins), and input handling (mouse click + keyboard arrow navigation/Enter to flip). Wire into `StartScreenGame` by adding a second cartridge entry and extending `TransitionToInsertedGame` to select between Solitaire/Memory cartridges (refactor cartridge launch to use an enum/kind rather than a single bool flag).

## Key Files
- src/Games/Memory/MemoryGame.cs - main game module implementing IGameModule
- src/Games/Memory/MemoryTile.cs - tile state (color pair id, face-up/matched)
- src/Games/Memory/MemoryColors.cs - color palette constants
- src/Games/Memory/MemoryLayout.cs - board/tile size and bounds constants
- src/Engine/StartScreenGame.cs - add Memory cartridge and launch wiring
- src/Cheron.csproj - ensure new files included (SDK-style glob should auto include)

## Risks & Open Questions
- Need to generalize `StartScreenCartridge.LaunchesSolitaire` bool into a cartridge-kind concept to support a second game without breaking existing Solitaire tests.
- Keyboard navigation focus indicator needs simple visual (e.g., highlight border) consistent with existing style.

## Steps
1. Create `MemoryColors.cs` with back color (maroon) and 8 distinct front colors.
2. Create `MemoryLayout.cs` with board size, tile size, grid spacing constants (4x4, 1:1 tiles).
3. Create `MemoryTile.cs` representing a tile's color id, face-up state, and matched state.
4. Create `MemoryGame.cs` implementing `IGameModule`: deal/shuffle, Initialize, SetViewport, LoadContent, Update (mouse + keyboard input, match-check timer), Draw (beveled tile back/front rendering), Dispose.
5. Update `StartScreenGame.cs` cartridge model to support multiple launchable game kinds (replace `LaunchesSolitaire` bool with a kind enum) without breaking existing behavior.
6. Add a "Memory" cartridge entry to `StartScreenGame` cartridge list and wire `TransitionToInsertedGame` to instantiate `MemoryGame` for it.
7. Build the solution and fix any compilation errors.
8. Run existing Solitaire tests to confirm no regression from the cartridge refactor.
