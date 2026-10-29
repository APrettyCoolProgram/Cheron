// 261028_code
// 261028_documentation

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Games.Solitaire;

/// <summary>Provides session-local accessibility options and a keyboard-operable settings panel.</summary>
public sealed partial class SolitaireGame
{
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
            var backwards = IsNewKey(keyboard, Keys.Up) || keyboard.IsKeyDown(Keys.LeftShift) ||
                keyboard.IsKeyDown(Keys.RightShift);
            _optionIndex = (_optionIndex + (backwards ? 3 : 1)) % 4;
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
        Fill(new Rectangle(0, 0, 1100, 800), Color.Black * 0.75f);
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
}
