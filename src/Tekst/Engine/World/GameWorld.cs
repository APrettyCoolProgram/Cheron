// 260620_code
// 260620_documentation

using Tekst.Models;

namespace Tekst.World;

/// <summary>Represents the game world and provides access to all rooms by identifier.</summary>
public class GameWorld
{
    /// <summary>A dictionary mapping room identifiers to their corresponding <see cref="Room"/> objects.</summary>
    private readonly Dictionary<string, Room> _rooms;

    /// <summary>Gets the identifier of the room where the game begins.</summary>
    public string StartingRoomId { get; }

    /// <summary>Initializes a new instance of the <see cref="GameWorld"/> class.</summary>
    /// <param name="rooms">The rooms available in the game world.</param>
    /// <param name="startingRoomId">The identifier of the starting room.</param>
    public GameWorld(IEnumerable<Room> rooms, string startingRoomId)
    {
        _rooms = rooms.ToDictionary(room => room.Id, StringComparer.OrdinalIgnoreCase);
        StartingRoomId = startingRoomId; //?
    }

    /// <summary> Gets the room with the specified identifier.</summary>
    /// <param name="id">The room identifier to look up.</param>
    /// <returns>The matching room, or <see langword="null"/> if no room exists with that identifier.</returns>
    public Room? GetRoom(string id)
    {
        if (_rooms.TryGetValue(id, out var room))
        {
            return (Room?)room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>Gets the room where the game starts.</summary>
    /// <remarks>
    /// If the configured starting room cannot be found, the first room in the collection is returned.
    /// </remarks>
    public Room StartingRoom
    {
        get
        {
            if (GetRoom(StartingRoomId) != null)
            {
                return GetRoom(StartingRoomId);
            }
            else
            {
                return _rooms.Values.First();
            }
        }
    }
}