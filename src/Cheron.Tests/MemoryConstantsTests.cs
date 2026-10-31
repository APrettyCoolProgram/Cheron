// 261030_code
// 261030_documentation

using Cheron.Games.Memory;
using Xunit;

namespace Cheron.Tests;

public sealed class MemoryColorsTests
{
    [Fact]
    public void TileFronts_HasExactlyEightDistinctColors()
    {
        Assert.Equal(8, MemoryColors.TileFronts.Length);
        Assert.Equal(8, MemoryColors.TileFronts.Distinct().Count());
    }
}

public sealed class MemoryAudioConstantsTests
{
    [Fact]
    public void FeedbackFrequencyArrays_AreNonEmpty()
    {
        Assert.NotEmpty(MemoryAudioConstants.FlipFrequencies);
        Assert.NotEmpty(MemoryAudioConstants.MatchFrequencies);
        Assert.NotEmpty(MemoryAudioConstants.MismatchFrequencies);
        Assert.NotEmpty(MemoryAudioConstants.WinFrequencies);
    }

    [Fact]
    public void FeedbackSoundVolume_IsWithinValidRange()
    {
        Assert.InRange(MemoryAudioConstants.FeedbackSoundVolume, 0f, 1f);
    }
}

public sealed class MemoryFeedbackSoundTests
{
    [Fact]
    public void Enum_DefinesExpectedOrder()
    {
        Assert.Equal(0, (int)MemoryFeedbackSound.Flip);
        Assert.Equal(1, (int)MemoryFeedbackSound.Match);
        Assert.Equal(2, (int)MemoryFeedbackSound.Mismatch);
        Assert.Equal(3, (int)MemoryFeedbackSound.Win);
    }
}
