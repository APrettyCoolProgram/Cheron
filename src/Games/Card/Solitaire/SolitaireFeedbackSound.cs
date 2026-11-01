// 261029_code
// 261029_documentation

namespace Cheron.Games.Card.Solitaire;

/// <summary>Identifies the cached sound associated with a player-visible outcome.</summary>
internal enum FeedbackSound
{
    /// <summary>A soft tone for drawing, recycling, moving, and undoing cards.</summary>
    Move,

    /// <summary>A rising pair of tones for advancing a foundation.</summary>
    Foundation,

    /// <summary>A quiet low tone for a rejected move.</summary>
    Invalid,

    /// <summary>A short ascending phrase after the final card settles.</summary>
    Win
}
