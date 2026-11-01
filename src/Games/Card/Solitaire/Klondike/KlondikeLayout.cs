// 261029_code
// 261029_documentation

namespace Cheron.Games.Card.Solitaire.Klondike;

/// <summary>Calculates Klondike-specific tableau layout positions.</summary>
internal static class KlondikeLayout
{
    /// <summary>Calculates the horizontal position for one Klondike tableau column.</summary>
    /// <param name="columnIndex">The zero-based tableau index.</param>
    /// <returns>The left edge of the tableau column in logical board coordinates.</returns>
    public static int GetTableauColumnX(int columnIndex) =>
        SolitaireLayout.FirstColumnX + columnIndex * SolitaireLayout.ColumnStep;
}
