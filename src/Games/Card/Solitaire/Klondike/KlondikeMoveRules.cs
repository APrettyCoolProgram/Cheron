// 261029_code
// 261029_documentation

using System.Collections.Generic;

namespace Cheron.Games.Card.Solitaire.Klondike;

/// <summary>Evaluates Klondike-specific move legality without mutating board state.</summary>
internal static class KlondikeMoveRules
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
}
