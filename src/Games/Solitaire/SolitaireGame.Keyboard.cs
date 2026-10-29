// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Games.Solitaire;

/// <summary>Provides keyboard focus, card-stack selection, and legal moves using the existing board rules.</summary>
public sealed partial class SolitaireGame
{
    /// <summary>The focused control or pile: five controls, stock, waste, four foundations, then seven columns.</summary>
    private int _keyboardFocus = -1;

    /// <summary>The focused face-up card index within a tableau column.</summary>
    private int _keyboardCardIndex;

    /// <summary>Whether keyboard focus outlines and descriptive footer text are visible.</summary>
    private bool _keyboardFocusVisible;

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
                var rank = card.Rank switch { 1 => "ACE", 11 => "JACK", 12 => "QUEEN", 13 => "KING", _ => card.Rank.ToString() };
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
}
