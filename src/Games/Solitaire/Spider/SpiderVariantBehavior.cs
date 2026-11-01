// 261101_code
// 261101_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire.Spider;

/// <summary>Implements Spider-specific behavior for the shared Solitaire orchestrator.</summary>
internal sealed class SpiderVariantBehavior : ISolitaireVariantBehavior
{
    public void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau, List<SolitaireCard> stock,
        List<SolitaireCard> waste, List<List<SolitaireCard>> foundations, List<List<SolitaireCard>> freeCells)
    {
        SpiderGameplay.InitializeDeal(deck, tableau, stock, foundations);
    }

    public IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste,
        List<List<SolitaireCard>> freeCells) => SpiderGameplay.EnumerateLeadingPiles(stock, waste);

    public int GetTableauColumnX(int columnIndex, int tableauColumnCount) =>
        SpiderLayout.GetTableauColumnX(columnIndex, tableauColumnCount);

    public bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination,
        List<List<SolitaireCard>> tableau, List<List<SolitaireCard>> freeCells, List<SolitaireCard> stock,
        List<SolitaireCard> waste) => SpiderGameplay.CanMoveToTableau(selectedPile, selectedCardIndex, destination);

    public bool CanMoveToFreeCell(List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex) => false;

    public bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock) =>
        SpiderGameplay.TryHandleTopRowClick(clickPosition, freeCells, getFreeCellBounds, selectedPile, tryMoveToFreeCell,
            showInvalidMove, selectTop, getStockBounds, getWasteBounds, waste, drawFromStock);

    public bool TryDropToVariantTopRow(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, Func<int, bool> tryMoveToFreeCell) => false;

    public bool TryGetTopRowCardAt(Point position, List<SolitaireCard> waste, Func<Rectangle> getWasteBounds,
        List<List<SolitaireCard>> freeCells, Func<int, Rectangle> getFreeCellBounds,
        out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds) =>
        SpiderGameplay.TryGetTopRowCardAt(position, out pile, out cardIndex, out bounds);
}
