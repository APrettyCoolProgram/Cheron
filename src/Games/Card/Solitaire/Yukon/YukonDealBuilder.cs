// 261101_code
// 261101_documentation

using System.Collections.Generic;

namespace Cheron.Games.Card.Solitaire.Yukon;

/// <summary>Builds Yukon-specific deal state.</summary>
internal static class YukonDealBuilder
{
    /// <summary>Deals seven Yukon tableau columns and removes dealt cards from the source deck.</summary>
    /// <param name="deck">The shuffled source deck.</param>
    /// <param name="tableau">The tableau collection to populate.</param>
    public static void DealTableau(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau)
    {
        var columnCounts = new[] { 1, 6, 7, 8, 9, 10, 11 };

        for (var columnIndex = 0; columnIndex < columnCounts.Length; columnIndex++)
        {
            var pileCount = columnCounts[columnIndex];
            var tableauPile = new List<SolitaireCard>(pileCount);

            for (var cardOffset = 0; cardOffset < pileCount; cardOffset++)
            {
                var card = deck[^1];
                deck.RemoveAt(deck.Count - 1);
                card.FaceUp = cardOffset >= pileCount - 5;
                tableauPile.Add(card);
            }

            tableau.Add(tableauPile);
        }
    }
}
