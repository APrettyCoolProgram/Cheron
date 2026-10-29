// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
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

        /// <summary>Draws the proportionally scaled board, cards, controls, and win message.</summary>
        /// <param name="gameTime">The host-supplied timing information; animation progress is advanced during updates.</param>
        /// <param name="graphicsDevice">The borrowed, host-owned graphics device.</param>
        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(BoardClearColor);
            var previousScissor = graphicsDevice.ScissorRectangle;
            var previousRasterizer = graphicsDevice.RasterizerState;
            graphicsDevice.ScissorRectangle = _game._viewport.ScissorBounds;

            _game.BeginBoardDrawing();
            if (_game._highContrast)
            {
                _game.Fill(GetBoardBounds(), Color.Black);
            }
            else
            {
                _game._spriteBatch.Draw(_game._artwork.Background, GetBoardBounds(), Color.White);
            }

            _game.Fill(GetTopBarBounds(), _game._highContrast ? Color.Black : TopBarTint);
            _game.Fill(GetTopBarDividerBounds(), BoardSeparatorLine);
            DrawToolbar();

            var stockBounds = GetStockBounds();

            DrawPile(_game._stock, stockBounds, faceDown: true);

            if (_game._stock.Count == 0)
            {
                _game.DrawText(BoardDealLabel, _game._largeText ? DealLabelLargeTextX : DealLabelX, DealLabelY,
                    _game.SmallTextScale, BoardDealPromptText);
                _game.DrawText(BoardAgainLabel, _game._largeText ? AgainLabelLargeTextX : AgainLabelX,
                    _game._largeText ? AgainLabelLargeTextY : AgainLabelY, _game.SmallTextScale, BoardDealPromptText);
            }

            DrawPile(_game._waste, GetWasteBounds());
            _game.DrawText(BoardStockLabel, _game._largeText ? StockLabelLargeTextX : StockLabelX, PileHeadingY,
                _game.SmallTextScale, BoardHeadingText);
            _game.DrawText(BoardWasteLabel, _game._largeText ? WasteLabelLargeTextX : WasteLabelX, PileHeadingY,
                _game.SmallTextScale, BoardHeadingText);

            for (var foundationIndex = 0; foundationIndex < _game._foundations.Count; foundationIndex++)
            {
                var foundationBounds = GetFoundationBounds(foundationIndex);

                DrawPile(_game._foundations[foundationIndex], foundationBounds);

                if (_game._foundations[foundationIndex].Count == 0)
                {
                    _game.DrawText(BoardFoundationAceLabel, foundationBounds.X + FoundationAceOffsetX,
                        foundationBounds.Y + FoundationAceOffsetY, 2, BoardPlaceholderText);
                    _game.DrawText(_game._largeText ? BoardFoundationToKingCompactLabel : BoardFoundationToKingLabel,
                        foundationBounds.X + FoundationInstructionOffsetX,
                        foundationBounds.Y + FoundationInstructionOffsetY, _game.SmallTextScale, BoardPlaceholderText);
                }
            }

            _game.DrawText(BoardFoundationsLabel, _game._largeText ? FoundationsLabelLargeTextX : FoundationsLabelX,
                PileHeadingY, _game.SmallTextScale, BoardHeadingText);

            for (var columnIndex = 0; columnIndex < _game._tableau.Count; columnIndex++)
            {
                var tableauPile = _game._tableau[columnIndex];
                var cardY = TableauTop;

                if (tableauPile.Count == 0)
                {
                    var slotBounds = new Rectangle(FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);
                    DrawSlot(slotBounds);
                    _game.DrawText(BoardTableauKingLabel, slotBounds.X + TableauKingOffsetX,
                        slotBounds.Y + TableauKingOffsetY, 2, BoardPlaceholderText);
                }

                for (var cardIndex = 0; cardIndex < tableauPile.Count; cardIndex++)
                {
                    var card = tableauPile[cardIndex];
                    var cardBounds = new Rectangle(FirstColumnX + columnIndex * ColumnStep, cardY, CardWidth, CardHeight);

                    if (!_game.IsDraggedCard(tableauPile, cardIndex))
                    {
                        DrawCard(card, cardBounds, _game.IsSelected(tableauPile, cardIndex));
                    }

                    cardY += card.FaceUp ? TableauStep : TableauFaceDownStep;
                }
            }

            DrawInteractionFeedback();
            DrawCardTransitions();
            DrawInvalidFeedback();
            DrawWinCelebration();

            _game.Fill(GetFooterBounds(), _game._highContrast ? Color.Black : FooterTint);
            _game.Fill(GetFooterDividerBounds(), BoardSeparatorLine);
            DrawAccessibleFooter();

            if (_game._hasWon && _game._cardTransitions.Count == 0)
            {
                _game._spriteBatch.Draw(_game._artwork.Banner, WinBannerBounds, Color.White);
                _game.DrawText(BoardWinLabel, WinTextX, WinTextY, 3, GoldHighlight);
            }

            if (_game._keyboardFocusVisible && !_game._optionsOpen)
            {
                DrawFocusCorners(_game.GetKeyboardBounds());
            }

            if (_game._optionsOpen)
            {
                DrawAccessibilityOptions();
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
            _game.DrawText(ToolbarTitleLabel, ToolbarTitleX, ToolbarTitleY, 2, ToolbarPrimaryText);
            _game.DrawText(_game._largeText ? ToolbarShortcutsCompactLabel : ToolbarShortcutsLabel, ToolbarShortcutsX,
                ToolbarShortcutsY, _game.SmallTextScale, ToolbarSecondaryText);
            _game.DrawText(ToolbarMovesLabel, ToolbarMovesX, ToolbarStatsCaptionY, _game.SmallTextScale,
                ToolbarSecondaryText);
            _game.DrawText(_game._moveCount.ToString(), ToolbarMovesX, ToolbarStatsValueY, 2, ToolbarPrimaryText);
            _game.DrawText(ToolbarTimeLabel, ToolbarTimeX, ToolbarStatsCaptionY, _game.SmallTextScale,
                ToolbarSecondaryText);
            var totalSeconds = (long)_game._playingTime.TotalSeconds;
            _game.DrawText($"{totalSeconds / 60:00}M {totalSeconds % 60:00}S", ToolbarTimeX, ToolbarStatsValueY,
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
                _game.Fill(bounds, enabled ? Color.White : ButtonDisabledHighContrastFill);
                DrawTargetOutline(bounds, Color.White);
            }
            else
            {
                _game._spriteBatch.Draw(texture, bounds, enabled ? Color.White : ButtonDisabledTint);
            }

            var ink = _game._highContrast ? (enabled ? Color.Black : Color.White)
                : enabled ? ButtonEnabledInk : ButtonDisabledInk;
            _game.DrawText(label, bounds.X + (bounds.Width - label.Length * 12 + 2) / 2, bounds.Y + 15,
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
                _game.Fill(new Rectangle((int)particleX, (int)particleY, ParticleWidth, ParticleHeight),
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
                _game.BeginBoardDrawing(Matrix.CreateTranslation(-centerX, 0, 0) *
                    Matrix.CreateScale(widthScale, 1, 1) * Matrix.CreateTranslation(centerX, 0, 0));
                DrawCardArtwork(entry.Key, bounds, false, !faceUp);
                _game._spriteBatch.End();
                _game.BeginBoardDrawing();
            }
        }

        /// <summary>Draws legal destinations, hover feedback, and the floating stack in that order.</summary>
        private void DrawInteractionFeedback()
        {
            if (_game._cardTransitions.Count > 0)
            {
                return;
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
            _game.Fill(new Rectangle(bounds.X, bounds.Y, bounds.Width, TargetOutlineThickness), color);
            _game.Fill(new Rectangle(bounds.X, bounds.Bottom - TargetOutlineThickness, bounds.Width,
                TargetOutlineThickness), color);
            _game.Fill(new Rectangle(bounds.X, bounds.Y, TargetOutlineThickness, bounds.Height), color);
            _game.Fill(new Rectangle(bounds.Right - TargetOutlineThickness, bounds.Y, TargetOutlineThickness,
                bounds.Height), color);
        }

        /// <summary>Draws the settings panel with large, always-readable labels and explicit on/off states.</summary>
        private void DrawAccessibilityOptions()
        {
            _game.Fill(GetBoardBounds(), Color.Black * 0.75f);
            var panelBounds = GetPanelBounds();
            _game.Fill(panelBounds, Color.Black);
            DrawTargetOutline(panelBounds, Color.White);
            _game.DrawText(AccessibilityTitle, FirstRowX, TitleY, 3, Color.White);
            for (var index = 0; index < OptionCount; index++)
            {
                var bounds = GetOptionBounds(index);
                _game.Fill(bounds, OptionsRowFill);
                var label = GetOptionLabel(index, _game._largeText, _game._highContrast, _game._reducedMotion);
                _game.DrawText(label, bounds.X + LabelPaddingX, bounds.Y + LabelPaddingY, 2, Color.White);
                if (index == _game._optionIndex)
                {
                    DrawFocusCorners(bounds);
                }
            }

            _game.DrawText(AccessibilityHelpNavigation, FirstRowX, HelpStartY, 2, Color.White);
            _game.DrawText(AccessibilityHelpPersistence, FirstRowX, HelpStartY + HelpLineStep, 2, Color.White);
            _game.DrawText(AccessibilityHelpCloseAndExit, FirstRowX, HelpStartY + HelpLineStep * 2, 2, Color.White);
            _game.DrawText(AccessibilityHelpShortcutsDisabled, FirstRowX, HelpStartY + HelpLineStep * 3, 2, Color.White);
        }

        /// <summary>Draws an empty pile using either the themed texture or a solid high-contrast surface.</summary>
        /// <param name="bounds">The logical pile bounds.</param>
        private void DrawSlot(Rectangle bounds)
        {
            if (_game._highContrast)
            {
                _game.Fill(bounds, Color.Black);
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
            _game.DrawText(message, FooterTextX, FooterStatusY, _game.SmallTextScale, FooterPrimaryText);
            var instructions = _game._keyboardFocusVisible
                ? FooterKeyboardInstructions
                : _game._audioUnavailable ? FooterSoundUnavailableInstructions
                : FooterDefaultInstructions;
            _game.DrawText(instructions, FooterTextX, FooterInstructionsY, _game.SmallTextScale,
                ToolbarSecondaryText);
            DrawToolbarButton(OptionsButtonBounds, FooterOptionsLabel, true);
        }

        /// <summary>Draws a static corner-shaped focus indicator distinct from continuous selection outlines.</summary>
        /// <param name="bounds">The control or exposed card region to mark.</param>
        private void DrawFocusCorners(Rectangle bounds)
        {
            bounds.Inflate(FocusCornerInflate, FocusCornerInflate);
            _game.Fill(new Rectangle(bounds.X, bounds.Y, FocusCornerLength, FocusCornerThickness), Color.White);
            _game.Fill(new Rectangle(bounds.Right - FocusCornerLength, bounds.Y, FocusCornerLength,
                FocusCornerThickness), Color.White);
            _game.Fill(new Rectangle(bounds.X, bounds.Bottom - FocusCornerThickness, FocusCornerLength,
                FocusCornerThickness), Color.White);
            _game.Fill(new Rectangle(bounds.Right - FocusCornerLength, bounds.Bottom - FocusCornerThickness,
                FocusCornerLength, FocusCornerThickness), Color.White);
            _game.Fill(new Rectangle(bounds.X, bounds.Y, FocusCornerThickness, FocusCornerLength), Color.White);
            _game.Fill(new Rectangle(bounds.Right - FocusCornerThickness, bounds.Y, FocusCornerThickness,
                FocusCornerLength), Color.White);
            _game.Fill(new Rectangle(bounds.X, bounds.Bottom - FocusCornerLength, FocusCornerThickness,
                FocusCornerLength), Color.White);
            _game.Fill(new Rectangle(bounds.Right - FocusCornerThickness, bounds.Bottom - FocusCornerLength,
                FocusCornerThickness, FocusCornerLength), Color.White);
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
                _game.Fill(bounds, faceDown ? Color.Black : Color.White);
                DrawTargetOutline(bounds, isSelected && !faceDown ? Color.Yellow : Color.White);
                if (faceDown)
                {
                    _game.DrawText(CardBackLabel, bounds.X + CardBackLabelOffsetX, bounds.Y + CardBackLabelOffsetY,
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
            _game.DrawText(rankLabel, bounds.X + CardRankTopOffsetX,
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
            _game.DrawText(rankLabel, bounds.Right - (rankLabel.Length * 6 * rankScale) - CardRankBottomPaddingX,
                bounds.Bottom - (7 * rankScale) - CardRankBottomPaddingY, rankScale, suitColor, preserveColor: true);
        }
    }
}
