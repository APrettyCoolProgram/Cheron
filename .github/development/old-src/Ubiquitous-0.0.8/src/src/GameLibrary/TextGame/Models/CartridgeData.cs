namespace TextGame.Models;

/// <summary>Root data structure for a text adventure game cartridge</summary>
public class CartridgeData
{
    /// <summary>Gets or sets the game title</summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the author's name</summary>
    public string Author { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the cartridge version</summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>Gets or sets the game description</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the type of game</summary>
    public string Type { get; set; } = "TextGame";
    
    /// <summary>Gets or sets the UI type for the game</summary>
    public string UIType { get; set; } = "TextGame";
    
    /// <summary>Gets or sets the difficulty level of the game</summary>
    public string Difficulty { get; set; } = "Easy";
    
    /// <summary>Gets or sets the game genre</summary>
    public string Genre { get; set; } = "Fantasy";
    
    /// <summary>Gets or sets the ID of the starting room</summary>
    public string StartingRoomId { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the collection of room definitions</summary>
    public List<RoomData> Rooms { get; set; } = new();
    
    /// <summary>Gets or sets the collection of item definitions</summary>
    public List<ItemData> Items { get; set; } = new();
}
