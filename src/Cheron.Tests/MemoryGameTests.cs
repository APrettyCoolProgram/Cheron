// 261030_code
// 261030_documentation

using Cheron.Games.Memory;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace Cheron.Tests;

public sealed class MemoryGameTests
{
    [Fact]
    public void StartNewGame_DealsShuffledBoard_With8ColorPairs()
    {
        var game = CreateDealtGame();

        Assert.Equal(16, game.TestTileCount);

        var colorCounts = new Dictionary<int, int>();
        for (var index = 0; index < game.TestTileCount; index++)
        {
            var tile = game.TestGetTile(index);
            Assert.False(tile.IsFaceUp);
            Assert.False(tile.IsMatched);
            colorCounts[tile.ColorId] = colorCounts.GetValueOrDefault(tile.ColorId) + 1;
        }

        Assert.Equal(8, colorCounts.Count);
        Assert.All(colorCounts.Values, count => Assert.Equal(2, count));
    }

    [Fact]
    public void TestStartNewGame_ResetsStatsAndOverlays()
    {
        var game = CreateDealtGame();

        game.TestTryFlipTile(0);
        game.TestSetElapsedSeconds(12.5);

        game.TestStartNewGame();

        Assert.Equal(0, game.TestGetMoveCount());
        Assert.Equal(0, game.TestGetElapsedSeconds());
        Assert.False(game.TestHasWon());
        Assert.Equal(-1, game.TestGetFirstPickIndex());
        Assert.Equal(-1, game.TestGetSecondPickIndex());
        Assert.False(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void TryFlipTile_FirstPick_FlipsTileFaceUp_WithoutIncrementingMoves()
    {
        var game = CreateDealtGame();

        game.TestTryFlipTile(0);

        Assert.True(game.TestGetTile(0).IsFaceUp);
        Assert.Equal(0, game.TestGetFirstPickIndex());
        Assert.Equal(-1, game.TestGetSecondPickIndex());
        Assert.Equal(0, game.TestGetMoveCount());
    }

    [Fact]
    public void TryFlipTile_AlreadyFaceUpOrMatchedTile_IsIgnored()
    {
        var game = CreateDealtGame();

        game.TestTryFlipTile(0);
        game.TestTryFlipTile(0);

        Assert.Equal(0, game.TestGetFirstPickIndex());
        Assert.Equal(-1, game.TestGetSecondPickIndex());
    }

    [Fact]
    public void TryFlipTile_MatchingSecondPick_MarksBothTilesMatched_AndIncrementsMoves()
    {
        var game = CreateDealtGame();
        var (firstIndex, secondIndex) = FindMatchingPair(game);

        game.TestTryFlipTile(firstIndex);
        game.TestTryFlipTile(secondIndex);

        Assert.True(game.TestGetTile(firstIndex).IsMatched);
        Assert.True(game.TestGetTile(secondIndex).IsMatched);
        Assert.Equal(-1, game.TestGetFirstPickIndex());
        Assert.Equal(-1, game.TestGetSecondPickIndex());
        Assert.Equal(1, game.TestGetMoveCount());
    }

    [Fact]
    public void TryFlipTile_MismatchedSecondPick_LeavesBothTilesPendingFaceUp_AndIncrementsMoves()
    {
        var game = CreateDealtGame();
        var (firstIndex, secondIndex) = FindMismatchedPair(game);

        game.TestTryFlipTile(firstIndex);
        game.TestTryFlipTile(secondIndex);

        Assert.True(game.TestGetTile(firstIndex).IsFaceUp);
        Assert.True(game.TestGetTile(secondIndex).IsFaceUp);
        Assert.False(game.TestGetTile(firstIndex).IsMatched);
        Assert.False(game.TestGetTile(secondIndex).IsMatched);
        Assert.Equal(firstIndex, game.TestGetFirstPickIndex());
        Assert.Equal(secondIndex, game.TestGetSecondPickIndex());
        Assert.Equal(1, game.TestGetMoveCount());
    }

    [Fact]
    public void ResolvePendingPair_FlipsMismatchedTilesBackDown()
    {
        var game = CreateDealtGame();
        var (firstIndex, secondIndex) = FindMismatchedPair(game);

        game.TestTryFlipTile(firstIndex);
        game.TestTryFlipTile(secondIndex);
        game.TestResolvePendingPair();

        Assert.False(game.TestGetTile(firstIndex).IsFaceUp);
        Assert.False(game.TestGetTile(secondIndex).IsFaceUp);
        Assert.Equal(-1, game.TestGetFirstPickIndex());
        Assert.Equal(-1, game.TestGetSecondPickIndex());
    }

    [Fact]
    public void MatchingAllPairs_SetsHasWon()
    {
        var game = CreateDealtGame();

        while (!game.TestHasWon())
        {
            var (firstIndex, secondIndex) = FindMatchingPair(game);
            game.TestTryFlipTile(firstIndex);
            game.TestTryFlipTile(secondIndex);
        }

        Assert.True(game.TestHasWon());
        Assert.Equal(8, game.TestGetMoveCount());
        Assert.All(Enumerable.Range(0, game.TestTileCount), index => Assert.True(game.TestGetTile(index).IsMatched));
    }

    [Fact]
    public void HandleKeyboardInput_ArrowRight_MovesFocusWithinBoard()
    {
        var game = CreateDealtGame();
        game.TestSetFocusedIndex(0);
        game.TestResetPreviousKeyboardState();

        game.TestHandleKeyboardInput(new KeyboardState(Keys.Right));

        Assert.Equal(1, game.TestGetFocusedIndex());
    }

    [Fact]
    public void HandleKeyboardInput_ArrowAtBoundary_ClampsFocus()
    {
        var game = CreateDealtGame();
        game.TestSetFocusedIndex(0);
        game.TestResetPreviousKeyboardState();

        game.TestHandleKeyboardInput(new KeyboardState(Keys.Left));

        Assert.Equal(0, game.TestGetFocusedIndex());
    }

    [Fact]
    public void HandleKeyboardInput_Enter_FlipsFocusedTile()
    {
        var game = CreateDealtGame();
        game.TestSetFocusedIndex(2);
        game.TestResetPreviousKeyboardState();

        game.TestHandleKeyboardInput(new KeyboardState(Keys.Enter));

        Assert.True(game.TestGetTile(2).IsFaceUp);
    }

    [Fact]
    public void HandleGlobalShortcuts_N_OpensSizePrompt()
    {
        var game = CreateDealtGame();
        game.TestTryFlipTile(0);
        game.TestSetElapsedSeconds(5);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleGlobalShortcuts(new KeyboardState(Keys.N));

        Assert.True(handled);
        Assert.True(game.TestIsSizePromptOpen());
        Assert.Equal(5, game.TestGetElapsedSeconds());
    }

    [Fact]
    public void ConfirmSizePromptSelection_StartsNewGame_WithSelectedSize()
    {
        var game = CreateDealtGame();
        game.TestTryFlipTile(0);
        game.TestSetElapsedSeconds(5);
        game.TestOpenSizePrompt();
        game.TestMoveSizePromptSelection(1);

        var selectedSize = game.TestGetSizePromptSelection();
        game.TestConfirmSizePromptSelection();

        Assert.False(game.TestIsSizePromptOpen());
        Assert.Equal(selectedSize, game.TestGetColumns());
        Assert.Equal(selectedSize, game.TestGetRows());
        Assert.Equal(0, game.TestGetMoveCount());
        Assert.Equal(0, game.TestGetElapsedSeconds());
        Assert.Equal(selectedSize * selectedSize, game.TestTileCount);
    }

    [Fact]
    public void CancelSizePrompt_ClosesPrompt_WithoutChangingBoard()
    {
        var game = CreateDealtGame();
        var originalColumns = game.TestGetColumns();
        game.TestOpenSizePrompt();
        game.TestMoveSizePromptSelection(1);

        game.TestCancelSizePrompt();

        Assert.False(game.TestIsSizePromptOpen());
        Assert.Equal(originalColumns, game.TestGetColumns());
    }

    [Fact]
    public void HandleGlobalShortcuts_M_TogglesSound()
    {
        var game = new MemoryGame();
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleGlobalShortcuts(new KeyboardState(Keys.M));

        Assert.True(handled);
        Assert.False(game.TestIsSoundEnabled());
    }

    [Fact]
    public void HandleGlobalShortcuts_F1_TogglesHelpOverlay()
    {
        var game = new MemoryGame();
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleGlobalShortcuts(new KeyboardState(Keys.F1));

        Assert.True(handled);
        Assert.True(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void GetTileIndexAt_PointWithinTileBounds_ReturnsMatchingIndex()
    {
        var game = CreateDealtGame();
        var bounds = MemoryLayout.GetTileBounds(0, 0, game.TestGetColumns(), game.TestGetRows());

        var index = game.TestGetTileIndexAt(bounds.Center);

        Assert.Equal(0, index);
    }

    [Fact]
    public void GetTileIndexAt_PointOutsideBoard_ReturnsNegativeOne()
    {
        var game = CreateDealtGame();

        var index = game.TestGetTileIndexAt(new Microsoft.Xna.Framework.Point(-10, -10));

        Assert.Equal(-1, index);
    }

    private static MemoryGame CreateDealtGame()
    {
        var game = new MemoryGame();
        game.TestStartNewGame();
        return game;
    }

    private static (int FirstIndex, int SecondIndex) FindMatchingPair(MemoryGame game)
    {
        for (var i = 0; i < game.TestTileCount; i++)
        {
            if (game.TestGetTile(i).IsMatched)
            {
                continue;
            }

            for (var j = i + 1; j < game.TestTileCount; j++)
            {
                if (!game.TestGetTile(j).IsMatched && game.TestGetTile(i).ColorId == game.TestGetTile(j).ColorId)
                {
                    return (i, j);
                }
            }
        }

        throw new InvalidOperationException("No matching pair found on the board.");
    }

    private static (int FirstIndex, int SecondIndex) FindMismatchedPair(MemoryGame game)
    {
        for (var i = 0; i < game.TestTileCount; i++)
        {
            if (game.TestGetTile(i).IsMatched)
            {
                continue;
            }

            for (var j = i + 1; j < game.TestTileCount; j++)
            {
                if (!game.TestGetTile(j).IsMatched && game.TestGetTile(i).ColorId != game.TestGetTile(j).ColorId)
                {
                    return (i, j);
                }
            }
        }

        throw new InvalidOperationException("No mismatched pair found on the board.");
    }
}
