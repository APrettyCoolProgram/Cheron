// 260619_code
// 260619_documentation

using Tekst.Models;

namespace Tekst.World;

/// <summary>Registry of every room in the game, keyed by room Id.</summary>
public class GameWorld
{
    private readonly Dictionary<string, Room> _rooms;

    public string StartingRoomId { get; }

    public GameWorld(IEnumerable<Room> rooms, string startingRoomId)
    {
        _rooms = rooms.ToDictionary(r => r.Id, StringComparer.OrdinalIgnoreCase);
        StartingRoomId = startingRoomId;
    }

    /// <summary>Returns the room with the given Id, or null if it does not exist.</summary>
    public Room? GetRoom(string id) =>
        _rooms.TryGetValue(id, out var room) ? room : null;

    /// <summary>Returns the starting room for the game.</summary>
    public Room StartingRoom => GetRoom(StartingRoomId) ?? _rooms.Values.First();
}