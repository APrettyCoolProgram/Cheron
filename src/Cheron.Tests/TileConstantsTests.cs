// 261030_code
// 261030_documentation

using Cheron.Games.Tile.Memory;
using Xunit;

namespace Cheron.Tests;

public sealed class TileColorsTests
{
    [Fact]
    public void TileFronts_HasExactlyEightDistinctColors()
    {
        Assert.Equal(8, TileColors.TileFronts.Length);
        Assert.Equal(8, TileColors.TileFronts.Distinct().Count());
    }
}

public sealed class TileAudioConstantsTests
{
    [Fact]
    public void FeedbackFrequencyArrays_AreNonEmpty()
    {
        Assert.NotEmpty(TileAudioConstants.FlipFrequencies);
        Assert.NotEmpty(TileAudioConstants.MatchFrequencies);
        Assert.NotEmpty(TileAudioConstants.MismatchFrequencies);
        Assert.NotEmpty(TileAudioConstants.WinFrequencies);
    }

    [Fact]
    public void FeedbackSoundVolume_IsWithinValidRange()
    {
        Assert.InRange(TileAudioConstants.FeedbackSoundVolume, 0f, 1f);
    }
}

public sealed class TileFeedbackSoundTests
{
    [Fact]
    public void Enum_DefinesExpectedOrder()
    {
        Assert.Equal(0, (int)TileFeedbackSound.Flip);
        Assert.Equal(1, (int)TileFeedbackSound.Match);
        Assert.Equal(2, (int)TileFeedbackSound.Mismatch);
        Assert.Equal(3, (int)TileFeedbackSound.Win);
    }
}
