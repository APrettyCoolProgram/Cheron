// 260620_code
// 260620_documentation

using Tekst.Cartridge;

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

    /// <summary>Maps a RoomData object into a Room object.</summary>
    /// <param name="room">The RoomData object to map.</param>
    /// <returns>A new Room object created from the provided data.</returns>
    public static Room MapRoom(RoomData room) => new()
    {
        Id          = room.Id,
        Title       = room.Title,
        Description = room.Description,
        Items       = room.Items.Select(Item.MapItem).ToList(),
        Exits       = room.Exits.Select(Exit.MapExit).ToList(),
    };
}