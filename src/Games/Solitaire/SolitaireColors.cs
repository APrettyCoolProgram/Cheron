// 261028_code
// 261028_documentation

using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes recurring UI colors used by Solitaire rendering.</summary>
internal static class SolitaireColors
{
    /// <summary>Primary light-ink color used for major toolbar text.</summary>
    internal static readonly Color ToolbarPrimaryText = new(246, 239, 218);

    /// <summary>Muted label color used for toolbar hints and small status text.</summary>
    internal static readonly Color ToolbarSecondaryText = new(153, 183, 176);

    /// <summary>Muted board heading color used for stock, waste, and foundation captions.</summary>
    internal static readonly Color BoardHeadingText = new(157, 184, 176);

    /// <summary>Muted placeholder color used for empty-slot rank/instruction text.</summary>
    internal static readonly Color BoardPlaceholderText = new(119, 153, 145);

    /// <summary>Divider color used for board top/footer separator lines.</summary>
    internal static readonly Color BoardSeparatorLine = new(67, 102, 102);

    /// <summary>Soft accent color used for empty-stock prompts.</summary>
    internal static readonly Color BoardDealPromptText = new(156, 186, 178);

    /// <summary>Gold accent used for win and hint highlighting.</summary>
    internal static readonly Color GoldHighlight = new(251, 219, 148);

    /// <summary>Mint accent used for legal-target and celebration highlights.</summary>
    internal static readonly Color MintHighlight = new(112, 220, 182);

    /// <summary>Base clear color used for the playfield background.</summary>
    internal static readonly Color BoardClearColor = new(12, 34, 39);

    /// <summary>Overlay tint used for the top toolbar strip in normal mode.</summary>
    internal static readonly Color TopBarTint = new(12, 31, 37, 220);

    /// <summary>Overlay tint used for the footer strip in normal mode.</summary>
    internal static readonly Color FooterTint = new(12, 31, 37, 230);

    /// <summary>Soft red feedback color used for invalid-move pulse text/highlights in normal mode.</summary>
    internal static readonly Color InvalidFeedbackColor = new(241, 150, 131);

    /// <summary>High-contrast disabled button fill color.</summary>
    internal static readonly Color ButtonDisabledHighContrastFill = new(48, 48, 48);

    /// <summary>Normal-theme disabled button tint.</summary>
    internal static readonly Color ButtonDisabledTint = new(104, 115, 113);

    /// <summary>Normal-theme enabled button text color.</summary>
    internal static readonly Color ButtonEnabledInk = new(31, 56, 59);

    /// <summary>Normal-theme disabled button text color.</summary>
    internal static readonly Color ButtonDisabledInk = new(44, 62, 63);

    /// <summary>Pale blue hover-outline color for pointer target previews.</summary>
    internal static readonly Color HoverOutline = new(191, 215, 227);

    /// <summary>Dark slate fill used for accessibility option rows.</summary>
    internal static readonly Color OptionsRowFill = new(28, 43, 48);

    /// <summary>Footer primary status text color.</summary>
    internal static readonly Color FooterPrimaryText = new(207, 220, 207);

    /// <summary>Card suit red color in normal theme.</summary>
    internal static readonly Color CardSuitRed = new(180, 51, 65);

    /// <summary>Card suit black color in normal theme.</summary>
    internal static readonly Color CardSuitBlack = new(36, 57, 66);

    /// <summary>Card suit red color in high-contrast mode.</summary>
    internal static readonly Color CardSuitRedHighContrast = new(145, 0, 20);
}
