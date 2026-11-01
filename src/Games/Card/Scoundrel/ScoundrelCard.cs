namespace Cheron.Games.Card.Scoundrel;

/// <summary>Represents a card in the Scoundrel game.</summary>
public sealed class ScoundrelCard
{
    /// <summary>Gets the rank of the card (1 = Ace, 11 = Jack, 12 = Queen, 13 = King).</summary>
    public int Rank { get; }

    /// <summary>Gets the suit of the card.</summary>
    public ScoundrelSuit Suit { get; }

    /// <summary>Initializes a new card.</summary>
    public ScoundrelCard(int rank, ScoundrelSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }

    /// <summary>Gets the card role used by Scoundrel rules.</summary>
    public ScoundrelCardType GetCardType() => ScoundrelRules.GetCardType(this);

    /// <summary>Gets the card value used by Scoundrel rules.</summary>
    public int GetValue() => ScoundrelRules.GetValue(this);
}
