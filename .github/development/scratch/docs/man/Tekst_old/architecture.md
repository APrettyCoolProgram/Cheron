<!--
  docs/man/Tekst/architecture.md
  Tekst Engine — Architecture reference
-->

# Architecture reference

This document describes every namespace, class, and member in the Tekst engine.

---

## Table of contents

1. [Namespace map](#namespace-map)
2. [Tekst.Cartridge](#tekstcartridge)
   - [CartridgeData](#cartridgedata)
   - [CartridgeLoader](#cartridgeloader)
   - [CartridgeLister](#cartridgelister)
3. [Tekst.Models](#tekstmodels)
   - [Room](#room)
   - [Item](#item)
   - [Exit](#exit)
4. [Tekst.World](#tekstworld)
   - [GameWorld](#gameworld)
5. [Tekst.State](#tekststate)
   - [GameState](#gamestate)
6. [Tekst.Engine](#tekstengine)
   - [CommandProcessor](#commandprocessor)
   - [CommandResult](#commandresult)
   - [WinEvaluator](#winevaluator)
7. [Tekst.Credits](#tekstcredits)
   - [OpeningCredit](#openingcredit)
   - [ClosingCredit](#closingcredit)
8. [Tekst.Catalog](#tekstcatalog)
   - [msg\_Cartridge](#msg_cartridge)
   - [msg\_Credit](#msg_credit)

---

## Namespace map

```
Tekst
├── Cartridge
│   ├── CartridgeData        root DTO + nested DTOs
│   ├── CartridgeLoader      load + validate + deserialise
│   └── CartridgeLister      (stub — not yet implemented)
├── Models
│   ├── Room                 runtime room object
│   ├── Item                 runtime item object
│   └── Exit                 runtime exit object
├── World
│   └── GameWorld            room graph + lookup
├── State
│   └── GameState            mutable session snapshot + game loop
├── Engine
│   ├── CommandProcessor     parser + verb handlers
│   ├── CommandResult        outcome of one command
│   └── WinEvaluator         victory-condition checker
├── Credits
│   ├── OpeningCredit        title screen renderer
│   └── ClosingCredit        end-of-game summary
└── Catalog
	├── msg_Cartridge        loader messages
	└── msg_Credit           credit messages
```

---

## Tekst.Cartridge

### CartridgeData

`CartridgeData` is the root deserialization target for a `.tekst` JSON file. All properties map directly to top-level JSON keys and use `System.Text.Json` with case-insensitive matching.

| Property | Type | Default | Description |
|---|---|---|---|
| `Title` | `TitleData` | new instance | Title screen content |
| `GameTitle` | `string` | `""` | Game title shown in the banner |
| `GameAuthor` | `string` | `"---"` | Game author |
| `GameVersion` | `string` | `""` | Game version string |
| `GameDescription` | `string` | `""` | Short description of the game |
| `GameEngine` | `string` | `""` | Engine identifier (should be `"Tekst"`) |
| `GameType` | `string` | `""` | Human-readable game type label |
| `GameTypeVariation` | `string` | `"A"` | Single-character type variation |
| `GameColorScheme` | `string` | `"B,W"` | Console colour scheme — `"Background,Foreground"` |
| `GameGenre` | `string` | `""` | Genre label |
| `GameDifficulty` | `string` | `""` | Difficulty label |
| `TextGameType` | `string` | `""` | Text game type label |
| `WinRule` | `WinRuleData` | new instance | Victory condition |
| `StartingRoomId` | `string` | `""` | Id of the room where the player begins |
| `Rooms` | `List<RoomData>` | `[]` | All rooms defined in the cartridge |

#### TitleData

Defines the title screen displayed at game start.

| Property | Type | Description |
|---|---|---|
| `Banner` | `List<string>` | Lines of ASCII art printed before the intro |
| `Intro` | `string` | Introductory paragraph shown after the banner |
| `HelpPrompt` | `string` | Single-line hint shown below the intro |

#### WinRuleData

Defines the victory condition.

| Property | Type | Description |
|---|---|---|
| `RequiredItemId` | `string` | Id of the item the player must be carrying |
| `RequiredRoomId` | `string` | Id of the room the player must be standing in |
| `VictoryText` | `string` | Message shown on victory. Supports `{0}` (turn count) and `{1}` (`"s"` or `""` for plural) |

#### RoomData

Defines a room in the cartridge.

| Property | Type | Description |
|---|---|---|
| `Id` | `string` | Unique room identifier |
| `Title` | `string` | Short heading shown when the player enters |
| `Description` | `string` | Atmospheric body text |
| `Items` | `List<ItemData>` | Items present at game start |
| `Exits` | `List<ExitData>` | Available exits |

#### ItemData

Defines an item.

| Property | Type | Default | Description |
|---|---|---|---|
| `Id` | `string` | `""` | Unique identifier used in command parsing |
| `Name` | `string` | `""` | Display name shown to the player |
| `Description` | `string` | `""` | Text returned when the player examines the item |
| `CanTake` | `bool` | `true` | Whether the player can pick this item up |

#### ExitData

Defines a directional exit.

| Property | Type | Description |
|---|---|---|
| `Direction` | `string` | Keyword the player types (e.g., `"north"`, `"up"`) |
| `TargetRoomId` | `string` | Id of the destination room |
| `MoveDescription` | `string?` | Optional text shown when the player travels through this exit |

---

### CartridgeLoader

`static class CartridgeLoader` — loads a `.tekst` file and assembles all runtime objects for a game session.

#### Load

```csharp
public static (GameWorld World, GameState State, CommandProcessor Processor, CartridgeData Data)
	Load(string cartName)
```

| Parameter | Description |
|---|---|
| `cartName` | File name of the cartridge (including `.tekst` extension) |

Returns a four-element tuple:

| Element | Type | Description |
|---|---|---|
| `World` | `GameWorld` | The assembled room graph |
| `State` | `GameState` | Initial game state, with `CurrentRoom` set to the starting room |
| `Processor` | `CommandProcessor` | Ready-to-use command interpreter |
| `Data` | `CartridgeData` | Raw deserialized cartridge data |

**Exceptions:**

| Exception | Condition |
|---|---|
| `FileNotFoundException` | The `.tekst` file does not exist |
| `InvalidOperationException` | The file cannot be deserialized, or the starting room id is not found |

The file is resolved from `AppContext.BaseDirectory/GameCartridges/<cartName>`.

#### ValidateCartridge *(private)*

Prints a status message and throws `FileNotFoundException` if the file does not exist.

#### Deserialize *(private)*

Reads the file and deserializes it with `System.Text.Json`. Uses case-insensitive property matching, skips comments, and allows trailing commas.

---

### CartridgeLister

`static class CartridgeLister` — stub for future cartridge discovery. No public members are currently implemented.

---

## Tekst.Models

Runtime domain objects. These are created from their `*Data` counterparts by static `Map*` factory methods and are used exclusively during a live game session.

### Room

A discrete location in the game world.

| Member | Type | Description |
|---|---|---|
| `Id` | `required string` | Unique identifier (e.g., `"great_hall"`) |
| `Title` | `required string` | Short header shown when the player enters |
| `Description` | `required string` | Full atmospheric description |
| `Items` | `List<Item>` | Items currently in the room (mutable during play) |
| `Exits` | `List<Exit>` | Available exits (read once at load) |
| `MapRoom(RoomData)` | `static Room` | Creates a `Room` from a `RoomData` DTO, mapping nested items and exits |

---

### Item

An object that can appear in a room or in the player's inventory.

| Member | Type | Description |
|---|---|---|
| `Id` | `required string` | Unique identifier used in command parsing (e.g., `"torch"`) |
| `Name` | `required string` | Display name shown to the player (e.g., `"a flickering torch"`) |
| `Description` | `required string` | Text returned when the player examines the item |
| `CanTake` | `bool` | Whether the player can add this item to their inventory. Defaults to `true` |
| `MapItem(ItemData)` | `static Item` | Creates an `Item` from an `ItemData` DTO |

---

### Exit

A one-way directional link from one room to another.

| Member | Type | Description |
|---|---|---|
| `Direction` | `required string` | Keyword the player types (e.g., `"north"`, `"up"`) |
| `TargetRoomId` | `required string` | Id of the destination `Room` |
| `MoveDescription` | `string?` | Optional text shown when the player moves through this exit |
| `MapExit(ExitData)` | `static Exit` | Creates an `Exit` from an `ExitData` DTO |

---

## Tekst.World

### GameWorld

Represents the complete game world and provides room lookup by id.

| Member | Description |
|---|---|
| `GameWorld(IEnumerable<Room>, string startingRoomId)` | Builds the internal room dictionary from the provided rooms |
| `StartingRoomId` | The id of the room where the game begins |
| `StartingRoom` | The `Room` object for the starting room. Falls back to the first room in the collection if the id is not found |
| `GetRoom(string id)` | Returns the `Room` with the given id, or `null` if not found. Look-up is case-insensitive |

---

## Tekst.State

### GameState

Mutable snapshot of all runtime data for a single game session. Also hosts the main game loop and the opening-room display as static methods.

#### Properties

| Property | Type | Description |
|---|---|---|
| `CurrentRoom` | `required Room` | The room the player is currently in |
| `Inventory` | `List<Item>` | Items the player is carrying |
| `TurnCount` | `int` | Number of turns taken so far |
| `IsGameWon` | `bool` | Set to `true` when the win condition is met |
| `IsGameOver` | `bool` | Set to `true` to end the game loop (win or quit) |

#### MainGameLoop

```csharp
public static void MainGameLoop(GameState state, CommandProcessor processor, CartridgeData cartData)
```

Runs until `state.IsGameOver` is `true`. Each iteration:

1. Displays a cyan `> ` prompt.
2. Reads a line of input.
3. Calls `processor.Process(input, state)` and prints the message in white.
4. Calls `WinEvaluator.Check(cartData.WinRule, state)` and, if a victory message is returned, prints it in green.

#### OpeningRoom

```csharp
public static void OpeningRoom(GameState state)
```

Prints the description of `state.CurrentRoom` before the main loop begins.

---

## Tekst.Engine

### CommandProcessor

Interprets raw player input and mutates `GameState` accordingly.

**Constructor:**

```csharp
public CommandProcessor(GameWorld world)
```

The `world` reference is used only for room-transition lookups in the `Go` handler.

#### Process

```csharp
public CommandResult Process(string raw, GameState state)
```

1. Increments `state.TurnCount`.
2. Normalises and tokenises the input.
3. Dispatches to the appropriate verb handler.

Returns a `CommandResult` carrying the message to display.

#### Supported verbs

| Verb(s) | Handler | Description |
|---|---|---|
| `look`, `l` | `Look` | Describes the current room |
| `go <dir>`, `move <dir>` | `Go` | Moves in the given direction |
| `north`/`n`, `south`/`s`, `east`/`e`, `west`/`w`, `up`/`u`, `down`/`d` | `Go` | Direction shortcuts |
| `take <item>`, `get <item>` | `Take` | Picks up an item from the room |
| `drop <item>` | `Drop` | Drops an item from inventory into the room |
| `inventory`, `i`, `inv` | `ShowInventory` | Lists carried items |
| `examine <item>`, `x`, `ex`, `inspect` | `Examine` | Shows item description |
| `quit`, `exit`, `q` | `Quit` | Ends the game |
| `help`, `?` | `Help` | Lists available commands |
| *(anything else)* | — | Returns `"You don't know how to ..."` |

#### DescribeRoom *(public static)*

```csharp
public static string DescribeRoom(Room room, GameState state)
```

Builds a formatted description of the room:

- `[ Title ]` heading
- Room description body
- List of items present
- `Obvious exits: <directions>` footer

Used by `Look`, `Go`, and `GameState.OpeningRoom`.

---

### CommandResult

The outcome of processing a single player command.

| Member | Type | Description |
|---|---|---|
| `Message` | `required string` | Narrative text to display |
| `ShowRoomDescription` | `bool` | Whether to print the room description after this result |
| `Say(string message, bool showRoom = false)` | `static CommandResult` | Factory method — creates a result with the given message |

---

### WinEvaluator

Evaluates the win condition defined in `WinRuleData` against the current `GameState`.

#### Check

```csharp
public static string? Check(WinRuleData rule, GameState state)
```

Returns a formatted victory message when both conditions are true:

- `state.Inventory` contains an item whose `Id` matches `rule.RequiredItemId` (case-insensitive).
- `state.CurrentRoom.Id` matches `rule.RequiredRoomId` (case-insensitive).

When the condition is met, sets `state.IsGameWon = true` and `state.IsGameOver = true`, then formats `rule.VictoryText` by substituting `{0}` with the turn count and `{1}` with `"s"` or `""`.

Returns `null` if the win condition is not yet satisfied.

---

## Tekst.Credits

### OpeningCredit

Displays the game title screen.

| Method | Description |
|---|---|
| `GameTitle(CartridgeData cartData)` | Clears the console, applies the colour scheme, displays the banner, intro, and help prompt |
| `DisplayBanner(List<string> banner)` | Iterates and prints each line of the banner |
| `SetColorScheme(string colorScheme)` | Parses a `"Background,Foreground"` string and sets console colours |
| `GetConsoleColor(string colorName)` | Maps a single-character colour code to a `ConsoleColor` value (see table below) |

#### Colour code table

| Code | `ConsoleColor` |
|---|---|
| `B` | `Black` |
| `U` | `Blue` |
| `C` | `Cyan` |
| `E` | `Gray` |
| `G` | `Green` |
| `M` | `Magenta` |
| `R` | `Red` |
| `W` | `White` |
| `Y` | `Yellow` |
| `DU` | `DarkBlue` |
| `DC` | `DarkCyan` |
| `DE` | `DarkGray` |
| `DG` | `DarkGreen` |
| `DM` | `DarkMagenta` |
| `DR` | `DarkRed` |
| `DY` | `DarkYellow` |
| *(other)* | `Red` (fallback) |

---

### ClosingCredit

Displays the end-of-game summary.

| Method | Description |
|---|---|
| `Fin(GameState state)` | Prints the turn-count summary via `msg_Credit.GameEnded` |

---

## Tekst.Catalog

Centralised, static message factories. Using these ensures all user-visible strings are defined in one place and are easy to change.

### msg\_Cartridge

| Method | Returns |
|---|---|
| `LookingFor(string cartPath)` | `"[CHERON] Looking for game cartridge:\n  {cartPath}\n"` |
| `NotFound(string cartPath)` | `"[CHERON] Cartridge not found!\n"` |
| `DeserializationFailed(string cartPath)` | `"[CHERON] Failed to deserialize cartridge!\n"` |

### msg\_Credit

| Method | Returns |
|---|---|
| `GameEnded(int turnCount)` | `"--- Game ended after {N} turn[s] ---"` (with surrounding blank lines) |

---

*END OF architecture.md*
