// 261031_code
// 261031_documentation

using Cheron.Games.Tile.Memory;
using Xunit;

namespace Cheron.Tests;

public sealed class TileLayoutTests
{
    [Fact]
    public void DefaultGridSize_Is4()
    {
        Assert.Equal(4, TileLayout.DefaultGridSize);
    }

    [Fact]
    public void MinAndMaxGridSize_AreFourAndTwelve()
    {
        Assert.Equal(4, TileLayout.MinGridSize);
        Assert.Equal(12, TileLayout.MaxGridSize);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    public void GetTileBounds_ReturnsSquareTile_ContainedWithinBoard(int gridSize)
    {
        var boardWidth = TileLayout.GetBoardWidth(gridSize);
        var boardHeight = TileLayout.GetBoardHeight(gridSize);

        for (var row = 0; row < gridSize; row++)
        {
            for (var column = 0; column < gridSize; column++)
            {
                var bounds = TileLayout.GetTileBounds(column, row, gridSize, gridSize);

                Assert.Equal(bounds.Width, bounds.Height);
                Assert.InRange(bounds.Width, 24, TileLayout.TileSize);
                Assert.True(bounds.Left >= 0);
                Assert.True(bounds.Top >= 0);
                Assert.True(bounds.Right <= boardWidth);
                Assert.True(bounds.Bottom <= boardHeight);
            }
        }
    }

    [Fact]
    public void GetTileBounds_AdjacentTiles_DoNotOverlap()
    {
        var left = TileLayout.GetTileBounds(0, 0, 4, 4);
        var right = TileLayout.GetTileBounds(1, 0, 4, 4);
        var below = TileLayout.GetTileBounds(0, 1, 4, 4);

        Assert.True(left.Right <= right.Left);
        Assert.True(left.Bottom <= below.Top);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(12)]
    public void BarActionBounds_PlaceNewGameLeftTop_SoundRightTop_AndHelpRightBottom(int gridSize)
    {
        var toolbarBounds = TileLayout.GetToolbarBounds(gridSize);
        var footerBounds = TileLayout.GetFooterBounds(gridSize, gridSize);
        var newGameBounds = TileLayout.GetNewGameButtonBounds(gridSize);
        var soundBounds = TileLayout.GetSoundButtonBounds(gridSize);
        var helpBounds = TileLayout.GetHelpButtonBounds(gridSize);

        Assert.True(toolbarBounds.Contains(newGameBounds));
        Assert.True(toolbarBounds.Contains(soundBounds));
        Assert.True(footerBounds.Contains(helpBounds));

        Assert.True(newGameBounds.Left <= TileLayout.BoardMargin + TileLayout.ButtonRightMargin);
        Assert.True(soundBounds.Right <= TileLayout.GetBoardWidth(gridSize));
        Assert.True(helpBounds.Right <= TileLayout.GetBoardWidth(gridSize));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(12)]
    public void GetOverlayPanelBounds_IsCenteredWithinBoard(int gridSize)
    {
        var panelBounds = TileLayout.GetOverlayPanelBounds(gridSize, gridSize);

        var expectedX = (TileLayout.GetBoardWidth(gridSize) - panelBounds.Width) / 2;
        var expectedY = (TileLayout.GetBoardHeight(gridSize) - panelBounds.Height) / 2;

        Assert.Equal(expectedX, panelBounds.X);
        Assert.Equal(expectedY, panelBounds.Y);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(12)]
    public void GetToolbarBounds_AndGetFooterBounds_SpanBoardWidth_WithoutOverlappingGrid(int gridSize)
    {
        var toolbarBounds = TileLayout.GetToolbarBounds(gridSize);
        var footerBounds = TileLayout.GetFooterBounds(gridSize, gridSize);

        Assert.Equal(TileLayout.GetBoardWidth(gridSize), toolbarBounds.Width);
        Assert.Equal(TileLayout.GetBoardWidth(gridSize), footerBounds.Width);
        Assert.Equal(0, toolbarBounds.Top);
        Assert.Equal(TileLayout.GetBoardHeight(gridSize), footerBounds.Bottom);
        Assert.True(toolbarBounds.Bottom <= footerBounds.Top);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    public void Grid_IsCenteredWithinBoardBody_ForAnyGridSize(int gridSize)
    {
        var toolbarBounds = TileLayout.GetToolbarBounds(gridSize);
        var footerBounds = TileLayout.GetFooterBounds(gridSize, gridSize);
        var firstTile = TileLayout.GetTileBounds(0, 0, gridSize, gridSize);
        var lastTile = TileLayout.GetTileBounds(gridSize - 1, gridSize - 1, gridSize, gridSize);

        var leftMargin = firstTile.Left;
        var rightMargin = TileLayout.GetBoardWidth(gridSize) - lastTile.Right;
        var topMargin = firstTile.Top - toolbarBounds.Bottom;
        var bottomMargin = footerBounds.Top - lastTile.Bottom;

        Assert.Equal(leftMargin, rightMargin);
        Assert.Equal(topMargin, bottomMargin);
    }

    [Theory]
    [InlineData(4, 12)]
    [InlineData(12, 4)]
    [InlineData(6, 10)]
    public void RectangularBoards_HaveCenteredGrid_WithBalancedMargins(int columns, int rows)
    {
        var toolbarBounds = TileLayout.GetToolbarBounds(columns, rows);
        var footerBounds = TileLayout.GetFooterBounds(columns, rows);
        var firstTile = TileLayout.GetTileBounds(0, 0, columns, rows);
        var lastTile = TileLayout.GetTileBounds(columns - 1, rows - 1, columns, rows);

        var leftMargin = firstTile.Left;
        var rightMargin = TileLayout.GetBoardWidth(columns, rows) - lastTile.Right;
        var topMargin = firstTile.Top - toolbarBounds.Bottom;
        var bottomMargin = footerBounds.Top - lastTile.Bottom;

        Assert.Equal(TileLayout.BoardMargin, leftMargin);
        Assert.Equal(leftMargin, rightMargin);
        Assert.Equal(TileLayout.BoardMargin, topMargin);
        Assert.Equal(topMargin, bottomMargin);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(12)]
    public void GetSizePromptOptionBounds_AreWithinPanel_AndDoNotOverlap(int gridSize)
    {
        var panelBounds = TileLayout.GetSizePromptPanelBounds(gridSize, gridSize);

        var firstOption = TileLayout.GetSizePromptOptionBounds(gridSize, gridSize, 0);
        var secondOption = TileLayout.GetSizePromptOptionBounds(gridSize, gridSize, 1);

        Assert.True(panelBounds.Contains(firstOption));
        Assert.True(panelBounds.Contains(secondOption));
        Assert.True(firstOption.Right <= secondOption.Left);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(12)]
    public void GetSizePromptConfirmAndCancelBounds_DoNotOverlap(int gridSize)
    {
        var confirmBounds = TileLayout.GetSizePromptConfirmBounds(gridSize, gridSize);
        var cancelBounds = TileLayout.GetSizePromptCancelBounds(gridSize, gridSize);

        Assert.True(confirmBounds.Right <= cancelBounds.Left || cancelBounds.Right <= confirmBounds.Left);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    public void GetSizePromptPanelBounds_IsCenteredWithinBoard(int gridSize)
    {
        var panelBounds = TileLayout.GetSizePromptPanelBounds(gridSize, gridSize);
        var expectedX = (TileLayout.GetBoardWidth(gridSize) - panelBounds.Width) / 2;
        var expectedY = (TileLayout.GetBoardHeight(gridSize) - panelBounds.Height) / 2;

        Assert.Equal(expectedX, panelBounds.X);
        Assert.Equal(expectedY, panelBounds.Y);
    }
}
