# Ubiquitous API Documentation

**Version:** 0.0.2  
**Framework:** .NET 10 / WPF  
**Namespaces:** Ubiquitous, TextGame

---

## Table of Contents
- [Overview](#overview)
- [Ubiquitous Application](#ubiquitous-application)
  - [App](#app)
  - [SplashScreen](#splashscreen)
  - [MainWindow](#mainwindow)
- [TextGame Library](#textgame-library)
  - [Game](#game)
  - [CommandParser](#commandparser)
  - [Player](#player)
  - [Room](#room)
  - [Item](#item)
  - [Inventory](#inventory)
  - [World](#world)
- [Usage Examples](#usage-examples)

---

## Overview

Ubiquitous is a WPF-based game engine currently in early development. This documentation covers the public API surface of both the core application components and the TextGame library.

**Current Version Features:**
- Application initialization and startup
- Splash screen with timer-based transitions
- Main application window with text game integration
- Enhanced UI option (MainWindowFancy) with status panel and inventory display
- Automatic version detection and display
- TextGame library for text-based adventure games
- Command parsing with natural language support
- Room-based world management
- Item and inventory systems
- Color-coded and flashing text support

---

## Ubiquitous Application

### App

**Namespace:** `Ubiquitous`  
**Inheritance:** `System.Windows.Application`

#### Description
The main application class that defines the entry point and startup behavior for the Ubiquitous game engine.

#### Class Declaration
```csharp
public partial class App : Application
```

#### Properties
Inherits all properties from `System.Windows.Application`, including:
- `Current` - Gets the Application object for the current AppDomain
- `MainWindow` - Gets or sets the main window of the application

#### Usage Example
```csharp
// Access the current application instance:
var app = Application.Current as App;
```

---

### SplashScreen

**Namespace:** `Ubiquitous`  
**Inheritance:** `System.Windows.Window`

#### Description
Represents the application splash screen displayed during startup. Shows the application logo and version information for 8 seconds before automatically transitioning to the main window.

#### Constructor

##### `SplashScreen()`
Initializes a new instance of the SplashScreen class, sets up the version display, and configures the timer for automatic transition.

```csharp
public SplashScreen()
```

#### Methods

##### `SetVersionText()` (Private)
Sets the version text displayed on the splash screen by reading the assembly version.

**Format:** "Version X.Y.Z" (major.minor.build)  
**Example Output:** "Version 0.0.2"

##### `Timer_Tick()` (Private)
Handles the timer tick event to transition from the splash screen to the main window after 8 seconds.

---

### MainWindow

**Namespace:** `Ubiquitous`  
**Inheritance:** `System.Windows.Window`

#### Description
Represents the primary application window. This is the main interface users interact with after the splash screen.

#### Constructor

##### `MainWindow()`
Initializes a new instance of the MainWindow class.

---

## TextGame Library

### Game

**Namespace:** `TextGame`

#### Description
Main game controller that manages the game state, game loop, and command processing for text-based adventure games.

#### Class Declaration
```csharp
public class Game
```

#### Properties
- `World` - Gets the current world instance containing all rooms
- `Player` - Gets the current player character

#### Constructor

##### `Game()`
Creates a new game instance with a command parser and world.

#### Methods

##### `Initialize(Room startingRoom, Player player)`
Initializes the game with a starting room and player character.

```csharp
public void Initialize(Room startingRoom, Player player)
```

**Parameters:**
- `startingRoom` - The room where the player begins
- `player` - The player character

##### `Start()`
Starts the main game loop. Processes player input until the game ends.

```csharp
public void Start()
```

**Throws:** `InvalidOperationException` if game not initialized

#### Usage Example
```csharp
var game = new Game();
var startRoom = new Room("Entrance", "A dark entrance hall.");
var player = new Player("Hero");
game.Initialize(startRoom, player);
game.Start();
```

---

### CommandParser

**Namespace:** `TextGame`

#### Description
Parses natural language commands into game actions. Supports various synonyms and command patterns.

#### Class Declaration
```csharp
public class CommandParser
```

#### Methods

##### `Parse(string input)`
Parses a player input string into a Command object.

```csharp
public Command? Parse(string input)
```

**Parameters:**
- `input` - The player's text input

**Returns:** `Command` object or `null` if input cannot be parsed

**Supported Commands:**
- Movement: "go north", "n", "move east"
- Observation: "look", "examine key", "look at door"
- Inventory: "take key", "drop sword", "inventory"
- Actions: "use key"
- System: "help", "quit"

##### `AddActionWord(string word, CommandAction action)`
Adds a custom action word to the parser vocabulary.

```csharp
public void AddActionWord(string word, CommandAction action)
```

##### `AddDirectionWord(string word, Direction direction)`
Adds a custom direction word to the parser vocabulary.

```csharp
public void AddDirectionWord(string word, Direction direction)
```

#### Enumerations

**CommandAction:**
- `Unknown`, `Move`, `Look`, `Take`, `Drop`, `Inventory`, `Examine`, `Use`, `Help`, `Quit`

**Direction:**
- `North`, `South`, `East`, `West`, `Up`, `Down`

---

### Player

**Namespace:** `TextGame`

#### Description
Represents the player character with inventory and custom properties.

#### Class Declaration
```csharp
public class Player
```

#### Properties
- `Name` - Player's name
- `Inventory` - Player's inventory collection
- `CurrentRoom` - The room the player is currently in
- `Properties` - Dictionary of custom properties

#### Constructor

##### `Player(string name, int inventoryCapacity = int.MaxValue)` 
Creates a new player character.

```csharp
public Player(string name, int inventoryCapacity = int.MaxValue)
```

**Parameters:**
- `name` - Player's name
- `inventoryCapacity` - Maximum inventory size (default: unlimited)

#### Methods

##### `SetProperty(string key, object value)`
Sets a custom property on the player.

##### `GetProperty<T>(string key)`
Gets a custom property value.

##### `HasProperty(string key)`
Checks if a property exists.

---

### Room

**Namespace:** `TextGame`

#### Description
Represents a location in the game world with exits, items, and descriptions.

#### Class Declaration
```csharp
public class Room
```

#### Properties
- `Name` - Room name/title
- `Description` - Room description text
- `IsVisited` - Whether the player has been here before

#### Constructor

##### `Room(string name, string description)`
Creates a new room.

```csharp
public Room(string name, string description)
```

#### Methods

##### `AddExit(Direction direction, Room room)`
Adds a one-way exit to another room.

```csharp
public void AddExit(Direction direction, Room room)
```

##### `AddTwoWayExit(Direction direction, Room room)`
Adds a bidirectional exit between this room and another.

```csharp
public void AddTwoWayExit(Direction direction, Room room)
```

##### `GetExit(Direction direction)`
Gets the room in a specific direction.

**Returns:** `Room?` - The connected room or null

##### `GetAvailableExits()`
Gets a list of available exit directions.

**Returns:** `List<string>` - Direction names in lowercase

##### `AddItem(Item item)`
Adds an item to this room.

##### `RemoveItem(Item item)`
Removes an item from this room.

##### `GetItem(string name)`
Gets an item by name (partial, case-insensitive match).

**Returns:** `Item?` - The matching item or null

##### `GetAllItems()`
Gets all items in the room.

**Returns:** `List<Item>`

---

### Item

**Namespace:** `TextGame`

#### Description
Represents an item that can exist in rooms or player inventory.

#### Class Declaration
```csharp
public class Item
```

#### Properties
- `Name` - Item name
- `Description` - Item description
- `Aliases` - Alternative names for the item
- `IsPickupable` - Whether the item can be taken
- `IsUsable` - Whether the item can be used
- `Properties` - Dictionary of custom properties

#### Events
- `Used` - Triggered when the item is used

#### Constructor

##### `Item(string name, string description)`
Creates a new item.

```csharp
public Item(string name, string description)
```

#### Methods

##### `AddAlias(string alias)`
Adds an alternative name for this item.

##### `SetProperty(string key, object value)`
Sets a custom property.

##### `GetProperty<T>(string key)`
Gets a custom property value.

#### Usage Example
```csharp
var key = new Item("Rusty Key", "An old key covered in rust.");
key.AddAlias("key");
key.IsPickupable = true;
key.IsUsable = true;
key.Used += (sender, e) => {
    // Handle key usage
    Console.WriteLine("You unlock the door!");
};
```

---

### Inventory

**Namespace:** `TextGame`

#### Description
Manages a collection of items with capacity limits.

#### Class Declaration
```csharp
public class Inventory
```

#### Properties
- `Capacity` - Maximum number of items
- `Count` - Current number of items

#### Constructor

##### `Inventory(int capacity = int.MaxValue)` 
Creates a new inventory with optional capacity limit.

#### Methods

##### `AddItem(Item item)`
Adds an item to the inventory.

**Returns:** `bool` - True if added, false if inventory is full

##### `RemoveItem(Item item)`
Removes an item from the inventory.

**Returns:** `bool` - True if removed, false if not found

##### `GetItem(string name)`
Gets an item by name (partial, case-insensitive match).

**Returns:** `Item?` - The matching item or null

##### `GetAllItems()`
Gets all items in the inventory.

**Returns:** `List<Item>`

##### `HasItem(Item item)` / `HasItem(string name)`
Checks if the inventory contains an item.

**Returns:** `bool`

##### `Clear()`
Removes all items from the inventory.

---

### World

**Namespace:** `TextGame`

#### Description
Manages the game world and all rooms.

#### Class Declaration
```csharp
public class World
```

#### Constructor

##### `World()`
Creates a new game world.

#### Methods

##### `AddRoom(Room room)`
Adds a room to the world.

##### `GetRoom(string name)`
Gets a room by name (case-insensitive).

**Returns:** `Room?` - The matching room or null

##### `GetAllRooms()`
Gets all rooms in the world.

**Returns:** `List<Room>`

##### `RemoveRoom(Room room)`
Removes a room from the world.

---

## Usage Examples

### Complete Application Flow

```csharp
// 1. Application starts via App.xaml
// 2. SplashScreen shows for 8 seconds
// 3. MainWindow appears
// 4. User interacts with the application
```

### Creating a Text Adventure Game

```csharp
// Create game world
var game = new Game();

// Create rooms
var entrance = new Room("Entrance Hall", "A grand entrance with marble floors.");
var library = new Room("Library", "Dusty bookshelves line the walls.");
var garden = new Room("Garden", "A peaceful garden with a fountain.");

// Connect rooms
entrance.AddTwoWayExit(Direction.North, library);
entrance.AddTwoWayExit(Direction.East, garden);

// Create items
var key = new Item("Brass Key", "A shiny brass key.");
key.AddAlias("key");
key.IsPickupable = true;
entrance.AddItem(key);

var book = new Item("Ancient Tome", "A leather-bound book with strange symbols.");
book.AddAlias("book");
book.AddAlias("tome");
book.IsPickupable = true;
library.AddItem(book);

// Create player
var player = new Player("Adventurer", inventoryCapacity: 10);

// Initialize and start game
game.Initialize(entrance, player);
game.Start();

// Player can now type commands like:
// "go north" - Move to library
// "take key" - Pick up the key
// "inventory" - Check inventory
// "examine book" - Look at the book
```

### Custom Command Words

```csharp
var parser = new CommandParser();

// Add custom action words
parser.AddActionWord("grab", CommandAction.Take);
parser.AddActionWord("discard", CommandAction.Drop);

// Add custom direction words
parser.AddDirectionWord("forward", Direction.North);
parser.AddDirectionWord("back", Direction.South);
```

### Item Events

```csharp
var magicWand = new Item("Magic Wand", "A wand crackling with energy.");
magicWand.IsUsable = true;

magicWand.Used += (sender, e) =>
{
    var player = e.Player;
    var room = e.Room;
    
    Console.WriteLine("The wand glows brightly!");
    player.SetProperty("MagicPower", true);
};
```

---

## Type Hierarchy

```
System.Windows.Application
    ??? Ubiquitous.App

System.Windows.Window
    ??? Ubiquitous.SplashScreen
    ??? Ubiquitous.MainWindow

TextGame
    ??? Game
    ??? CommandParser
    ??? Player
    ??? Room
    ??? Item
    ??? Inventory
    ??? World
```

---

## Assembly Information

**Main Assembly:** Ubiquitous  
**Version:** 0.0.2.0  
**Target Framework:** net10.0-windows  
**Output Type:** WinExe

**Library Assembly:** TextGame  
**Version:** 0.0.2.0  
**Target Framework:** net10.0  
**Output Type:** Library

---

## Dependencies

### Framework Dependencies
- .NET 10.0 SDK
- Windows Presentation Foundation (WPF)
- System.Reflection
- System.Windows.Threading
- System.Linq

### No External NuGet Packages
Both assemblies use only built-in .NET framework features.

---

## Best Practices

### Game Development

1. **Room Design**
   - Give rooms descriptive names and detailed descriptions
   - Use `AddTwoWayExit` for bidirectional connections
   - Add exits logically (opposite directions should connect properly)

2. **Item Management**
   - Always add aliases for common item names
   - Set `IsPickupable` appropriately (not all items should be takeable)
   - Use custom properties for complex item states

3. **Command Parsing**
   - The parser handles common synonyms automatically
   - Add custom words for domain-specific vocabulary
   - Test commands from a player's perspective

4. **Player Inventory**
   - Set reasonable capacity limits
   - Consider weight or size properties for realism
   - Provide feedback when inventory is full

---

## Future API Extensions

### Version 0.0.3+
- Game UI integration into MainWindow
- Save/load game state
- Settings management

### Version 0.1.0+
- Additional game types (visual adventures, RPG)
- Advanced parsing (multi-word commands, context awareness)
- NPC system
- Quest/objective tracking

---

## Support and Resources

- **Repository:** [https://github.com/APrettyCoolProgram/ubiquitous](https://github.com/APrettyCoolProgram/ubiquitous)
- **Issues:** [https://github.com/APrettyCoolProgram/ubiquitous/issues](https://github.com/APrettyCoolProgram/ubiquitous/issues)

---

**Last Updated:** December 20, 2025  
**API Version:** 0.0.2
