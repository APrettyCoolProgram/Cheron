// 261101_code
// 261101_documentation

using System.Collections.Generic;

namespace Cheron.Games.Solitaire.Yukon;

/// <summary>Evaluates tableau-move legality under Yukon rules.</summary>
internal static class YukonMoveRules
{
    /// <summary>Checks whether a face-up Yukon selection can be placed onto the destination tableau column.</summary>
    public static bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination)
    {
        if (selectedPile is null || selectedCardIndex < 0 || selectedCardIndex >= selectedPile.Count)
        {
            return false;
        }

        if (!IsMovableSelection(selectedPile, selectedCardIndex))
        {
            return false;
        }

        var movingCard = selectedPile[selectedCardIndex];
        if (destination.Count == 0)
        {
            return true;
        }

        var destinationTop = destination[^1];
        return destinationTop.FaceUp && destinationTop.Rank == movingCard.Rank + 1 &&
            destinationTop.IsRed != movingCard.IsRed;
    }

    /// <summary>Checks whether all cards in the selected suffix are face up.</summary>
    public static bool IsMovableSelection(List<SolitaireCard> pile, int selectedCardIndex)
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

        return true;
    }
}
