// 261030_code
// 261030_documentation

using Cheron.Games.Memory;
using Xunit;

namespace Cheron.Tests;

public sealed class MemoryTileTests
{
    [Fact]
    public void Constructor_SetsColorId_AndDefaultsFaceDownAndUnmatched()
    {
        var tile = new MemoryTile(3);

        Assert.Equal(3, tile.ColorId);
        Assert.False(tile.IsFaceUp);
        Assert.False(tile.IsMatched);
    }

    [Fact]
    public void IsFaceUp_CanBeSetIndependently()
    {
        var tile = new MemoryTile(0);

        tile.IsFaceUp = true;

        Assert.True(tile.IsFaceUp);
        Assert.False(tile.IsMatched);
    }

    [Fact]
    public void IsMatched_CanBeSetIndependently()
    {
        var tile = new MemoryTile(0);

        tile.IsMatched = true;

        Assert.True(tile.IsMatched);
        Assert.False(tile.IsFaceUp);
    }
}
