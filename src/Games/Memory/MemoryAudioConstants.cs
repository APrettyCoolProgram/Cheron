// 261030_code
// 261030_documentation

namespace Cheron.Games.Memory;

/// <summary>Centralizes fixed audio synthesis/playback values used by Memory feedback sounds.</summary>
internal static class MemoryAudioConstants
{
    /// <summary>The sample rate for synthesized feedback sounds, in hertz.</summary>
    internal const int FeedbackSoundSampleRate = 22050;

    /// <summary>The playback volume applied to cached feedback sound instances.</summary>
    internal const float FeedbackSoundVolume = 0.2f;

    /// <summary>The frequency used for the tile-flip cue.</summary>
    internal static readonly double[] FlipFrequencies = [392];

    /// <summary>The frequency pair used for the matched-pair cue.</summary>
    internal static readonly double[] MatchFrequencies = [523.25, 659.25];

    /// <summary>The frequency used for the mismatched-pair cue.</summary>
    internal static readonly double[] MismatchFrequencies = [164.81];

    /// <summary>The ascending frequencies used for the win cue.</summary>
    internal static readonly double[] WinFrequencies = [523.25, 659.25, 783.99, 1046.5];

    /// <summary>The per-note duration for the tile-flip cue, in seconds.</summary>
    internal const double FlipNoteSeconds = 0.06;

    /// <summary>The per-note duration for the matched-pair cue, in seconds.</summary>
    internal const double MatchNoteSeconds = 0.09;

    /// <summary>The per-note duration for the mismatched-pair cue, in seconds.</summary>
    internal const double MismatchNoteSeconds = 0.16;

    /// <summary>The per-note duration for the win cue, in seconds.</summary>
    internal const double WinNoteSeconds = 0.12;
}
