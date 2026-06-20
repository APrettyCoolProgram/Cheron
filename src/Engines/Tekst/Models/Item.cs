// 260619_code
// 260619_documentation

namespace Tekst.Models;

/// <summary>An object that exists in a room or in the player's inventory.</summary>
public class Item
{
    /// <summary>Unique identifier used in command parsing (e.g., "torch").</summary>
    public required string Id { get; init; }

    /// <summary>Display name shown to the player (e.g., "a flickering torch").</summary>
    public required string Name { get; init; }

    /// <summary>Text returned when the player examines the item.</summary>
    public required string Description { get; init; }

    /// <summary>Whether the player can add this item to their inventory.</summary>
    public bool CanTake { get; init; } = true;
}
