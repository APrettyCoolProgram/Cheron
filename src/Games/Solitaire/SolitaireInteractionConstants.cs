// 261028_code
// 261028_documentation

using System;

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes pointer and hint interaction timing/threshold values for Solitaire.</summary>
internal static class SolitaireInteractionConstants
{
    /// <summary>The minimum pointer movement in logical board units required to start dragging.</summary>
    internal const int DragThreshold = 6;

    /// <summary>The maximum interval between two clicks on the same card.</summary>
    internal static readonly TimeSpan DoubleClickInterval = TimeSpan.FromMilliseconds(350);

    /// <summary>The amount of time a hint remains visible after being set.</summary>
    internal static readonly TimeSpan HintDisplayDuration = TimeSpan.FromSeconds(4);
}
