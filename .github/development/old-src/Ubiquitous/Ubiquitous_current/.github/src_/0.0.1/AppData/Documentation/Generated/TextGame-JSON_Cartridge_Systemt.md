# TextGame JSON Cartridge System

## Overview

The TextGame Cartridge System allows you to create text adventure games using simple JSON files. This makes it easy to design, share, and load games without writing any code.

## Cartridge File Structure

A cartridge is a JSON file with the following structure:

```json
{
  "name": "Game Title",
  "description": "Brief description of your game",
  "author": "Your Name",
  "version": "1.0.0",
  "startingRoomId": "room_id_to_start_in",
  "rooms": [ /* array of room objects */ ],
  "items": [ /* array of item objects */ ]
}
```

### Room Object

Each room in the `rooms` array has this structure:

```json
{
  "id": "unique_room_id",
  "name": "Room Display Name",
  "description": "Detailed description shown when player enters or looks around",
  "exits": {
    "north": "target_room_id",
    "south": "another_room_id",
    "east": "yet_another_room_id"
  },
  "itemIds": ["item1_id", "item2_id"]
}
```

**Room Fields:**
- `id`: Unique identifier for the room (used internally)
- `name`: Display name shown to players
- `description`: Full description of the room
- `exits`: Dictionary of direction ? room ID mappings
- `itemIds`: Array of item IDs that start in this room

**Supported Directions:**
- Standard: `north`, `south`, `east`, `west`
- Vertical: `up`, `down`
- Extended: `northeast`, `northwest`, `southeast`, `southwest`
- Custom: Any direction word works (e.g., `enter`, `outside`, `forward`)

### Item Object

Each item in the `items` array has this structure:

```json
{
  "id": "unique_item_id",
  "name": "item",
  "description": "Description shown when examining the item",
  "canTake": true,
  "useDescription": "Text shown when using the item"
}
```

**Item Fields:**
- `id`: Unique identifier for the item (used internally)
- `name`: Name used in commands (e.g., "take key")
- `description`: Detailed description shown with "examine" command
- `canTake`: Boolean - whether item can be picked up
- `useDescription`: Text displayed when player uses the item

## Example Cartridge

Here's a minimal example:

```json
{
  "name": "Simple Cave",
  "description": "A basic cave exploration game",
  "author": "You",
  "version": "1.0.0",
  "startingRoomId": "cave_entrance",
  "rooms": [
    {
      "id": "cave_entrance",
      "name": "Cave Entrance",
      "description": "You stand at the entrance of a dark cave. A torch burns on the wall.",
      "exits": {
        "north": "deep_cave"
      },
      "itemIds": ["torch"]
    },
    {
      "id": "deep_cave",
      "name": "Deep Cave",
      "description": "The cave extends into darkness. You can barely see.",
      "exits": {
        "south": "cave_entrance"
      },
      "itemIds": ["treasure"]
    }
  ],
  "items": [
    {
      "id": "torch",
      "name": "torch",
      "description": "A burning torch mounted on the cave wall.",
      "canTake": true,
      "useDescription": "The torch illuminates the area, revealing hidden details."
    },
    {
      "id": "treasure",
      "name": "treasure",
      "description": "A chest filled with gold coins!",
      "canTake": true,
      "useDescription": "You admire the gleaming gold. You've won!"
    }
  ]
}
```

## Loading Cartridges in Code

### Basic Usage

```csharp
using TextGame;

// Load a cartridge
var startingRoom = GameBuilder.LoadFromCartridge("Cartridge/my-game.json");

if (startingRoom != null)
{
    var engine = new GameEngine();
    engine.OutputGenerated += (sender, msg) => Console.WriteLine(msg);
    engine.Start(startingRoom);
}
```

### Finding Available Cartridges

```csharp
// Find all cartridges in a directory
var cartridges = GameBuilder.FindCartridges("Cartridge/");

foreach (var cartridgePath in cartridges)
{
    Console.WriteLine($"Found: {cartridgePath}");
}
```

### Using CartridgeLoader Directly

```csharp
using TextGame.Loaders;

// Load metadata without building the game
var metadata = CartridgeLoader.LoadCartridgeMetadata("Cartridge/game.json");
Console.WriteLine($"Game: {metadata.Name} by {metadata.Author}");

// Load and build the game
var startingRoom = CartridgeLoader.LoadFromJson("Cartridge/game.json");
```

## Cartridge Directory Structure

Organize your cartridges like this:

```
GameLibrary/TextGame/Cartridge/
??? haunted-mansion.json
??? space-station.json
??? fantasy/
?   ??? dragon-quest.json
?   ??? wizard-tower.json
??? scifi/
    ??? alien-planet.json
    ??? time-machine.json
```

The `FindCartridges()` method will search subdirectories recursively.

## Design Tips

### Creating Engaging Rooms

1. **Be Descriptive**: Paint a vivid picture with your descriptions
2. **Provide Context**: Give players hints about what they can do
3. **Use Atmosphere**: Describe sounds, smells, and feelings
4. **Show, Don't Tell**: Instead of "There's a key here," try "A brass key glints in the corner"

### Item Design

1. **Make Names Simple**: Use single words or short phrases
2. **Distinguish Takeable vs. Fixed**: Use `canTake: false` for scenery items
3. **Write Interesting Use Text**: Make using items feel rewarding
4. **Consider Puzzles**: Items can hint at puzzles (key for locked door, etc.)

### Creating Good Puzzles

1. **Logical Progression**: Players should be able to deduce solutions
2. **Fair Clues**: Descriptions should hint at the solution
3. **Multiple Approaches**: Consider allowing different solutions
4. **Test Your Game**: Have someone else play it first

### Navigation Design

1. **Consistent Geography**: North/South should be opposites
2. **Memorable Landmarks**: Help players build a mental map
3. **Reasonable Size**: Start with 5-10 rooms, expand as needed
4. **Return Paths**: Every exit should have a way back

## Validation

Before distributing your cartridge, check:

- [ ] All room IDs in exits actually exist
- [ ] All item IDs in rooms actually exist
- [ ] Starting room ID exists
- [ ] Every room has a path back to start (or a reason not to)
- [ ] All required fields are present
- [ ] JSON is valid (use a JSON validator)
- [ ] Item names are lowercase and simple
- [ ] Descriptions are engaging and clear

## Sample Cartridges Included

The TextGame library includes two sample cartridges:

1. **haunted-mansion.json** - Classic mansion exploration
2. **space-station.json** - Sci-fi space station adventure

Study these examples to learn cartridge design patterns.

## Advanced Features (Future)

Potential future enhancements to the cartridge system:

- Conditional exits (locked doors requiring items)
- NPCs and dialogue trees
- Variables and game state
- Custom commands per cartridge
- Item combinations
- Scripted events
- Multiple endings tracking
- Achievement definitions

## Troubleshooting

**"Starting room not found"**
- Verify `startingRoomId` matches an actual room `id`

**"Item not appearing in room"**
- Check that item ID in room's `itemIds` matches an item's `id`

**"Can't go in direction"**
- Verify target room ID exists
- Check for typos in room IDs

**"Deserialization failed"**
- Validate JSON syntax using an online JSON validator
- Check for missing commas or quotes
- Ensure all required fields are present

## Contributing Cartridges

Want to share your cartridge? Consider:

1. Test it thoroughly
2. Include an interesting story or puzzle
3. Write clear descriptions
4. Add your name as author
5. Include version number for updates
6. Share on GitHub or game forums!

## License

Cartridges you create are your own work. The TextGame engine is available for use in your projects.
