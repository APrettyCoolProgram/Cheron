// 261101_code
// 261101_documentation

using global::Cheron.Games.Card.Solitaire;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace Cheron.Tests;

public sealed class SolitaireVariantPromptTests
{
    private static KeyboardState Press(params Keys[] keys) => new(keys);

    private static void Click(SolitaireGame game, Point point)
    {
        game.TestUpdatePointer(point, true, TimeSpan.Zero);
        game.TestUpdatePointer(point, false, TimeSpan.Zero);
    }

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
    public void PromptArrowKey_CyclesToSpiderVariant()
    {
        var game = new SolitaireGame();

        game.TestSetPromptVariant(SolitaireVariant.FreeCell);
        game.TestSetNewGamePromptOpen(true);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleAccessibilityKeys(Press(Keys.Right));

        Assert.True(handled);
        Assert.Equal(SolitaireVariant.Spider, game.TestGetNewGamePromptVariant());
    }

    [Fact]
    public void PromptArrowKey_CyclesToYukonVariant()
    {
        var game = new SolitaireGame();

        game.TestSetPromptVariant(SolitaireVariant.Spider);
        game.TestSetNewGamePromptOpen(true);
        game.TestResetPreviousKeyboardState();

        var handled = game.TestHandleAccessibilityKeys(Press(Keys.Right));

        Assert.True(handled);
        Assert.Equal(SolitaireVariant.Yukon, game.TestGetNewGamePromptVariant());
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
    public void PromptPointerClickOnSpider_StartsSpiderGame()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestOpenNewGamePrompt();

        var clickPoint = SolitaireLayout.GetNewGamePromptSpiderBounds().Center;
        game.TestUpdatePointer(clickPoint, true, TimeSpan.Zero);
        game.TestUpdatePointer(clickPoint, false, TimeSpan.Zero);

        Assert.False(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.Spider, game.TestGetVariant());
    }

    [Fact]
    public void PromptPointerClickOnYukon_StartsYukonGame()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Klondike);
        game.TestOpenNewGamePrompt();

        var clickPoint = SolitaireLayout.GetNewGamePromptYukonBounds().Center;
        game.TestUpdatePointer(clickPoint, true, TimeSpan.Zero);
        game.TestUpdatePointer(clickPoint, false, TimeSpan.Zero);

        Assert.False(game.TestIsNewGamePromptOpen());
        Assert.Equal(SolitaireVariant.Yukon, game.TestGetVariant());
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
    public void StartNewGame_Spider_DealsExpectedLayout()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Spider);
        game.TestStartNewGame();

        Assert.Equal(new[] { 6, 6, 6, 6, 5, 5, 5, 5, 5, 5 }, game.TestGetTableauCounts());
        Assert.Equal(new[] { 5, 5, 5, 5, 4, 4, 4, 4, 4, 4 }, game.TestGetTableauFaceDownCounts());
        Assert.Equal(50, game.TestGetStockCount());
        Assert.Equal(new[] { 0, 0, 0, 0 }, game.TestGetFoundationCounts());
        Assert.Empty(game.TestGetFreeCellCounts());
    }

    [Fact]
    public void StartNewGame_Yukon_DealsExpectedLayout()
    {
        var game = new SolitaireGame();

        game.TestSetVariant(SolitaireVariant.Yukon);
        game.TestStartNewGame();

        Assert.Equal(new[] { 1, 6, 7, 8, 9, 10, 11 }, game.TestGetTableauCounts());
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6 }, game.TestGetTableauFaceDownCounts());
        Assert.Equal(0, game.TestGetStockCount());
        Assert.Equal(new[] { 0, 0, 0, 0 }, game.TestGetFoundationCounts());
        Assert.Empty(game.TestGetFreeCellCounts());
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
    public void NewGameShortcutN_OpensPrompt()
    {
        var game = new SolitaireGame();

        game.TestResetPreviousKeyboardState();

        game.TestHandleKeyboardFrame(Press(Keys.N));

        Assert.True(game.TestIsNewGamePromptOpen());
    }

    [Fact]
    public void OptionsShortcutO_TogglesOptions()
    {
        var game = new SolitaireGame();

        game.TestResetPreviousKeyboardState();
        var opened = game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.True(opened);
        Assert.True(game.TestIsOptionsOpen());

        game.TestHandleAccessibilityKeys(Press());
        var closed = game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.True(closed);
        Assert.False(game.TestIsOptionsOpen());
    }

    [Fact]
    public void SoundShortcutM_TogglesSound_WhenAvailable()
    {
        var game = new SolitaireGame();

        Assert.True(game.TestIsSoundEnabled());

        game.TestResetPreviousKeyboardState();
        var handled = game.TestHandleAccessibilityKeys(Press(Keys.M));

        Assert.True(handled);
        Assert.False(game.TestIsSoundEnabled());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.M));

        Assert.True(game.TestIsSoundEnabled());
    }

    [Fact]
    public void SoundShortcutM_DoesNotToggle_WhenAudioUnavailable()
    {
        var game = new SolitaireGame();
        game.TestSetAudioUnavailable(true);

        Assert.True(game.TestIsSoundEnabled());

        game.TestResetPreviousKeyboardState();
        var handled = game.TestHandleAccessibilityKeys(Press(Keys.M));

        Assert.True(handled);
        Assert.True(game.TestIsSoundEnabled());
    }

    [Fact]
    public void ToolbarSoundButton_ClickInside_TogglesSound()
    {
        var game = new SolitaireGame();
        Assert.True(game.TestIsSoundEnabled());

        Click(game, SolitaireLayout.SoundButtonBounds.Center);

        Assert.False(game.TestIsSoundEnabled());
    }

    [Fact]
    public void ToolbarOptionsButton_ClickInside_OpensOptions()
    {
        var game = new SolitaireGame();
        Assert.False(game.TestIsOptionsOpen());

        Click(game, SolitaireLayout.OptionsButtonBounds.Center);

        Assert.True(game.TestIsOptionsOpen());
    }

    [Fact]
    public void ToolbarHelpButton_ClickInside_OpensHelp()
    {
        var game = new SolitaireGame();
        Assert.False(game.TestIsHelpOverlayOpen());

        Click(game, SolitaireLayout.HelpButtonBounds.Center);

        Assert.True(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void ToolbarButtons_ClickOutside_DoNotTriggerActions()
    {
        var game = new SolitaireGame();
        var soundOutside = new Point(SolitaireLayout.SoundButtonBounds.Center.X, SolitaireLayout.SoundButtonBounds.Y - 1);
        var optionsOutside = new Point(SolitaireLayout.OptionsButtonBounds.Center.X, SolitaireLayout.OptionsButtonBounds.Y - 1);
        var helpOutside = new Point(SolitaireLayout.HelpButtonBounds.Center.X, SolitaireLayout.HelpButtonBounds.Y - 1);

        Assert.True(game.TestIsSoundEnabled());
        Assert.False(game.TestIsOptionsOpen());
        Assert.False(game.TestIsHelpOverlayOpen());

        Click(game, soundOutside);
        Click(game, optionsOutside);
        Click(game, helpOutside);

        Assert.True(game.TestIsSoundEnabled());
        Assert.False(game.TestIsOptionsOpen());
        Assert.False(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void SoundButton_ClickInside_DoesNotToggle_WhenAudioUnavailable()
    {
        var game = new SolitaireGame();
        game.TestSetAudioUnavailable(true);

        Assert.True(game.TestIsSoundEnabled());

        Click(game, SolitaireLayout.SoundButtonBounds.Center);

        Assert.True(game.TestIsSoundEnabled());
    }

    [Fact]
    public void HelpOverlay_BlocksOptionsAndSoundShortcuts_UntilClosed()
    {
        var game = new SolitaireGame();

        game.TestOpenHelpOverlay();
        Assert.True(game.TestIsHelpOverlayOpen());

        game.TestResetPreviousKeyboardState();
        game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.False(game.TestIsOptionsOpen());

        game.TestHandleAccessibilityKeys(Press());
        var soundBefore = game.TestIsSoundEnabled();
        game.TestHandleAccessibilityKeys(Press(Keys.M));
        Assert.Equal(soundBefore, game.TestIsSoundEnabled());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.F1));
        Assert.False(game.TestIsHelpOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.True(game.TestIsOptionsOpen());
    }

    [Fact]
    public void OptionsOpen_BlocksNewGameShortcutN_UntilClosed()
    {
        var game = new SolitaireGame();

        game.TestResetPreviousKeyboardState();
        game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.True(game.TestIsOptionsOpen());

        game.TestHandleKeyboardFrame(Press());
        game.TestHandleKeyboardFrame(Press(Keys.N));
        Assert.False(game.TestIsNewGamePromptOpen());

        game.TestHandleKeyboardFrame(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.O));
        Assert.False(game.TestIsOptionsOpen());

        game.TestHandleKeyboardFrame(Press());
        game.TestHandleKeyboardFrame(Press(Keys.N));
        Assert.True(game.TestIsNewGamePromptOpen());
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
    public void TopBarControlBounds_AreOrderedLeftToRight_AndNonOverlapping()
    {
        var controls = new[]
        {
            SolitaireLayout.NewGameButtonBounds,
            SolitaireLayout.UndoButtonBounds,
            SolitaireLayout.HintButtonBounds,
            SolitaireLayout.OptionsButtonBounds,
            SolitaireLayout.HelpButtonBounds,
            SolitaireLayout.SoundButtonBounds
        };

        for (var index = 0; index < controls.Length - 1; index++)
        {
            Assert.True(controls[index].Right <= controls[index + 1].Left,
                $"Control {index} overlaps or is out of order with control {index + 1}.");
        }
    }

    [Fact]
    public void TopBarControlBounds_StayWithinToolbarAndBoard()
    {
        var controls = new[]
        {
            SolitaireLayout.NewGameButtonBounds,
            SolitaireLayout.UndoButtonBounds,
            SolitaireLayout.HintButtonBounds,
            SolitaireLayout.OptionsButtonBounds,
            SolitaireLayout.HelpButtonBounds,
            SolitaireLayout.SoundButtonBounds
        };

        foreach (var bounds in controls)
        {
            Assert.InRange(bounds.Left, 0, SolitaireLayout.BoardWidth - 1);
            Assert.InRange(bounds.Right, 1, SolitaireLayout.BoardWidth);
            Assert.InRange(bounds.Top, 0, SolitaireLayout.TopBarHeight - 1);
            Assert.InRange(bounds.Bottom, 1, SolitaireLayout.TopBarHeight);
        }
    }

    [Fact]
    public void KeyboardFocusBounds_MatchTopBarControlBounds_ForTopControls()
    {
        var game = new SolitaireGame();

        game.TestFocusKeyboardTarget(0);
        Assert.Equal(SolitaireLayout.NewGameButtonBounds, game.TestGetKeyboardBounds());

        game.TestFocusKeyboardTarget(1);
        Assert.Equal(SolitaireLayout.UndoButtonBounds, game.TestGetKeyboardBounds());

        game.TestFocusKeyboardTarget(2);
        Assert.Equal(SolitaireLayout.HintButtonBounds, game.TestGetKeyboardBounds());

        game.TestFocusKeyboardTarget(3);
        Assert.Equal(SolitaireLayout.SoundButtonBounds, game.TestGetKeyboardBounds());

        game.TestFocusKeyboardTarget(4);
        Assert.Equal(SolitaireLayout.OptionsButtonBounds, game.TestGetKeyboardBounds());
    }

    [Fact]
    public void KeyboardDescription_ReflectsTopControlShortcutText()
    {
        var game = new SolitaireGame();

        game.TestFocusKeyboardTarget(0);
        Assert.Equal("NEW GAME  N", game.TestGetKeyboardDescription());

        game.TestFocusKeyboardTarget(3);
        Assert.Equal("SOUND ON  M", game.TestGetKeyboardDescription());

        game.TestFocusKeyboardTarget(4);
        Assert.Equal("OPTIONS  O", game.TestGetKeyboardDescription());
    }

    [Fact]
    public void HelpShortcutF1_ClosesHelp_WhenAlreadyOpen()
    {
        var game = new SolitaireGame();
        game.TestOpenHelpOverlay();
        Assert.True(game.TestIsHelpOverlayOpen());

        game.TestResetPreviousKeyboardState();
        var handled = game.TestHandleAccessibilityKeys(Press(Keys.F1));

        Assert.True(handled);
        Assert.False(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void SmokePassOverlay_BlocksHelpAndSoundShortcuts_UntilClosed()
    {
        var game = new SolitaireGame();

        game.TestResetPreviousKeyboardState();
        game.TestHandleAccessibilityKeys(Press(Keys.LeftShift, Keys.F9));
        Assert.True(game.TestIsSmokePassOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.F1));
        Assert.False(game.TestIsHelpOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        var soundBefore = game.TestIsSoundEnabled();
        game.TestHandleAccessibilityKeys(Press(Keys.M));
        Assert.Equal(soundBefore, game.TestIsSoundEnabled());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.F9));
        Assert.False(game.TestIsSmokePassOverlayOpen());

        game.TestHandleAccessibilityKeys(Press());
        game.TestHandleAccessibilityKeys(Press(Keys.F1));
        Assert.True(game.TestIsHelpOverlayOpen());
    }

    [Fact]
    public void KeyboardDescription_ShowsSoundUnavailable_WhenAudioUnavailable()
    {
        var game = new SolitaireGame();
        game.TestSetAudioUnavailable(true);

        game.TestFocusKeyboardTarget(3);

        Assert.Equal("SOUND UNAVAILABLE", game.TestGetKeyboardDescription());
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

        game.TestSetVariant(SolitaireVariant.Spider);
        game.TestOpenHelpOverlay();
        Assert.Equal("SPIDER", game.TestGetHelpSubtitle());
        Assert.Contains("ONLY FACE-UP SAME-SUIT DESCENDING RUNS MOVE AS GROUPS", game.TestGetHelpLines());

        game.TestSetVariant(SolitaireVariant.Yukon);
        game.TestOpenHelpOverlay();
        Assert.Equal("YUKON", game.TestGetHelpSubtitle());
        Assert.Contains("MOVE ANY FACE-UP GROUP ONTO A CARD ONE RANK HIGHER", game.TestGetHelpLines());
    }
}
