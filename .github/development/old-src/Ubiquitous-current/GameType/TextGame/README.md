# TextGame Library

A simple yet powerful text-based adventure game engine inspired by Zork, built for .NET 10.

## Features

- **Room-based navigation** with multiple directional exits (N, S, E, W, Up, Down, NE, NW, SE, SW)
- **Item system** with takeable/non-takeable objects and custom actions
- **Light/dark mechanics** - rooms can be dark and require a light source
- **Player inventory** management
- **Extensible command system** with natural language parsing
- **Score and move tracking**
- **Easy game creation** with fluent builder pattern

## Quick Start

### Creating a Simple Game

```csharp
using TextGame;

// Use the built-in sample game
var game = GameBuilder.CreateSampleGame();
game.Start();

// Process commands
var response = game.ProcessCommand("look");
Console.WriteLine(response);

response = game.ProcessCommand("take lamp");
Console.WriteLine(response);

response = game.ProcessCommand("go east");
Console.WriteLine(response);
```

### Creating Your Own Game

```csharp
using TextGame;

// Create rooms
var startRoom = new Room(
    "Forest Clearing",
    "You are in a small clearing surrounded by tall trees. A path leads north.",
    isLit: true
);

var cave = new Room(
    "Dark Cave",
    "You've entered a dark, forbidding cave.",
    isLit: false
);

// Connect rooms
startRoom.AddExit(Direction.North, cave);
cave.AddExit(Direction.South, startRoom);

// Create items
var torch = new Item("torch", "A wooden torch wrapped in oil-soaked cloth.")
{
    IsLightSource = true,
    IsTakeable = true
};
torch.AddAlias("light", "firebrand");

var sword = new Item("sword", "A gleaming steel sword.")
{
    IsTakeable = true
};
sword.AddCustomAction("swing", "You swing the sword through the air. Whoosh!");

// Add items to rooms
startRoom.AddItem(torch);
cave.AddItem(sword);

// Build the game
var game = new GameBuilder()
    .WithTitle("Cave Adventure")
    .WithIntro("Welcome to Cave Adventure!\n\nType 'help' for commands.")
    .WithStartingRoom(startRoom)
    .Build();

game.Start();
```

## Available Commands

### Movement
- `go [direction]` or `move [direction]` or `walk [direction]`
- Short forms: `north`, `south`, `east`, `west`, `up`, `down`
- Even shorter: `n`, `s`, `e`, `w`, `u`, `d`

### Item Interaction
- `take [item]` or `get [item]` - Pick up an item
- `drop [item]` - Drop an item from inventory
- `examine [item]` or `x [item]` - Look at an item closely
- `use [item]` - Use an item (if it has custom actions)
- `light [item]` - Light a light source
- `turn on/off [item]` - Turn a light source on or off

### Information
- `look` or `l` - Look around the current room
- `inventory` or `i` - Check your inventory
- `score` - View your score and move count

### System
- `help` or `?` - Show available commands
- `quit`, `exit`, or `q` - Quit the game

## Core Classes

### Room
Represents a location in the game world.

```csharp
var room = new Room("Room Name", "Room description", isLit: true);
room.AddExit(Direction.North, anotherRoom);
room.AddItem(item);
room.DarkDescription = "Custom darkness message";
```

### Item
Represents objects that can be interacted with.

```csharp
var item = new Item("item name", "Item description", isTakeable: true);
item.AddAlias("synonym1", "synonym2");
item.IsLightSource = true;
item.AddCustomAction("read", "The text says: Hello World!");
```

### Player
Manages player state (automatically created by GameEngine).

```csharp
player.CurrentRoom;           // Current location
player.Inventory;             // List of items
player.Score;                 // Current score
player.Moves;                 // Number of moves made
player.HasLightSource();      // Check if player has a lit light source
```

### GameEngine
The main game controller.

```csharp
var engine = new GameEngine(startingRoom, "Game Title", "Introduction text");
engine.Start();
engine.ProcessCommand("look");  // Returns response string
engine.IsRunning;               // Check if game is still running
engine.Player;                  // Access player object
```

### GameBuilder
Fluent interface for creating games.

```csharp
var game = new GameBuilder()
    .WithTitle("My Adventure")
    .WithIntro("Welcome message")
    .WithStartingRoom(startRoom)
    .Build();
```

## Example: The Abandoned Mansion

The library includes a sample game "The Abandoned Mansion" that demonstrates all features:

```csharp
var game = GameBuilder.CreateSampleGame();
game.Start();

// The sample game includes:
// - Multiple rooms (Entrance Hall, Library, Dark Cellar, Attic)
// - Various items (lamp, book, chest, key)
// - Light/dark mechanics (cellar requires lamp)
// - Custom item actions (reading books, opening chests)
```

## Extending the Library

### Custom Commands
Add custom commands by accessing the Commands dictionary:

```csharp
// Note: Direct command access would require making Commands public
// For now, use custom item actions as shown above
```

### Custom Actions
Items support custom actions that respond to verbs:

```csharp
var magicOrb = new Item("magic orb", "A glowing crystal orb.");
magicOrb.AddCustomAction("rub", "The orb glows brighter and grants you a vision!");
magicOrb.AddCustomAction("break", "You wouldn't want to break something so valuable!");
```

## Integration Example

To use in a console application:

```csharp
using TextGame;

var game = GameBuilder.CreateSampleGame();
Console.WriteLine(game.GameIntro);
game.Start();

Console.WriteLine(game.ProcessCommand("look"));

while (game.IsRunning)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(input))
        continue;
    
    var response = game.ProcessCommand(input);
    Console.WriteLine(response);
}
```

## License

This is part of the Ubiquitous project.
