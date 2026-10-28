# TextGame Library

A simple text-based adventure game engine inspired by classic games like Zork.

## Features

- Room-based navigation system
- Item inventory management
- Simple command parser
- Event-driven output system
- **JSON Cartridge System** for easy game creation
- Extensible game building framework

## Quick Start with JSON Cartridges

The easiest way to create a game is using JSON cartridge files:

```csharp
using TextGame;

// Load a game from a JSON cartridge
var startingRoom = GameBuilder.LoadFromCartridge("Cartridge/haunted-mansion.json");

if (startingRoom != null)
{
    var engine = new GameEngine();
    engine.OutputGenerated += (sender, message) => Console.WriteLine(message);
    engine.Start(startingRoom);
    
    // Process player commands
    while (engine.IsRunning)
    {
        string command = Console.ReadLine() ?? "";
        engine.ProcessCommand(command);
    }
}
```

### Finding Available Cartridges

```csharp
var cartridges = GameBuilder.FindCartridges("Cartridge/");
foreach (var cartridge in cartridges)
{
    Console.WriteLine($"Found game: {cartridge}");
}
```

## JSON Cartridge System

Create games using simple JSON files! No coding required.

### Minimal Example

Create a file called `my-game.json`:

```json
{
  "name": "My Adventure",
  "description": "A simple adventure game",
  "author": "Your Name",
  "version": "1.0.0",
  "startingRoomId": "start",
  "rooms": [
    {
      "id": "start",
      "name": "Starting Room",
      "description": "You are in a small room.",
      "exits": { "north": "treasure_room" },
      "itemIds": ["key"]
    },
    {
      "id": "treasure_room",
      "name": "Treasure Room",
      "description": "A room filled with gold!",
      "exits": { "south": "start" },
      "itemIds": ["treasure"]
    }
  ],
  "items": [
    {
      "id": "key",
      "name": "key",
      "description": "A rusty old key.",
      "canTake": true,
      "useDescription": "You turn the key in a lock."
    },
    {
      "id": "treasure",
      "name": "treasure",
      "description": "A chest of gold coins!",
      "canTake": true,
      "useDescription": "You admire your treasure."
    }
  ]
}
```

See [Cartridge/README.md](Cartridge/README.md) for complete documentation on creating JSON cartridges.

### Included Sample Cartridges

- `haunted-mansion.json` - Classic haunted house exploration
- `space-station.json` - Sci-fi space station adventure

## Traditional Code-Based Game Creation

You can also create games programmatically:

```csharp
using TextGame;

// Create a game engine
var engine = new GameEngine();

// Subscribe to output events
engine.OutputGenerated += (sender, message) => Console.WriteLine(message);

// Build a game world using the traditional method
var startingRoom = GameBuilder.BuildSampleGame();

// Start the game
engine.Start(startingRoom);

// Process commands
engine.ProcessCommand("look");
engine.ProcessCommand("go north");
engine.ProcessCommand("take book");
engine.ProcessCommand("inventory");
```

## Available Commands

- `go/move [direction]` - Move in a direction (north, south, east, west, up, down)
- `look` - Look around the current room
- `take/get [item]` - Take an item
- `inventory/inv` - Show your inventory
- `use [item]` - Use an item from your inventory
- `examine [item]` - Examine an item closely
- `help` - Show help message
- `quit/exit` - Quit the game

## Creating Custom Games (Programmatically)

You can create your own game worlds by:

1. Creating `Room` objects with names and descriptions
2. Connecting rooms using `AddExit(direction, room)`
3. Creating `Item` objects and adding them to rooms
4. Building your starting room and passing it to `engine.Start()`

### Example

```csharp
var room1 = new Room("Forest", "You are in a dark forest.");
var room2 = new Room("Clearing", "You are in a sunny clearing.");

room1.AddExit("north", room2);
room2.AddExit("south", room1);

var sword = new Item("sword", "A shiny steel sword.", true, "You swing the sword.");
room1.AddItem(sword);

var engine = new GameEngine();
engine.OutputGenerated += (s, msg) => Console.WriteLine(msg);
engine.Start(room1);
```

See `GameBuilder.cs` for a complete example.

## Architecture

- **GameEngine**: Core game loop and command processor
- **Room**: Represents locations in the game world
- **Item**: Represents objects that can be interacted with
- **GameBuilder**: Helper for constructing games
- **CartridgeLoader**: Loads games from JSON files
- **GameCartridge**: Data models for JSON deserialization

## Project Structure

```
TextGame/
??? GameEngine.cs           # Core game engine
??? Room.cs                 # Room class
??? Item.cs                 # Item class
??? GameBuilder.cs          # Game building utilities
??? Models/
?   ??? GameCartridge.cs    # JSON data models
??? Loaders/
?   ??? CartridgeLoader.cs  # JSON loading system
??? Cartridge/
    ??? README.md           # Cartridge system documentation
    ??? haunted-mansion.json
    ??? space-station.json
```

## Integration Examples

This library can be used in:

- **Console Applications**: Simple text-based gameplay
- **WPF Applications**: Rich UI with graphics
- **ASP.NET Web Apps**: Browser-based text adventures
- **Discord/Slack Bots**: Play via chat
- **Unity Games**: Text adventure mini-games
- **Mobile Apps**: Cross-platform with .NET MAUI

## Future Enhancements

Potential additions:

- Save/load game state
- NPC (Non-Player Character) system
- Combat mechanics
- Conditional exits (locked doors)
- Item combinations
- Puzzle dependency system
- Multiple endings
- Achievement tracking
- Sound effect hooks
- Game scripting support
