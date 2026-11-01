// 261030_code
// 261030_documentation

namespace Cheron.Games.Tile.Memory;

/// <summary>Identifies the cached sound associated with a player-visible Memory outcome.</summary>
internal enum TileFeedbackSound
{
    /// <summary>A soft tone played when a tile is flipped face up.</summary>
    Flip,

    /// <summary>A rising pair of tones played when two flipped tiles match.</summary>
    Match,

    /// <summary>A quiet low tone played when two flipped tiles do not match.</summary>
    Mismatch,

    /// <summary>A short ascending phrase played once every pair has been matched.</summary>
    Win
}
