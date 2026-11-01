// 261101_code
// 261101_documentation

namespace Cheron.Games.Card.Solitaire.Yukon;

/// <summary>Calculates Yukon-specific tableau layout positions.</summary>
internal static class YukonLayout
{
    /// <summary>Calculates the horizontal position for one Yukon tableau column.</summary>
    /// <param name="columnIndex">The zero-based tableau index.</param>
    /// <returns>The left edge of the tableau column in logical board coordinates.</returns>
    public static int GetTableauColumnX(int columnIndex) =>
        SolitaireLayout.FirstColumnX + columnIndex * SolitaireLayout.ColumnStep;
}
