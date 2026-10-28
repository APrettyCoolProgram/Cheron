// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using System.Linq;
using Cheron.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Games.Solitaire;

/// <summary>Implements Klondike rules, state, interaction, and rendering independently of the application host.</summary>
public sealed partial class SolitaireGame : IGameModule
{
    /// <summary>The width of a card or empty pile slot, in pixels.</summary>
    private const int CardWidth = 78;

    /// <summary>The height of a card or empty pile slot, in pixels.</summary>
    private const int CardHeight = 108;

    /// <summary>The horizontal space between adjacent foundation slots, in pixels.</summary>
    private const int CardGap = 18;

    /// <summary>The vertical position of the first card in every tableau column, in pixels.</summary>
    private const int TableauTop = 218;

    /// <summary>The vertical offset between consecutive tableau cards when the preceding card is face up.</summary>
    private const int TableauStep = 29;

    /// <summary>The horizontal position of the first tableau column, in pixels.</summary>
    private const int FirstColumnX = 32;

    /// <summary>The horizontal offset between consecutive tableau columns, in pixels.</summary>
    private const int ColumnStep = 148;

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

    /// <summary>The module-owned white pixel used to draw solid rectangles and bitmap lettering.</summary>
    private Texture2D _pixelTexture = null!;

    /// <summary>The module-owned cached textures used for the table, cards, buttons, and pile slots.</summary>
    private SolitaireArtwork _artwork = null!;

    /// <summary>The last processed keyboard state, used to detect new shortcut key presses.</summary>
    private KeyboardState _previousKeyboardState;

    /// <summary>The source pile for a pending move, or <see langword="null"/> when no cards are selected.</summary>
    private List<Card> _selectedPile = null!;

    /// <summary>The index of the first selected card; subsequent cards in the same pile move with it.</summary>
    private int _selectedCardIndex;

    /// <summary>Whether the last completed move left all four foundations with thirteen cards each.</summary>
    private bool _hasWon;

    /// <summary>Gets the Solitaire window title.</summary>
    public string Title => "Cheron Solitaire";

    /// <summary>Gets the initial window size used by the board layout, in pixels.</summary>
    public Point PreferredWindowSize => new(1100, 800);

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
        _artwork = new SolitaireArtwork(graphicsDevice);
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

        if (keyboardState.IsKeyDown(Keys.R) && !_previousKeyboardState.IsKeyDown(Keys.R))
        {
            StartNewGame();
        }
        else
        {
            HandleControlKeys(keyboardState);
        }

        var mouseState = Mouse.GetState();

        UpdatePointer(new Point(mouseState.X, mouseState.Y),
            mouseState.LeftButton == ButtonState.Pressed, gameTime.TotalGameTime);

        _previousKeyboardState = keyboardState;
    }

    /// <summary>Draws the board, cards, controls, and win message.</summary>
    /// <remarks>
    /// Uses a fixed 1100-by-800 layout in window coordinates and requires loaded drawing resources.
    /// </remarks>
    /// <param name="gameTime">The host-supplied timing information; animation progress is advanced during updates.</param>
    /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(new Color(12, 34, 39));

        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
        _spriteBatch.Draw(_artwork.Background, new Rectangle(0, 0, 1100, 800), Color.White);

        Fill(new Rectangle(0, 0, 1100, 72), new Color(12, 31, 37, 220));
        Fill(new Rectangle(0, 71, 1100, 1), new Color(67, 102, 102));
        DrawToolbar();

        var stockBounds = new Rectangle(32, 82, CardWidth, CardHeight);

        DrawPile(_stock, stockBounds, faceDown: true);

        if (_stock.Count == 0)
        {
            DrawText("DEAL", 47, 122, 1, new Color(156, 186, 178));
            DrawText("AGAIN", 44, 138, 1, new Color(156, 186, 178));
        }

        DrawPile(_waste, new Rectangle(128, 82, CardWidth, CardHeight));
        DrawText("STOCK", 40, 199, 1, new Color(157, 184, 176));
        DrawText("WASTE", 136, 199, 1, new Color(157, 184, 176));

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            var foundationBounds = GetFoundationBounds(foundationIndex);

            DrawPile(_foundations[foundationIndex], foundationBounds);

            if (_foundations[foundationIndex].Count == 0)
            {
                DrawText("A", foundationBounds.X + 33, foundationBounds.Y + 24, 2, new Color(119, 153, 145));
                DrawText("TO KING", foundationBounds.X + 19, foundationBounds.Y + 65, 1, new Color(119, 153, 145));
            }
        }

        DrawText("FOUNDATIONS", 796, 199, 1, new Color(157, 184, 176));

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            var tableauPile = _tableau[columnIndex];
            var cardY = TableauTop;

            if (tableauPile.Count == 0)
            {
                var slotBounds = new Rectangle(FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);
                _spriteBatch.Draw(_artwork.Slot, slotBounds, Color.White);
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

        Fill(new Rectangle(0, 728, 1100, 72), new Color(12, 31, 37, 230));
        Fill(new Rectangle(0, 728, 1100, 1), new Color(67, 102, 102));
        var footerMessage = _invalidTimeRemaining > TimeSpan.Zero
            ? "MOVE NOT ALLOWED  TRY ANOTHER TARGET OR USE HINT"
            : _hintTimeRemaining > TimeSpan.Zero ? _hintMessage : "BUILD DOWN IN ALTERNATING COLORS";
        DrawText(footerMessage, 32, 747, 1, new Color(207, 220, 207));
        DrawText("DRAG OR CLICK TO MOVE  DOUBLE CLICK TO BUILD", 32, 767, 1, new Color(153, 183, 176));
        DrawText(_audioUnavailable ? "SOUND UNAVAILABLE" : "ACES TO KINGS", 883, 747, 1, new Color(235, 205, 143));
        DrawText("ESC  EXIT", 907, 767, 1, new Color(153, 183, 176));

        if (_hasWon && _cardTransitions.Count == 0)
        {
            _spriteBatch.Draw(_artwork.Banner, new Rectangle(370, 376, 360, 56), Color.White);
            DrawText("YOU WIN!", 483, 394, 3, new Color(251, 219, 148));
        }

        _spriteBatch.End();
    }

    /// <summary>Releases owned drawing and audio resources without disposing the borrowed graphics device.</summary>
    public void Dispose()
    {
        DisposeFeedbackSounds();
        _artwork?.Dispose();
        _pixelTexture?.Dispose();
        _spriteBatch?.Dispose();
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

        var deck = new List<Card>(52);

        foreach (var suit in Enum.GetValues<Suit>())
        {
            for (var rank = 1; rank <= 13; rank++)
            {
                deck.Add(new Card(rank, suit));
            }
        }

        var random = new Random();

        for (var cardIndex = deck.Count - 1; cardIndex > 0; cardIndex--)
        {
            var swapIndex = random.Next(cardIndex + 1);
            (deck[cardIndex], deck[swapIndex]) = (deck[swapIndex], deck[cardIndex]);
        }

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

            _tableau.Add(tableauPile);
        }

        _stock.AddRange(deck);

        for (var foundationIndex = 0; foundationIndex < 4; foundationIndex++)
        {
            _foundations.Add(new List<Card>());
        }

        AnimateDeal();
    }

    /// <summary>Routes a board click to restart, draw, select, or move cards.</summary>
    /// <param name="clickPosition">The mouse position in window coordinates.</param>
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
        var sourcePile = _selectedPile!;
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
    /// <param name="mouseY">The mouse's vertical position in window coordinates.</param>
    /// <returns>The matching card index, or <c>-1</c> when no exposed strip contains the position.</returns>
    private int GetTableauCardIndex(int columnIndex, int mouseY)
    {
        var cardY = TableauTop;
        var tableauPile = _tableau[columnIndex];

        for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
        {
            var card = tableauPile[cardIndex];
            var cardBottom = cardY + (cardIndex == tableauPile.Count - 1
                ? CardHeight : card.FaceUp ? TableauStep : 22);

            if (mouseY >= cardY && mouseY < cardBottom)
            {
                return cardIndex;
            }

            cardY += card.FaceUp ? TableauStep : 22;
        }

        return -1;
    }

    /// <summary>Calculates a foundation slot's bounds in window coordinates.</summary>
    /// <param name="foundationIndex">The zero-based index of the foundation slot.</param>
    /// <returns>The rectangle used to draw and hit-test the foundation.</returns>
    private Rectangle GetFoundationBounds(int foundationIndex) =>
        new(1100 - 32 - (4 - foundationIndex) * (CardWidth + CardGap), 82, CardWidth, CardHeight);

    /// <summary>Draws a pile's top non-dragged card, or an outlined slot when no card remains visible.</summary>
    /// <param name="pile">The pile to draw.</param>
    /// <param name="bounds">The card or slot bounds in window coordinates.</param>
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
            _spriteBatch.Draw(_artwork.Slot, bounds, Color.White);
        }
    }

    /// <summary>Draws a rounded card, soft shadow, suit artwork, and optional selection highlight.</summary>
    /// <param name="card">The card whose rank, suit, and visibility determine its appearance.</param>
    /// <param name="bounds">The card bounds in window coordinates.</param>
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
            var highlightOpacity = Math.Clamp(_selectionElapsed / 0.12f, 0, 1);
            _spriteBatch.Draw(_artwork.Selection,
                new Rectangle(bounds.X - 3, bounds.Y - 3, bounds.Width + 6, bounds.Height + 6),
                Color.White * highlightOpacity);
        }

        _spriteBatch.Draw(faceDown ? _artwork.CardBack : _artwork.CardFace, bounds, Color.White);

        if (faceDown)
        {
            return;
        }

        var suitColor = card.IsRed ? new Color(180, 51, 65) : new Color(36, 57, 66);
        var rankLabel = card.Rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => card.Rank.ToString()
        };

        var suitTexture = _artwork.Suits[(int)card.Suit];

        DrawText(rankLabel, bounds.X + 8, bounds.Y + 8, 2, suitColor);
        _spriteBatch.Draw(suitTexture, new Rectangle(bounds.Right - 26, bounds.Y + 5, 20, 20), suitColor);
        _spriteBatch.Draw(suitTexture, new Rectangle(bounds.X + 19, bounds.Y + 35, 40, 40), suitColor);
        _spriteBatch.Draw(suitTexture, new Rectangle(bounds.X + 7, bounds.Bottom - 25, 18, 18), suitColor);
        DrawText(rankLabel, bounds.Right - (rankLabel.Length * 12) - 7, bounds.Bottom - 22, 2, suitColor);
    }

    /// <summary>Fills a rectangle using the module's single-pixel texture.</summary>
    /// <param name="bounds">The rectangle to fill in window coordinates.</param>
    /// <param name="color">The fill color.</param>
    private void Fill(Rectangle bounds, Color color) => _spriteBatch.Draw(_pixelTexture, bounds, color);

    /// <summary>Draws uppercase bitmap text, leaving blank character widths for unsupported characters.</summary>
    /// <param name="text">The text to draw; letters, digits, spaces, and exclamation marks are supported.</param>
    /// <param name="textX">The starting horizontal position in window coordinates.</param>
    /// <param name="textY">The starting vertical position in window coordinates.</param>
    /// <param name="scale">The size of each glyph pixel in screen pixels.</param>
    /// <param name="color">The text color.</param>
    private void DrawText(string text, int textX, int textY, int scale, Color color)
    {
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
                for (var glyphRow = 0; glyphRow < 7; glyphRow++)
                {
                    var rowBits = glyphIndex == 36
                        ? (glyphRow < 5 || glyphRow == 6 ? 4 : 0)
                        : Glyphs[glyphIndex, glyphRow];

                    for (var glyphColumn = 0; glyphColumn < 5; glyphColumn++)
                    {
                        var pixelMask = 1 << (4 - glyphColumn);

                        if ((rowBits & pixelMask) != 0)
                        {
                            var pixelBounds = new Rectangle(
                                textX + glyphColumn * scale, textY + glyphRow * scale, scale, scale);
                            Fill(pixelBounds, color);
                        }
                    }
                }
            }

            textX += 6 * scale;
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