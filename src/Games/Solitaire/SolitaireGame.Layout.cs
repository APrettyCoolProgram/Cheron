using System;
using Cheron.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cheron.Games.Solitaire;

/// <summary>Fits the logical card table to the current drawable area and maps pointer input consistently.</summary>
public sealed partial class SolitaireGame
{
    /// <summary>The current host-supplied viewport, initially matching the unscaled board.</summary>
    private GameViewport _viewport = new(new Point(1100, 800), new Point(1100, 800), new Point(1100, 800));

    /// <summary>The owned rasterizer state used to keep board artwork out of centered window margins.</summary>
    private RasterizerState _boardRasterizerState = null!;

    /// <summary>Applies window or DPI changes without modifying the deal or interrupting existing card transitions.</summary>
    /// <remarks>
    /// A pending pointer gesture is cancelled to prevent a resize from turning a held press into an unintended move.
    /// A dragged stack returns to its source; elapsed time, history, hints, and ordinary selection are preserved.
    /// </remarks>
    /// <param name="viewport">The host-computed drawing transform and mouse-coordinate mapping.</param>
    public void SetViewport(GameViewport viewport)
    {
        if (viewport == _viewport)
        {
            return;
        }

        var before = _isDragging ? CaptureCardPresentation() : null;
        ResetPointerGesture();
        if (before is not null)
        {
            ClearSelection();
            AnimateChanges(before);
        }

        _viewport = viewport;
        _pointerPosition = new Point(-1, -1);
    }

    /// <summary>Maps client-space pointer updates to the board before applying any gesture or hit test.</summary>
    /// <param name="clientPosition">The pointer position in window client units.</param>
    /// <param name="isDown">Whether the left button is currently down.</param>
    /// <param name="time">The monotonically increasing game time used for double-clicks.</param>
    private void UpdateClientPointer(Point clientPosition, bool isDown, TimeSpan time)
    {
        UpdatePointer(_viewport.ToLogical(clientPosition), isDown, time);
    }

    /// <summary>Starts a clipped batch, composing local animation transforms before viewport scaling.</summary>
    /// <param name="localTransform">The optional board-space transform used for a card flip.</param>
    private void BeginBoardDrawing(Matrix? localTransform = null)
    {
        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, rasterizerState: _boardRasterizerState,
            transformMatrix: (localTransform ?? Matrix.Identity) * _viewport.Transform);
    }
}
