// 261028_code
// 261028_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Provides helper methods for variant-based branching in shared orchestration code.</summary>
internal static class SolitaireVariantExtensions
{
    /// <summary>Returns whether the active variant is FreeCell.</summary>
    /// <param name="variant">The active variant.</param>
    /// <returns><see langword="true"/> for FreeCell; otherwise, <see langword="false"/>.</returns>
    public static bool IsFreeCell(this SolitaireVariant variant) => variant == SolitaireVariant.FreeCell;

    /// <summary>Toggles between the two supported Solitaire variants.</summary>
    /// <param name="variant">The current variant.</param>
    /// <returns>The alternate variant.</returns>
    public static SolitaireVariant ToggleVariant(this SolitaireVariant variant) =>
        variant == SolitaireVariant.Klondike ? SolitaireVariant.FreeCell : SolitaireVariant.Klondike;
}
