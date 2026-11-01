// 261101_code
// 261101_documentation

using System.Collections.Generic;

namespace Cheron.Games.Solitaire.Spider;

/// <summary>Builds Spider-specific deal and pile initialization state.</summary>
internal static class SpiderDealBuilder
{
    /// <summary>Creates a two-deck Spider source from the shared shuffled single deck.</summary>
    public static List<SolitaireCard> CreateDoubleDeck(List<SolitaireCard> shuffledSingleDeck)
    {
        var doubleDeck = new List<SolitaireCard>(104);

        foreach (var card in shuffledSingleDeck)
        {
            doubleDeck.Add(new SolitaireCard(card.Rank, card.Suit));
            doubleDeck.Add(new SolitaireCard(card.Rank, card.Suit));
        }

        return doubleDeck;
    }

    /// <summary>Deals Spider tableau columns (6,6,6,6,5,5,5,5,5,5), flipping only each column's top card face up.</summary>
    public static void DealTableau(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau)
    {
        var columnSizes = new[] { 6, 6, 6, 6, 5, 5, 5, 5, 5, 5 };

        foreach (var size in columnSizes)
        {
            var pile = new List<SolitaireCard>(size);
            for (var cardIndex = 0; cardIndex < size; cardIndex++)
            {
                var card = deck[^1];
                deck.RemoveAt(deck.Count - 1);
                card.FaceUp = cardIndex == size - 1;
                pile.Add(card);
            }

            tableau.Add(pile);
        }
    }
}
