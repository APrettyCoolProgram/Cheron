# Making of Cheron R26.11 (AKira)

## GAMES  

* Tile.Memory
* Solitaire.Spider

## Prompts

### Card game type

Review the Cheron game engine: D:\Repositories\GitHub\APCP\Public\Cheron\src\.

Currently there is a `Cheron.Games.Solitaire` namespace.

We are going to:

1. Create a new namespace called `Cheron.Games.Card`, whose files will be located in the "Cheron/Games/Card/" folder

2. Move the `Cheron.Games.Solitaire` namespace to `Cheron.Games.Card.Solitaire`, and the relevent files to "Cheron/Games/Card/Solitaire/"

3. Move all of the solitaire variants (e.g., "Klondike") to their respective namespace `Cheron.Games.Card.Solitaire` (e.g., `Cheron.Games.Card.Solitaire.Klondike`) and folder (e.g., "Cheron/Games/Card/Solitaire/Klondike")

### Namespace refactor

Review the Cheron game engine: "D:\Repositories\GitHub\APCP\Public\Cheron\src\".

Please modify all of the source code files in "src/Games/Card/*" so their namespace matches their location (e.g., "src/Games/Solitaire/FreeCell/FreeCellBoardRenderer.cs" namespace is `Cheron.Games.Card.Solitaire.FreeCell`), and make sure all references to those namespaces are updated

---

## New card game: Scoundrel

### High-level plan

```text
Cheron is a custom game engine designed to create/play a variety of computer game genres. It is written in .NET 10, C#, and MonoGame. The codebase is here:

'D:/Repositories/GitHub/APCP/Public/Cheron/src/'

We are going to be creating a new card game type in Cheron called "Scoundrel".

The logic for Scoundrel will be in the `Cheron.Games.Card.Scoundrel` namespace, and all source code will be located in "src/Games/Card/Scoundrel/*".

These are the rules of the game: 'D:/Repositories/GitHub/APCP/Public/Cheron/docs/making-of/R26-11-Akira/Scoundrel-Instructions.md`

Create a high-level step-by-step plan to implement Scoundrel.

Use the existing codebase as a reference, following the existing conventions and style. Refer to existing card games (e.g., `Cheron.Games.Card.Solitaire.Klondike`, `Cheron.Games.Card.Solitaire.FreeCell`, etc.), if needed. 
```

### Step-by-step plan



---
## Copilot documentation
