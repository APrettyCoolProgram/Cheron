// 261029_code
// 261029_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Card.Solitaire.Klondike;

/// <summary>Implements Klondike-specific behavior for the shared Solitaire orchestrator.</summary>
internal sealed class KlondikeVariantBehavior : ISolitaireVariantBehavior
{
    public void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau, List<SolitaireCard> stock,
        List<SolitaireCard> waste, List<List<SolitaireCard>> foundations, List<List<SolitaireCard>> freeCells)
    {
        KlondikeGameplay.InitializeDeal(deck, tableau, stock, foundations);
    }

    public IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<SolitaireCard> stock, List<SolitaireCard> waste,
        List<List<SolitaireCard>> freeCells) => KlondikeGameplay.EnumerateLeadingPiles(stock, waste);

    public int GetTableauColumnX(int columnIndex, int tableauColumnCount) => KlondikeLayout.GetTableauColumnX(columnIndex);

    public bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination,
        List<List<SolitaireCard>> tableau, List<List<SolitaireCard>> freeCells, List<SolitaireCard> stock,
        List<SolitaireCard> waste) => KlondikeGameplay.CanMoveToTableau(selectedPile, selectedCardIndex, destination);

    public bool CanMoveToFreeCell(List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex) => false;

    public bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop, Func<Rectangle> getStockBounds,
        Func<Rectangle> getWasteBounds, List<SolitaireCard> waste, Action drawFromStock) =>
        KlondikeGameplay.TryHandleTopRowClick(clickPosition, getStockBounds(), getWasteBounds(), waste, drawFromStock,
            selectTop);

    public bool TryDropToVariantTopRow(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, Func<int, bool> tryMoveToFreeCell) => false;

    public bool TryGetTopRowCardAt(Point position, List<SolitaireCard> waste, Func<Rectangle> getWasteBounds,
        List<List<SolitaireCard>> freeCells, Func<int, Rectangle> getFreeCellBounds,
        out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds) =>
        KlondikeGameplay.TryGetCardAtWaste(position, waste, getWasteBounds(), out pile, out cardIndex, out bounds);
}
