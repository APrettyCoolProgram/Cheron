namespace TextGame;

/// <summary>
/// Represents an object in the game world
/// </summary>
/// <remarks>
/// Items can be examined, taken (if CanTake is true), used, and carried in inventory.
/// Items can be either portable objects or fixed scenery depending on the CanTake property.
/// </remarks>
public class Item
{
    /// <summary>
    /// Gets or sets the name of the item
    /// </summary>
    /// <value>The item's name as used in commands and displayed to the player</value>
    /// <remarks>
    /// This is the identifier used when players reference the item in commands.
    /// Item name matching is case-insensitive.
    /// </remarks>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the detailed description of the item
    /// </summary>
    /// <value>A description shown when the player examines the item</value>
    /// <remarks>
    /// This provides additional narrative detail about the item's appearance,
    /// history, or potential uses.
    /// </remarks>
    public string Description { get; set; }
    
    /// <summary>
    /// Gets or sets the description displayed when the item is used
    /// </summary>
    /// <value>Text shown when the player uses this item from inventory</value>
    /// <remarks>
    /// Defaults to "Nothing happens." if not specified. This can describe
    /// the result of using the item or provide hints about its purpose.
    /// </remarks>
    public string UseDescription { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the item can be taken
    /// </summary>
    /// <value>true if the item can be added to inventory; otherwise, false</value>
    /// <remarks>
    /// Set to false for scenery, fixed objects, or items that are too heavy to carry.
    /// Set to true for portable items that can be collected and used.
    /// </remarks>
    public bool CanTake { get; set; }

    /// <summary>
    /// Initializes a new instance of the Item class
    /// </summary>
    /// <param name="name">The name of the item</param>
    /// <param name="description">The detailed description of the item</param>
    /// <param name="canTake">Whether the item can be taken (default: true)</param>
    /// <param name="useDescription">Description when item is used (default: "Nothing happens.")</param>
    /// <remarks>
    /// Creates a new item with the specified properties. By default, items are takeable
    /// and have a generic use description.
    /// </remarks>
    public Item(string name, string description, bool canTake = true, string useDescription = "Nothing happens.")
    {
        Name = name;
        Description = description;
        CanTake = canTake;
        UseDescription = useDescription;
    }
}
