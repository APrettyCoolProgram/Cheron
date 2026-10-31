// 261031_code
// 261031_documentation

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Engine;

/// <summary>Owns the MonoGame application lifecycle and delegates game-specific work to a module.</summary>
internal sealed class GameHost : Game
{

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly IGameModule _gameModule;

    private bool _isGameModuleDisposed;

    /// <summary>The last preferred window size applied to the graphics device manager.</summary>
    private Point _appliedPreferredWindowSize;

    /// <summary>The last sizing notification sent to the active module.</summary>
    private GameViewport _viewport;

    /// <summary>Creates a host for the supplied game module.</summary>
    /// <param name="gameModule">The module whose lifecycle and disposal are owned by this host.</param>
    public GameHost(IGameModule gameModule)
    {
        ArgumentNullException.ThrowIfNull(gameModule);

        _gameModule = gameModule;

        _appliedPreferredWindowSize = gameModule.PreferredWindowSize;

        _graphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = _appliedPreferredWindowSize.X,
            PreferredBackBufferHeight = _appliedPreferredWindowSize.Y
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = gameModule.Title;
        Window.AllowUserResizing = false;
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
        RefreshViewport();
        _gameModule.LoadContent(GraphicsDevice);
    }

    /// <summary>Handles global exit controls and advances the game module.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    protected override void Update(GameTime gameTime)
    {
        if (ShouldExitGame())
        {
            Exit();
        }

        if (RefreshViewport())
        {
            _gameModule.Update(gameTime);
        }

        base.Update(gameTime);
    }

    /// <summary>Delegates rendering to the game module before drawing MonoGame components.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    protected override void Draw(GameTime gameTime)
    {
        if (RefreshViewport())
        {
            _gameModule.Draw(gameTime, GraphicsDevice);
        }

        base.Draw(gameTime);
    }

    /// <summary>Notifies the module when client or drawable dimensions change, including DPI changes.</summary>
    /// <returns>Whether the current surface has positive dimensions and can be used.</returns>
    private bool RefreshViewport()
    {
        ApplyPreferredWindowSize();

        if (!TryGetCurrentSurfaceSizes(out var clientAreaSize, out var drawableSize))
        {
            return false;
        }

        NotifyModuleViewport(clientAreaSize, drawableSize);

        return true;
    }

    /// <summary>Applies a changed preferred window size from the active module.</summary>
    private void ApplyPreferredWindowSize()
    {
        var preferredWindowSize = _gameModule.PreferredWindowSize;
        if (preferredWindowSize == _appliedPreferredWindowSize)
        {
            return;
        }

        _appliedPreferredWindowSize = preferredWindowSize;
        _graphicsDeviceManager.PreferredBackBufferWidth = preferredWindowSize.X;
        _graphicsDeviceManager.PreferredBackBufferHeight = preferredWindowSize.Y;
        _graphicsDeviceManager.ApplyChanges();
    }

    /// <summary>Gets the current client and back-buffer sizes when both are valid for rendering.</summary>
    /// <param name="clientSize">When this method returns, contains the current client size.</param>
    /// <param name="backBufferSize">When this method returns, contains the current back-buffer size.</param>
    /// <returns><see langword="true"/> when both sizes are valid; otherwise, <see langword="false"/>.</returns>
    private bool TryGetCurrentSurfaceSizes(out Point clientAreaSize, out Point drawableSize)
    {
        clientAreaSize = Window.ClientBounds.Size;
        drawableSize = GetBackBufferSize();
        return HasValidDimensions(clientAreaSize, drawableSize);
    }

    /// <summary>Builds a viewport from current sizes and notifies the module when it changes.</summary>
    /// <param name="clientSize">The current client area size.</param>
    /// <param name="backBufferSize">The current back-buffer size.</param>
    private void NotifyModuleViewport(Point clientAreaSize, Point drawableSize)
    {
        var viewport = new GameViewport(_gameModule.PreferredWindowSize, clientAreaSize, drawableSize);
        if (viewport == _viewport)
        {
            return;
        }

        _viewport = viewport;
        _gameModule.SetViewport(viewport);
    }

    /// <summary>Gets whether host-level input requests the application to exit.</summary>
    /// <returns><see langword="true"/> when the back button or escape key is pressed; otherwise, <see langword="false"/>.</returns>
    private static bool ShouldExitGame()
    {
        return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape);
    }

    /// <summary>Gets the current back-buffer dimensions from the active graphics presentation parameters.</summary>
    /// <returns>The current back-buffer size.</returns>
    private Point GetBackBufferSize()
    {
        var presentationParameters = GraphicsDevice.PresentationParameters;
        return new Point(presentationParameters.BackBufferWidth, presentationParameters.BackBufferHeight);
    }

    /// <summary>Gets whether client and back-buffer dimensions are positive and usable for rendering.</summary>
    /// <param name="clientSize">The current client area size.</param>
    /// <param name="backBufferSize">The current back-buffer size.</param>
    /// <returns><see langword="true"/> when both sizes are usable; otherwise, <see langword="false"/>.</returns>
    private static bool HasValidDimensions(Point clientAreaSize, Point drawableSize)
    {
        return clientAreaSize.X > 0 && clientAreaSize.Y > 0 && drawableSize.X > 0 && drawableSize.Y > 0;
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
