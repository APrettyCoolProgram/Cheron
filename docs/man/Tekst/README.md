<!--
  260619_code
  260619_documentation
-->

<div align="center">

  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="../../../.github/repository/logo/CheronLogo-Trans-256x256.png">
    <source media="(prefers-color-scheme: light)" srcset="../../../.github/repository/logo/CheronLogo-Trans-512x512.png">
    <img alt="Fallback image description" src="../../../.github/repository/logo/CheronLogo-Trans-512x512.png">
  </picture>

  ![RELEASE](https://img.shields.io/badge/release-26.6-teal)&nbsp;
  ![STAGE](https://img.shields.io/badge/ALPHA-red)&nbsp; <!-- Alpha = Red, Beta = Yellow, Stable = Green -->
  ![LICENSE](https://img.shields.io/badge/License-apache-blue)&nbsp;
  ![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey)&nbsp;

  <h3>Manuals</h3>

</div>

# Tekst Engine

**Tekst** is an interactive text-based game engine for the Cheron platform. It loads a self-contained game cartridge (a `.tekst` file) and runs a turn-based, parser-driven adventure in a console window.

- **Target framework:** .NET 10
- **Version:** 26.6.0
- **License:** Apache 2.0

---

## Table of contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Launch sequence](#launch-sequence)
4. [Documentation index](#documentation-index)

---

## Overview

A Tekst game is a graph of **rooms** connected by directional **exits**. Rooms may contain **items** that the player can pick up, examine, and drop. The player wins by satisfying a **win rule** — carrying a required item into a required room. Every action the player types advances the **turn counter**.

Tekst is entirely data-driven. No code changes are required to create a new game; all content lives in a `.tekst` cartridge file.

---

## Architecture

```
CheronTextGame (launcher)
│
└── CartridgeLoader          loads .tekst → CartridgeData
		│
		├── GameWorld        room dictionary, lookup by id
		├── GameState        mutable runtime snapshot
		└── CommandProcessor parses player input → CommandResult
				│
				├── WinEvaluator   checks victory condition
				├── OpeningCredit  title screen + colour scheme
				└── ClosingCredit  end-of-game summary
```

| Namespace / folder   | Responsibility                                              |
|----------------------|-------------------------------------------------------------|
| `Tekst.Cartridge`    | Deserialise `.tekst` files; raw data-transfer objects       |
| `Tekst.Models`       | Runtime domain objects: `Room`, `Item`, `Exit`              |
| `Tekst.World`        | `GameWorld` — room graph and lookup                         |
| `Tekst.State`        | `GameState` — mutable session data and main game loop       |
| `Tekst.Engine`       | `CommandProcessor`, `CommandResult`, `WinEvaluator`         |
| `Tekst.Credits`      | `OpeningCredit`, `ClosingCredit` — console UI               |
| `Tekst.Catalog`      | Centralised user-facing message strings                     |

---

## Launch sequence

```
Program.Main(args)
  └─ Launch(gameName)
	   1. CartridgeLoader.Load("{gameName}.tekst")
			├─ ValidateCartridge   — confirm file exists
			├─ Deserialize         — JSON → CartridgeData
			├─ Room.MapRoom        — CartridgeData → domain objects
			├─ new GameWorld(rooms, startingRoomId)
			└─ new GameState { CurrentRoom = startRoom }

	   2. OpeningCredit.GameTitle(cartData)
			├─ SetColorScheme      — apply console colours
			├─ DisplayBanner       — ASCII art banner lines
			└─ print Intro + HelpPrompt

	   3. GameState.OpeningRoom(state)
			└─ print starting room description

	   4. GameState.MainGameLoop(state, processor, cartData)
			└─ loop until IsGameOver
				 ├─ read player input
				 ├─ CommandProcessor.Process(input, state)
				 ├─ print CommandResult.Message
				 └─ WinEvaluator.Check(winRule, state)

	   5. ClosingCredit.Fin(state)
			└─ print turn count summary
```

Usage from the command line:

```
CheronTextGame <CartridgeName>
```

`<CartridgeName>` is the file stem of a `.tekst` file placed in the `GameCartridges/` directory next to the executable. The `.tekst` extension is appended automatically.

**Example:**

```
CheronTextGame TheAbandonedKeep
```

---

## Documentation index

| Document | Description |
|---|---|
| [architecture.md](architecture.md) | Detailed breakdown of every component and class |
| [cartridge-format.md](cartridge-format.md) | Full `.tekst` JSON file format reference |
| [commands.md](commands.md) | Player command reference |
| [creating-a-cartridge.md](creating-a-cartridge.md) | Step-by-step guide to authoring a new game |

---

*END OF README.md*
