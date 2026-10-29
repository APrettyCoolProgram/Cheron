// 261028_code
// 261028_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes toolbar labels and hint text used by the Solitaire UI.</summary>
internal static class SolitaireTextConstants
{
    /// <summary>The toolbar label for starting a new deal.</summary>
    internal const string NewGameLabel = "NEW GAME";

    /// <summary>The toolbar label for undoing the last action.</summary>
    internal const string UndoLabel = "UNDO";

    /// <summary>The toolbar label for requesting a one-step hint.</summary>
    internal const string HintLabel = "HINT";

    /// <summary>The toolbar status label when sound cannot be used.</summary>
    internal const string AudioUnavailableLabel = "NA";

    /// <summary>The toolbar status label when sound is enabled.</summary>
    internal const string SoundEnabledLabel = "SND";

    /// <summary>The toolbar status label when sound is disabled.</summary>
    internal const string SoundDisabledLabel = "OFF";

    /// <summary>Hint text for moving a card to a foundation.</summary>
    internal const string HintToFoundationMessage = "HINT  MOVE GOLD CARD TO GOLD FOUNDATION";

    /// <summary>Hint text for moving a card to a tableau column.</summary>
    internal const string HintToTableauMessage = "HINT  MOVE GOLD CARD TO GOLD TABLEAU";

    /// <summary>Hint text for drawing from a non-empty stock.</summary>
    internal const string HintDrawFromStockMessage = "HINT  CLICK STOCK TO DRAW";

    /// <summary>Hint text for recycling when stock is empty.</summary>
    internal const string HintRecycleStockMessage = "HINT  CLICK EMPTY STOCK TO RECYCLE";

    /// <summary>Hint text shown when no immediate suggestion exists.</summary>
    internal const string HintNoSuggestionMessage = "HINT  NO SUGGESTED MOVE  TRY UNDO OR NEW GAME";
}
