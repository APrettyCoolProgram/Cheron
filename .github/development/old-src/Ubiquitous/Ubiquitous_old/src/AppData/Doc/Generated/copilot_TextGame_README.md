# TextGame Framework

A comprehensive .NET class library for building text-based adventure games inspired by classic interactive fiction like Zork.

## Cartridge System

**NEW in v1.0**: Create complete text adventure games using JSON files - no coding required!

?? **[See the Cartridge System Documentation](README_CARTRIDGE_INDEX.md)** for complete details.

### Quick Example

```json
{
  "Title": "My Adventure",
  "StartingRoomId": "start",
  "Rooms": [
    {
      "Id": "start",
      "Name": "Starting Room",
      "Description": "You are in a room.",
      "Exits": [{"Direction": "north", "DestinationRoomId": "library"}],
      "ItemIds": ["key"]
    }
  ],
  "Items": [
    {
      "Id": "key",
      "Name": "key",
      "Description": "A brass key.",
      "CanTake": true
    }
  ]
}
```

### Loading a Cartridge

```csharp
Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
if (startingRoom != null)
{
    var player = new Player("Hero");
    var game = new Game();
    game.Initialize(startingRoom, player);
    game.Start();
}
```

## Features

- **JSON Cartridge System**: Create games with JSON files (new!)
- **Room-based navigation**: Move between connected rooms using cardinal directions
- **Item system**: Pick up, drop, examine, and use items
- **Inventory management**: Track collected items
- **Flexible command parsing**: Support for common text adventure commands
- **Event-driven output**: Subscribe to game events for custom UI integration
- **Extensible design**: Easy to create custom game worlds

## Documentation

- **[Cartridge System Index](README_CARTRIDGE_INDEX.md)** - Navigation for all cartridge documentation
- **[Creating Games Guide](../../Cartridges/README.md)** - User-friendly guide for game creators
- **[Technical Documentation](CARTRIDGE_SYSTEM.md)** - Complete technical reference
- **[UI Integration Guide](QUICKSTART_UI_INTEGRATION.md)** - Fast integration into your UI
- **[Implementation Overview](CARTRIDGE_IMPLEMENTATION.md)** - System architecture

## Quick Start

### Method 1: Using Cartridges (Recommended)

```csharp
using TextGame;

// Load a game from a cartridge
Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");

if (startingRoom != null)
{
    var player = new Player("Hero");
    var game = new Game();
    game.Initialize(startingRoom, player);
    game.OutputGenerated += text => Console.WriteLine(text);
    game.Start();
    
    // Process commands
    while (game.IsRunning)
    {
        var input = Console.ReadLine();
        game.ProcessInput(input);
    }
}
```

### Method 2: Programmatic (Legacy)

```csharp
using TextGame;

// Create rooms programmatically
var entrance = new Room("Entrance Hall", "A grand entrance.");
var library = new Room("Library", "A dusty library.");

entrance.AddExit("north", library);
library.AddExit("south", entrance);

var key = new Item("key", "A brass key.", true);
entrance.AddItem(key);

var player = new Player("Hero");
var game = new Game();
game.Initialize(entrance, player);
game.OutputGenerated += text => Console.WriteLine(text);
game.Start();
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

### GameBuilder (NEW)
Public API for loading games from cartridges.

**Key Methods:**
- `LoadFromCartridge(path)` - Load game from JSON file
- `FindCartridges(directory)` - Discover available cartridges
- `BuildSampleGame()` - Create hardcoded sample game (backward compatibility)

### Game
The main game engine that processes commands and manages game state.

**Key Methods:**
- `Initialize(startingRoom, player)` - Set up the game
- `Start()` - Begin the game
- `ProcessInput(command)` - Process a player command
- `Stop()` - End the game
- `OutputGenerated` event - Subscribe to receive game output

### Room
Represents a location in the game world.

**Properties:**
- `Name` - Display name
- `Description` - Room description
- `Exits` - Dictionary of available exits (direction -> Room)
- `Items` - Dictionary of items in the room

**Methods:**
- `AddExit(direction, room)` - Add exit to another room
- `AddItem(item)` - Add item to room
- `RemoveItem(itemName)` - Remove item from room

### Item
Represents an interactive object in the game.

**Properties:**
- `Name` - Display name
- `Description` - Item description
- `CanTake` - Can be picked up
- `UseDescription` - Text shown when used

### Player
Tracks player state and inventory.

**Properties:**
- `Name` - Player name
- `CurrentRoom` - Current location
- `Inventory` - Player's inventory
- `MoveCount` - Number of moves made

## Creating a Game

### Option 1: Using JSON Cartridges (Easy)
1. Copy `Cartridges/Template.json`
2. Define your rooms, exits, and items in JSON
3. Place in `Cartridges/` folder
4. Load with `GameBuilder.LoadFromCartridge()`

See **[Creating Games Guide](../../Cartridges/README.md)** for details.

### Option 2: Programmatically (Advanced)
1. Create Room objects with descriptions
2. Connect rooms with AddExit()
3. Create Item objects
4. Place items in rooms with AddItem()
5. Create Player and Game instances
6. Initialize and start the game

## Sample Cartridges Included

- **HauntedMansion.json** - Default game with 4 rooms and 4 items
- **LostCave.json** - Tutorial adventure
- **Template.json** - Starter template for new games

## Future Enhancements

- NPC conversations
- Locked doors requiring items
- Win/lose conditions
- Puzzle system
- Save/load game state
- Player stats and attributes
- Item combinations
- Dynamic room descriptions

## License

Part of the Ubiquitous project.
