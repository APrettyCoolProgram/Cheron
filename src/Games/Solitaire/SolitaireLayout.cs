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

    /// <summary>The horizontal position of the first tableau column, in logical board units.</summary>
    internal const int FirstColumnX = 32;

    /// <summary>The horizontal offset between consecutive tableau columns, in logical board units.</summary>
    internal const int ColumnStep = 148;

    /// <summary>The logical board width used for viewport fitting and layout math.</summary>
    internal const int BoardWidth = 1100;

    /// <summary>The logical board height used for viewport fitting and layout math.</summary>
    internal const int BoardHeight = 800;

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

    /// <summary>Calculates a foundation slot's bounds in logical board coordinates.</summary>
    /// <param name="foundationIndex">The zero-based index of the foundation slot.</param>
    /// <returns>The rectangle used to draw and hit-test the foundation.</returns>
    internal static Rectangle GetFoundationBounds(int foundationIndex) =>
        new(BoardWidth - 32 - (4 - foundationIndex) * (CardWidth + CardGap), 82, CardWidth, CardHeight);
}
