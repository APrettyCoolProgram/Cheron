// 261028_code
// 261028_documentation

using System.Collections.Generic;

namespace Cheron.Games.Solitaire.Klondike;

/// <summary>Builds Klondike-specific deal state.</summary>
internal static class KlondikeDealBuilder
{
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
}
