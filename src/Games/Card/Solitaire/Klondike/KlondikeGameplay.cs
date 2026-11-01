// 261029_code
// 261029_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Card.Solitaire.Klondike;

/// <summary>Provides Klondike-specific orchestration helpers that operate on shared Solitaire state collections.</summary>
internal static class KlondikeGameplay
{
    /// <summary>Checks whether a selected run can move to the destination tableau under Klondike rules.</summary>
    public static bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination)
    {
        if (selectedPile is null || ReferenceEquals(destination, selectedPile))
        {
            return false;
        }

        var movingCard = selectedPile[selectedCardIndex];
        return KlondikeMoveRules.CanMoveToTableau(movingCard, destination);
    }

    /// <summary>Returns the leading piles that are variant-specific for Klondike history serialization.</summary>
    public static IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste)
    {
        yield return stock;
        yield return waste;
    }

    /// <summary>Attempts to resolve a top-row click against Klondike stock and waste.</summary>
    public static bool TryHandleTopRowClick(Point clickPosition, Rectangle stockBounds, Rectangle wasteBounds,
        List<SolitaireCard> waste, Action drawFromStock, Action<List<SolitaireCard>> selectTop)
    {
        if (stockBounds.Contains(clickPosition))
        {
            drawFromStock();
            return true;
        }

        if (wasteBounds.Contains(clickPosition))
        {
            selectTop(waste);
            return true;
        }

        return false;
    }

    /// <summary>Attempts to hit-test the top waste card in Klondike.</summary>
    public static bool TryGetCardAtWaste(Point position, List<SolitaireCard> waste, Rectangle wasteBounds,
        out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds)
    {
        pile = waste;
        cardIndex = pile.Count - 1;
        bounds = wasteBounds;
        return cardIndex >= 0 && bounds.Contains(position);
    }

    /// <summary>Initializes a new Klondike deal from the shared shuffled deck.</summary>
    public static void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau,
        List<SolitaireCard> stock, List<List<SolitaireCard>> foundations)
    {
        KlondikeDealBuilder.DealTableau(deck, tableau);
        stock.AddRange(deck);
        SolitaireFoundationRules.InitializeFoundations(foundations);
    }
}
