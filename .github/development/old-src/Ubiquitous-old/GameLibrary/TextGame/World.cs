namespace TextGame;

/// <summary>Represents the game world containing all rooms</summary>
public class World
{
    private readonly Dictionary<string, Room> _rooms;

    /// <summary>Initializes a new instance of the World class</summary>
    public World()
    {
        _rooms = new Dictionary<string, Room>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Adds a room to the world</summary>
    public void AddRoom(Room room)
    {
        if (room == null)
            throw new ArgumentNullException(nameof(room));

        _rooms[room.Name] = room;
    }

    /// <summary>Gets a room by name</summary>
    public Room? GetRoom(string name)
    {
        return _rooms.TryGetValue(name, out var room) ? room : null;
    }

    /// <summary>Gets all rooms in the world</summary>
    public IEnumerable<Room> GetAllRooms()
    {
        return _rooms.Values;
    }

    /// <summary>Checks if a room exists in the world</summary>
    public bool HasRoom(string name)
    {
        return _rooms.ContainsKey(name);
    }
}
