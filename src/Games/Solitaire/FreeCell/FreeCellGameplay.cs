// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire.FreeCell;

/// <summary>Provides FreeCell-specific orchestration helpers that operate on shared Solitaire state collections.</summary>
internal static class FreeCellGameplay
{
    /// <summary>Returns the leading piles that are variant-specific for FreeCell history serialization.</summary>
    /// <param name="freeCells">The FreeCell holding cells.</param>
    /// <returns>The leading pile sequence for FreeCell.</returns>
    public static IEnumerable<List<SolitaireCard>> EnumerateLeadingPiles(List<List<SolitaireCard>> freeCells) => freeCells;

    /// <summary>Checks whether a selected run can move to the destination cascade under FreeCell rules.</summary>
    public static bool CanMoveToTableau(List<SolitaireCard>? selectedPile, int selectedCardIndex, List<SolitaireCard> destination,
        List<List<SolitaireCard>> tableau, List<List<SolitaireCard>> freeCells, List<SolitaireCard> stock,
        List<SolitaireCard> waste)
    {
        if (selectedPile is null || ReferenceEquals(destination, selectedPile))
        {
            return false;
        }

        var movingCard = selectedPile[selectedCardIndex];
        if (!FreeCellMoveRules.CanMoveToCascade(movingCard, destination))
        {
            return false;
        }

        if (ReferenceEquals(selectedPile, waste) || ReferenceEquals(selectedPile, stock))
        {
            return false;
        }

        if (tableau.Contains(selectedPile) && !FreeCellMoveRules.IsMovableRun(selectedPile, selectedCardIndex))
        {
            return false;
        }

        var runLength = selectedPile.Count - selectedCardIndex;
        var emptyFreeCells = freeCells.Count(cell => cell.Count == 0);
        var emptyCascades = tableau.Count(pile => pile.Count == 0);
        var destinationIsEmpty = destination.Count == 0;
        return FreeCellMoveRules.CanMoveRun(runLength, emptyFreeCells, emptyCascades, destinationIsEmpty);
    }

    /// <summary>Checks whether a selection can move into one FreeCell holding cell.</summary>
    public static bool CanMoveToCell(SolitaireVariant variant, List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex)
    {
        if (!variant.IsFreeCell() || selectedPile is null)
        {
            return false;
        }

        var destination = freeCells[freeCellIndex];
        if (ReferenceEquals(destination, selectedPile))
        {
            return false;
        }

        return selectedCardIndex == selectedPile.Count - 1 && FreeCellMoveRules.CanMoveToCell(destination);
    }

    /// <summary>Attempts to resolve a top-row click against FreeCell holding cells.</summary>
    public static bool TryHandleTopRowClick(Point clickPosition, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, List<SolitaireCard>? selectedPile, Func<int, bool> tryMoveToFreeCell,
        Action<Point> showInvalidMove, Action<List<SolitaireCard>> selectTop)
    {
        for (var freeCellIndex = 0; freeCellIndex < freeCells.Count; freeCellIndex++)
        {
            if (!getFreeCellBounds(freeCellIndex).Contains(clickPosition))
            {
                continue;
            }

            if (selectedPile is not null)
            {
                if (tryMoveToFreeCell(freeCellIndex))
                {
                    return true;
                }

                if (!ReferenceEquals(selectedPile, freeCells[freeCellIndex]))
                {
                    showInvalidMove(clickPosition);
                }
            }

            selectTop(freeCells[freeCellIndex]);
            return true;
        }

        return false;
    }

    /// <summary>Attempts to drop selected cards onto FreeCell holding cells.</summary>
    public static bool TryDropToCell(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, Func<int, bool> tryMoveToFreeCell)
    {
        for (var freeCellIndex = 0; freeCellIndex < freeCells.Count; freeCellIndex++)
        {
            if (getFreeCellBounds(freeCellIndex).Contains(position) && tryMoveToFreeCell(freeCellIndex))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Attempts to hit-test top cards in FreeCell holding cells.</summary>
    public static bool TryGetCardAtCell(Point position, List<List<SolitaireCard>> freeCells,
        Func<int, Rectangle> getFreeCellBounds, out List<SolitaireCard>? pile, out int cardIndex, out Rectangle bounds)
    {
        for (var freeCellIndex = 0; freeCellIndex < freeCells.Count; freeCellIndex++)
        {
            pile = freeCells[freeCellIndex];
            cardIndex = pile.Count - 1;
            bounds = getFreeCellBounds(freeCellIndex);
            if (cardIndex >= 0 && bounds.Contains(position))
            {
                return true;
            }
        }

        pile = null;
        cardIndex = -1;
        bounds = Rectangle.Empty;
        return false;
    }

    /// <summary>Initializes a new FreeCell deal from the shared shuffled deck.</summary>
    public static void InitializeDeal(List<SolitaireCard> deck, List<List<SolitaireCard>> tableau,
        List<List<SolitaireCard>> foundations, List<List<SolitaireCard>> freeCells)
    {
        FreeCellDealBuilder.DealCascades(deck, tableau);
        SolitaireFoundationRules.InitializeFoundations(foundations);
        FreeCellDealBuilder.InitializeCells(freeCells);
    }

    /// <summary>Attempts to move a selected card to one FreeCell holding cell.</summary>
    public static bool TryMoveToCell(SolitaireVariant variant, List<SolitaireCard>? selectedPile, int selectedCardIndex,
        List<List<SolitaireCard>> freeCells, int freeCellIndex, Func<int, bool> canMoveToCell,
        Action moveSelectedCards, Action clearSelection)
    {
        if (!variant.IsFreeCell() || selectedPile is null)
        {
            return false;
        }

        var destination = freeCells[freeCellIndex];
        if (ReferenceEquals(selectedPile, destination))
        {
            clearSelection();
            return true;
        }

        if (!canMoveToCell(freeCellIndex))
        {
            return false;
        }

        moveSelectedCards();
        return true;
    }
}
