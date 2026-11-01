// 261101_code
// 261101_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire.Spider;

/// <summary>Provides Spider-specific orchestration helpers that operate on shared Solitaire state collections.</summary>
internal static class SpiderGameplay
{
    /// <summary>Returns the leading piles that are variant-specific for Spider history serialization.</summary>
    public static IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste)
    {
        yield return stock;
        yield return waste;
    }

    /// <summary>Checks whether a selected run can move to a tableau column under Spider rules.</summary>
    public static bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination)
    {
        if (selectedPile is null || ReferenceEquals(destination, selectedPile))
        {
            return false;
        }

        return SpiderMoveRules.CanMoveToTableau(selectedPile, selectedCardIndex, destination);
    }

    /// <summary>Attempts to resolve top-row input for Spider. The current implementation does not expose stock-row actions.</summary>
    public static bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock) => false;

    /// <summary>Attempts to hit-test top-row cards for Spider. The current implementation has no top-row piles.</summary>
    public static bool TryGetTopRowCardAt(Point position, out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds)
    {
        pile = null;
        cardIndex = -1;
        bounds = Rectangle.Empty;
        return false;
    }

    /// <summary>Initializes a new Spider deal from a shared shuffled single deck.</summary>
    public static void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau,
        List<SolitaireCard> stock, List<List<SolitaireCard>> foundations)
    {
        var spiderDeck = SpiderDealBuilder.CreateDoubleDeck(deck);
        SpiderDealBuilder.DealTableau(spiderDeck, tableau);
        stock.AddRange(spiderDeck);
        SolitaireFoundationRules.InitializeFoundations(foundations);
        deck.Clear();
        deck.AddRange(spiderDeck);
    }
}
