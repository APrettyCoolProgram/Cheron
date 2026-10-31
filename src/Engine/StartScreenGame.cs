// 261029_code
// 261029_documentation

using System;
using System.Collections.Generic;
using Cheron.Engine;
using Cheron.Games.Memory;
using Cheron.Games.Solitaire;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Cheron.Games.Solitaire.SolitaireLayout;

namespace Cheron.Engine;

/// <summary>Provides the startup screen that can transition into a selected game module.</summary>
internal sealed class StartScreenGame : IGameModule
{
    private readonly List<StartScreenCartridge> _cartridges =
    [
        new("Solitaire", StartScreenGameKind.Solitaire, new Point(370, 560)),
        new("Memory", StartScreenGameKind.Memory, new Point(590, 560))
    ];

    private static readonly Rectangle ConsoleBounds = new(260, 140, 580, 260);

    private readonly List<NeonLine> _neonLines = new();

    private readonly Color _backgroundColor = new(18, 14, 30);
    private readonly Color _backgroundTopColor = new(30, 22, 54);
    private readonly Color _backgroundBottomColor = new(10, 10, 20);
    private readonly Color _consoleColor = new(48, 52, 74);
    private readonly Color _consoleTopColor = new(74, 80, 110);
    private readonly Color _consolePanelColor = new(34, 40, 62);
    private readonly Color _consoleTrimColor = new(20, 24, 40);
    private readonly Color _slotColor = new(14, 16, 26);
    private readonly Color _slotInnerColor = new(8, 10, 18);
    private readonly Color _slotGlowColor = new(66, 236, 255);
    private readonly Color _buttonOffColor = new(74, 74, 98);
    private readonly Color _buttonOnColor = new(120, 255, 190);
    private readonly Color _buttonRimColor = new(16, 20, 32);
    private readonly Color _buttonGlowColor = new(120, 255, 190);
    private readonly Color _cartridgeColor = new(56, 56, 74);
    private readonly Color _cartridgeRidgeColor = new(36, 36, 52);
    private readonly Color _cartridgeLabelColor = new(220, 228, 248);
    private readonly Color _statusTextColor = new(184, 230, 255);

    private const int CartridgeWidth = 160;
    private const int CartridgeHeight = 200;
    private const double PowerOnDelaySeconds = 1.0;

    private GameViewport _viewport;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private Texture2D _glyphTexture = null!;
    private SoundEffect? _insertClickSound;
    private SoundEffect? _sadPowerSound;
    private SoundEffect? _startupPowerSound;

    private MouseState _previousMouseState;
    private Point _pointerPosition;

    private int _draggingIndex = -1;
    private Point _dragOffset;

    private int _insertedCartridgeIndex = -1;
    private string _statusText = "CHERON";
    private bool _powerOnRequested;
    private double _powerOnElapsedSeconds;
    private double _powerFailureFlashSeconds;
    private double _idleAnimationSeconds;

    private IGameModule? _activeGame;
    private bool _activeGameInitialized;
    private bool _activeGameContentLoaded;

    public string Title => _activeGame?.Title ?? "Cheron";

    public Point PreferredWindowSize => BoardSize;

    public void Initialize()
    {
        InitializeNeonLines();
        _activeGame?.Initialize();
        _activeGameInitialized = _activeGame is not null;
    }

    public void SetViewport(GameViewport viewport)
    {
        _viewport = viewport;
        _activeGame?.SetViewport(viewport);
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);
        _glyphTexture = SolitaireBitmapFont.CreateGlyphTexture(graphicsDevice);
        _insertClickSound = CreateInsertClickSound();
        _sadPowerSound = CreateSadPowerSound();
        _startupPowerSound = CreateStartupPowerSound();

        if (_activeGame is not null)
        {
            _activeGame.LoadContent(graphicsDevice);
            _activeGameContentLoaded = true;
        }
    }

    public void Update(GameTime gameTime)
    {
        if (_activeGame is not null)
        {
            _activeGame.Update(gameTime);
            return;
        }

        _idleAnimationSeconds += gameTime.ElapsedGameTime.TotalSeconds;
        HandleMouseInput();

        if (_powerOnRequested)
        {
            _powerOnElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_powerOnElapsedSeconds >= PowerOnDelaySeconds)
            {
                TransitionToInsertedGame();
            }
        }

        if (_powerFailureFlashSeconds > 0)
        {
            _powerFailureFlashSeconds = Math.Max(0, _powerFailureFlashSeconds - gameTime.ElapsedGameTime.TotalSeconds);
        }
    }

    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        if (_activeGame is not null)
        {
            _activeGame.Draw(gameTime, graphicsDevice);
            return;
        }

        graphicsDevice.Clear(_backgroundColor);

        _spriteBatch.Begin(transformMatrix: _viewport.Transform, samplerState: SamplerState.PointClamp);
        DrawBackground();
        DrawConsole();
        DrawCartridges();
        DrawLabels();
        _spriteBatch.End();
    }

    public void Dispose()
    {
        _activeGame?.Dispose();
        _spriteBatch?.Dispose();
        _glyphTexture?.Dispose();
        _insertClickSound?.Dispose();
        _sadPowerSound?.Dispose();
        _startupPowerSound?.Dispose();
        _pixel?.Dispose();
    }

    private static readonly Point BoardSize = new(BoardWidth, BoardHeight);

    private void DrawBackground()
    {
        DrawRect(new Rectangle(0, 0, BoardSize.X, 500), _backgroundTopColor);
        DrawRect(new Rectangle(0, 500, BoardSize.X, BoardSize.Y - 500), _backgroundBottomColor);

        DrawRect(new Rectangle(0, 488, BoardSize.X, 10), new Color(24, 28, 44));
        DrawRect(new Rectangle(0, 498, BoardSize.X, 6), new Color(60, 180, 255, 70));

        foreach (var line in _neonLines)
        {
            var alpha = line.MinAlpha +
                (int)((line.MaxAlpha - line.MinAlpha) * (0.5f + 0.5f * MathF.Sin((float)_idleAnimationSeconds * 0.38f + line.PhaseOffset)));
            DrawRect(new Rectangle(0, line.Y, BoardSize.X, line.Height), new Color(line.Color, Math.Clamp(alpha, 4, 76)));
        }
    }

    private void DrawConsole()
    {
        var consoleBounds = ConsoleBounds;
        DrawRect(new Rectangle(consoleBounds.X + 10, consoleBounds.Bottom + 14, consoleBounds.Width - 20, 24),
            new Color(8, 10, 20, 140));

        DrawRect(new Rectangle(consoleBounds.X - 18, consoleBounds.Y - 9,
                consoleBounds.Width + 36, consoleBounds.Height + 18),
            new Color(96, 130, 255, 34));

        DrawRect(consoleBounds, _consoleColor);
        DrawRect(new Rectangle(consoleBounds.X, consoleBounds.Y, consoleBounds.Width, 56), _consoleTopColor);
        DrawRect(new Rectangle(consoleBounds.X + 6, consoleBounds.Y + 10, consoleBounds.Width - 12, 6),
            new Color(180, 208, 255, 70));
        DrawRect(new Rectangle(consoleBounds.X, consoleBounds.Bottom - 18, consoleBounds.Width, 18),
            _consoleTrimColor);

        DrawRect(new Rectangle(consoleBounds.X + 30, consoleBounds.Y + 30, 520, 60), _consolePanelColor);
        DrawRect(new Rectangle(consoleBounds.X + 40, consoleBounds.Y + 38, 500, 8), new Color(130, 188, 255, 82));
        DrawRect(new Rectangle(consoleBounds.X + 40, consoleBounds.Y + 52, 500, 5), new Color(26, 36, 62));

        var slotBounds = GetCartridgeSlotBounds();
        DrawRect(new Rectangle(slotBounds.X - 8, slotBounds.Y - 8, slotBounds.Width + 16, slotBounds.Height + 16),
            new Color(20, 24, 40));
        DrawRect(new Rectangle(slotBounds.X - 4, slotBounds.Y - 4, slotBounds.Width + 8, slotBounds.Height + 8),
            new Color(64, 88, 126));
        DrawRect(slotBounds, _slotColor);
        DrawRect(new Rectangle(slotBounds.X + 6, slotBounds.Y + 4, slotBounds.Width - 12, slotBounds.Height - 8),
            _slotInnerColor);
        DrawRect(new Rectangle(slotBounds.X + 10, slotBounds.Y + 8, slotBounds.Width - 20, slotBounds.Height - 16),
            new Color(4, 6, 12));

        var slotHoverBoost = slotBounds.Contains(_pointerPosition) || _draggingIndex >= 0 ? 64 : 34;
        DrawRect(new Rectangle(slotBounds.X - 3, slotBounds.Y - 3, slotBounds.Width + 6, 2),
            new Color(_slotGlowColor, slotHoverBoost));
        DrawRect(new Rectangle(slotBounds.X - 3, slotBounds.Bottom + 1, slotBounds.Width + 6, 2),
            new Color(_slotGlowColor, slotHoverBoost / 2));

        var shimmer = 0.5f + (MathF.Sin((float)_idleAnimationSeconds * 2.2f) * 0.5f);
        var slotTextColor = new Color(
            (byte)(120 + 90 * shimmer),
            (byte)(150 + 80 * (1 - shimmer)),
            (byte)(210 + 40 * shimmer));
        DrawCenteredText("INSERT CARTRIDGE", new Rectangle(slotBounds.X + 8, slotBounds.Y + 4, slotBounds.Width - 16,
            slotBounds.Height - 8), 1, slotTextColor);

        var buttonBounds = GetPowerButtonBounds();
        DrawRect(new Rectangle(buttonBounds.X - 4, buttonBounds.Y - 4, buttonBounds.Width + 8, buttonBounds.Height + 8),
            _buttonRimColor);
        DrawRect(buttonBounds, _buttonOffColor);
        DrawRect(new Rectangle(buttonBounds.X + 8, buttonBounds.Y + 8, buttonBounds.Width - 16, 6),
            new Color(230, 244, 255, 92));

        var powerIndicatorColor = _powerFailureFlashSeconds > 0
            ? new Color(255, 92, 92)
            : _powerOnRequested
                ? new Color(170, 255, 200)
                : new Color(100, 116, 144);
        DrawRect(new Rectangle(buttonBounds.X + buttonBounds.Width - 20, buttonBounds.Y + buttonBounds.Height - 18, 10, 10),
            powerIndicatorColor);
    }

    private void DrawCartridges()
    {
        for (var index = 0; index < _cartridges.Count; index++)
        {
            var cartridge = _cartridges[index];
            var drawBounds = GetCartridgeBounds(index);

            var hovered = drawBounds.Contains(_pointerPosition);

            DrawRect(new Rectangle(drawBounds.X + 6, drawBounds.Bottom + 4, drawBounds.Width - 12, 10),
                new Color(8, 12, 24, 120));
            DrawRect(drawBounds, _cartridgeColor);
            DrawRect(new Rectangle(drawBounds.X, drawBounds.Y, drawBounds.Width, 8), new Color(130, 170, 255, 78));
            DrawRect(new Rectangle(drawBounds.X + 8, drawBounds.Y + 36, drawBounds.Width - 16, 6), _cartridgeRidgeColor);

            var titleBounds = new Rectangle(drawBounds.X + 10, drawBounds.Y + 10, drawBounds.Width - 20, 26);
            DrawRect(titleBounds, _cartridgeLabelColor);
            DrawRect(new Rectangle(titleBounds.X, titleBounds.Y, titleBounds.Width, 2), new Color(255, 255, 255, 100));
            DrawRect(new Rectangle(titleBounds.X, titleBounds.Y, 2, titleBounds.Height), new Color(255, 255, 255, 78));
            DrawRect(new Rectangle(titleBounds.X, titleBounds.Bottom - 2, titleBounds.Width, 2), new Color(34, 42, 58, 120));
            DrawRect(new Rectangle(titleBounds.Right - 2, titleBounds.Y, 2, titleBounds.Height), new Color(34, 42, 58, 90));

            var coverBounds = new Rectangle(drawBounds.X + 10, drawBounds.Y + 44, drawBounds.Width - 20, drawBounds.Height - 54);
            DrawCartridgeCover(index, coverBounds);

            if (index == _draggingIndex && IsDraggingCartridgeInInsertZone())
            {
                var pulse = 0.5f + 0.5f * MathF.Sin((float)_idleAnimationSeconds * 6f);
                var neon = new Color(
                    (byte)(80 + 80 * pulse),
                    (byte)(200 + 55 * pulse),
                    (byte)255,
                    (byte)(140 + 90 * pulse));
                DrawRect(new Rectangle(drawBounds.X - 4, drawBounds.Y - 4, drawBounds.Width + 8, 2), neon);
                DrawRect(new Rectangle(drawBounds.X - 4, drawBounds.Bottom + 2, drawBounds.Width + 8, 2), neon);
                DrawRect(new Rectangle(drawBounds.X - 4, drawBounds.Y - 2, 2, drawBounds.Height + 4), neon);
                DrawRect(new Rectangle(drawBounds.Right + 2, drawBounds.Y - 2, 2, drawBounds.Height + 4), neon);
            }
            else if (hovered && _draggingIndex < 0)
            {
                DrawRect(new Rectangle(drawBounds.X - 2, drawBounds.Y - 2, drawBounds.Width + 4, 2),
                    new Color(90, 240, 255, 42));
                DrawRect(new Rectangle(drawBounds.X - 2, drawBounds.Bottom, drawBounds.Width + 4, 2),
                    new Color(250, 110, 220, 34));
            }

            DrawCenteredText(cartridge.Label, new Rectangle(drawBounds.X + 12, drawBounds.Y + 13, drawBounds.Width - 24, 20),
                2, _consoleColor);

            if (index == _insertedCartridgeIndex)
            {
                DrawRect(new Rectangle(drawBounds.X, drawBounds.Y, drawBounds.Width, 4), _buttonOnColor);
            }
        }
    }

    private void DrawLabels()
    {
        var superTitleBounds = new Rectangle(ConsoleBounds.X + 120, ConsoleBounds.Y + 186, 340, 24);
        DrawCenteredText("SUPERCHERON-128", superTitleBounds, 2, Color.White);

        var powerLabelBounds = new Rectangle(GetPowerButtonBounds().X + 4, GetPowerButtonBounds().Y + 18, 48, 18);
        DrawCenteredText("POWER", powerLabelBounds, 1, _cartridgeLabelColor);
    }

    private Rectangle GetCartridgeSlotBounds() => new(ConsoleBounds.X + 200, ConsoleBounds.Y + 42, 190, 22);

    private Rectangle GetPowerButtonBounds() => new(ConsoleBounds.X + 430, ConsoleBounds.Y + 95, 86, 50);

    private Rectangle GetCartridgeBounds(int index)
    {
        var cartridge = _cartridges[index];
        return new Rectangle(cartridge.Position.X, cartridge.Position.Y, CartridgeWidth, CartridgeHeight);
    }

    private bool IsDraggingCartridgeInInsertZone()
    {
        if (_draggingIndex < 0)
        {
            return false;
        }

        var slotBounds = GetCartridgeSlotBounds();
        var cartridgeBounds = GetCartridgeBounds(_draggingIndex);
        var cartridgeCenter = new Point(cartridgeBounds.Center.X, cartridgeBounds.Center.Y);
        var snapZone = slotBounds;
        snapZone.Inflate(40, 36);
        return snapZone.Contains(cartridgeCenter);
    }

    private void HandleMouseInput()
    {
        var mouseState = Mouse.GetState();
        var logicalPosition = _viewport.ToLogical(mouseState.Position);
        _pointerPosition = logicalPosition;
        var leftPressed = mouseState.LeftButton == ButtonState.Pressed;
        var leftJustPressed = leftPressed && _previousMouseState.LeftButton == ButtonState.Released;
        var leftJustReleased = !leftPressed && _previousMouseState.LeftButton == ButtonState.Pressed;

        if (leftJustPressed)
        {
            if (TryPressPowerButton(logicalPosition))
            {
                _previousMouseState = mouseState;
                return;
            }

            TryBeginDrag(logicalPosition);
        }

        if (_draggingIndex >= 0 && leftPressed)
        {
            var newPosition = new Point(logicalPosition.X - _dragOffset.X, logicalPosition.Y - _dragOffset.Y);
            _cartridges[_draggingIndex].Position = newPosition;
        }

        if (_draggingIndex >= 0 && leftJustReleased)
        {
            EndDragAndSnap();
        }

        _previousMouseState = mouseState;
    }

    private void TryBeginDrag(Point position)
    {
        if (_powerOnRequested)
        {
            return;
        }

        for (var index = _cartridges.Count - 1; index >= 0; index--)
        {
            var bounds = GetCartridgeBounds(index);
            if (!bounds.Contains(position))
            {
                continue;
            }

            _draggingIndex = index;
            _dragOffset = new Point(position.X - bounds.X, position.Y - bounds.Y);
            if (_insertedCartridgeIndex == index)
            {
                _insertedCartridgeIndex = -1;
                _statusText = "CHERON";
            }

            break;
        }
    }

    private void EndDragAndSnap()
    {
        var draggingIndex = _draggingIndex;
        _draggingIndex = -1;

        if (draggingIndex < 0)
        {
            return;
        }

        var slotBounds = GetCartridgeSlotBounds();
        var cartridgeBounds = GetCartridgeBounds(draggingIndex);
        var cartridgeCenter = new Point(cartridgeBounds.Center.X, cartridgeBounds.Center.Y);
        var snapZone = slotBounds;
        snapZone.Inflate(40, 36);

        if (!snapZone.Contains(cartridgeCenter))
        {
            _cartridges[draggingIndex].Position = _cartridges[draggingIndex].HomePosition;
            _statusText = "CHERON";
            return;
        }

        _insertedCartridgeIndex = draggingIndex;
        _cartridges[draggingIndex].Position = new Point(slotBounds.Center.X - CartridgeWidth / 2,
            slotBounds.Y - CartridgeHeight + 16);
        _insertClickSound?.Play(0.46f, 0f, 0f);
        _statusText = "PRESS POWER";
        _powerOnRequested = false;
        _powerOnElapsedSeconds = 0;
    }

    private bool TryPressPowerButton(Point logicalPosition)
    {
        if (_powerOnRequested)
        {
            return false;
        }

        if (!GetPowerButtonBounds().Contains(logicalPosition))
        {
            return false;
        }

        if (_insertedCartridgeIndex < 0)
        {
            _powerFailureFlashSeconds = 0.35;
            _sadPowerSound?.Play(0.52f, -0.2f, 0f);
            _statusText = "CHERON";
            return true;
        }

        _powerOnRequested = true;
        _powerOnElapsedSeconds = 0;
        _powerFailureFlashSeconds = 0;
        _startupPowerSound?.Play(0.56f, 0.1f, 0f);
        _statusText = "POWERING ON";
        return true;
    }

    private void TransitionToInsertedGame()
    {
        if (_insertedCartridgeIndex < 0)
        {
            _powerOnRequested = false;
            _powerOnElapsedSeconds = 0;
            _statusText = "CHERON";
            return;
        }

        var cartridge = _cartridges[_insertedCartridgeIndex];
        if (cartridge.GameKind == StartScreenGameKind.None)
        {
            _powerOnRequested = false;
            _powerOnElapsedSeconds = 0;
            _statusText = "CARTRIDGE NOT READY";
            return;
        }

        IGameModule nextGame = cartridge.GameKind switch
        {
            StartScreenGameKind.Solitaire => new SolitaireGame(),
            StartScreenGameKind.Memory => new MemoryGame(),
            _ => throw new InvalidOperationException($"Unsupported game kind: {cartridge.GameKind}.")
        };

        nextGame.Initialize();
        nextGame.SetViewport(_viewport);
        nextGame.LoadContent(_pixel.GraphicsDevice);

        _activeGame = nextGame;
        _activeGameInitialized = true;
        _activeGameContentLoaded = true;
    }

    private void DrawRect(Rectangle rectangle, Color color)
    {
        _spriteBatch.Draw(_pixel, rectangle, color);
    }

    private void DrawCartridgeCover(int index, Rectangle coverBounds)
    {
        var coverPalettes = new[]
        {
            new[] { new Color(38, 50, 78), new Color(60, 86, 132), new Color(92, 122, 168) },
            new[] { new Color(52, 46, 70), new Color(78, 70, 108), new Color(112, 102, 146) },
            new[] { new Color(34, 58, 70), new Color(56, 90, 110), new Color(84, 128, 150) },
            new[] { new Color(60, 44, 62), new Color(92, 68, 98), new Color(124, 96, 136) }
        };

        var palette = coverPalettes[index % coverPalettes.Length];
        DrawRect(coverBounds, palette[0]);
        DrawRect(new Rectangle(coverBounds.X + 6, coverBounds.Y + 6, coverBounds.Width - 12, coverBounds.Height / 3), palette[1]);
        DrawRect(new Rectangle(coverBounds.X + 10, coverBounds.Y + 14, coverBounds.Width - 20, 3),
            new Color(240, 248, 255, 66));
        DrawRect(new Rectangle(coverBounds.X + 6, coverBounds.Y + coverBounds.Height / 2, coverBounds.Width - 12,
            coverBounds.Height / 2 - 10), palette[2]);
    }

    private void DrawCenteredText(string text, Rectangle bounds, int scale, Color color)
    {
        var textWidth = GetTextWidth(text, scale);
        var textHeight = 7 * scale;
        var textX = bounds.X + (bounds.Width - textWidth) / 2;
        var textY = bounds.Y + (bounds.Height - textHeight) / 2;
        SolitaireBitmapFont.DrawText(_spriteBatch, _glyphTexture, false, text, textX, textY, scale, color,
            preserveColor: true);
    }

    private static int GetTextWidth(string text, int scale)
    {
        return text.Length * 6 * scale;
    }

    private void InitializeNeonLines()
    {
        _neonLines.Clear();
        var random = new Random();
        var lineCount = 4;
        var minY = ConsoleBounds.Y + 4;
        var maxY = ConsoleBounds.Bottom - 12;

        for (var index = 0; index < lineCount; index++)
        {
            var y = random.Next(minY, maxY);
            var height = random.Next(2, 6);
            var minAlpha = random.Next(4, 14);
            var maxAlpha = random.Next(36, 72);
            var phaseOffset = (float)(random.NextDouble() * Math.PI * 2);
            var color = (index % 3) switch
            {
                0 => new Color(78, 230, 255),
                1 => new Color(255, 92, 220),
                _ => new Color(120, 220, 255)
            };

            _neonLines.Add(new NeonLine(y, height, minAlpha, maxAlpha, phaseOffset, color));
        }
    }

    private static SoundEffect CreateInsertClickSound()
    {
        const int sampleRate = 22050;
        const int durationSamples = 1800;
        var pcm = new byte[durationSamples * 2];

        for (var sampleIndex = 0; sampleIndex < durationSamples; sampleIndex++)
        {
            var t = sampleIndex / (float)sampleRate;
            var envelope = MathF.Exp(-24f * t);
            var wave = MathF.Sin(2 * MathF.PI * 900f * t) + 0.55f * MathF.Sin(2 * MathF.PI * 1850f * t);
            var amplitude = wave * envelope * 0.55f;
            var value = (short)Math.Clamp(amplitude * short.MaxValue, short.MinValue, short.MaxValue);
            pcm[sampleIndex * 2] = (byte)(value & 0xFF);
            pcm[sampleIndex * 2 + 1] = (byte)((value >> 8) & 0xFF);
        }

        return new SoundEffect(pcm, sampleRate, AudioChannels.Mono);
    }

    private static SoundEffect CreateSadPowerSound()
    {
        const int sampleRate = 22050;
        const int durationSamples = 7000;
        var pcm = new byte[durationSamples * 2];

        for (var sampleIndex = 0; sampleIndex < durationSamples; sampleIndex++)
        {
            var t = sampleIndex / (float)sampleRate;
            var envelope = MathF.Exp(-3.4f * t);
            var downSweep = 240f - 120f * (sampleIndex / (float)durationSamples);
            var wave = MathF.Sin(2 * MathF.PI * downSweep * t) + 0.35f * MathF.Sin(2 * MathF.PI * (downSweep * 0.5f) * t);
            var amplitude = wave * envelope * 0.5f;
            var value = (short)Math.Clamp(amplitude * short.MaxValue, short.MinValue, short.MaxValue);
            pcm[sampleIndex * 2] = (byte)(value & 0xFF);
            pcm[sampleIndex * 2 + 1] = (byte)((value >> 8) & 0xFF);
        }

        return new SoundEffect(pcm, sampleRate, AudioChannels.Mono);
    }

    private static SoundEffect CreateStartupPowerSound()
    {
        const int sampleRate = 22050;
        const int durationSamples = 8200;
        var pcm = new byte[durationSamples * 2];

        for (var sampleIndex = 0; sampleIndex < durationSamples; sampleIndex++)
        {
            var t = sampleIndex / (float)sampleRate;
            var envelope = MathF.Exp(-2.8f * t);
            var upSweep = 220f + 280f * (sampleIndex / (float)durationSamples);
            var wave = MathF.Sin(2 * MathF.PI * upSweep * t) + 0.25f * MathF.Sin(2 * MathF.PI * (upSweep * 2f) * t);
            var amplitude = wave * envelope * 0.52f;
            var value = (short)Math.Clamp(amplitude * short.MaxValue, short.MinValue, short.MaxValue);
            pcm[sampleIndex * 2] = (byte)(value & 0xFF);
            pcm[sampleIndex * 2 + 1] = (byte)((value >> 8) & 0xFF);
        }

        return new SoundEffect(pcm, sampleRate, AudioChannels.Mono);
    }

    private readonly record struct NeonLine(int Y, int Height, int MinAlpha, int MaxAlpha, float PhaseOffset, Color Color);

    /// <summary>Identifies which game module a start-screen cartridge launches.</summary>
    private enum StartScreenGameKind
    {
        /// <summary>The cartridge is not wired to a playable game yet.</summary>
        None,

        /// <summary>The cartridge launches the Solitaire game module.</summary>
        Solitaire,

        /// <summary>The cartridge launches the Memory game module.</summary>
        Memory
    }

    private sealed class StartScreenCartridge
    {
        public StartScreenCartridge(string label, StartScreenGameKind gameKind, Point position)
        {
            Label = label;
            GameKind = gameKind;
            Position = position;
            HomePosition = position;
        }

        public string Label { get; }

        public StartScreenGameKind GameKind { get; }

        public Point Position { get; set; }

        public Point HomePosition { get; }
    }
}
