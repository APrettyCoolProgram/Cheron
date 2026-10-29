// 261029_code
// 261029_documentation

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cheron.Engine;

/// <summary>Defines the contract for a game module hosted by the engine.</summary>
/// <remarks>
/// The host invokes lifecycle members on the game thread and disposes the module before disposing
/// the graphics device. Implementations borrow the graphics device and own any resources they create.
/// </remarks>
internal interface IGameModule : IDisposable
{
    /// <summary>Gets the title displayed in the application window.</summary>
    string Title { get; }

    /// <summary>Gets the logical canvas size used to request the initial client size.</summary>
    Point PreferredWindowSize { get; }

    /// <summary>Initializes game state before graphics resources are loaded.</summary>
    void Initialize();

    /// <summary>Applies the initial viewport and any later window or drawable-size changes.</summary>
    /// <remarks>
    /// Called on the game thread before input or drawing uses the new dimensions.
    /// </remarks>
    /// <param name="viewport">The host-computed drawing transform and mouse-coordinate mapping.</param>
    void SetViewport(GameViewport viewport);

    /// <summary>Creates graphics resources after the graphics device becomes available.</summary>
    /// <param name="graphicsDevice">The host-owned graphics device, which the module must not dispose.</param>
    void LoadContent(GraphicsDevice graphicsDevice);

    /// <summary>Advances game logic and processes module-specific input.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    void Update(GameTime gameTime);

    /// <summary>Draws the module using its rendering resources.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    /// <param name="graphicsDevice">The host-owned graphics device, which the module must not dispose.</param>
    void Draw(GameTime gameTime, GraphicsDevice graphicsDevice);
}
