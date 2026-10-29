// 261028_code
// 261028_documentation

using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes fixed geometry for Solitaire's accessibility options panel.</summary>
internal static class SolitaireOptionsLayout
{
    /// <summary>The horizontal position of the options panel frame.</summary>
    internal const int PanelX = 160;

    /// <summary>The vertical position of the options panel frame.</summary>
    internal const int PanelY = 180;

    /// <summary>The width of the options panel frame.</summary>
    internal const int PanelWidth = 780;

    /// <summary>The height of the options panel frame.</summary>
    internal const int PanelHeight = 444;

    /// <summary>The horizontal position of the first option row.</summary>
    internal const int FirstRowX = 196;

    /// <summary>The vertical position of the first option row.</summary>
    internal const int FirstRowY = 250;

    /// <summary>The vertical offset between adjacent option rows.</summary>
    internal const int RowStep = 56;

    /// <summary>The width of each option row.</summary>
    internal const int RowWidth = 708;

    /// <summary>The height of each option row.</summary>
    internal const int RowHeight = 44;

    /// <summary>Gets a settings row's shared drawing and hit-test bounds.</summary>
    /// <param name="index">The row index.</param>
    /// <returns>The row bounds in logical board coordinates.</returns>
    internal static Rectangle GetOptionBounds(int index) => new(FirstRowX, FirstRowY + index * RowStep, RowWidth, RowHeight);
}
