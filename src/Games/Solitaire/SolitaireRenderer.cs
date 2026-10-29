// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using Cheron.Games.Solitaire.FreeCell;
using Cheron.Games.Solitaire.Klondike;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Cheron.Games.Solitaire.SolitaireAccessibilityOptions;
using static Cheron.Games.Solitaire.SolitaireCelebrationConstants;
using static Cheron.Games.Solitaire.SolitaireColors;
using static Cheron.Games.Solitaire.SolitaireLayout;
using static Cheron.Games.Solitaire.SolitaireOptionsLayout;
using static Cheron.Games.Solitaire.SolitaireTextConstants;
using static Cheron.Games.Solitaire.SolitaireTimingConstants;

namespace Cheron.Games.Solitaire;

internal sealed partial class SolitaireGame
{
    /// <summary>Renders the Solitaire board and toolbar using the owning game state.</summary>
    private sealed class SolitaireRenderer(SolitaireGame game)
    {
        private readonly SolitaireGame _game = game;

        /// <summary>Fills a rectangle using the module's single-pixel texture.</summary>
        /// <param name="bounds">The rectangle to fill in logical board coordinates.</param>
        /// <param name="color">The fill color.</param>
        private void Fill(Rectangle bounds, Color color) => _game._spriteBatch.Draw(_game._pixelTexture, bounds, color);

        /// <summary>Draws uppercase bitmap text, leaving blank character widths for unsupported characters.</summary>
        /// <param name="text">The text to draw; letters, digits, spaces, and exclamation marks are supported.</param>
        /// <param name="textX">The starting horizontal position in logical board coordinates.</param>
        /// <param name="textY">The starting vertical position in logical board coordinates.</param>
        /// <param name="scale">The size of each glyph pixel in logical board units.</param>
        /// <param name="color">The normal-theme text color.</param>
        /// <param name="preserveColor">Whether to retain contrasting card or button ink in high-contrast mode.</param>
        private void DrawText(string text, int textX, int textY, int scale, Color color, bool preserveColor = false)
        {
            SolitaireBitmapFont.DrawText(_game._spriteBatch, _game._glyphTexture, _game._highContrast,
                text, textX, textY, scale, color, preserveColor);
        }

        /// <summary>Starts a clipped sprite batch, composing local animation transforms before viewport scaling.</summary>
        /// <param name="localTransform">The optional board-space transform used for a card flip.</param>
        private void BeginBoardDrawing(Matrix? localTransform = null)
        {
            _game._spriteBatch.Begin(samplerState: SamplerState.LinearClamp,
                rasterizerState: _game._boardRasterizerState,
                transformMatrix: (localTransform ?? Matrix.Identity) * _game._viewport.Transform);
        }

        /// <summary>Draws the proportionally scaled board, cards, controls, and win message.</summary>
        /// <param name="gameTime">The host-supplied timing information; animation progress is advanced during updates.</param>
        /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(BoardClearColor);
            var previousScissor = graphicsDevice.ScissorRectangle;
            var previousRasterizer = graphicsDevice.RasterizerState;
            graphicsDevice.ScissorRectangle = _game._viewport.ScissorBounds;

            BeginBoardDrawing();
            if (_game._highContrast)
            {
                Fill(GetBoardBounds(), Color.Black);
            }
            else
            {
                _game._spriteBatch.Draw(_game._artwork.Background, GetBoardBounds(), Color.White);
            }

            Fill(GetTopBarBounds(), _game._highContrast ? Color.Black : TopBarTint);
            Fill(GetTopBarDividerBounds(), BoardSeparatorLine);
            DrawToolbar();

            if (_game._variant.IsFreeCell())
            {
                FreeCellBoardRenderer.DrawTopRow(_game._freeCells, _game.SmallTextScale, DrawPile, DrawText);
            }
            else
            {
                KlondikeBoardRenderer.DrawTopRow(_game._stock, _game._waste, _game._largeText, _game.SmallTextScale,
                    DrawPile, DrawText);
            }

            for (var foundationIndex = 0; foundationIndex < _game._foundations.Count; foundationIndex++)
            {
                var foundationBounds = GetFoundationBounds(foundationIndex);

                DrawPile(_game._foundations[foundationIndex], foundationBounds);

                if (_game._foundations[foundationIndex].Count == 0)
                {
                    DrawText(BoardFoundationAceLabel, foundationBounds.X + FoundationAceOffsetX,
                        foundationBounds.Y + FoundationAceOffsetY, 2, BoardPlaceholderText);
                    DrawText(_game._largeText ? BoardFoundationToKingCompactLabel : BoardFoundationToKingLabel,
                        foundationBounds.X + FoundationInstructionOffsetX,
                        foundationBounds.Y + FoundationInstructionOffsetY, _game.SmallTextScale, BoardPlaceholderText);
                }
            }

            DrawText(BoardFoundationsLabel, _game._largeText ? FoundationsLabelLargeTextX : FoundationsLabelX,
                PileHeadingY, _game.SmallTextScale, BoardHeadingText);

            for (var columnIndex = 0; columnIndex < _game._tableau.Count; columnIndex++)
            {
                var tableauPile = _game._tableau[columnIndex];

                if (tableauPile.Count == 0)
                {
                    var slotBounds = _game.GetTableauCardBounds(columnIndex, 0);
                    DrawSlot(slotBounds);
                    DrawText(BoardTableauKingLabel, slotBounds.X + TableauKingOffsetX,
                        slotBounds.Y + TableauKingOffsetY, 2, BoardPlaceholderText);
                }

                for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
                {
                    var card = tableauPile[cardIndex];
                    var cardBounds = _game.GetTableauCardBounds(columnIndex, cardIndex);

                    if (!_game.IsDraggedCard(tableauPile, cardIndex))
                    {
                        DrawCard(card, cardBounds, _game.IsSelected(tableauPile, cardIndex));
                    }
                }
            }

            DrawInteractionFeedback();
            DrawCardTransitions();
            DrawInvalidFeedback();
            DrawWinCelebration();

            Fill(GetFooterBounds(), _game._highContrast ? Color.Black : FooterTint);
            Fill(GetFooterDividerBounds(), BoardSeparatorLine);
            DrawAccessibleFooter();

            if (_game._hasWon && _game._cardTransitions.Count == 0)
            {
                _game._spriteBatch.Draw(_game._artwork.Banner, WinBannerBounds, Color.White);
                DrawText(BoardWinLabel, WinTextX, WinTextY, 3, GoldHighlight);
            }

            if (_game._keyboardFocusVisible && !_game._optionsOpen)
            {
                DrawFocusCorners(_game.GetKeyboardBounds());
            }

            if (_game._optionsOpen)
            {
                DrawAccessibilityOptions();
            }

            if (_game.IsNewGamePromptOpen)
            {
                DrawNewGameVariantPrompt();
            }

            if (_game._smokePassOverlayOpen)
            {
                DrawSmokePassOverlay();
            }

            _game._spriteBatch.End();
            graphicsDevice.ScissorRectangle = previousScissor;
            graphicsDevice.RasterizerState = previousRasterizer;
        }

        /// <summary>Draws the toolbar and live statistics, including disabled-state and shortcut labels.</summary>
        private void DrawToolbar()
        {
            var canUseBoard = _game._cardTransitions.Count == 0 && !_game._isDragging;
            DrawToolbarButton(NewGameButtonBounds, NewGameLabel, true);
            DrawToolbarButton(UndoButtonBounds, UndoLabel, canUseBoard && _game._undoHistory.Count > 0);
            DrawToolbarButton(HintButtonBounds, HintLabel, canUseBoard && !_game._hasWon);
            DrawToolbarButton(SoundButtonBounds,
                _game._audioUnavailable ? AudioUnavailableLabel : _game._soundEnabled ? SoundEnabledLabel : SoundDisabledLabel,
                !_game._audioUnavailable);
            DrawText(ToolbarTitleLabel, ToolbarTitleX, ToolbarTitleY, 2, ToolbarPrimaryText);
            DrawText(_game._largeText ? ToolbarShortcutsCompactLabel : ToolbarShortcutsLabel, ToolbarShortcutsX,
                ToolbarShortcutsY, _game.SmallTextScale, ToolbarSecondaryText);
            DrawText(ToolbarMovesLabel, ToolbarMovesX, ToolbarStatsCaptionY, _game.SmallTextScale,
                ToolbarSecondaryText);
            DrawText(_game._moveCount.ToString(), ToolbarMovesX, ToolbarStatsValueY, 2, ToolbarPrimaryText);
            DrawText(ToolbarTimeLabel, ToolbarTimeX, ToolbarStatsCaptionY, _game.SmallTextScale,
                ToolbarSecondaryText);
            var totalSeconds = (long)_game._playingTime.TotalSeconds;
            DrawText($"{totalSeconds / 60:00}M {totalSeconds % 60:00}S", ToolbarTimeX, ToolbarStatsValueY,
                _game.SmallTextScale, ToolbarPrimaryText);
        }

        /// <summary>Draws one themed button with hover feedback only while enabled.</summary>
        /// <param name="bounds">The shared drawing and hit-test bounds.</param>
        /// <param name="label">The button text.</param>
        /// <param name="enabled">Whether the button's action is available.</param>
        internal void DrawToolbarButton(Rectangle bounds, string label, bool enabled)
        {
            var isHovered = enabled && bounds.Contains(_game._pointerPosition);
            var texture = isHovered ? _game._artwork.ButtonHover : _game._artwork.Button;
            if (_game._highContrast)
            {
                Fill(bounds, enabled ? Color.White : ButtonDisabledHighContrastFill);
                DrawTargetOutline(bounds, Color.White);
            }
            else
            {
                _game._spriteBatch.Draw(texture, bounds, enabled ? Color.White : ButtonDisabledTint);
            }

            var ink = _game._highContrast ? (enabled ? Color.Black : Color.White)
                : enabled ? ButtonEnabledInk : ButtonDisabledInk;
            DrawText(label, bounds.X + (bounds.Width - label.Length * 12 + 2) / 2, bounds.Y + 15,
                2, ink, preserveColor: true);
        }

        /// <summary>Draws a theme-aware rejected-move outline, kept static when reduced motion is enabled.</summary>
        private void DrawInvalidFeedback()
        {
            if (_game._invalidTimeRemaining > TimeSpan.Zero && _game._invalidBounds != Rectangle.Empty)
            {
                var opacity = _game._reducedMotion ? 1
                    : (float)(_game._invalidTimeRemaining.TotalSeconds / InvalidFeedbackDuration.TotalSeconds);
                var color = _game._highContrast ? Color.White : InvalidFeedbackColor;
                DrawTargetOutline(_game._invalidBounds, color * Math.Min(1, opacity * 2));
            }
        }

        /// <summary>Draws a small, finite gold-and-mint burst without obscuring controls or changing card ownership.</summary>
        private void DrawWinCelebration()
        {
            if (_game._reducedMotion || !_game._hasWon || !_game._winFeedbackShown ||
                _game._celebrationElapsed >= CelebrationDuration)
            {
                return;
            }

            var elapsedSeconds = (float)_game._celebrationElapsed.TotalSeconds;
            var opacity = Math.Clamp((float)CelebrationDuration.TotalSeconds - elapsedSeconds, 0, 1);
            for (var particleIndex = 0; particleIndex < ParticleCount; particleIndex++)
            {
                var horizontalDirection = particleIndex % 2 == 0 ? -1 : 1;
                var horizontalSpeed = BaseHorizontalSpeed + particleIndex * HorizontalSpeedStep;
                var particleX = OriginX + horizontalDirection * (BaseHorizontalOffset + horizontalSpeed * elapsedSeconds);
                var particleY = OriginY - (BaseRise + particleIndex % RiseVariationCycle * RiseVariationStep) * elapsedSeconds +
                    VerticalAcceleration * elapsedSeconds * elapsedSeconds;
                if (particleX < MinX || particleX > MaxX || particleY < MinY || particleY > MaxY)
                {
                    continue;
                }

                var particleColor = particleIndex % 3 == 0 ? MintHighlight : GoldHighlight;
                Fill(new Rectangle((int)particleX, (int)particleY, ParticleWidth, ParticleHeight),
                    particleColor * opacity);
            }
        }

        /// <summary>Draws moving cards above the board, compressing their complete artwork horizontally during flips.</summary>
        private void DrawCardTransitions()
        {
            foreach (var entry in _game._cardTransitions)
            {
                var transition = entry.Value;
                if (_game._animationElapsed < transition.Delay)
                {
                    continue;
                }

                var progress = Math.Clamp((_game._animationElapsed - transition.Delay) / MoveDuration, 0, 1);
                var easedProgress = progress * progress * (3 - 2 * progress);
                var position = Vector2.Lerp(transition.Start.Bounds.Location.ToVector2(),
                    transition.End.Bounds.Location.ToVector2(), easedProgress);
                var bounds = new Rectangle((int)MathF.Round(position.X), (int)MathF.Round(position.Y), CardWidth,
                    CardHeight);
                var isFlipping = transition.Start.FaceUp != transition.End.FaceUp;
                var faceUp = progress < 0.5f ? transition.Start.FaceUp : transition.End.FaceUp;
                if (!isFlipping || progress >= 1)
                {
                    DrawCardArtwork(entry.Key, bounds, false, !faceUp);
                    continue;
                }

                var widthScale = Math.Max(0.02f, Math.Abs(1 - 2 * progress));
                var centerX = bounds.Center.X;
                _game._spriteBatch.End();
                BeginBoardDrawing(Matrix.CreateTranslation(-centerX, 0, 0) *
                    Matrix.CreateScale(widthScale, 1, 1) * Matrix.CreateTranslation(centerX, 0, 0));
                DrawCardArtwork(entry.Key, bounds, false, !faceUp);
                _game._spriteBatch.End();
                BeginBoardDrawing();
            }
        }

        /// <summary>Draws legal destinations, hover feedback, and the floating stack in that order.</summary>
        private void DrawInteractionFeedback()
        {
            if (_game._cardTransitions.Count > 0)
            {
                return;
            }

            if (_game._variant.IsFreeCell())
            {
                FreeCellBoardRenderer.DrawLegalCellTargets(_game._freeCells.Count, _game.CanMoveToFreeCell,
                    bounds => DrawTargetOutline(bounds, MintHighlight));
            }

            for (var foundationIndex = 0; foundationIndex < _game._foundations.Count; foundationIndex++)
            {
                if (_game.CanMoveToFoundation(foundationIndex))
                {
                    DrawTargetOutline(GetFoundationBounds(foundationIndex), MintHighlight);
                }
            }

            for (var columnIndex = 0; columnIndex < _game._tableau.Count; columnIndex++)
            {
                if (_game.CanMoveToTableau(columnIndex))
                {
                    var pile = _game._tableau[columnIndex];
                    var bounds = _game.GetTableauCardBounds(columnIndex, Math.Max(0, pile.Count - 1));
                    DrawTargetOutline(bounds, MintHighlight);
                }
            }

            if (!_game._isDragging &&
                _game.TryGetCardAt(_game._pointerPosition, out var hoveredPile, out var hoveredIndex, out var hoveredBounds))
            {
                if (_game._tableau.Contains(hoveredPile) && hoveredIndex < hoveredPile.Count - 1)
                {
                    hoveredBounds.Height = TableauStep;
                }

                DrawTargetOutline(hoveredBounds, HoverOutline);
            }

            DrawHintFeedback();

            if (_game._isDragging && _game._selectedPile is not null)
            {
                var position = _game._pointerPosition - _game._dragOffset;
                for (var cardIndex = _game._selectedCardIndex; cardIndex < _game._selectedPile.Count; cardIndex++)
                {
                    DrawCard(_game._selectedPile[cardIndex],
                        new Rectangle(position.X, position.Y, CardWidth, CardHeight),
                        cardIndex == _game._selectedCardIndex);
                    position.Y += TableauStep;
                }
            }
        }

        /// <summary>Draws gold outlines around the suggested source and destination while a hint is active.</summary>
        private void DrawHintFeedback()
        {
            if (_game._hintTimeRemaining <= TimeSpan.Zero)
            {
                return;
            }

            var color = GoldHighlight;
            if (_game._hintSource != Rectangle.Empty)
            {
                DrawTargetOutline(_game._hintSource, color);
            }

            if (_game._hintDestination != Rectangle.Empty)
            {
                DrawTargetOutline(_game._hintDestination, color);
            }
        }

        /// <summary>Draws a thin, high-contrast outline just outside a card or drop slot.</summary>
        /// <param name="bounds">The bounds to outline.</param>
        /// <param name="color">The feedback color.</param>
        private void DrawTargetOutline(Rectangle bounds, Color color)
        {
            bounds.Inflate(TargetOutlineInflate, TargetOutlineInflate);
            Fill(new Rectangle(bounds.X, bounds.Y, bounds.Width, TargetOutlineThickness), color);
            Fill(new Rectangle(bounds.X, bounds.Bottom - TargetOutlineThickness, bounds.Width,
                TargetOutlineThickness), color);
            Fill(new Rectangle(bounds.X, bounds.Y, TargetOutlineThickness, bounds.Height), color);
            Fill(new Rectangle(bounds.Right - TargetOutlineThickness, bounds.Y, TargetOutlineThickness,
                bounds.Height), color);
        }

        /// <summary>Draws the settings panel with large, always-readable labels and explicit on/off states.</summary>
        private void DrawAccessibilityOptions()
        {
            Fill(GetBoardBounds(), Color.Black * 0.75f);
            var panelBounds = GetPanelBounds();
            Fill(panelBounds, Color.Black);
            DrawTargetOutline(panelBounds, Color.White);
            DrawText(AccessibilityTitle, FirstRowX, TitleY, 3, Color.White);
            for (var index = 0; index < OptionCount; index++)
            {
                var bounds = GetOptionBounds(index);
                Fill(bounds, OptionsRowFill);
                var label = GetOptionLabel(index, _game._largeText, _game._highContrast, _game._reducedMotion);
                DrawText(label, bounds.X + LabelPaddingX, bounds.Y + LabelPaddingY, 2, Color.White);
                if (index == _game._optionIndex)
                {
                    DrawFocusCorners(bounds);
                }
            }

            DrawText(AccessibilityHelpNavigation, FirstRowX, HelpStartY, 2, Color.White);
            DrawText(AccessibilityHelpPersistence, FirstRowX, HelpStartY + HelpLineStep, 2, Color.White);
            DrawText(AccessibilityHelpCloseAndExit, FirstRowX, HelpStartY + HelpLineStep * 2, 2, Color.White);
            DrawText(AccessibilityHelpShortcutsDisabled, FirstRowX, HelpStartY + HelpLineStep * 3, 2, Color.White);
        }

        /// <summary>Draws an empty pile using either the themed texture or a solid high-contrast surface.</summary>
        /// <param name="bounds">The logical pile bounds.</param>
        private void DrawSlot(Rectangle bounds)
        {
            if (_game._highContrast)
            {
                Fill(bounds, Color.Black);
                DrawTargetOutline(bounds, Color.White);
            }
            else
            {
                _game._spriteBatch.Draw(_game._artwork.Slot, bounds, Color.White);
            }
        }

        /// <summary>Draws footer status and discoverable access to settings without overlapping larger text.</summary>
        private void DrawAccessibleFooter()
        {
            var message = _game._invalidTimeRemaining > TimeSpan.Zero
                ? FooterInvalidMoveMessage
                : _game._hintTimeRemaining > TimeSpan.Zero ? _game._hintMessage
                : _game._keyboardFocusVisible ? _game.GetKeyboardDescription() : FooterDefaultMessage;
            DrawText(message, FooterTextX, FooterStatusY, _game.SmallTextScale, FooterPrimaryText);
            var instructions = _game._keyboardFocusVisible
                ? FooterKeyboardInstructions
                : _game._audioUnavailable ? FooterSoundUnavailableInstructions
                : FooterDefaultInstructions;
            DrawText(instructions, FooterTextX, FooterInstructionsY, _game.SmallTextScale,
                ToolbarSecondaryText);
            DrawToolbarButton(OptionsButtonBounds, FooterOptionsLabel, true);
        }

        /// <summary>Draws a static corner-shaped focus indicator distinct from continuous selection outlines.</summary>
        /// <param name="bounds">The control or exposed card region to mark.</param>
        private void DrawFocusCorners(Rectangle bounds)
        {
            bounds.Inflate(FocusCornerInflate, FocusCornerInflate);
            Fill(new Rectangle(bounds.X, bounds.Y, FocusCornerLength, FocusCornerThickness), Color.White);
            Fill(new Rectangle(bounds.Right - FocusCornerLength, bounds.Y, FocusCornerLength,
                FocusCornerThickness), Color.White);
            Fill(new Rectangle(bounds.X, bounds.Bottom - FocusCornerThickness, FocusCornerLength,
                FocusCornerThickness), Color.White);
            Fill(new Rectangle(bounds.Right - FocusCornerLength, bounds.Bottom - FocusCornerThickness,
                FocusCornerLength, FocusCornerThickness), Color.White);
            Fill(new Rectangle(bounds.X, bounds.Y, FocusCornerThickness, FocusCornerLength), Color.White);
            Fill(new Rectangle(bounds.Right - FocusCornerThickness, bounds.Y, FocusCornerThickness,
                FocusCornerLength), Color.White);
            Fill(new Rectangle(bounds.X, bounds.Bottom - FocusCornerLength, FocusCornerThickness,
                FocusCornerLength), Color.White);
            Fill(new Rectangle(bounds.Right - FocusCornerThickness, bounds.Bottom - FocusCornerLength,
                FocusCornerThickness, FocusCornerLength), Color.White);
        }

        /// <summary>Draws the modal variant picker shown before starting a new deal.</summary>
        private void DrawNewGameVariantPrompt()
        {
            Fill(GetBoardBounds(), Color.Black * 0.7f);
            var panelBounds = GetNewGamePromptPanelBounds();
            Fill(panelBounds, Color.Black);
            DrawTargetOutline(panelBounds, Color.White);

            DrawText(NewGameVariantPromptTitle, panelBounds.X + 110, panelBounds.Y + 22, 2, GoldHighlight);

            var klondikeBounds = GetNewGamePromptKlondikeBounds();
            var freeCellBounds = GetNewGamePromptFreeCellBounds();
            var confirmBounds = GetNewGamePromptConfirmBounds();
            var cancelBounds = GetNewGamePromptCancelBounds();

            var klondikeSelected = _game._newGamePromptVariant == SolitaireVariant.Klondike;
            var freeCellSelected = _game._newGamePromptVariant == SolitaireVariant.FreeCell;

            Fill(klondikeBounds, klondikeSelected ? GoldHighlight : OptionsRowFill);
            Fill(freeCellBounds, freeCellSelected ? GoldHighlight : OptionsRowFill);
            DrawTargetOutline(klondikeBounds, Color.White);
            DrawTargetOutline(freeCellBounds, Color.White);

            DrawText(NewGameVariantKlondikeLabel, klondikeBounds.X + 34, klondikeBounds.Y + 18, 2,
                klondikeSelected ? Color.Black : Color.White, preserveColor: true);
            DrawText(NewGameVariantFreeCellLabel, freeCellBounds.X + 40, freeCellBounds.Y + 18, 2,
                freeCellSelected ? Color.Black : Color.White, preserveColor: true);

            DrawFocusCorners(klondikeSelected ? klondikeBounds : freeCellBounds);
            DrawTargetOutline(confirmBounds, Color.White);
            DrawTargetOutline(cancelBounds, Color.White);
            DrawText(NewGameVariantConfirmLabel, confirmBounds.X + 6, confirmBounds.Y + 2, 2, Color.White);
            DrawText(NewGameVariantCancelLabel, cancelBounds.X + 6, cancelBounds.Y + 2, 2, Color.White);
        }

        /// <summary>Draws an in-app manual checklist for smoke-testing UX prompt behavior at runtime.</summary>
        private void DrawSmokePassOverlay()
        {
            Fill(GetBoardBounds(), Color.Black * 0.75f);
            var panelBounds = new Rectangle(120, 120, 860, 560);
            Fill(panelBounds, Color.Black);
            DrawTargetOutline(panelBounds, Color.White);

            DrawText(SmokePassTitle, panelBounds.X + 170, panelBounds.Y + 24, 2, GoldHighlight);
            DrawText(SmokePassStep1, panelBounds.X + 42, panelBounds.Y + 92, 2, Color.White);
            DrawText(SmokePassStep2, panelBounds.X + 42, panelBounds.Y + 142, 2, Color.White);
            DrawText(SmokePassStep3, panelBounds.X + 42, panelBounds.Y + 192, 2, Color.White);
            DrawText(SmokePassStep4, panelBounds.X + 42, panelBounds.Y + 242, 2, Color.White);
            DrawText(SmokePassStep5, panelBounds.X + 42, panelBounds.Y + 292, 2, Color.White);
            DrawText(SmokePassStep6, panelBounds.X + 42, panelBounds.Y + 342, 2, Color.White);
            DrawText(SmokePassFooter, panelBounds.X + 42, panelBounds.Bottom - 58, 2, Color.White);
        }

        /// <summary>Draws a pile's top non-dragged card, or an outlined slot when no card remains visible.</summary>
        /// <param name="pile">The pile to draw.</param>
        /// <param name="bounds">The card or slot bounds in logical board coordinates.</param>
        /// <param name="faceDown">Whether to show the card back regardless of its face-up state.</param>
        private void DrawPile(List<Card> pile, Rectangle bounds, bool faceDown = false)
        {
            var visibleCardIndex = pile.Count - 1;
            if (visibleCardIndex >= 0 && _game.IsDraggedCard(pile, visibleCardIndex))
            {
                visibleCardIndex = _game._selectedCardIndex - 1;
            }

            while (visibleCardIndex >= 0 && _game._cardTransitions.ContainsKey(pile[visibleCardIndex]))
            {
                visibleCardIndex--;
            }

            if (visibleCardIndex >= 0)
            {
                DrawCard(pile[visibleCardIndex], bounds, _game.IsSelected(pile, visibleCardIndex), faceDown);
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
            if (!_game._cardTransitions.ContainsKey(card))
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
            var shadowBounds = new Rectangle(bounds.X + CardShadowOffsetX, bounds.Y + CardShadowOffsetY,
                bounds.Width + CardShadowExpandWidth, bounds.Height + CardShadowExpandHeight);

            _game._spriteBatch.Draw(_game._artwork.CardShadow, shadowBounds, Color.White);

            if (isSelected && !faceDown)
            {
                var highlightOpacity = _game._reducedMotion
                    ? 1
                    : Math.Clamp(_game._selectionElapsed / SelectionHighlightFadeDuration, 0, 1);
                _game._spriteBatch.Draw(_game._artwork.Selection,
                    new Rectangle(bounds.X - CardSelectionInset, bounds.Y - CardSelectionInset,
                        bounds.Width + CardSelectionInset * 2, bounds.Height + CardSelectionInset * 2),
                    Color.White * highlightOpacity);
            }

            if (_game._highContrast)
            {
                Fill(bounds, faceDown ? Color.Black : Color.White);
                DrawTargetOutline(bounds, isSelected && !faceDown ? Color.Yellow : Color.White);
                if (faceDown)
                {
                    DrawText(CardBackLabel, bounds.X + CardBackLabelOffsetX, bounds.Y + CardBackLabelOffsetY,
                        2, Color.White);
                }
            }
            else
            {
                _game._spriteBatch.Draw(faceDown ? _game._artwork.CardBack : _game._artwork.CardFace, bounds,
                    Color.White);
            }

            if (faceDown)
            {
                return;
            }

            var suitColor = _game._highContrast ? (card.IsRed ? CardSuitRedHighContrast : Color.Black)
                : card.IsRed ? CardSuitRed : CardSuitBlack;
            var rankLabel = SolitaireCardLabels.GetShortRankLabel(card.Rank);

            var suitTexture = _game._artwork.Suits[(int)card.Suit];

            var rankScale = _game._largeText ? 3 : 2;
            DrawText(rankLabel, bounds.X + CardRankTopOffsetX,
                bounds.Y + (_game._largeText ? CardRankTopOffsetYLarge : CardRankTopOffsetY), rankScale, suitColor,
                preserveColor: true);
            _game._spriteBatch.Draw(suitTexture,
                new Rectangle(bounds.Right - CardSuitTopRightOffset, bounds.Y + CardSuitTopOffsetY,
                    CardSuitTopSize, CardSuitTopSize), suitColor);
            _game._spriteBatch.Draw(suitTexture,
                new Rectangle(bounds.X + CardSuitCenterOffsetX, bounds.Y + CardSuitCenterOffsetY,
                    CardSuitCenterSize, CardSuitCenterSize), suitColor);
            _game._spriteBatch.Draw(suitTexture,
                new Rectangle(bounds.X + CardSuitBottomOffsetX, bounds.Bottom - CardSuitBottomOffset,
                    CardSuitBottomSize, CardSuitBottomSize), suitColor);
            DrawText(rankLabel, bounds.Right - (rankLabel.Length * 6 * rankScale) - CardRankBottomPaddingX,
                bounds.Bottom - (7 * rankScale) - CardRankBottomPaddingY, rankScale, suitColor, preserveColor: true);
        }
    }
}
