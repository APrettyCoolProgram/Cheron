// 261028_code
// 261028_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Provides short and long rank labels used by card rendering and descriptions.</summary>
internal static class SolitaireCardLabels
{
    /// <summary>Gets the short rank text used for card faces.</summary>
    /// <param name="rank">The card rank from one (ace) through thirteen (king).</param>
    /// <returns>The short rank label.</returns>
    public static string GetShortRankLabel(int rank) => rank switch
    {
        1 => "A",
        11 => "J",
        12 => "Q",
        13 => "K",
        _ => rank.ToString()
    };

    /// <summary>Gets the spoken-style rank text used by keyboard descriptions.</summary>
    /// <param name="rank">The card rank from one (ace) through thirteen (king).</param>
    /// <returns>The long rank label.</returns>
    public static string GetLongRankLabel(int rank) => rank switch
    {
        1 => "ACE",
        11 => "JACK",
        12 => "QUEEN",
        13 => "KING",
        _ => rank.ToString()
    };
}
