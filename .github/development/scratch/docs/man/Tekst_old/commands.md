<!--
  docs/man/Tekst/commands.md
  Tekst Engine — Player command reference
-->

# Player command reference

All commands are case-insensitive. Excess whitespace is ignored.

---

## Table of contents

1. [Command syntax](#command-syntax)
2. [Movement](#movement)
3. [Interaction](#interaction)
4. [Inventory](#inventory)
5. [Information](#information)
6. [Session control](#session-control)
7. [Unknown commands](#unknown-commands)

---

## Command syntax

```
<verb> [noun]
```

A command is a verb optionally followed by a noun. Multi-word nouns are supported (the engine joins all tokens after the verb). Verbs that accept a noun will ask for clarification if none is given.

---

## Movement

### look

```
look
l
```

Describes the current room — title, description, visible items, and available exits. Does **not** advance the turn counter beyond the usual increment.

---

### go

```
go <direction>
move <direction>
<direction>
```

Moves the player in the given direction if an exit with a matching direction keyword exists in the current room.

**Direction keywords:**

| Full form | Short form |
|---|---|
| `north` | `n` |
| `south` | `s` |
| `east` | `e` |
| `west` | `w` |
| `up` | `u` |
| `down` | `d` |

The short forms can be used directly as standalone commands (e.g., typing `n` is equivalent to `go north`).

**Outcomes:**

| Condition | Message |
|---|---|
| No direction given | `"Go where?"` |
| No exit in that direction | `"You can't go that way."` |
| Exit exists but destination room is missing | `"The way ahead is blocked by an unseen force."` |
| Movement succeeds | Optional `moveDescription`, then the destination room description |

---

## Interaction

### take / get

```
take <item>
get <item>
```

Picks up the named item from the current room and adds it to the player's inventory.

**Outcomes:**

| Condition | Message |
|---|---|
| No noun given | `"Take what?"` |
| Item not in room | `"You don't see any "<noun>" here."` |
| Item is not takeable (`canTake: false`) | `"You can't take the <name>."` |
| Success | `"You pick up <name>."` |

---

### drop

```
drop <item>
```

Removes the named item from the player's inventory and places it in the current room.

**Outcomes:**

| Condition | Message |
|---|---|
| No noun given | `"Drop what?"` |
| Item not in inventory | `"You're not carrying any "<noun>"."` |
| Success | `"You drop <name>."` |

---

### examine / inspect

```
examine <item>
x <item>
ex <item>
inspect <item>
```

Displays the detailed description of the named item. The item may be in the current room or in the player's inventory.

**Outcomes:**

| Condition | Message |
|---|---|
| No noun given | `"Examine what?"` |
| Item not visible | `"You don't see any "<noun>" here."` |
| Success | The item's `description` text |

---

## Inventory

### inventory

```
inventory
inv
i
```

Lists all items currently in the player's inventory.

**Outcomes:**

| Condition | Message |
|---|---|
| Inventory is empty | `"You are carrying nothing."` |
| Inventory has items | Bulleted list of item names |

---

## Information

### help

```
help
?
```

Prints the list of available commands. Does not advance any game logic.

**Output:**

```
Available commands:
  look (l)          - Describe your surroundings
  go <direction>    - Move (north/south/east/west/up/down, or just n/s/e/w/u/d)
  take <item>       - Pick up an item
  drop <item>       - Drop a carried item
  inventory (i)     - List what you're carrying
  examine <item>    - Get a detailed look at something
  quit (q)          - End the game
  help (?)          - Show this list
```

---

## Session control

### quit / exit

```
quit
exit
q
```

Ends the current game session immediately. Sets `IsGameOver = true`, which exits the main game loop.

**Output:**

```
You abandon your quest and step back into the shadows. Farewell.
```

---

## Unknown commands

Any input whose first token does not match a known verb returns:

```
You don't know how to "<input>".
```

The turn counter is still incremented.

---

## Item matching

The engine matches an item by checking, in order:

1. Whether the noun exactly matches the item's `id` (case-insensitive).
2. Whether the noun is contained within the item's `name` (case-insensitive substring).

The first match is used. If multiple items could match, the one that appears first in the relevant list is selected.

---

*END OF commands.md*
