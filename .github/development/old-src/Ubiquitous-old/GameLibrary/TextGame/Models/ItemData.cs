namespace TextGame.Models;

/// <summary>Data structure for an item in a game cartridge</summary>
public class ItemData
{
    /// <summary>Gets or sets the unique identifier for this item</summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the display name of the item</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the detailed description of the item</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>Gets or sets whether the item can be taken by the player</summary>
    public bool CanTake { get; set; } = true;
    
    /// <summary>Gets or sets the description displayed when the item is used</summary>
    public string UseDescription { get; set; } = "Nothing happens.";
}
