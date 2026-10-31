// 261031_code
// 261029_documentation

using System;
using System.Collections.Generic;

namespace Cheron.Games.Solitaire;

/// <summary>Builds shuffled deals and initializes board piles for a new game.</summary>
internal static class SolitaireDealBuilder
{
    /// <summary>Creates a shuffled standard deck using the provided random source.</summary>
    /// <param name="random">The random source used to shuffle cards.</param>
    /// <returns>A shuffled deck ordered from first draw at the end of the list.</returns>
    public static List<SolitaireCard> CreateShuffledDeck(Random random)
    {
        var deck = CreateOrderedDeck();

        Shuffle(deck, random);

        return deck;
    }

    /// <summary>Creates an ordered standard deck from Ace through King in each suit.</summary>
    /// <returns>The ordered deck.</returns>
    private static List<SolitaireCard> CreateOrderedDeck()
    {
        var deck = new List<SolitaireCard>(52);

        foreach (var suit in Enum.GetValues<SolitaireSuit>())
        {
            for (var rank = 1; rank <= 13; rank++)
            {
                deck.Add(new SolitaireCard(rank, suit));
            }
        }

        return deck;
    }

    /// <summary>Shuffles the deck in place using the Fisher-Yates algorithm.</summary>
    /// <param name="deck">The deck to shuffle.</param>
    /// <param name="random">The random source used to select swap positions.</param>
    private static void Shuffle(List<SolitaireCard> deck, Random random)
    {
        for (var cardIndex = deck.Count - 1; cardIndex > 0; cardIndex--)
        {
            var swapIndex = random.Next(cardIndex + 1);

            (deck[cardIndex], deck[swapIndex]) = (deck[swapIndex], deck[cardIndex]);
        }
    }
}
