// 261028_code
// 261028_documentation

using System.Collections.Generic;

namespace Cheron.Games.Solitaire;

/// <summary>Evaluates legal move targets for selected cards without mutating board state.</summary>
internal static class MoveRules
{
    /// <summary>Checks whether a card can be placed onto a tableau column.</summary>
    /// <param name="movingCard">The selected card that would be placed.</param>
    /// <param name="destination">The destination tableau pile.</param>
    /// <returns><see langword="true"/> when the move is legal; otherwise, <see langword="false"/>.</returns>
    public static bool CanMoveToTableau(Card movingCard, List<Card> destination)
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
    public static bool CanMoveToFoundation(Card movingCard, List<Card> destination)
    {
        if (destination.Count == 0)
        {
            return movingCard.Rank == 1;
        }

        var destinationTop = destination[^1];
        return destinationTop.Suit == movingCard.Suit && destinationTop.Rank + 1 == movingCard.Rank;
    }
}
