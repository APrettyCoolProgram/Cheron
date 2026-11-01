// 261030_code
// 261030_documentation

using Cheron.Games.Tile.Memory;
using Xunit;

namespace Cheron.Tests;

public sealed class TileBoardTileTests
{
    [Fact]
    public void Constructor_SetsColorId_AndDefaultsFaceDownAndUnmatched()
    {
        var tile = new TileBoardTile(3);

        Assert.Equal(3, tile.ColorId);
        Assert.False(tile.IsFaceUp);
        Assert.False(tile.IsMatched);
    }

    [Fact]
    public void IsFaceUp_CanBeSetIndependently()
    {
        var tile = new TileBoardTile(0);

        tile.IsFaceUp = true;

        Assert.True(tile.IsFaceUp);
        Assert.False(tile.IsMatched);
    }

    [Fact]
    public void IsMatched_CanBeSetIndependently()
    {
        var tile = new TileBoardTile(0);

        tile.IsMatched = true;

        Assert.True(tile.IsMatched);
        Assert.False(tile.IsFaceUp);
    }
}
