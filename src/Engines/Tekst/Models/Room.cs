// 260619_code
// 260619_documentation

namespace Tekst.Models;

/// <summary>A discrete location in the game world.</summary>
public class Room
{
    /// <summary>Unique identifier for this room (e.g., "great_hall").</summary>
    public required string Id { get; init; }

    /// <summary>Short title shown as a header when the player enters.</summary>
    public required string Title { get; init; }

    /// <summary>Full atmospheric description of the room.</summary>
    public required string Description { get; init; }

    /// <summary>Items currently present in this room.</summary>
    public List<Item> Items { get; init; } = [];

    /// <summary>Exits available from this room, keyed by direction.</summary>
    public List<Exit> Exits { get; init; } = [];
}
