// 260621_code
// 260620_documentation

using System.Text.Json;
using Tekst.Catalog;
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
        ReadCommentHandling         = JsonCommentHandling.Skip,
        AllowTrailingCommas         = true,
    };

    /// <summary>
    /// Loads the cartridge named by <paramref name="cartName"/> and returns the assembled world,
    /// initial game state, command processor, and raw cartridge data.
    /// </summary>
    /// <param name="cartName">The file name of the cartridge to load.</param>
    /// <returns>
    /// A tuple containing the constructed <see cref="GameWorld"/>, <see cref="GameState"/>,
    /// <see cref="CommandProcessor"/>, and <see cref="CartridgeShell"/>.
    /// </returns>
    /// <exception cref="FileNotFoundException">Thrown when the .cart file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be deserialized or the starting room is missing.</exception>
    public static (GameWorld World, GameState State, CommandProcessor Processor, CartridgeShell Data) Load(string cartName)
    {
        var cartPath = Path.Combine(AppContext.BaseDirectory, "GameCartridges", cartName);

        ValidateCartridge(cartPath);

        CartridgeShell cartridgeShell = Deserialize(cartPath);

        List<Room> rooms = cartridgeShell.Rooms.ConvertAll(Room.MapRoom);
        GameWorld world  = new GameWorld(rooms, cartridgeShell.StartingRoomId);
        Room? startRoom  = world.GetRoom(cartridgeShell.StartingRoomId);
        GameState state  = new GameState
        {
            CurrentRoom = startRoom
        };
        CommandProcessor processor = new CommandProcessor(world);

        return (world, state, processor, cartridgeShell);
    }

    /// <summary>Checks if the cartridge file exists at the specified path. If it does not exist, a <see cref="FileNotFoundException"/> is thrown.</summary>
    /// <param name="cartPath">The path to the cartridge file.</param>
    /// <exception cref="FileNotFoundException">Thrown when the .cart file does not exist.</exception>
    private static void ValidateCartridge(string cartPath)
    {
        Console.WriteLine(msg_Cartridge.LookingFor(cartPath));

        if (!File.Exists(cartPath))
        {
            throw new FileNotFoundException(msg_Cartridge.NotFound(cartPath), cartPath);
        }
    }

    /// <summary>Reads the cartridge file from the specified path and deserializes it into a <see cref="CartridgeShell"/> object.</summary>
    /// <param name="cartPath">The path to the cartridge file.</param>
    /// <returns>The deserialized <see cref="CartridgeShell"/> object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be deserialized.</exception>
    private static CartridgeShell Deserialize(string cartPath)
    {
        var json = File.ReadAllText(cartPath);

        return JsonSerializer.Deserialize<CartridgeShell>(json, JsonOptions)
            ?? throw new InvalidOperationException(msg_Cartridge.DeserializationFailed(cartPath));
    }
}