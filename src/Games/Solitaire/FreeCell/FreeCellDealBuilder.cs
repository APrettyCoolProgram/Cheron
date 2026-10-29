// 261029_code
// 261029_documentation

using System.Collections.Generic;

namespace Cheron.Games.Solitaire.FreeCell;

/// <summary>Builds FreeCell-specific deal and pile initialization state.</summary>
internal static class FreeCellDealBuilder
{
    /// <summary>Deals eight FreeCell cascades from the end of the deck and removes dealt cards from it.</summary>
    /// <param name="deck">The deck to deal from.</param>
    /// <param name="cascades">The FreeCell cascades collection to populate.</param>
    public static void DealCascades(List<SolitaireCard> deck, List<List<SolitaireCard>> cascades)
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

    /// <summary>Initializes the four FreeCell holding cells.</summary>
    /// <param name="freeCells">The holding-cell collection to populate.</param>
    public static void InitializeCells(List<List<SolitaireCard>> freeCells)
    {
        for (var freeCellIndex = 0; freeCellIndex < 4; freeCellIndex++)
        {
            freeCells.Add(new List<SolitaireCard>(1));
        }
    }
}
