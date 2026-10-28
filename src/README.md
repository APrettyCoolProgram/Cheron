# Cheron Solitaire

A simple Klondike solitaire game built with MonoGame. It uses no external game assets or font packages.

## Appearance

Solitaire uses a shaded teal table, ivory rounded cards, soft shadows, geometric suit symbols,
patterned card backs, gold selection outlines, and a hover-highlighted new-game button.
Artwork is generated once during content loading and disposed with the game module.
The compact bitmap lettering remains asset-free. Cards support dragging, but movement and flips are not animated.
All visual assets and rendering remain inside `Games\Solitaire`.

## Organization

- `Engine`: game-neutral host and `IGameModule` lifecycle contract. Engine code must not depend on game namespaces.
- `Games\Solitaire`: Solitaire rules, state, interaction, and rendering. Other games belong in sibling folders and namespaces.
- `Program.cs`: application composition; selects the module passed to `GameHost`.

These are folder/namespace boundaries within the existing project, not separate assemblies yet.
The host owns the MonoGame window, graphics-device lifecycle, global exit controls, and active module.
It initializes the module, loads its resources once a graphics device is available, and forwards update/draw calls.
On shutdown it disposes the module before MonoGame disposes the graphics device.
The module borrows the graphics device and owns/disposes the textures and sprite batches it creates.

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
- Click the stock to draw one card to the waste. Click the empty stock to recycle the waste.
- Click a face-up card to select it; click a valid tableau pile or foundation to move it.
- Alternatively, drag a face-up card or tableau stack and release over a valid pile. Invalid drops leave cards in place.
- A drag begins after six pixels of movement; a shorter press/release is a click.
- Double-click the same card within 350 milliseconds to send an eligible top card to a foundation.
- Pale outlines mark the hovered card; mint outlines identify legal destinations for the selection or dragged stack.
- Tableau stacks build downward in alternating colors, with Kings on empty columns.
- Foundations build upward by suit, starting with Aces.
- Press **Escape** to exit.

## Interaction verification

- Select a visible strip in a face-up tableau stack; confirm the selected card and cards above it move together.
- Drag to a mint-highlighted destination; check that exposed face-down cards flip and no card is duplicated.
- Drop on the wrong color, stock, background, or footer; confirm the original piles are unchanged.
- Click a card and then a destination without dragging; check that click-to-move still works.
- Double-click an Ace, then its same-suit Two when available; confirm only eligible top cards enter foundations.
- Restart while dragging; confirm the new deal has 52 cards and releasing the button does not move any of them.
