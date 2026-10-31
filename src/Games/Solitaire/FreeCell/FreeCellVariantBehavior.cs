// 261029_code
// 261029_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire.FreeCell;

/// <summary>Implements FreeCell-specific behavior for the shared Solitaire orchestrator.</summary>
internal sealed class FreeCellVariantBehavior : ISolitaireVariantBehavior
{
    public void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau, List<SolitaireCard> stock,
        List<SolitaireCard> waste, List<List<SolitaireCard>> foundations, List<List<SolitaireCard>> freeCells)
    {
        FreeCellGameplay.InitializeDeal(deck, tableau, foundations, freeCells);
    }

    public IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste,
        List<List<SolitaireCard>> freeCells) => FreeCellGameplay.EnumerateLeadingPiles(freeCells);

    public int GetTableauColumnX(int columnIndex, int tableauColumnCount) =>
        FreeCellLayout.GetCascadeColumnX(columnIndex, tableauColumnCount);

    public bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination,
        List<List<SolitaireCard>> tableau, List<List<SolitaireCard>> freeCells, List<SolitaireCard> stock,
        List<SolitaireCard> waste) =>
        FreeCellGameplay.CanMoveToTableau(selectedPile, selectedCardIndex, destination, tableau, freeCells, stock, waste);

    public bool CanMoveToFreeCell(List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex) =>
        FreeCellGameplay.CanMoveToCell(SolitaireVariant.FreeCell, selectedPile, selectedCardIndex, freeCells, freeCellIndex);

    public bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock) =>
        FreeCellGameplay.TryHandleTopRowClick(clickPosition, freeCells, getFreeCellBounds, selectedPile,
            tryMoveToFreeCell, showInvalidMove, selectTop);

    public bool TryDropToVariantTopRow(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, Func<int, bool> tryMoveToFreeCell) =>
        FreeCellGameplay.TryDropToCell(position, freeCells, getFreeCellBounds, tryMoveToFreeCell);

    public bool TryGetTopRowCardAt(Point position, List<SolitaireCard> waste, Func<Rectangle> getWasteBounds,
        List<List<SolitaireCard>> freeCells, Func<int, Rectangle> getFreeCellBounds,
        out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds) =>
        FreeCellGameplay.TryGetCardAtCell(position, freeCells, getFreeCellBounds, out pile, out cardIndex, out bounds);
}
