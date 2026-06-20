// 260619_code
// 260619_documentation

using Tekst.Cartridge;

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

    /// <summary>Maps an ItemData object into an Item object.</summary>
    /// <param name="item">The ItemData object to map.</param>
    /// <returns>A new Item object created from the provided data.</returns>
    public static Item MapItem(ItemData item) => new()
    {
        Id          = item.Id,
        Name        = item.Name,
        Description = item.Description,
        CanTake     = item.CanTake,
    };
}
