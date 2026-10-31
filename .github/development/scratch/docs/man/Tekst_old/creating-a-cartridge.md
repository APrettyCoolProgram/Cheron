<!--
  docs/man/Tekst/creating-a-cartridge.md
  Tekst Engine — Guide to authoring a new game cartridge
-->

# Creating a cartridge

This guide walks through building a new Tekst game from scratch. No code changes are required — everything lives in a single `.tekst` file.

---

## Table of contents

1. [Prerequisites](#prerequisites)
2. [Step 1 — Create the file](#step-1--create-the-file)
3. [Step 2 — Fill in the metadata](#step-2--fill-in-the-metadata)
4. [Step 3 — Design the title screen](#step-3--design-the-title-screen)
5. [Step 4 — Map out the rooms](#step-4--map-out-the-rooms)
6. [Step 5 — Add items](#step-5--add-items)
7. [Step 6 — Connect the rooms with exits](#step-6--connect-the-rooms-with-exits)
8. [Step 7 — Define the win condition](#step-7--define-the-win-condition)
9. [Step 8 — Set the starting room](#step-8--set-the-starting-room)
10. [Step 9 — Test the cartridge](#step-9--test-the-cartridge)
11. [Design tips](#design-tips)
12. [Checklist](#checklist)

---

## Prerequisites

- A text editor with JSON support (syntax highlighting and bracket-matching help).
- The Tekst engine built and placed in a local directory.
- Basic familiarity with JSON syntax.

---

## Step 1 — Create the file

Create a new file in the `GameCartridges/` directory next to the Tekst executable. The file name becomes the argument you pass when launching the game.

```
GameCartridges/
└── MyGame.tekst
```

The `.tekst` extension is required. The file must be valid UTF-8 JSON.

---

## Step 2 — Fill in the metadata

Open the file and start with the top-level metadata fields. These fields are informational; most are optional but good practice.

```json
{
  "gameTitle":         "My Game",
  "gameAuthor":        "Your Name",
  "gameVersion":       "1.0",
  "gameDescription":   "A short sentence describing your game.",
  "gameEngine":        "Tekst",
  "gameType":          "Interactive text-based adventure",
  "gameTypeVariation": "A",
  "gameColorScheme":   "B,W",
  "gameGenre":         "Fantasy",
  "gameDifficulty":    "Easy"
}
```

For `gameColorScheme`, choose a `"Background,Foreground"` pair using the [colour codes](cartridge-format.md#colour-codes). The default `"B,W"` (black background, white text) is a safe starting point.

---

## Step 3 — Design the title screen

Add the `title` object. The banner is displayed first, followed by the intro and help prompt.

```json
"title": {
  "banner": [
	"╔════════════════╗",
	"║    MY  GAME    ║",
	"╚════════════════╝"
  ],
  "intro":      "One sentence that sets the scene.",
  "helpPrompt": "Type HELP for a list of commands."
},
```

**Tips:**
- Keep banner lines the same width for a clean appearance.
- Use box-drawing characters (`╔`, `║`, `╚`, etc.) for borders if your environment supports them.
- The intro should hint at the goal without giving it away.

---

## Step 4 — Map out the rooms

Before writing JSON, sketch a map on paper or in a text file. Give each room:

- A short, unique **id** (no spaces; use underscores).
- A **title** (three to five words).
- A **description** (two to four atmospheric sentences).

**Example sketch:**

```
[entrance_hall] --north--> [tower_summit]
[tower_summit]  --south--> [entrance_hall]
```

A room with no exits is a dead end — the player will be stuck. Make sure every room can be left.

---

## Step 5 — Add items

For each item, decide:

- **Id** — the keyword a player types (e.g., `key`, `torch`). Keep it short and unambiguous.
- **Name** — display name with article (e.g., `"a brass key"`, `"the rusty lantern"`).
- **Description** — one or two sentences returned by `examine`.
- **CanTake** — `true` for collectibles, `false` for scenery.

Place items in the room where the player will first encounter them:

```json
"items": [
  {
	"id":          "key",
	"name":        "a brass key",
	"description": "Small and tarnished, but the mechanism looks intact.",
	"canTake":     true
  }
]
```

---

## Step 6 — Connect the rooms with exits

For each connection in your map, add an exit to the source room. Exits are one-way — add a return exit in the destination room to allow the player to go back.

```json
"exits": [
  {
	"direction":       "north",
	"targetRoomId":    "tower_summit",
	"moveDescription": "You climb the winding stairs."
  }
]
```

`moveDescription` is optional. When present, it is shown before the destination room description.

**Supported standard directions:** `north`, `south`, `east`, `west`, `up`, `down`.

Custom direction strings (e.g., `"in"`, `"out"`) are allowed but the engine will only recognise them via `go <direction>` — the direction shortcut commands (`n`, `s`, etc.) will not apply.

---

## Step 7 — Define the win condition

Add the `winRule` object. The player wins when they are in `requiredRoomId` while carrying `requiredItemId`.

```json
"winRule": {
  "requiredItemId": "key",
  "requiredRoomId": "tower_summit",
  "victoryText":    "\nYou unlock the door at the top of the tower!\n\n*** YOU WIN in {0} turn{1}! ***"
},
```

Use `{0}` for the turn count and `{1}` for the plural suffix (see [Victory text placeholders](cartridge-format.md#victory-text-placeholders)).

The win condition is evaluated after every command, so the player can trigger it by moving into the required room while already carrying the item, or by taking the item while already in the required room.

---

## Step 8 — Set the starting room

Set `startingRoomId` to the id of the room where the player begins. The id must match a room defined in the `rooms` array.

```json
"startingRoomId": "entrance_hall",
```

---

## Step 9 — Test the cartridge

1. Save the `.tekst` file.
2. Launch the engine, passing your cartridge name:

   ```
   CheronTekst MyGame
   ```

3. Walk through every room and test every exit.
4. Pick up and examine every item.
5. Verify the win condition triggers correctly.
6. Try entering unknown commands to confirm they are handled gracefully.

**Common problems:**

| Symptom | Likely cause |
|---|---|
| `[CHERON] Cartridge not found!` | File is not in `GameCartridges/` or the name is wrong |
| `[CHERON] Failed to deserialize cartridge!` | Invalid JSON — check for missing commas or unclosed braces |
| Room description shows `null` | A `targetRoomId` in an exit does not match any room `id` |
| Win condition never triggers | `requiredItemId` or `requiredRoomId` does not exactly match an item or room id |
| Item cannot be taken | `canTake` is `false`, or the noun does not match the item's `id` or `name` |

---

## Design tips

**Keep ids simple.** Use lowercase, no spaces. `great_hall`, `crown`, `iron_key` — all easy to type at a prompt.

**Write descriptions in present tense.** "A cold stone chamber. Torchlight flickers on the walls." This is the classic text adventure voice and draws the player in.

**Offer a reason to explore.** Place items across different rooms so the player has motivation to move around rather than staying in one place.

**Use `moveDescription` sparingly.** It adds colour to significant transitions, but becomes noise if every exit has one.

**Test the critical path first.** Make sure the sequence of actions required to win actually works before polishing other content.

**Dead ends need an exit.** A room with no exits creates an unwinnable state. Always provide at least one way out.

**`canTake: false` is useful for atmosphere.** Scenery items that can be examined but not taken add depth without complicating inventory logic.

---

## Checklist

Before releasing a cartridge, verify:

- [ ] Every room has a unique `id`.
- [ ] Every room can be reached from the starting room.
- [ ] Every room has at least one exit (unless intentionally terminal).
- [ ] Every exit's `targetRoomId` matches an existing room `id`.
- [ ] Every item has a unique `id` within its room.
- [ ] The `startingRoomId` matches an existing room `id`.
- [ ] The `winRule.requiredItemId` matches an existing item `id`.
- [ ] The `winRule.requiredRoomId` matches an existing room `id`.
- [ ] The `victoryText` reads correctly for both singular (`1 turn`) and plural (`N turns`).
- [ ] The JSON is valid (use a linter or editor with JSON validation).
- [ ] The game can be completed from start to finish.

---

*END OF creating-a-cartridge.md*
