// 260619_code
// 260619_documentation

using System.Text.Json;
using Tekst.Engine;
using Tekst.Models;
using Tekst.State;
using Tekst.World;

namespace Tekst.Cartridge;

/// <summary>Loads a <c>.cart</c> JSON file and creates the game objects needed to run a session.</summary>
public static class CartridgeLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>
    /// Loads the cartridge named by <paramref name="cartName"/> and returns the assembled world,
    /// initial game state, command processor, and raw cartridge data.
    /// </summary>
    /// <param name="cartName">The file name of the cartridge to load.</param>
    /// <returns>
    /// A tuple containing the constructed <see cref="GameWorld"/>, <see cref="GameState"/>,
    /// <see cref="CommandProcessor"/>, and <see cref="CartridgeData"/>.
    /// </returns>
    /// <exception cref="FileNotFoundException">Thrown when the .cart file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be deserialized or the starting room is missing.</exception>
    public static (GameWorld World, GameState State, CommandProcessor Processor, CartridgeData Data) Load(string cartName)
    {
        var cartPath = Path.Combine(AppContext.BaseDirectory, "GameCartridges", cartName);

        Console.WriteLine($"---- Looking for game cartridge at: {cartPath} ----");

        if (!File.Exists(cartPath))
        {
            throw new FileNotFoundException($"Game cartridge path not found: {cartPath}", cartPath);
        }

        var json = File.ReadAllText(cartPath);
        var data = JsonSerializer.Deserialize<CartridgeData>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize cartridge: {cartPath}");

        var rooms = data.Rooms.Select(MapRoom).ToList();
        var world = new GameWorld(rooms, data.StartingRoomId);

        var startRoom = world.GetRoom(data.StartingRoomId)
            ?? throw new InvalidOperationException($"StartingRoomId '{data.StartingRoomId}' not found in cartridge.");

        var state     = new GameState { CurrentRoom = startRoom };
        var processor = new CommandProcessor(world);

        return (world, state, processor, data);
    }

    /// <summary>Maps a RoomData object into a Room object.</summary>
    /// <param name="room">The RoomData object to map.</param>
    /// <returns>A new Room object created from the provided data.</returns>
    private static Room MapRoom(RoomData room) => new()
    {
        Id          = room.Id,
        Title       = room.Title,
        Description = room.Description,
        Items       = room.Items.Select(MapItem).ToList(),
        Exits       = room.Exits.Select(MapExit).ToList(),
    };

    /// <summary>Maps an ItemData object into an Item object.</summary>
    /// <param name="item">The ItemData object to map.</param>
    /// <returns>A new Item object created from the provided data.</returns>
    private static Item MapItem(ItemData item) => new()
    {
        Id          = item.Id,
        Name        = item.Name,
        Description = item.Description,
        CanTake     = item.CanTake,
    };

    /// <summary>Maps an ExitData object into an Exit object.</summary>
    /// <param name="exit">The ExitData object to map.</param>
    /// <returns>A new Exit object created from the provided data.</returns>
    private static Exit MapExit(ExitData exit) => new()
    {
        Direction       = exit.Direction,
        TargetRoomId    = exit.TargetRoomId,
        MoveDescription = exit.MoveDescription,
    };
}