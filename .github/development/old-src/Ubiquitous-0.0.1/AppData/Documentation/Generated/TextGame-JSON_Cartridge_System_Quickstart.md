# TextGame JSON Cartridge System - Quick Reference

## What is a Cartridge?

A cartridge is a JSON file that defines a complete text adventure game. It contains:
- Game metadata (name, author, version)
- Room definitions with descriptions and connections
- Item definitions with properties and behaviors
- Starting location

## Minimal Cartridge Template

```json
{
  "name": "Your Game Name",
  "description": "Brief description of your game",
  "author": "Your Name",
  "version": "1.0.0",
  "startingRoomId": "first_room",
  "rooms": [
    {
      "id": "first_room",
      "name": "Starting Location",
      "description": "You are here.",
      "exits": {},
      "itemIds": []
    }
  ],
  "items": []
}
```

## Loading a Cartridge

```csharp
using TextGame;

// Load from file
var startingRoom = GameBuilder.LoadFromCartridge("path/to/game.json");

// Create engine
var engine = new GameEngine();
engine.OutputGenerated += (s, msg) => Console.WriteLine(msg);

// Start game
if (startingRoom != null)
{
    engine.Start(startingRoom);
}
```

## Sample Cartridges Included

1. **haunted-mansion.json** - Classic mansion exploration
   - 4 rooms, 4 items
   - Good for learning basics

2. **space-station.json** - Sci-fi adventure
   - 5 rooms, 6 items
   - Shows thematic design

3. **lost-temple.json** - Jungle temple exploration
   - 5 rooms, 9 items
   - Demonstrates rich descriptions

## Room Structure

```json
{
  "id": "unique_id",           // Internal identifier
  "name": "Display Name",       // Shown to player
  "description": "Full text",   // Room description
  "exits": {                    // Connections
    "north": "room_id",
    "south": "room_id"
  },
  "itemIds": ["item1", "item2"] // Items in room
}
```

### Common Directions
- north, south, east, west
- up, down
- northeast, northwest, southeast, southwest
- Custom: enter, exit, outside, forward, back

## Item Structure

```json
{
  "id": "unique_id",            // Internal identifier
  "name": "displayname",        // Used in commands
  "description": "Full text",   // Examine description
  "canTake": true,              // Can be picked up?
  "useDescription": "Use text"  // Shown when used
}
```

## Best Practices

### Naming
- **Room IDs**: snake_case (entrance_hall)
- **Item IDs**: snake_case (brass_key)
- **Item Names**: lowercase (key, lamp, sword)
- **Display Names**: Title Case (Entrance Hall)

### Descriptions
- Be vivid and atmospheric
- Mention items naturally
- Hint at available actions
- Keep them concise but engaging

### Navigation
- Always provide return paths
- Be consistent (north ? south)
- Create a mental map
- Avoid dead ends (unless intentional)

### Items
- Make names simple and memorable
- Use canTake: false for scenery
- Write interesting use descriptions
- Consider puzzle applications

## File Organization

```
Cartridge/
??? README.md              # Full documentation
??? your-game.json         # Your cartridge
??? haunted-mansion.json   # Sample
??? space-station.json     # Sample
??? lost-temple.json       # Sample
```

## Validation Checklist

Before distributing your cartridge:

- [ ] All room exit targets exist
- [ ] All item IDs in rooms exist
- [ ] Starting room ID is valid
- [ ] JSON is syntactically correct
- [ ] No orphaned rooms (unreachable)
- [ ] All items have descriptions
- [ ] Game is playable start to finish
- [ ] No typos in descriptions
- [ ] Version number is set
- [ ] Author name is filled in

## Common Mistakes

**Problem**: "Starting room not found"
**Solution**: Check startingRoomId matches a room's id exactly

**Problem**: Item doesn't appear
**Solution**: Verify item ID in room's itemIds matches item's id

**Problem**: Can't go in direction
**Solution**: Check exit target room ID exists and is spelled correctly

**Problem**: JSON parse error
**Solution**: Validate JSON syntax (missing comma, quote, bracket)

## Tools

### JSON Validators
- jsonlint.com
- Visual Studio Code (built-in)
- Most modern text editors

### Recommended Editors
- Visual Studio Code
- Notepad++
- Sublime Text
- Any editor with JSON support

## Example: Two-Room Game

```json
{
  "name": "The Cave",
  "description": "A simple cave adventure",
  "author": "Example Author",
  "version": "1.0.0",
  "startingRoomId": "entrance",
  "rooms": [
    {
      "id": "entrance",
      "name": "Cave Entrance",
      "description": "You stand at the mouth of a dark cave.",
      "exits": {
        "north": "depths"
      },
      "itemIds": ["torch"]
    },
    {
      "id": "depths",
      "name": "Cave Depths",
      "description": "The cave extends into darkness.",
      "exits": {
        "south": "entrance"
      },
      "itemIds": ["treasure"]
    }
  ],
  "items": [
    {
      "id": "torch",
      "name": "torch",
      "description": "A wooden torch.",
      "canTake": true,
      "useDescription": "The torch illuminates the cave."
    },
    {
      "id": "treasure",
      "name": "treasure",
      "description": "A chest of gold!",
      "canTake": true,
      "useDescription": "You're rich!"
    }
  ]
}
```

## Getting Help

- See `Cartridge/README.md` for complete documentation
- Study the included sample cartridges
- Check the TextGame main README.md

## Next Steps

1. Copy a sample cartridge
2. Modify it to create your own game
3. Test it by loading in the engine
4. Share your creation!

Happy game making!
