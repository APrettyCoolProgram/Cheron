// 261031_code
// 261029_documentation

using System.Collections.Generic;

namespace Cheron.Games.Card.Solitaire;

/// <summary>Provides shared foundation initialization and move rules for all Solitaire variants.</summary>
internal static class SolitaireFoundationRules
{
    /// <summary>Initializes the four empty foundations for a new deal.</summary>
    /// <param name="foundations">The foundations collection to populate.</param>
    public static void InitializeFoundations(List<List<SolitaireCard>> foundations)
    {
        for (var foundationIndex = 0; foundationIndex < 4; foundationIndex++)
        {
            foundations.Add(new List<SolitaireCard>());
        }
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
}
