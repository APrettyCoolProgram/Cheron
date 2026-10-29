// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Defines variant-specific behavior consumed by the shared Solitaire orchestrator.</summary>
internal interface ISolitaireVariantBehavior
{
    /// <summary>Initializes a new deal for the variant.</summary>
    void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau, List<SolitaireCard> stock,
        List<SolitaireCard> waste, List<List<SolitaireCard>> foundations, List<List<SolitaireCard>> freeCells);

    /// <summary>Enumerates variant-specific leading piles used by history serialization.</summary>
    IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste,
        List<List<SolitaireCard>> freeCells);

    /// <summary>Gets one tableau column x-position for the variant.</summary>
    int GetTableauColumnX(int columnIndex, int tableauColumnCount);

    /// <summary>Checks whether a selection can move to a tableau column under variant rules.</summary>
    bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination,
        List<List<SolitaireCard>> tableau, List<List<SolitaireCard>> freeCells, List<SolitaireCard> stock,
        List<SolitaireCard> waste);

    /// <summary>Checks whether a selection can move to a FreeCell holding slot under variant rules.</summary>
    bool CanMoveToFreeCell(List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex);

    /// <summary>Handles variant-specific top-row click behavior.</summary>
    bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock);

    /// <summary>Handles variant-specific top-row drop behavior.</summary>
    bool TryDropToVariantTopRow(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, Func<int, bool> tryMoveToFreeCell);

    /// <summary>Handles variant-specific top-row card hit testing.</summary>
    bool TryGetTopRowCardAt(Point position, List<SolitaireCard> waste, Func<Rectangle> getWasteBounds,
        List<List<SolitaireCard>> freeCells, Func<int, Rectangle> getFreeCellBounds,
        out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds);
}
