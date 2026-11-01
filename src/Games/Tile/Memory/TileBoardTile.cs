// 261030_code
// 261030_documentation

namespace Cheron.Games.Tile.Memory;

/// <summary>Represents a single tile's matching state on the Memory board.</summary>
internal sealed class TileBoardTile
{
    /// <summary>Initializes a tile for the given color pair.</summary>
    /// <param name="colorId">The index into <see cref="TileColors.TileFronts"/> identifying this tile's pair.</param>
    public TileBoardTile(int colorId)
    {
        ColorId = colorId;
    }

    /// <summary>The index into <see cref="TileColors.TileFronts"/> identifying this tile's matching pair.</summary>
    public int ColorId { get; }

    /// <summary>Whether the tile is currently showing its solid-color front face.</summary>
    public bool IsFaceUp { get; set; }

    /// <summary>Whether the tile has been permanently matched and revealed.</summary>
    public bool IsMatched { get; set; }
}
