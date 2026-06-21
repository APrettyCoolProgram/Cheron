<!--
  docs/man/Tekst/cartridge-format.md
  Tekst Engine — Cartridge (.tekst) file format reference
-->

# Cartridge file format

A Tekst game is defined entirely in a single `.tekst` file — a UTF-8 JSON document. No code changes are required to create a new game.

---

## Table of contents

1. [File location](#file-location)
2. [Top-level structure](#top-level-structure)
3. [title object](#title-object)
4. [winRule object](#winrule-object)
5. [rooms array](#rooms-array)
   - [items array](#items-array)
   - [exits array](#exits-array)
6. [Colour codes](#colour-codes)
7. [Victory text placeholders](#victory-text-placeholders)
8. [Full annotated example](#full-annotated-example)

---

## File location

Cartridge files must be placed in the `GameCartridges/` directory next to the Tekst executable. The file name (without the `.tekst` extension) is passed as the command-line argument when launching the game.

```
<install dir>/
├── CheronTekst.exe
└── GameCartridges/
	└── MyGame.tekst
```

---

## Top-level structure

```json
{
  "gameTitle":         "string",
  "gameAuthor":        "string",
  "gameVersion":       "string",
  "gameDescription":   "string",
  "gameEngine":        "Tekst",
  "gameType":          "string",
  "gameTypeVariation": "A",
  "gameColorScheme":   "B,W",
  "gameGenre":         "string",
  "gameDifficulty":    "string",
  "textGameType":      "string",
  "title":             { ... },
  "winRule":           { ... },
  "startingRoomId":    "string",
  "rooms":             [ ... ]
}
```

| Key | Type | Required | Default | Description |
|---|---|---|---|---|
| `gameTitle` | string | yes | `""` | Game title shown in the banner |
| `gameAuthor` | string | no | `"---"` | Author credit |
| `gameVersion` | string | no | `""` | Version label for the game content |
| `gameDescription` | string | no | `""` | Short description |
| `gameEngine` | string | no | `""` | Should be `"Tekst"` |
| `gameType` | string | no | `""` | Human-readable type label |
| `gameTypeVariation` | string | no | `"A"` | Single-character variation code |
| `gameColorScheme` | string | no | `"B,W"` | Console colours — `"Background,Foreground"` (see [Colour codes](#colour-codes)) |
| `gameGenre` | string | no | `""` | Genre label |
| `gameDifficulty` | string | no | `""` | Difficulty label |
| `textGameType` | string | no | `""` | Supplementary type label |
| `title` | object | yes | — | Title screen content |
| `winRule` | object | yes | — | Victory condition |
| `startingRoomId` | string | yes | `""` | Id of the room where the player starts |
| `rooms` | array | yes | `[]` | All rooms in the game world |

---

## title object

```json
"title": {
  "banner":     [ "line 1", "line 2" ],
  "intro":      "string",
  "helpPrompt": "string"
}
```

| Key | Type | Description |
|---|---|---|
| `banner` | string array | Lines of ASCII art (or plain text) printed at the top of the title screen |
| `intro` | string | One or two sentences that set the scene, printed below the banner |
| `helpPrompt` | string | Single-line hint, e.g. `"Type HELP for a list of commands."` |

---

## winRule object

```json
"winRule": {
  "requiredItemId": "string",
  "requiredRoomId": "string",
  "victoryText":    "string"
}
```

| Key | Type | Description |
|---|---|---|
| `requiredItemId` | string | Id of the item the player must be carrying when the win condition is checked |
| `requiredRoomId` | string | Id of the room the player must be standing in |
| `victoryText` | string | Message printed on victory. Supports [placeholders](#victory-text-placeholders) |

The win condition is checked after every command. Both conditions must be true simultaneously.

---

## rooms array

Each element in `rooms` defines one location.

```json
{
  "id":          "string",
  "title":       "string",
  "description": "string",
  "items":       [ ... ],
  "exits":       [ ... ]
}
```

| Key | Type | Required | Description |
|---|---|---|---|
| `id` | string | yes | Unique identifier for this room — used by exits and `startingRoomId` |
| `title` | string | yes | Short header shown when the player enters (wrapped in `[ ]` by the engine) |
| `description` | string | yes | Full atmospheric description |
| `items` | array | no | Items present at game start. Defaults to empty |
| `exits` | array | no | Directional links to other rooms. Defaults to empty |

Room ids must be unique within a cartridge. The engine resolves them case-insensitively.

---

### items array

Each element in `items` defines one object that can appear in a room or inventory.

```json
{
  "id":          "string",
  "name":        "string",
  "description": "string",
  "canTake":     true
}
```

| Key | Type | Required | Default | Description |
|---|---|---|---|---|
| `id` | string | yes | `""` | Identifier used in command matching (e.g., `"torch"`) |
| `name` | string | yes | `""` | Display name with article (e.g., `"a flickering torch"`) |
| `description` | string | yes | `""` | Text shown when the player types `examine <item>` |
| `canTake` | bool | no | `true` | Set to `false` for scenery items that cannot be picked up |

---

### exits array

Each element in `exits` defines a one-way connection to another room.

```json
{
  "direction":       "string",
  "targetRoomId":    "string",
  "moveDescription": "string"
}
```

| Key | Type | Required | Description |
|---|---|---|---|
| `direction` | string | yes | Keyword the player types to use this exit (e.g., `"north"`, `"up"`) |
| `targetRoomId` | string | yes | Id of the destination room |
| `moveDescription` | string | no | Optional text shown when the player travels through this exit |

Standard direction keywords: `north`, `south`, `east`, `west`, `up`, `down`. Short forms (`n`, `s`, `e`, `w`, `u`, `d`) are handled by the engine regardless of how the direction is stored in the cartridge.

Exits are one-way. To allow travel in both directions, add a matching exit in the destination room.

---

## Colour codes

The `gameColorScheme` field is a comma-separated pair: `"Background,Foreground"`.

| Code | Console colour |
|---|---|
| `B` | Black |
| `U` | Blue |
| `C` | Cyan |
| `E` | Gray |
| `G` | Green |
| `M` | Magenta |
| `R` | Red |
| `W` | White |
| `Y` | Yellow |
| `DU` | DarkBlue |
| `DC` | DarkCyan |
| `DE` | DarkGray |
| `DG` | DarkGreen |
| `DM` | DarkMagenta |
| `DR` | DarkRed |
| `DY` | DarkYellow |

**Example:** `"B,W"` — black background, white foreground.

---

## Victory text placeholders

The `victoryText` string in `winRule` supports two placeholders:

| Placeholder | Replaced with |
|---|---|
| `{0}` | The number of turns the player took |
| `{1}` | `"s"` when `turnCount != 1`, otherwise `""` (for grammatical pluralisation of "turn") |

**Example:**

```json
"victoryText": "You win in {0} turn{1}!"
```

Renders as `"You win in 1 turn!"` or `"You win in 4 turns!"`.

---

## Full annotated example

The following is a minimal but complete cartridge with two rooms and one collectable item.

```json
{
  "gameTitle":         "The Dark Tower",
  "gameAuthor":        "A. Author",
  "gameVersion":       "1.0",
  "gameDescription":   "Reach the top of the tower.",
  "gameEngine":        "Tekst",
  "gameType":          "Interactive text-based adventure",
  "gameTypeVariation": "A",
  "gameColorScheme":   "B,W",
  "gameGenre":         "Fantasy",
  "gameDifficulty":    "Easy",
  "textGameType":      "Interactive text-based adventure",

  "title": {
	"banner": [
	  "╔══════════════════════╗",
	  "║    THE DARK TOWER    ║",
	  "╚══════════════════════╝"
	],
	"intro":      "A single candle lights the entrance hall.",
	"helpPrompt": "Type HELP for a list of commands."
  },

  "winRule": {
	"requiredItemId": "key",
	"requiredRoomId": "summit",
	"victoryText":    "\nYou reach the summit and unlock the door!\n\n*** YOU WIN in {0} turn{1}! ***"
  },

  "startingRoomId": "hall",

  "rooms": [
	{
	  "id":          "hall",
	  "title":       "Entrance Hall",
	  "description": "Stone walls. A staircase climbs north.",
	  "items": [
		{
		  "id":          "key",
		  "name":        "a brass key",
		  "description": "Small and tarnished.",
		  "canTake":     true
		}
	  ],
	  "exits": [
		{
		  "direction":       "north",
		  "targetRoomId":    "summit",
		  "moveDescription": "You climb the winding stairs."
		}
	  ]
	},
	{
	  "id":          "summit",
	  "title":       "Tower Summit",
	  "description": "The city spreads below. A locked door stands before you.",
	  "items":       [],
	  "exits": [
		{
		  "direction":    "south",
		  "targetRoomId": "hall"
		}
	  ]
	}
  ]
}
```

---

*END OF cartridge-format.md*
