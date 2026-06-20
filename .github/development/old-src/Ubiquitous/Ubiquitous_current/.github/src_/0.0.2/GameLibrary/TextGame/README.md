# TextGame Library

A flexible text adventure game engine for creating interactive fiction games similar to classic games like Zork.

## Features

- **Room-based navigation**: Move between connected rooms using cardinal directions
- **Item system**: Pick up, drop, examine, and use items
- **Inventory management**: Track collected items
- **Flexible command parsing**: Support for common text adventure commands
- **Event-driven output**: Subscribe to game events for custom UI integration
- **Extensible design**: Easy to create custom game worlds

## Quick Start

### Basic Usage

```csharp
using TextGame;

// Create the game engine
var engine = new GameEngine();

// Subscribe to output events
engine.OutputGenerated += (sender, text) => Console.WriteLine(text);

// Create rooms
var rooms = new Dictionary<string, Room>
{
    ["start"] = new Room
    {
        Id = "start",
        Name = "Starting Room",
        Description = "You are in a small room. There is a door to the north.",
        Exits = new Dictionary<string, string> { ["north"] = "hallway" },
        Items = new List<string> { "key" }
    },
    ["hallway"] = new Room
    {
        Id = "hallway",
        Name = "Hallway",
        Description = "A long hallway stretches before you.",
        Exits = new Dictionary<string, string> { ["south"] = "start" }
    }
};

// Create items
var items = new Dictionary<string, Item>
{
    ["key"] = new Item
    {
        Id = "key",
        Name = "brass key",
        Description = "An old brass key with intricate engravings.",
        IsCollectable = true
    }
};

// Initialize and start the game
engine.Initialize(rooms, items, "start");

// Process player commands
while (engine.IsRunning)
{
    var command = Console.ReadLine();
    if (command != null)
    {
        engine.ProcessCommand(command);
    }
}
```

## Supported Commands

### Movement
- `go [direction]` - Move in a direction
- `north`, `south`, `east`, `west` (or `n`, `s`, `e`, `w`) - Move in that direction

### Interaction
- `look` or `l` - Look at the current room
- `examine [item]` or `x [item]` - Examine an item
- `take [item]` or `get [item]` - Pick up an item
- `drop [item]` - Drop an item from inventory
- `use [item]` - Use an item

### Information
- `inventory` or `i` - Show your inventory
- `help` or `?` - Show available commands

### Game Control
- `quit` or `exit` - Exit the game

## Classes

### GameEngine
The main game engine that processes commands and manages game state.

**Key Methods:**
- `Initialize(rooms, items, startingRoomId)` - Set up the game world
- `ProcessCommand(command)` - Process a player command
- `OutputGenerated` event - Subscribe to receive game output

### Room
Represents a location in the game world.

**Properties:**
- `Id` - Unique identifier
- `Name` - Display name
- `Description` - Room description
- `Exits` - Dictionary of available exits (direction -> room ID)
- `Items` - List of item IDs in the room
- `IsVisited` - Whether the player has visited this room

### Item
Represents an interactive object in the game.

**Properties:**
- `Id` - Unique identifier
- `Name` - Display name
- `Description` - Item description
- `IsCollectable` - Can be picked up
- `IsUsable` - Can be used

### Player
Tracks player state and inventory.

**Properties:**
- `CurrentRoomId` - Current location
- `Inventory` - List of item IDs carried
- `Score` - Player's score
- `MoveCount` - Number of moves made

**Methods:**
- `AddItem(itemId)` - Add item to inventory
- `RemoveItem(itemId)` - Remove item from inventory
- `HasItem(itemId)` - Check if item is in inventory

## Creating a Game

1. Define your rooms with descriptions and exits
2. Define your items with properties
3. Create a `GameEngine` instance
4. Subscribe to `OutputGenerated` events
5. Call `Initialize()` with your game data
6. Process player commands in a loop

## Future Enhancements

- Save/load game state
- More complex command parsing
- NPC system
- Combat mechanics
- Puzzle system with dependencies
- Conditional exits (locked doors)
- Score/achievement tracking
- Multi-use items with state changes

## License

Part of the Ubiquitus project.
