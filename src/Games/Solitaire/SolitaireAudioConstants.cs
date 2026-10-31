// 261029_code
// 261029_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes fixed audio synthesis/playback values used by Solitaire feedback sounds.</summary>
internal static class SolitaireAudioConstants
{
    /// <summary>The sample rate for synthesized feedback sounds, in hertz.</summary>
    internal const int FeedbackSoundSampleRate = 22050;

    /// <summary>The playback volume applied to cached feedback sound instances.</summary>
    internal const float FeedbackSoundVolume = 0.18f;

    /// <summary>The frequencies used for the standard move cue.</summary>
    internal static readonly double[] MoveFrequencies = [330];

    /// <summary>The frequency pair used for the foundation-advance cue.</summary>
    internal static readonly double[] FoundationFrequencies = [523.25, 659.25];

    /// <summary>The frequency used for the invalid-move cue.</summary>
    internal static readonly double[] InvalidFrequencies = [196];

    /// <summary>The ascending frequencies used for the win cue.</summary>
    internal static readonly double[] WinFrequencies = [523.25, 659.25, 783.99, 1046.5];

    /// <summary>The per-note duration for the standard move cue, in seconds.</summary>
    internal const double MoveNoteSeconds = 0.07;

    /// <summary>The per-note duration for the foundation-advance cue, in seconds.</summary>
    internal const double FoundationNoteSeconds = 0.08;

    /// <summary>The per-note duration for the invalid-move cue, in seconds.</summary>
    internal const double InvalidNoteSeconds = 0.1;

    /// <summary>The per-note duration for the win cue, in seconds.</summary>
    internal const double WinNoteSeconds = 0.12;
}
