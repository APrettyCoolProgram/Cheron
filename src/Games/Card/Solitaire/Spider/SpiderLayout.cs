// 261101_code
// 261101_documentation

using System;

namespace Cheron.Games.Card.Solitaire.Spider;

/// <summary>Calculates Spider-specific tableau layout positions.</summary>
internal static class SpiderLayout
{
    /// <summary>Calculates the horizontal position for one Spider tableau column.</summary>
    /// <param name="columnIndex">The zero-based tableau index.</param>
    /// <param name="columnCount">The number of tableau columns in play.</param>
    /// <returns>The left edge of the tableau column in logical board coordinates.</returns>
    public static int GetTableauColumnX(int columnIndex, int columnCount)
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
