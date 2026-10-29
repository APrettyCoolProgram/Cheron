// 261029_code
// 261029_documentation

using System;

namespace Cheron.Games.Solitaire.FreeCell;

/// <summary>Calculates FreeCell-specific tableau layout positions.</summary>
internal static class FreeCellLayout
{
    /// <summary>Calculates the horizontal position for one FreeCell cascade column.</summary>
    /// <param name="columnIndex">The zero-based cascade index.</param>
    /// <param name="columnCount">The number of cascades in play.</param>
    /// <returns>The left edge of the cascade column in logical board coordinates.</returns>
    public static int GetCascadeColumnX(int columnIndex, int columnCount)
    {
        if (columnCount <= 1)
        {
            return SolitaireLayout.FirstColumnX;
        }

        var maxRight = SolitaireLayout.BoardWidth - 32 - SolitaireLayout.CardWidth;
        var availableSpan = Math.Max(0, maxRight - SolitaireLayout.FirstColumnX);
        var dynamicStep = availableSpan / (columnCount - 1);
        return SolitaireLayout.FirstColumnX + columnIndex * dynamicStep;
    }
}
