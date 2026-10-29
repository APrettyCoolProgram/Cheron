// 261028_code
// 261028_documentation

using static Cheron.Games.Solitaire.SolitaireTextConstants;

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes accessibility options metadata and row-label formatting.</summary>
internal static class SolitaireAccessibilityOptions
{
    /// <summary>The number of rows in the accessibility options modal.</summary>
    internal const int OptionCount = 4;

    /// <summary>The index of the final accessibility option row.</summary>
    internal const int LastOptionIndex = OptionCount - 1;

    /// <summary>Formats one accessibility option row label, including its current ON/OFF state when applicable.</summary>
    /// <param name="index">The zero-based option row index.</param>
    /// <param name="largeText">Whether larger text is enabled.</param>
    /// <param name="highContrast">Whether high contrast is enabled.</param>
    /// <param name="reducedMotion">Whether reduced motion is enabled.</param>
    /// <returns>The option row label text.</returns>
    internal static string GetOptionLabel(int index, bool largeText, bool highContrast, bool reducedMotion) => index switch
    {
        0 => $"F2  LARGER TEXT  {(largeText ? "ON" : "OFF")}",
        1 => $"F3  HIGH CONTRAST  {(highContrast ? "ON" : "OFF")}",
        2 => $"F4  REDUCED MOTION  {(reducedMotion ? "ON" : "OFF")}",
        _ => AccessibilityCloseOptionsLabel
    };
}
