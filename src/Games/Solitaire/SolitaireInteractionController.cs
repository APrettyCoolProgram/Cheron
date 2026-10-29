// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static Cheron.Games.Solitaire.SolitaireAccessibilityOptions;
using static Cheron.Games.Solitaire.SolitaireInteractionConstants;
using static Cheron.Games.Solitaire.SolitaireLayout;
using static Cheron.Games.Solitaire.SolitaireOptionsLayout;
using static Cheron.Games.Solitaire.SolitaireTextConstants;
using static Cheron.Games.Solitaire.SolitaireTimingConstants;

namespace Cheron.Games.Solitaire;

internal sealed partial class SolitaireGame
{
    /// <summary>Coordinates Solitaire keyboard and pointer interaction using the owning game state.</summary>
    private sealed class SolitaireInteractionController(SolitaireGame game)
    {
        private readonly SolitaireGame _game = game;

        /// <summary>Maps client-space pointer updates to board space before applying gestures or hit tests.</summary>
        /// <param name="clientPosition">The pointer position in window client units.</param>
        /// <param name="isDown">Whether the left button is currently down.</param>
        /// <param name="time">The monotonically increasing game time used for double-clicks.</param>
        internal void UpdateClientPointer(Point clientPosition, bool isDown, TimeSpan time)
        {
            UpdatePointer(_game._viewport.ToLogical(clientPosition), isDown, time);
        }

        /// <summary>Processes press, movement, and release transitions without modifying piles during a drag.</summary>
        /// <param name="position">The current pointer position in logical board coordinates.</param>
        /// <param name="isDown">Whether the left mouse button is down.</param>
        /// <param name="time">The monotonically increasing game time used to recognize double-clicks.</param>
        internal void UpdatePointer(Point position, bool isDown, TimeSpan time)
        {
            _game._pointerPosition = position;
            if (_game._smokePassOverlayOpen)
            {
                if (!isDown && _game._pointerWasDown)
                {
                    _game._smokePassOverlayOpen = false;
                }

                _game._pointerWasDown = isDown;
                return;
            }

            if (_game.IsNewGamePromptOpen)
            {
                UpdateNewGamePromptPointer(position, isDown);
                return;
            }

            if (_game._optionsOpen)
            {
                _game.UpdateOptionsPointer(position, isDown);
                return;
            }

            if (isDown && !_game._pointerWasDown)
            {
                _game._keyboardFocusVisible = false;
            }

            if (_game._cardTransitions.Count > 0)
            {
                if (isDown && !_game._pointerWasDown && NewGameButtonBounds.Contains(position))
                {
                    _game.OpenNewGamePrompt();
                }
                else if (isDown && !_game._pointerWasDown && SoundButtonBounds.Contains(position))
                {
                    _game.ToggleSound();
                }
                else if (isDown && !_game._pointerWasDown && OptionsButtonBounds.Contains(position))
                {
                    _game.ToggleOptions();
                }

                _game._pressedPile = null;
                _game._isDragging = false;
                _game._lastClickedCard = null;
                _game._ignoreUntilRelease = isDown;
                _game._pointerWasDown = isDown;
                return;
            }

            if (_game._ignoreUntilRelease)
            {
                _game._ignoreUntilRelease = isDown;
                _game._pointerWasDown = isDown;
                return;
            }

            if (isDown && !_game._pointerWasDown)
            {
                _game._pressPosition = position;
                _game._pressedPile = null;
                if (_game.TryGetCardAt(position, out var pile, out var cardIndex, out var bounds))
                {
                    _game._pressedPile = pile;
                    _game._pressedCardIndex = cardIndex;
                    _game._dragOffset = position - bounds.Location;
                }
            }

            var movement = position - _game._pressPosition;
            var movedBeyondThreshold = movement.ToVector2().LengthSquared() >= DragThreshold * DragThreshold;
            if (_game._pointerWasDown && _game._pressedPile is not null && !_game._isDragging && movedBeyondThreshold)
            {
                _game.Select(_game._pressedPile, _game._pressedCardIndex);
                _game.ClearHint();
                _game._isDragging = true;
                _game._lastClickedCard = null;
            }

            if (!isDown && _game._pointerWasDown)
            {
                if (_game._isDragging)
                {
                    var before = _game.CaptureCardPresentation();
                    DropSelectedCards(position);
                    if (_game._cardTransitions.Count == 0)
                    {
                        _game.AnimateChanges(before);
                    }

                    _game.ClearSelection();
                }
                else if (!movedBeyondThreshold)
                {
                    HandleReleasedClick(position, time);
                }
                else
                {
                    _game._lastClickedCard = null;
                }

                _game._pressedPile = null;
                _game._isDragging = false;
            }

            _game._ignoreUntilRelease &= isDown;
            _game._pointerWasDown = isDown;
        }

        /// <summary>Handles a short click, using card identity, position, and time to recognize double-clicks.</summary>
        /// <param name="position">The released pointer position.</param>
        /// <param name="time">The current game time.</param>
        internal void HandleReleasedClick(Point position, TimeSpan time)
        {
            if (_game.TryGetCardAt(position, out var pile, out var cardIndex, out _))
            {
                var card = pile[cardIndex];
                var clickDistance = (position - _game._lastClickPosition).ToVector2().LengthSquared();
                if (ReferenceEquals(card, _game._lastClickedCard) && time - _game._lastClickTime <= DoubleClickInterval &&
                    clickDistance < DragThreshold * DragThreshold)
                {
                    _game.Select(pile, cardIndex);
                    var movedToFoundation = false;
                    for (var foundationIndex = 0; foundationIndex < _game._foundations.Count; foundationIndex++)
                    {
                        if (_game.TryMoveToFoundation(foundationIndex))
                        {
                            movedToFoundation = true;
                            break;
                        }
                    }

                    if (!movedToFoundation)
                    {
                        _game.ShowInvalidMove(position);
                    }

                    _game._lastClickedCard = null;
                    return;
                }

                _game.HandleClick(position);
                _game._lastClickedCard = _game._selectedPile is not null && ReferenceEquals(_game._selectedPile, pile) &&
                    _game._selectedCardIndex == cardIndex ? card : null;
            }
            else
            {
                _game.HandleClick(position);
                _game._lastClickedCard = null;
            }

            _game._lastClickTime = time;
            _game._lastClickPosition = position;
        }

        /// <summary>Cancels pending dragging and double-click history when a new deal starts.</summary>
        internal void ResetPointerGesture()
        {
            _game._pressedPile = null;
            _game._isDragging = false;
            _game._lastClickedCard = null;
            _game._ignoreUntilRelease = _game._pointerWasDown;
        }

        /// <summary>Attempts to drop selected cards only onto an actual foundation, holding cell, or tableau hit area.</summary>
        /// <param name="position">The pointer position at release.</param>
        internal void DropSelectedCards(Point position)
        {
            if (_game.TryDropToVariantTopRow(position))
            {
                return;
            }

            for (var foundationIndex = 0; foundationIndex < _game._foundations.Count; foundationIndex++)
            {
                if (GetFoundationBounds(foundationIndex).Contains(position) && _game.CanMoveToFoundation(foundationIndex))
                {
                    _game.TryMoveToFoundation(foundationIndex);
                    return;
                }
            }

            for (var columnIndex = 0; columnIndex < _game._tableau.Count; columnIndex++)
            {
                if (_game.GetTableauBounds(columnIndex).Contains(position) && _game.CanMoveToTableau(columnIndex))
                {
                    _game.TryMoveToTableau(columnIndex);
                    return;
                }
            }

            _game.ShowInvalidMove(position);
        }

        /// <summary>Detects a new keyboard press without repeating held keys.</summary>
        /// <param name="keyboard">The current keyboard state.</param>
        /// <param name="key">The key to inspect.</param>
        /// <returns>Whether the key transitioned from released to pressed.</returns>
        internal bool IsNewKey(KeyboardState keyboard, Keys key) =>
            keyboard.IsKeyDown(key) && !_game._previousKeyboardState.IsKeyDown(key);

        /// <summary>Opens or closes settings while cancelling any pending pointer gesture without moving cards.</summary>
        internal void ToggleOptions()
        {
            var before = _game._isDragging ? _game.CaptureCardPresentation() : null;
            ResetPointerGesture();
            if (before is not null)
            {
                _game.ClearSelection();
                _game.AnimateChanges(before);
            }

            _game._optionsOpen = !_game._optionsOpen;
            _game._optionIndex = 0;
        }

        /// <summary>Handles settings shortcuts before ordinary board controls so the panel cannot activate the board.</summary>
        /// <param name="keyboard">The current keyboard state.</param>
        /// <returns>Whether the keyboard update belongs to settings rather than the board.</returns>
        internal bool HandleAccessibilityKeys(KeyboardState keyboard)
        {
            if (IsNewKey(keyboard, Keys.F9) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
            {
                _game._smokePassOverlayOpen = !_game._smokePassOverlayOpen;
                _game._optionsOpen = false;
                _game._newGamePromptOpen = false;
                ResetPointerGesture();
                return true;
            }

            if (_game._smokePassOverlayOpen)
            {
                if (IsNewKey(keyboard, Keys.Escape) || IsNewKey(keyboard, Keys.Back) || IsNewKey(keyboard, Keys.F9))
                {
                    _game._smokePassOverlayOpen = false;
                }

                return true;
            }

            if (_game.IsNewGamePromptOpen)
            {
                if (IsNewKey(keyboard, Keys.Left) || IsNewKey(keyboard, Keys.Right) ||
                    IsNewKey(keyboard, Keys.Up) || IsNewKey(keyboard, Keys.Down))
                {
                    _game._newGamePromptVariant = _game._newGamePromptVariant == SolitaireVariant.Klondike
                        ? SolitaireVariant.FreeCell
                        : SolitaireVariant.Klondike;
                    return true;
                }

                if (IsNewKey(keyboard, Keys.Enter) || IsNewKey(keyboard, Keys.Space))
                {
                    _game._variant = _game._newGamePromptVariant;
                    _game._newGamePromptOpen = false;
                    _game.StartNewGame();
                    return true;
                }

                if (IsNewKey(keyboard, Keys.Back) || IsNewKey(keyboard, Keys.Escape))
                {
                    _game._newGamePromptOpen = false;
                    return true;
                }

                return true;
            }

            if (IsNewKey(keyboard, Keys.F1))
            {
                ToggleOptions();
                return true;
            }

            if (IsNewKey(keyboard, Keys.F2))
            {
                _game._largeText = !_game._largeText;
                return true;
            }

            if (IsNewKey(keyboard, Keys.F3))
            {
                _game._highContrast = !_game._highContrast;
                return true;
            }

            if (IsNewKey(keyboard, Keys.F4))
            {
                ToggleReducedMotion();
                return true;
            }

            if (!_game._optionsOpen)
            {
                return false;
            }

            if (IsNewKey(keyboard, Keys.Tab) || IsNewKey(keyboard, Keys.Down) || IsNewKey(keyboard, Keys.Up))
            {
                var navigateBackward = IsNewKey(keyboard, Keys.Up) || keyboard.IsKeyDown(Keys.LeftShift) ||
                    keyboard.IsKeyDown(Keys.RightShift);
                _game._optionIndex = (_game._optionIndex + (navigateBackward ? LastOptionIndex : 1)) % OptionCount;
            }
            else if (IsNewKey(keyboard, Keys.Enter) || IsNewKey(keyboard, Keys.Space))
            {
                ActivateOption(_game._optionIndex);
            }

            return true;
        }

        /// <summary>Changes reduced motion immediately, settling existing transitions without changing their logical results.</summary>
        internal void ToggleReducedMotion()
        {
            _game._reducedMotion = !_game._reducedMotion;
            if (_game._reducedMotion)
            {
                _game._cardTransitions.Clear();
                _game._selectionElapsed = 1;
                _game._celebrationElapsed = CelebrationDuration;
            }
        }

        /// <summary>Activates one settings row without changing history, statistics, or the deal.</summary>
        /// <param name="index">The settings row index.</param>
        internal void ActivateOption(int index)
        {
            switch (index)
            {
                case 0:
                    _game._largeText = !_game._largeText;
                    break;
                case 1:
                    _game._highContrast = !_game._highContrast;
                    break;
                case 2:
                    ToggleReducedMotion();
                    break;
                case 3:
                    ToggleOptions();
                    break;
            }
        }

        /// <summary>Consumes new-game prompt pointer input for variant selection and prompt dismissal.</summary>
        /// <param name="position">The logical pointer position.</param>
        /// <param name="isDown">Whether the left button is down.</param>
        internal void UpdateNewGamePromptPointer(Point position, bool isDown)
        {
            if (_game._ignoreUntilRelease)
            {
                _game._ignoreUntilRelease = isDown;
                _game._pointerWasDown = isDown;
                return;
            }

            if (isDown && !_game._pointerWasDown)
            {
                _game._pressPosition = position;
            }

            if (!isDown && _game._pointerWasDown)
            {
                var panelBounds = GetNewGamePromptPanelBounds();
                var klondikeBounds = GetNewGamePromptKlondikeBounds();
                var freeCellBounds = GetNewGamePromptFreeCellBounds();
                var confirmBounds = GetNewGamePromptConfirmBounds();
                var cancelBounds = GetNewGamePromptCancelBounds();

                if (klondikeBounds.Contains(position) && klondikeBounds.Contains(_game._pressPosition))
                {
                    _game._newGamePromptVariant = SolitaireVariant.Klondike;
                    _game._variant = SolitaireVariant.Klondike;
                    _game._newGamePromptOpen = false;
                    _game.StartNewGame();
                }
                else if (freeCellBounds.Contains(position) && freeCellBounds.Contains(_game._pressPosition))
                {
                    _game._newGamePromptVariant = SolitaireVariant.FreeCell;
                    _game._variant = SolitaireVariant.FreeCell;
                    _game._newGamePromptOpen = false;
                    _game.StartNewGame();
                }
                else if (confirmBounds.Contains(position) && confirmBounds.Contains(_game._pressPosition))
                {
                    _game._variant = _game._newGamePromptVariant;
                    _game._newGamePromptOpen = false;
                    _game.StartNewGame();
                }
                else if ((cancelBounds.Contains(position) && cancelBounds.Contains(_game._pressPosition)) ||
                    (!panelBounds.Contains(position) && !panelBounds.Contains(_game._pressPosition)))
                {
                    _game._newGamePromptOpen = false;
                }
            }

            _game._ignoreUntilRelease &= isDown;
            _game._pointerWasDown = isDown;
        }

        /// <summary>Consumes settings pointer input, requiring press and release on the same row.</summary>
        /// <param name="position">The logical pointer position.</param>
        /// <param name="isDown">Whether the left button is down.</param>
        internal void UpdateOptionsPointer(Point position, bool isDown)
        {
            if (_game._ignoreUntilRelease)
            {
                _game._ignoreUntilRelease = isDown;
                _game._pointerWasDown = isDown;
                return;
            }

            if (isDown && !_game._pointerWasDown)
            {
                _game._pressPosition = position;
            }

            if (!isDown && _game._pointerWasDown)
            {
                for (var index = 0; index < OptionCount; index++)
                {
                    var bounds = GetOptionBounds(index);
                    if (bounds.Contains(position) && bounds.Contains(_game._pressPosition))
                    {
                        _game._optionIndex = index;
                        ActivateOption(index);
                        break;
                    }
                }
            }

            _game._ignoreUntilRelease &= isDown;
            _game._pointerWasDown = isDown;
        }

        /// <summary>Processes edge-triggered keyboard navigation without interfering with active pointer gestures.</summary>
        /// <param name="keyboard">The current keyboard state.</param>
        internal void HandleBoardKeyboard(KeyboardState keyboard)
        {
            if (_game._optionsOpen || _game._isDragging || _game._pointerWasDown)
            {
                return;
            }

            if (IsNewKey(keyboard, Keys.Tab))
            {
                var backwards = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
                var next = !_game._keyboardFocusVisible ? (backwards ? 17 : 0)
                    : (_game._keyboardFocus + (backwards ? 17 : 1)) % 18;
                FocusKeyboardTarget(next);
                return;
            }

            if (_game._cardTransitions.Count > 0)
            {
                if (_game._keyboardFocusVisible && (IsNewKey(keyboard, Keys.Enter) || IsNewKey(keyboard, Keys.Space)))
                {
                    ActivateKeyboardControl();
                }

                return;
            }

            if (IsNewKey(keyboard, Keys.Back))
            {
                _game.ClearSelection();
                _game.ClearHint();
                return;
            }

            if (IsNewKey(keyboard, Keys.Left) || IsNewKey(keyboard, Keys.Right))
            {
                var next = !_game._keyboardFocusVisible || _game._keyboardFocus < 5 ? 5
                    : 5 + (_game._keyboardFocus - 5 + (IsNewKey(keyboard, Keys.Left) ? 12 : 1)) % 13;
                FocusKeyboardTarget(next);
                return;
            }

            if (IsNewKey(keyboard, Keys.Up) || IsNewKey(keyboard, Keys.Down))
            {
                if (!_game._keyboardFocusVisible)
                {
                    FocusKeyboardTarget(11);
                }
                else if (_game._keyboardFocus >= 11)
                {
                    _game._keyboardCardIndex += IsNewKey(keyboard, Keys.Up) ? -1 : 1;
                    NormalizeKeyboardCard();
                }

                return;
            }

            if (!_game._keyboardFocusVisible)
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
        internal void FocusKeyboardTarget(int index)
        {
            _game._keyboardFocus = index;
            _game._keyboardFocusVisible = true;
            _game._keyboardCardIndex = index >= 11 ? Math.Max(0, _game._tableau[index - 11].Count - 1) : 0;
            NormalizeKeyboardCard();
        }

        /// <summary>Keeps tableau focus on an exposed face-up card after moves, undo, or a new deal.</summary>
        internal void NormalizeKeyboardCard()
        {
            if (_game._keyboardFocus < 11)
            {
                return;
            }

            var pile = _game._tableau[_game._keyboardFocus - 11];
            var firstFaceUp = pile.FindIndex(card => card.FaceUp);
            _game._keyboardCardIndex = pile.Count == 0 ? 0
                : Math.Clamp(_game._keyboardCardIndex, Math.Max(0, firstFaceUp), pile.Count - 1);
        }

        /// <summary>Gets the pile under keyboard focus, excluding controls and stock.</summary>
        /// <returns>The focused source or destination pile, or null for a control or stock.</returns>
        internal List<Card> GetKeyboardPile() => _game._keyboardFocus switch
        {
            6 => _game._waste,
            >= 7 and <= 10 => _game._foundations[_game._keyboardFocus - 7],
            >= 11 => _game._tableau[_game._keyboardFocus - 11],
            _ => null
        };

        /// <summary>Activates a focused toolbar control, retaining its existing availability checks.</summary>
        internal void ActivateKeyboardControl()
        {
            switch (_game._keyboardFocus)
            {
                case 0:
                    _game.OpenNewGamePrompt();
                    break;
                case 1:
                    _game.UndoLastAction();
                    break;
                case 2:
                    _game.ShowHint();
                    break;
                case 3:
                    _game.ToggleSound();
                    break;
                case 4:
                    ToggleOptions();
                    break;
            }
        }

        /// <summary>Selects a focused card stack or moves the existing selection onto the focused legal destination.</summary>
        internal void ActivateKeyboardTarget()
        {
            if (_game._keyboardFocus < 5)
            {
                ActivateKeyboardControl();
                return;
            }

            _game.ClearHint();
            if (_game._keyboardFocus == 5)
            {
                _game.DrawFromStock();
                return;
            }

            NormalizeKeyboardCard();
            var pile = GetKeyboardPile();
            var cardIndex = _game._keyboardFocus >= 11 ? _game._keyboardCardIndex : pile.Count - 1;
            if (_game._selectedPile is not null && !ReferenceEquals(_game._selectedPile, pile) && _game._keyboardFocus != 6)
            {
                var canMove = _game._keyboardFocus >= 11 ? _game.CanMoveToTableau(_game._keyboardFocus - 11)
                    : _game.CanMoveToFoundation(_game._keyboardFocus - 7);
                if (canMove)
                {
                    _game.MoveSelectedCards(pile);
                    NormalizeKeyboardCard();
                }
                else
                {
                    _game.ShowInvalidMove(GetKeyboardBounds().Center);
                }

                return;
            }

            if (cardIndex >= 0 && pile.Count > 0 && pile[cardIndex].FaceUp)
            {
                if (ReferenceEquals(_game._selectedPile, pile) && _game._selectedCardIndex == cardIndex)
                {
                    _game.ClearSelection();
                }
                else
                {
                    _game.Select(pile, cardIndex);
                }
            }
            else
            {
                _game.ShowInvalidMove(GetKeyboardBounds().Center);
            }
        }

        /// <summary>Moves the selected stack, or focused card when nothing is selected, to its first legal foundation.</summary>
        internal void MoveKeyboardCardToFoundation()
        {
            if (_game._selectedPile is null)
            {
                NormalizeKeyboardCard();
                var pile = GetKeyboardPile();
                var cardIndex = _game._keyboardFocus >= 11 ? _game._keyboardCardIndex : (pile?.Count ?? 0) - 1;
                if (pile is null || cardIndex < 0 || pile.Count == 0 || !pile[cardIndex].FaceUp)
                {
                    _game.ShowInvalidMove(GetKeyboardBounds().Center);
                    return;
                }

                _game.Select(pile, cardIndex);
            }

            for (var foundationIndex = 0; foundationIndex < _game._foundations.Count; foundationIndex++)
            {
                if (_game.TryMoveToFoundation(foundationIndex))
                {
                    NormalizeKeyboardCard();
                    return;
                }
            }

            _game.ShowInvalidMove(GetKeyboardBounds().Center);
        }

        /// <summary>Gets the visible bounds of the focused control, slot, or card strip.</summary>
        /// <returns>The focused logical rectangle.</returns>
        internal Rectangle GetKeyboardBounds()
        {
            NormalizeKeyboardCard();
            if (_game._keyboardFocus >= 11)
            {
                var pile = _game._tableau[_game._keyboardFocus - 11];
                var bounds = _game.GetTableauCardBounds(_game._keyboardFocus - 11, _game._keyboardCardIndex);
                if (_game._keyboardCardIndex < pile.Count - 1)
                {
                    bounds.Height = TableauStep;
                }

                return bounds;
            }

            return _game._keyboardFocus switch
            {
                0 => NewGameButtonBounds,
                1 => UndoButtonBounds,
                2 => HintButtonBounds,
                3 => SoundButtonBounds,
                4 => OptionsButtonBounds,
                5 => GetStockBounds(),
                6 => GetWasteBounds(),
                >= 7 and <= 10 => GetFoundationBounds(_game._keyboardFocus - 7),
                _ => Rectangle.Empty
            };
        }

        /// <summary>Describes keyboard focus using rank, suit names, and selection state rather than color alone.</summary>
        /// <returns>The concise footer description.</returns>
        internal string GetKeyboardDescription()
        {
            NormalizeKeyboardCard();
            var label = _game._keyboardFocus switch
            {
                0 => KeyboardDescriptionNewGame,
                1 => _game._undoHistory.Count == 0 || _game._cardTransitions.Count > 0
                    ? KeyboardDescriptionUndoUnavailable : KeyboardDescriptionUndoAvailable,
                2 => _game._hasWon || _game._cardTransitions.Count > 0
                    ? KeyboardDescriptionHintUnavailable : KeyboardDescriptionHintAvailable,
                3 => _game._audioUnavailable
                    ? KeyboardDescriptionSoundUnavailable
                    : _game._soundEnabled ? KeyboardDescriptionSoundOn : KeyboardDescriptionSoundOff,
                4 => KeyboardDescriptionOptions,
                5 => _game._stock.Count > 0
                    ? $"{KeyboardDescriptionStockPrefix}  {_game._stock.Count} {KeyboardDescriptionStockDrawSuffix}"
                    : _game._waste.Count > 0 ? KeyboardDescriptionStockEmptyRecycle : KeyboardDescriptionStockAndWasteEmpty,
                6 => KeyboardDescriptionWaste,
                >= 7 and <= 10 => $"{KeyboardDescriptionFoundationPrefix} {_game._keyboardFocus - 6}",
                _ => $"{KeyboardDescriptionTableauPrefix} {_game._keyboardFocus - 10}"
            };
            var pile = GetKeyboardPile();
            if (pile is not null)
            {
                if (pile.Count == 0)
                {
                    label += KeyboardDescriptionEmptySuffix;
                }
                else
                {
                    var card = pile[_game._keyboardFocus >= 11 ? _game._keyboardCardIndex : pile.Count - 1];
                    var rank = SolitaireCardLabels.GetLongRankLabel(card.Rank);
                    label += card.FaceUp ? $"  {rank} OF {card.Suit}" : KeyboardDescriptionFaceDownSuffix;
                }
            }

            if (_game._selectedPile is not null)
            {
                if (_game._keyboardFocus >= 7 && !ReferenceEquals(_game._selectedPile, pile))
                {
                    var canMove = _game._keyboardFocus >= 11 ? _game.CanMoveToTableau(_game._keyboardFocus - 11)
                        : _game.CanMoveToFoundation(_game._keyboardFocus - 7);
                    label += canMove ? KeyboardDescriptionLegalTargetSuffix : KeyboardDescriptionInvalidTargetSuffix;
                }

                label += $"{KeyboardDescriptionSelectedPrefix} {_game._selectedPile.Count - _game._selectedCardIndex} {KeyboardDescriptionSelectedSuffix}";
            }

            return label;
        }
    }
}
