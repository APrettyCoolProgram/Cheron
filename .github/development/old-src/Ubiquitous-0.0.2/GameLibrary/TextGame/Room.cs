namespace TextGame;

/// <summary>
/// Represents a location in the text adventure game.
/// </summary>
public class Room
{
    /// <summary>
    /// Gets or sets the unique identifier for this room.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the room.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the room.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the available exits from this room.
    /// Key: Direction (e.g., "north", "south", "east", "west")
    /// Value: Room ID to connect to
    /// </summary>
    public Dictionary<string, string> Exits { get; set; } = new();

    /// <summary>
    /// Gets or sets the items present in this room.
    /// </summary>
    public List<string> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets whether this room has been visited.
    /// </summary>
    public bool IsVisited { get; set; }
}
