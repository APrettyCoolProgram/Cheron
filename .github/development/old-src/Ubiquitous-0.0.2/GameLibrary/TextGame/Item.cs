namespace TextGame;

/// <summary>
/// Represents an item that can be collected, used, or examined in the game.
/// </summary>
public class Item
{
    /// <summary>
    /// Gets or sets the unique identifier for this item.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the item.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description shown when the item is examined.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this item can be picked up by the player.
    /// </summary>
    public bool IsCollectable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this item can be used.
    /// </summary>
    public bool IsUsable { get; set; }
}
