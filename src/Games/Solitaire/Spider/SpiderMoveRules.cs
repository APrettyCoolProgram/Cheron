// 261101_code
// 261101_documentation

using System.Collections.Generic;

namespace Cheron.Games.Solitaire.Spider;

/// <summary>Evaluates tableau-move legality under Spider rules.</summary>
internal static class SpiderMoveRules
{
    /// <summary>Checks whether a face-up, same-suit descending run can be moved to the destination tableau column.</summary>
    public static bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination)
    {
        if (selectedPile is null || selectedCardIndex < 0 || selectedCardIndex >= selectedPile.Count)
        {
            return false;
        }

        if (!IsMovableRun(selectedPile, selectedCardIndex))
        {
            return false;
        }

        var movingCard = selectedPile[selectedCardIndex];
        if (destination.Count == 0)
        {
            return true;
        }

        var destinationTop = destination[^1];
        return destinationTop.FaceUp && destinationTop.Rank == movingCard.Rank + 1;
    }

    /// <summary>Checks whether the selected suffix is a legal Spider run (face up, descending by one, same suit).</summary>
    public static bool IsMovableRun(List<SolitaireCard> pile, int selectedCardIndex)
    {
        if (selectedCardIndex < 0 || selectedCardIndex >= pile.Count)
        {
            return false;
        }

        for (var cardIndex = selectedCardIndex; cardIndex < pile.Count; cardIndex++)
        {
            if (!pile[cardIndex].FaceUp)
            {
                return false;
            }
        }

        for (var cardIndex = selectedCardIndex; cardIndex < pile.Count - 1; cardIndex++)
        {
            var current = pile[cardIndex];
            var next = pile[cardIndex + 1];
            if (current.Suit != next.Suit || current.Rank != next.Rank + 1)
            {
                return false;
            }
        }

        return true;
    }
}
