# TextGame Cartridge System

## Overview

The TextGame library includes a comprehensive JSON-based cartridge system that allows anyone to create text adventure games without writing code. Simply create a JSON file following the cartridge format specification, place it in the `Cartridges` folder, and it's ready to play!

## Architecture

### Components

1. **Data Models** (`TextGame.Models` namespace)
   - `CartridgeData`: Root data structure containing game metadata and collections
   - `RoomData`: Represents a location with exits and items
   - `ItemData`: Represents an object that can be interacted with
   - `ExitData`: Represents a connection between two rooms

2. **Loader** (`TextGame.Loaders` namespace)
   - `CartridgeLoader`: Handles JSON deserialization and object graph construction
   - Validates cartridge data for consistency and completeness
   - Provides helpful error messages when loading fails

3. **Builder** (`TextGame` namespace)
   - `GameBuilder`: Public API for loading games from cartridges
   - Maintains backward compatibility with hardcoded sample game
   - Provides cartridge discovery functionality

## Usage

### Loading a Cartridge

```csharp
// Load a specific cartridge file
Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");

if (startingRoom != null)
{
    var player = new Player("Hero");
    var game = new Game();
    game.Initialize(startingRoom, player);
    game.Start();
}
```

### Finding Available Cartridges

```csharp
// Find all cartridges in a directory
List<string> cartridges = GameBuilder.FindCartridges("Cartridges");

foreach (var cartridgePath in cartridges)
{
    Console.WriteLine($"Found cartridge: {cartridgePath}");
}
```

### Loading Sample Game

```csharp
// For backward compatibility - hardcoded sample game
Room startingRoom = GameBuilder.BuildSampleGame();
```

## JSON Cartridge Format

### Minimal Example

```json
{
  "Title": "My Adventure",
  "StartingRoomId": "start",
  "Rooms": [
    {
      "Id": "start",
      "Name": "Starting Room",
      "Description": "You are in a room.",
      "Exits": [],
      "ItemIds": []
    }
  ],
  "Items": []
}
```

### Complete Example

```json
{
  "Title": "The Haunted Mansion",
  "Author": "Game Designer",
  "Version": "1.0.0",
  "Description": "A spooky adventure",
  "StartingRoomId": "entrance",
  "Rooms": [
    {
      "Id": "entrance",
      "Name": "Entrance Hall",
      "Description": "A grand entrance hall.",
      "Exits": [
        { "Direction": "north", "DestinationRoomId": "library" }
      ],
      "ItemIds": ["key"]
    },
    {
      "Id": "library",
      "Name": "Library",
      "Description": "A dusty library.",
      "Exits": [
        { "Direction": "south", "DestinationRoomId": "entrance" }
      ],
      "ItemIds": []
    }
  ],
  "Items": [
    {
      "Id": "key",
      "Name": "key",
      "Description": "A brass key.",
      "CanTake": true,
      "UseDescription": "The key turns in the lock."
    }
  ]
}
```

## Data Model Reference

### CartridgeData

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| Title | string | Yes | - | Game title |
| Author | string | No | "" | Creator name |
| Version | string | No | "1.0.0" | Version string |
| Description | string | No | "" | Game description |
| StartingRoomId | string | Yes | - | ID of starting room |
| Rooms | List\<RoomData\> | Yes | - | All rooms in game |
| Items | List\<ItemData\> | No | [] | All items in game |

### RoomData

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| Id | string | Yes | - | Unique room identifier |
| Name | string | Yes | - | Display name |
| Description | string | Yes | - | Room description |
| Exits | List\<ExitData\> | No | [] | Available exits |
| ItemIds | List\<string\> | No | [] | Items in room |

### ExitData

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| Direction | string | Yes | - | Direction name (e.g., "north") |
| DestinationRoomId | string | Yes | - | Target room ID |

### ItemData

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| Id | string | Yes | - | Unique item identifier |
| Name | string | Yes | - | Item name for commands |
| Description | string | Yes | - | Examination text |
| CanTake | bool | No | true | Can be picked up |
| UseDescription | string | No | "Nothing happens." | Use action text |

## Validation

The `CartridgeLoader` validates cartridges during loading:

### Required Fields
- Title must not be empty
- StartingRoomId must not be empty
- At least one room must be defined
- All rooms must have an Id
- All items must have an Id

### Referential Integrity
- StartingRoomId must reference an existing room
- Exit DestinationRoomIds must reference existing rooms
- Room ItemIds must reference existing items

### Uniqueness
- Room Ids must be unique
- Item Ids must be unique

### Error Handling

The loader provides clear error messages:
- Missing required fields
- Invalid references
- Duplicate IDs
- JSON parsing errors

Errors are written to `Console` and the loader returns `null` on failure.

## Best Practices

### Naming Conventions

**Room IDs**: Use lowercase with underscores
```json
"Id": "entrance_hall"
"Id": "secret_chamber"
"Id": "main_corridor"
```

**Item IDs**: Use lowercase with underscores
```json
"Id": "brass_key"
"Id": "old_map"
"Id": "magic_sword"
```

**Item Names**: Use simple, natural names
```json
"Name": "key"      // Good - what player types
"Name": "lamp"     // Good - simple
"Name": "brass key with intricate engravings"  // Bad - too verbose
```

### Room Design

1. **Descriptive**: Paint a vivid picture
2. **Consistent**: Maintain tone and style
3. **Connected**: Ensure logical navigation
4. **Interesting**: Add details that matter

### Item Design

1. **Purposeful**: Items should have a reason to exist
2. **Clear**: Names should be obvious
3. **Appropriate**: Set CanTake correctly
4. **Satisfying**: UseDescription should give good feedback

## File Organization

```
Project Root/
??? Cartridges/                    # Cartridge JSON files
?   ??? HauntedMansion.json       # Default game
?   ??? LostCave.json             # Tutorial game
?   ??? Template.json              # Starter template
?   ??? README.md                  # User documentation
?
??? GameLibrary/TextGame/
    ??? Models/                    # Data structures
    ?   ??? CartridgeData.cs
    ?   ??? RoomData.cs
    ?   ??? ItemData.cs
    ?
    ??? Loaders/                   # JSON loading
    ?   ??? CartridgeLoader.cs
    ?
    ??? GameBuilder.cs             # Public API
```

## Integration Example

Here's how to integrate cartridge loading into a UI:

```csharp
public class GameWindow
{
    private void InitializeGame()
    {
        // Let user select a cartridge
        var cartridges = GameBuilder.FindCartridges("Cartridges");
        string selectedCartridge = ShowCartridgeSelector(cartridges);
        
        // Load the selected cartridge
        Room? startingRoom = GameBuilder.LoadFromCartridge(selectedCartridge);
        
        if (startingRoom == null)
        {
            MessageBox.Show("Failed to load cartridge. Check console for errors.");
            return;
        }
        
        // Initialize game
        var player = new Player("Hero");
        _game = new Game();
        _game.Initialize(startingRoom, player);
        _game.OutputGenerated += OnGameOutput;
        _game.Start();
    }
}
```

## Extensibility

The cartridge system is designed for future expansion:

### Potential Future Features

1. **Player Configuration**
   - Starting inventory
   - Player stats/attributes

2. **Room Features**
   - Locked exits (require items)
   - Hidden exits (require actions)
   - Dynamic descriptions (change with game state)

3. **Item Features**
   - Combinable items (crafting)
   - Item-specific interactions
   - Conditions for use

4. **Game Logic**
   - Win/lose conditions
   - Puzzles and challenges
   - NPC conversations

5. **Metadata**
   - Difficulty rating
   - Estimated play time
   - Tags/categories

### Adding New Features

To extend the cartridge format:

1. Add properties to data models
2. Update `CartridgeLoader.BuildGameWorld()`
3. Update validation if needed
4. Update documentation
5. Provide migration guide for existing cartridges

## Performance Considerations

### Loading Time
- JSON parsing is fast for typical game sizes
- Complex validation adds minimal overhead
- Consider caching for frequently-loaded cartridges

### Memory Usage
- Object graph is built entirely in memory
- Room connections create reference cycles (no issue with GC)
- Typical games use negligible memory

### Optimization Tips
1. Keep descriptions concise
2. Limit total rooms to reasonable numbers (< 100)
3. Use string interning for repeated text if needed

## Troubleshooting

### Common Issues

**Problem**: "Cartridge file not found"
- **Solution**: Verify path is correct and file exists

**Problem**: JSON parsing fails
- **Solution**: Validate JSON syntax with online validator

**Problem**: "Starting room not found"
- **Solution**: Check StartingRoomId matches a room's Id exactly

**Problem**: Items not appearing in rooms
- **Solution**: Verify ItemIds reference valid item Ids

### Debug Mode

Enable detailed logging:

```csharp
// CartridgeLoader writes to Console
// Redirect if needed:
Console.SetOut(new DebugTextWriter());
```

## License & Credits

This cartridge system is part of the Ubiquitous Game Engine.
- Uses System.Text.Json for parsing
- Follows JSON RFC 8259 specification
- Supports JSON comments and trailing commas

## Version History

### Version 1.0.0
- Initial release
- Basic cartridge loading
- Data validation
- Sample cartridges included
- Comprehensive documentation

---

For user-friendly documentation, see `Cartridges/README.md`
