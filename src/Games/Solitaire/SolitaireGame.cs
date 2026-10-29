// 261029_code
// 261029_documentation

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cheron.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cheron.Games.Solitaire.FreeCell;
using Cheron.Games.Solitaire.Klondike;
using static Cheron.Games.Solitaire.SolitaireAccessibilityOptions;
using static Cheron.Games.Solitaire.SolitaireAudioConstants;
using static Cheron.Games.Solitaire.SolitaireCelebrationConstants;
using static Cheron.Games.Solitaire.SolitaireColors;
using static Cheron.Games.Solitaire.SolitaireInteractionConstants;
using static Cheron.Games.Solitaire.SolitaireLayout;
using static Cheron.Games.Solitaire.SolitaireOptionsLayout;
using static Cheron.Games.Solitaire.SolitaireTextConstants;
using static Cheron.Games.Solitaire.SolitaireTimingConstants;

namespace Cheron.Games.Solitaire;

/// <summary>Implements shared Solitaire state, interaction, and rendering orchestration across Klondike and FreeCell.</summary>
internal sealed partial class SolitaireGame : IGameModule
{
    /// <summary>The face-down draw pile, with the next card at the end of the list.</summary>
    private readonly List<Card> _stock = new();

    /// <summary>The face-up drawn cards, with the available top card at the end of the list.</summary>
    private readonly List<Card> _waste = new();

    /// <summary>The seven tableau columns, each ordered from its bottom card to its exposed top card.</summary>
    private readonly List<List<Card>> _tableau = new();

    /// <summary>The four foundation piles, each built from Ace to King in a suit determined by its first card.</summary>
    private readonly List<List<Card>> _foundations = new();

    /// <summary>The four FreeCell holding cells, each containing at most one card.</summary>
    private readonly List<List<Card>> _freeCells = new();

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

    /// <summary>The currently active solitaire variant.</summary>
    private SolitaireVariant _variant = SolitaireVariant.Klondike;

    /// <summary>The variant-behavior strategy map used to reduce shared branching.</summary>
    private readonly Dictionary<SolitaireVariant, ISolitaireVariantBehavior> _variantBehaviors = new();

    /// <summary>The variant currently highlighted by the new-game picker prompt.</summary>
    private SolitaireVariant _newGamePromptVariant = SolitaireVariant.Klondike;

    /// <summary>Whether the new-game variant picker prompt is currently visible.</summary>
    private bool _newGamePromptOpen;

    /// <summary>The renderer responsible for drawing the board and UI.</summary>
    private readonly SolitaireRenderer _renderer;

    /// <summary>The interaction controller responsible for keyboard and pointer orchestration.</summary>
    private readonly SolitaireInteractionController _interactionController;

    /// <summary>Initializes rendering and interaction delegation state for this game module.</summary>
    public SolitaireGame()
    {
        _variantBehaviors[SolitaireVariant.Klondike] = new KlondikeVariantBehavior();
        _variantBehaviors[SolitaireVariant.FreeCell] = new FreeCellVariantBehavior();
        _renderer = new SolitaireRenderer(this);
        _interactionController = new SolitaireInteractionController(this);
    }

    /// <summary>Whether the new-game variant picker prompt is currently visible.</summary>
    private bool IsNewGamePromptOpen => _newGamePromptOpen;

    /// <summary>Gets the strategy object for the active variant.</summary>
    private ISolitaireVariantBehavior ActiveVariantBehavior => _variantBehaviors[_variant];

    /// <summary>Gets the Solitaire window title.</summary>
    public string Title => WindowTitle;

    /// <summary>Gets the logical card-table size, also used to request the initial window size.</summary>
    public Point PreferredWindowSize => new(BoardWidth, BoardHeight);

    /// <summary>Deals a shuffled game before graphics resources are loaded.</summary>
    public void Initialize()
    {
        StartNewGame();
    }

    /// <summary>Opens the new-game variant picker prompt with the current variant preselected.</summary>
    private void OpenNewGamePrompt()
    {
        _newGamePromptVariant = _variant;
        _newGamePromptOpen = true;
    }

    /// <summary>Applies the currently highlighted new-game prompt selection and starts a deal.</summary>
    private void ConfirmNewGamePromptSelection()
    {
        _variant = _newGamePromptVariant;
        _newGamePromptOpen = false;
        StartNewGame();
    }

    /// <summary>Applies a specific prompt variant and starts a deal immediately.</summary>
    /// <param name="variant">The variant to start.</param>
    private void StartNewGameFromPromptVariant(SolitaireVariant variant)
    {
        _newGamePromptVariant = variant;
        ConfirmNewGamePromptSelection();
    }

    /// <summary>Cancels the current new-game prompt without changing the active deal.</summary>
    private void CancelNewGamePrompt()
    {
        _newGamePromptOpen = false;
    }

    /// <summary>Opens the in-game Help overlay and resets its scroll position.</summary>
    private void OpenHelpOverlay()
    {
        _helpOpen = true;
        _helpScrollOffset = 0;
        _optionsOpen = false;
        _newGamePromptOpen = false;
        _smokePassOverlayOpen = false;
    }

    /// <summary>Closes the in-game Help overlay without changing board state.</summary>
    private void CloseHelpOverlay()
    {
        _helpOpen = false;
    }

    /// <summary>Scrolls the Help overlay content by one or more line steps.</summary>
    /// <param name="direction">Positive values scroll down; negative values scroll up.</param>
    private void ScrollHelpOverlay(int direction)
    {
        if (!_helpOpen || direction == 0)
        {
            return;
        }

        var lineStep = _largeText ? 34 : 26;
        _helpScrollOffset = Math.Clamp(_helpScrollOffset + direction * lineStep, 0, GetHelpMaxScrollOffset());
    }

    /// <summary>Gets the variant-specific help subtitle shown in the Help overlay.</summary>
    private string GetHelpSubtitle() => _variant.IsFreeCell() ? HelpOverlayFreeCellSubtitle : HelpOverlayKlondikeSubtitle;

    /// <summary>Gets variant-specific gameplay instruction lines for the Help overlay.</summary>
    private IReadOnlyList<string> GetHelpLines() => _variant.IsFreeCell() ? FreeCellHelpLines : KlondikeHelpLines;

    /// <summary>Calculates the maximum vertical scroll offset for Help overlay content.</summary>
    private int GetHelpMaxScrollOffset()
    {
        var lineHeight = _largeText ? 34 : 26;
        var contentHeight = GetHelpContentBounds().Height;
        var totalHeight = GetHelpLines().Count * lineHeight;
        return Math.Max(0, totalHeight - contentHeight + 10);
    }

    /// <summary>Gets whether the new-game prompt is open for tests.</summary>
    internal bool TestIsNewGamePromptOpen() => _newGamePromptOpen;

    /// <summary>Gets whether the in-app manual smoke-pass overlay is open for tests.</summary>
    internal bool TestIsSmokePassOverlayOpen() => _smokePassOverlayOpen;

    /// <summary>Gets whether the options panel is open for tests.</summary>
    internal bool TestIsOptionsOpen() => _optionsOpen;

    /// <summary>Gets whether the Help overlay is open for tests.</summary>
    internal bool TestIsHelpOverlayOpen() => _helpOpen;

    /// <summary>Gets whether session-local sound is currently enabled for tests.</summary>
    internal bool TestIsSoundEnabled() => _soundEnabled;

    /// <summary>Sets the audio-unavailable state for tests.</summary>
    internal void TestSetAudioUnavailable(bool unavailable)
    {
        _audioUnavailable = unavailable;
    }

    /// <summary>Gets the current Help overlay scroll offset for tests.</summary>
    internal int TestGetHelpScrollOffset() => _helpScrollOffset;

    /// <summary>Gets the active variant for tests.</summary>
    internal SolitaireVariant TestGetVariant() => _variant;

    /// <summary>Gets the prompt-selected variant for tests.</summary>
    internal SolitaireVariant TestGetNewGamePromptVariant() => _newGamePromptVariant;

    /// <summary>Sets the active variant for tests.</summary>
    internal void TestSetVariant(SolitaireVariant variant)
    {
        _variant = variant;
    }

    /// <summary>Sets the prompt-selected variant for tests.</summary>
    internal void TestSetPromptVariant(SolitaireVariant variant)
    {
        _newGamePromptVariant = variant;
    }

    /// <summary>Sets prompt visibility for tests.</summary>
    internal void TestSetNewGamePromptOpen(bool isOpen)
    {
        _newGamePromptOpen = isOpen;
    }

    /// <summary>Opens the prompt for tests.</summary>
    internal void TestOpenNewGamePrompt()
    {
        OpenNewGamePrompt();
    }

    /// <summary>Opens the Help overlay for tests.</summary>
    internal void TestOpenHelpOverlay()
    {
        OpenHelpOverlay();
    }

    /// <summary>Closes the Help overlay for tests.</summary>
    internal void TestCloseHelpOverlay()
    {
        CloseHelpOverlay();
    }

    /// <summary>Scrolls Help overlay content for tests.</summary>
    internal void TestScrollHelpOverlay(int direction)
    {
        ScrollHelpOverlay(direction);
    }

    /// <summary>Gets the active Help subtitle for tests.</summary>
    internal string TestGetHelpSubtitle() => GetHelpSubtitle();

    /// <summary>Gets active Help lines for tests.</summary>
    internal string[] TestGetHelpLines() => GetHelpLines().ToArray();

    /// <summary>Toggles prompt variant for tests.</summary>
    internal void TestTogglePromptVariant()
    {
        _newGamePromptVariant = _newGamePromptVariant.ToggleVariant();
    }

    /// <summary>Confirms the prompt selection for tests.</summary>
    internal void TestConfirmNewGamePromptSelection()
    {
        ConfirmNewGamePromptSelection();
    }

    /// <summary>Cancels the prompt selection for tests.</summary>
    internal void TestCancelNewGamePromptSelection()
    {
        CancelNewGamePrompt();
    }

    /// <summary>Processes one accessibility keyboard frame for tests and advances key-edge state.</summary>
    internal bool TestHandleAccessibilityKeys(KeyboardState keyboard)
    {
        var handled = HandleAccessibilityKeys(keyboard);
        _previousKeyboardState = keyboard;
        return handled;
    }

    /// <summary>Processes one keyboard frame through normal update shortcut routing for tests.</summary>
    internal void TestHandleKeyboardFrame(KeyboardState keyboard)
    {
        if (!HandleAccessibilityKeys(keyboard))
        {
            if (IsNewKey(keyboard, Keys.N))
            {
                OpenNewGamePrompt();
            }
            else
            {
                HandleControlKeys(keyboard);
                HandleBoardKeyboard(keyboard);
            }
        }

        _previousKeyboardState = keyboard;
    }

    /// <summary>Resets keyboard edge-detection state for tests.</summary>
    internal void TestResetPreviousKeyboardState()
    {
        _previousKeyboardState = default;
    }

    /// <summary>Processes one pointer update for tests.</summary>
    internal void TestUpdatePointer(Point position, bool isDown, TimeSpan time)
    {
        UpdatePointer(position, isDown, time);
    }

    /// <summary>Sets keyboard focus target index for tests.</summary>
    internal void TestFocusKeyboardTarget(int index)
    {
        FocusKeyboardTarget(index);
    }

    /// <summary>Gets focused keyboard bounds for tests.</summary>
    internal Rectangle TestGetKeyboardBounds()
    {
        return GetKeyboardBounds();
    }

    /// <summary>Gets focused keyboard description for tests.</summary>
    internal string TestGetKeyboardDescription()
    {
        return GetKeyboardDescription();
    }

    /// <summary>Starts a new game for tests using the currently selected variant.</summary>
    internal void TestStartNewGame()
    {
        StartNewGame();
    }

    /// <summary>Gets tableau column card counts for tests.</summary>
    internal int[] TestGetTableauCounts()
    {
        return _tableau.Select(pile => pile.Count).ToArray();
    }

    /// <summary>Gets tableau column face-down card counts for tests.</summary>
    internal int[] TestGetTableauFaceDownCounts()
    {
        return _tableau.Select(pile => pile.Count(card => !card.FaceUp)).ToArray();
    }

    /// <summary>Gets stock count for tests.</summary>
    internal int TestGetStockCount() => _stock.Count;

    /// <summary>Gets FreeCell slot counts for tests.</summary>
    internal int[] TestGetFreeCellCounts()
    {
        return _freeCells.Select(cell => cell.Count).ToArray();
    }

    /// <summary>Gets foundation pile counts for tests.</summary>
    internal int[] TestGetFoundationCounts()
    {
        return _foundations.Select(pile => pile.Count).ToArray();
    }

    /// <summary>Gets one tableau card bounds for tests.</summary>
    internal Rectangle TestGetTableauCardBounds(int columnIndex, int cardIndex)
    {
        return GetTableauCardBounds(columnIndex, cardIndex);
    }

    /// <summary>Gets the number of active card transitions for tests.</summary>
    internal int TestGetCardTransitionCount() => _cardTransitions.Count;

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
        _glyphTexture = SolitaireBitmapFont.CreateGlyphTexture(graphicsDevice);
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
            if (IsNewKey(keyboardState, Keys.N))
            {
                OpenNewGamePrompt();
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
        _renderer.Draw(gameTime, graphicsDevice);
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

    /// <summary>The module-owned sounds, ordered by <see cref="FeedbackSound"/>.</summary>
    private readonly List<SoundEffect> _feedbackSounds = new();

    /// <summary>The module-owned playback instances, stopped immediately when muted.</summary>
    private readonly List<SoundEffectInstance> _feedbackVoices = new();

    /// <summary>The active card transitions, ordered from underlying cards to overlying cards.</summary>
    private readonly Dictionary<Card, CardTransition> _cardTransitions = new();

    /// <summary>Variant-specific Help lines for Klondike gameplay.</summary>
    private static readonly string[] KlondikeHelpLines =
    [
        HelpKlondikeLine1,
        HelpKlondikeLine2,
        HelpKlondikeLine3,
        HelpKlondikeLine4,
        HelpKlondikeLine5,
        HelpKlondikeLine6,
        HelpKlondikeLine7,
        HelpKlondikeLine8
    ];

    /// <summary>Variant-specific Help lines for FreeCell gameplay.</summary>
    private static readonly string[] FreeCellHelpLines =
    [
        HelpFreeCellLine1,
        HelpFreeCellLine2,
        HelpFreeCellLine3,
        HelpFreeCellLine4,
        HelpFreeCellLine5,
        HelpFreeCellLine6,
        HelpFreeCellLine7,
        HelpFreeCellLine8
    ];

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

    /// <summary>Creates original, short PCM sounds and playback instances without external assets.</summary>
    private void LoadFeedbackSounds()
    {
        try
        {
            AddFeedbackSound(MoveFrequencies, MoveNoteSeconds);
            AddFeedbackSound(FoundationFrequencies, FoundationNoteSeconds);
            AddFeedbackSound(InvalidFrequencies, InvalidNoteSeconds);
            AddFeedbackSound(WinFrequencies, WinNoteSeconds);
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
        CapturePile(_stock, GetStockBounds(), snapshot);
        CapturePile(_waste, GetWasteBounds(), snapshot);
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
                var start = new CardPresentation(GetStockBounds(), false);
                var end = new CardPresentation(GetTableauCardBounds(columnIndex, cardIndex), card.FaceUp);
                _cardTransitions.Add(card, new CardTransition(start, end, dealIndex++ * 0.025f));
            }
        }
    }

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

    /// <summary>Enumerates the existing pile objects in a stable order for history capture and restoration.</summary>
    /// <returns>Variant-dependent piles, then tableau columns, then foundations.</returns>
    private IEnumerable<List<Card>> EnumeratePiles()
    {
        var leadingPiles = ActiveVariantBehavior.EnumerateLeadingPiles(_stock, _waste, _freeCells);

        foreach (var pile in leadingPiles)
        {
            yield return pile;
        }

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
            OpenNewGamePrompt();
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

        if (HelpButtonBounds.Contains(position))
        {
            OpenHelpOverlay();
            return true;
        }

        if (OptionsButtonBounds.Contains(position))
        {
            ToggleOptions();
            return true;
        }

        return false;
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
                SetHint(GetStockBounds(), Rectangle.Empty,
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
            return GetWasteBounds();
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
        _hintTimeRemaining = HintDisplayDuration;
    }

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
        _interactionController.UpdatePointer(position, isDown, time);
    }

    /// <summary>Handles a short click, using card identity, position, and time to recognize double-clicks.</summary>
    /// <param name="position">The released pointer position.</param>
    /// <param name="time">The current game time.</param>
    private void HandleReleasedClick(Point position, TimeSpan time)
    {
        _interactionController.HandleReleasedClick(position, time);
    }

    /// <summary>Cancels pending dragging and double-click history when a new deal starts.</summary>
    private void ResetPointerGesture()
    {
        _interactionController.ResetPointerGesture();
    }

    /// <summary>Attempts to drop selected cards only onto an actual foundation or tableau hit area.</summary>
    /// <param name="position">The pointer position at release.</param>
    private void DropSelectedCards(Point position)
    {
        _interactionController.DropSelectedCards(position);
    }

    /// <summary>Attempts a variant-specific top-row drop target before shared drop handling.</summary>
    /// <param name="position">The pointer position at release.</param>
    /// <returns>Whether a variant-specific top-row drop was handled.</returns>
    private bool TryDropToVariantTopRow(Point position)
    {
        return ActiveVariantBehavior.TryDropToVariantTopRow(position, _freeCells, GetFreeCellBounds,
            TryMoveToFreeCell);
    }

    /// <summary>Checks tableau legality without changing selection or card state.</summary>
    /// <param name="columnIndex">The destination tableau index.</param>
    /// <returns>Whether the selection can move to a distinct tableau pile.</returns>
    private bool CanMoveToTableau(int columnIndex)
    {
        var destination = _tableau[columnIndex];
        return ActiveVariantBehavior.CanMoveToTableau(_selectedPile, _selectedCardIndex, destination, _tableau,
            _freeCells, _stock, _waste);
    }

    /// <summary>Checks FreeCell holding-cell legality without changing selection or card state.</summary>
    /// <param name="freeCellIndex">The destination FreeCell holding-cell index.</param>
    /// <returns>Whether the selection can move to the target holding cell.</returns>
    private bool CanMoveToFreeCell(int freeCellIndex) =>
        ActiveVariantBehavior.CanMoveToFreeCell(_selectedPile, _selectedCardIndex, _freeCells, freeCellIndex);

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
        return SolitaireMoveRules.CanMoveToFoundation(movingCard, destination);
    }

    /// <summary>Calculates the horizontal position for one tableau column in the active variant.</summary>
    /// <param name="columnIndex">The tableau column index.</param>
    /// <returns>The left edge of the column in logical board coordinates.</returns>
    private int GetTableauColumnX(int columnIndex)
    {
        return ActiveVariantBehavior.GetTableauColumnX(columnIndex, _tableau.Count);
    }

    /// <summary>Gets the complete visible tableau hit area, excluding the footer.</summary>
    /// <param name="columnIndex">The tableau index.</param>
    /// <returns>The union of its cards, or one card-sized slot for an empty column.</returns>
    private Rectangle GetTableauBounds(int columnIndex)
    {
        var pile = _tableau[columnIndex];
        var bottom = pile.Count == 0 ? TableauTop + CardHeight : GetTableauCardBounds(columnIndex, pile.Count - 1).Bottom;
        return new Rectangle(GetTableauColumnX(columnIndex), TableauTop,
            CardWidth, Math.Min(bottom, FooterY) - TableauTop);
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
            cardY += _tableau[columnIndex][index].FaceUp ? TableauStep : TableauFaceDownStep;
        }

        return new Rectangle(GetTableauColumnX(columnIndex), cardY, CardWidth, CardHeight);
    }

    /// <summary>Finds the visible, face-up card under the pointer, ignoring hidden stock cards.</summary>
    /// <param name="position">The pointer position.</param>
    /// <param name="pile">The containing pile, or null when no selectable card was hit.</param>
    /// <param name="cardIndex">The card index within the containing pile.</param>
    /// <param name="bounds">The card's full drawing bounds.</param>
    /// <returns>Whether a selectable card was hit.</returns>
    private bool TryGetCardAt(Point position, out List<Card> pile, out int cardIndex, out Rectangle bounds)
    {
        if (ActiveVariantBehavior.TryGetTopRowCardAt(position, _waste, GetWasteBounds, _freeCells,
                GetFreeCellBounds, out var topRowPile, out cardIndex, out bounds))
        {
            pile = topRowPile!;
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

    /// <summary>Whether small labels and card ranks use the larger text layout.</summary>
    private bool _largeText;

    /// <summary>Whether the board uses solid black surfaces and high-contrast text and cards.</summary>
    private bool _highContrast;

    /// <summary>Whether slides, flips, fades, and celebration particles are suppressed.</summary>
    private bool _reducedMotion;

    /// <summary>Whether the modal accessibility settings panel is open.</summary>
    private bool _optionsOpen;

    /// <summary>Whether the in-game Help overlay is visible.</summary>
    private bool _helpOpen;

    /// <summary>The current vertical scroll offset for Help overlay content.</summary>
    private int _helpScrollOffset;

    /// <summary>Whether the in-app manual smoke-pass checklist overlay is visible.</summary>
    private bool _smokePassOverlayOpen;

    /// <summary>The focused settings row: larger text, high contrast, reduced motion, or close.</summary>
    private int _optionIndex;

    /// <summary>The previous mouse-wheel value used to detect edge-triggered Help scrolling.</summary>
    private int _previousScrollWheelValue;

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
        _interactionController.IsNewKey(keyboard, key);

    /// <summary>Opens or closes settings while cancelling any pending pointer gesture without moving cards.</summary>
    private void ToggleOptions()
    {
        _interactionController.ToggleOptions();
    }

    /// <summary>Handles settings shortcuts before ordinary board controls so the panel cannot activate the board.</summary>
    /// <param name="keyboard">The current keyboard state.</param>
    /// <returns>Whether the keyboard update belongs to settings rather than the board.</returns>
    private bool HandleAccessibilityKeys(KeyboardState keyboard)
    {
        return _interactionController.HandleAccessibilityKeys(keyboard);
    }

    /// <summary>Changes reduced motion immediately, settling existing transitions without changing their logical results.</summary>
    private void ToggleReducedMotion()
    {
        _interactionController.ToggleReducedMotion();
    }

    /// <summary>Activates one settings row without changing history, statistics, or the deal.</summary>
    /// <param name="index">The settings row index.</param>
    private void ActivateOption(int index)
    {
        _interactionController.ActivateOption(index);
    }

    /// <summary>Consumes settings pointer input, requiring press and release on the same row.</summary>
    /// <param name="position">The logical pointer position.</param>
    /// <param name="isDown">Whether the left button is down.</param>
    private void UpdateOptionsPointer(Point position, bool isDown)
    {
        _interactionController.UpdateOptionsPointer(position, isDown);
    }

    /// <summary>Processes edge-triggered keyboard navigation without interfering with active pointer gestures.</summary>
    /// <param name="keyboard">The current keyboard state.</param>
    private void HandleBoardKeyboard(KeyboardState keyboard)
    {
        _interactionController.HandleBoardKeyboard(keyboard);
    }

    /// <summary>Focuses a control or pile, choosing its top card without altering the current selection.</summary>
    /// <param name="index">The target index in keyboard traversal order.</param>
    private void FocusKeyboardTarget(int index)
    {
        _interactionController.FocusKeyboardTarget(index);
    }

    /// <summary>Keeps tableau focus on an exposed face-up card after moves, undo, or a new deal.</summary>
    private void NormalizeKeyboardCard()
    {
        _interactionController.NormalizeKeyboardCard();
    }

    /// <summary>Gets the pile under keyboard focus, excluding controls and stock.</summary>
    /// <returns>The focused source or destination pile, or null for a control or stock.</returns>
    private List<Card> GetKeyboardPile() => _interactionController.GetKeyboardPile();

    /// <summary>Activates a focused toolbar control, retaining its existing availability checks.</summary>
    private void ActivateKeyboardControl()
    {
        _interactionController.ActivateKeyboardControl();
    }

    /// <summary>Selects a focused card stack or moves the existing selection onto the focused legal destination.</summary>
    private void ActivateKeyboardTarget()
    {
        _interactionController.ActivateKeyboardTarget();
    }

    /// <summary>Moves the selected stack, or focused card when nothing is selected, to its first legal foundation.</summary>
    private void MoveKeyboardCardToFoundation()
    {
        _interactionController.MoveKeyboardCardToFoundation();
    }

    /// <summary>Gets the visible bounds of the focused control, slot, or card strip.</summary>
    /// <returns>The focused logical rectangle.</returns>
    private Rectangle GetKeyboardBounds()
    {
        return _interactionController.GetKeyboardBounds();
    }

    /// <summary>Describes keyboard focus using rank, suit names, and selection state rather than color alone.</summary>
    /// <returns>The concise footer description.</returns>
    private string GetKeyboardDescription()
    {
        return _interactionController.GetKeyboardDescription();
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
        _interactionController.UpdateClientPointer(clientPosition, isDown, time);
    }

    /// <summary>Shuffles a standard deck and deals the active variant's opening board.</summary>
    private void StartNewGame()
    {
        _stock.Clear();
        _waste.Clear();
        _tableau.Clear();
        _foundations.Clear();
        _freeCells.Clear();
        ClearSelection();
        ResetPointerGesture();
        ResetControls();
        ResetFeedback();
        _hasWon = false;

        var deck = SolitaireDealBuilder.CreateShuffledDeck(new Random());
        ActiveVariantBehavior.InitializeDeal(deck, _tableau, _stock, _waste, _foundations, _freeCells);

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
        var handledTopRowClick = ActiveVariantBehavior.TryHandleTopRowClick(clickPosition, _freeCells,
            GetFreeCellBounds, _selectedPile, TryMoveToFreeCell, ShowInvalidMove, SelectTop, GetStockBounds,
            GetWasteBounds, _waste, DrawFromStock);
        if (handledTopRowClick)
        {
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

    /// <summary>Attempts to move the selected card to a FreeCell holding cell.</summary>
    /// <param name="freeCellIndex">The zero-based index of the destination FreeCell holding cell.</param>
    /// <returns>Whether the selected card was moved to the FreeCell holding cell.</returns>
    private bool TryMoveToFreeCell(int freeCellIndex)
    {
        return FreeCellGameplay.TryMoveToCell(_variant, _selectedPile, _selectedCardIndex, _freeCells, freeCellIndex,
            CanMoveToFreeCell, () => MoveSelectedCards(_freeCells[freeCellIndex]), ClearSelection);
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
                : card.FaceUp ? TableauStep : TableauFaceDownStep;
            var cardBottom = cardY + exposedHeight;

            if (mouseY >= cardY && mouseY < cardBottom)
            {
                return cardIndex;
            }

            cardY += card.FaceUp ? TableauStep : TableauFaceDownStep;
        }

        return -1;
    }
}
