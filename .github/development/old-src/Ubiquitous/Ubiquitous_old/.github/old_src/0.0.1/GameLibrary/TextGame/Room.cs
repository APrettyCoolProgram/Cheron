namespace TextGame;

/// <summary>
/// Represents a location in the game world
/// </summary>
/// <remarks>
/// A Room contains a name, description, connections to other rooms (exits),
/// and items present in the room. Rooms form the graph structure of the game world.
/// </remarks>
public class Room
{
    /// <summary>
    /// Gets or sets the display name of the room
    /// </summary>
    /// <value>The room's name as displayed to the player</value>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the narrative description of the room
    /// </summary>
    /// <value>A description of the room's appearance and atmosphere</value>
    public string Description { get; set; }
    
    /// <summary>
    /// Gets or sets the available exits from this room
    /// </summary>
    /// <value>Dictionary mapping direction names (e.g., "north") to connected rooms</value>
    /// <remarks>
    /// Keys are direction strings (case-insensitive). Values are the Room objects
    /// that can be reached by traveling in that direction.
    /// </remarks>
    public Dictionary<string, Room> Exits { get; set; }
    
    /// <summary>
    /// Gets or sets the items present in this room
    /// </summary>
    /// <value>Dictionary mapping item names to Item objects</value>
    /// <remarks>
    /// Items can be examined, taken (if CanTake is true), or used by the player.
    /// When an item is taken, it is removed from this dictionary.
    /// </remarks>
    public Dictionary<string, Item> Items { get; set; }

    /// <summary>
    /// Initializes a new instance of the Room class
    /// </summary>
    /// <param name="name">The display name of the room</param>
    /// <param name="description">The narrative description of the room</param>
    /// <remarks>
    /// Creates a room with empty exits and items dictionaries.
    /// Use AddExit() and AddItem() to populate the room.
    /// </remarks>
    public Room(string name, string description)
    {
        Name = name;
        Description = description;
        Exits = new Dictionary<string, Room>();
        Items = new Dictionary<string, Item>();
    }

    /// <summary>
    /// Adds an exit from this room to another room
    /// </summary>
    /// <param name="direction">The direction name (e.g., "north", "down")</param>
    /// <param name="room">The destination room</param>
    /// <remarks>
    /// Direction names are automatically converted to lowercase for consistency.
    /// If an exit already exists in that direction, it will be replaced.
    /// </remarks>
    public void AddExit(string direction, Room room)
    {
        Exits[direction.ToLower()] = room;
    }

    /// <summary>
    /// Adds an item to this room
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <remarks>
    /// The item is added using its Name property as the dictionary key.
    /// If an item with the same name already exists, it will be replaced.
    /// </remarks>
    public void AddItem(Item item)
    {
        Items[item.Name] = item;
    }

    /// <summary>
    /// Removes an item from this room
    /// </summary>
    /// <param name="itemName">The name of the item to remove</param>
    /// <remarks>
    /// Typically called when a player takes an item. If the item doesn't exist,
    /// the method has no effect (no error is raised).
    /// </remarks>
    public void RemoveItem(string itemName)
    {
        Items.Remove(itemName);
    }
}
