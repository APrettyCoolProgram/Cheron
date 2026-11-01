// 261101_code
// 261101_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Cheron.Games.Solitaire.SolitaireColors;
using static Cheron.Games.Solitaire.SolitaireLayout;
using static Cheron.Games.Solitaire.SolitaireTextConstants;

namespace Cheron.Games.Solitaire.Klondike;

/// <summary>Renders Klondike-specific board elements using shared drawing delegates.</summary>
internal static class KlondikeBoardRenderer
{
    /// <summary>Draws Klondike stock/waste area and heading labels.</summary>
    internal static void DrawTopRow(List<SolitaireCard> stock, List<SolitaireCard> waste, bool largeText,
        int smallTextScale,
        Action<List<SolitaireCard>, Rectangle, bool> drawPile,
        Action<string, int, int, int, Color, bool> drawText)
    {
        var stockBounds = GetStockBounds();
        drawPile(stock, stockBounds, true);

        if (stock.Count == 0)
        {
            drawText(BoardDealLabel, largeText ? DealLabelLargeTextX : DealLabelX, DealLabelY, smallTextScale,
                BoardDealPromptText, false);
            drawText(BoardAgainLabel, largeText ? AgainLabelLargeTextX : AgainLabelX,
                largeText ? AgainLabelLargeTextY : AgainLabelY, smallTextScale, BoardDealPromptText, false);
        }

        drawPile(waste, GetWasteBounds(), false);
        drawText(BoardStockLabel, largeText ? StockLabelLargeTextX : StockLabelX, PileHeadingY, smallTextScale,
            BoardHeadingText, false);
        drawText(BoardWasteLabel, largeText ? WasteLabelLargeTextX : WasteLabelX, PileHeadingY, smallTextScale,
            BoardHeadingText, false);
    }
}
