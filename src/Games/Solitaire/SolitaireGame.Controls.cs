using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Games.Solitaire;

/// <summary>Provides toolbar controls, reversible board history, one-step hints, and game statistics.</summary>
public sealed partial class SolitaireGame
{
    /// <summary>The new-game button bounds shared by drawing and pointer handling.</summary>
    private static readonly Rectangle NewGameButtonBounds = new(32, 20, 132, 44);

    /// <summary>The undo button bounds in logical board coordinates.</summary>
    private static readonly Rectangle UndoButtonBounds = new(180, 20, 104, 44);

    /// <summary>The hint button bounds in logical board coordinates.</summary>
    private static readonly Rectangle HintButtonBounds = new(300, 20, 104, 44);

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
        if (keyboardState.IsKeyDown(Keys.M) && !_previousKeyboardState.IsKeyDown(Keys.M))
        {
            ToggleSound();
        }

        if (_cardTransitions.Count > 0 || _isDragging)
        {
            return;
        }

        if (keyboardState.IsKeyDown(Keys.U) && !_previousKeyboardState.IsKeyDown(Keys.U))
        {
            UndoLastAction();
        }
        else if (keyboardState.IsKeyDown(Keys.H) && !_previousKeyboardState.IsKeyDown(Keys.H))
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
        DrawToolbarButton(NewGameButtonBounds, "NEW GAME", true);
        DrawToolbarButton(UndoButtonBounds, "UNDO", canUseBoard && _undoHistory.Count > 0);
        DrawToolbarButton(HintButtonBounds, "HINT", canUseBoard && !_hasWon);
        DrawToolbarButton(SoundButtonBounds, _audioUnavailable ? "NA" : _soundEnabled ? "SND" : "OFF",
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
                            "HINT  MOVE GOLD CARD TO GOLD FOUNDATION");
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
                                    "HINT  MOVE GOLD CARD TO GOLD TABLEAU");
                                return;
                            }
                        }
                    }
                }
            }

            if (_stock.Count > 0 || _waste.Count > 0)
            {
                SetHint(new Rectangle(32, 82, CardWidth, CardHeight), Rectangle.Empty,
                    _stock.Count > 0 ? "HINT  CLICK STOCK TO DRAW" : "HINT  CLICK EMPTY STOCK TO RECYCLE");
            }
            else
            {
                SetHint(Rectangle.Empty, Rectangle.Empty, "HINT  NO SUGGESTED MOVE  TRY UNDO OR NEW GAME");
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
}
