# Game Cartridges

This folder contains JSON-formatted game cartridges that define text adventure games for the TextGame engine.

## Cartridge Format

A game cartridge is a JSON file with the following structure:

```json
{
  "title": "Game Title",
  "intro": "Game introduction text",
  "rooms": [
    {
      "id": "unique_room_id",
      "name": "Room Display Name",
      "description": "Room description shown to player",
      "isLit": true,
      "exits": [
        { "direction": "North", "targetRoomId": "another_room_id" }
      ],
      "items": ["item_id1", "item_id2"]
    }
  ],
  "items": [
    {
      "id": "unique_item_id",
      "name": "item display name",
      "description": "Item description",
      "aliases": ["alias1", "alias2"],
      "isTakeable": true,
      "isLightSource": false,
      "customActions": [
        { "verb": "read", "response": "Response when reading" }
      ]
    }
  ],
  "startingRoomId": "room_id_where_game_starts"
}
```

## Properties

### Game Level
- **title**: The game's title displayed to players
- **intro**: Introduction text shown when the game starts
- **startingRoomId**: The ID of the room where the player begins

### Room Properties
- **id**: Unique identifier for the room (used in exits and startingRoomId)
- **name**: Display name shown in the room header
- **description**: Text describing the room when the player looks
- **isLit**: Whether the room has light (false requires light source to see)
- **exits**: Array of available exits from this room
- **items**: Array of item IDs present in this room

### Exit Properties
- **direction**: Direction name (North, South, East, West, Up, Down, NorthEast, NorthWest, SouthEast, SouthWest)
- **targetRoomId**: ID of the room this exit leads to

### Item Properties
- **id**: Unique identifier for the item
- **name**: Display name of the item
- **description**: Text shown when examining the item
- **aliases**: Alternative names the player can use to refer to this item
- **isTakeable**: Whether the player can pick up this item
- **isLightSource**: Whether this item provides light in dark rooms
- **customActions**: Array of special actions for this item

### Custom Action Properties
- **verb**: The command word that triggers this action (e.g., "read", "open", "use")
- **response**: Text shown when the player performs this action

## Loading Cartridges

### From Code
```csharp
// Load cartridge data
var cartridge = Cartridge.CartridgeLoader.LoadCartridgeFromJson("path/to/cartridge.json");

// Build game from cartridge
var game = TextGame.CartridgeBuilder.BuildGameFromCartridge(cartridge);
```

### Embedded Resources
To embed a cartridge in the library:
1. Place the JSON file in the Games folder
2. The file is automatically included as an embedded resource
3. Access via assembly manifest resource stream

## Example: The Abandoned Mansion

See `TheAbandonedMansion.json` for a complete example of a working game cartridge.

## Creating Your Own Cartridge

1. Copy an existing cartridge as a template
2. Modify the title and intro
3. Design your rooms with unique IDs
4. Create items and place them in rooms
5. Connect rooms with exits
6. Set the starting room ID
7. Test by loading with `CartridgeLoader.LoadCartridgeFromJson()`

## Tips

- Use descriptive IDs (e.g., "entranceHall", "darkCellar")
- Always provide at least one alias matching the item name
- Dark rooms create tension - use sparingly
- Custom actions add interactivity beyond basic commands
- Test all exit connections to ensure rooms are properly linked
