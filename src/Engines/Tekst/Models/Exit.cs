// 260619_code
// 260619_documentation

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
}
