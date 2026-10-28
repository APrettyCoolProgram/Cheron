# Cheron Solitaire

A simple Klondike solitaire game built with MonoGame. It uses no external game assets or font packages.

## Appearance

Solitaire uses a shaded teal table, ivory rounded cards, soft shadows, geometric suit symbols,
patterned card backs, gold selection outlines, and a hover-highlighted new-game button.
Artwork is generated once during content loading and disposed with the game module.
The compact bitmap lettering remains asset-free. Cards slide and flip over 220 milliseconds, with a short
staggered deal and a 120-millisecond selection fade. Invalid drags glide back to their source.
Animations use elapsed time rather than frame counts and do not change game rules or card ownership.
Board input briefly pauses during transitions; **New Game**, sound toggling, **R**, and **Escape** remain available.
All visual assets and rendering remain inside `Games\Solitaire`.

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
On shutdown it disposes the module before MonoGame disposes the graphics device.
The module borrows the graphics device and owns/disposes the textures, sprite batches, sounds, and playback instances it creates.

To add a game, implement `IGameModule` under its own `Games` folder, supply its title and preferred window size,
and select it in `Program.cs`. A launcher or dynamic plugin system is not required.
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
- Undo and Hint are disabled during animations and dragging; Undo is also disabled when history is empty.
- Click the stock to draw one card to the waste. Click the empty stock to recycle the waste.
- Click a face-up card to select it; click a valid tableau pile or foundation to move it.
- Alternatively, drag a face-up card or tableau stack and release over a valid pile. Invalid drops leave cards in place.
- A drag begins after six pixels of movement; a shorter press/release is a click.
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
