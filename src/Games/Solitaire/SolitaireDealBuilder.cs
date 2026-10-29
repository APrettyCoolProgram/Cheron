// 261028_code
// 261028_documentation

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

    /// <summary>Deals seven tableau columns from the end of the deck and removes dealt cards from it.</summary>
    /// <param name="deck">The deck to deal from.</param>
    /// <param name="tableau">The tableau collection to populate.</param>
    public static void DealTableau(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau)
    {
        for (var columnIndex = 0; columnIndex < 7; columnIndex++)
        {
            var tableauPile = new List<SolitaireCard>();
            for (var cardIndex = 0; cardIndex <= columnIndex; cardIndex++)
            {
                var card = deck[^1];
                deck.RemoveAt(deck.Count - 1);
                card.FaceUp = cardIndex == columnIndex;
                tableauPile.Add(card);
            }

            tableau.Add(tableauPile);
        }
    }

    /// <summary>Initializes the four empty foundations for a new deal.</summary>
    /// <param name="foundations">The foundations collection to populate.</param>
    public static void InitializeFoundations(List<List<SolitaireCard>> foundations)
    {
        for (var foundationIndex = 0; foundationIndex < 4; foundationIndex++)
        {
            foundations.Add(new List<Card>());
        }
    }

    /// <summary>Deals eight FreeCell cascades from the end of the deck and removes dealt cards from it.</summary>
    /// <param name="deck">The deck to deal from.</param>
    /// <param name="cascades">The FreeCell cascades collection to populate.</param>
    public static void DealFreeCellCascades(List<SolitaireCard> deck, List<List<SolitaireCard>> cascades)
    {
        for (var cascadeIndex = 0; cascadeIndex < 8; cascadeIndex++)
        {
            cascades.Add(new List<SolitaireCard>());
        }

        var dealIndex = 0;
        while (deck.Count > 0)
        {
            var card = deck[^1];
            deck.RemoveAt(deck.Count - 1);
            card.FaceUp = true;
            cascades[dealIndex % cascades.Count].Add(card);
            dealIndex++;
        }
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
