// 261101_code
// 261101_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire.Yukon;

/// <summary>Implements Yukon-specific behavior for the shared Solitaire orchestrator.</summary>
internal sealed class YukonVariantBehavior : ISolitaireVariantBehavior
{
    public void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau, List<SolitaireCard> stock,
        List<SolitaireCard> waste, List<List<SolitaireCard>> foundations, List<List<SolitaireCard>> freeCells)
    {
        YukonGameplay.InitializeDeal(deck, tableau, foundations);
    }

    public IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste,
        List<List<SolitaireCard>> freeCells) => YukonGameplay.EnumerateLeadingPiles();

    public int GetTableauColumnX(int columnIndex, int tableauColumnCount) => YukonLayout.GetTableauColumnX(columnIndex);

    public bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination,
        List<List<SolitaireCard>> tableau, List<List<SolitaireCard>> freeCells, List<SolitaireCard> stock,
        List<SolitaireCard> waste) => YukonGameplay.CanMoveToTableau(selectedPile, selectedCardIndex, destination);

    public bool CanMoveToFreeCell(List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex) => false;

    public bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock) =>
        YukonGameplay.TryHandleTopRowClick(clickPosition, freeCells, getFreeCellBounds, selectedPile,
            tryMoveToFreeCell, showInvalidMove, selectTop, getStockBounds, getWasteBounds, waste, drawFromStock);

    public bool TryDropToVariantTopRow(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, Func<int, bool> tryMoveToFreeCell) => false;

    public bool TryGetTopRowCardAt(Point position, List<SolitaireCard> waste, Func<Rectangle> getWasteBounds,
        List<List<SolitaireCard>> freeCells, Func<int, Rectangle> getFreeCellBounds,
        out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds) =>
        YukonGameplay.TryGetTopRowCardAt(position, out pile, out cardIndex, out bounds);
}
