namespace TextGame;

/// <summary>Represents a location in the game world</summary>
public class Room
{
    /// <summary>Gets or sets the display name of the room</summary>
    public string Name { get; set; }
    
    /// <summary>Gets or sets the narrative description of the room</summary>
    public string Description { get; set; }
    
    /// <summary>Gets or sets the available exits from this room</summary>
    public Dictionary<string, Room> Exits { get; set; }
    
    /// <summary>Gets or sets the items present in this room</summary>
    public Dictionary<string, Item> Items { get; set; }

    /// <summary>Initializes a new instance of the Room class</summary>
    public Room(string name, string description)
    {
        Name = name;
        Description = description;
        Exits = new Dictionary<string, Room>(StringComparer.OrdinalIgnoreCase);
        Items = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Adds an exit from this room to another room</summary>
    public void AddExit(string direction, Room room)
    {
        Exits[direction.ToLower()] = room;
    }

    /// <summary>Adds an item to this room</summary>
    public void AddItem(Item item)
    {
        Items[item.Name.ToLower()] = item;
    }

    /// <summary>Removes an item from this room</summary>
    public void RemoveItem(Item item)
    {
        Items.Remove(item.Name.ToLower());
    }
    
    /// <summary>Gets the room in the specified direction</summary>
    public Room? GetExit(Direction direction)
    {
        var directionKey = direction.ToString().ToLower();
        return Exits.TryGetValue(directionKey, out var room) ? room : null;
    }
    
    /// <summary>Gets an item by name</summary>
    public Item? GetItem(string name)
    {
        return Items.TryGetValue(name.ToLower(), out var item) ? item : null;
    }
    
    /// <summary>Gets all available exit directions</summary>
    public List<string> GetAvailableExits()
    {
        return Exits.Keys.Select(k => char.ToUpper(k[0]) + k.Substring(1)).ToList();
    }
    
    /// <summary>Gets all items in the room</summary>
    public List<Item> GetAllItems()
    {
        return Items.Values.ToList();
    }
}
