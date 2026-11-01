// 261031_code
// 261031_documentation

using System;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Tile.Memory;

/// <summary>Defines the board, toolbar, footer, and tile geometry for the Memory game.</summary>
internal static class TileLayout
{
    /// <summary>The smallest selectable board dimension (columns and rows).</summary>
    public const int MinGridSize = 4;

    /// <summary>The largest selectable board dimension (columns and rows).</summary>
    public const int MaxGridSize = 12;

    /// <summary>The default number of tile columns and rows on a freshly started board.</summary>
    public const int DefaultGridSize = 4;

    /// <summary>The side length, in logical pixels, of each square 1:1 tile.</summary>
    public const int TileSize = 120;

    /// <summary>The gap, in logical pixels, between adjacent tiles.</summary>
    public const int TileSpacing = 16;

    /// <summary>The corner radius, in logical pixels, applied to every rounded tile.</summary>
    public const int CornerRadius = 16;

    /// <summary>The horizontal and vertical offset, in logical pixels, of a tile's drop shadow.</summary>
    public const int ShadowOffset = 6;

    /// <summary>The margin, in logical pixels, around every edge of the tile grid.</summary>
    public const int BoardMargin = 12;

    /// <summary>The height of the top toolbar strip.</summary>
    public const int TopBarHeight = 84;

    /// <summary>The height of the bottom footer strip.</summary>
    public const int FooterHeight = 48;

    /// <summary>The width of each toolbar button.</summary>
    public const int ButtonWidth = 112;

    /// <summary>The height of each toolbar button.</summary>
    public const int ButtonHeight = 48;

    /// <summary>The horizontal gap between adjacent toolbar buttons.</summary>
    public const int ButtonGap = 10;

    /// <summary>The margin from the right edge of the toolbar to the first button.</summary>
    public const int ButtonRightMargin = 14;

    /// <summary>The corner radius, in logical pixels, applied to toolbar buttons and overlay panels.</summary>
    public const int ButtonCornerRadius = 10;

    /// <summary>Gets the maximum playable horizontal span used to size adaptive boards.</summary>
    private static int MaxGridSpan => DefaultGridSize * TileSize + (DefaultGridSize - 1) * TileSpacing;

    /// <summary>Computes the logical width of the Memory window canvas for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static int GetBoardWidth(int gridSize) => GetBoardWidth(gridSize, gridSize);

    /// <summary>Computes the logical width of the Memory window canvas for the current board size.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static int GetBoardWidth(int columns, int rows) => BoardMargin * 2 + GetGridWidth(columns, rows);

    /// <summary>Computes the logical height of the tile grid area, excluding the toolbar and footer, for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static int GetGridHeight(int gridSize) => GetGridHeight(gridSize, gridSize);

    /// <summary>Computes the logical height of the game window canvas for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static int GetBoardHeight(int gridSize) => GetBoardHeight(gridSize, gridSize);

    /// <summary>Computes the logical height of the game window canvas for the current board size.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static int GetBoardHeight(int columns, int rows) =>
        TopBarHeight + BoardMargin + GetGridHeight(columns, rows) + BoardMargin + FooterHeight;

    /// <summary>Gets the side length used for each tile for the current board dimensions.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static int GetTileSize(int columns, int rows)
    {
        var widthLimited = (MaxGridSpan - (columns - 1) * TileSpacing) / columns;
        var heightLimited = (MaxGridSpan - (rows - 1) * TileSpacing) / rows;
        return Math.Max(24, Math.Min(TileSize, Math.Min(widthLimited, heightLimited)));
    }

    /// <summary>Computes the on-screen bounds of the tile at the given grid column and row.</summary>
    /// <param name="column">The zero-based column index.</param>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetTileBounds(int column, int row, int columns, int rows)
    {
        var tileSize = GetTileSize(columns, rows);
        var gridWidth = GetGridWidth(columns, rows);
        var startX = (GetBoardWidth(columns, rows) - gridWidth) / 2;
        var startY = TopBarHeight + BoardMargin;
        var x = startX + column * (tileSize + TileSpacing);
        var y = startY + row * (tileSize + TileSpacing);
        return new Rectangle(x, y, tileSize, tileSize);
    }

    /// <summary>Computes the full tile-grid width for the given board dimensions.</summary>
    private static int GetGridWidth(int columns, int rows)
    {
        var tileSize = GetTileSize(columns, rows);
        return columns * tileSize + (columns - 1) * TileSpacing;
    }

    /// <summary>Computes the full tile-grid height for the given board dimensions.</summary>
    private static int GetGridHeight(int columns, int rows)
    {
        var tileSize = GetTileSize(columns, rows);
        return rows * tileSize + (rows - 1) * TileSpacing;
    }

    /// <summary>Gets the bounds of the top toolbar strip for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static Rectangle GetToolbarBounds(int gridSize) => GetToolbarBounds(gridSize, gridSize);

    /// <summary>Gets the bounds of the top toolbar strip.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetToolbarBounds(int columns, int rows) => new(0, 0, GetBoardWidth(columns, rows), TopBarHeight);

    /// <summary>Gets the bounds of the bottom footer strip.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetFooterBounds(int columns, int rows) =>
        new(0, GetBoardHeight(columns, rows) - FooterHeight, GetBoardWidth(columns, rows), FooterHeight);

    /// <summary>Gets the bounds of the New Game action area, left-aligned in the toolbar, for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static Rectangle GetNewGameButtonBounds(int gridSize) => GetNewGameButtonBounds(gridSize, gridSize);

    /// <summary>Gets the bounds of the New Game action area, left-aligned in the toolbar.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetNewGameButtonBounds(int columns, int rows)
    {
        var x = ButtonRightMargin;
        var y = (TopBarHeight - ButtonHeight) / 2;
        return new Rectangle(x, y, ButtonWidth, ButtonHeight);
    }

    /// <summary>Gets the bounds of the Sound toggle action area, right-aligned in the toolbar, for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static Rectangle GetSoundButtonBounds(int gridSize) => GetSoundButtonBounds(gridSize, gridSize);

    /// <summary>Gets the bounds of the Sound toggle action area, right-aligned in the toolbar.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetSoundButtonBounds(int columns, int rows)
    {
        var x = GetBoardWidth(columns, rows) - ButtonRightMargin - ButtonWidth;
        var y = (TopBarHeight - ButtonHeight) / 2;
        return new Rectangle(x, y, ButtonWidth, ButtonHeight);
    }

    /// <summary>Gets the bounds of the Help action area, right-aligned in the footer, for a square board.</summary>
    /// <param name="gridSize">The number of tile columns and rows on the board.</param>
    public static Rectangle GetHelpButtonBounds(int gridSize) => GetHelpButtonBounds(gridSize, gridSize);

    /// <summary>Gets the bounds of the Help action area, right-aligned in the footer.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetHelpButtonBounds(int columns, int rows)
    {
        var footerBounds = GetFooterBounds(columns, rows);
        var x = footerBounds.Right - ButtonRightMargin - ButtonWidth;
        var y = footerBounds.Y + (footerBounds.Height - ButtonHeight) / 2;
        return new Rectangle(x, y, ButtonWidth, ButtonHeight);
    }

    /// <summary>Gets the bounds of the centered help/win/size-prompt overlay panel.</summary>
    /// <param name="columns">The number of tile columns on the board.</param>
    /// <param name="rows">The number of tile rows on the board.</param>
    public static Rectangle GetOverlayPanelBounds(int columns, int rows)
    {
        const int panelWidth = 560;
        const int panelHeight = 420;
        return new Rectangle((GetBoardWidth(columns, rows) - panelWidth) / 2,
            (GetBoardHeight(columns, rows) - panelHeight) / 2,
            panelWidth, panelHeight);
    }

    /// <summary>Gets the bounds of the centered board-size picker prompt panel.</summary>
    /// <param name="columns">The number of tile columns on the current board.</param>
    /// <param name="rows">The number of tile rows on the current board.</param>
    public static Rectangle GetSizePromptPanelBounds(int columns, int rows) => GetOverlayPanelBounds(columns, rows);

    /// <summary>Gets the bounds of one selectable board-size option within the size picker prompt.</summary>
    /// <param name="columns">The number of tile columns on the current board, used to center the panel.</param>
    /// <param name="rows">The number of tile rows on the current board, used to center the panel.</param>
    /// <param name="optionIndex">The zero-based index of the size option, ordered left-to-right then top-to-bottom.</param>
    public static Rectangle GetSizePromptOptionBounds(int columns, int rows, int optionIndex)
    {
        const int optionWidth = 140;
        const int optionHeight = 64;
        const int optionGap = 16;
        const int optionsPerRow = 3;

        var panelBounds = GetSizePromptPanelBounds(columns, rows);
        var column = optionIndex % optionsPerRow;
        var row = optionIndex / optionsPerRow;
        var gridWidth = optionsPerRow * optionWidth + (optionsPerRow - 1) * optionGap;
        var startX = panelBounds.X + (panelBounds.Width - gridWidth) / 2;
        var startY = panelBounds.Y + 96;

        var x = startX + column * (optionWidth + optionGap);
        var y = startY + row * (optionHeight + optionGap);
        return new Rectangle(x, y, optionWidth, optionHeight);
    }

    /// <summary>Gets the confirm action bounds for the size picker prompt.</summary>
    /// <param name="columns">The number of tile columns on the current board, used to center the panel.</param>
    /// <param name="rows">The number of tile rows on the current board, used to center the panel.</param>
    public static Rectangle GetSizePromptConfirmBounds(int columns, int rows)
    {
        var panelBounds = GetSizePromptPanelBounds(columns, rows);
        return new Rectangle(panelBounds.X + 114, panelBounds.Bottom - 76, 150, 44);
    }

    /// <summary>Gets the cancel action bounds for the size picker prompt.</summary>
    /// <param name="columns">The number of tile columns on the current board, used to center the panel.</param>
    /// <param name="rows">The number of tile rows on the current board, used to center the panel.</param>
    public static Rectangle GetSizePromptCancelBounds(int columns, int rows)
    {
        var panelBounds = GetSizePromptPanelBounds(columns, rows);
        return new Rectangle(panelBounds.Right - 264, panelBounds.Bottom - 76, 150, 44);
    }
}

