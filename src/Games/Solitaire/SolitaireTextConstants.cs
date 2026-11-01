// 261029_code
// 261029_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes toolbar labels and hint text used by the Solitaire UI.</summary>
internal static class SolitaireTextConstants
{
    /// <summary>The title shown in the host window.</summary>
    internal const string WindowTitle = "Cheron Solitaire";

    /// <summary>The toolbar label for starting a new deal.</summary>
    internal const string NewGameLabel = "NEW GAME";

    /// <summary>The toolbar label for undoing the last action.</summary>
    internal const string UndoLabel = "UNDO";

    /// <summary>The toolbar label for requesting a one-step hint.</summary>
    internal const string HintLabel = "HINT";

    /// <summary>The toolbar label for opening variant instructions.</summary>
    internal const string HelpLabel = "HELP";

    /// <summary>The toolbar status label when sound cannot be used.</summary>
    internal const string AudioUnavailableLabel = "NA";

    /// <summary>The toolbar status label when sound is enabled.</summary>
    internal const string SoundEnabledLabel = "SND";

    /// <summary>The toolbar status label when sound is disabled.</summary>
    internal const string SoundDisabledLabel = "OFF";

    /// <summary>Hint text for moving a card to a foundation.</summary>
    internal const string HintToFoundationMessage = "HINT  MOVE GOLD CARD TO GOLD FOUNDATION";

    /// <summary>Hint text for moving a card to a tableau column.</summary>
    internal const string HintToTableauMessage = "HINT  MOVE GOLD CARD TO GOLD TABLEAU";

    /// <summary>Hint text for drawing from a non-empty stock.</summary>
    internal const string HintDrawFromStockMessage = "HINT  CLICK STOCK TO DRAW";

    /// <summary>Hint text for recycling when stock is empty.</summary>
    internal const string HintRecycleStockMessage = "HINT  CLICK EMPTY STOCK TO RECYCLE";

    /// <summary>Hint text shown when no immediate suggestion exists.</summary>
    internal const string HintNoSuggestionMessage = "HINT  NO SUGGESTED MOVE  TRY UNDO OR NEW GAME";

    /// <summary>The title shown in the accessibility options modal.</summary>
    internal const string AccessibilityTitle = "ACCESSIBILITY";

    /// <summary>The new-game variant prompt title shown over the board.</summary>
    internal const string NewGameVariantPromptTitle = "START NEW GAME";

    /// <summary>The new-game prompt variant option label for Klondike.</summary>
    internal const string NewGameVariantKlondikeLabel = "KLONDIKE";

    /// <summary>The new-game prompt variant option label for FreeCell.</summary>
    internal const string NewGameVariantFreeCellLabel = "FREECELL";

    /// <summary>The new-game prompt variant option label for Spider.</summary>
    internal const string NewGameVariantSpiderLabel = "SPIDER";

    /// <summary>The new-game variant prompt confirm label.</summary>
    internal const string NewGameVariantConfirmLabel = "ENTER  START";

    /// <summary>The new-game variant prompt cancel label.</summary>
    internal const string NewGameVariantCancelLabel = "BACKSPACE  CANCEL";

    /// <summary>The manual smoke-pass overlay title.</summary>
    internal const string SmokePassTitle = "MANUAL UX SMOKE PASS";

    /// <summary>The manual smoke-pass overlay step 1.</summary>
    internal const string SmokePassStep1 = "1  OPEN NEW GAME PROMPT  (N OR NEW GAME BUTTON)";

    /// <summary>The manual smoke-pass overlay step 2.</summary>
    internal const string SmokePassStep2 = "2  VERIFY KLONDIKE FREECELL AND SPIDER OPTIONS RENDER";

    /// <summary>The manual smoke-pass overlay step 3.</summary>
    internal const string SmokePassStep3 = "3  KEYBOARD  ARROWS TOGGLE   ENTER START   ESC CANCEL";

    /// <summary>The manual smoke-pass overlay step 4.</summary>
    internal const string SmokePassStep4 = "4  POINTER  CLICK VARIANT TO START   CLICK CANCEL";

    /// <summary>The manual smoke-pass overlay step 5.</summary>
    internal const string SmokePassStep5 = "5  CONFIRM PROMPT BLOCKS BOARD/OPTIONS INPUT";

    /// <summary>The manual smoke-pass overlay step 6.</summary>
    internal const string SmokePassStep6 = "6  DURING DEAL ANIMATION  TOGGLE OVERLAY WITHOUT BREAKING PLAY";

    /// <summary>The manual smoke-pass overlay footer.</summary>
    internal const string SmokePassFooter = "SHIFT+F9 TO CLOSE";

    /// <summary>The close action label shown in the accessibility options list.</summary>
    internal const string AccessibilityCloseOptionsLabel = "O  CLOSE OPTIONS";

    /// <summary>The accessibility help row describing keyboard navigation and activation.</summary>
    internal const string AccessibilityHelpNavigation = "TAB OR UP DOWN  CHOOSE   ENTER OR SPACE  TOGGLE";

    /// <summary>The accessibility help row explaining settings persistence.</summary>
    internal const string AccessibilityHelpPersistence = "SETTINGS LAST UNTIL THE APPLICATION CLOSES";

    /// <summary>The accessibility help row describing close and exit shortcuts.</summary>
    internal const string AccessibilityHelpCloseAndExit = "O CLOSES OPTIONS   ESC EXITS THE GAME";

    /// <summary>The accessibility help row noting board shortcuts are suspended while modal is open.</summary>
    internal const string AccessibilityHelpShortcutsDisabled =
        "BOARD SHORTCUTS ARE DISABLED WHILE OPTIONS ARE OPEN";

    /// <summary>The footer message shown after an invalid move.</summary>
    internal const string FooterInvalidMoveMessage = "MOVE NOT ALLOWED  TRY ANOTHER TARGET OR USE HINT";

    /// <summary>The footer message shown when no hint or focus description is active.</summary>
    internal const string FooterDefaultMessage = "BUILD DOWN IN ALTERNATING COLORS";

    /// <summary>The footer instructions shown while keyboard focus mode is active.</summary>
    internal const string FooterKeyboardInstructions =
        "TAB FOCUS  ARROWS NAVIGATE  ENTER ACT  F BUILD  BACKSPACE CLEAR";

    /// <summary>The footer instructions shown when audio is unavailable.</summary>
    internal const string FooterSoundUnavailableInstructions = "SOUND UNAVAILABLE  TAB KEYBOARD PLAY  ESC EXIT";

    /// <summary>The footer instructions shown during normal pointer-first play.</summary>
    internal const string FooterDefaultInstructions = "DRAG OR CLICK TO MOVE  TAB KEYBOARD PLAY  ESC EXIT";

    /// <summary>The title shown in the Help overlay.</summary>
    internal const string HelpOverlayTitle = "HOW TO PLAY";

    /// <summary>The Help overlay subtitle for Klondike instructions.</summary>
    internal const string HelpOverlayKlondikeSubtitle = "KLONDIKE";

    /// <summary>The Help overlay subtitle for FreeCell instructions.</summary>
    internal const string HelpOverlayFreeCellSubtitle = "FREECELL";

    /// <summary>The Help overlay subtitle for Spider instructions.</summary>
    internal const string HelpOverlaySpiderSubtitle = "SPIDER";

    /// <summary>The close/scroll hint shown in the Help overlay footer.</summary>
    internal const string HelpOverlayFooterHint = "ESC CLOSE   UP DOWN OR WHEEL SCROLL";

    /// <summary>Klondike Help line 1.</summary>
    internal const string HelpKlondikeLine1 = "GOAL  BUILD FOUR FOUNDATIONS FROM ACE TO KING";

    /// <summary>Klondike Help line 2.</summary>
    internal const string HelpKlondikeLine2 = "TABLEAU BUILDS DOWN BY ALTERNATING COLORS";

    /// <summary>Klondike Help line 3.</summary>
    internal const string HelpKlondikeLine3 = "MOVE A FACE-UP RUN AS A GROUP";

    /// <summary>Klondike Help line 4.</summary>
    internal const string HelpKlondikeLine4 = "ONLY A KING MAY FILL AN EMPTY TABLEAU COLUMN";

    /// <summary>Klondike Help line 5.</summary>
    internal const string HelpKlondikeLine5 = "CLICK STOCK TO DRAW CARDS TO WASTE";

    /// <summary>Klondike Help line 6.</summary>
    internal const string HelpKlondikeLine6 = "WHEN STOCK IS EMPTY CLICK IT TO RECYCLE WASTE";

    /// <summary>Klondike Help line 7.</summary>
    internal const string HelpKlondikeLine7 = "DOUBLE-CLICK A CARD TO TRY MOVING IT TO FOUNDATION";

    /// <summary>Klondike Help line 8.</summary>
    internal const string HelpKlondikeLine8 = "USE HINT FOR A SUGGESTED MOVE AND UNDO TO STEP BACK";

    /// <summary>FreeCell Help line 1.</summary>
    internal const string HelpFreeCellLine1 = "GOAL  BUILD FOUR FOUNDATIONS FROM ACE TO KING";

    /// <summary>FreeCell Help line 2.</summary>
    internal const string HelpFreeCellLine2 = "ALL CARDS START FACE UP IN EIGHT CASCADES";

    /// <summary>FreeCell Help line 3.</summary>
    internal const string HelpFreeCellLine3 = "TABLEAU BUILDS DOWN BY ALTERNATING COLORS";

    /// <summary>FreeCell Help line 4.</summary>
    internal const string HelpFreeCellLine4 = "USE FREECELLS TO HOLD ONE CARD EACH";

    /// <summary>FreeCell Help line 5.</summary>
    internal const string HelpFreeCellLine5 = "EMPTY FREECELLS AND COLUMNS INCREASE MOVE CAPACITY";

    /// <summary>FreeCell Help line 6.</summary>
    internal const string HelpFreeCellLine6 = "ANY CARD MAY MOVE TO AN EMPTY COLUMN";

    /// <summary>FreeCell Help line 7.</summary>
    internal const string HelpFreeCellLine7 = "DOUBLE-CLICK A CARD TO TRY MOVING IT TO FOUNDATION";

    /// <summary>FreeCell Help line 8.</summary>
    internal const string HelpFreeCellLine8 = "PLAN AHEAD  BLOCKED LOW CARDS CAN STOP PROGRESS";

    /// <summary>Spider Help line 1.</summary>
    internal const string HelpSpiderLine1 = "GOAL  BUILD COMPLETE KING-TO-ACE SUIT RUNS";

    /// <summary>Spider Help line 2.</summary>
    internal const string HelpSpiderLine2 = "TEN TABLEAU COLUMNS START WITH FACE-DOWN STACKS";

    /// <summary>Spider Help line 3.</summary>
    internal const string HelpSpiderLine3 = "ONLY FACE-UP SAME-SUIT DESCENDING RUNS MOVE AS GROUPS";

    /// <summary>Spider Help line 4.</summary>
    internal const string HelpSpiderLine4 = "SINGLE CARDS MAY MOVE ONTO A CARD ONE RANK HIGHER";

    /// <summary>Spider Help line 5.</summary>
    internal const string HelpSpiderLine5 = "ANY CARD OR RUN MAY MOVE TO AN EMPTY COLUMN";

    /// <summary>Spider Help line 6.</summary>
    internal const string HelpSpiderLine6 = "WHEN A COLUMN'S TOP FACE-DOWN CARD IS EXPOSED IT FLIPS";

    /// <summary>Spider Help line 7.</summary>
    internal const string HelpSpiderLine7 = "COMPLETE SUIT RUNS CLEAR TO FOUNDATIONS AUTOMATICALLY";

    /// <summary>Spider Help line 8.</summary>
    internal const string HelpSpiderLine8 = "PLAN SPACES CAREFULLY  EMPTY COLUMNS ENABLE BIG MOVES";

    /// <summary>The footer-adjacent options action label.</summary>
    internal const string FooterOptionsLabel = "OPTIONS";

    /// <summary>The keyboard-focus description for the New Game control.</summary>
    internal const string KeyboardDescriptionNewGame = "NEW GAME  N";

    /// <summary>The keyboard-focus description when Undo cannot be used.</summary>
    internal const string KeyboardDescriptionUndoUnavailable = "UNDO UNAVAILABLE";

    /// <summary>The keyboard-focus description when Undo is available.</summary>
    internal const string KeyboardDescriptionUndoAvailable = "UNDO  U";

    /// <summary>The keyboard-focus description when Hint cannot be used.</summary>
    internal const string KeyboardDescriptionHintUnavailable = "HINT UNAVAILABLE";

    /// <summary>The keyboard-focus description when Hint is available.</summary>
    internal const string KeyboardDescriptionHintAvailable = "HINT  H";

    /// <summary>The keyboard-focus description when sound cannot be used.</summary>
    internal const string KeyboardDescriptionSoundUnavailable = "SOUND UNAVAILABLE";

    /// <summary>The keyboard-focus description when sound is enabled.</summary>
    internal const string KeyboardDescriptionSoundOn = "SOUND ON  M";

    /// <summary>The keyboard-focus description when sound is disabled.</summary>
    internal const string KeyboardDescriptionSoundOff = "SOUND OFF  M";

    /// <summary>The keyboard-focus description for the options control.</summary>
    internal const string KeyboardDescriptionOptions = "OPTIONS  O";

    /// <summary>The keyboard-focus description prefix for a non-empty stock pile.</summary>
    internal const string KeyboardDescriptionStockPrefix = "STOCK";

    /// <summary>The keyboard-focus description suffix for drawing from stock.</summary>
    internal const string KeyboardDescriptionStockDrawSuffix = "CARDS  ENTER DRAW";

    /// <summary>The keyboard-focus description when stock is empty and waste can be recycled.</summary>
    internal const string KeyboardDescriptionStockEmptyRecycle = "STOCK EMPTY  ENTER RECYCLE";

    /// <summary>The keyboard-focus description when both stock and waste are empty.</summary>
    internal const string KeyboardDescriptionStockAndWasteEmpty = "STOCK AND WASTE EMPTY";

    /// <summary>The keyboard-focus description for the waste pile.</summary>
    internal const string KeyboardDescriptionWaste = "WASTE";

    /// <summary>The keyboard-focus description prefix for foundation piles.</summary>
    internal const string KeyboardDescriptionFoundationPrefix = "FOUNDATION";

    /// <summary>The keyboard-focus description prefix for tableau columns.</summary>
    internal const string KeyboardDescriptionTableauPrefix = "TABLEAU";

    /// <summary>Suffix appended to a keyboard-focus description when a pile is empty.</summary>
    internal const string KeyboardDescriptionEmptySuffix = " EMPTY";

    /// <summary>Suffix appended when a focused card is face down.</summary>
    internal const string KeyboardDescriptionFaceDownSuffix = "  FACE DOWN";

    /// <summary>Suffix appended when the current target is legal for the selected cards.</summary>
    internal const string KeyboardDescriptionLegalTargetSuffix = "  LEGAL TARGET";

    /// <summary>Suffix appended when the current target is invalid for the selected cards.</summary>
    internal const string KeyboardDescriptionInvalidTargetSuffix = "  INVALID TARGET";

    /// <summary>Suffix prefix appended to indicate the number of selected cards.</summary>
    internal const string KeyboardDescriptionSelectedPrefix = "  SELECTED";

    /// <summary>Suffix suffix appended after the selected-card count.</summary>
    internal const string KeyboardDescriptionSelectedSuffix = "CARDS";

    /// <summary>Label shown on an empty stock pile to indicate dealing.</summary>
    internal const string BoardDealLabel = "DEAL";

    /// <summary>Label shown below the deal prompt on an empty stock pile.</summary>
    internal const string BoardAgainLabel = "AGAIN";

    /// <summary>Heading label for the stock pile.</summary>
    internal const string BoardStockLabel = "STOCK";

    /// <summary>Heading label for the waste pile.</summary>
    internal const string BoardWasteLabel = "WASTE";

    /// <summary>Heading label for the FreeCell holding cells.</summary>
    internal const string BoardFreeCellsLabel = "FREECELLS";

    /// <summary>Label shown over each individual FreeCell holding slot.</summary>
    internal const string BoardFreeCellLabel = "CELL";

    /// <summary>Placeholder rank shown in an empty foundation slot.</summary>
    internal const string BoardFoundationAceLabel = "A";

    /// <summary>Compact foundation build instruction used in larger-text mode.</summary>
    internal const string BoardFoundationToKingCompactLabel = "TO K";

    /// <summary>Full foundation build instruction used in normal-text mode.</summary>
    internal const string BoardFoundationToKingLabel = "TO KING";

    /// <summary>Heading label for foundation piles.</summary>
    internal const string BoardFoundationsLabel = "FOUNDATIONS";

    /// <summary>Placeholder rank shown in an empty tableau slot.</summary>
    internal const string BoardTableauKingLabel = "K";

    /// <summary>Toolbar game title label.</summary>
    internal const string ToolbarTitleLabel = "SOLITAIRE";

    /// <summary>Toolbar shortcut legend shown in larger-text mode.</summary>
    internal const string ToolbarShortcutsCompactLabel = "O OPTIONS";

    /// <summary>Toolbar shortcut legend shown in normal-text mode.</summary>
    internal const string ToolbarShortcutsLabel = "R NEW GAME  U UNDO  H HINT  M SOUND";

    /// <summary>Toolbar label for move count.</summary>
    internal const string ToolbarMovesLabel = "MOVES";

    /// <summary>Toolbar label for elapsed time.</summary>
    internal const string ToolbarTimeLabel = "TIME";

    /// <summary>Banner text shown after a completed win.</summary>
    internal const string BoardWinLabel = "YOU WIN!";

    /// <summary>Card-back caption used in high-contrast mode.</summary>
    internal const string CardBackLabel = "BACK";
}
