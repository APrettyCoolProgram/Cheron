# Text Adventure Game Cartridges

Welcome to the Ubiquitous Text Adventure Cartridge System! This folder contains JSON cartridge files that define complete text adventure games. You can create your own adventures without writing any code!

## Quick Start

1. **Play an Existing Game**: The UI will automatically load cartridges from this folder
2. **Create Your Own**: Copy `Template.json` and customize it
3. **Share Your Adventure**: JSON cartridges are portable and easy to share

## What's Included

- **HauntedMansion.json** - The default adventure featuring a mysterious mansion (Basic UI)
- **LostCave.json** - A simple tutorial adventure in a cave system (Basic UI)
- **DragonsTreasure.json** - An epic quest to find the legendary Dragon's Treasure (Fancy UI)
- **EnchantedForest.json** - Journey through a magical forest (Basic UI)
- **Template.json** - A starter template for creating your own adventures

## Cartridge Format Overview

A cartridge is a JSON file with the following structure:

```json
{
  "Title": "Your Game Title",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Description": "Brief description of your game",
  "UIType": "Basic",
  "Difficulty": "Easy",
  "Type": "Fantasy",
  "StartingRoomId": "room_id_where_game_starts",
  "Rooms": [ /* array of room objects */ ],
  "Items": [ /* array of item objects */ ]
}
```

## Creating Your Own Adventure

### Step 1: Start with the Template

Copy `Template.json` to a new file (e.g., `MyAdventure.json`).

### Step 2: Define Your Metadata

```json
{
  "Title": "The Mystery Lighthouse",
  "Author": "Jane Smith",
  "Version": "1.0.0",
  "Description": "Solve the mystery of the abandoned lighthouse.",
  "UIType": "Basic",
  "Difficulty": "Intermediate",
  "Type": "Fantasy",
  "StartingRoomId": "beach"
}
```

### Step 3: Create Your Rooms

Each room needs:
- **Id**: Unique identifier (use lowercase with underscores)
- **Name**: Display name shown to player
- **Description**: What the player sees
- **Exits**: Connections to other rooms
- **ItemIds**: Items found in this room

Example room:

```json
{
  "Id": "beach",
  "Name": "Rocky Beach",
  "Description": "You stand on a rocky beach. Waves crash against the shore. To the north, a lighthouse looms against the stormy sky.",
  "Exits": [
    { "Direction": "north", "DestinationRoomId": "lighthouse_entrance" }
  ],
  "ItemIds": ["seashell", "driftwood"]
}
```

### Step 4: Create Your Items

Each item needs:
- **Id**: Unique identifier (use lowercase with underscores)
- **Name**: Name used in commands (e.g., "key", "lamp")
- **Description**: Shown when item is examined
- **CanTake**: true if item can be picked up, false for scenery
- **UseDescription**: What happens when item is used

Example item:

```json
{
  "Id": "lighthouse_key",
  "Name": "key",
  "Description": "A rusty iron key with a lighthouse emblem.",
  "CanTake": true,
  "UseDescription": "The key turns in the lock with a loud click."
}
```

## Field Reference

### Cartridge Root Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Title | string | Yes | Name of your game |
| Author | string | No | Your name or username |
| Version | string | No | Version number (default: "1.0.0") |
| Description | string | No | Brief overview of the game |
| UIType | string | No | User interface type: "Basic" or "Fancy" (default: "Basic") |
| Difficulty | string | No | Difficulty level: "Easy", "Intermediate", "Hard", or "Expert" (default: "Easy") |
| Type | string | No | Game genre: "Fantasy", "Science Fiction", or "Romance" (default: "Fantasy") |
| StartingRoomId | string | Yes | ID of the room where player starts |
| Rooms | array | Yes | List of room objects |
| Items | array | No | List of item objects |

### UI Types

- **Basic**: Simple, straightforward text interface - ideal for classic text adventures
- **Fancy**: Enhanced interface with additional visual elements and styling

### Difficulty Levels

- **Easy**: Beginner-friendly adventures with simple puzzles
- **Intermediate**: Moderate challenge with some complexity
- **Hard**: Challenging adventures requiring careful thought
- **Expert**: Very difficult adventures for experienced players

### Game Types

- **Fantasy**: Medieval, magical, or mythical settings
- **Science Fiction**: Futuristic or space-themed adventures
- **Romance**: Story-driven adventures focused on relationships

### Room Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | string | Yes | Unique identifier for this room |
| Name | string | Yes | Display name shown to player |
| Description | string | Yes | Narrative description of the room |
| Exits | array | No | List of exit objects (empty = dead end) |
| ItemIds | array | No | List of item IDs in this room |

### Exit Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Direction | string | Yes | Direction name (north, south, etc.) |
| DestinationRoomId | string | Yes | ID of the room this exit leads to |

### Item Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | string | Yes | Unique identifier for this item |
| Name | string | Yes | Name used in commands |
| Description | string | Yes | Detailed description when examined |
| CanTake | boolean | No | Can player pick this up? (default: true) |
| UseDescription | string | No | What happens when used (default: "Nothing happens.") |

## Best Practices

### Naming Conventions

- **Room IDs**: Use lowercase with underscores (e.g., `entrance_hall`, `secret_room`)
- **Item IDs**: Use lowercase with underscores (e.g., `brass_key`, `old_map`)
- **Item Names**: Use short, simple names that players would naturally type (e.g., "key" not "brass key")
- **Directions**: Use standard compass directions when possible (north, south, east, west, up, down)

### Room Design

- **Clear Descriptions**: Paint a vivid picture with your descriptions
- **Logical Exits**: If you can go north from Room A to Room B, consider adding a south exit from Room B back to Room A
- **Dead Ends**: Not all rooms need multiple exits - dead ends can create tension
- **Consistency**: Keep description style consistent throughout your adventure

### Item Design

- **Takeable vs. Scenery**: Set `CanTake: false` for large, heavy, or fixed objects
- **Meaningful Names**: Use names players would naturally type ("key" is better than "skeleton_key_with_ornate_handle")
- **Descriptive Use Text**: Make using items satisfying with good feedback
- **Strategic Placement**: Consider which rooms should contain which items for good pacing

### Choosing UI Type

- **Use Basic UI** for:
  - Classic text adventure feel
  - Simple, distraction-free gameplay
  - Retro-style games
  - Games focused purely on narrative

- **Use Fancy UI** for:
  - Enhanced visual presentation
  - Games that benefit from additional styling
  - Modern text adventure experience
  - Showcasing your adventure with polish

### Testing Your Cartridge

1. **Validate JSON**: Ensure your JSON syntax is correct (use a JSON validator)
2. **Check IDs**: Make sure all referenced IDs exist
   - StartingRoomId must match a room's Id
   - Exit DestinationRoomIds must match room Ids
   - Room ItemIds must match item Ids
3. **Test Connections**: Verify all rooms are reachable
4. **Proofread**: Check for typos in descriptions and names
5. **Test Both UIs**: Try your cartridge with both Basic and Fancy UI types

## Common Directions

Standard directions you can use in exits:
- Cardinal: `north`, `south`, `east`, `west`
- Vertical: `up`, `down`
- Diagonal: `northeast`, `northwest`, `southeast`, `southwest`
- Abbreviated: `n`, `s`, `e`, `w`, `ne`, `nw`, `se`, `sw`
- Other: `in`, `out`, `enter`, `exit`
- Custom: You can use any direction that makes sense for your game!

## Example: Simple Two-Room Adventure

```json
{
  "Title": "The Tiny Cottage",
  "Author": "Tutorial",
  "Version": "1.0.0",
  "Description": "A simple two-room adventure",
  "UIType": "Basic",
  "Difficulty": "Easy",
  "Type": "Fantasy",
  "StartingRoomId": "cottage",
  "Rooms": [
    {
      "Id": "cottage",
      "Name": "Cozy Cottage",
      "Description": "You are in a small, cozy cottage. A fireplace crackles warmly. A door leads outside to the garden.",
      "Exits": [
        { "Direction": "out", "DestinationRoomId": "garden" }
      ],
      "ItemIds": ["candle"]
    },
    {
      "Id": "garden",
      "Name": "Cottage Garden",
      "Description": "A peaceful garden surrounds the cottage. Flowers bloom in neat rows. The cottage door is to the north.",
      "Exits": [
        { "Direction": "in", "DestinationRoomId": "cottage" }
      ],
      "ItemIds": ["rose"]
    }
  ],
  "Items": [
    {
      "Id": "candle",
      "Name": "candle",
      "Description": "A half-melted candle. It still has some wick left.",
      "CanTake": true,
      "UseDescription": "You light the candle. It casts a warm, flickering glow."
    },
    {
      "Id": "rose",
      "Name": "rose",
      "Description": "A beautiful red rose with soft petals.",
      "CanTake": true,
      "UseDescription": "You smell the rose. Its fragrance is lovely."
    }
  ]
}
```

## Troubleshooting

### "Cartridge file not found"
- Make sure your JSON file is in the `Cartridges` folder
- Check that the file extension is `.json` (not `.txt`)

### "Failed to deserialize cartridge data"
- Validate your JSON syntax using a JSON validator
- Check for missing commas, brackets, or quotes
- Ensure all strings are in quotes

### "Starting room not found"
- Verify `StartingRoomId` matches one of your room's `Id` values exactly
- Check for typos or case mismatches

### "Exit references non-existent room"
- Ensure all `DestinationRoomId` values match actual room `Id` values
- Check for typos in room IDs

### "Room references non-existent item"
- Verify all item IDs in `ItemIds` arrays match actual item `Id` values
- Check for typos in item IDs

### "Wrong UI loads"
- Check the `UIType` field is set to either "Basic" or "Fancy"
- Verify the cartridge loader properly reads the UIType field

## Tips for Great Adventures

1. **Start Small**: Begin with 3-5 rooms and expand from there
2. **Tell a Story**: Give your adventure a narrative arc
3. **Create Atmosphere**: Use descriptions to set mood and tone
4. **Add Details**: Interesting items make locations memorable
5. **Test Thoroughly**: Play through your adventure multiple times
6. **Get Feedback**: Have others play and provide feedback
7. **Iterate**: Refine based on playtesting
8. **Choose the Right UI**: Match the UI type to your game's style

## Sharing Your Cartridges

Cartridge JSON files are:
- **Portable**: Works on any computer with Ubiquitous
- **Text-based**: Easy to version control with Git
- **Shareable**: Email, upload, or share however you like
- **Editable**: Anyone can open and modify with a text editor

## Need Help?

- Review the included example cartridges for inspiration
- Use `Template.json` as a starting point
- Refer to this guide for field references
- Check the troubleshooting section for common issues

Happy adventuring! ??
