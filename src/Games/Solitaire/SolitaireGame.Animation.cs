// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Provides presentation-only transitions while keeping card rules and pile ownership immediate.</summary>
internal sealed partial class SolitaireGame
{
    /// <summary>The duration of an ordinary card slide or flip, in seconds.</summary>
    private const float MoveDuration = 0.22f;

    /// <summary>The active card transitions, ordered from underlying cards to overlying cards.</summary>
    private readonly Dictionary<Card, CardTransition> _cardTransitions = new();

    /// <summary>The elapsed time since the current transition group began, in seconds.</summary>
    private float _animationElapsed;

    /// <summary>The elapsed time since the current card selection began, in seconds.</summary>
    private float _selectionElapsed;

    /// <summary>Describes a card's position and visibility before a logical operation.</summary>
    /// <param name="Bounds">The full card rectangle in board coordinates.</param>
    /// <param name="FaceUp">Whether the card face was visible.</param>
    private readonly record struct CardPresentation(Rectangle Bounds, bool FaceUp);

    /// <summary>Describes the endpoints and delay of one presentation-only card transition.</summary>
    /// <param name="Start">The source position and face state.</param>
    /// <param name="End">The destination position and face state.</param>
    /// <param name="Delay">The delay before this transition begins, in seconds.</param>
    private readonly record struct CardTransition(CardPresentation Start, CardPresentation End, float Delay);

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
}
