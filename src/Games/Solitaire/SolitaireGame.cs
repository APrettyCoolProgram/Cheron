// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cheron.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Games.Solitaire;

/// <summary>Implements Klondike rules, state, interaction, and rendering independently of the application host.</summary>
internal sealed class SolitaireGame : IGameModule
{
    /// <summary>The width of a card or empty pile slot, in logical board units.</summary>
    private const int CardWidth = 78;

    /// <summary>The height of a card or empty pile slot, in logical board units.</summary>
    private const int CardHeight = 108;

    /// <summary>The horizontal space between adjacent foundation slots, in logical board units.</summary>
    private const int CardGap = 18;

    /// <summary>The vertical position of the first card in every tableau column, in logical board units.</summary>
    private const int TableauTop = 218;

    /// <summary>The vertical offset between consecutive tableau cards when the preceding card is face up.</summary>
    private const int TableauStep = 29;

    /// <summary>The horizontal position of the first tableau column, in logical board units.</summary>
    private const int FirstColumnX = 32;

    /// <summary>The horizontal offset between consecutive tableau columns, in logical board units.</summary>
    private const int ColumnStep = 148;

    /// <summary>The logical board width used for viewport fitting and layout math.</summary>
    private const int BoardWidth = 1100;

    /// <summary>The logical board height used for viewport fitting and layout math.</summary>
    private const int BoardHeight = 800;

    /// <summary>The title shown in the host window.</summary>
    private const string GameTitle = "Cheron Solitaire";

    /// <summary>Stores seven five-bit bitmap rows for each uppercase letter followed by each decimal digit.</summary>
    /// <remarks>
    /// The first dimension selects A through Z, then 0 through 9. The second selects a glyph row.
    /// Bits four through zero represent pixels from left to right; exclamation marks are generated separately.
    /// </remarks>
    private static readonly int[,] Glyphs =
    {
        { 14, 17, 17, 31, 17, 17, 17 }, { 30, 17, 17, 30, 17, 17, 30 },
        { 14, 17, 16, 16, 16, 17, 14 }, { 30, 17, 17, 17, 17, 17, 30 },
        { 31, 16, 16, 30, 16, 16, 31 }, { 31, 16, 16, 30, 16, 16, 16 },
        { 14, 17, 16, 23, 17, 17, 15 }, { 17, 17, 17, 31, 17, 17, 17 },
        { 14, 4, 4, 4, 4, 4, 14 }, { 7, 2, 2, 2, 18, 18, 12 },
        { 17, 18, 20, 24, 20, 18, 17 }, { 16, 16, 16, 16, 16, 16, 31 },
        { 17, 27, 21, 21, 17, 17, 17 }, { 17, 25, 21, 19, 17, 17, 17 },
        { 14, 17, 17, 17, 17, 17, 14 }, { 30, 17, 17, 30, 16, 16, 16 },
        { 14, 17, 17, 17, 21, 18, 13 }, { 30, 17, 17, 30, 20, 18, 17 },
        { 15, 16, 16, 14, 1, 1, 30 }, { 31, 4, 4, 4, 4, 4, 4 },
        { 17, 17, 17, 17, 17, 17, 14 }, { 17, 17, 17, 17, 17, 10, 4 },
        { 17, 17, 17, 21, 21, 21, 10 }, { 17, 17, 10, 4, 10, 17, 17 },
        { 17, 17, 10, 4, 4, 4, 4 }, { 31, 1, 2, 4, 8, 16, 31 },
        { 14, 17, 19, 21, 25, 17, 14 }, { 4, 12, 4, 4, 4, 4, 14 },
        { 14, 17, 1, 2, 4, 8, 31 }, { 30, 1, 1, 14, 1, 1, 30 },
        { 2, 6, 10, 18, 31, 2, 2 }, { 31, 16, 16, 30, 1, 1, 30 },
        { 14, 16, 16, 30, 17, 17, 14 }, { 31, 1, 2, 4, 8, 8, 8 },
        { 14, 17, 17, 14, 17, 17, 14 }, { 14, 17, 17, 15, 1, 1, 14 }
    };

    /// <summary>The face-down draw pile, with the next card at the end of the list.</summary>
    private readonly List<Card> _stock = new();

    /// <summary>The face-up drawn cards, with the available top card at the end of the list.</summary>
    private readonly List<Card> _waste = new();

    /// <summary>The seven tableau columns, each ordered from its bottom card to its exposed top card.</summary>
    private readonly List<List<Card>> _tableau = new();

    /// <summary>The four foundation piles, each built from Ace to King in a suit determined by its first card.</summary>
    private readonly List<List<Card>> _foundations = new();

    /// <summary>The module-owned sprite batch, created during content loading and released during disposal.</summary>
    private SpriteBatch _spriteBatch = null!;

    /// <summary>The module-owned white pixel used to draw solid rectangles.</summary>
    private Texture2D _pixelTexture = null!;

    /// <summary>The module-owned padded bitmap glyph atlas, filtered when the viewport uses a fractional scale.</summary>
    private Texture2D _glyphTexture = null!;

    /// <summary>The module-owned cached textures used for the table, cards, buttons, and pile slots.</summary>
    private SolitaireArtwork _artwork = null!;

    /// <summary>The last processed keyboard state, used to detect new shortcut key presses.</summary>
    private KeyboardState _previousKeyboardState;

    /// <summary>The source pile for a pending move, or <see langword="null"/> when no cards are selected.</summary>
    private List<Card>? _selectedPile;

    /// <summary>The index of the first selected card; subsequent cards in the same pile move with it.</summary>
    private int _selectedCardIndex;

    /// <summary>Whether the last completed move left all four foundations with thirteen cards each.</summary>
    private bool _hasWon;

    /// <summary>Gets the Solitaire window title.</summary>
    public string Title => GameTitle;

    /// <summary>Gets the logical card-table size, also used to request the initial window size.</summary>
    public Point PreferredWindowSize => new(BoardWidth, BoardHeight);

    /// <summary>Deals a shuffled game before graphics resources are loaded.</summary>
    public void Initialize()
    {
        StartNewGame();
    }

    /// <summary>Creates owned drawing resources, card-table artwork, and optional synthesized sounds.</summary>
    /// <remarks>
    /// Call once on the graphics thread after the device becomes available and before drawing.
    /// Audio-device failures are reported in diagnostics and on the board; visual feedback remains available.
    /// The module releases these resources in <see cref="Dispose"/>; it does not own the graphics device.
    /// </remarks>
    /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
        _glyphTexture = CreateGlyphTexture(graphicsDevice);
        _artwork = new SolitaireArtwork(graphicsDevice);
        _boardRasterizerState = new RasterizerState { ScissorTestEnable = true };
        LoadFeedbackSounds();
    }

    /// <summary>Updates statistics, animations, feedback, toolbar shortcuts, clicks, and card dragging.</summary>
    /// <param name="gameTime">The host-supplied timing information used for statistics, gestures, and animations.</param>
    public void Update(GameTime gameTime)
    {
        UpdateControls(gameTime.ElapsedGameTime);
        UpdateAnimations(gameTime.ElapsedGameTime);
        UpdateFeedback(gameTime.ElapsedGameTime);
        var keyboardState = Keyboard.GetState();

        if (!HandleAccessibilityKeys(keyboardState))
        {
            if (IsNewKey(keyboardState, Keys.R))
            {
                StartNewGame();
            }
            else
            {
                HandleControlKeys(keyboardState);
                HandleBoardKeyboard(keyboardState);
            }
        }

        var mouseState = Mouse.GetState();

        UpdateClientPointer(new Point(mouseState.X, mouseState.Y),
            mouseState.LeftButton == ButtonState.Pressed, gameTime.TotalGameTime);

        _previousKeyboardState = keyboardState;
    }

    /// <summary>Draws the proportionally scaled board, cards, controls, and win message.</summary>
    /// <remarks>
    /// Fits the fixed logical canvas into the host viewport with centered margins.
    /// Draws directly at drawable resolution and requires loaded drawing resources.
    /// </remarks>
    /// <param name="gameTime">The host-supplied timing information; animation progress is advanced during updates.</param>
    /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(new Color(12, 34, 39));
        var previousScissor = graphicsDevice.ScissorRectangle;
        var previousRasterizer = graphicsDevice.RasterizerState;
        graphicsDevice.ScissorRectangle = _viewport.ScissorBounds;

        BeginBoardDrawing();
        if (_highContrast)
        {
            Fill(new Rectangle(0, 0, BoardWidth, BoardHeight), Color.Black);
        }
        else
        {
            _spriteBatch.Draw(_artwork.Background, new Rectangle(0, 0, BoardWidth, BoardHeight), Color.White);
        }

        Fill(new Rectangle(0, 0, BoardWidth, 72), _highContrast ? Color.Black : new Color(12, 31, 37, 220));
        Fill(new Rectangle(0, 71, BoardWidth, 1), new Color(67, 102, 102));
        DrawToolbar();

        var stockBounds = new Rectangle(32, 82, CardWidth, CardHeight);

        DrawPile(_stock, stockBounds, faceDown: true);

        if (_stock.Count == 0)
        {
            DrawText("DEAL", _largeText ? 35 : 47, 122, SmallTextScale, new Color(156, 186, 178));
            DrawText("AGAIN", _largeText ? 38 : 44, _largeText ? 142 : 138, SmallTextScale, new Color(156, 186, 178));
        }

        DrawPile(_waste, new Rectangle(128, 82, CardWidth, CardHeight));
        DrawText("STOCK", _largeText ? 36 : 40, 199, SmallTextScale, new Color(157, 184, 176));
        DrawText("WASTE", _largeText ? 132 : 136, 199, SmallTextScale, new Color(157, 184, 176));

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            var foundationBounds = GetFoundationBounds(foundationIndex);

            DrawPile(_foundations[foundationIndex], foundationBounds);

            if (_foundations[foundationIndex].Count == 0)
            {
                DrawText("A", foundationBounds.X + 33, foundationBounds.Y + 24, 2, new Color(119, 153, 145));
                DrawText(_largeText ? "TO K" : "TO KING", foundationBounds.X + 19, foundationBounds.Y + 65,
                    SmallTextScale, new Color(119, 153, 145));
            }
        }

        DrawText("FOUNDATIONS", _largeText ? 802 : 796, 199, SmallTextScale, new Color(157, 184, 176));

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            var tableauPile = _tableau[columnIndex];
            var cardY = TableauTop;

            if (tableauPile.Count == 0)
            {
                var slotBounds = new Rectangle(FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);
                DrawSlot(slotBounds);
                DrawText("K", slotBounds.X + 33, slotBounds.Y + 46, 2, new Color(119, 153, 145));
            }

            for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
            {
                var card = tableauPile[cardIndex];
                var cardBounds = new Rectangle(FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);

                if (!IsDraggedCard(tableauPile, cardIndex))
                {
                    DrawCard(card, cardBounds, IsSelected(tableauPile, cardIndex));
                }

                cardY += card.FaceUp ? TableauStep : 22;
            }
        }

        DrawInteractionFeedback();
        DrawCardTransitions();
        DrawInvalidFeedback();
        DrawWinCelebration();

        Fill(new Rectangle(0, 728, BoardWidth, 72), _highContrast ? Color.Black : new Color(12, 31, 37, 230));
        Fill(new Rectangle(0, 728, BoardWidth, 1), new Color(67, 102, 102));
        DrawAccessibleFooter();

        if (_hasWon && _cardTransitions.Count == 0)
        {
            _spriteBatch.Draw(_artwork.Banner, new Rectangle(370, 376, 360, 56), Color.White);
            DrawText("YOU WIN!", 483, 394, 3, new Color(251, 219, 148));
        }

        if (_keyboardFocusVisible && !_optionsOpen)
        {
            DrawFocusCorners(GetKeyboardBounds());
        }

        if (_optionsOpen)
        {
            DrawAccessibilityOptions();
        }

        _spriteBatch.End();
        graphicsDevice.ScissorRectangle = previousScissor;
        graphicsDevice.RasterizerState = previousRasterizer;
    }

    /// <summary>Releases owned drawing and audio resources without disposing the borrowed graphics device.</summary>
    public void Dispose()
    {
        DisposeFeedbackSounds();
        _boardRasterizerState?.Dispose();
        _artwork?.Dispose();
        _glyphTexture?.Dispose();
        _pixelTexture?.Dispose();
        _spriteBatch?.Dispose();
    }

    /// <summary>The sound button bounds, shared by drawing and pointer input.</summary>
    private static readonly Rectangle SoundButtonBounds = new(1008, 20, 68, 44);

    /// <summary>The lifetime of rejected-move outlines and instructions.</summary>
    private static readonly TimeSpan InvalidFeedbackDuration = TimeSpan.FromSeconds(1.4);

    /// <summary>The duration of the nonblocking win celebration.</summary>
    private static readonly TimeSpan CelebrationDuration = TimeSpan.FromSeconds(3);

    /// <summary>The duration of an ordinary card slide or flip, in seconds.</summary>
    private const float MoveDuration = 0.22f;

    /// <summary>The module-owned sounds, ordered by <see cref="FeedbackSound"/>.</summary>
    private readonly List<SoundEffect> _feedbackSounds = new();

    /// <summary>The module-owned playback instances, stopped immediately when muted.</summary>
    private readonly List<SoundEffectInstance> _feedbackVoices = new();

    /// <summary>The active card transitions, ordered from underlying cards to overlying cards.</summary>
    private readonly Dictionary<Card, CardTransition> _cardTransitions = new();

    /// <summary>Whether sounds are enabled for this module session, preserved across deals.</summary>
    private bool _soundEnabled = true;

    /// <summary>Whether the audio device could not be initialized or used.</summary>
    private bool _audioUnavailable;

    /// <summary>The elapsed time since the current transition group began, in seconds.</summary>
    private float _animationElapsed;

    /// <summary>The elapsed time since the current card selection began, in seconds.</summary>
    private float _selectionElapsed;

    /// <summary>The time remaining for the current rejected-move feedback.</summary>
    private TimeSpan _invalidTimeRemaining;

    /// <summary>The rejected destination or source card to outline.</summary>
    private Rectangle _invalidBounds;

    /// <summary>Whether the current win has already started its one-shot celebration.</summary>
    private bool _winFeedbackShown;

    /// <summary>The elapsed time since the current win celebration began.</summary>
    private TimeSpan _celebrationElapsed;

    /// <summary>Identifies the cached sound associated with a player-visible outcome.</summary>
    private enum FeedbackSound
    {
        /// <summary>A soft tone for drawing, recycling, moving, and undoing cards.</summary>
        Move,
        /// <summary>A rising pair of tones for advancing a foundation.</summary>
        Foundation,
        /// <summary>A quiet low tone for a rejected move.</summary>
        Invalid,
        /// <summary>A short ascending phrase after the final card settles.</summary>
        Win
    }

    /// <summary>Describes a card's position and visibility before a logical operation.</summary>
    /// <param name="Bounds">The full card rectangle in board coordinates.</param>
    /// <param name="FaceUp">Whether the card face was visible.</param>
    private readonly record struct CardPresentation(Rectangle Bounds, bool FaceUp);

    /// <summary>Describes the endpoints and delay of one presentation-only card transition.</summary>
    /// <param name="Start">The source position and face state.</param>
    /// <param name="End">The destination position and face state.</param>
    /// <param name="Delay">The delay before this transition begins, in seconds.</param>
    private readonly record struct CardTransition(CardPresentation Start, CardPresentation End, float Delay);

    /// <summary>Creates original, short PCM sounds and playback instances without external assets.</summary>
    private void LoadFeedbackSounds()
    {
        try
        {
            AddFeedbackSound([330], 0.07);
            AddFeedbackSound([523.25, 659.25], 0.08);
            AddFeedbackSound([196], 0.1);
            AddFeedbackSound([523.25, 659.25, 783.99, 1046.5], 0.12);
        }
        catch (NoAudioHardwareException exception)
        {
            ReportAudioUnavailable(exception);
        }
    }

    /// <summary>Creates one cached mono sound with smooth attack and release envelopes for each note.</summary>
    /// <param name="frequencies">The note frequencies in hertz.</param>
    /// <param name="noteSeconds">The duration of each note in seconds.</param>
    private void AddFeedbackSound(double[] frequencies, double noteSeconds)
    {
        const int sampleRate = 22050;
        var noteSamples = (int)(sampleRate * noteSeconds);
        var samples = new byte[noteSamples * frequencies.Length * 2];
        for (var noteIndex = 0; noteIndex < frequencies.Length; noteIndex++)
        {
            for (var sampleIndex = 0; sampleIndex < noteSamples; sampleIndex++)
            {
                var progress = (double)sampleIndex / (noteSamples - 1);
                var envelope = Math.Sin(Math.PI * progress);
                var wave = Math.Sin(2 * Math.PI * frequencies[noteIndex] * sampleIndex / sampleRate);
                var sample = (short)(wave * envelope * envelope * short.MaxValue * 0.5);
                var byteIndex = (noteIndex * noteSamples + sampleIndex) * 2;
                samples[byteIndex] = (byte)(sample & 255);
                samples[byteIndex + 1] = (byte)((sample >> 8) & 255);
            }
        }

        var sound = new SoundEffect(samples, sampleRate, AudioChannels.Mono);
        _feedbackSounds.Add(sound);
        var voice = sound.CreateInstance();
        _feedbackVoices.Add(voice);
        voice.Volume = 0.18f;
    }

    /// <summary>Plays one quiet cue, restarting its existing voice rather than accumulating overlapping copies.</summary>
    /// <param name="sound">The outcome to announce.</param>
    private void PlayFeedbackSound(FeedbackSound sound)
    {
        if (!_soundEnabled || _audioUnavailable || _feedbackVoices.Count == 0)
        {
            return;
        }

        try
        {
            var voice = _feedbackVoices[(int)sound];
            voice.Stop();
            voice.Play();
        }
        catch (NoAudioHardwareException exception)
        {
            ReportAudioUnavailable(exception);
        }
    }

    /// <summary>Reports audio failure through diagnostics and the visible sound-unavailable state.</summary>
    /// <param name="exception">The audio-device failure reported by MonoGame.</param>
    private void ReportAudioUnavailable(NoAudioHardwareException exception)
    {
        Trace.TraceWarning($"Solitaire sound unavailable: {exception}");
        _audioUnavailable = true;
        DisposeFeedbackSounds();
    }

    /// <summary>Toggles session-local audio without modifying the board or replaying muted cues.</summary>
    private void ToggleSound()
    {
        if (_audioUnavailable)
        {
            return;
        }

        _soundEnabled = !_soundEnabled;
        if (!_soundEnabled)
        {
            foreach (var voice in _feedbackVoices)
            {
                voice.Stop();
            }
        }
    }

    /// <summary>Releases playback instances before their backing sound resources.</summary>
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

    /// <summary>Clears transient feedback without changing the session's sound preference.</summary>
    private void ResetFeedback()
    {
        _invalidTimeRemaining = TimeSpan.Zero;
        _invalidBounds = Rectangle.Empty;
        _winFeedbackShown = false;
        _celebrationElapsed = TimeSpan.Zero;
        foreach (var voice in _feedbackVoices)
        {
            voice.Stop();
        }
    }

    /// <summary>Shows brief, nonblocking feedback for an actual rejected player move.</summary>
    /// <param name="position">The attempted destination or double-click position.</param>
    private void ShowInvalidMove(Point position)
    {
        ClearHint();
        _invalidBounds = Rectangle.Empty;
        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            var bounds = GetFoundationBounds(foundationIndex);
            if (bounds.Contains(position))
            {
                _invalidBounds = bounds;
                break;
            }
        }

        if (_invalidBounds == Rectangle.Empty)
        {
            for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
            {
                if (GetTableauBounds(columnIndex).Contains(position))
                {
                    _invalidBounds = GetTableauCardBounds(columnIndex, Math.Max(0, _tableau[columnIndex].Count - 1));
                    break;
                }
            }
        }

        _invalidTimeRemaining = InvalidFeedbackDuration;
        PlayFeedbackSound(FeedbackSound.Invalid);
    }

    /// <summary>Advances feedback and starts a single celebration after the winning transition finishes.</summary>
    /// <param name="elapsed">The elapsed update time, independent of frame rate.</param>
    private void UpdateFeedback(TimeSpan elapsed)
    {
        _invalidTimeRemaining = elapsed >= _invalidTimeRemaining ? TimeSpan.Zero : _invalidTimeRemaining - elapsed;
        if (!_hasWon)
        {
            _winFeedbackShown = false;
            _celebrationElapsed = TimeSpan.Zero;
            return;
        }

        if (!_winFeedbackShown && _cardTransitions.Count == 0)
        {
            _winFeedbackShown = true;
            _celebrationElapsed = TimeSpan.Zero;
            PlayFeedbackSound(FeedbackSound.Win);
        }
        else if (_winFeedbackShown)
        {
            _celebrationElapsed = elapsed >= CelebrationDuration - _celebrationElapsed
                ? CelebrationDuration : _celebrationElapsed + elapsed;
        }
    }

    /// <summary>Draws a theme-aware rejected-move outline, kept static when reduced motion is enabled.</summary>
    private void DrawInvalidFeedback()
    {
        if (_invalidTimeRemaining > TimeSpan.Zero && _invalidBounds != Rectangle.Empty)
        {
            var opacity = _reducedMotion ? 1
                : (float)(_invalidTimeRemaining.TotalSeconds / InvalidFeedbackDuration.TotalSeconds);
            var color = _highContrast ? Color.White : new Color(241, 150, 131);
            DrawTargetOutline(_invalidBounds, color * Math.Min(1, opacity * 2));
        }
    }

    /// <summary>Draws a small, finite gold-and-mint burst without obscuring controls or changing card ownership.</summary>
    private void DrawWinCelebration()
    {
        if (_reducedMotion || !_hasWon || !_winFeedbackShown || _celebrationElapsed >= CelebrationDuration)
        {
            return;
        }

        var elapsedSeconds = (float)_celebrationElapsed.TotalSeconds;
        var opacity = Math.Clamp(3 - elapsedSeconds, 0, 1);
        for (var particleIndex = 0; particleIndex < 28; particleIndex++)
        {
            var horizontalDirection = particleIndex % 2 == 0 ? -1 : 1;
            var horizontalSpeed = 42 + particleIndex * 5;
            var particleX = 550 + horizontalDirection * (80 + horizontalSpeed * elapsedSeconds);
            var particleY = 410 - (145 + particleIndex % 7 * 18) * elapsedSeconds +
                100 * elapsedSeconds * elapsedSeconds;
            if (particleX < 32 || particleX > 1062 || particleY < 82 || particleY > 714)
            {
                continue;
            }

            var particleColor = particleIndex % 3 == 0 ? new Color(112, 220, 182) : new Color(251, 219, 148);
            Fill(new Rectangle((int)particleX, (int)particleY, 4, 6), particleColor * opacity);
        }
    }

    /// <summary>Advances transitions using elapsed time and removes completed visual overrides.</summary>
    /// <param name="elapsed">The elapsed update time, independent of frame rate.</param>
    private void UpdateAnimations(TimeSpan elapsed)
    {
        _selectionElapsed += (float)elapsed.TotalSeconds;
        _animationElapsed += (float)elapsed.TotalSeconds;
        if (_cardTransitions.Count == 0)
        {
            return;
        }

        var complete = true;
        foreach (var transition in _cardTransitions.Values)
        {
            complete &= _animationElapsed >= transition.Delay + MoveDuration;
        }

        if (complete)
        {
            _cardTransitions.Clear();
        }
    }

    /// <summary>Captures board positions, using the floating stack as the source during a drag.</summary>
    /// <returns>A presentation snapshot keyed by card identity.</returns>
    private Dictionary<Card, CardPresentation> CaptureCardPresentation()
    {
        var snapshot = new Dictionary<Card, CardPresentation>(52);
        CapturePile(_stock, new Rectangle(32, 82, CardWidth, CardHeight), snapshot);
        CapturePile(_waste, new Rectangle(128, 82, CardWidth, CardHeight), snapshot);
        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            CapturePile(_foundations[foundationIndex], GetFoundationBounds(foundationIndex), snapshot);
        }

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            for (var cardIndex = 0; cardIndex < _tableau[columnIndex].Count; cardIndex++)
            {
                var card = _tableau[columnIndex][cardIndex];
                snapshot[card] = new CardPresentation(GetTableauCardBounds(columnIndex, cardIndex), card.FaceUp);
            }
        }

        if (_isDragging && _selectedPile is not null)
        {
            var position = _pointerPosition - _dragOffset;
            for (var cardIndex = _selectedCardIndex; cardIndex < _selectedPile.Count; cardIndex++)
            {
                var card = _selectedPile[cardIndex];
                snapshot[card] = new CardPresentation(new Rectangle(position.X, position.Y, CardWidth, CardHeight), true);
                position.Y += TableauStep;
            }
        }

        return snapshot;
    }

    /// <summary>Adds all cards in a non-tableau pile to a presentation snapshot.</summary>
    /// <param name="pile">The pile to capture in bottom-to-top order.</param>
    /// <param name="bounds">The rectangle shared by cards in this pile.</param>
    /// <param name="snapshot">The snapshot to populate.</param>
    private static void CapturePile(List<Card> pile, Rectangle bounds, Dictionary<Card, CardPresentation> snapshot)
    {
        foreach (var card in pile)
        {
            snapshot[card] = new CardPresentation(bounds, card.FaceUp);
        }
    }

    /// <summary>Starts slides and flips for cards whose presentation changed after a logical operation.</summary>
    /// <param name="before">The presentation captured before the operation.</param>
    private void AnimateChanges(Dictionary<Card, CardPresentation> before)
    {
        if (_reducedMotion)
        {
            _cardTransitions.Clear();
            return;
        }

        var wasDragging = _isDragging;
        _isDragging = false;
        var after = CaptureCardPresentation();
        _isDragging = wasDragging;
        _cardTransitions.Clear();
        _animationElapsed = 0;
        foreach (var entry in after)
        {
            if (before.TryGetValue(entry.Key, out var start) && start != entry.Value)
            {
                _cardTransitions.Add(entry.Key, new CardTransition(start, entry.Value, 0));
            }
        }
    }

    /// <summary>Starts a short staggered deal from the stock to the freshly populated tableau.</summary>
    private void AnimateDeal()
    {
        _cardTransitions.Clear();
        _animationElapsed = 0;
        if (_reducedMotion)
        {
            return;
        }

        var dealIndex = 0;
        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            for (var cardIndex = 0; cardIndex < _tableau[columnIndex].Count; cardIndex++)
            {
                var card = _tableau[columnIndex][cardIndex];
                var start = new CardPresentation(new Rectangle(32, 82, CardWidth, CardHeight), false);
                var end = new CardPresentation(GetTableauCardBounds(columnIndex, cardIndex), card.FaceUp);
                _cardTransitions.Add(card, new CardTransition(start, end, dealIndex++ * 0.025f));
            }
        }
    }

    /// <summary>Draws moving cards above the board, compressing their complete artwork horizontally during flips.</summary>
    private void DrawCardTransitions()
    {
        foreach (var entry in _cardTransitions)
        {
            var transition = entry.Value;
            if (_animationElapsed < transition.Delay)
            {
                continue;
            }

            var progress = Math.Clamp((_animationElapsed - transition.Delay) / MoveDuration, 0, 1);
            var easedProgress = progress * progress * (3 - 2 * progress);
            var position = Vector2.Lerp(transition.Start.Bounds.Location.ToVector2(),
                transition.End.Bounds.Location.ToVector2(), easedProgress);
            var bounds = new Rectangle((int)MathF.Round(position.X), (int)MathF.Round(position.Y), CardWidth, CardHeight);
            var isFlipping = transition.Start.FaceUp != transition.End.FaceUp;
            var faceUp = progress < 0.5f ? transition.Start.FaceUp : transition.End.FaceUp;
            if (!isFlipping || progress >= 1)
            {
                DrawCardArtwork(entry.Key, bounds, false, !faceUp);
                continue;
            }

            var widthScale = Math.Max(0.02f, Math.Abs(1 - 2 * progress));
            var centerX = bounds.Center.X;
            _spriteBatch.End();
            BeginBoardDrawing(Matrix.CreateTranslation(-centerX, 0, 0) *
                Matrix.CreateScale(widthScale, 1, 1) * Matrix.CreateTranslation(centerX, 0, 0));
            DrawCardArtwork(entry.Key, bounds, false, !faceUp);
            _spriteBatch.End();
            BeginBoardDrawing();
        }
    }

    /// <summary>The new-game button bounds shared by drawing and pointer handling.</summary>
    private static readonly Rectangle NewGameButtonBounds = new(32, 20, 132, 44);

    /// <summary>The undo button bounds in logical board coordinates.</summary>
    private static readonly Rectangle UndoButtonBounds = new(180, 20, 104, 44);

    /// <summary>The hint button bounds in logical board coordinates.</summary>
    private static readonly Rectangle HintButtonBounds = new(300, 20, 104, 44);

    /// <summary>The toolbar label for starting a new deal.</summary>
    private const string NewGameLabel = "NEW GAME";

    /// <summary>The toolbar label for undoing the last action.</summary>
    private const string UndoLabel = "UNDO";

    /// <summary>The toolbar label for requesting a one-step hint.</summary>
    private const string HintLabel = "HINT";

    /// <summary>The toolbar status label when sound cannot be used.</summary>
    private const string AudioUnavailableLabel = "NA";

    /// <summary>The toolbar status label when sound is enabled.</summary>
    private const string SoundEnabledLabel = "SND";

    /// <summary>The toolbar status label when sound is disabled.</summary>
    private const string SoundDisabledLabel = "OFF";

    /// <summary>Hint text for moving a card to a foundation.</summary>
    private const string HintToFoundationMessage = "HINT  MOVE GOLD CARD TO GOLD FOUNDATION";

    /// <summary>Hint text for moving a card to a tableau column.</summary>
    private const string HintToTableauMessage = "HINT  MOVE GOLD CARD TO GOLD TABLEAU";

    /// <summary>Hint text for drawing from a non-empty stock.</summary>
    private const string HintDrawFromStockMessage = "HINT  CLICK STOCK TO DRAW";

    /// <summary>Hint text for recycling when stock is empty.</summary>
    private const string HintRecycleStockMessage = "HINT  CLICK EMPTY STOCK TO RECYCLE";

    /// <summary>Hint text shown when no immediate suggestion exists.</summary>
    private const string HintNoSuggestionMessage = "HINT  NO SUGGESTED MOVE  TRY UNDO OR NEW GAME";

    /// <summary>The board states saved before successful actions, newest first.</summary>
    private readonly Stack<BoardSnapshot> _undoHistory = new();

    /// <summary>The number of successful actions remaining in the current history.</summary>
    private int _moveCount;

    /// <summary>The elapsed playing time since the first successful action.</summary>
    private TimeSpan _playingTime;

    /// <summary>Whether any successful action has started the timer for this deal.</summary>
    private bool _timerStarted;

    /// <summary>The time remaining before the current hint disappears.</summary>
    private TimeSpan _hintTimeRemaining;

    /// <summary>The highlighted source card or stock slot for the current hint.</summary>
    private Rectangle _hintSource;

    /// <summary>The highlighted destination for the current hint, or an empty rectangle for stock actions.</summary>
    private Rectangle _hintDestination;

    /// <summary>The concise hint message shown in the footer.</summary>
    private string _hintMessage = string.Empty;

    /// <summary>Stores one card reference and its face state independently of subsequent mutations.</summary>
    /// <param name="Card">The original card identity.</param>
    /// <param name="FaceUp">The saved visibility of the card face.</param>
    private readonly record struct SavedCard(Card Card, bool FaceUp);

    /// <summary>Stores every pile's order, face states, and move count before a successful action.</summary>
    /// <param name="Piles">Stock, waste, tableau, and foundation contents in enumeration order.</param>
    /// <param name="MoveCount">The move count before the action.</param>
    private sealed record BoardSnapshot(SavedCard[][] Piles, int MoveCount);

    /// <summary>Enumerates the existing pile objects in a stable order for history capture and restoration.</summary>
    /// <returns>Stock, waste, tableau columns, then foundations.</returns>
    private IEnumerable<List<Card>> EnumeratePiles()
    {
        yield return _stock;
        yield return _waste;
        foreach (var pile in _tableau)
        {
            yield return pile;
        }

        foreach (var pile in _foundations)
        {
            yield return pile;
        }
    }

    /// <summary>Saves a complete board state immediately before a known legal, nonempty action.</summary>
    private void RecordAction()
    {
        var piles = EnumeratePiles()
            .Select(pile => pile.Select(card => new SavedCard(card, card.FaceUp)).ToArray())
            .ToArray();
        _undoHistory.Push(new BoardSnapshot(piles, _moveCount));
        _moveCount++;
        _timerStarted = true;
        ClearHint();
        ResetFeedback();
    }

    /// <summary>Restores the last action, including automatic flips, without rewinding elapsed playing time.</summary>
    private void UndoLastAction()
    {
        if (_isDragging || _cardTransitions.Count > 0 || _undoHistory.Count == 0)
        {
            return;
        }

        var before = CaptureCardPresentation();
        var snapshot = _undoHistory.Pop();
        var pileIndex = 0;
        foreach (var pile in EnumeratePiles())
        {
            pile.Clear();
            foreach (var savedCard in snapshot.Piles[pileIndex++])
            {
                savedCard.Card.FaceUp = savedCard.FaceUp;
                pile.Add(savedCard.Card);
            }
        }

        _moveCount = snapshot.MoveCount;
        _hasWon = _foundations.All(pile => pile.Count == 13);
        ClearSelection();
        ResetPointerGesture();
        ClearHint();
        ResetFeedback();
        AnimateChanges(before);
        PlayFeedbackSound(FeedbackSound.Move);
    }

    /// <summary>Resets history, statistics, and hint feedback for a new deal.</summary>
    private void ResetControls()
    {
        _undoHistory.Clear();
        _moveCount = 0;
        _playingTime = TimeSpan.Zero;
        _timerStarted = false;
        ClearHint();
    }

    /// <summary>Advances the playing timer and hint lifetime independently of frame rate.</summary>
    /// <param name="elapsed">The elapsed update time.</param>
    private void UpdateControls(TimeSpan elapsed)
    {
        if (_timerStarted && !_hasWon)
        {
            _playingTime += elapsed;
        }

        _hintTimeRemaining = elapsed >= _hintTimeRemaining ? TimeSpan.Zero : _hintTimeRemaining - elapsed;
    }

    /// <summary>Handles shortcut edges, allowing sound toggling even during animations or dragging.</summary>
    /// <param name="keyboardState">The current keyboard state.</param>
    private void HandleControlKeys(KeyboardState keyboardState)
    {
        if (IsNewKey(keyboardState, Keys.M))
        {
            ToggleSound();
        }

        if (_cardTransitions.Count > 0 || _isDragging)
        {
            return;
        }

        if (IsNewKey(keyboardState, Keys.U))
        {
            UndoLastAction();
        }
        else if (IsNewKey(keyboardState, Keys.H))
        {
            ShowHint();
        }
    }

    /// <summary>Handles toolbar clicks without allowing them to become board gestures.</summary>
    /// <param name="position">The click position in logical board coordinates.</param>
    /// <returns>Whether a toolbar button occupies the position, including a disabled button.</returns>
    private bool HandleToolbarClick(Point position)
    {
        if (NewGameButtonBounds.Contains(position))
        {
            StartNewGame();
            return true;
        }

        if (UndoButtonBounds.Contains(position))
        {
            UndoLastAction();
            return true;
        }

        if (HintButtonBounds.Contains(position))
        {
            ShowHint();
            return true;
        }

        if (SoundButtonBounds.Contains(position))
        {
            ToggleSound();
            return true;
        }

        if (OptionsButtonBounds.Contains(position))
        {
            ToggleOptions();
            return true;
        }

        return false;
    }

    /// <summary>Draws the toolbar and live statistics, including disabled-state and shortcut labels.</summary>
    private void DrawToolbar()
    {
        var canUseBoard = _cardTransitions.Count == 0 && !_isDragging;
        DrawToolbarButton(NewGameButtonBounds, NewGameLabel, true);
        DrawToolbarButton(UndoButtonBounds, UndoLabel, canUseBoard && _undoHistory.Count > 0);
        DrawToolbarButton(HintButtonBounds, HintLabel, canUseBoard && !_hasWon);
        DrawToolbarButton(SoundButtonBounds,
            _audioUnavailable ? AudioUnavailableLabel : _soundEnabled ? SoundEnabledLabel : SoundDisabledLabel,
            !_audioUnavailable);
        DrawText("SOLITAIRE", 440, 26, 2, new Color(246, 239, 218));
        DrawText(_largeText ? "F1 OPTIONS" : "R NEW GAME  U UNDO  H HINT  M SOUND", 440, 51,
            SmallTextScale, new Color(153, 183, 176));
        DrawText("MOVES", 740, 24, SmallTextScale, new Color(153, 183, 176));
        DrawText(_moveCount.ToString(), 740, 44, 2, new Color(246, 239, 218));
        DrawText("TIME", 850, 24, SmallTextScale, new Color(153, 183, 176));
        var totalSeconds = (long)_playingTime.TotalSeconds;
        DrawText($"{totalSeconds / 60:00}M {totalSeconds % 60:00}S", 850, 44,
            SmallTextScale, new Color(246, 239, 218));
    }

    /// <summary>Draws one themed button with hover feedback only while enabled.</summary>
    /// <param name="bounds">The shared drawing and hit-test bounds.</param>
    /// <param name="label">The button text.</param>
    /// <param name="enabled">Whether the button's action is available.</param>
    private void DrawToolbarButton(Rectangle bounds, string label, bool enabled)
    {
        var isHovered = enabled && bounds.Contains(_pointerPosition);
        var texture = isHovered ? _artwork.ButtonHover : _artwork.Button;
        if (_highContrast)
        {
            Fill(bounds, enabled ? Color.White : new Color(48, 48, 48));
            DrawTargetOutline(bounds, Color.White);
        }
        else
        {
            _spriteBatch.Draw(texture, bounds, enabled ? Color.White : new Color(104, 115, 113));
        }

        var ink = _highContrast ? (enabled ? Color.Black : Color.White)
            : enabled ? new Color(31, 56, 59) : new Color(44, 62, 63);
        DrawText(label, bounds.X + (bounds.Width - label.Length * 12 + 2) / 2, bounds.Y + 15,
            2, ink, preserveColor: true);
    }

    /// <summary>Clears the current hint without modifying selection, history, or board state.</summary>
    private void ClearHint()
    {
        _hintTimeRemaining = TimeSpan.Zero;
        _hintMessage = string.Empty;
        _hintSource = Rectangle.Empty;
        _hintDestination = Rectangle.Empty;
    }

    /// <summary>Finds one useful legal move, or suggests stock use, without changing the player's selection.</summary>
    /// <remarks>
    /// Prefers foundations, then moves that expose hidden tableau cards, then other tableau moves.
    /// Does not recommend moving whole King-led columns to empty columns or reversing foundations.
    /// This is a one-step suggestion, not a solver or a guarantee of progress toward a win.
    /// </remarks>
    private void ShowHint()
    {
        if (_isDragging || _cardTransitions.Count > 0 || _hasWon)
        {
            return;
        }

        _invalidTimeRemaining = TimeSpan.Zero;
        var originalPile = _selectedPile;
        var originalIndex = _selectedCardIndex;
        try
        {
            foreach (var pile in HintSourcePiles())
            {
                if (pile.Count == 0 || !pile[^1].FaceUp)
                {
                    continue;
                }

                _selectedPile = pile;
                _selectedCardIndex = pile.Count - 1;
                for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
                {
                    if (CanMoveToFoundation(foundationIndex))
                    {
                        SetHint(GetHintCardBounds(pile, _selectedCardIndex), GetFoundationBounds(foundationIndex),
                            HintToFoundationMessage);
                        return;
                    }
                }
            }

            for (var priority = 0; priority < 2; priority++)
            {
                foreach (var pile in HintSourcePiles())
                {
                    for (var cardIndex = 0; cardIndex < pile.Count; cardIndex++)
                    {
                        if (!pile[cardIndex].FaceUp || (ReferenceEquals(pile, _waste) && cardIndex != pile.Count - 1))
                        {
                            continue;
                        }

                        var revealsCard = _tableau.Contains(pile) && cardIndex > 0 && !pile[cardIndex - 1].FaceUp;
                        if ((priority == 0) != revealsCard)
                        {
                            continue;
                        }

                        _selectedPile = pile;
                        _selectedCardIndex = cardIndex;
                        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
                        {
                            if (CanMoveToTableau(columnIndex) &&
                                !(_tableau[columnIndex].Count == 0 && _tableau.Contains(pile) && cardIndex == 0))
                            {
                                var destinationIndex = Math.Max(0, _tableau[columnIndex].Count - 1);
                                SetHint(GetHintCardBounds(pile, cardIndex),
                                    GetTableauCardBounds(columnIndex, destinationIndex),
                                    HintToTableauMessage);
                                return;
                            }
                        }
                    }
                }
            }

            if (_stock.Count > 0 || _waste.Count > 0)
            {
                SetHint(new Rectangle(32, 82, CardWidth, CardHeight), Rectangle.Empty,
                    _stock.Count > 0 ? HintDrawFromStockMessage : HintRecycleStockMessage);
            }
            else
            {
                SetHint(Rectangle.Empty, Rectangle.Empty, HintNoSuggestionMessage);
            }
        }
        finally
        {
            _selectedPile = originalPile;
            _selectedCardIndex = originalIndex;
        }
    }

    /// <summary>Enumerates hint source piles without proposing foundation reversals.</summary>
    /// <returns>The waste followed by the seven tableau columns.</returns>
    private IEnumerable<List<Card>> HintSourcePiles()
    {
        yield return _waste;
        foreach (var pile in _tableau)
        {
            yield return pile;
        }
    }

    /// <summary>Gets the exposed region of a hinted card so covered cards remain readable.</summary>
    /// <param name="pile">The waste or a tableau pile.</param>
    /// <param name="cardIndex">The card index within the pile.</param>
    /// <returns>The visible source rectangle in board coordinates.</returns>
    private Rectangle GetHintCardBounds(List<Card> pile, int cardIndex)
    {
        if (ReferenceEquals(pile, _waste))
        {
            return new Rectangle(128, 82, CardWidth, CardHeight);
        }

        var bounds = GetTableauCardBounds(_tableau.IndexOf(pile), cardIndex);
        if (cardIndex < pile.Count - 1)
        {
            bounds.Height = TableauStep;
        }

        return bounds;
    }

    /// <summary>Shows a timed hint without executing the suggested action.</summary>
    /// <param name="source">The source rectangle, or empty for a message-only hint.</param>
    /// <param name="destination">The destination rectangle, or empty for a stock or message-only hint.</param>
    /// <param name="message">The footer instructions.</param>
    private void SetHint(Rectangle source, Rectangle destination, string message)
    {
        _hintSource = source;
        _hintDestination = destination;
        _hintMessage = message;
        _hintTimeRemaining = TimeSpan.FromSeconds(4);
    }

    /// <summary>Draws gold outlines around the suggested source and destination while a hint is active.</summary>
    private void DrawHintFeedback()
    {
        if (_hintTimeRemaining <= TimeSpan.Zero)
        {
            return;
        }

        var color = new Color(251, 219, 148);
        if (_hintSource != Rectangle.Empty)
        {
            DrawTargetOutline(_hintSource, color);
        }

        if (_hintDestination != Rectangle.Empty)
        {
            DrawTargetOutline(_hintDestination, color);
        }
    }

    /// <summary>The minimum pointer movement in logical board units required to start dragging.</summary>
    private const int DragThreshold = 6;

    /// <summary>The maximum interval between two clicks on the same card.</summary>
    private static readonly TimeSpan DoubleClickInterval = TimeSpan.FromMilliseconds(350);

    /// <summary>The current pointer position in logical board coordinates.</summary>
    private Point _pointerPosition;

    /// <summary>The pointer position at the start of the current press.</summary>
    private Point _pressPosition;

    /// <summary>The offset from the pressed card's top-left corner to the pointer.</summary>
    private Point _dragOffset;

    /// <summary>The pile pressed as a potential drag source, or null when no face-up card was pressed.</summary>
    private List<Card>? _pressedPile;

    /// <summary>The index of the pressed card within the potential source pile.</summary>
    private int _pressedCardIndex;

    /// <summary>Whether the left button was down during the previous pointer update.</summary>
    private bool _pointerWasDown;

    /// <summary>Whether the current gesture has crossed the drag threshold.</summary>
    private bool _isDragging;

    /// <summary>Whether a held button must be released after a restart before another gesture can begin.</summary>
    private bool _ignoreUntilRelease;

    /// <summary>The card last clicked without moving it, or null when no double-click is pending.</summary>
    private Card? _lastClickedCard;

    /// <summary>The position of the last click used for double-click detection.</summary>
    private Point _lastClickPosition;

    /// <summary>The time of the last click used for double-click detection.</summary>
    private TimeSpan _lastClickTime;

    /// <summary>Processes press, movement, and release transitions without modifying piles during a drag.</summary>
    /// <param name="position">The current pointer position in logical board coordinates.</param>
    /// <param name="isDown">Whether the left mouse button is down.</param>
    /// <param name="time">The monotonically increasing game time used to recognize double-clicks.</param>
    private void UpdatePointer(Point position, bool isDown, TimeSpan time)
    {
        _pointerPosition = position;
        if (_optionsOpen)
        {
            UpdateOptionsPointer(position, isDown);
            return;
        }

        if (isDown && !_pointerWasDown)
        {
            _keyboardFocusVisible = false;
        }

        if (_cardTransitions.Count > 0)
        {
            if (isDown && !_pointerWasDown && NewGameButtonBounds.Contains(position))
            {
                StartNewGame();
            }
            else if (isDown && !_pointerWasDown && SoundButtonBounds.Contains(position))
            {
                ToggleSound();
            }
            else if (isDown && !_pointerWasDown && OptionsButtonBounds.Contains(position))
            {
                ToggleOptions();
            }

            _pressedPile = null;
            _isDragging = false;
            _lastClickedCard = null;
            _ignoreUntilRelease = isDown;
            _pointerWasDown = isDown;
            return;
        }

        if (_ignoreUntilRelease)
        {
            _ignoreUntilRelease = isDown;
            _pointerWasDown = isDown;
            return;
        }

        if (isDown && !_pointerWasDown)
        {
            _pressPosition = position;
            _pressedPile = null;
            if (TryGetCardAt(position, out var pile, out var cardIndex, out var bounds))
            {
                _pressedPile = pile;
                _pressedCardIndex = cardIndex;
                _dragOffset = position - bounds.Location;
            }
        }

        var movement = position - _pressPosition;
        var movedBeyondThreshold = movement.ToVector2().LengthSquared() >= DragThreshold * DragThreshold;
        if (_pointerWasDown && _pressedPile is not null && !_isDragging && movedBeyondThreshold)
        {
            Select(_pressedPile, _pressedCardIndex);
            ClearHint();
            _isDragging = true;
            _lastClickedCard = null;
        }

        if (!isDown && _pointerWasDown)
        {
            if (_isDragging)
            {
                var before = CaptureCardPresentation();
                DropSelectedCards(position);
                if (_cardTransitions.Count == 0)
                {
                    AnimateChanges(before);
                }

                ClearSelection();
            }
            else if (!movedBeyondThreshold)
            {
                HandleReleasedClick(position, time);
            }
            else
            {
                _lastClickedCard = null;
            }

            _pressedPile = null;
            _isDragging = false;
        }

        _ignoreUntilRelease &= isDown;
        _pointerWasDown = isDown;
    }

    /// <summary>Handles a short click, using card identity, position, and time to recognize double-clicks.</summary>
    /// <param name="position">The released pointer position.</param>
    /// <param name="time">The current game time.</param>
    private void HandleReleasedClick(Point position, TimeSpan time)
    {
        if (TryGetCardAt(position, out var pile, out var cardIndex, out _))
        {
            var card = pile[cardIndex];
            var clickDistance = (position - _lastClickPosition).ToVector2().LengthSquared();
            if (ReferenceEquals(card, _lastClickedCard) && time - _lastClickTime <= DoubleClickInterval &&
                clickDistance < DragThreshold * DragThreshold)
            {
                Select(pile, cardIndex);
                var movedToFoundation = false;
                for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
                {
                    if (TryMoveToFoundation(foundationIndex))
                    {
                        movedToFoundation = true;
                        break;
                    }
                }

                if (!movedToFoundation)
                {
                    ShowInvalidMove(position);
                }

                _lastClickedCard = null;
                return;
            }

            HandleClick(position);
            _lastClickedCard = _selectedPile is not null && ReferenceEquals(_selectedPile, pile) &&
                _selectedCardIndex == cardIndex ? card : null;
        }
        else
        {
            HandleClick(position);
            _lastClickedCard = null;
        }

        _lastClickTime = time;
        _lastClickPosition = position;
    }

    /// <summary>Cancels pending dragging and double-click history when a new deal starts.</summary>
    private void ResetPointerGesture()
    {
        _pressedPile = null;
        _isDragging = false;
        _lastClickedCard = null;
        _ignoreUntilRelease = _pointerWasDown;
    }

    /// <summary>Attempts to drop selected cards only onto an actual foundation or tableau hit area.</summary>
    /// <param name="position">The pointer position at release.</param>
    private void DropSelectedCards(Point position)
    {
        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            if (GetFoundationBounds(foundationIndex).Contains(position) && CanMoveToFoundation(foundationIndex))
            {
                TryMoveToFoundation(foundationIndex);
                return;
            }
        }

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            if (GetTableauBounds(columnIndex).Contains(position) && CanMoveToTableau(columnIndex))
            {
                TryMoveToTableau(columnIndex);
                return;
            }
        }

        ShowInvalidMove(position);
    }

    /// <summary>Checks tableau legality without changing selection or card state.</summary>
    /// <param name="columnIndex">The destination tableau index.</param>
    /// <returns>Whether the selection can move to a distinct tableau pile.</returns>
    private bool CanMoveToTableau(int columnIndex)
    {
        var destination = _tableau[columnIndex];
        if (_selectedPile is null || ReferenceEquals(destination, _selectedPile))
        {
            return false;
        }

        var movingCard = _selectedPile[_selectedCardIndex];
        return MoveRules.CanMoveToTableau(movingCard, destination);
    }

    /// <summary>Checks foundation legality without changing selection or card state.</summary>
    /// <param name="foundationIndex">The destination foundation index.</param>
    /// <returns>Whether a single selected top card can advance a distinct foundation.</returns>
    private bool CanMoveToFoundation(int foundationIndex)
    {
        var destination = _foundations[foundationIndex];
        if (_selectedPile is null || ReferenceEquals(destination, _selectedPile) ||
            _selectedCardIndex != _selectedPile.Count - 1)
        {
            return false;
        }

        var movingCard = _selectedPile[_selectedCardIndex];
        return MoveRules.CanMoveToFoundation(movingCard, destination);
    }

    /// <summary>Gets the complete visible tableau hit area, excluding the footer.</summary>
    /// <param name="columnIndex">The tableau index.</param>
    /// <returns>The union of its cards, or one card-sized slot for an empty column.</returns>
    private Rectangle GetTableauBounds(int columnIndex)
    {
        var pile = _tableau[columnIndex];
        var bottom = pile.Count == 0 ? TableauTop + CardHeight : GetTableauCardBounds(columnIndex, pile.Count - 1).Bottom;
        return new Rectangle(FirstColumnX + columnIndex * ColumnStep, TableauTop,
            CardWidth, Math.Min(bottom, 728) - TableauTop);
    }

    /// <summary>Gets the drawing bounds of a particular tableau card.</summary>
    /// <param name="columnIndex">The tableau index.</param>
    /// <param name="cardIndex">The card index within the column.</param>
    /// <returns>The full card rectangle, including any covered portion.</returns>
    private Rectangle GetTableauCardBounds(int columnIndex, int cardIndex)
    {
        var cardY = TableauTop;
        for (var index = 0; index < cardIndex; index++)
        {
            cardY += _tableau[columnIndex][index].FaceUp ? TableauStep : 22;
        }

        return new Rectangle(FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);
    }

    /// <summary>Finds the visible, face-up card under the pointer, ignoring hidden stock cards.</summary>
    /// <param name="position">The pointer position.</param>
    /// <param name="pile">The containing pile, or null when no selectable card was hit.</param>
    /// <param name="cardIndex">The card index within the containing pile.</param>
    /// <param name="bounds">The card's full drawing bounds.</param>
    /// <returns>Whether a selectable card was hit.</returns>
    private bool TryGetCardAt(Point position, out List<Card> pile, out int cardIndex, out Rectangle bounds)
    {
        pile = _waste;
        cardIndex = pile.Count - 1;
        bounds = new Rectangle(128, 82, CardWidth, CardHeight);
        if (cardIndex >= 0 && bounds.Contains(position))
        {
            return true;
        }

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            pile = _foundations[foundationIndex];
            cardIndex = pile.Count - 1;
            bounds = GetFoundationBounds(foundationIndex);
            if (cardIndex >= 0 && bounds.Contains(position))
            {
                return true;
            }
        }

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            if (!GetTableauBounds(columnIndex).Contains(position))
            {
                continue;
            }

            pile = _tableau[columnIndex];
            cardIndex = GetTableauCardIndex(columnIndex, position.Y);
            if (cardIndex >= 0 && pile[cardIndex].FaceUp)
            {
                bounds = GetTableauCardBounds(columnIndex, cardIndex);
                return true;
            }
        }

        pile = null;
        cardIndex = -1;
        bounds = Rectangle.Empty;
        return false;
    }

    /// <summary>Determines whether a card is currently rendered in the floating drag stack.</summary>
    /// <param name="pile">The card's pile.</param>
    /// <param name="cardIndex">The index of the card.</param>
    /// <returns>Whether source rendering should omit the card.</returns>
    private bool IsDraggedCard(List<Card> pile, int cardIndex) =>
        _isDragging && ReferenceEquals(pile, _selectedPile) && cardIndex >= _selectedCardIndex;

    /// <summary>Draws legal destinations, hover feedback, and the floating stack in that order.</summary>
    private void DrawInteractionFeedback()
    {
        if (_cardTransitions.Count > 0)
        {
            return;
        }

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            if (CanMoveToFoundation(foundationIndex))
            {
                DrawTargetOutline(GetFoundationBounds(foundationIndex), new Color(112, 220, 182));
            }
        }

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            if (CanMoveToTableau(columnIndex))
            {
                var pile = _tableau[columnIndex];
                var bounds = GetTableauCardBounds(columnIndex, Math.Max(0, pile.Count - 1));
                DrawTargetOutline(bounds, new Color(112, 220, 182));
            }
        }

        if (!_isDragging && TryGetCardAt(_pointerPosition, out var hoveredPile, out var hoveredIndex, out var hoveredBounds))
        {
            if (_tableau.Contains(hoveredPile) && hoveredIndex < hoveredPile.Count - 1)
            {
                hoveredBounds.Height = TableauStep;
            }

            DrawTargetOutline(hoveredBounds, new Color(191, 215, 227));
        }

        DrawHintFeedback();

        if (_isDragging && _selectedPile is not null)
        {
            var position = _pointerPosition - _dragOffset;
            for (var cardIndex = _selectedCardIndex; cardIndex < _selectedPile.Count; cardIndex++)
            {
                DrawCard(_selectedPile[cardIndex], new Rectangle(position.X, position.Y, CardWidth, CardHeight),
                    cardIndex == _selectedCardIndex);
                position.Y += TableauStep;
            }
        }
    }

    /// <summary>Draws a thin, high-contrast outline just outside a card or drop slot.</summary>
    /// <param name="bounds">The bounds to outline.</param>
    /// <param name="color">The feedback color.</param>
    private void DrawTargetOutline(Rectangle bounds, Color color)
    {
        bounds.Inflate(2, 2);
        Fill(new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), color);
        Fill(new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), color);
        Fill(new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), color);
        Fill(new Rectangle(bounds.Right - 2, bounds.Y, 2, bounds.Height), color);
    }

    /// <summary>The always-available settings button in logical board coordinates.</summary>
    private static readonly Rectangle OptionsButtonBounds = new(882, 742, 186, 40);

    /// <summary>Whether small labels and card ranks use the larger text layout.</summary>
    private bool _largeText;

    /// <summary>Whether the board uses solid black surfaces and high-contrast text and cards.</summary>
    private bool _highContrast;

    /// <summary>Whether slides, flips, fades, and celebration particles are suppressed.</summary>
    private bool _reducedMotion;

    /// <summary>Whether the modal accessibility settings panel is open.</summary>
    private bool _optionsOpen;

    /// <summary>The focused settings row: larger text, high contrast, reduced motion, or close.</summary>
    private int _optionIndex;

    /// <summary>The focused control or pile: five controls, stock, waste, four foundations, then seven columns.</summary>
    private int _keyboardFocus = -1;

    /// <summary>The focused face-up card index within a tableau column.</summary>
    private int _keyboardCardIndex;

    /// <summary>Whether keyboard focus outlines and descriptive footer text are visible.</summary>
    private bool _keyboardFocusVisible;

    /// <summary>Gets the scale used for small interface labels.</summary>
    private int SmallTextScale => _largeText ? 2 : 1;

    /// <summary>Detects a new keyboard press without repeating held keys.</summary>
    /// <param name="keyboard">The current keyboard state.</param>
    /// <param name="key">The key to inspect.</param>
    /// <returns>Whether the key transitioned from released to pressed.</returns>
    private bool IsNewKey(KeyboardState keyboard, Keys key) =>
        keyboard.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);

    /// <summary>Opens or closes settings while cancelling any pending pointer gesture without moving cards.</summary>
    private void ToggleOptions()
    {
        var before = _isDragging ? CaptureCardPresentation() : null;
        ResetPointerGesture();
        if (before is not null)
        {
            ClearSelection();
            AnimateChanges(before);
        }

        _optionsOpen = !_optionsOpen;
        _optionIndex = 0;
    }

    /// <summary>Handles settings shortcuts before ordinary board controls so the panel cannot activate the board.</summary>
    /// <param name="keyboard">The current keyboard state.</param>
    /// <returns>Whether the keyboard update belongs to settings rather than the board.</returns>
    private bool HandleAccessibilityKeys(KeyboardState keyboard)
    {
        if (IsNewKey(keyboard, Keys.F1))
        {
            ToggleOptions();
            return true;
        }

        if (IsNewKey(keyboard, Keys.F2))
        {
            _largeText = !_largeText;
            return true;
        }

        if (IsNewKey(keyboard, Keys.F3))
        {
            _highContrast = !_highContrast;
            return true;
        }

        if (IsNewKey(keyboard, Keys.F4))
        {
            ToggleReducedMotion();
            return true;
        }

        if (!_optionsOpen)
        {
            return false;
        }

        if (IsNewKey(keyboard, Keys.Tab) || IsNewKey(keyboard, Keys.Down) || IsNewKey(keyboard, Keys.Up))
        {
            var navigateBackward = IsNewKey(keyboard, Keys.Up) || keyboard.IsKeyDown(Keys.LeftShift) ||
                keyboard.IsKeyDown(Keys.RightShift);
            _optionIndex = (_optionIndex + (navigateBackward ? 3 : 1)) % 4;
        }
        else if (IsNewKey(keyboard, Keys.Enter) || IsNewKey(keyboard, Keys.Space))
        {
            ActivateOption(_optionIndex);
        }

        return true;
    }

    /// <summary>Changes reduced motion immediately, settling existing transitions without changing their logical results.</summary>
    private void ToggleReducedMotion()
    {
        _reducedMotion = !_reducedMotion;
        if (_reducedMotion)
        {
            _cardTransitions.Clear();
            _selectionElapsed = 1;
            _celebrationElapsed = CelebrationDuration;
        }
    }

    /// <summary>Activates one settings row without changing history, statistics, or the deal.</summary>
    /// <param name="index">The settings row index.</param>
    private void ActivateOption(int index)
    {
        switch (index)
        {
            case 0:
                _largeText = !_largeText;
                break;
            case 1:
                _highContrast = !_highContrast;
                break;
            case 2:
                ToggleReducedMotion();
                break;
            case 3:
                ToggleOptions();
                break;
        }
    }

    /// <summary>Gets a settings row's shared drawing and hit-test bounds.</summary>
    /// <param name="index">The row index.</param>
    /// <returns>The row bounds in logical board coordinates.</returns>
    private static Rectangle GetOptionBounds(int index) => new(196, 250 + index * 56, 708, 44);

    /// <summary>Consumes settings pointer input, requiring press and release on the same row.</summary>
    /// <param name="position">The logical pointer position.</param>
    /// <param name="isDown">Whether the left button is down.</param>
    private void UpdateOptionsPointer(Point position, bool isDown)
    {
        if (_ignoreUntilRelease)
        {
            _ignoreUntilRelease = isDown;
            _pointerWasDown = isDown;
            return;
        }

        if (isDown && !_pointerWasDown)
        {
            _pressPosition = position;
        }

        if (!isDown && _pointerWasDown)
        {
            for (var index = 0; index < 4; index++)
            {
                var bounds = GetOptionBounds(index);
                if (bounds.Contains(position) && bounds.Contains(_pressPosition))
                {
                    _optionIndex = index;
                    ActivateOption(index);
                    break;
                }
            }
        }

        _ignoreUntilRelease &= isDown;
        _pointerWasDown = isDown;
    }

    /// <summary>Draws the settings panel with large, always-readable labels and explicit on/off states.</summary>
    private void DrawAccessibilityOptions()
    {
        Fill(new Rectangle(0, 0, BoardWidth, BoardHeight), Color.Black * 0.75f);
        Fill(new Rectangle(160, 180, 780, 444), Color.Black);
        DrawTargetOutline(new Rectangle(160, 180, 780, 444), Color.White);
        DrawText("ACCESSIBILITY", 196, 202, 3, Color.White);
        for (var index = 0; index < 4; index++)
        {
            var bounds = GetOptionBounds(index);
            Fill(bounds, new Color(28, 43, 48));
            var label = index switch
            {
                0 => $"F2  LARGER TEXT  {(_largeText ? "ON" : "OFF")}",
                1 => $"F3  HIGH CONTRAST  {(_highContrast ? "ON" : "OFF")}",
                2 => $"F4  REDUCED MOTION  {(_reducedMotion ? "ON" : "OFF")}",
                _ => "F1  CLOSE OPTIONS"
            };
            DrawText(label, bounds.X + 18, bounds.Y + 15, 2, Color.White);
            if (index == _optionIndex)
            {
                DrawFocusCorners(bounds);
            }
        }

        DrawText("TAB OR UP DOWN  CHOOSE   ENTER OR SPACE  TOGGLE", 196, 500, 2, Color.White);
        DrawText("SETTINGS LAST UNTIL THE APPLICATION CLOSES", 196, 530, 2, Color.White);
        DrawText("F1 CLOSES OPTIONS   ESC EXITS THE GAME", 196, 560, 2, Color.White);
        DrawText("BOARD SHORTCUTS ARE DISABLED WHILE OPTIONS ARE OPEN", 196, 590, 2, Color.White);
    }

    /// <summary>Draws an empty pile using either the themed texture or a solid high-contrast surface.</summary>
    /// <param name="bounds">The logical pile bounds.</param>
    private void DrawSlot(Rectangle bounds)
    {
        if (_highContrast)
        {
            Fill(bounds, Color.Black);
            DrawTargetOutline(bounds, Color.White);
        }
        else
        {
            _spriteBatch.Draw(_artwork.Slot, bounds, Color.White);
        }
    }

    /// <summary>Draws footer status and discoverable access to settings without overlapping larger text.</summary>
    private void DrawAccessibleFooter()
    {
        var message = _invalidTimeRemaining > TimeSpan.Zero
            ? "MOVE NOT ALLOWED  TRY ANOTHER TARGET OR USE HINT"
            : _hintTimeRemaining > TimeSpan.Zero ? _hintMessage
            : _keyboardFocusVisible ? GetKeyboardDescription() : "BUILD DOWN IN ALTERNATING COLORS";
        DrawText(message, 32, 741, SmallTextScale, new Color(207, 220, 207));
        var instructions = _keyboardFocusVisible
            ? "TAB FOCUS  ARROWS NAVIGATE  ENTER ACT  F BUILD  BACKSPACE CLEAR"
            : _audioUnavailable ? "SOUND UNAVAILABLE  TAB KEYBOARD PLAY  ESC EXIT"
            : "DRAG OR CLICK TO MOVE  TAB KEYBOARD PLAY  ESC EXIT";
        DrawText(instructions, 32, 767, SmallTextScale, new Color(153, 183, 176));
        DrawToolbarButton(OptionsButtonBounds, "OPTIONS F1", true);
    }

    /// <summary>Processes edge-triggered keyboard navigation without interfering with active pointer gestures.</summary>
    /// <param name="keyboard">The current keyboard state.</param>
    private void HandleBoardKeyboard(KeyboardState keyboard)
    {
        if (_optionsOpen || _isDragging || _pointerWasDown)
        {
            return;
        }

        if (IsNewKey(keyboard, Keys.Tab))
        {
            var backwards = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            var next = !_keyboardFocusVisible ? (backwards ? 17 : 0)
                : (_keyboardFocus + (backwards ? 17 : 1)) % 18;
            FocusKeyboardTarget(next);
            return;
        }

        if (_cardTransitions.Count > 0)
        {
            if (_keyboardFocusVisible && (IsNewKey(keyboard, Keys.Enter) || IsNewKey(keyboard, Keys.Space)))
            {
                ActivateKeyboardControl();
            }

            return;
        }

        if (IsNewKey(keyboard, Keys.Back))
        {
            ClearSelection();
            ClearHint();
            return;
        }

        if (IsNewKey(keyboard, Keys.Left) || IsNewKey(keyboard, Keys.Right))
        {
            var next = !_keyboardFocusVisible || _keyboardFocus < 5 ? 5
                : 5 + (_keyboardFocus - 5 + (IsNewKey(keyboard, Keys.Left) ? 12 : 1)) % 13;
            FocusKeyboardTarget(next);
            return;
        }

        if (IsNewKey(keyboard, Keys.Up) || IsNewKey(keyboard, Keys.Down))
        {
            if (!_keyboardFocusVisible)
            {
                FocusKeyboardTarget(11);
            }
            else if (_keyboardFocus >= 11)
            {
                _keyboardCardIndex += IsNewKey(keyboard, Keys.Up) ? -1 : 1;
                NormalizeKeyboardCard();
            }

            return;
        }

        if (!_keyboardFocusVisible)
        {
            return;
        }

        if (IsNewKey(keyboard, Keys.F))
        {
            MoveKeyboardCardToFoundation();
        }
        else if (IsNewKey(keyboard, Keys.Enter) || IsNewKey(keyboard, Keys.Space))
        {
            ActivateKeyboardTarget();
        }
    }

    /// <summary>Focuses a control or pile, choosing its top card without altering the current selection.</summary>
    /// <param name="index">The target index in keyboard traversal order.</param>
    private void FocusKeyboardTarget(int index)
    {
        _keyboardFocus = index;
        _keyboardFocusVisible = true;
        _keyboardCardIndex = index >= 11 ? Math.Max(0, _tableau[index - 11].Count - 1) : 0;
        NormalizeKeyboardCard();
    }

    /// <summary>Keeps tableau focus on an exposed face-up card after moves, undo, or a new deal.</summary>
    private void NormalizeKeyboardCard()
    {
        if (_keyboardFocus < 11)
        {
            return;
        }

        var pile = _tableau[_keyboardFocus - 11];
        var firstFaceUp = pile.FindIndex(card => card.FaceUp);
        _keyboardCardIndex = pile.Count == 0 ? 0
            : Math.Clamp(_keyboardCardIndex, Math.Max(0, firstFaceUp), pile.Count - 1);
    }

    /// <summary>Gets the pile under keyboard focus, excluding controls and stock.</summary>
    /// <returns>The focused source or destination pile, or null for a control or stock.</returns>
    private List<Card> GetKeyboardPile() => _keyboardFocus switch
    {
        6 => _waste,
        >= 7 and <= 10 => _foundations[_keyboardFocus - 7],
        >= 11 => _tableau[_keyboardFocus - 11],
        _ => null
    };

    /// <summary>Activates a focused toolbar control, retaining its existing availability checks.</summary>
    private void ActivateKeyboardControl()
    {
        switch (_keyboardFocus)
        {
            case 0:
                StartNewGame();
                break;
            case 1:
                UndoLastAction();
                break;
            case 2:
                ShowHint();
                break;
            case 3:
                ToggleSound();
                break;
            case 4:
                ToggleOptions();
                break;
        }
    }

    /// <summary>Selects a focused card stack or moves the existing selection onto the focused legal destination.</summary>
    private void ActivateKeyboardTarget()
    {
        if (_keyboardFocus < 5)
        {
            ActivateKeyboardControl();
            return;
        }

        ClearHint();
        if (_keyboardFocus == 5)
        {
            DrawFromStock();
            return;
        }

        NormalizeKeyboardCard();
        var pile = GetKeyboardPile();
        var cardIndex = _keyboardFocus >= 11 ? _keyboardCardIndex : pile.Count - 1;
        if (_selectedPile is not null && !ReferenceEquals(_selectedPile, pile) && _keyboardFocus != 6)
        {
            var canMove = _keyboardFocus >= 11 ? CanMoveToTableau(_keyboardFocus - 11)
                : CanMoveToFoundation(_keyboardFocus - 7);
            if (canMove)
            {
                MoveSelectedCards(pile);
                NormalizeKeyboardCard();
            }
            else
            {
                ShowInvalidMove(GetKeyboardBounds().Center);
            }

            return;
        }

        if (cardIndex >= 0 && pile.Count > 0 && pile[cardIndex].FaceUp)
        {
            if (ReferenceEquals(_selectedPile, pile) && _selectedCardIndex == cardIndex)
            {
                ClearSelection();
            }
            else
            {
                Select(pile, cardIndex);
            }
        }
        else
        {
            ShowInvalidMove(GetKeyboardBounds().Center);
        }
    }

    /// <summary>Moves the selected stack, or focused card when nothing is selected, to its first legal foundation.</summary>
    private void MoveKeyboardCardToFoundation()
    {
        if (_selectedPile is null)
        {
            NormalizeKeyboardCard();
            var pile = GetKeyboardPile();
            var cardIndex = _keyboardFocus >= 11 ? _keyboardCardIndex : (pile?.Count ?? 0) - 1;
            if (pile is null || cardIndex < 0 || pile.Count == 0 || !pile[cardIndex].FaceUp)
            {
                ShowInvalidMove(GetKeyboardBounds().Center);
                return;
            }

            Select(pile, cardIndex);
        }

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            if (TryMoveToFoundation(foundationIndex))
            {
                NormalizeKeyboardCard();
                return;
            }
        }

        ShowInvalidMove(GetKeyboardBounds().Center);
    }

    /// <summary>Gets the visible bounds of the focused control, slot, or card strip.</summary>
    /// <returns>The focused logical rectangle.</returns>
    private Rectangle GetKeyboardBounds()
    {
        NormalizeKeyboardCard();
        if (_keyboardFocus >= 11)
        {
            var pile = _tableau[_keyboardFocus - 11];
            var bounds = GetTableauCardBounds(_keyboardFocus - 11, _keyboardCardIndex);
            if (_keyboardCardIndex < pile.Count - 1)
            {
                bounds.Height = TableauStep;
            }

            return bounds;
        }

        return _keyboardFocus switch
        {
            0 => NewGameButtonBounds,
            1 => UndoButtonBounds,
            2 => HintButtonBounds,
            3 => SoundButtonBounds,
            4 => OptionsButtonBounds,
            5 => new Rectangle(32, 82, CardWidth, CardHeight),
            6 => new Rectangle(128, 82, CardWidth, CardHeight),
            >= 7 and <= 10 => GetFoundationBounds(_keyboardFocus - 7),
            _ => Rectangle.Empty
        };
    }

    /// <summary>Describes keyboard focus using rank, suit names, and selection state rather than color alone.</summary>
    /// <returns>The concise footer description.</returns>
    private string GetKeyboardDescription()
    {
        NormalizeKeyboardCard();
        var label = _keyboardFocus switch
        {
            0 => "NEW GAME  R",
            1 => _undoHistory.Count == 0 || _cardTransitions.Count > 0 ? "UNDO UNAVAILABLE" : "UNDO  U",
            2 => _hasWon || _cardTransitions.Count > 0 ? "HINT UNAVAILABLE" : "HINT  H",
            3 => _audioUnavailable ? "SOUND UNAVAILABLE" : _soundEnabled ? "SOUND ON  M" : "SOUND OFF  M",
            4 => "OPTIONS  F1",
            5 => _stock.Count > 0 ? $"STOCK  {_stock.Count} CARDS  ENTER DRAW"
                : _waste.Count > 0 ? "STOCK EMPTY  ENTER RECYCLE" : "STOCK AND WASTE EMPTY",
            6 => "WASTE",
            >= 7 and <= 10 => $"FOUNDATION {_keyboardFocus - 6}",
            _ => $"TABLEAU {_keyboardFocus - 10}"
        };
        var pile = GetKeyboardPile();
        if (pile is not null)
        {
            if (pile.Count == 0)
            {
                label += " EMPTY";
            }
            else
            {
                var card = pile[_keyboardFocus >= 11 ? _keyboardCardIndex : pile.Count - 1];
                var rank = GetLongRankLabel(card.Rank);
                label += card.FaceUp ? $"  {rank} OF {card.Suit}" : "  FACE DOWN";
            }
        }

        if (_selectedPile is not null)
        {
            if (_keyboardFocus >= 7 && !ReferenceEquals(_selectedPile, pile))
            {
                var canMove = _keyboardFocus >= 11 ? CanMoveToTableau(_keyboardFocus - 11)
                    : CanMoveToFoundation(_keyboardFocus - 7);
                label += canMove ? "  LEGAL TARGET" : "  INVALID TARGET";
            }

            label += $"  SELECTED {_selectedPile.Count - _selectedCardIndex} CARDS";
        }

        return label;
    }

    /// <summary>Draws a static corner-shaped focus indicator distinct from continuous selection outlines.</summary>
    /// <param name="bounds">The control or exposed card region to mark.</param>
    private void DrawFocusCorners(Rectangle bounds)
    {
        bounds.Inflate(5, 5);
        const int length = 10;
        const int thickness = 3;
        Fill(new Rectangle(bounds.X, bounds.Y, length, thickness), Color.White);
        Fill(new Rectangle(bounds.Right - length, bounds.Y, length, thickness), Color.White);
        Fill(new Rectangle(bounds.X, bounds.Bottom - thickness, length, thickness), Color.White);
        Fill(new Rectangle(bounds.Right - length, bounds.Bottom - thickness, length, thickness), Color.White);
        Fill(new Rectangle(bounds.X, bounds.Y, thickness, length), Color.White);
        Fill(new Rectangle(bounds.Right - thickness, bounds.Y, thickness, length), Color.White);
        Fill(new Rectangle(bounds.X, bounds.Bottom - length, thickness, length), Color.White);
        Fill(new Rectangle(bounds.Right - thickness, bounds.Bottom - length, thickness, length), Color.White);
    }

    /// <summary>Stores the current host-supplied viewport, initially matching the unscaled board.</summary>
    private GameViewport _viewport = new(new Point(BoardWidth, BoardHeight), new Point(BoardWidth, BoardHeight),
        new Point(BoardWidth, BoardHeight));

    /// <summary>Stores the owned rasterizer state that keeps board artwork out of centered window margins.</summary>
    private RasterizerState _boardRasterizerState = null!;

    /// <summary>Applies window or DPI changes without modifying the deal or interrupting card transitions.</summary>
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

    /// <summary>Maps client-space pointer updates to board space before applying gestures or hit tests.</summary>
    /// <param name="clientPosition">The pointer position in window client units.</param>
    /// <param name="isDown">Whether the left button is currently down.</param>
    /// <param name="time">The monotonically increasing game time used for double-clicks.</param>
    private void UpdateClientPointer(Point clientPosition, bool isDown, TimeSpan time)
    {
        UpdatePointer(_viewport.ToLogical(clientPosition), isDown, time);
    }

    /// <summary>Starts a clipped sprite batch, composing local animation transforms before viewport scaling.</summary>
    /// <param name="localTransform">The optional board-space transform used for a card flip.</param>
    private void BeginBoardDrawing(Matrix? localTransform = null)
    {
        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, rasterizerState: _boardRasterizerState,
            transformMatrix: (localTransform ?? Matrix.Identity) * _viewport.Transform);
    }

    /// <summary>Shuffles a standard deck and deals seven tableau columns, leaving the remaining cards in stock.</summary>
    private void StartNewGame()
    {
        _stock.Clear();
        _waste.Clear();
        _tableau.Clear();
        _foundations.Clear();
        ClearSelection();
        ResetPointerGesture();
        ResetControls();
        ResetFeedback();
        _hasWon = false;

        var deck = DealBuilder.CreateShuffledDeck(new Random());
        DealBuilder.DealTableau(deck, _tableau);
        _stock.AddRange(deck);
        DealBuilder.InitializeFoundations(_foundations);
        AnimateDeal();
    }

    /// <summary>Routes a board click to restart, draw, select, or move cards.</summary>
    /// <param name="clickPosition">The mouse position in logical board coordinates.</param>
    private void HandleClick(Point clickPosition)
    {
        if (HandleToolbarClick(clickPosition))
        {
            return;
        }

        ClearHint();
        var stockBounds = new Rectangle(32, 82, CardWidth, CardHeight);

        if (stockBounds.Contains(clickPosition))
        {
            DrawFromStock();

            return;
        }

        var wasteBounds = new Rectangle(128, 82, CardWidth, CardHeight);

        if (wasteBounds.Contains(clickPosition))
        {
            SelectTop(_waste);

            return;
        }

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            if (!GetFoundationBounds(foundationIndex).Contains(clickPosition))
            {
                continue;
            }

            if (_selectedPile is not null)
            {
                if (TryMoveToFoundation(foundationIndex))
                {
                    return;
                }

                if (!ReferenceEquals(_selectedPile, _foundations[foundationIndex]))
                {
                    ShowInvalidMove(clickPosition);
                }
            }

            SelectTop(_foundations[foundationIndex]);

            return;
        }

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            if (!GetTableauBounds(columnIndex).Contains(clickPosition))
            {
                continue;
            }

            if (_selectedPile is not null)
            {
                if (TryMoveToTableau(columnIndex))
                {
                    return;
                }

                ShowInvalidMove(clickPosition);
            }

            var clickedCardIndex = GetTableauCardIndex(columnIndex, clickPosition.Y);
            if (clickedCardIndex >= 0 && _tableau[columnIndex][clickedCardIndex].FaceUp)
            {
                Select(_tableau[columnIndex], clickedCardIndex);
            }
            else
            {
                ClearSelection();
            }

            return;
        }

        ClearSelection();
    }

    /// <summary>Draws one stock card or recycles the waste face down into an empty stock.</summary>
    private void DrawFromStock()
    {
        var before = CaptureCardPresentation();
        ClearSelection();

        if (_stock.Count > 0)
        {
            RecordAction();
            var drawnCard = _stock[^1];
            _stock.RemoveAt(_stock.Count - 1);
            drawnCard.FaceUp = true;
            _waste.Add(drawnCard);
            AnimateChanges(before);
            PlayFeedbackSound(FeedbackSound.Move);

            return;
        }

        if (_waste.Count > 0)
        {
            RecordAction();
            for (var cardIndex = _waste.Count - 1; cardIndex >= 0; cardIndex--)
            {
                var recycledCard = _waste[cardIndex];
                recycledCard.FaceUp = false;
                _stock.Add(recycledCard);
            }

            _waste.Clear();
            AnimateChanges(before);
            PlayFeedbackSound(FeedbackSound.Move);
        }
    }

    /// <summary>Attempts to move the selected cards to a tableau column or deselect their current column.</summary>
    /// <param name="destinationColumnIndex">The zero-based index of the destination tableau column.</param>
    /// <returns>Whether a legal move was made or the current column was deselected.</returns>
    private bool TryMoveToTableau(int destinationColumnIndex)
    {
        if (_selectedPile is null)
        {
            return false;
        }

        var destinationPile = _tableau[destinationColumnIndex];

        if (ReferenceEquals(_selectedPile, destinationPile))
        {
            ClearSelection();

            return true;
        }

        if (!CanMoveToTableau(destinationColumnIndex))
        {
            return false;
        }

        MoveSelectedCards(destinationPile);

        return true;
    }

    /// <summary>Attempts to move a single selected top card to a same-suit, ascending foundation.</summary>
    /// <param name="foundationIndex">The zero-based index of the destination foundation.</param>
    /// <returns>Whether the selected card was moved to the foundation.</returns>
    private bool TryMoveToFoundation(int foundationIndex)
    {
        if (!CanMoveToFoundation(foundationIndex))
        {
            return false;
        }

        MoveSelectedCards(_foundations[foundationIndex]);
        return true;
    }

    /// <summary>Transfers a validated selection, reveals any newly exposed tableau card, and checks for a win.</summary>
    /// <remarks>
    /// Requires a nonempty selection and a distinct destination already validated by the caller.
    /// Card order is preserved and the selection is cleared after the transfer.
    /// </remarks>
    /// <param name="destinationPile">The pile that receives the selected card and all cards above it.</param>
    private void MoveSelectedCards(List<Card> destinationPile)
    {
        var before = CaptureCardPresentation();
        RecordAction();
        var sourcePile = _selectedPile ?? throw new InvalidOperationException("No cards are selected.");
        var selectedCardCount = sourcePile.Count - _selectedCardIndex;

        destinationPile.AddRange(sourcePile.GetRange(_selectedCardIndex, selectedCardCount));
        sourcePile.RemoveRange(_selectedCardIndex, selectedCardCount);

        if (_tableau.Contains(sourcePile) && sourcePile.Count > 0 && !sourcePile[^1].FaceUp)
        {
            sourcePile[^1].FaceUp = true;
        }

        ClearSelection();
        _hasWon = _foundations.All(foundationPile => foundationPile.Count == 13);
        AnimateChanges(before);
        if (!_hasWon)
        {
            PlayFeedbackSound(_foundations.Contains(destinationPile) ? FeedbackSound.Foundation : FeedbackSound.Move);
        }
    }

    /// <summary>Selects the top card of a pile, or clears the selection when the pile is empty.</summary>
    /// <param name="pile">The pile whose top card should be selected.</param>
    private void SelectTop(List<Card> pile)
    {
        if (pile.Count == 0)
        {
            ClearSelection();

            return;
        }

        Select(pile, pile.Count - 1);
    }

    /// <summary>Selects a card and the cards above it as the source for a subsequent move.</summary>
    /// <param name="pile">The pile containing the selected card.</param>
    /// <param name="cardIndex">The zero-based index of the first selected card.</param>
    private void Select(List<Card> pile, int cardIndex)
    {
        if (!ReferenceEquals(_selectedPile, pile) || _selectedCardIndex != cardIndex)
        {
            _selectionElapsed = 0;
        }

        _selectedPile = pile;
        _selectedCardIndex = cardIndex;
    }

    /// <summary>Clears the selected pile and resets its card index.</summary>
    private void ClearSelection()
    {
        _selectedPile = null;
        _selectedCardIndex = 0;
    }

    /// <summary>Determines whether a card is the first card in the current selection.</summary>
    /// <param name="pile">The pile containing the card.</param>
    /// <param name="cardIndex">The zero-based index of the card within the pile.</param>
    /// <returns>Whether the pile and card index match the current selection.</returns>
    private bool IsSelected(List<Card> pile, int cardIndex) =>
        ReferenceEquals(_selectedPile, pile) && _selectedCardIndex == cardIndex;

    /// <summary>Finds the card whose exposed vertical strip contains the mouse position.</summary>
    /// <remarks>
    /// Covered cards expose only their stacking offset; the top card exposes its full height.
    /// The caller checks horizontal bounds and excludes the toolbar and footer.
    /// </remarks>
    /// <param name="columnIndex">The zero-based index of the tableau column.</param>
    /// <param name="mouseY">The mouse's vertical position in logical board coordinates.</param>
    /// <returns>The matching card index, or <c>-1</c> when no exposed strip contains the position.</returns>
    private int GetTableauCardIndex(int columnIndex, int mouseY)
    {
        var cardY = TableauTop;
        var tableauPile = _tableau[columnIndex];

        for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
        {
            var card = tableauPile[cardIndex];
            var exposedHeight = cardIndex == tableauPile.Count - 1
                ? CardHeight
                : card.FaceUp ? TableauStep : 22;
            var cardBottom = cardY + exposedHeight;

            if (mouseY >= cardY && mouseY < cardBottom)
            {
                return cardIndex;
            }

            cardY += card.FaceUp ? TableauStep : 22;
        }

        return -1;
    }

    /// <summary>Calculates a foundation slot's bounds in logical board coordinates.</summary>
    /// <param name="foundationIndex">The zero-based index of the foundation slot.</param>
    /// <returns>The rectangle used to draw and hit-test the foundation.</returns>
    private Rectangle GetFoundationBounds(int foundationIndex) =>
        new(BoardWidth - 32 - (4 - foundationIndex) * (CardWidth + CardGap), 82, CardWidth, CardHeight);

    /// <summary>Draws a pile's top non-dragged card, or an outlined slot when no card remains visible.</summary>
    /// <param name="pile">The pile to draw.</param>
    /// <param name="bounds">The card or slot bounds in logical board coordinates.</param>
    /// <param name="faceDown">Whether to show the card back regardless of its face-up state.</param>
    private void DrawPile(List<Card> pile, Rectangle bounds, bool faceDown = false)
    {
        var visibleCardIndex = pile.Count - 1;
        if (visibleCardIndex >= 0 && IsDraggedCard(pile, visibleCardIndex))
        {
            visibleCardIndex = _selectedCardIndex - 1;
        }

        while (visibleCardIndex >= 0 && _cardTransitions.ContainsKey(pile[visibleCardIndex]))
        {
            visibleCardIndex--;
        }

        if (visibleCardIndex >= 0)
        {
            DrawCard(pile[visibleCardIndex], bounds, IsSelected(pile, visibleCardIndex), faceDown);
        }
        else
        {
            DrawSlot(bounds);
        }
    }

    /// <summary>Draws a rounded card, soft shadow, suit artwork, and optional selection highlight.</summary>
    /// <param name="card">The card whose rank, suit, and visibility determine its appearance.</param>
    /// <param name="bounds">The card bounds in logical board coordinates.</param>
    /// <param name="isSelected">Whether the face-up card should receive a selection border.</param>
    /// <param name="faceDown">Whether to show the card back regardless of its face-up state.</param>
    private void DrawCard(Card card, Rectangle bounds, bool isSelected, bool faceDown = false)
    {
        if (!_cardTransitions.ContainsKey(card))
        {
            DrawCardArtwork(card, bounds, isSelected, faceDown || !card.FaceUp);
        }
    }

    /// <summary>Draws a complete card using an explicit presentation face state rather than its logical state.</summary>
    /// <param name="card">The card whose rank and suit are drawn.</param>
    /// <param name="bounds">The card rectangle in board coordinates.</param>
    /// <param name="isSelected">Whether to fade in the selection outline.</param>
    /// <param name="faceDown">Whether to draw the back instead of the face.</param>
    private void DrawCardArtwork(Card card, Rectangle bounds, bool isSelected, bool faceDown)
    {
        var shadowBounds = new Rectangle(bounds.X - 6, bounds.Y - 4, bounds.Width + 16, bounds.Height + 16);

        _spriteBatch.Draw(_artwork.CardShadow, shadowBounds, Color.White);

        if (isSelected && !faceDown)
        {
            var highlightOpacity = _reducedMotion ? 1 : Math.Clamp(_selectionElapsed / 0.12f, 0, 1);
            _spriteBatch.Draw(_artwork.Selection,
                new Rectangle(bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6),
                Color.White * highlightOpacity);
        }

        if (_highContrast)
        {
            Fill(bounds, faceDown ? Color.Black : Color.White);
            DrawTargetOutline(bounds, isSelected && !faceDown ? Color.Yellow : Color.White);
            if (faceDown)
            {
                DrawText("BACK", bounds.X + 16, bounds.Y + 46, 2, Color.White);
            }
        }
        else
        {
            _spriteBatch.Draw(faceDown ? _artwork.CardBack : _artwork.CardFace, bounds, Color.White);
        }

        if (faceDown)
        {
            return;
        }

        var suitColor = _highContrast ? (card.IsRed ? new Color(145, 0, 20) : Color.Black)
            : card.IsRed ? new Color(180, 51, 65) : new Color(36, 57, 66);
        var rankLabel = GetShortRankLabel(card.Rank);

        var suitTexture = _artwork.Suits[(int)card.Suit];

        var rankScale = _largeText ? 3 : 2;
        DrawText(rankLabel, bounds.X + 8, bounds.Y + (_largeText ? 5 : 8), rankScale, suitColor, preserveColor: true);
        _spriteBatch.Draw(suitTexture, new Rectangle(bounds.Right - 26, bounds.Y + 5, 20, 20), suitColor);
        _spriteBatch.Draw(suitTexture, new Rectangle(bounds.X + 19, bounds.Y + 35, 40, 40), suitColor);
        _spriteBatch.Draw(suitTexture, new Rectangle(bounds.X + 7, bounds.Bottom - 25, 18, 18), suitColor);
        DrawText(rankLabel, bounds.Right - (rankLabel.Length * 6 * rankScale) - 7,
            bounds.Bottom - (7 * rankScale) - 8, rankScale, suitColor, preserveColor: true);
    }

    /// <summary>Gets the short rank text used for card faces.</summary>
    /// <param name="rank">The card rank from one (ace) through thirteen (king).</param>
    /// <returns>The short rank label.</returns>
    private static string GetShortRankLabel(int rank) => rank switch
    {
        1 => "A",
        11 => "J",
        12 => "Q",
        13 => "K",
        _ => rank.ToString()
    };

    /// <summary>Gets the spoken-style rank text used by keyboard descriptions.</summary>
    /// <param name="rank">The card rank from one (ace) through thirteen (king).</param>
    /// <returns>The long rank label.</returns>
    private static string GetLongRankLabel(int rank) => rank switch
    {
        1 => "ACE",
        11 => "JACK",
        12 => "QUEEN",
        13 => "KING",
        _ => rank.ToString()
    };

    /// <summary>Fills a rectangle using the module's single-pixel texture.</summary>
    /// <param name="bounds">The rectangle to fill in logical board coordinates.</param>
    /// <param name="color">The fill color.</param>
    private void Fill(Rectangle bounds, Color color) => _spriteBatch.Draw(_pixelTexture, bounds, color);

    /// <summary>Builds a padded atlas of the existing five-by-seven glyphs for smoothly scaled lettering.</summary>
    /// <param name="graphicsDevice">The borrowed device that creates the owned texture.</param>
    /// <returns>The glyph atlas, which the module must dispose.</returns>
    private static Texture2D CreateGlyphTexture(GraphicsDevice graphicsDevice)
    {
        const int atlasWidth = 37 * 7;
        var pixels = new Color[atlasWidth * 9];
        for (var glyphIndex = 0; glyphIndex < 37; glyphIndex++)
        {
            for (var glyphRow = 0; glyphRow < 7; glyphRow++)
            {
                var rowBits = glyphIndex == 36
                    ? (glyphRow < 5 || glyphRow == 6 ? 4 : 0)
                    : Glyphs[glyphIndex, glyphRow];
                for (var glyphColumn = 0; glyphColumn < 5; glyphColumn++)
                {
                    if ((rowBits & (1 << (4 - glyphColumn))) != 0)
                    {
                        pixels[(glyphRow + 1) * atlasWidth + glyphIndex * 7 + glyphColumn + 1] = Color.White;
                    }
                }
            }
        }

        var texture = new Texture2D(graphicsDevice, atlasWidth, 9);
        texture.SetData(pixels);
        return texture;
    }

    /// <summary>Draws uppercase bitmap text, leaving blank character widths for unsupported characters.</summary>
    /// <param name="text">The text to draw; letters, digits, spaces, and exclamation marks are supported.</param>
    /// <param name="textX">The starting horizontal position in logical board coordinates.</param>
    /// <param name="textY">The starting vertical position in logical board coordinates.</param>
    /// <param name="scale">The size of each glyph pixel in logical board units.</param>
    /// <param name="color">The normal-theme text color.</param>
    /// <param name="preserveColor">Whether to retain contrasting card or button ink in high-contrast mode.</param>
    private void DrawText(string text, int textX, int textY, int scale, Color color, bool preserveColor = false)
    {
        if (_highContrast && !preserveColor)
        {
            color = Color.White;
        }

        foreach (var character in text.ToUpperInvariant())
        {
            if (character == ' ')
            {
                textX += 4 * scale;
                continue;
            }

            var glyphIndex = character switch
            {
                >= 'A' and <= 'Z' => character - 'A',
                >= '0' and <= '9' => character - '0' + 26,
                '!' => 36,
                _ => -1
            };

            if (glyphIndex >= 0)
            {
                _spriteBatch.Draw(_glyphTexture, new Rectangle(textX, textY, 5 * scale, 7 * scale),
                    new Rectangle(glyphIndex * 7 + 1, 1, 5, 7), color);
            }

            textX += 6 * scale;
        }
    }

    /// <summary>Builds shuffled deals and initializes board piles for a new game.</summary>
    private static class DealBuilder
    {
        /// <summary>Creates a shuffled standard deck using the provided random source.</summary>
        /// <param name="random">The random source used to shuffle cards.</param>
        /// <returns>A shuffled deck ordered from first draw at the end of the list.</returns>
        public static List<Card> CreateShuffledDeck(Random random)
        {
            var deck = CreateOrderedDeck();
            Shuffle(deck, random);
            return deck;
        }

        /// <summary>Deals seven tableau columns from the end of the deck and removes dealt cards from it.</summary>
        /// <param name="deck">The deck to deal from.</param>
        /// <param name="tableau">The tableau collection to populate.</param>
        public static void DealTableau(List<Card> deck, List<List<Card>> tableau)
        {
            for (var columnIndex = 0; columnIndex < 7; columnIndex++)
            {
                var tableauPile = new List<Card>();
                for (var cardIndex = 0; cardIndex <= columnIndex; cardIndex++)
                {
                    var card = deck[^1];
                    deck.RemoveAt(deck.Count - 1);
                    card.FaceUp = cardIndex == columnIndex;
                    tableauPile.Add(card);
                }

                tableau.Add(tableauPile);
            }
        }

        /// <summary>Initializes the four empty foundations for a new deal.</summary>
        /// <param name="foundations">The foundations collection to populate.</param>
        public static void InitializeFoundations(List<List<Card>> foundations)
        {
            for (var foundationIndex = 0; foundationIndex < 4; foundationIndex++)
            {
                foundations.Add(new List<Card>());
            }
        }

        /// <summary>Creates an ordered standard deck from Ace through King in each suit.</summary>
        /// <returns>The ordered deck.</returns>
        private static List<Card> CreateOrderedDeck()
        {
            var deck = new List<Card>(52);
            foreach (var suit in Enum.GetValues<Suit>())
            {
                for (var rank = 1; rank <= 13; rank++)
                {
                    deck.Add(new Card(rank, suit));
                }
            }

            return deck;
        }

        /// <summary>Shuffles the deck in place using the Fisher-Yates algorithm.</summary>
        /// <param name="deck">The deck to shuffle.</param>
        /// <param name="random">The random source used to select swap positions.</param>
        private static void Shuffle(List<Card> deck, Random random)
        {
            for (var cardIndex = deck.Count - 1; cardIndex > 0; cardIndex--)
            {
                var swapIndex = random.Next(cardIndex + 1);
                (deck[cardIndex], deck[swapIndex]) = (deck[swapIndex], deck[cardIndex]);
            }
        }
    }

    /// <summary>Evaluates legal move targets for selected cards without mutating board state.</summary>
    private static class MoveRules
    {
        /// <summary>Checks whether a card can be placed onto a tableau column.</summary>
        /// <param name="movingCard">The selected card that would be placed.</param>
        /// <param name="destination">The destination tableau pile.</param>
        /// <returns><see langword="true"/> when the move is legal; otherwise, <see langword="false"/>.</returns>
        public static bool CanMoveToTableau(Card movingCard, List<Card> destination)
        {
            if (destination.Count == 0)
            {
                return movingCard.Rank == 13;
            }

            var destinationTop = destination[^1];
            return destinationTop.FaceUp && destinationTop.Rank == movingCard.Rank + 1 &&
                destinationTop.IsRed != movingCard.IsRed;
        }

        /// <summary>Checks whether a card can be placed onto a foundation pile.</summary>
        /// <param name="movingCard">The selected card that would be placed.</param>
        /// <param name="destination">The destination foundation pile.</param>
        /// <returns><see langword="true"/> when the move is legal; otherwise, <see langword="false"/>.</returns>
        public static bool CanMoveToFoundation(Card movingCard, List<Card> destination)
        {
            if (destination.Count == 0)
            {
                return movingCard.Rank == 1;
            }

            var destinationTop = destination[^1];
            return destinationTop.Suit == movingCard.Suit && destinationTop.Rank + 1 == movingCard.Rank;
        }
    }

    /// <summary>Identifies the four suits in a standard playing-card deck.</summary>
    private enum Suit
    {
        /// <summary>The black clubs suit.</summary>
        Clubs,

        /// <summary>The red diamonds suit.</summary>
        Diamonds,

        /// <summary>The red hearts suit.</summary>
        Hearts,

        /// <summary>The black spades suit.</summary>
        Spades
    }

    /// <summary>Stores a playing card's identity and face-up state.</summary>
    /// <param name="rank">The card rank, using 1 for Ace through 13 for King.</param>
    /// <param name="suit">The card's suit.</param>
    private sealed class Card(int rank, Suit suit)
    {
        /// <summary>Gets the card rank, using 1 for Ace through 13 for King.</summary>
        public int Rank { get; } = rank;

        /// <summary>Gets the card's suit.</summary>
        public Suit Suit { get; } = suit;

        /// <summary>Gets or sets whether the card's face is visible.</summary>
        public bool FaceUp { get; set; }

        /// <summary>Gets whether the card belongs to a red suit.</summary>
        public bool IsRed => Suit is Suit.Diamonds or Suit.Hearts;
    }
}