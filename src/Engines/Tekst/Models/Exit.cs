// 260619_code
// 260619_documentation

using Tekst.Cartridge;

namespace Tekst.Models;

/// <summary>A one-way directional link from one room to another.</summary>
public class Exit
{
    /// <summary>The direction keyword the player types (e.g., "north", "up").</summary>
    public required string Direction { get; init; }

    /// <summary>The Id of the destination <see cref="Room"/>.</summary>
    public required string TargetRoomId { get; init; }

    /// <summary>Optional description shown when the player moves through this exit.</summary>
    public string? MoveDescription { get; init; }

    /// <summary>Maps an ExitData object into an Exit object.</summary>
    /// <param name="exit">The ExitData object to map.</param>
    /// <returns>A new Exit object created from the provided data.</returns>
    public static Exit MapExit(ExitData exit) => new()
    {
        Direction       = exit.Direction,
        TargetRoomId    = exit.TargetRoomId,
        MoveDescription = exit.MoveDescription,
    };
}
