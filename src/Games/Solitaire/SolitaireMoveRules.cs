// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;

namespace Cheron.Games.Solitaire;

/// <summary>Evaluates legal move targets for selected cards without mutating board state.</summary>
internal static class SolitaireMoveRules
{
    /// <summary>Checks whether a card can be placed onto a Klondike tableau column.</summary>
    /// <param name="movingCard">The selected card that would be placed.</param>
    /// <param name="destination">The destination tableau pile.</param>
    /// <returns><see langword="true"/> when the move is legal; otherwise, <see langword="false"/>.</returns>
    public static bool CanMoveToTableau(SolitaireCard movingCard, List<SolitaireCard> destination)
    {
        if (destination.Count == 0)
        {
            return movingCard.Rank == 13;
        }

        var destinationTop = destination[^1];
        return destinationTop.FaceUp && destinationTop.Rank == movingCard.Rank + 1 &&
            destinationTop.IsRed != movingCard.IsRed;
    }

    /// <summary>Checks whether a card can be placed onto a foundation pile.</summary>
    /// <param name="movingCard">The selected card that would be placed.</param>
    /// <param name="destination">The destination foundation pile.</param>
    /// <returns><see langword="true"/> when the move is legal; otherwise, <see langword="false"/>.</returns>
    public static bool CanMoveToFoundation(SolitaireCard movingCard, List<SolitaireCard> destination)
    {
        if (destination.Count == 0)
        {
            return movingCard.Rank == 1;
        }

        var destinationTop = destination[^1];
        return destinationTop.Suit == movingCard.Suit && destinationTop.Rank + 1 == movingCard.Rank;
    }

    /// <summary>Checks whether a card can be placed into a FreeCell holding cell.</summary>
    /// <param name="destination">The destination FreeCell holding pile.</param>
    /// <returns><see langword="true"/> when the cell is empty; otherwise, <see langword="false"/>.</returns>
    public static bool CanMoveToFreeCell(List<SolitaireCard> destination) => destination.Count == 0;

    /// <summary>Checks whether a card can be placed onto a FreeCell cascade.</summary>
    /// <param name="movingCard">The selected card that would be placed.</param>
    /// <param name="destination">The destination cascade pile.</param>
    /// <returns><see langword="true"/> when the move is legal; otherwise, <see langword="false"/>.</returns>
    public static bool CanMoveToFreeCellCascade(SolitaireCard movingCard, List<SolitaireCard> destination)
    {
        if (destination.Count == 0)
        {
            return true;
        }

        var destinationTop = destination[^1];
        return destinationTop.Rank == movingCard.Rank + 1 && destinationTop.IsRed != movingCard.IsRed;
    }

    /// <summary>Checks whether a selected run forms a valid descending, alternating-color FreeCell sequence.</summary>
    /// <param name="pile">The source cascade pile.</param>
    /// <param name="startIndex">The index of the first card in the selected run.</param>
    /// <returns><see langword="true"/> when the run is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsFreeCellMovableRun(List<SolitaireCard> pile, int startIndex)
    {
        if (startIndex < 0 || startIndex >= pile.Count)
        {
            return false;
        }

        for (var cardIndex = startIndex; cardIndex < pile.Count - 1; cardIndex++)
        {
            var current = pile[cardIndex];
            var next = pile[cardIndex + 1];
            if (current.Rank != next.Rank + 1 || current.IsRed == next.IsRed)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Checks whether enough free spaces exist to move a FreeCell run as one action.</summary>
    /// <param name="runLength">Number of cards being moved.</param>
    /// <param name="emptyFreeCells">Count of currently empty FreeCell holding cells.</param>
    /// <param name="emptyCascades">Count of currently empty cascades excluding the destination cascade when applicable.</param>
    /// <param name="destinationIsEmptyCascade">Whether destination cascade is empty.</param>
    /// <returns><see langword="true"/> when the run is within legal move capacity; otherwise, <see langword="false"/>.</returns>
    public static bool CanMoveFreeCellRun(int runLength, int emptyFreeCells, int emptyCascades,
        bool destinationIsEmptyCascade)
    {
        if (runLength <= 0)
        {
            return false;
        }

        var emptyCascadeFactor = Math.Max(0, destinationIsEmptyCascade ? emptyCascades - 1 : emptyCascades);
        var maxMovable = (emptyFreeCells + 1) * (1 << emptyCascadeFactor);
        return runLength <= maxMovable;
    }
}
