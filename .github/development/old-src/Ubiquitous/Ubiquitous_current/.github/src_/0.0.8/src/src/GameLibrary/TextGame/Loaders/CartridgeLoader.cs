using System.Text.Json;
using TextGame.Models;

namespace TextGame.Loaders;

/// <summary>Loads text adventure games from JSON cartridge files</summary>
public static class CartridgeLoader
{
    /// <summary>Loads a game from a JSON cartridge file</summary>
    /// <param name="cartridgePath">Path to the JSON cartridge file</param>
    /// <returns>The starting room of the game, or null if loading fails</returns>
    /// <remarks>
    /// This method:
    /// 1. Reads and deserializes the JSON file
    /// 2. Validates the cartridge data
    /// 3. Creates all Room and Item objects
    /// 4. Establishes room connections
    /// 5. Places items in rooms
    /// 6. Returns the starting room
    /// 
    /// If any step fails, the method returns null and prints error details to Console.
    /// </remarks>
    public static Room? LoadFromJson(string cartridgePath)
    {
        try
        {
            if (!File.Exists(cartridgePath))
            {
                Console.WriteLine($"Cartridge file not found: {cartridgePath}");
                return null;
            }

            string jsonContent = File.ReadAllText(cartridgePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            var cartridge = JsonSerializer.Deserialize<CartridgeData>(jsonContent, options);
            
            if (cartridge == null)
            {
                Console.WriteLine("Failed to deserialize cartridge data");
                return null;
            }

            return BuildGameWorld(cartridge);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cartridge: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads only the metadata from a cartridge file without building the game world
    /// </summary>
    /// <param name="cartridgePath">Path to the JSON cartridge file</param>
    /// <returns>The CartridgeData metadata object, or null if loading fails</returns>
    /// <remarks>
    /// This is useful for browsing available cartridges without building the full game.
    /// It allows quick access to title, description, author, and version information.
    /// </remarks>
    public static CartridgeData? LoadCartridgeMetadata(string cartridgePath)
    {
        try
        {
            if (!File.Exists(cartridgePath))
            {
                Console.WriteLine($"Cartridge file not found: {cartridgePath}");
                return null;
            }

            string jsonContent = File.ReadAllText(cartridgePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            return JsonSerializer.Deserialize<CartridgeData>(jsonContent, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cartridge metadata: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Finds all cartridge files in the specified directory
    /// </summary>
    /// <param name="cartridgeDirectory">Directory to search for cartridges</param>
    /// <returns>List of cartridge file paths</returns>
    /// <remarks>
    /// Searches recursively for all .json files in the specified directory.
    /// If the directory doesn't exist, it will be created and an empty list returned.
    /// </remarks>
    public static List<string> FindCartridges(string cartridgeDirectory)
    {
        try
        {
            if (!Directory.Exists(cartridgeDirectory))
            {
                Directory.CreateDirectory(cartridgeDirectory);
                return new List<string>();
            }

            return Directory.GetFiles(cartridgeDirectory, "*.json", SearchOption.AllDirectories).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finding cartridges: {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Builds the game world from cartridge data
    /// </summary>
    /// <param name="cartridge">The deserialized cartridge data</param>
    /// <returns>The starting room, or null if building fails</returns>
    private static Room? BuildGameWorld(CartridgeData cartridge)
    {
        if (ValidateCartridge(cartridge) == false)
        {
            return null;
        }

        var roomLookup = new Dictionary<string, Room>();
        var itemLookup = new Dictionary<string, Item>();

        foreach (var itemData in cartridge.Items)
        {
            var item = new Item(
                itemData.Name,
                itemData.Description,
                itemData.CanTake,
                itemData.UseDescription
            );
            itemLookup[itemData.Id] = item;
        }

        foreach (var roomData in cartridge.Rooms)
        {
            var room = new Room(roomData.Name, roomData.Description);
            roomLookup[roomData.Id] = room;
        }

        foreach (var roomData in cartridge.Rooms)
        {
            var room = roomLookup[roomData.Id];
            
            foreach (var exitData in roomData.Exits)
            {
                if (roomLookup.TryGetValue(exitData.DestinationRoomId, out var destinationRoom))
                {
                    room.AddExit(exitData.Direction, destinationRoom);
                }
                else
                {
                    Console.WriteLine($"Warning: Exit in room '{roomData.Id}' references non-existent room '{exitData.DestinationRoomId}'");
                }
            }

            foreach (var itemId in roomData.ItemIds)
            {
                if (itemLookup.TryGetValue(itemId, out var item))
                {
                    room.AddItem(item);
                }
                else
                {
                    Console.WriteLine($"Warning: Room '{roomData.Id}' references non-existent item '{itemId}'");
                }
            }
        }

        if (!roomLookup.TryGetValue(cartridge.StartingRoomId, out var startingRoom))
        {
            Console.WriteLine($"Error: Starting room '{cartridge.StartingRoomId}' not found");
            return null;
        }

        Console.WriteLine($"Loaded cartridge: {cartridge.Title} by {cartridge.Author}");
        return startingRoom;
    }

    /// <summary>Validates cartridge data for required fields and consistency</summary>
    /// <param name="cartridge">The cartridge data to validate</param>
    /// <returns>true if validation passes; otherwise false</returns>
    private static bool ValidateCartridge(CartridgeData cartridge)
    {
        if (string.IsNullOrWhiteSpace(cartridge.Title))
        {
            Console.WriteLine("Validation error: Title is required");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cartridge.StartingRoomId))
        {
            Console.WriteLine("Validation error: StartingRoomId is required");
            return false;
        }

        if (cartridge.Rooms.Count == 0)
        {
            Console.WriteLine("Validation error: At least one room is required");
            return false;
        }

        var roomIds = new HashSet<string>();
        foreach (var room in cartridge.Rooms)
        {
            if (string.IsNullOrWhiteSpace(room.Id))
            {
                Console.WriteLine("Validation error: All rooms must have an Id");
                return false;
            }

            if (!roomIds.Add(room.Id))
            {
                Console.WriteLine($"Validation error: Duplicate room Id '{room.Id}'");
                return false;
            }
        }

        var itemIds = new HashSet<string>();
        foreach (var item in cartridge.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                Console.WriteLine("Validation error: All items must have an Id");
                return false;
            }

            if (!itemIds.Add(item.Id))
            {
                Console.WriteLine($"Validation error: Duplicate item Id '{item.Id}'");
                return false;
            }
        }

        if (!roomIds.Contains(cartridge.StartingRoomId))
        {
            Console.WriteLine($"Validation error: StartingRoomId '{cartridge.StartingRoomId}' does not match any room");
            return false;
        }

        return true;
    }
}
