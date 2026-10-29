using Cheron.Games.Solitaire;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace Solitaire.Tests;

public sealed class SolitaireVariantPromptTests
{
    private static KeyboardState Press(params Keys[] keys) => new(keys);

    [Fact]
    public void OpenPrompt_PreselectsCurrentVariant_AndOpensPrompt()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.FreeCell);
        game.TestOpenNewGamePrompt();

        Assert.True(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.FreeCell, game.TestGetNewGamePromptVariant());
    }

    [Fact]
    public void TogglePromptVariant_SwitchesSelection()
    {
        var game = new SolitaireGame();

        game.TestSetPromptVariant(SolitaireVariant.Klondike);
        game.TestTogglePromptVariant();

        Assert.Equal(SolitaireVariant.FreeCell, game.TestGetNewGamePromptVariant());
    }

    [Fact]
    public void ConfirmPrompt_UpdatesVariant_AndClosesPrompt()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestSetPromptVariant(SolitaireVariant.FreeCell);
        game.TestSetNewGamePromptOpen(true);

        game.TestConfirmNewGamePromptSelection();

        Assert.Equal(SolitaireVariant.FreeCell, game.TestGetVariant());
        Assert.False(game.TestIsNewGamePromptOpen());
    }

    [Fact]
    public void CancelPrompt_ClosesPrompt_WithoutChangingVariant()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestSetPromptVariant(SolitaireVariant.FreeCell);
        game.TestSetNewGamePromptOpen(true);

        game.TestCancelNewGamePromptSelection();

        Assert.Equal(SolitaireVariant.Klondike, game.TestGetVariant());
        Assert.False(game.TestIsNewGamePromptOpen());
    }

    [Fact]
    public void PromptArrowKey_TogglesHighlightedVariant()
    {
        var game = new SolitaireGame();

        game.TestSetPromptVariant(SolitaireVariant.Klondike);
        game.TestSetNewGamePromptOpen(true);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleAccessibilityKeys(Press(Keys.Right));

        Assert.True(handled);
        Assert.Equal(SolitaireVariant.FreeCell, game.TestGetNewGamePromptVariant());
    }

    [Fact]
    public void PromptEscape_ClosesPrompt_WithoutChangingVariant()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestSetPromptVariant(SolitaireVariant.FreeCell);
        game.TestSetNewGamePromptOpen(true);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleAccessibilityKeys(Press(Keys.Escape));

        Assert.True(handled);
        Assert.False(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.Klondike, game.TestGetVariant());
    }

    [Fact]
    public void PromptEnter_ConfirmsHighlightedVariant()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestSetPromptVariant(SolitaireVariant.FreeCell);
        game.TestSetNewGamePromptOpen(true);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleAccessibilityKeys(Press(Keys.Enter));

        Assert.True(handled);
        Assert.False(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.FreeCell, game.TestGetVariant());
    }

    [Fact]
    public void PromptPointerClickOnFreeCell_StartsFreeCellGame()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestOpenNewGamePrompt();

        var clickPoint = SolitaireLayout.GetNewGamePromptFreeCellBounds().Center;
        game.TestUpdatePointer(clickPoint, true, TimeSpan.Zero);
        game.TestUpdatePointer(clickPoint, false, TimeSpan.Zero);

        Assert.False(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.FreeCell, game.TestGetVariant());
    }

    [Fact]
    public void PromptPointerClickCancel_ClosesPromptWithoutChangingVariant()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestOpenNewGamePrompt();

        var cancelPoint = SolitaireLayout.GetNewGamePromptCancelBounds().Center;
        game.TestUpdatePointer(cancelPoint, true, TimeSpan.Zero);
        game.TestUpdatePointer(cancelPoint, false, TimeSpan.Zero);

        Assert.False(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.Klondike, game.TestGetVariant());
    }

    [Fact]
    public void StartNewGame_Klondike_DealsExpectedLayout()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestStartNewGame();

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, game.TestGetTableauCounts());
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6 }, game.TestGetTableauFaceDownCounts());
        Assert.Equal(24, game.TestGetStockCount());
        Assert.Equal(new[] { 0, 0, 0, 0 }, game.TestGetFoundationCounts());
        Assert.Empty(game.TestGetFreeCellCounts());
    }

    [Fact]
    public void StartNewGame_FreeCell_DealsExpectedLayout()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.FreeCell);
        game.TestStartNewGame();

        Assert.Equal(new[] { 7, 7, 7, 7, 6, 6, 6, 6 }, game.TestGetTableauCounts());
        Assert.Equal(new[] { 0, 0, 0, 0, 0, 0, 0, 0 }, game.TestGetTableauFaceDownCounts());
        Assert.Equal(0, game.TestGetStockCount());
        Assert.Equal(new[] { 0, 0, 0, 0 }, game.TestGetFoundationCounts());
        Assert.Equal(new[] { 0, 0, 0, 0 }, game.TestGetFreeCellCounts());
    }

    [Fact]
    public void SmokePassShortcut_TogglesOverlay_AndClosesOtherModals()
    {
        var game = new SolitaireGame();

        game.TestSetNewGamePromptOpen(true);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleAccessibilityKeys(Press(Keys.LeftShift, Keys.F9));

        Assert.True(handled);
        Assert.True(game.TestIsSmokePassOverlayOpen());
        Assert.False(game.TestIsNewGamePromptOpen());

        game.TestHandleAccessibilityKeys(Press());
        handled = game.TestHandleAccessibilityKeys(Press(Keys.LeftShift, Keys.F9));

        Assert.True(handled);
        Assert.False(game.TestIsSmokePassOverlayOpen());
    }

    [Fact]
    public void SmokePassOverlay_BlocksOptionsShortcut_UntilClosed()
    {
        var game = new SolitaireGame();

        game.TestResetPreviousKeyboardState();
        game.TestHandleAccessibilityKeys(Press(Keys.LeftShift, Keys.F9));
        Assert.True(game.TestIsSmokePassOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.False(game.TestIsOptionsOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.F9));
        Assert.False(game.TestIsSmokePassOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.True(game.TestIsOptionsOpen());
    }

    [Fact]
    public void SmokePassToggle_DuringDealAnimation_DoesNotClearTransitions()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestStartNewGame();
        var initialTransitionCount = game.TestGetCardTransitionCount();
        Assert.True(initialTransitionCount > 0);

        game.TestResetPreviousKeyboardState();
        game.TestHandleAccessibilityKeys(Press(Keys.LeftShift, Keys.F9));
        Assert.True(game.TestIsSmokePassOverlayOpen());
        Assert.Equal(initialTransitionCount, game.TestGetCardTransitionCount());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.F9));
        Assert.False(game.TestIsSmokePassOverlayOpen());
        Assert.Equal(initialTransitionCount, game.TestGetCardTransitionCount());
    }

    [Fact]
    public void FreeCell_EighthTableauColumn_FitsWithinBoardWidth()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.FreeCell);
        game.TestStartNewGame();

        var eighthColumnTopCardBounds = game.TestGetTableauCardBounds(7, 0);
        Assert.True(eighthColumnTopCardBounds.Right <= SolitaireLayout.BoardWidth);
    }

    [Fact]
    public void HelpShortcut_OpensOverlay_AndEscapeClosesIt()
    {
        var game = new SolitaireGame();

        game.TestResetPreviousKeyboardState();
        var opened = game.TestHandleAccessibilityKeys(Press(Keys.F1));

        Assert.True(opened);
        Assert.True(game.TestIsHelpOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.Escape));

        Assert.False(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void HelpOverlay_ScrollsWithinBounds()
    {
        var game = new SolitaireGame();

        game.TestOpenHelpOverlay();
        var initialOffset = game.TestGetHelpScrollOffset();
        game.TestScrollHelpOverlay(1);

        Assert.True(game.TestGetHelpScrollOffset() >= initialOffset);

        for (var index = 0; index < 40; index++)
        {
            game.TestScrollHelpOverlay(1);
        }

        var maxOffset = game.TestGetHelpScrollOffset();
        game.TestScrollHelpOverlay(1);

        Assert.Equal(maxOffset, game.TestGetHelpScrollOffset());

        for (var index = 0; index < 60; index++)
        {
            game.TestScrollHelpOverlay(-1);
        }

        Assert.Equal(0, game.TestGetHelpScrollOffset());
    }

    [Fact]
    public void HelpOverlay_UsesVariantSpecificInstructions()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestOpenHelpOverlay();
        Assert.Equal("KLONDIKE", game.TestGetHelpSubtitle());
        Assert.Contains("TABLEAU BUILDS DOWN BY ALTERNATING COLORS", game.TestGetHelpLines());

        game.TestSetVariant(SolitaireVariant.FreeCell);
        game.TestOpenHelpOverlay();
        Assert.Equal("FREECELL", game.TestGetHelpSubtitle());
        Assert.Contains("USE FREECELLS TO HOLD ONE CARD EACH", game.TestGetHelpLines());
    }
}
