// 261028_code
// 261028_documentation

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Engine;

/// <summary>Owns the MonoGame application lifecycle and delegates game-specific work to a module.</summary>
public sealed class GameHost : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly IGameModule _gameModule;

    private bool _isGameModuleDisposed;

    /// <summary>Creates a host for the supplied game module.</summary>
    /// <param name="gameModule">The module whose lifecycle and disposal are owned by this host.</param>
    public GameHost(IGameModule gameModule)
    {
        ArgumentNullException.ThrowIfNull(gameModule);

        _gameModule = gameModule;

        _graphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = gameModule.PreferredWindowSize.X,
            PreferredBackBufferHeight = gameModule.PreferredWindowSize.Y
        };

        Content.RootDirectory = "Content";
        IsMouseVisible        = true;
        Window.Title          = gameModule.Title;
    }

    /// <summary>Initializes module state before MonoGame loads graphics resources.</summary>
    protected override void Initialize()
    {
        _gameModule.Initialize();
        base.Initialize();
    }

    /// <summary>Loads module resources using the host-owned graphics device.</summary>
    protected override void LoadContent()
    {
        _gameModule.LoadContent(GraphicsDevice);
    }

    /// <summary>Handles global exit controls and advances the game module.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _gameModule.Update(gameTime);
        base.Update(gameTime);
    }

    /// <summary>Delegates rendering to the game module before drawing MonoGame components.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    protected override void Draw(GameTime gameTime)
    {
        _gameModule.Draw(gameTime, GraphicsDevice);
        base.Draw(gameTime);
    }

    /// <summary>Disposes the module once before releasing the host's resources.</summary>
    /// <param name="disposing">Whether managed resources should be released.</param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && !_isGameModuleDisposed)
            {
                _isGameModuleDisposed = true;
                _gameModule.Dispose();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
}
