using System.Text.Json;
using System.IO;
using TextGame.Models;

namespace TextGame.Loaders;

/// <summary>
/// Handles loading and parsing of JSON cartridge files
/// </summary>
/// <remarks>
/// The CartridgeLoader provides functionality to:
/// - Load complete games from JSON files
/// - Load only cartridge metadata for browsing
/// - Discover available cartridges in a directory
/// - Build runtime game objects from cartridge data
/// </remarks>
public class CartridgeLoader
{
    /// <summary>
    /// Loads a game from a JSON cartridge file
    /// </summary>
    /// <param name="jsonPath">Path to the cartridge JSON file</param>
    /// <returns>The starting room of the game, or null if loading fails</returns>
    /// <remarks>
    /// This method reads the JSON file, deserializes it to a GameCartridge object,
    /// builds the complete game graph (rooms, items, connections), and returns
    /// the starting room. Any errors during loading are logged to the console.
    /// </remarks>
    public static Room? LoadFromJson(string jsonPath)
    {
        try
        {
            string jsonContent = File.ReadAllText(jsonPath);
            var cartridge = JsonSerializer.Deserialize<GameCartridge>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            if (cartridge == null)
            {
                Console.WriteLine("Failed to deserialize cartridge.");
                return null;
            }

            return BuildGameFromCartridge(cartridge);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cartridge: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Builds a game graph from a GameCartridge data model
    /// </summary>
    /// <param name="cartridge">The cartridge data to build from</param>
    /// <returns>The starting room, or null if the starting room ID is invalid</returns>
    /// <remarks>
    /// This method constructs the runtime game objects in the following order:
    /// 1. Create all Item objects from ItemData
    /// 2. Create all Room objects from RoomData
    /// 3. Populate rooms with their items
    /// 4. Establish room connections (exits)
    /// 5. Return the starting room
    /// This order prevents circular reference issues.
    /// </remarks>
    private static Room? BuildGameFromCartridge(GameCartridge cartridge)
    {
        var rooms = new Dictionary<string, Room>();
        var items = new Dictionary<string, Item>();

        // Create all items first
        foreach (var itemData in cartridge.Items)
        {
            items[itemData.Id] = new Item(
                itemData.Name,
                itemData.Description,
                itemData.CanTake,
                itemData.UseDescription
            );
        }

        // Create all rooms
        foreach (var roomData in cartridge.Rooms)
        {
            rooms[roomData.Id] = new Room(roomData.Name, roomData.Description);
        }

        // Add items to rooms and establish exits
        foreach (var roomData in cartridge.Rooms)
        {
            var room = rooms[roomData.Id];

            // Add items to this room
            foreach (var itemId in roomData.ItemIds)
            {
                if (items.TryGetValue(itemId, out var item))
                {
                    room.AddItem(item);
                }
            }

            // Establish exits
            foreach (var exit in roomData.Exits)
            {
                if (rooms.TryGetValue(exit.Value, out var targetRoom))
                {
                    room.AddExit(exit.Key, targetRoom);
                }
            }
        }

        // Return the starting room
        if (rooms.TryGetValue(cartridge.StartingRoomId, out var startingRoom))
        {
            return startingRoom;
        }

        Console.WriteLine($"Starting room '{cartridge.StartingRoomId}' not found.");
        return null;
    }

    /// <summary>
    /// Loads only the metadata from a cartridge file
    /// </summary>
    /// <param name="jsonPath">Path to the cartridge JSON file</param>
    /// <returns>The GameCartridge metadata object, or null if loading fails</returns>
    /// <remarks>
    /// This is useful for browsing available cartridges without building the full game.
    /// It allows quick access to name, description, author, and version information.
    /// </remarks>
    public static GameCartridge? LoadCartridgeMetadata(string jsonPath)
    {
        try
        {
            string jsonContent = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<GameCartridge>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cartridge metadata: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Finds all JSON cartridge files in a directory
    /// </summary>
    /// <param name="cartridgeDirectory">Directory path to search</param>
    /// <returns>List of paths to cartridge JSON files</returns>
    /// <remarks>
    /// Searches recursively for all .json files in the specified directory.
    /// If the directory doesn't exist, it will be created and an empty list returned.
    /// </remarks>
    public static List<string> FindCartridges(string cartridgeDirectory)
    {
        if (!Directory.Exists(cartridgeDirectory))
        {
            Directory.CreateDirectory(cartridgeDirectory);
            return new List<string>();
        }

        return Directory.GetFiles(cartridgeDirectory, "*.json", SearchOption.AllDirectories).ToList();
    }
}
