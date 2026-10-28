using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Provides pointer gestures and destination feedback for the Solitaire board.</summary>
public sealed partial class SolitaireGame
{
    /// <summary>The minimum pointer movement in pixels required to start dragging.</summary>
    private const int DragThreshold = 6;

    /// <summary>The maximum interval between two clicks on the same card.</summary>
    private static readonly TimeSpan DoubleClickInterval = TimeSpan.FromMilliseconds(350);

    /// <summary>The current pointer position in window coordinates.</summary>
    private Point _pointerPosition;

    /// <summary>The pointer position at the start of the current press.</summary>
    private Point _pressPosition;

    /// <summary>The offset from the pressed card's top-left corner to the pointer.</summary>
    private Point _dragOffset;

    /// <summary>The pile pressed as a potential drag source, or null when no face-up card was pressed.</summary>
    private List<Card> _pressedPile = null!;

    /// <summary>The index of the pressed card within the potential source pile.</summary>
    private int _pressedCardIndex;

    /// <summary>Whether the left button was down during the previous pointer update.</summary>
    private bool _pointerWasDown;

    /// <summary>Whether the current gesture has crossed the drag threshold.</summary>
    private bool _isDragging;

    /// <summary>Whether a held button must be released after a restart before another gesture can begin.</summary>
    private bool _ignoreUntilRelease;

    /// <summary>The card last clicked without moving it, or null when no double-click is pending.</summary>
    private Card _lastClickedCard = null!;

    /// <summary>The position of the last click used for double-click detection.</summary>
    private Point _lastClickPosition;

    /// <summary>The time of the last click used for double-click detection.</summary>
    private TimeSpan _lastClickTime;

    /// <summary>Processes press, movement, and release transitions without modifying piles during a drag.</summary>
    /// <param name="position">The current pointer position in window coordinates.</param>
    /// <param name="isDown">Whether the left mouse button is down.</param>
    /// <param name="time">The monotonically increasing game time used to recognize double-clicks.</param>
    private void UpdatePointer(Point position, bool isDown, TimeSpan time)
    {
        _pointerPosition = position;
        if (_cardTransitions.Count > 0)
        {
            if (isDown && !_pointerWasDown && NewGameButtonBounds.Contains(position))
            {
                StartNewGame();
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
                for (var foundationIndex = 0; foundationIndex < _foundations.Count; foundationIndex++)
                {
                    if (TryMoveToFoundation(foundationIndex))
                    {
                        break;
                    }
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
        return destination.Count == 0 ? movingCard.Rank == 13 :
            destination[^1].FaceUp && destination[^1].Rank == movingCard.Rank + 1 &&
            destination[^1].IsRed != movingCard.IsRed;
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
        return destination.Count == 0 ? movingCard.Rank == 1 :
            destination[^1].Suit == movingCard.Suit && destination[^1].Rank + 1 == movingCard.Rank;
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
}
