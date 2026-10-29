// 261028_code
// 261028_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Stores a playing card's identity and face-up state.</summary>
/// <param name="rank">The card rank, using 1 for Ace through 13 for King.</param>
/// <param name="suit">The card's suit.</param>
internal sealed class Card(int rank, Suit suit)
{
    /// <summary>Gets the card rank, using 1 for Ace through 13 for King.</summary>
    public int Rank { get; } = rank;

    /// <summary>Gets the card's suit.</summary>
    public Suit Suit { get; } = suit;

    /// <summary>Gets or sets whether the card's face is visible.</summary>
    public bool FaceUp { get; set; }

    /// <summary>Gets whether the card belongs to a red suit.</summary>
    public bool IsRed => Suit is Suit.Diamonds or Suit.Hearts;
}
