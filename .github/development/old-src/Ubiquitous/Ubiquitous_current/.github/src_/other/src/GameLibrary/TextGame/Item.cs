namespace TextGame;

/// <summary>Represents an object in the game world</summary>
public class Item
{
    /// <summary>Gets or sets the name of the item</summary>
    public string Name { get; set; }
    
    /// <summary>Gets or sets the detailed description of the item</summary>
    public string Description { get; set; }
    
    /// <summary>Gets or sets the description displayed when the item is used</summary>
    public string UseDescription { get; set; }
    
    /// <summary>Gets or sets a value indicating whether the item can be picked up</summary>
    public bool IsPickupable { get; set; }
    
    /// <summary>Gets or sets a value indicating whether the item can be used</summary>
    public bool IsUsable { get; set; }

    /// <summary>Event triggered when the item is used</summary>
    public event Action<Player, Room>? Used;

    /// <summary>Initializes a new instance of the Item class</summary>
    public Item(string name, string description, bool isPickupable = true, string useDescription = "Nothing happens.")
    {
        Name = name;
        Description = description;
        IsPickupable = isPickupable;
        UseDescription = useDescription;
        IsUsable = false;
    }

    /// <summary>Called when the item is used</summary>
    public void OnUse(Player player, Room room)
    {
        Used?.Invoke(player, room);
    }
}
