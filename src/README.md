# Cheron Solitaire

A simple Klondike solitaire game built with MonoGame. It uses no external game assets or font packages.

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
- Click a face-up card to select it; click a valid tableau column or foundation to move it.
- Tableau stacks build downward in alternating colors, with Kings on empty columns.
- Foundations build upward by suit, starting with Aces.
- Press **Escape** to exit.
