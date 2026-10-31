namespace TextGame.Models;

/// <summary>Data structure for a room in a game cartridge</summary>
public class RoomData
{
    /// <summary>Gets or sets the unique identifier for this room</summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the display name of the room</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the narrative description of the room</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the available exits from this room</summary>
    public List<ExitData> Exits { get; set; } = new();
    
    /// <summary>Gets or sets the IDs of items present in this room</summary>
    public List<string> ItemIds { get; set; } = new();
}

/// <summary>Data structure for an exit from one room to another</summary>
public class ExitData
{
    /// <summary>Gets or sets the direction of this exit</summary>
    public string Direction { get; set; } = string.Empty;
    
    /// <summary>Gets or sets the destination room ID</summary>
    public string DestinationRoomId { get; set; } = string.Empty;
}
