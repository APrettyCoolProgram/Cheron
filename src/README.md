# Cheron Solitaire

A simple Klondike solitaire game built with MonoGame. It uses no external game assets or font packages.

## Appearance

Solitaire uses a shaded teal table, ivory rounded cards, soft shadows, geometric suit symbols,
patterned card backs, gold selection outlines, and a hover-highlighted new-game button.
Artwork is generated once during content loading and disposed with the game module.
The compact bitmap lettering remains asset-free. Cards slide and flip over 220 milliseconds, with a short
staggered deal and a 120-millisecond selection fade. Invalid drags glide back to their source.
Animations use elapsed time rather than frame counts and do not change game rules or card ownership.
Board input briefly pauses during transitions; **New Game**, sound toggling, **Options**, **R**, and **Escape** remain available.
All visual assets and rendering remain inside `Games\Solitaire`.

## Accessibility

Click **Options F1** or press **F1** to open or close accessibility settings. Use the mouse, or **Tab/Shift+Tab**
and **Up/Down** to choose a row, then **Enter/Space** to toggle it. **F2**, **F3**, and **F4** also toggle the
corresponding settings directly, including during animations. Options consumes board clicks and shortcuts while open;
the timer and existing transitions continue. Opening it cancels any held pointer gesture without moving cards.
**Escape** retains its existing meaning of exiting the application, not dismissing settings.

- **F2 - Larger text:** doubles small interface labels from 7 to 14 logical units high and enlarges card ranks
  from 14 to 21. Button labels remain at their already larger 14-unit size. Statistics and footer text have dedicated
  placements to avoid overlap; the compact shortcut line becomes an Options reminder. Window scaling still applies,
  so maximizing the window can help further. This is not a zoom or reflow mode.
- **F3 - High contrast:** uses solid black board/slot surfaces, white interface text, white card faces, black or
  dark-red rank/suit ink, and a yellow selection outline. The solid text/background color pairs exceed 4.5:1 contrast
  before antialiasing. Suit silhouettes and written rank/suit descriptions avoid relying solely on color.
- **F4 - Reduced motion:** immediately settles active transitions and skips subsequent dealing, slides, flips,
  return animations, selection fades, invalid-outline fading, and win particles. Logical moves, undo, static
  highlights, instructions, the win banner, and optional sounds still work. Turning motion back on does not replay
  completed effects. Sound can be disabled separately with **M**.

Settings default to off, survive New Game, and last until the application closes; nothing is written to disk.
These are visual and input accommodations, not screen-reader integration or OS accessibility-setting detection.
There is no spoken card narration or native accessibility tree in this pass.

### Keyboard play

- **Tab / Shift+Tab:** cycle New Game, Undo, Hint, Sound, Options, stock, waste, four foundations, and seven tableau columns.
- **Left / Right:** cycle the board piles, wrapping at either end. From a toolbar control, either arrow enters the stock.
- **Up / Down:** choose a face-up card within the focused tableau column; hidden cards cannot be selected.
  Entering a column focuses its top card; move upward to select a longer stack.
- **Enter / Space:** activate a control or stock, select the focused card and cards above it, or move an existing
  selection to the focused legal tableau/foundation. Activating the same selected card again clears selection.
  An invalid destination keeps the source selected and reports the rejection; focusing waste allows a new source selection.
- **F:** send the current selection, or the focused card when nothing is selected, to its first legal foundation.
- **Backspace:** clear selection and hint feedback. **R**, **U**, **H**, and **M** retain their existing shortcuts.

For example, focus a tableau column, use Up to choose a stack, press Enter, navigate to its destination, and press Enter again.
White corner marks show keyboard focus, separate from the continuous selected-card and legal-destination outlines.
The footer names the focused pile, card rank/suit, and selected-card count. It also labels legal/invalid destinations
for the current selection in words. Empty/disabled targets are described explicitly.
Disabled Undo/Hint controls remain in the traversal order but cannot execute unavailable actions.
Board activation is blocked during card transitions; Tab traversal, New Game, Sound, and Options remain available.
A real mouse press hides keyboard focus; merely resting or moving the mouse does not disrupt keyboard play.

## Window layout

Resize or maximize the window to scale the entire 1100-by-800 logical card table proportionally.
Cards, controls, text, highlights, animations, and celebrations retain their relative positions and aspect ratio.
Wide or tall windows use centered dark margins rather than stretching or cropping the board.
The toolbar and tableau do not reflow; very small windows therefore make the whole game smaller.

The engine's `GameViewport` measures both the client area and drawable back buffer, allowing different DPI scales.
Solitaire draws directly into the drawable surface with a shared transform, not into a low-resolution intermediate image.
Existing card textures and bitmap lettering are retained; this does not add vector artwork or scalable fonts.
The original glyphs are cached in a padded texture atlas so fractional scaling filters thin strokes rather than dropping them.
Pointer coordinates are mapped through the same viewport before hit testing, so controls, dragging, and double-clicks
stay aligned at different window sizes and DPI scales. Margin clicks do not activate the board, and dragged cards
are clipped out of the margins. Gesture distances use logical board units and scale with the board.
Resizing cancels a held pointer gesture and returns any dragged stack without moving cards; a fresh press is required.
Ordinary selections, hints, history, statistics, and active deal/move animations survive resizing.

## Feedback

Quiet synthesized tones acknowledge draws, recycling, moves, and undo; foundation advances use a rising pair of tones.
Rejected destination clicks, double-clicks, and drops produce a soft low tone, a fading coral destination outline
when over a pile, and brief footer instructions. Invalid drags still glide back without changing the board.
Gold selection outlines, mint legal-target outlines, and pale hover outlines remain available with sound off.
A three-second gold-and-mint burst and short chime celebrate a win after the final card settles.
The celebration never blocks Undo or New Game; either action cancels it and any playing cue.

Click **SND/OFF** or press **M** to toggle sound, including during animations. Muting stops active sounds immediately.
The preference lasts for this application session and survives New Game; it is not saved to disk.
If MonoGame reports missing audio hardware, the button shows **NA**, the footer reports **SOUND UNAVAILABLE**,
and a diagnostic warning records the failure. Gameplay and visual feedback continue normally.
Sounds are generated once during content loading and disposed with the module; no audio assets or packages are added.

## Organization

- `Engine`: game-neutral host and `IGameModule` lifecycle contract. Engine code must not depend on game namespaces.
- `Games\Solitaire`: Solitaire rules, state, interaction, and rendering. Other games belong in sibling folders and namespaces.
- `Program.cs`: application composition; selects the module passed to `GameHost`.

These are folder/namespace boundaries within the existing project, not separate assemblies yet.
The host owns the MonoGame window, graphics-device lifecycle, global exit controls, and active module.
It initializes the module, loads its resources once a graphics device is available, and forwards update/draw calls.
It enables native window resizing and sends `SetViewport` notifications before using changed client or drawable dimensions.
Zero-sized minimized surfaces are skipped until usable dimensions return.
On shutdown it disposes the module before MonoGame disposes the graphics device.
The module borrows the graphics device and owns/disposes the textures, sprite batches, sounds, and playback instances it creates.

To add a game, implement `IGameModule` under its own `Games` folder, supply its title and logical canvas size through
`PreferredWindowSize`, handle `SetViewport` for drawing and input mapping, and select it in `Program.cs`.
The preferred size also requests the initial window dimensions. A launcher or dynamic plugin system is not required.
Keep game-specific rules and drawing in that module; put genuinely shared helpers in `Engine` as needs emerge.
The existing Solitaire drawing helpers remain local until another game demonstrates a shared requirement.

## Run

From this directory, run:

```powershell
dotnet run --project Cheron.csproj
```

## Controls

- Click **New Game** or press **R** to deal a shuffled game.
- Click **Undo** or press **U** to reverse the last successful action, including any automatic card flip.
- Click **Hint** or press **H** for four seconds of gold source/destination highlights and footer instructions.
- Click **SND/OFF** or press **M** to toggle sound; **NA** indicates unavailable audio hardware.
- Click **Options F1** or press **F1** for accessibility settings; **F2/F3/F4** toggle text, contrast, and motion.
- Use **Tab**, arrows, and **Enter/Space** for keyboard play; see the keyboard instructions above.
- Undo and Hint are disabled during animations and dragging; Undo is also disabled when history is empty.
- Click the stock to draw one card to the waste. Click the empty stock to recycle the waste.
- Click a face-up card to select it; click a valid tableau pile or foundation to move it.
- Alternatively, drag a face-up card or tableau stack and release over a valid pile. Invalid drops leave cards in place.
- A drag begins after six logical board units of movement; a shorter press/release is a click.
- Double-click the same card within 350 milliseconds to send an eligible top card to a foundation.
- Pale outlines mark the hovered card; mint outlines identify legal destinations for the selection or dragged stack.
- Tableau stacks build downward in alternating colors, with Kings on empty columns.
- Foundations build upward by suit, starting with Aces.
- Press **Escape** to exit.

## Statistics and hints

Each successful card or stack move, stock draw, or waste recycle counts as one move and can be undone.
Invalid moves, selections, and hints do not count. Undo restores the previous move count and exact pile order
and face states; there is no redo. New Game clears the undo history and resets both statistics.
The timer starts with the first successful action, continues during animations and undo, and stops on a win.
Undoing a win resumes it without rewinding elapsed time. The display uses minutes and seconds.

Hints prefer foundation moves, then tableau moves exposing hidden cards, then other tableau moves.
When no such move is found, the hint suggests drawing or recycling if available, or reports no suggested move.
Hints never execute a move or alter the current selection. They are one-step suggestions, not a solver or
proof that the deal can be won; they do not recommend foundation reversals or whole-column King shuffles.

## Interaction verification

- Select a visible strip in a face-up tableau stack; confirm the selected card and cards above it move together.
- Drag to a mint-highlighted destination; check that exposed face-down cards flip and no card is duplicated.
- Drop on the wrong color, stock, background, or footer; confirm the original piles are unchanged.
- Click a card and then a destination without dragging; check that click-to-move still works.
- Double-click an Ace, then its same-suit Two when available; confirm only eligible top cards enter foundations.
- Restart while dragging; confirm the new deal has 52 cards and releasing the button does not move any of them.
- Start a new game; confirm all seven columns are dealt in under one second without duplicate cards.
- Draw and recycle the stock; confirm cards glide between piles and flip to the correct side.
- Try clicking or holding the mouse during a move; confirm it cannot queue an unintended move after the animation.
- Restart during a deal, move, or return animation; confirm only the new deal remains visible.
- Undo a stack move revealing a hidden card; confirm the original stack order and face-down card are restored.
- Draw several cards, recycle, then undo repeatedly; confirm waste/stock order and the move count are restored.
- Request a hint with a card selected; confirm both selection and statistics are unchanged, then follow the gold outlines.
- Wait before the first move; confirm the timer stays at zero, starts on a successful action, and resets on New Game.
- Win a game and undo the last foundation move; confirm the win message clears and the timer resumes.
- Try an invalid destination click, double-click, and drag; confirm one quiet cue and brief instructions per attempt.
- Mute during a move or win chime; confirm audio stops immediately and later actions stay silent.
- Hold **M**; confirm it toggles only once per press. Start a new game and confirm the mute preference is retained.
- Complete a game; confirm the celebration starts after the last slide, ends after three seconds, and does not repeat.
- Undo or restart during the celebration; confirm particles and the chime stop immediately.
- Resize to small, wide, and tall windows; confirm the full board stays centered and unstretched, including the footer.
- At each size, try toolbar buttons, clicks, double-clicks, and dragging; confirm targets align and margins are inactive.
- Resize while holding a card or stock press; release and confirm no unintended action occurs, then try a fresh press.
- Resize during a deal or flip; confirm transitions remain aligned with their destinations and finish normally.
- Minimize and restore, then maximize and restore; confirm the current deal is retained.
- On a high-DPI display, or when moving between displays with different scaling, verify drawing and pointer alignment.
- Toggle larger text and contrast separately and together; verify two-digit card ranks, toolbar, footer, and settings fit.
- Open Options while dragging or dealing; confirm no click reaches a card behind the panel and held releases are ignored.
- Play using only Tab, arrows, Enter/Space, F, and Backspace; try stack moves, foundations, recycling, hints, and undo.
- Toggle reduced motion during a deal or move; confirm cards settle immediately without changing card count or history.
- Win with reduced motion enabled; confirm the static banner appears without particles, then undo and play again.
- Restart with all accessibility settings enabled; confirm preferences remain enabled and keyboard focus stays valid.
