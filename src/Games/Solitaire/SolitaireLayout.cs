// 261028_code
// 261028_documentation

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

    /// <summary>The horizontal position of the stock pile slot.</summary>
    internal const int StockX = 32;

    /// <summary>The horizontal position of the waste pile slot.</summary>
    internal const int WasteX = 128;

    /// <summary>The vertical position shared by stock and waste pile slots.</summary>
    internal const int StockWasteY = 82;

    /// <summary>The new-game button bounds shared by drawing and pointer handling.</summary>
    internal static readonly Rectangle NewGameButtonBounds = new(32, 20, 132, 44);

    /// <summary>The undo button bounds in logical board coordinates.</summary>
    internal static readonly Rectangle UndoButtonBounds = new(180, 20, 104, 44);

    /// <summary>The hint button bounds in logical board coordinates.</summary>
    internal static readonly Rectangle HintButtonBounds = new(300, 20, 104, 44);

    /// <summary>The sound button bounds, shared by drawing and pointer input.</summary>
    internal static readonly Rectangle SoundButtonBounds = new(1008, 20, 68, 44);

    /// <summary>The always-available settings button in logical board coordinates.</summary>
    internal static readonly Rectangle OptionsButtonBounds = new(882, 742, 186, 40);

    /// <summary>The win banner bounds in logical board coordinates.</summary>
    internal static readonly Rectangle WinBannerBounds = new(370, 376, 360, 56);

    /// <summary>The horizontal position of the win text inside the win banner.</summary>
    internal const int WinTextX = 483;

    /// <summary>The vertical position of the win text inside the win banner.</summary>
    internal const int WinTextY = 394;

    /// <summary>Gets the stock pile bounds in logical board coordinates.</summary>
    /// <returns>The stock pile rectangle.</returns>
    internal static Rectangle GetStockBounds() => new(StockX, StockWasteY, CardWidth, CardHeight);

    /// <summary>Gets the waste pile bounds in logical board coordinates.</summary>
    /// <returns>The waste pile rectangle.</returns>
    internal static Rectangle GetWasteBounds() => new(WasteX, StockWasteY, CardWidth, CardHeight);

    /// <summary>Calculates a foundation slot's bounds in logical board coordinates.</summary>
    /// <param name="foundationIndex">The zero-based index of the foundation slot.</param>
    /// <returns>The rectangle used to draw and hit-test the foundation.</returns>
    internal static Rectangle GetFoundationBounds(int foundationIndex) =>
        new(BoardWidth - 32 - (4 - foundationIndex) * (CardWidth + CardGap), StockWasteY, CardWidth, CardHeight);
}
