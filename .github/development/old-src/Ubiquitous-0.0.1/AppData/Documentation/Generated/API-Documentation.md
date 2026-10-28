# Ubiquitus API Documentation

**Generated**: December 19, 2025  
**Version**: 0.0.1  
**TextGame Library Version**: 0.1.0

This document provides comprehensive API documentation for the Ubiquitus application and the TextGame library.

---

## Table of Contents

1. [Ubiquitus Application](#ubiquitus-application)
   - [App Class](#app-class)
   - [MainWindow Class](#mainwindow-class)
   - [SplashScreen Class](#splashscreen-class)
2. [TextGame Library](#textgame-library)
   - [GameEngine Class](#gameengine-class)
   - [Room Class](#room-class)
   - [Item Class](#item-class)
   - [GameBuilder Class](#gamebuilder-class)
3. [Models](#models)
   - [GameCartridge Class](#gamecartridge-class)
   - [RoomData Class](#roomdata-class)
   - [ItemData Class](#itemdata-class)
4. [Loaders](#loaders)
   - [CartridgeLoader Class](#cartridgeloader-class)

---

## Ubiquitus Application

### App Class

**Namespace**: `Ubiquitus`  
**Assembly**: Ubiquitus.exe

Main application class that handles the application startup sequence.

#### Properties

None

#### Methods

##### Application_Startup

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
```

Handles the application startup event. Creates and displays the splash screen, then shows the main window after the splash screen closes.

**Parameters**:
- `sender` (object): The event sender
- `e` (StartupEventArgs): Startup event arguments

**Remarks**: The splash screen displays for 3 seconds (configured in SplashScreen.xaml.cs).

---

### MainWindow Class

**Namespace**: `Ubiquitus`  
**Assembly**: Ubiquitus.exe

Main window of the Ubiquitus application. This is the primary user interface.

#### Properties

None (XAML-defined properties)

#### Constructor

```csharp
public MainWindow()
```

Initializes a new instance of the MainWindow class. Calls InitializeComponent() to load the XAML-defined user interface.

**Remarks**: Currently displays a placeholder interface. Future versions will include terminal-style text display, command input, cartridge management, and game state controls.

---

### SplashScreen Class

**Namespace**: `Ubiquitus`  
**Assembly**: Ubiquitus.exe

Splash screen window displayed on application startup.

#### Properties

None (XAML-defined properties)

#### Constructor

```csharp
public SplashScreen()
```

Initializes a new instance of the SplashScreen class. Sets up a DispatcherTimer that automatically closes the splash screen after 3 seconds.

**Remarks**: The window is borderless, centered on screen, and displays the Ubiquitus logo (ubiquitus-v1_1024x1024.png).

---

## TextGame Library

### GameEngine Class

**Namespace**: `TextGame`  
**Assembly**: TextGame.dll

Core game engine for text-based adventure games. Manages game state, processes commands, and generates output.

#### Events

##### OutputGenerated

```csharp
public event EventHandler<string>? OutputGenerated
```

Event raised when the game generates output text. Subscribe to receive all game output including room descriptions, command responses, error messages, and help text.

#### Properties

##### IsRunning

```csharp
public bool IsRunning { get; }
```

Gets a value indicating whether the game is currently running.

**Value**: true if the game is running; otherwise, false

**Remarks**: The game is running after Start() is called and until the player quits.

#### Constructor

```csharp
public GameEngine()
```

Initializes a new instance of the GameEngine class. Creates empty room and inventory dictionaries and sets the game state to not running.

#### Methods

##### Start

```csharp
public void Start(Room startingRoom)
```

Starts the game at the specified starting room.

**Parameters**:
- `startingRoom` (Room): The room where the game begins

**Remarks**: Sets the current room, marks the game as running, displays a welcome message, and describes the starting room.

##### ProcessCommand

```csharp
public void ProcessCommand(string input)
```

Processes a player command.

**Parameters**:
- `input` (string): The command string entered by the player

**Remarks**: Supported commands include: go/move, look, take/get, inventory/inv, use, examine, help, quit/exit. Commands are case-insensitive and multi-word item names are supported.

**Supported Commands**:
- `go/move [direction]` - Navigate (north, south, east, west, up, down)
- `look` - Examine current location
- `take/get [item]` - Pick up items
- `inventory/inv` - View carried items
- `use [item]` - Use items from inventory
- `examine [item]` - Get detailed descriptions
- `help` - Display available commands
- `quit/exit` - End game session

---

### Room Class

**Namespace**: `TextGame`  
**Assembly**: TextGame.dll

Represents a location in the game world. Contains name, description, exits, and items.

#### Properties

##### Name

```csharp
public string Name { get; set; }
```

Gets or sets the display name of the room.

##### Description

```csharp
public string Description { get; set; }
```

Gets or sets the narrative description of the room.

##### Exits

```csharp
public Dictionary<string, Room> Exits { get; set; }
```

Gets or sets the available exits from this room. Keys are direction names (case-insensitive), values are the Room objects that can be reached.

##### Items

```csharp
public Dictionary<string, Item> Items { get; set; }
```

Gets or sets the items present in this room. Items can be examined, taken (if CanTake is true), or used.

#### Constructor

```csharp
public Room(string name, string description)
```

Initializes a new instance of the Room class.

**Parameters**:
- `name` (string): The display name of the room
- `description` (string): The narrative description of the room

**Remarks**: Creates a room with empty exits and items dictionaries. Use AddExit() and AddItem() to populate.

#### Methods

##### AddExit

```csharp
public void AddExit(string direction, Room room)
```

Adds an exit from this room to another room.

**Parameters**:
- `direction` (string): The direction name (e.g., "north", "down")
- `room` (Room): The destination room

**Remarks**: Direction names are automatically converted to lowercase for consistency.

##### AddItem

```csharp
public void AddItem(Item item)
```

Adds an item to this room.

**Parameters**:
- `item` (Item): The item to add

**Remarks**: The item is added using its Name property as the dictionary key.

##### RemoveItem

```csharp
public void RemoveItem(string itemName)
```

Removes an item from this room.

**Parameters**:
- `itemName` (string): The name of the item to remove

**Remarks**: Typically called when a player takes an item.

---

### Item Class

**Namespace**: `TextGame`  
**Assembly**: TextGame.dll

Represents an object in the game world. Can be examined, taken, used, and carried.

#### Properties

##### Name

```csharp
public string Name { get; set; }
```

Gets or sets the name of the item. This is the identifier used in player commands.

##### Description

```csharp
public string Description { get; set; }
```

Gets or sets the detailed description shown when examining the item.

##### UseDescription

```csharp
public string UseDescription { get; set; }
```

Gets or sets the description displayed when the item is used. Defaults to "Nothing happens."

##### CanTake

```csharp
public bool CanTake { get; set; }
```

Gets or sets whether the item can be taken. Set to false for scenery or fixed objects.

#### Constructor

```csharp
public Item(string name, string description, bool canTake = true, string useDescription = "Nothing happens.")
```

Initializes a new instance of the Item class.

**Parameters**:
- `name` (string): The name of the item
- `description` (string): The detailed description of the item
- `canTake` (bool): Whether the item can be taken (default: true)
- `useDescription` (string): Description when item is used (default: "Nothing happens.")

---

### GameBuilder Class

**Namespace**: `TextGame`  
**Assembly**: TextGame.dll

Utility class for building and loading text adventure games.

#### Methods

##### LoadFromCartridge

```csharp
public static Room? LoadFromCartridge(string cartridgePath)
```

Loads a game from a JSON cartridge file.

**Parameters**:
- `cartridgePath` (string): Path to the JSON cartridge file

**Returns**: Starting room or null if load fails

**Remarks**: Delegates to CartridgeLoader.LoadFromJson() to handle JSON deserialization and game graph construction.

##### BuildSampleGame

```csharp
public static Room BuildSampleGame()
```

Builds the default sample game (Haunted Mansion).

**Returns**: The starting room of the sample game

**Remarks**: Creates a hardcoded game with 4 rooms and 4 items. Kept for backward compatibility and testing.

##### FindCartridges

```csharp
public static List<string> FindCartridges(string cartridgeDirectory)
```

Finds all cartridge files in the specified directory.

**Parameters**:
- `cartridgeDirectory` (string): Directory to search for cartridges

**Returns**: List of cartridge file paths

**Remarks**: Searches recursively for all .json files. Creates directory if it doesn't exist.

---

## Models

### GameCartridge Class

**Namespace**: `TextGame.Models`  
**Assembly**: TextGame.dll

Data model representing a complete game cartridge for JSON serialization.

#### Properties

- `Name` (string): The name of the game
- `Description` (string): A brief summary of the game
- `Author` (string): The creator's name
- `Version` (string): Version number (semantic versioning)
- `StartingRoomId` (string): The ID of the starting room
- `Rooms` (List&lt;RoomData&gt;): Collection of room definitions
- `Items` (List&lt;ItemData&gt;): Collection of item definitions

---

### RoomData Class

**Namespace**: `TextGame.Models`  
**Assembly**: TextGame.dll

Data model representing a room definition for JSON serialization.

#### Properties

- `Id` (string): Unique identifier for this room
- `Name` (string): Display name of the room
- `Description` (string): Narrative description
- `Exits` (Dictionary&lt;string, string&gt;): Direction to room ID mappings
- `ItemIds` (List&lt;string&gt;): List of item IDs in this room

---

### ItemData Class

**Namespace**: `TextGame.Models`  
**Assembly**: TextGame.dll

Data model representing an item definition for JSON serialization.

#### Properties

- `Id` (string): Unique identifier for this item
- `Name` (string): Display name of the item
- `Description` (string): Detailed description
- `CanTake` (bool): Whether the item can be taken (default: true)
- `UseDescription` (string): Description when used (default: "Nothing happens.")

---

## Loaders

### CartridgeLoader Class

**Namespace**: `TextGame.Loaders`  
**Assembly**: TextGame.dll

Handles loading and parsing of JSON cartridge files.

#### Methods

##### LoadFromJson

```csharp
public static Room? LoadFromJson(string jsonPath)
```

Loads a game from a JSON cartridge file.

**Parameters**:
- `jsonPath` (string): Path to the cartridge JSON file

**Returns**: The starting room of the game, or null if loading fails

**Remarks**: Reads the JSON file, deserializes it, builds the complete game graph, and returns the starting room.

##### LoadCartridgeMetadata

```csharp
public static GameCartridge? LoadCartridgeMetadata(string jsonPath)
```

Loads only the metadata from a cartridge file.

**Parameters**:
- `jsonPath` (string): Path to the cartridge JSON file

**Returns**: The GameCartridge metadata object, or null if loading fails

**Remarks**: Useful for browsing available cartridges without building the full game.

##### FindCartridges

```csharp
public static List<string> FindCartridges(string cartridgeDirectory)
```

Finds all JSON cartridge files in a directory.

**Parameters**:
- `cartridgeDirectory` (string): Directory path to search

**Returns**: List of paths to cartridge JSON files

**Remarks**: Searches recursively. Creates directory if it doesn't exist.

---

## Usage Examples

### Basic Game Engine Usage

```csharp
using TextGame;

// Create engine and subscribe to output
var engine = new GameEngine();
engine.OutputGenerated += (sender, message) => Console.WriteLine(message);

// Load a game
var startingRoom = GameBuilder.LoadFromCartridge("Cartridge/haunted-mansion.json");

// Start playing
engine.Start(startingRoom);

// Process commands
engine.ProcessCommand("look");
engine.ProcessCommand("take key");
engine.ProcessCommand("go north");
engine.ProcessCommand("inventory");
```

### Loading Cartridge Metadata

```csharp
using TextGame.Loaders;

var cartridges = CartridgeLoader.FindCartridges("Cartridge/");

foreach (var path in cartridges)
{
    var metadata = CartridgeLoader.LoadCartridgeMetadata(path);
    if (metadata != null)
    {
        Console.WriteLine($"{metadata.Name} v{metadata.Version}");
        Console.WriteLine($"By {metadata.Author}");
        Console.WriteLine($"{metadata.Description}");
        Console.WriteLine();
    }
}
```

### Creating a Game Programmatically

```csharp
using TextGame;

var startRoom = new Room("Start", "You are at the beginning.");
var endRoom = new Room("End", "You have reached the end!");

startRoom.AddExit("north", endRoom);
endRoom.AddExit("south", startRoom);

var key = new Item("key", "A golden key.", true, "The key glows briefly.");
startRoom.AddItem(key);

var engine = new GameEngine();
engine.OutputGenerated += (s, msg) => Console.WriteLine(msg);
engine.Start(startRoom);
```

---

## Version History

### Version 0.0.1 (December 19, 2025)

**Ubiquitus Application**:
- Initial WPF application structure
- Splash screen implementation
- Main window placeholder

**TextGame Library v0.1.0**:
- Core game engine with command parser
- Room navigation system
- Inventory management
- Item interaction system
- JSON cartridge system
- Cartridge loader and discovery

---

## See Also

- [README.md](../../../README.md) - Project overview and quick start
- [CHANGELOG.md](../../CHANGELOG.md) - Version history
- [ROADMAP.md](../../ROADMAP.md) - Future development plans
- [Development Blog](../../Devblog/devblog.md) - Detailed development notes

---

*Generated from XML documentation comments | .NET 10.0 | December 2025*
