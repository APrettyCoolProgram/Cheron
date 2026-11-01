// 261101_code
// 261101_documentation

namespace Cheron.Games.Card.Solitaire;

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

    /// <summary>Returns whether the active variant is Yukon.</summary>
    /// <param name="variant">The active variant.</param>
    /// <returns><see langword="true"/> for Yukon; otherwise, <see langword="false"/>.</returns>
    public static bool IsYukon(this SolitaireVariant variant) => variant == SolitaireVariant.Yukon;

    /// <summary>Returns the next variant in prompt-order cycling.</summary>
    /// <param name="variant">The current variant.</param>
    /// <returns>The next variant option.</returns>
    public static SolitaireVariant NextPromptVariant(this SolitaireVariant variant) =>
        variant switch
        {
            SolitaireVariant.Klondike => SolitaireVariant.FreeCell,
            SolitaireVariant.FreeCell => SolitaireVariant.Spider,
            SolitaireVariant.Spider => SolitaireVariant.Yukon,
            _ => SolitaireVariant.Klondike
        };

    /// <summary>Returns the previous variant in prompt-order cycling.</summary>
    /// <param name="variant">The current variant.</param>
    /// <returns>The previous variant option.</returns>
    public static SolitaireVariant PreviousPromptVariant(this SolitaireVariant variant) =>
        variant switch
        {
            SolitaireVariant.Klondike => SolitaireVariant.Yukon,
            SolitaireVariant.FreeCell => SolitaireVariant.Klondike,
            SolitaireVariant.Spider => SolitaireVariant.FreeCell,
            _ => SolitaireVariant.Spider
        };
}
