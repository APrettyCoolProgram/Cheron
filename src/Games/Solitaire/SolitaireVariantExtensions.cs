// 261029_code
// 261029_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Provides helper methods for variant-based branching in shared orchestration code.</summary>
internal static class SolitaireVariantExtensions
{
    /// <summary>Returns whether the active variant is FreeCell.</summary>
    /// <param name="variant">The active variant.</param>
    /// <returns><see langword="true"/> for FreeCell; otherwise, <see langword="false"/>.</returns>
    public static bool IsFreeCell(this SolitaireVariant variant) => variant == SolitaireVariant.FreeCell;

    /// <summary>Returns whether the active variant is Spider.</summary>
    /// <param name="variant">The active variant.</param>
    /// <returns><see langword="true"/> for Spider; otherwise, <see langword="false"/>.</returns>
    public static bool IsSpider(this SolitaireVariant variant) => variant == SolitaireVariant.Spider;

    /// <summary>Returns the next variant in prompt-order cycling.</summary>
    /// <param name="variant">The current variant.</param>
    /// <returns>The next variant option.</returns>
    public static SolitaireVariant NextPromptVariant(this SolitaireVariant variant) =>
        variant switch
        {
            SolitaireVariant.Klondike => SolitaireVariant.FreeCell,
            SolitaireVariant.FreeCell => SolitaireVariant.Spider,
            _ => SolitaireVariant.Klondike
        };

    /// <summary>Returns the previous variant in prompt-order cycling.</summary>
    /// <param name="variant">The current variant.</param>
    /// <returns>The previous variant option.</returns>
    public static SolitaireVariant PreviousPromptVariant(this SolitaireVariant variant) =>
        variant switch
        {
            SolitaireVariant.Klondike => SolitaireVariant.Spider,
            SolitaireVariant.FreeCell => SolitaireVariant.Klondike,
            _ => SolitaireVariant.FreeCell
        };
}
