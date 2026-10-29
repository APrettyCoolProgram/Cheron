// 261028_code
// 261028_documentation

using System;
using Microsoft.Xna.Framework;

namespace Cheron.Engine;

/// <summary>Maps a fixed logical canvas to a centered, aspect-preserving area of a drawable back buffer.</summary>
/// <remarks>
/// Client coordinates are the units reported by MonoGame mouse input. Back-buffer coordinates are drawable pixels;
/// their sizes may differ on high-DPI displays. No render target or graphics-device resource is owned here.
/// </remarks>
public readonly record struct GameViewport
{
    /// <summary>Gets the game's fixed logical canvas size.</summary>
    public Point LogicalSize { get; }

    /// <summary>Gets the window client size in mouse-coordinate units.</summary>
    public Point ClientSize { get; }

    /// <summary>Gets the drawable back-buffer size in pixels.</summary>
    public Point BackBufferSize { get; }

    /// <summary>Gets the uniform logical-to-drawable scale.</summary>
    public float Scale { get; }

    /// <summary>Gets the centered canvas offset in drawable pixels.</summary>
    public Vector2 Offset { get; }

    /// <summary>Gets the logical-to-back-buffer drawing transform.</summary>
    public Matrix Transform => Matrix.CreateScale(Scale, Scale, 1) *
        Matrix.CreateTranslation(Offset.X, Offset.Y, 0);

    /// <summary>Gets the pixel rectangle enclosing the canvas, clipped to valid drawable pixels.</summary>
    public Rectangle ScissorBounds => Rectangle.Intersect(new Rectangle(0, 0, BackBufferSize.X, BackBufferSize.Y),
        new Rectangle((int)MathF.Floor(Offset.X), (int)MathF.Floor(Offset.Y),
            (int)MathF.Ceiling(Offset.X + LogicalSize.X * Scale) - (int)MathF.Floor(Offset.X),
            (int)MathF.Ceiling(Offset.Y + LogicalSize.Y * Scale) - (int)MathF.Floor(Offset.Y)));

    /// <summary>Creates a viewport from logical, client, and drawable sizes without assuming a DPI scale.</summary>
    /// <param name="logicalSize">The fixed logical canvas dimensions.</param>
    /// <param name="clientSize">The current client dimensions in mouse-coordinate units.</param>
    /// <param name="backBufferSize">The current drawable dimensions in pixels.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any supplied dimension is not positive.
    /// </exception>
    public GameViewport(Point logicalSize, Point clientSize, Point backBufferSize)
    {
        if (logicalSize.X <= 0 || logicalSize.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalSize), "Canvas dimensions must be positive.");
        }

        if (clientSize.X <= 0 || clientSize.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(clientSize), "Client dimensions must be positive.");
        }

        if (backBufferSize.X <= 0 || backBufferSize.Y <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(backBufferSize), "Drawable dimensions must be positive.");
        }

        LogicalSize = logicalSize;
        ClientSize = clientSize;
        BackBufferSize = backBufferSize;
        Scale = Math.Min((float)backBufferSize.X / logicalSize.X, (float)backBufferSize.Y / logicalSize.Y);
        Offset = new Vector2((backBufferSize.X - logicalSize.X * Scale) / 2,
            (backBufferSize.Y - logicalSize.Y * Scale) / 2);
    }

    /// <summary>Converts mouse coordinates to logical coordinates without clamping margins onto the canvas.</summary>
    /// <remarks>
    /// Positions in centered margins remain outside the logical canvas, allowing normal hit tests to reject them.
    /// Coordinates outside the window are also preserved for dragging and releasing beyond the board.
    /// </remarks>
    /// <param name="clientPosition">The pointer position in window client units.</param>
    /// <returns>The corresponding logical point, rounded down to preserve negative margin coordinates.</returns>
    public Point ToLogical(Point clientPosition)
    {
        var drawablePosition = new Vector2(
            (float)clientPosition.X * BackBufferSize.X / ClientSize.X,
            (float)clientPosition.Y * BackBufferSize.Y / ClientSize.Y);
        var logicalPosition = (drawablePosition - Offset) / Scale;
        return new Point((int)MathF.Floor(logicalPosition.X), (int)MathF.Floor(logicalPosition.Y));
    }
}
