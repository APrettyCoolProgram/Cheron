// 261030_code
// 261030_documentation

using System;
using System.Collections.Generic;
using Cheron.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Cheron.Games.Memory.MemoryAudioConstants;
using static Cheron.Games.Memory.MemoryTextConstants;

namespace Cheron.Games.Memory;

/// <summary>Implements the Memory tile-matching game module, including toolbar, help, and sound options.</summary>
internal sealed class MemoryGame : IGameModule
{
    /// <summary>The number of seconds a mismatched pair remains face up before flipping back down.</summary>
    private const double MismatchRevealSeconds = 0.75;

    /// <summary>The selectable board dimensions, ordered as shown in the size picker prompt.</summary>
    private static readonly int[] SelectableGridSizes = [4, 6, 8, 10, 12];

    /// <summary>The board tiles in row-major order.</summary>
    private readonly List<MemoryTile> _tiles = new();

    /// <summary>The number of tile columns on the current board.</summary>
    private int _columns = MemoryLayout.DefaultGridSize;

    /// <summary>The number of tile rows on the current board.</summary>
    private int _rows = MemoryLayout.DefaultGridSize;

    /// <summary>The host-owned graphics device, cached to rebuild artwork when the board size changes.</summary>
    private GraphicsDevice _graphicsDevice = null!;

    /// <summary>The module-owned sprite batch, created during content loading and released during disposal.</summary>
    private SpriteBatch _spriteBatch = null!;

    /// <summary>The module-owned white pixel used to draw solid rectangles.</summary>
    private Texture2D _pixelTexture = null!;

    /// <summary>The module-owned padded bitmap glyph atlas shared with Solitaire's font rendering.</summary>
    private Texture2D _glyphTexture = null!;

    /// <summary>The module-owned cached textures used for the table, tiles, and toolbar chrome.</summary>
    private MemoryArtwork _artwork = null!;

    /// <summary>The host-computed drawing transform and mouse-coordinate mapping.</summary>
    private GameViewport _viewport;

    /// <summary>The last processed mouse state, used to detect new clicks.</summary>
    private MouseState _previousMouseState;

    /// <summary>The last processed keyboard state, used to detect new key presses.</summary>
    private KeyboardState _previousKeyboardState;

    /// <summary>The current pointer position in logical board coordinates, used for button hover.</summary>
    private Point _pointerPosition;

    /// <summary>The grid index of the keyboard-focused tile.</summary>
    private int _focusedIndex;

    /// <summary>The index of the first face-up, unmatched tile awaiting a second pick, or -1 when none.</summary>
    private int _firstPickIndex = -1;

    /// <summary>The index of the second face-up, unmatched tile awaiting resolution, or -1 when none.</summary>
    private int _secondPickIndex = -1;

    /// <summary>The elapsed seconds since a mismatched pair was fully revealed, or zero when idle.</summary>
    private double _mismatchElapsedSeconds;

    /// <summary>The shared random generator used to shuffle tile colors.</summary>
    private readonly Random _random = new();

    /// <summary>Whether the in-game Help overlay is currently visible.</summary>
    private bool _helpOpen;

    /// <summary>Whether the board-size picker prompt is currently visible.</summary>
    private bool _sizePromptOpen;

    /// <summary>The index into <see cref="SelectableGridSizes"/> currently highlighted in the size picker prompt.</summary>
    private int _sizePromptIndex = Array.IndexOf(SelectableGridSizes, MemoryLayout.DefaultGridSize);

    /// <summary>Whether every pair on the board has been matched.</summary>
    private bool _hasWon;

    /// <summary>Whether the one-shot win feedback sound has already played for the current board.</summary>
    private bool _winFeedbackPlayed;

    /// <summary>The number of completed two-tile picks attempted on the current board.</summary>
    private int _moveCount;

    /// <summary>The elapsed seconds since the current board was dealt, frozen once the board is won.</summary>
    private double _elapsedSeconds;

    /// <summary>Whether sounds are enabled for this module session, preserved across deals.</summary>
    private bool _soundEnabled = true;

    /// <summary>Whether the audio device could not be initialized or used.</summary>
    private bool _audioUnavailable;

    /// <summary>The module-owned sounds, ordered by <see cref="MemoryFeedbackSound"/>.</summary>
    private readonly List<SoundEffect> _feedbackSounds = new();

    /// <summary>The module-owned playback instances, stopped immediately when muted.</summary>
    private readonly List<SoundEffectInstance> _feedbackVoices = new();

    /// <summary>Gets the Memory window title.</summary>
    public string Title => TitleLabel;

    /// <summary>Gets the logical board size for the current tile dimensions.</summary>
    public Point PreferredWindowSize => new(MemoryLayout.GetBoardWidth(_columns), MemoryLayout.GetBoardHeight(_rows));

    /// <summary>Deals a shuffled board before graphics resources are loaded.</summary>
    public void Initialize()
    {
        StartNewGame();
    }

    /// <summary>Applies the initial viewport and any later window or drawable-size changes.</summary>
    /// <param name="viewport">The host-computed drawing transform and mouse-coordinate mapping.</param>
    public void SetViewport(GameViewport viewport)
    {
        _viewport = viewport;
    }

    /// <summary>Creates graphics resources after the graphics device becomes available.</summary>
    /// <param name="graphicsDevice">The host-owned graphics device, which the module must not dispose.</param>
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
        _glyphTexture = Solitaire.SolitaireBitmapFont.CreateGlyphTexture(graphicsDevice);
        RebuildArtwork();
        LoadFeedbackSounds();
    }

    /// <summary>Recreates the cached artwork set to match the current board size.</summary>
    private void RebuildArtwork()
    {
        _artwork?.Dispose();
        var tileSize = MemoryLayout.GetTileSize(_columns, _rows);
        var cornerRadius = Math.Max(6, Math.Min(MemoryLayout.CornerRadius, tileSize / 4));

        _artwork = new MemoryArtwork(_graphicsDevice, MemoryLayout.GetBoardWidth(_columns), MemoryLayout.GetBoardHeight(_rows),
            tileSize, cornerRadius, MemoryColors.GetPairColors(_columns * _rows / 2),
            MemoryLayout.ButtonWidth, MemoryLayout.ButtonHeight, MemoryLayout.ButtonCornerRadius,
            560, 420);
    }

    /// <summary>Advances game logic and processes board, toolbar, and overlay input.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    public void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        _pointerPosition = _viewport.ToLogical(mouseState.Position);
        var keyboardState = Keyboard.GetState();

        if (!_hasWon && _firstPickIndex < 0 && _secondPickIndex < 0)
        {
            _elapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (HandleGlobalShortcuts(keyboardState))
        {
            _previousMouseState = mouseState;
            _previousKeyboardState = keyboardState;
            return;
        }

        if (_sizePromptOpen)
        {
            HandleSizePromptKeyboardInput(keyboardState);
            HandleSizePromptMouseInput(mouseState);
            _previousMouseState = mouseState;
            _previousKeyboardState = keyboardState;
            return;
        }

        if (_helpOpen)
        {
            HandleOverlayMouseInput(mouseState);
            _previousMouseState = mouseState;
            _previousKeyboardState = keyboardState;
            return;
        }

        if (_firstPickIndex >= 0 && _secondPickIndex >= 0)
        {
            _mismatchElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_mismatchElapsedSeconds >= MismatchRevealSeconds)
            {
                ResolvePendingPair();
            }

            _previousMouseState = mouseState;
            _previousKeyboardState = keyboardState;
            return;
        }

        HandleToolbarMouseInput(mouseState);
        HandleBoardMouseInput(mouseState);
        HandleKeyboardInput(keyboardState);

        _previousMouseState = mouseState;
        _previousKeyboardState = keyboardState;
    }

    /// <summary>Draws the board using the module's rendering resources.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    /// <param name="graphicsDevice">The host-owned graphics device, which the module must not dispose.</param>
    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(MemoryColors.TableBackground);

        _spriteBatch.Begin(transformMatrix: _viewport.Transform, samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_artwork.Background, new Rectangle(0, 0, MemoryLayout.GetBoardWidth(_columns), MemoryLayout.GetBoardHeight(_rows)), Color.White);

        for (var index = 0; index < _tiles.Count; index++)
        {
            DrawTile(index);
        }

        DrawToolbar();
        DrawFooter();

        if (_hasWon)
        {
            DrawWinOverlay();
        }
        else if (_sizePromptOpen)
        {
            DrawSizePromptOverlay();
        }
        else if (_helpOpen)
        {
            DrawHelpOverlay();
        }

        _spriteBatch.End();
    }

    /// <summary>Releases rendering and audio resources owned by this module.</summary>
    public void Dispose()
    {
        DisposeFeedbackSounds();
        _spriteBatch?.Dispose();
        _artwork?.Dispose();
        _glyphTexture?.Dispose();
        _pixelTexture?.Dispose();
    }

    /// <summary>Builds and shuffles a new board of matching color pairs for the current size, resetting stats and overlays.</summary>
    private void StartNewGame()
    {
        _tiles.Clear();

        var colorIds = new List<int>();
        var pairCount = (_columns * _rows) / 2;
        for (var colorId = 0; colorId < pairCount; colorId++)
        {
            colorIds.Add(colorId);
            colorIds.Add(colorId);
        }

        for (var index = colorIds.Count - 1; index > 0; index--)
        {
            var swapIndex = _random.Next(index + 1);
            (colorIds[index], colorIds[swapIndex]) = (colorIds[swapIndex], colorIds[index]);
        }

        foreach (var colorId in colorIds)
        {
            _tiles.Add(new MemoryTile(colorId));
        }

        _focusedIndex = 0;
        _firstPickIndex = -1;
        _secondPickIndex = -1;
        _mismatchElapsedSeconds = 0;
        _helpOpen = false;
        _sizePromptOpen = false;
        _hasWon = false;
        _winFeedbackPlayed = false;
        _moveCount = 0;
        _elapsedSeconds = 0;
    }

    /// <summary>Opens the board-size picker prompt with the current size preselected.</summary>
    private void OpenSizePrompt()
    {
        _sizePromptIndex = Math.Max(0, Array.IndexOf(SelectableGridSizes, _columns));
        _sizePromptOpen = true;
        _helpOpen = false;
    }

    /// <summary>Applies the currently highlighted size, rebuilds artwork if needed, and deals a new board.</summary>
    private void ConfirmSizePromptSelection()
    {
        var selectedSize = SelectableGridSizes[_sizePromptIndex];
        var sizeChanged = selectedSize != _columns || selectedSize != _rows;
        _columns = selectedSize;
        _rows = selectedSize;
        _sizePromptOpen = false;

        if (sizeChanged && _graphicsDevice is not null)
        {
            RebuildArtwork();
        }

        StartNewGame();
    }

    /// <summary>Closes the size picker prompt without starting a new game.</summary>
    private void CancelSizePrompt()
    {
        _sizePromptOpen = false;
    }

    /// <summary>Moves the size picker highlight by the given number of options, clamped to the available range.</summary>
    /// <param name="delta">The number of options to move; positive moves forward in the list.</param>
    private void MoveSizePromptSelection(int delta)
    {
        _sizePromptIndex = Math.Clamp(_sizePromptIndex + delta, 0, SelectableGridSizes.Length - 1);
    }

    /// <summary>Processes the New Game, Help, and Sound shortcuts, which remain active in every mode.</summary>
    /// <param name="keyboardState">The current keyboard state.</param>
    /// <returns>Whether a global shortcut was handled this frame.</returns>
    private bool HandleGlobalShortcuts(KeyboardState keyboardState)
    {
        if (IsNewPress(keyboardState, Keys.N) && !_sizePromptOpen)
        {
            OpenSizePrompt();
            return true;
        }

        if (IsNewPress(keyboardState, Keys.M))
        {
            ToggleSound();
            return true;
        }

        if (IsNewPress(keyboardState, Keys.F1) && !_sizePromptOpen)
        {
            _helpOpen = !_helpOpen;
            return true;
        }

        if (IsNewPress(keyboardState, Keys.Escape) && (_helpOpen || _hasWon))
        {
            _helpOpen = false;
            return true;
        }

        return false;
    }

    /// <summary>Processes mouse clicks against the toolbar buttons while the board is interactive.</summary>
    /// <param name="mouseState">The current mouse state.</param>
    private void HandleToolbarMouseInput(MouseState mouseState)
    {
        if (!IsNewClick(mouseState))
        {
            return;
        }

        if (MemoryLayout.GetNewGameButtonBounds(_columns).Contains(_pointerPosition))
        {
            OpenSizePrompt();
        }
        else if (MemoryLayout.GetHelpButtonBounds(_columns).Contains(_pointerPosition))
        {
            _helpOpen = true;
        }
        else if (MemoryLayout.GetSoundButtonBounds(_columns).Contains(_pointerPosition) && !_audioUnavailable)
        {
            ToggleSound();
        }
    }

    /// <summary>Processes mouse clicks against the board-size picker prompt.</summary>
    /// <param name="mouseState">The current mouse state.</param>
    private void HandleSizePromptMouseInput(MouseState mouseState)
    {
        if (!IsNewClick(mouseState))
        {
            return;
        }

        for (var optionIndex = 0; optionIndex < SelectableGridSizes.Length; optionIndex++)
        {
            if (MemoryLayout.GetSizePromptOptionBounds(_columns, _rows, optionIndex).Contains(_pointerPosition))
            {
                _sizePromptIndex = optionIndex;
                return;
            }
        }

        if (MemoryLayout.GetSizePromptConfirmBounds(_columns, _rows).Contains(_pointerPosition))
        {
            ConfirmSizePromptSelection();
        }
        else if (MemoryLayout.GetSizePromptCancelBounds(_columns, _rows).Contains(_pointerPosition))
        {
            CancelSizePrompt();
        }
    }

    /// <summary>Processes keyboard navigation and confirmation for the board-size picker prompt.</summary>
    /// <param name="keyboardState">The current keyboard state.</param>
    private void HandleSizePromptKeyboardInput(KeyboardState keyboardState)
    {
        if (IsNewPress(keyboardState, Keys.Left) || IsNewPress(keyboardState, Keys.Up))
        {
            MoveSizePromptSelection(-1);
        }
        else if (IsNewPress(keyboardState, Keys.Right) || IsNewPress(keyboardState, Keys.Down))
        {
            MoveSizePromptSelection(1);
        }
        else if (IsNewPress(keyboardState, Keys.Enter) || IsNewPress(keyboardState, Keys.Space))
        {
            ConfirmSizePromptSelection();
        }
        else if (IsNewPress(keyboardState, Keys.Escape))
        {
            CancelSizePrompt();
        }
    }

    /// <summary>Processes mouse clicks against the tile grid.</summary>
    /// <param name="mouseState">The current mouse state.</param>
    private void HandleBoardMouseInput(MouseState mouseState)
    {
        if (!IsNewClick(mouseState))
        {
            return;
        }

        var tileIndex = GetTileIndexAt(_pointerPosition);
        if (tileIndex >= 0)
        {
            _focusedIndex = tileIndex;
            TryFlipTile(tileIndex);
        }
    }

    /// <summary>Processes mouse clicks used only to dismiss the Help overlay.</summary>
    /// <param name="mouseState">The current mouse state.</param>
    private void HandleOverlayMouseInput(MouseState mouseState)
    {
        if (IsNewClick(mouseState))
        {
            _helpOpen = false;
        }
    }

    /// <summary>Determines whether the left mouse button was newly pressed this frame.</summary>
    /// <param name="mouseState">The current mouse state.</param>
    private bool IsNewClick(MouseState mouseState) =>
        mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;

    /// <summary>Processes keyboard navigation and tile selection.</summary>
    /// <param name="keyboardState">The current keyboard state.</param>
    private void HandleKeyboardInput(KeyboardState keyboardState)
    {
        if (IsNewPress(keyboardState, Keys.Left))
        {
            MoveFocus(-1, 0);
        }
        else if (IsNewPress(keyboardState, Keys.Right))
        {
            MoveFocus(1, 0);
        }
        else if (IsNewPress(keyboardState, Keys.Up))
        {
            MoveFocus(0, -1);
        }
        else if (IsNewPress(keyboardState, Keys.Down))
        {
            MoveFocus(0, 1);
        }
        else if (IsNewPress(keyboardState, Keys.Enter) || IsNewPress(keyboardState, Keys.Space))
        {
            TryFlipTile(_focusedIndex);
        }
    }

    /// <summary>Determines whether a key was pressed this frame but not the previous frame.</summary>
    /// <param name="keyboardState">The current keyboard state.</param>
    /// <param name="key">The key to test.</param>
    private bool IsNewPress(KeyboardState keyboardState, Keys key) =>
        keyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);

    /// <summary>Moves the keyboard focus by the given column/row delta, clamped to the board bounds.</summary>
    /// <param name="deltaColumn">The number of columns to move, positive moves right.</param>
    /// <param name="deltaRow">The number of rows to move, positive moves down.</param>
    private void MoveFocus(int deltaColumn, int deltaRow)
    {
        var column = Math.Clamp(_focusedIndex % _columns + deltaColumn, 0, _columns - 1);
        var row = Math.Clamp(_focusedIndex / _columns + deltaRow, 0, _rows - 1);
        _focusedIndex = row * _columns + column;
    }

    /// <summary>Attempts to flip the tile at the given index face up, resolving a pair if two are now revealed.</summary>
    /// <param name="tileIndex">The index of the tile to flip.</param>
    private void TryFlipTile(int tileIndex)
    {
        if (tileIndex < 0 || tileIndex >= _tiles.Count)
        {
            return;
        }

        var tile = _tiles[tileIndex];
        if (tile.IsMatched || tile.IsFaceUp)
        {
            return;
        }

        tile.IsFaceUp = true;
        PlayFeedbackSound(MemoryFeedbackSound.Flip);

        if (_firstPickIndex < 0)
        {
            _firstPickIndex = tileIndex;
            return;
        }

        if (_secondPickIndex < 0)
        {
            _secondPickIndex = tileIndex;
            _mismatchElapsedSeconds = 0;
            _moveCount++;

            if (_tiles[_firstPickIndex].ColorId == _tiles[_secondPickIndex].ColorId)
            {
                _tiles[_firstPickIndex].IsMatched = true;
                _tiles[_secondPickIndex].IsMatched = true;
                _firstPickIndex = -1;
                _secondPickIndex = -1;
                PlayFeedbackSound(MemoryFeedbackSound.Match);
                CheckForWin();
            }
            else
            {
                PlayFeedbackSound(MemoryFeedbackSound.Mismatch);
            }
        }
    }

    /// <summary>Flips back down any unmatched pending pair once the reveal delay elapses.</summary>
    private void ResolvePendingPair()
    {
        if (_firstPickIndex >= 0)
        {
            _tiles[_firstPickIndex].IsFaceUp = false;
        }

        if (_secondPickIndex >= 0)
        {
            _tiles[_secondPickIndex].IsFaceUp = false;
        }

        _firstPickIndex = -1;
        _secondPickIndex = -1;
        _mismatchElapsedSeconds = 0;
    }

    /// <summary>Marks the board as won and plays the win cue when every tile has been matched.</summary>
    private void CheckForWin()
    {
        foreach (var tile in _tiles)
        {
            if (!tile.IsMatched)
            {
                return;
            }
        }

        _hasWon = true;
        if (!_winFeedbackPlayed)
        {
            _winFeedbackPlayed = true;
            PlayFeedbackSound(MemoryFeedbackSound.Win);
        }
    }

    /// <summary>Finds the grid index of the tile containing the given logical point, or -1 when none.</summary>
    /// <param name="logicalPosition">The logical-space point to test.</param>
    private int GetTileIndexAt(Point logicalPosition)
    {
        for (var index = 0; index < _tiles.Count; index++)
        {
            var bounds = MemoryLayout.GetTileBounds(index % _columns, index / _columns, _columns, _rows);
            if (bounds.Contains(logicalPosition))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>Draws a single tile's rounded back or front face, its drop shadow, and any focus outline.</summary>
    /// <param name="index">The grid index of the tile to draw.</param>
    private void DrawTile(int index)
    {
        var tile = _tiles[index];
        var bounds = MemoryLayout.GetTileBounds(index % _columns, index / _columns, _columns, _rows);

        var shadowBounds = new Rectangle(bounds.X - 8 + MemoryLayout.ShadowOffset, bounds.Y - 8 + MemoryLayout.ShadowOffset,
            bounds.Width + 16, bounds.Height + 16);
        _spriteBatch.Draw(_artwork.TileShadow, shadowBounds, Color.White);

        if (index == _focusedIndex && !_helpOpen && !_sizePromptOpen && !_hasWon)
        {
            var focusBounds = bounds;
            focusBounds.Inflate(6, 6);
            _spriteBatch.Draw(_artwork.FocusOutline, focusBounds, Color.White);
        }

        var faceTexture = tile.IsFaceUp || tile.IsMatched
            ? _artwork.TileFronts[tile.ColorId]
            : _artwork.TileBack;

        _spriteBatch.Draw(faceTexture, bounds, Color.White);
    }

    /// <summary>Draws a filled rectangle using the module's shared pixel texture.</summary>
    /// <param name="bounds">The rectangle to fill in logical board coordinates.</param>
    /// <param name="color">The fill color.</param>
    private void Fill(Rectangle bounds, Color color) => _spriteBatch.Draw(_pixelTexture, bounds, color);

    /// <summary>Draws uppercase bitmap text using Solitaire's shared glyph atlas.</summary>
    /// <param name="text">The text to draw.</param>
    /// <param name="textX">The starting horizontal position in logical board coordinates.</param>
    /// <param name="textY">The starting vertical position in logical board coordinates.</param>
    /// <param name="scale">The size of each glyph pixel in logical board units.</param>
    /// <param name="color">The text color.</param>
    private void DrawText(string text, int textX, int textY, int scale, Color color) =>
        Solitaire.SolitaireBitmapFont.DrawText(_spriteBatch, _glyphTexture, false, text, textX, textY, scale, color);

    /// <summary>Draws the top toolbar strip, title, statistics, and buttons.</summary>
    private void DrawToolbar()
    {
        var toolbarBounds = MemoryLayout.GetToolbarBounds(_columns);
        Fill(toolbarBounds, new Color(30, 20, 32));
        Fill(new Rectangle(toolbarBounds.X, toolbarBounds.Bottom - 2, toolbarBounds.Width, 2), new Color(176, 96, 150));

        const int titleScale = 3;
        DrawText(TitleLabel, 24, (toolbarBounds.Height - 7 * titleScale) / 2, titleScale, new Color(240, 220, 236));

        var movesText = $"{MovesCaptionLabel} {_moveCount}";
        var timeText = $"{TimeCaptionLabel} {FormatElapsedTime(_elapsedSeconds)}";
        var newGameBounds = MemoryLayout.GetNewGameButtonBounds(_columns);
        const int statsScale = 1;
        var statsWidth = Math.Max(movesText.Length, timeText.Length) * 6 * statsScale;
        var titleRight = 24 + TitleLabel.Length * 6 * titleScale;
        var statsLeftBound = titleRight + 16;
        var statsRightBound = newGameBounds.X - 16;
        var statsX = Math.Max(statsLeftBound, statsRightBound - statsWidth);
        DrawText(movesText, statsX, toolbarBounds.Height / 2 - 7 * statsScale - 2, statsScale, new Color(200, 180, 210));
        DrawText(timeText, statsX, toolbarBounds.Height / 2 + 3, statsScale, new Color(200, 180, 210));

        DrawToolbarButton(newGameBounds, NewGameLabel, "N");
        DrawToolbarButton(MemoryLayout.GetHelpButtonBounds(_columns), HelpLabel, "F1");
        DrawSoundButton(MemoryLayout.GetSoundButtonBounds(_columns));
    }

    /// <summary>Draws one themed toolbar button with its label and shortcut, highlighting it on hover.</summary>
    /// <param name="bounds">The button's drawing and hit-test bounds.</param>
    /// <param name="label">The primary button label.</param>
    /// <param name="shortcut">The keyboard shortcut shown beneath the label.</param>
    private void DrawToolbarButton(Rectangle bounds, string label, string shortcut)
    {
        var isHovered = bounds.Contains(_pointerPosition);
        _spriteBatch.Draw(isHovered ? _artwork.ButtonHover : _artwork.Button, bounds, Color.White);
        DrawButtonText(bounds, label, shortcut);
    }

    /// <summary>Draws the Sound toggle button, reflecting the current enabled state or audio availability.</summary>
    /// <param name="bounds">The button's drawing and hit-test bounds.</param>
    private void DrawSoundButton(Rectangle bounds)
    {
        var isHovered = !_audioUnavailable && bounds.Contains(_pointerPosition);
        _spriteBatch.Draw(isHovered ? _artwork.ButtonHover : _artwork.Button, bounds, Color.White);

        var label = _audioUnavailable ? AudioUnavailableLabel : $"{SoundLabel} {(_soundEnabled ? "ON" : "OFF")}";
        DrawButtonText(bounds, label, "M");
    }

    /// <summary>Draws a centered two-line button label and shortcut.</summary>
    /// <param name="bounds">The button bounds.</param>
    /// <param name="label">The primary label text.</param>
    /// <param name="shortcut">The shortcut text.</param>
    private void DrawButtonText(Rectangle bounds, string label, string shortcut)
    {
        const int labelScale = 1;
        const int shortcutScale = 1;
        const int lineGap = 4;
        var blockHeight = 7 * labelScale + lineGap + 7 * shortcutScale;
        var firstLineY = bounds.Y + (bounds.Height - blockHeight) / 2;
        var secondLineY = firstLineY + 7 * labelScale + lineGap;
        var color = new Color(240, 230, 240);

        var labelX = bounds.X + (bounds.Width - label.Length * 6 * labelScale) / 2;
        var shortcutX = bounds.X + (bounds.Width - shortcut.Length * 6 * shortcutScale) / 2;
        DrawText(label, labelX, firstLineY, labelScale, color);
        DrawText(shortcut, shortcutX, secondLineY, shortcutScale, new Color(190, 160, 190));
    }

    /// <summary>Draws the bottom footer strip and its shortcut legend.</summary>
    private void DrawFooter()
    {
        var footerBounds = MemoryLayout.GetFooterBounds(_columns, _rows);
        Fill(footerBounds, new Color(30, 20, 32));
        Fill(new Rectangle(footerBounds.X, footerBounds.Y, footerBounds.Width, 2), new Color(176, 96, 150));

        const int scale = 1;
        var textY = footerBounds.Y + (footerBounds.Height - 7 * scale) / 2;
        DrawText(FooterShortcutsLabel, MemoryLayout.BoardMargin, textY, scale, new Color(200, 180, 210));
    }

    /// <summary>Draws the semi-transparent scrim and centered panel shared by the help and win overlays.</summary>
    /// <param name="headline">The headline drawn at the top of the panel.</param>
    /// <param name="bodyLines">The body text lines drawn beneath the headline.</param>
    /// <param name="footerLine">The instruction line drawn at the bottom of the panel.</param>
    private void DrawOverlay(string headline, IReadOnlyList<string> bodyLines, string footerLine)
    {
        Fill(new Rectangle(0, 0, MemoryLayout.GetBoardWidth(_columns), MemoryLayout.GetBoardHeight(_rows)), new Color(0, 0, 0, 150));

        var panelBounds = MemoryLayout.GetSizePromptPanelBounds(_columns, _rows);
        _spriteBatch.Draw(_artwork.Banner, panelBounds, Color.White);

        const int headlineScale = 3;
        var headlineX = panelBounds.X + (panelBounds.Width - headline.Length * 6 * headlineScale) / 2;
        var cursorY = panelBounds.Y + 28;
        DrawText(headline, headlineX, cursorY, headlineScale, new Color(255, 221, 120));
        cursorY += 7 * headlineScale + 24;

        const int bodyScale = 2;
        foreach (var line in bodyLines)
        {
            if (line.Length > 0)
            {
                var lineX = panelBounds.X + (panelBounds.Width - line.Length * 6 * bodyScale) / 2;
                DrawText(line, lineX, cursorY, bodyScale, new Color(232, 224, 236));
            }

            cursorY += 7 * bodyScale + 10;
        }

        var footerX = panelBounds.X + (panelBounds.Width - footerLine.Length * 6) / 2;
        DrawText(footerLine, footerX, panelBounds.Bottom - 32, 1, new Color(190, 160, 190));
    }

    /// <summary>Draws the Help overlay listing controls and shortcuts.</summary>
    private void DrawHelpOverlay()
    {
        DrawOverlay(HelpTitleLabel, HelpLines, HelpCloseLabel);
    }

    /// <summary>Draws the win celebration overlay shown once every pair has been matched.</summary>
    private void DrawWinOverlay()
    {
        var summaryLines = new[]
        {
            "",
            $"{MovesCaptionLabel}: {_moveCount}      {TimeCaptionLabel}: {FormatElapsedTime(_elapsedSeconds)}"
        };

        DrawOverlay(WinMessageLabel, summaryLines, WinSubMessageLabel);
    }

    /// <summary>Draws the board-size picker prompt, highlighting the currently selected size.</summary>
    private void DrawSizePromptOverlay()
    {
        Fill(new Rectangle(0, 0, MemoryLayout.GetBoardWidth(_columns), MemoryLayout.GetBoardHeight(_rows)), new Color(0, 0, 0, 150));

        var panelBounds = MemoryLayout.GetOverlayPanelBounds(_columns, _rows);
        _spriteBatch.Draw(_artwork.Banner, panelBounds, Color.White);

        const int headlineScale = 3;
        var headlineX = panelBounds.X + (panelBounds.Width - SizePromptTitleLabel.Length * 6 * headlineScale) / 2;
        var cursorY = panelBounds.Y + 28;
        DrawText(SizePromptTitleLabel, headlineX, cursorY, headlineScale, new Color(255, 221, 120));

        for (var optionIndex = 0; optionIndex < SelectableGridSizes.Length; optionIndex++)
        {
            var optionBounds = MemoryLayout.GetSizePromptOptionBounds(_columns, _rows, optionIndex);
            var isSelected = optionIndex == _sizePromptIndex;
            var isHovered = optionBounds.Contains(_pointerPosition);
            Fill(optionBounds, isSelected ? new Color(176, 96, 150) : isHovered ? new Color(70, 50, 72) : new Color(50, 36, 54));

            var size = SelectableGridSizes[optionIndex];
            var label = $"{size}x{size}";
            var labelX = optionBounds.X + (optionBounds.Width - label.Length * 6 * 2) / 2;
            var labelY = optionBounds.Y + (optionBounds.Height - 7 * 2) / 2;
            DrawText(label, labelX, labelY, 2, new Color(240, 230, 240));
        }

        var confirmBounds = MemoryLayout.GetSizePromptConfirmBounds(_columns, _rows);
        Fill(confirmBounds, confirmBounds.Contains(_pointerPosition) ? new Color(90, 150, 96) : new Color(60, 110, 66));
        var confirmLabelX = confirmBounds.X + (confirmBounds.Width - SizePromptConfirmLabel.Length * 6) / 2;
        var confirmLabelY = confirmBounds.Y + (confirmBounds.Height - 7) / 2;
        DrawText(SizePromptConfirmLabel, confirmLabelX, confirmLabelY, 1, new Color(240, 230, 240));

        var cancelBounds = MemoryLayout.GetSizePromptCancelBounds(_columns, _rows);
        Fill(cancelBounds, cancelBounds.Contains(_pointerPosition) ? new Color(150, 90, 96) : new Color(110, 60, 66));
        var cancelLabelX = cancelBounds.X + (cancelBounds.Width - SizePromptCancelLabel.Length * 6) / 2;
        var cancelLabelY = cancelBounds.Y + (cancelBounds.Height - 7) / 2;
        DrawText(SizePromptCancelLabel, cancelLabelX, cancelLabelY, 1, new Color(240, 230, 240));

        var hintX = panelBounds.X + (panelBounds.Width - SizePromptHintLabel.Length * 6) / 2;
        DrawText(SizePromptHintLabel, hintX, panelBounds.Bottom - 32, 1, new Color(190, 160, 190));
    }

    /// <summary>Formats an elapsed-seconds value as a minutes:seconds string.</summary>
    /// <param name="totalSeconds">The elapsed time in seconds.</param>
    private static string FormatElapsedTime(double totalSeconds)
    {
        var totalWholeSeconds = (int)totalSeconds;
        var minutes = totalWholeSeconds / 60;
        var seconds = totalWholeSeconds % 60;
        return $"{minutes}:{seconds:00}";
    }

    /// <summary>Creates original, short PCM sounds and playback instances without external assets.</summary>
    private void LoadFeedbackSounds()
    {
        try
        {
            AddFeedbackSound(FlipFrequencies, FlipNoteSeconds);
            AddFeedbackSound(MatchFrequencies, MatchNoteSeconds);
            AddFeedbackSound(MismatchFrequencies, MismatchNoteSeconds);
            AddFeedbackSound(WinFrequencies, WinNoteSeconds);
        }
        catch (NoAudioHardwareException)
        {
            _audioUnavailable = true;
        }
    }

    /// <summary>Creates one cached mono sound with a smooth attack and release envelope for each note.</summary>
    /// <param name="frequencies">The note frequencies in hertz.</param>
    /// <param name="noteSeconds">The duration of each note in seconds.</param>
    private void AddFeedbackSound(double[] frequencies, double noteSeconds)
    {
        var noteSamples = (int)(FeedbackSoundSampleRate * noteSeconds);
        var samples = new byte[noteSamples * frequencies.Length * 2];
        for (var noteIndex = 0; noteIndex < frequencies.Length; noteIndex++)
        {
            for (var sampleIndex = 0; sampleIndex < noteSamples; sampleIndex++)
            {
                var progress = (double)sampleIndex / (noteSamples - 1);
                var envelope = Math.Sin(Math.PI * progress);
                var wave = Math.Sin(2 * Math.PI * frequencies[noteIndex] * sampleIndex / FeedbackSoundSampleRate);
                var sample = (short)(wave * envelope * envelope * short.MaxValue * 0.5);
                var byteIndex = (noteIndex * noteSamples + sampleIndex) * 2;
                samples[byteIndex] = (byte)(sample & 255);
                samples[byteIndex + 1] = (byte)((sample >> 8) & 255);
            }
        }

        var sound = new SoundEffect(samples, FeedbackSoundSampleRate, AudioChannels.Mono);
        _feedbackSounds.Add(sound);
        var voice = sound.CreateInstance();
        _feedbackVoices.Add(voice);
        voice.Volume = FeedbackSoundVolume;
    }

    /// <summary>Plays one cached cue, restarting it if it is already playing.</summary>
    /// <param name="sound">The feedback sound to play.</param>
    private void PlayFeedbackSound(MemoryFeedbackSound sound)
    {
        if (!_soundEnabled || _audioUnavailable || _feedbackVoices.Count == 0)
        {
            return;
        }

        var voice = _feedbackVoices[(int)sound];
        voice.Stop();
        voice.Play();
    }

    /// <summary>Toggles sound on or off, stopping any currently playing cue when muted.</summary>
    private void ToggleSound()
    {
        _soundEnabled = !_soundEnabled;
        if (!_soundEnabled)
        {
            foreach (var voice in _feedbackVoices)
            {
                voice.Stop();
            }
        }
    }

    /// <summary>Releases every cached feedback sound and playback instance.</summary>
    private void DisposeFeedbackSounds()
    {
        foreach (var voice in _feedbackVoices)
        {
            voice.Dispose();
        }

        _feedbackVoices.Clear();

        foreach (var sound in _feedbackSounds)
        {
            sound.Dispose();
        }

        _feedbackSounds.Clear();
    }

    /// <summary>Starts a new game for tests.</summary>
    internal void TestStartNewGame() => StartNewGame();

    /// <summary>Gets the number of tiles on the board for tests.</summary>
    internal int TestTileCount => _tiles.Count;

    /// <summary>Gets the tile at the given index for tests.</summary>
    internal MemoryTile TestGetTile(int index) => _tiles[index];

    /// <summary>Gets the keyboard-focused tile index for tests.</summary>
    internal int TestGetFocusedIndex() => _focusedIndex;

    /// <summary>Sets the keyboard-focused tile index for tests.</summary>
    internal void TestSetFocusedIndex(int index)
    {
        _focusedIndex = index;
    }

    /// <summary>Attempts to flip the tile at the given index for tests.</summary>
    internal void TestTryFlipTile(int tileIndex) => TryFlipTile(tileIndex);

    /// <summary>Flips back down any unresolved mismatched pair for tests.</summary>
    internal void TestResolvePendingPair() => ResolvePendingPair();

    /// <summary>Gets the first pending pick index for tests.</summary>
    internal int TestGetFirstPickIndex() => _firstPickIndex;

    /// <summary>Gets the second pending pick index for tests.</summary>
    internal int TestGetSecondPickIndex() => _secondPickIndex;

    /// <summary>Sets the elapsed seconds since the pending mismatch was revealed for tests.</summary>
    internal void TestSetMismatchElapsedSeconds(double seconds)
    {
        _mismatchElapsedSeconds = seconds;
    }

    /// <summary>Gets whether every pair has been matched for tests.</summary>
    internal bool TestHasWon() => _hasWon;

    /// <summary>Gets the completed move count for tests.</summary>
    internal int TestGetMoveCount() => _moveCount;

    /// <summary>Gets the elapsed board time in seconds for tests.</summary>
    internal double TestGetElapsedSeconds() => _elapsedSeconds;

    /// <summary>Sets the elapsed board time in seconds for tests.</summary>
    internal void TestSetElapsedSeconds(double seconds)
    {
        _elapsedSeconds = seconds;
    }

    /// <summary>Gets whether the Help overlay is open for tests.</summary>
    internal bool TestIsHelpOverlayOpen() => _helpOpen;

    /// <summary>Gets whether sound is currently enabled for tests.</summary>
    internal bool TestIsSoundEnabled() => _soundEnabled;

    /// <summary>Sets the audio-unavailable state for tests.</summary>
    internal void TestSetAudioUnavailable(bool unavailable)
    {
        _audioUnavailable = unavailable;
    }

    /// <summary>Resets keyboard edge-detection state for tests.</summary>
    internal void TestResetPreviousKeyboardState()
    {
        _previousKeyboardState = default;
    }

    /// <summary>Processes one frame of global shortcuts for tests and advances key-edge state.</summary>
    internal bool TestHandleGlobalShortcuts(KeyboardState keyboardState)
    {
        var handled = HandleGlobalShortcuts(keyboardState);
        _previousKeyboardState = keyboardState;
        return handled;
    }

    /// <summary>Processes one frame of board keyboard navigation/selection for tests and advances key-edge state.</summary>
    internal void TestHandleKeyboardInput(KeyboardState keyboardState)
    {
        HandleKeyboardInput(keyboardState);
        _previousKeyboardState = keyboardState;
    }

    /// <summary>Finds the grid index of the tile containing the given logical point for tests.</summary>
    internal int TestGetTileIndexAt(Point logicalPosition) => GetTileIndexAt(logicalPosition);

    /// <summary>Gets the current number of tile columns for tests.</summary>
    internal int TestGetColumns() => _columns;

    /// <summary>Gets the current number of tile rows for tests.</summary>
    internal int TestGetRows() => _rows;

    /// <summary>Gets whether the board-size picker prompt is open for tests.</summary>
    internal bool TestIsSizePromptOpen() => _sizePromptOpen;

    /// <summary>Gets the currently highlighted size in the size picker prompt for tests.</summary>
    internal int TestGetSizePromptSelection() => SelectableGridSizes[_sizePromptIndex];

    /// <summary>Opens the board-size picker prompt for tests.</summary>
    internal void TestOpenSizePrompt() => OpenSizePrompt();

    /// <summary>Moves the size picker highlight for tests.</summary>
    internal void TestMoveSizePromptSelection(int delta) => MoveSizePromptSelection(delta);

    /// <summary>Confirms the currently highlighted size picker selection for tests.</summary>
    internal void TestConfirmSizePromptSelection() => ConfirmSizePromptSelection();

    /// <summary>Cancels the size picker prompt without changing the board for tests.</summary>
    internal void TestCancelSizePrompt() => CancelSizePrompt();
}
