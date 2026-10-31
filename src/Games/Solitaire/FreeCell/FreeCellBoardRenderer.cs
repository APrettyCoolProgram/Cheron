// 261029_code
// 261029_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Cheron.Games.Solitaire.SolitaireColors;
using static Cheron.Games.Solitaire.SolitaireLayout;
using static Cheron.Games.Solitaire.SolitaireTextConstants;

namespace Cheron.Games.Solitaire.FreeCell;

/// <summary>Renders FreeCell-specific board elements using shared drawing delegates.</summary>
internal static class FreeCellBoardRenderer
{
    /// <summary>Draws FreeCell holding cells and their heading labels.</summary>
    internal static void DrawTopRow(List<List<SolitaireCard>> freeCells, int smallTextScale,
        Action<List<SolitaireCard>, Rectangle, bool> drawPile,
        Action<string, int, int, int, Color, bool> drawText)
    {
        for (var freeCellIndex = 0; freeCellIndex < freeCells.Count; freeCellIndex++)
        {
            var freeCellBounds = GetFreeCellBounds(freeCellIndex);
            drawPile(freeCells[freeCellIndex], freeCellBounds, false);
            if (freeCells[freeCellIndex].Count == 0)
            {
                drawText(BoardFreeCellLabel, freeCellBounds.X + 8, PileHeadingY, smallTextScale, BoardHeadingText,
                    false);
            }
        }

        drawText(BoardFreeCellsLabel, StockLabelX, PileHeadingY, smallTextScale, BoardHeadingText, false);
    }

    /// <summary>Draws highlight outlines for legal FreeCell holding-cell targets.</summary>
    internal static void DrawLegalCellTargets(int freeCellCount, Func<int, bool> canMoveToFreeCell,
        Action<Rectangle> drawTargetOutline)
    {
        for (var freeCellIndex = 0; freeCellIndex < freeCellCount; freeCellIndex++)
        {
            if (canMoveToFreeCell(freeCellIndex))
            {
                drawTargetOutline(GetFreeCellBounds(freeCellIndex));
            }
        }
    }
}
