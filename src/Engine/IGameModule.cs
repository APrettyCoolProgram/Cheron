// 261028_code
// 261028_documentation

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cheron.Engine;

/// <summary>Defines a game hosted by the engine without depending on a particular game's rules.</summary>
/// <remarks>
/// The host calls lifecycle methods on the game thread and disposes the module before the graphics device.
/// The module borrows the graphics device and owns any resources it creates.
/// </remarks>
public interface IGameModule : IDisposable
{
    /// <summary>Gets the title displayed in the application window.</summary>
    string Title { get; }

    /// <summary>Gets the logical canvas size, also used to request the initial client size.</summary>
    Point PreferredWindowSize { get; }

    /// <summary>Initializes game state before graphics resources are loaded.</summary>
    void Initialize();

    /// <summary>Receives the initial viewport and subsequent window or drawable-size changes.</summary>
    /// <remarks>
    /// Called on the game thread before input or drawing uses the new dimensions.
    /// </remarks>
    /// <param name="viewport">The host-computed drawing transform and mouse-coordinate mapping.</param>
    void SetViewport(GameViewport viewport);

    /// <summary>Creates the module's graphics resources after the device becomes available.</summary>
    /// <param name="graphicsDevice">The host-owned graphics device, which the module must not dispose.</param>
    void LoadContent(GraphicsDevice graphicsDevice);

    /// <summary>Advances game logic and processes game-specific input.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    void Update(GameTime gameTime);

    /// <summary>Draws the game using its own rendering resources.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    /// <param name="graphicsDevice">The host-owned graphics device, which the module must not dispose.</param>
    void Draw(GameTime gameTime, GraphicsDevice graphicsDevice);
}
