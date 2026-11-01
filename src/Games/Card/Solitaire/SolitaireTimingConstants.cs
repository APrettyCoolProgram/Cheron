// 261029_code
// 261029_documentation

using System;

namespace Cheron.Games.Card.Solitaire;

/// <summary>Centralizes timing values used by Solitaire feedback and animation behavior.</summary>
internal static class SolitaireTimingConstants
{
    /// <summary>The lifetime of rejected-move outlines and instructions.</summary>
    internal static readonly TimeSpan InvalidFeedbackDuration = TimeSpan.FromSeconds(1.4);

    /// <summary>The duration of the nonblocking win celebration.</summary>
    internal static readonly TimeSpan CelebrationDuration = TimeSpan.FromSeconds(3);

    /// <summary>The duration of an ordinary card slide or flip, in seconds.</summary>
    internal const float MoveDuration = 0.22f;

    /// <summary>The fade-in duration of the card selection highlight, in seconds.</summary>
    internal const float SelectionHighlightFadeDuration = 0.12f;
}
