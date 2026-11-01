# Scoundrel

## Overview

Scoundrel is a **single-player roguelike dungeon card game**. You play as an adventurer exploring a dangerous dungeon

Your objective is to **survive the dungeon** without your health reaching zero.

## Winning and losing

You win when all cards in the dungeon deck have been resolved — you've cleared the dungeon!

You lose when your health drops to zero or below at any point during the game.

## What you need

The dungeon is represented by a deck of standard playing cards, with the following modifications:

* Remove **all 6 red face cards** (Jack, Queen, and King of Hearts and Diamonds)
* Remove **both red Aces**: Ace of Hearts and Ace of Diamonds
* Remove **both Jokers**

This removes 8 cards, leaving you with a **44-card deck**.

Before playing a game, shuffle the deck.

## The three card types

Each card in the dungeon has a specific role based on its suit:

| Card Type | Suits                  | Values         | Effect                                      |
|-----------|------------------------|----------------|---------------------------------------------|
| Monsters  | ♠ Spades, ♣ Clubs      | 2–14 (Ace = 14) | Deal damage equal to their value           |
| Weapons   | ♦ Diamonds             | 2–10           | Reduce damage from monsters                 |
| Potions   | ♥ Hearts               | 2–10           | Restore health equal to their value         |

Monster values:
* 2 through 10 are face value.
* Jack = 11
* Queen = 12
* King = 13
* Ace = 14.

## Starting health

You begin the game with **20 health points**. Your health can never exceed this maximum.

## Gameplay

### Creating a room

On each turn, flip cards from the top of the Dungeon until you have four face-up. These four cards form a Room.

You must resolve any three of them, in any order.

Resolved cards go into the discard pile.

The fourth stays face-up and joins your next Room.

#### Avoiding a room

Before resolving any cards, you may place the four cards that comprise the Room at the bottom of the Dungeon.

You can avoid as many Rooms as you like, but never two in a row.

### Weapons

If you pick up a weapon, you must equip it.

If you currently have a weapon equipped, and pick up a new weapon, the old weapon is discarded.

Each weapon does as much damage as its value.

#### Weapon degradation

Once a weapon has killed a monster, it can only fight monsters with a value strictly less than its last kill.

### Fighting Monsters

When you choose to face a monster, you have two options:

1. **Barehanded**: Take damage equal to the monster's full value. A 12 Monster deals 12 damage.
2. **With a weapon**: The weapon's power is subtracted from the monster's strength. You take the remaining difference as damage (minimum 0).

Example:
You have a 8 Weapon and face a 12 Monster.
Damage taken = 12 − 8 = **4 damage**.
If your weapon was a 14, you'd take **0 damage**.

### Health potions

Health potions restore health equal to their face value. However, there are two important restrictions:

1. You may only drink one potion per room. Additional potions in the same room must be left behind or resolved without healing.
2. Your health cannot exceed your maximum (20 on Normal). A 10 Potion at 18 health only heals 2 points.
