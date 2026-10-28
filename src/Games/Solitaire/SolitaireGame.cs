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
public sealed class SolitaireGame : IGameModule
{
    private const int CardWidth = 78;
    private const int CardHeight = 108;
    private const int CardGap = 18;
    private const int TableauTop = 218;
    private const int TableauStep = 29;
    private const int FirstColumnX = 32;
    private const int ColumnStep = 148;

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

    private readonly List<Card> _stock = new();
    private readonly List<Card> _waste = new();
    private readonly List<List<Card>> _tableau = new();
    private readonly List<List<Card>> _foundations = new();
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixelTexture = null!;
    private MouseState _previousMouseState;
    private KeyboardState _previousKeyboardState;
    private List<Card> _selectedPile = null!;
    private int _selectedCardIndex;
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

    /// <summary>Creates the sprite batch and single-pixel texture owned by this module.</summary>
    /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
    }

    /// <summary>Handles restart key presses and new left mouse clicks.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    public void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.R) && !_previousKeyboardState.IsKeyDown(Keys.R))
        {
            StartNewGame();
        }

        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            HandleClick(new Point(mouseState.X, mouseState.Y));
        }

        _previousMouseState = mouseState;
        _previousKeyboardState = keyboardState;
    }

    /// <summary>Draws the board, cards, controls, and win message.</summary>
    /// <param name="gameTime">The timing information supplied by MonoGame.</param>
    /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(new Color(24, 105, 66));
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawNewGameButton();
        DrawPile(_stock, new Rectangle(32, 82, CardWidth, CardHeight), faceDown: true);
        DrawPile(_waste, new Rectangle(128, 82, CardWidth, CardHeight));

        for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
        {
            DrawPile(_foundations[foundationIndex], GetFoundationBounds(foundationIndex));
        }

        for (var columnIndex = 0; columnIndex < _tableau.Count; columnIndex++)
        {
            var tableauPile = _tableau[columnIndex];
            var cardY = TableauTop;
            for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
            {
                var card = tableauPile[cardIndex];
                var cardBounds = new Rectangle(
                    FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);

                DrawCard(card, cardBounds, IsSelected(tableauPile, cardIndex));
                cardY += card.FaceUp ? TableauStep : 22;
            }
        }

        DrawText("CLICK STOCK TO DRAW", 32, 198, 1, Color.White);
        DrawText("BUILD DOWN IN ALTERNATING COLORS", 32, 744, 1, Color.White);
        DrawText("R RESTART", 32, 762, 1, Color.White);
        if (_hasWon)
        {
            DrawText("YOU WIN!", 490, 170, 3, Color.Gold);
        }

        _spriteBatch.End();
    }

    /// <summary>Releases the module-owned drawing resources without disposing the borrowed graphics device.</summary>
    public void Dispose()
    {
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
        _selectedPile = null;
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
    }

    /// <summary>Routes a board click to restart, draw, select, or move cards.</summary>
    /// <param name="clickPosition">The mouse position in window coordinates.</param>
    private void HandleClick(Point clickPosition)
    {
        if (new Rectangle(32, 20, 132, 44).Contains(clickPosition))
        {
            StartNewGame();
            return;
        }

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

            if (_selectedPile is not null && TryMoveToFoundation(foundationIndex))
            {
                return;
            }

            SelectTop(_foundations[foundationIndex]);
            return;
        }

        var columnIndex = (clickPosition.X - FirstColumnX) / ColumnStep;
        if (clickPosition.X < FirstColumnX || columnIndex < 0 || columnIndex >= _tableau.Count ||
            clickPosition.X >= FirstColumnX + columnIndex * ColumnStep + CardWidth)
        {
            ClearSelection();
            return;
        }

        var clickedCardIndex = GetTableauCardIndex(columnIndex, clickPosition.Y);
        if (_selectedPile is not null && TryMoveToTableau(columnIndex))
        {
            return;
        }

        if (clickedCardIndex >= 0 && _tableau[columnIndex][clickedCardIndex].FaceUp)
        {
            Select(_tableau[columnIndex], clickedCardIndex);
        }
        else
        {
            ClearSelection();
        }
    }

    /// <summary>Draws one stock card or recycles the waste face down into an empty stock.</summary>
    private void DrawFromStock()
    {
        ClearSelection();
        if (_stock.Count > 0)
        {
            var drawnCard = _stock[^1];
            _stock.RemoveAt(_stock.Count - 1);
            drawnCard.FaceUp = true;
            _waste.Add(drawnCard);
            return;
        }

        if (_waste.Count > 0)
        {
            for (var cardIndex = _waste.Count - 1; cardIndex >= 0; cardIndex--)
            {
                var recycledCard = _waste[cardIndex];
                recycledCard.FaceUp = false;
                _stock.Add(recycledCard);
            }

            _waste.Clear();
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

        var movingCard = _selectedPile[_selectedCardIndex];
        if (destinationPile.Count == 0)
        {
            if (movingCard.Rank != 13)
            {
                return false;
            }
        }
        else
        {
            var targetCard = destinationPile[^1];
            if (!targetCard.FaceUp || targetCard.Rank != movingCard.Rank + 1 || targetCard.IsRed == movingCard.IsRed)
            {
                return false;
            }
        }

        MoveSelectedCards(destinationPile);
        return true;
    }

    /// <summary>Attempts to move a single selected top card to a same-suit, ascending foundation.</summary>
    /// <param name="foundationIndex">The zero-based index of the destination foundation.</param>
    /// <returns>Whether the selected card was moved to the foundation.</returns>
    private bool TryMoveToFoundation(int foundationIndex)
    {
        if (_selectedPile is null || _selectedCardIndex != _selectedPile.Count - 1)
        {
            return false;
        }

        var destinationPile = _foundations[foundationIndex];
        var movingCard = _selectedPile[_selectedCardIndex];
        if (destinationPile.Count == 0)
        {
            if (movingCard.Rank != 1)
            {
                return false;
            }
        }
        else
        {
            var targetCard = destinationPile[^1];
            if (targetCard.Suit != movingCard.Suit || targetCard.Rank + 1 != movingCard.Rank)
            {
                return false;
            }
        }

        MoveSelectedCards(destinationPile);
        return true;
    }

    /// <summary>Transfers a validated selection, reveals any newly exposed tableau card, and checks for a win.</summary>
    /// <param name="destinationPile">The pile that receives the selected card and all cards above it.</param>
    private void MoveSelectedCards(List<Card> destinationPile)
    {
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

    /// <summary>Finds the first card whose vertical hit area contains the mouse position.</summary>
    /// <param name="columnIndex">The zero-based index of the tableau column.</param>
    /// <param name="mouseY">The mouse's vertical position in window coordinates.</param>
    /// <returns>The matching card index, or <c>-1</c> when no hit area contains the position.</returns>
    private int GetTableauCardIndex(int columnIndex, int mouseY)
    {
        var cardY = TableauTop;
        var tableauPile = _tableau[columnIndex];
        for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
        {
            var card = tableauPile[cardIndex];
            var cardBottom = cardY + (card.FaceUp ? CardHeight : 22);
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

    /// <summary>Draws the new-game button and its label.</summary>
    private void DrawNewGameButton()
    {
        var buttonBounds = new Rectangle(32, 20, 132, 44);
        Fill(buttonBounds, new Color(232, 224, 193));
        DrawBorder(buttonBounds, new Color(20, 57, 43), 2);
        DrawText("NEW GAME", 43, 35, 2, new Color(20, 57, 43));
    }

    /// <summary>Draws a pile's top card, or an outlined slot for an empty pile.</summary>
    /// <param name="pile">The pile to draw.</param>
    /// <param name="bounds">The card or slot bounds in window coordinates.</param>
    /// <param name="faceDown">Whether to show the card back regardless of its face-up state.</param>
    private void DrawPile(List<Card> pile, Rectangle bounds, bool faceDown = false)
    {
        if (pile.Count > 0)
        {
            DrawCard(pile[^1], bounds, IsSelected(pile, pile.Count - 1), faceDown);
        }
        else
        {
            DrawBorder(bounds, new Color(177, 211, 186), 2);
        }
    }

    /// <summary>Draws a card face or back, highlighting the selection when the face is visible.</summary>
    /// <param name="card">The card whose rank, suit, and visibility determine its appearance.</param>
    /// <param name="bounds">The card bounds in window coordinates.</param>
    /// <param name="isSelected">Whether the face-up card should receive a selection border.</param>
    /// <param name="faceDown">Whether to show the card back regardless of its face-up state.</param>
    private void DrawCard(Card card, Rectangle bounds, bool isSelected, bool faceDown = false)
    {
        if (faceDown || !card.FaceUp)
        {
            Fill(bounds, new Color(32, 74, 139));
            DrawBorder(bounds, Color.White, 2);
            var insetBounds = new Rectangle(bounds.X + 5, bounds.Y + 5, bounds.Width - 10, bounds.Height - 10);
            DrawBorder(insetBounds, new Color(175, 198, 229), 1);
            return;
        }

        Fill(bounds, new Color(249, 247, 235));
        DrawBorder(bounds, isSelected ? Color.Gold : new Color(35, 43, 39), isSelected ? 4 : 2);

        var suitColor = card.IsRed ? new Color(190, 38, 48) : new Color(30, 39, 43);
        var rankLabel = card.Rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => card.Rank.ToString()
        };
        var suitLabel = card.Suit.ToString()[..1];

        DrawText(rankLabel, bounds.X + 7, bounds.Y + 7, 2, suitColor);
        DrawText(suitLabel, bounds.X + 7, bounds.Y + 27, 2, suitColor);
        DrawText(suitLabel, bounds.X + bounds.Width - 19, bounds.Y + bounds.Height - 22, 2, suitColor);
    }

    /// <summary>Fills a rectangle using the module's single-pixel texture.</summary>
    /// <param name="bounds">The rectangle to fill in window coordinates.</param>
    /// <param name="color">The fill color.</param>
    private void Fill(Rectangle bounds, Color color) => _spriteBatch.Draw(_pixelTexture, bounds, color);

    /// <summary>Draws the four edges of a rectangular border.</summary>
    /// <param name="bounds">The outer border bounds in window coordinates.</param>
    /// <param name="color">The border color.</param>
    /// <param name="thickness">The thickness of each edge in pixels.</param>
    private void DrawBorder(Rectangle bounds, Color color, int thickness)
    {
        Fill(new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);
        Fill(new Rectangle(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), color);
        Fill(new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);
        Fill(new Rectangle(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), color);
    }

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
