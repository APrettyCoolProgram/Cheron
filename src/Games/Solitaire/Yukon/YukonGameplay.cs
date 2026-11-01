// 261101_code
// 261101_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire.Yukon;

/// <summary>Provides Yukon-specific orchestration helpers that operate on shared Solitaire state collections.</summary>
internal static class YukonGameplay
{
    /// <summary>Returns the leading piles that are variant-specific for Yukon history serialization.</summary>
    public static IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles()
    {
        yield break;
    }

    /// <summary>Checks whether a selected face-up Yukon stack can move to the destination tableau column.</summary>
    public static bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination)
    {
        if (selectedPile is null || ReferenceEquals(destination, selectedPile))
        {
            return false;
        }

        return YukonMoveRules.CanMoveToTableau(selectedPile, selectedCardIndex, destination);
    }

    /// <summary>Attempts to resolve top-row input for Yukon. The variant has no top-row stock, waste, or cells.</summary>
    public static bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock) => false;

    /// <summary>Attempts to hit-test top-row cards for Yukon. The variant has no top-row piles.</summary>
    public static bool TryGetTopRowCardAt(Point position, out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds)
    {
        pile = null;
        cardIndex = -1;
        bounds = Rectangle.Empty;
        return false;
    }

    /// <summary>Initializes a new Yukon deal from the shared shuffled deck.</summary>
    public static void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau,
        List<List<SolitaireCard>> foundations)
    {
        YukonDealBuilder.DealTableau(deck, tableau);
        SolitaireFoundationRules.InitializeFoundations(foundations);
    }
}
