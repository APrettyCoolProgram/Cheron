namespace TextGame.Models;

/// <summary>
/// Data model representing a complete game cartridge
/// </summary>
/// <remarks>
/// This class is used for JSON serialization/deserialization of game cartridges.
/// It contains all metadata, rooms, and items required to build a complete game.
/// </remarks>
public class GameCartridge
{
    /// <summary>
    /// Gets or sets the name of the game
    /// </summary>
    /// <value>The display name of the game</value>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the game
    /// </summary>
    /// <value>A brief summary of the game's story or theme</value>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the author's name
    /// </summary>
    /// <value>The name of the person or team who created this game</value>
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the version number
    /// </summary>
    /// <value>Version number following semantic versioning (e.g., "1.0.0")</value>
    public string Version { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ID of the starting room
    /// </summary>
    /// <value>The room ID where the game begins</value>
    /// <remarks>
    /// Must match the Id of one of the rooms in the Rooms list.
    /// </remarks>
    public string StartingRoomId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of rooms in the game
    /// </summary>
    /// <value>Collection of room definitions</value>
    public List<RoomData> Rooms { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the list of items in the game
    /// </summary>
    /// <value>Collection of item definitions</value>
    public List<ItemData> Items { get; set; } = new();
}

/// <summary>
/// Data model representing a room definition
/// </summary>
/// <remarks>
/// This class is used for JSON serialization of room data. It contains
/// references to connected rooms and items rather than object instances.
/// </remarks>
public class RoomData
{
    /// <summary>
    /// Gets or sets the unique identifier for this room
    /// </summary>
    /// <value>A unique string identifying this room</value>
    /// <remarks>
    /// Used as a reference in exit definitions and the starting room ID.
    /// </remarks>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the room
    /// </summary>
    /// <value>The room's name as shown to the player</value>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the room
    /// </summary>
    /// <value>A narrative description of the room's appearance</value>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the exits from this room
    /// </summary>
    /// <value>Dictionary mapping direction names to room IDs</value>
    /// <remarks>
    /// Keys are direction strings (e.g., "north", "down").
    /// Values are room IDs that must match other rooms in the cartridge.
    /// </remarks>
    public Dictionary<string, string> Exits { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the items present in this room
    /// </summary>
    /// <value>List of item IDs found in this room</value>
    /// <remarks>
    /// Item IDs must match items defined in the cartridge's Items list.
    /// </remarks>
    public List<string> ItemIds { get; set; } = new();
}

/// <summary>
/// Data model representing an item definition
/// </summary>
/// <remarks>
/// This class is used for JSON serialization of item data.
/// </remarks>
public class ItemData
{
    /// <summary>
    /// Gets or sets the unique identifier for this item
    /// </summary>
    /// <value>A unique string identifying this item</value>
    /// <remarks>
    /// Used as a reference in room item lists.
    /// </remarks>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the item
    /// </summary>
    /// <value>The item's name as used in commands</value>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the detailed description of the item
    /// </summary>
    /// <value>Description shown when examining the item</value>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets whether the item can be taken
    /// </summary>
    /// <value>true if the item can be added to inventory; otherwise, false</value>
    public bool CanTake { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the description shown when using the item
    /// </summary>
    /// <value>Text displayed when the player uses this item</value>
    public string UseDescription { get; set; } = "Nothing happens.";
}
