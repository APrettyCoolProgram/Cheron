// 261101_code
// 261101_documentation

using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes fixed board dimensions and shared UI/card layout geometry for Solitaire.</summary>
internal static class SolitaireLayout
{
    /// <summary>The width of a card or empty pile slot, in logical board units.</summary>
    internal const int CardWidth = 78;

    /// <summary>The height of a card or empty pile slot, in logical board units.</summary>
    internal const int CardHeight = 108;

    /// <summary>The horizontal space between adjacent foundation slots, in logical board units.</summary>
    internal const int CardGap = 18;

    /// <summary>The vertical position of the first card in every tableau column, in logical board units.</summary>
    internal const int TableauTop = 218;

    /// <summary>The vertical offset between consecutive tableau cards when the preceding card is face up.</summary>
    internal const int TableauStep = 29;

    /// <summary>The vertical offset between consecutive tableau cards when the preceding card is face down.</summary>
    internal const int TableauFaceDownStep = 22;

    /// <summary>The horizontal inset used for the high-contrast card-back label.</summary>
    internal const int CardBackLabelOffsetX = 16;

    /// <summary>The vertical inset used for the high-contrast card-back label.</summary>
    internal const int CardBackLabelOffsetY = 46;

    /// <summary>The horizontal offset applied to card shadow bounds.</summary>
    internal const int CardShadowOffsetX = -6;

    /// <summary>The vertical offset applied to card shadow bounds.</summary>
    internal const int CardShadowOffsetY = -4;

    /// <summary>The width expansion applied to card shadow bounds.</summary>
    internal const int CardShadowExpandWidth = 16;

    /// <summary>The height expansion applied to card shadow bounds.</summary>
    internal const int CardShadowExpandHeight = 16;

    /// <summary>The inset used for selection highlight bounds around a card.</summary>
    internal const int CardSelectionInset = 3;

    /// <summary>The horizontal offset for the top-left rank label on a face-up card.</summary>
    internal const int CardRankTopOffsetX = 8;

    /// <summary>The vertical offset for the top-left rank label in larger-text mode.</summary>
    internal const int CardRankTopOffsetYLarge = 5;

    /// <summary>The vertical offset for the top-left rank label in normal-text mode.</summary>
    internal const int CardRankTopOffsetY = 8;

    /// <summary>The horizontal offset from the right edge for the top-right small suit icon.</summary>
    internal const int CardSuitTopRightOffset = 26;

    /// <summary>The vertical offset for the top-right small suit icon.</summary>
    internal const int CardSuitTopOffsetY = 5;

    /// <summary>The size of the top-right small suit icon.</summary>
    internal const int CardSuitTopSize = 20;

    /// <summary>The horizontal offset for the center suit icon.</summary>
    internal const int CardSuitCenterOffsetX = 19;

    /// <summary>The vertical offset for the center suit icon.</summary>
    internal const int CardSuitCenterOffsetY = 35;

    /// <summary>The size of the center suit icon.</summary>
    internal const int CardSuitCenterSize = 40;

    /// <summary>The horizontal offset for the bottom-left small suit icon.</summary>
    internal const int CardSuitBottomOffsetX = 7;

    /// <summary>The vertical offset from the bottom for the bottom-left small suit icon.</summary>
    internal const int CardSuitBottomOffset = 25;

    /// <summary>The size of the bottom-left small suit icon.</summary>
    internal const int CardSuitBottomSize = 18;

    /// <summary>The horizontal padding from the right edge for the bottom-right rank label.</summary>
    internal const int CardRankBottomPaddingX = 7;

    /// <summary>The vertical padding from the bottom edge for the bottom-right rank label.</summary>
    internal const int CardRankBottomPaddingY = 8;

    /// <summary>The amount by which target outlines are inflated beyond their source bounds.</summary>
    internal const int TargetOutlineInflate = 2;

    /// <summary>The thickness of target-outline edges.</summary>
    internal const int TargetOutlineThickness = 2;

    /// <summary>The amount by which focus-corner bounds are inflated.</summary>
    internal const int FocusCornerInflate = 5;

    /// <summary>The length of each focus-corner segment.</summary>
    internal const int FocusCornerLength = 10;

    /// <summary>The thickness of each focus-corner segment.</summary>
    internal const int FocusCornerThickness = 3;

    /// <summary>The horizontal position of the first tableau column, in logical board units.</summary>
    internal const int FirstColumnX = 32;

    /// <summary>The horizontal offset between consecutive tableau columns, in logical board units.</summary>
    internal const int ColumnStep = 148;

    /// <summary>The logical board width used for viewport fitting and layout math.</summary>
    internal const int BoardWidth = 1100;

    /// <summary>The logical board height used for viewport fitting and layout math.</summary>
    internal const int BoardHeight = 800;

    /// <summary>The height of the top toolbar strip.</summary>
    internal const int TopBarHeight = 72;

    /// <summary>The vertical position of the top toolbar divider line.</summary>
    internal const int TopBarDividerY = 71;

    /// <summary>The vertical position of the footer strip.</summary>
    internal const int FooterY = 728;

    /// <summary>The height of the footer strip.</summary>
    internal const int FooterHeight = 72;

    /// <summary>The horizontal position of footer status/instruction text.</summary>
    internal const int FooterTextX = 32;

    /// <summary>The vertical position of the footer status text line.</summary>
    internal const int FooterStatusY = 741;

    /// <summary>The vertical position of the footer instruction text line.</summary>
    internal const int FooterInstructionsY = 767;

    /// <summary>The horizontal position of the toolbar title label.</summary>
    internal const int ToolbarTitleX = 440;

    /// <summary>The vertical position of the toolbar title label.</summary>
    internal const int ToolbarTitleY = 26;

    /// <summary>The horizontal position of the toolbar shortcut legend.</summary>
    internal const int ToolbarShortcutsX = 440;

    /// <summary>The vertical position of the toolbar shortcut legend.</summary>
    internal const int ToolbarShortcutsY = 51;

    /// <summary>The horizontal position of the toolbar move statistics block.</summary>
    internal const int ToolbarMovesX = 740;

    /// <summary>The horizontal position of the toolbar time statistics block.</summary>
    internal const int ToolbarTimeX = 850;

    /// <summary>The vertical position of toolbar statistics captions.</summary>
    internal const int ToolbarStatsCaptionY = 24;

    /// <summary>The vertical position of toolbar statistics values.</summary>
    internal const int ToolbarStatsValueY = 44;

    /// <summary>The horizontal position of the stock pile slot.</summary>
    internal const int StockX = 32;

    /// <summary>The horizontal position of the waste pile slot.</summary>
    internal const int WasteX = 128;

    /// <summary>The vertical position shared by stock and waste pile slots.</summary>
    internal const int StockWasteY = 82;

    /// <summary>The horizontal position of the stock heading in larger-text mode.</summary>
    internal const int StockLabelLargeTextX = 36;

    /// <summary>The horizontal position of the stock heading in normal-text mode.</summary>
    internal const int StockLabelX = 40;

    /// <summary>The horizontal position of the waste heading in larger-text mode.</summary>
    internal const int WasteLabelLargeTextX = 132;

    /// <summary>The horizontal position of the waste heading in normal-text mode.</summary>
    internal const int WasteLabelX = 136;

    /// <summary>The vertical position of stock/waste/foundations heading labels.</summary>
    internal const int PileHeadingY = 199;

    /// <summary>The horizontal position of the foundations heading in larger-text mode.</summary>
    internal const int FoundationsLabelLargeTextX = 802;

    /// <summary>The horizontal position of the foundations heading in normal-text mode.</summary>
    internal const int FoundationsLabelX = 796;

    /// <summary>The horizontal position of the deal prompt in larger-text mode.</summary>
    internal const int DealLabelLargeTextX = 35;

    /// <summary>The horizontal position of the deal prompt in normal-text mode.</summary>
    internal const int DealLabelX = 47;

    /// <summary>The vertical position of the deal prompt label.</summary>
    internal const int DealLabelY = 122;

    /// <summary>The horizontal position of the again prompt in larger-text mode.</summary>
    internal const int AgainLabelLargeTextX = 38;

    /// <summary>The horizontal position of the again prompt in normal-text mode.</summary>
    internal const int AgainLabelX = 44;

    /// <summary>The vertical position of the again prompt in larger-text mode.</summary>
    internal const int AgainLabelLargeTextY = 142;

    /// <summary>The vertical position of the again prompt in normal-text mode.</summary>
    internal const int AgainLabelY = 138;

    /// <summary>The horizontal offset of the empty-foundation ace label relative to foundation bounds.</summary>
    internal const int FoundationAceOffsetX = 33;

    /// <summary>The vertical offset of the empty-foundation ace label relative to foundation bounds.</summary>
    internal const int FoundationAceOffsetY = 24;

    /// <summary>The horizontal offset of the empty-foundation instruction relative to foundation bounds.</summary>
    internal const int FoundationInstructionOffsetX = 19;

    /// <summary>The vertical offset of the empty-foundation instruction relative to foundation bounds.</summary>
    internal const int FoundationInstructionOffsetY = 65;

    /// <summary>The horizontal offset of the empty-tableau king label relative to slot bounds.</summary>
    internal const int TableauKingOffsetX = 33;

    /// <summary>The vertical offset of the empty-tableau king label relative to slot bounds.</summary>
    internal const int TableauKingOffsetY = 46;

    /// <summary>The new-game button bounds shared by drawing and pointer handling.</summary>
    internal static readonly Rectangle NewGameButtonBounds = new(12, 20, 100, 44);

    /// <summary>The undo button bounds in logical board coordinates.</summary>
    internal static readonly Rectangle UndoButtonBounds = new(116, 20, 60, 44);

    /// <summary>The hint button bounds in logical board coordinates.</summary>
    internal static readonly Rectangle HintButtonBounds = new(180, 20, 60, 44);

    /// <summary>The help button bounds in logical board coordinates.</summary>
    internal static readonly Rectangle HelpButtonBounds = new(830, 20, 72, 44);

    /// <summary>The footer sound toggle bounds, shared by drawing and pointer input.</summary>
    internal static readonly Rectangle SoundButtonBounds = new(902, 20, 186, 44);

    /// <summary>The footer options action bounds in logical board coordinates.</summary>
    internal static readonly Rectangle OptionsButtonBounds = new(734, 20, 96, 44);

    /// <summary>The win banner bounds in logical board coordinates.</summary>
    internal static readonly Rectangle WinBannerBounds = new(370, 376, 360, 56);

    /// <summary>The horizontal position of the win text inside the win banner.</summary>
    internal const int WinTextX = 483;

    /// <summary>The vertical position of the win text inside the win banner.</summary>
    internal const int WinTextY = 394;

    /// <summary>Gets the full board bounds in logical board coordinates.</summary>
    /// <returns>The full board rectangle.</returns>
    internal static Rectangle GetBoardBounds() => new(0, 0, BoardWidth, BoardHeight);

    /// <summary>Gets the top toolbar strip bounds in logical board coordinates.</summary>
    /// <returns>The top toolbar rectangle.</returns>
    internal static Rectangle GetTopBarBounds() => new(0, 0, BoardWidth, TopBarHeight);

    /// <summary>Gets the top toolbar divider bounds in logical board coordinates.</summary>
    /// <returns>The top toolbar divider rectangle.</returns>
    internal static Rectangle GetTopBarDividerBounds() => new(0, TopBarDividerY, BoardWidth, 1);

    /// <summary>Gets the footer strip bounds in logical board coordinates.</summary>
    /// <returns>The footer strip rectangle.</returns>
    internal static Rectangle GetFooterBounds() => new(0, FooterY, BoardWidth, FooterHeight);

    /// <summary>Gets the footer divider bounds in logical board coordinates.</summary>
    /// <returns>The footer divider rectangle.</returns>
    internal static Rectangle GetFooterDividerBounds() => new(0, FooterY, BoardWidth, 1);

    /// <summary>Gets the stock pile bounds in logical board coordinates.</summary>
    /// <returns>The stock pile rectangle.</returns>
    internal static Rectangle GetStockBounds() => new(StockX, StockWasteY, CardWidth, CardHeight);

    /// <summary>Gets the waste pile bounds in logical board coordinates.</summary>
    /// <returns>The waste pile rectangle.</returns>
    internal static Rectangle GetWasteBounds() => new(WasteX, StockWasteY, CardWidth, CardHeight);

    /// <summary>Calculates a FreeCell holding slot's bounds in logical board coordinates.</summary>
    /// <param name="freeCellIndex">The zero-based index of the FreeCell holding slot.</param>
    /// <returns>The rectangle used to draw and hit-test the FreeCell slot.</returns>
    internal static Rectangle GetFreeCellBounds(int freeCellIndex) =>
        new(StockX + freeCellIndex * (CardWidth + CardGap), StockWasteY, CardWidth, CardHeight);

    /// <summary>Gets the outer panel bounds for the new-game variant picker prompt.</summary>
    /// <returns>The prompt panel bounds.</returns>
    internal static Rectangle GetNewGamePromptPanelBounds() =>
        new(BoardWidth / 2 - 380, BoardHeight / 2 - 120, 760, 240);

    /// <summary>Gets the Klondike option bounds within the new-game variant picker prompt.</summary>
    /// <returns>The Klondike option bounds.</returns>
    internal static Rectangle GetNewGamePromptKlondikeBounds() =>
        new(GetNewGamePromptPanelBounds().X + 30, GetNewGamePromptPanelBounds().Y + 82, 160, 52);

    /// <summary>Gets the FreeCell option bounds within the new-game variant picker prompt.</summary>
    /// <returns>The FreeCell option bounds.</returns>
    internal static Rectangle GetNewGamePromptFreeCellBounds() =>
        new(GetNewGamePromptPanelBounds().X + 210, GetNewGamePromptPanelBounds().Y + 82, 160, 52);

    /// <summary>Gets the Spider option bounds within the new-game variant picker prompt.</summary>
    /// <returns>The Spider option bounds.</returns>
    internal static Rectangle GetNewGamePromptSpiderBounds() =>
        new(GetNewGamePromptPanelBounds().X + 390, GetNewGamePromptPanelBounds().Y + 82, 160, 52);

    /// <summary>Gets the Yukon option bounds within the new-game variant picker prompt.</summary>
    /// <returns>The Yukon option bounds.</returns>
    internal static Rectangle GetNewGamePromptYukonBounds() =>
        new(GetNewGamePromptPanelBounds().X + 570, GetNewGamePromptPanelBounds().Y + 82, 160, 52);

    /// <summary>Gets the confirm action bounds for the new-game variant picker prompt.</summary>
    /// <returns>The confirm action bounds.</returns>
    internal static Rectangle GetNewGamePromptConfirmBounds() =>
        new(GetNewGamePromptPanelBounds().X + 237, GetNewGamePromptPanelBounds().Bottom - 76, 286, 24);

    /// <summary>Gets the cancel action bounds for the new-game variant picker prompt.</summary>
    /// <returns>The cancel action bounds.</returns>
    internal static Rectangle GetNewGamePromptCancelBounds() =>
        new(GetNewGamePromptPanelBounds().X + 237, GetNewGamePromptPanelBounds().Bottom - 46, 286, 24);

    /// <summary>Gets the outer panel bounds for the in-game Help overlay.</summary>
    /// <returns>The Help overlay panel bounds.</returns>
    internal static Rectangle GetHelpPanelBounds() => new(146, 116, 808, 568);

    /// <summary>Gets the clipping bounds used for scrollable Help body text.</summary>
    /// <returns>The Help content viewport bounds.</returns>
    internal static Rectangle GetHelpContentBounds() =>
        new(GetHelpPanelBounds().X + 34, GetHelpPanelBounds().Y + 92, GetHelpPanelBounds().Width - 68,
            GetHelpPanelBounds().Height - 162);

    /// <summary>Gets the close hint bounds at the bottom of the Help overlay.</summary>
    /// <returns>The Help overlay close hint bounds.</returns>
    internal static Rectangle GetHelpCloseHintBounds() =>
        new(GetHelpPanelBounds().X + 34, GetHelpPanelBounds().Bottom - 52, GetHelpPanelBounds().Width - 68, 24);

    /// <summary>Calculates a foundation slot's bounds in logical board coordinates.</summary>
    /// <param name="foundationIndex">The zero-based index of the foundation slot.</param>
    /// <returns>The rectangle used to draw and hit-test the foundation.</returns>
    internal static Rectangle GetFoundationBounds(int foundationIndex) =>
        new(BoardWidth - 32 - (4 - foundationIndex) * (CardWidth + CardGap), StockWasteY, CardWidth, CardHeight);
}
