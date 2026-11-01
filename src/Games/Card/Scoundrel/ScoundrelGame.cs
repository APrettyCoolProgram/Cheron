using System;
using Cheron.Engine;
using global::Cheron.Games.Card.Solitaire;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cheron.Games.Card.Scoundrel;

internal sealed class ScoundrelGame : IGameModule
{
    private const int MaximumHealth = 20;

    private readonly Random _random = new();
    private ScoundrelGameState _gameState = null!;
    private GameViewport _viewport;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private Texture2D _glyphTexture = null!;
    private KeyboardState _previousKeyboardState;

    private int _selectedRoomIndex;
    private int _resolvedInRoom;
    private bool _potionUsedInRoom;
    private bool _avoidedPreviousRoom;
    private string _statusMessage = string.Empty;

    public string Title => "Scoundrel";

    public Point PreferredWindowSize => ScoundrelLayout.BoardSize;

    public void Initialize() => StartNewGame();

    public void SetViewport(GameViewport viewport) => _viewport = viewport;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);
        _glyphTexture = SolitaireBitmapFont.CreateGlyphTexture(graphicsDevice);
    }

    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (IsNewKeyPress(keyboard, Keys.N))
        {
            StartNewGame();
        }

        if (_gameState.GameStatus == ScoundrelGameStatus.Playing)
        {
            if (IsNewKeyPress(keyboard, Keys.D1))
            {
                _selectedRoomIndex = 0;
            }

            if (IsNewKeyPress(keyboard, Keys.D2))
            {
                _selectedRoomIndex = 1;
            }

            if (IsNewKeyPress(keyboard, Keys.D3))
            {
                _selectedRoomIndex = 2;
            }

            if (IsNewKeyPress(keyboard, Keys.D4))
            {
                _selectedRoomIndex = 3;
            }

            if (IsNewKeyPress(keyboard, Keys.A))
            {
                TryAvoidRoom();
            }

            if (IsNewKeyPress(keyboard, Keys.B))
            {
                TryFightSelectedMonsterBarehanded();
            }

            if (IsNewKeyPress(keyboard, Keys.W))
            {
                TryFightSelectedMonsterWithWeapon();
            }

            if (IsNewKeyPress(keyboard, Keys.E))
            {
                TryEquipSelectedWeapon();
            }

            if (IsNewKeyPress(keyboard, Keys.P))
            {
                TryDrinkSelectedPotion();
            }
        }

        _previousKeyboardState = keyboard;
    }

    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(new Color(18, 28, 20));

        _spriteBatch.Begin(transformMatrix: _viewport.Transform, samplerState: SamplerState.PointClamp);

        Fill(ScoundrelLayout.TopBar, new Color(34, 56, 41));
        Fill(ScoundrelLayout.Footer, new Color(28, 44, 34));
        Fill(ScoundrelLayout.DungeonPile, new Color(36, 64, 55));
        Fill(ScoundrelLayout.DiscardPile, new Color(52, 40, 40));
        Fill(ScoundrelLayout.RoomCards, new Color(24, 38, 30));
        Fill(ScoundrelLayout.EquippedWeaponSlot, new Color(52, 50, 34));
        Fill(ScoundrelLayout.HealthPanel, new Color(54, 26, 26));
        Fill(ScoundrelLayout.StatusPanel, new Color(30, 34, 52));
        Fill(ScoundrelLayout.ActionPanel, new Color(46, 46, 30));

        DrawText("SCOUNDREL", 32, 30, 4, new Color(235, 244, 228));
        DrawText($"HEALTH {_gameState.Health}", 965, 275, 2, new Color(252, 215, 215));
        DrawText($"STATUS {_gameState.GameStatus}", 960, 395, 2, new Color(212, 224, 252));
        DrawText($"DUNGEON {_gameState.RoomState.DungeonPile.Count}", 60, 155, 2, new Color(206, 235, 222));
        DrawText($"DISCARD {_gameState.RoomState.DiscardPile.Count}", 60, 275, 2, new Color(242, 214, 214));
        DrawText($"RESOLVED {_resolvedInRoom}/{GetRequiredResolutionsForCurrentRoom()}", 960, 520, 2, new Color(242, 236, 203));
        DrawText("1-4 SELECT  B BAREHAND  W WEAPON  E EQUIP  P POTION  A AVOID  N NEW", 32, 716, 2, new Color(220, 234, 224));
        DrawText(_statusMessage, 32, 744, 2, new Color(220, 234, 224));

        if (_gameState.EquippedWeapon is null)
        {
            DrawText("WEAPON NONE", 960, 155, 2, new Color(245, 236, 188));
        }
        else
        {
            DrawText($"WEAPON {GetCardLabel(_gameState.EquippedWeapon)}", 960, 155, 2, new Color(245, 236, 188));
        }

        if (_gameState.LastWeaponKillValue.HasValue)
        {
            DrawText($"LAST KILL {_gameState.LastWeaponKillValue.Value}", 960, 185, 2, new Color(245, 236, 188));
        }

        DrawRoomCards();
        DrawSelectedCardDetails();

        if (_gameState.GameStatus != ScoundrelGameStatus.Playing)
        {
            DrawTerminalOverlay();
        }

        _spriteBatch.End();
    }

    public void Dispose()
    {
        _glyphTexture?.Dispose();
        _pixel?.Dispose();
        _spriteBatch?.Dispose();
    }

    private void StartNewGame()
    {
        _gameState = new ScoundrelGameState
        {
            Health = MaximumHealth,
            GameStatus = ScoundrelGameStatus.Playing
        };

        _gameState.RoomState.DungeonPile.AddRange(ScoundrelDeckBuilder.CreateShuffledDeck(_random));
        _selectedRoomIndex = 0;
        _resolvedInRoom = 0;
        _potionUsedInRoom = false;
        _avoidedPreviousRoom = false;
        _statusMessage = "1-4 SELECT  B BAREHAND  W WEAPON  E EQUIP  P POTION  A AVOID  N NEW";
        FillRoomToCapacity();
    }

    private bool IsNewKeyPress(KeyboardState keyboard, Keys key)
    {
        return keyboard.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
    }

    private void FillRoomToCapacity()
    {
        while (_gameState.RoomState.Cards.Count < 4 && _gameState.RoomState.DungeonPile.Count > 0)
        {
            var lastIndex = _gameState.RoomState.DungeonPile.Count - 1;
            var card = _gameState.RoomState.DungeonPile[lastIndex];
            _gameState.RoomState.DungeonPile.RemoveAt(lastIndex);
            _gameState.RoomState.Cards.Add(card);
        }

        ClampSelectedIndex();

        if (_gameState.RoomState.Cards.Count == 0 && _gameState.RoomState.DungeonPile.Count == 0)
        {
            _gameState.GameStatus = ScoundrelGameStatus.Won;
            _statusMessage = "YOU ESCAPED THE DUNGEON";
        }
    }

    private int GetRequiredResolutionsForCurrentRoom()
    {
        if (_gameState.RoomState.Cards.Count == 0)
        {
            return 0;
        }

        return _gameState.RoomState.DungeonPile.Count > 0
            ? Math.Max(1, _gameState.RoomState.Cards.Count - 1)
            : _gameState.RoomState.Cards.Count;
    }

    private void CompleteResolutionCycle()
    {
        if (_gameState.GameStatus != ScoundrelGameStatus.Playing)
        {
            return;
        }

        if (_resolvedInRoom < GetRequiredResolutionsForCurrentRoom())
        {
            return;
        }

        _resolvedInRoom = 0;
        _potionUsedInRoom = false;
        FillRoomToCapacity();
    }

    private bool TryAvoidRoom()
    {
        if (_gameState.RoomState.Cards.Count != 4)
        {
            _statusMessage = "CAN ONLY AVOID A FULL ROOM";
            return false;
        }

        if (_resolvedInRoom > 0)
        {
            _statusMessage = "TOO LATE TO AVOID";
            return false;
        }

        if (_avoidedPreviousRoom)
        {
            _statusMessage = "CANNOT AVOID TWICE IN A ROW";
            return false;
        }

        _gameState.RoomState.DungeonPile.InsertRange(0, _gameState.RoomState.Cards);
        _gameState.RoomState.Cards.Clear();
        _avoidedPreviousRoom = true;
        _potionUsedInRoom = false;
        FillRoomToCapacity();
        _statusMessage = "ROOM AVOIDED";
        return true;
    }

    private void TryFightSelectedMonsterBarehanded()
    {
        if (!TryGetSelectedCard(out var card, out var cardIndex))
        {
            return;
        }

        if (card.GetCardType() != ScoundrelCardType.Monster)
        {
            _statusMessage = "SELECT A MONSTER TO FIGHT";
            return;
        }

        var monsterValue = ScoundrelRules.GetValue(card);
        _gameState.Health -= monsterValue;
        ResolveSelectedCard(cardIndex, card, true);

        if (_gameState.Health <= 0)
        {
            _gameState.GameStatus = ScoundrelGameStatus.Lost;
            _statusMessage = "YOU DIED";
            return;
        }

        _statusMessage = $"BAREHANDED: -{monsterValue} HP";
    }

    private void TryFightSelectedMonsterWithWeapon()
    {
        if (!TryGetSelectedCard(out var card, out var cardIndex))
        {
            return;
        }

        if (card.GetCardType() != ScoundrelCardType.Monster)
        {
            _statusMessage = "SELECT A MONSTER TO FIGHT";
            return;
        }

        if (_gameState.EquippedWeapon is null)
        {
            _statusMessage = "NO WEAPON EQUIPPED";
            return;
        }

        var monsterValue = ScoundrelRules.GetValue(card);

        if (!ScoundrelRules.CanWeaponFight(monsterValue, _gameState.LastWeaponKillValue))
        {
            _statusMessage = "WEAPON CANNOT FIGHT THIS MONSTER";
            return;
        }

        var damage = ScoundrelRules.GetMonsterDamage(monsterValue, _gameState.EquippedWeapon);
        _gameState.Health -= damage;
        _gameState.LastWeaponKillValue = monsterValue;
        ResolveSelectedCard(cardIndex, card, true);

        if (_gameState.Health <= 0)
        {
            _gameState.GameStatus = ScoundrelGameStatus.Lost;
            _statusMessage = "YOU DIED";
            return;
        }

        _statusMessage = damage == 0 ? "WEAPON BLOCKED ALL DAMAGE" : $"WEAPON FIGHT: -{damage} HP";
    }

    private void TryEquipSelectedWeapon()
    {
        if (!TryGetSelectedCard(out var card, out var cardIndex))
        {
            return;
        }

        if (card.GetCardType() != ScoundrelCardType.Weapon)
        {
            _statusMessage = "SELECT A WEAPON TO EQUIP";
            return;
        }

        if (_gameState.EquippedWeapon is not null)
        {
            _gameState.RoomState.DiscardPile.Add(_gameState.EquippedWeapon);
        }

        _gameState.EquippedWeapon = card;
        _gameState.LastWeaponKillValue = null;
        ResolveSelectedCard(cardIndex, card, false);
        _statusMessage = $"EQUIPPED {GetCardLabel(card)}";
    }

    private void TryDrinkSelectedPotion()
    {
        if (!TryGetSelectedCard(out var card, out var cardIndex))
        {
            return;
        }

        if (card.GetCardType() != ScoundrelCardType.Potion)
        {
            _statusMessage = "SELECT A POTION";
            return;
        }

        if (_potionUsedInRoom)
        {
            ResolveSelectedCard(cardIndex, card, true);
            _statusMessage = "POTION DISCARDED (ALREADY USED THIS ROOM)";
            return;
        }

        var potionValue = ScoundrelRules.GetValue(card);
        var before = _gameState.Health;
        _gameState.Health = ScoundrelRules.Heal(_gameState.Health, potionValue, MaximumHealth);
        _potionUsedInRoom = true;
        ResolveSelectedCard(cardIndex, card, true);
        _statusMessage = $"HEALED +{_gameState.Health - before}";
    }

    private bool TryGetSelectedCard(out ScoundrelCard card, out int cardIndex)
    {
        card = null!;
        cardIndex = _selectedRoomIndex;

        if (_gameState.RoomState.Cards.Count == 0)
        {
            _statusMessage = "NO CARD SELECTED";
            return false;
        }

        ClampSelectedIndex();
        cardIndex = _selectedRoomIndex;
        card = _gameState.RoomState.Cards[cardIndex];
        return true;
    }

    private void ResolveSelectedCard(int cardIndex, ScoundrelCard card, bool addToDiscard)
    {
        _gameState.RoomState.Cards.RemoveAt(cardIndex);

        if (addToDiscard)
        {
            _gameState.RoomState.DiscardPile.Add(card);
        }

        _resolvedInRoom++;
        _avoidedPreviousRoom = false;
        ClampSelectedIndex();
        CompleteResolutionCycle();
    }

    private void DrawRoomCards()
    {
        const int cardWidth = 150;
        const int cardHeight = 220;
        const int cardGap = 18;
        var startX = ScoundrelLayout.RoomCards.X + 20;
        var topY = ScoundrelLayout.RoomCards.Y + 30;

        for (var index = 0; index < 4; index++)
        {
            var bounds = new Rectangle(startX + index * (cardWidth + cardGap), topY, cardWidth, cardHeight);

            var isSelected = index == _selectedRoomIndex;
            var border = isSelected ? new Color(244, 213, 108) : new Color(236, 238, 230);
            Fill(bounds, border);
            Fill(new Rectangle(bounds.X + 3, bounds.Y + 3, bounds.Width - 6, bounds.Height - 6), new Color(253, 253, 250));

            if (index >= _gameState.RoomState.Cards.Count)
            {
                DrawText($"{index + 1}", bounds.X + 64, bounds.Y + 18, 2, new Color(120, 124, 126));
                DrawText("EMPTY", bounds.X + 38, bounds.Y + 100, 2, new Color(120, 124, 126));
                continue;
            }

            var card = _gameState.RoomState.Cards[index];
            var cardColor = card.Suit is ScoundrelSuit.Hearts or ScoundrelSuit.Diamonds
                ? new Color(176, 28, 28)
                : new Color(28, 28, 28);

            DrawText($"{index + 1}", bounds.X + 64, bounds.Y + 18, 2, new Color(82, 82, 82));
            DrawText(GetCardLabel(card), bounds.X + 18, bounds.Y + 48, 3, cardColor);
            DrawText(card.GetCardType().ToString(), bounds.X + 18, bounds.Y + 178, 2, new Color(62, 62, 62));
        }
    }

    private void DrawSelectedCardDetails()
    {
        if (_gameState.RoomState.Cards.Count == 0)
        {
            DrawText("SELECTED NONE", 960, 560, 2, new Color(242, 236, 203));
            return;
        }

        ClampSelectedIndex();
        var card = _gameState.RoomState.Cards[_selectedRoomIndex];
        DrawText($"SELECTED {GetCardLabel(card)}", 960, 560, 2, new Color(242, 236, 203));
        DrawText($"TYPE {card.GetCardType().ToString().ToUpperInvariant()}", 960, 588, 2, new Color(242, 236, 203));
        DrawText($"VALUE {ScoundrelRules.GetValue(card)}", 960, 616, 2, new Color(242, 236, 203));

        if (_gameState.LastWeaponKillValue.HasValue)
        {
            DrawText($"WEAPON LIMIT < {_gameState.LastWeaponKillValue.Value}", 960, 644, 2, new Color(242, 236, 203));
        }
    }

    private void DrawTerminalOverlay()
    {
        var panel = new Rectangle(280, 310, 540, 170);
        Fill(panel, new Color(0, 0, 0, 190));
        Fill(new Rectangle(panel.X + 4, panel.Y + 4, panel.Width - 8, panel.Height - 8), new Color(18, 32, 24));

        var title = _gameState.GameStatus == ScoundrelGameStatus.Won ? "YOU ESCAPED" : "YOU DIED";
        DrawText(title, 430, 360, 4, new Color(245, 245, 232));
        DrawText("PRESS N TO START NEW GAME", 352, 420, 2, new Color(220, 234, 224));
    }

    private static string GetCardLabel(ScoundrelCard card)
    {
        var rank = card.Rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => card.Rank.ToString()
        };

        var suit = card.Suit switch
        {
            ScoundrelSuit.Clubs => "C",
            ScoundrelSuit.Diamonds => "D",
            ScoundrelSuit.Hearts => "H",
            _ => "S"
        };

        return rank + suit;
    }

    private void ClampSelectedIndex()
    {
        if (_gameState.RoomState.Cards.Count == 0)
        {
            _selectedRoomIndex = 0;
            return;
        }

        _selectedRoomIndex = Math.Clamp(_selectedRoomIndex, 0, _gameState.RoomState.Cards.Count - 1);
    }

    private void Fill(Rectangle bounds, Color color)
    {
        _spriteBatch.Draw(_pixel, bounds, color);
    }

    private void DrawText(string text, int x, int y, int scale, Color color)
    {
        SolitaireBitmapFont.DrawText(_spriteBatch, _glyphTexture, false, text, x, y, scale, color, preserveColor: true);
    }
}
