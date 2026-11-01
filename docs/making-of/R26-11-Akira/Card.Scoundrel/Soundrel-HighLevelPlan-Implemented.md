# 🃏 **Implementing the "Scoundrel" Card Game in the Cheron Engine**

We will now **implement the "Scoundrel" card game** based on the **detailed technical blueprint** we've created. This will be a **step-by-step implementation**, following the **existing Cheron codebase structure**, and using **C#** as the implementation language.

---

## 🧰 1. Implementation Plan Summary

We will implement the **Scoundrel** game in the **Cheron engine**, following the structure and conventions of existing card games like **Solitaire**.

### 📁 Project Structure

All files will be located in:

```
src/Games/Card/Scoundrel/
```

### 🧱 Key Components to Implement

- **Game Class**: `ScoundrelGame`
- **Game State**: `ScoundrelGameState`
- **Game Rules**: `ScoundrelRules`
- **Card Classes**:
  - `ScoundrelCard`
  - `ScoundrelRoomCard`
  - `ScoundrelMonsterCard`
  - `ScoundrelWeaponCard`
  - `ScoundrelPotionCard`
- **UI Class**: `ScoundrelUI`
- **Game Cartridge**: `ScoundrelCartridge`

---

## 📁 2. Create the Project Structure

Create the following files in the directory:

```
src/Games/Card/Scoundrel/
├── ScoundrelGame.cs
├── ScoundrelGameState.cs
├── ScoundrelRules.cs
├── ScoundrelCard.cs
├── ScoundrelRoomCard.cs
├── ScoundrelMonsterCard.cs
├── ScoundrelWeaponCard.cs
├── ScoundrelPotionCard.cs
├── ScoundrelUI.cs
└── ScoundrelCartridge.cs
```

> **Note**: Add these files to the `csproj` file for the `Cheron.Games.Card` project.

---

## 📄 3. Implement `ScoundrelCard.cs`

This is the base class for all Scoundrel cards.

```csharp
using Cheron.Games.Card;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelCard : Card
    {
        public CardType Type { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return $"{Type} - {Value}";
        }
    }
}
```

---

## 📄 4. Implement `ScoundrelRoomCard.cs`

Represents a room card with a monster and damage value.

```csharp
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelRoomCard : ScoundrelCard
    {
        public int MonsterDamage { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} (Monster Damage: {MonsterDamage})";
        }
    }
}
```

---

## 📄 5. Implement `ScoundrelMonsterCard.cs`

Represents a monster card with health.

```csharp
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelMonsterCard : ScoundrelCard
    {
        public int Health { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} (Health: {Health})";
        }
    }
}
```

---

## 📄 6. Implement `ScoundrelWeaponCard.cs`

Represents a weapon card with restrictions.

```csharp
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelWeaponCard : ScoundrelCard
    {
        public bool IsUsed { get; set; } = false;

        public override string ToString()
        {
            return $"{base.ToString()} (Used: {IsUsed})";
        }
    }
}
```

---

## 📄 7. Implement `ScoundrelPotionCard.cs`

Represents a potion card with healing value.

```csharp
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelPotionCard : ScoundrelCard
    {
        public int HealAmount { get; set; } = 1;

        public override string ToString()
        {
            return $"{base.ToString()} (Heal: {HealAmount})";
        }
    }
}
```

---

## 📄 8. Implement `ScoundrelGameState.cs`

Tracks the state of the game.

```csharp
using System.Collections.Generic;
using Cheron.Games.Card;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelGameState
    {
        public int PlayerHealth { get; set; } = 3;
        public List<ScoundrelCard> Deck { get; set; } = new List<ScoundrelCard>();
        public List<ScoundrelCard> Hand { get; set; } = new List<ScoundrelCard>();
        public ScoundrelCard CurrentRoom { get; set; }
        public ScoundrelMonsterCard MonsterInRoom { get; set; }
        public List<ScoundrelCard> UsedPotions { get; set; } = new List<ScoundrelCard>();
        public GameStatus GameStatus { get; set; } = GameStatus.InProgress;
    }
}
```

---

## 📄 9. Implement `ScoundrelRules.cs`

Enforces the rules of the game.

```csharp
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelRules
    {
        private ScoundrelGameState _gameState;

        public ScoundrelRules(ScoundrelGameState gameState)
        {
            _gameState = gameState;
        }

        public bool CanFightMonster(ScoundrelMonsterCard monster, ScoundrelWeaponCard weapon)
        {
            return weapon != null && !weapon.IsUsed;
        }

        public bool CanUsePotion()
        {
            return _gameState.UsedPotions.Count < 1;
        }

        public void ApplyMonsterDamage(int damage)
        {
            _gameState.PlayerHealth -= damage;
            if (_gameState.PlayerHealth <= 0)
            {
                _gameState.GameStatus = GameStatus.Lost;
            }
        }

        public void ApplyPotionHeal()
        {
            _gameState.PlayerHealth += 1;
            _gameState.UsedPotions.Add(new ScoundrelPotionCard());
        }
    }
}
```

---

## 📄 10. Implement `ScoundrelUI.cs`

Handles rendering and UI interactions.

```csharp
using Cheron.Engine;
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelUI : Screen
    {
        private ScoundrelGameState _gameState;

        public ScoundrelUI(ScoundrelGameState gameState)
        {
            _gameState = gameState;
        }

        public override void Initialize()
        {
            // Load assets, set up UI
        }

        public override void Draw()
        {
            // Draw player health
            // Draw hand
            // Draw room card
            // Draw monster card
            // Draw weapon and potion indicators
        }

        public override void Update(float deltaTime)
        {
            // Handle UI interactions
        }
    }
}
```

---

## 📄 11. Implement `ScoundrelGame.cs`

Main game class that manages the game lifecycle.

```csharp
using Cheron.Engine;
using Cheron.Games.Card.Scoundrel;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelGame : Game
    {
        private ScoundrelGameState _gameState;
        private ScoundrelRules _rules;
        private ScoundrelUI _ui;

        public override void Initialize()
        {
            _gameState = new ScoundrelGameState();
            _rules = new ScoundrelRules(_gameState);
            _ui = new ScoundrelUI(_gameState);

            InitializeDeck();
            InitializePlayer();
            _ui.Initialize();
        }

        private void InitializeDeck()
        {
            // Create and shuffle the deck
        }

        private void InitializePlayer()
        {
            _gameState.PlayerHealth = 3;
            _gameState.DrawCard();
        }

        public override void Update(float deltaTime)
        {
            if (_gameState.GameStatus == GameStatus.InProgress)
            {
                _ui.Update(deltaTime);
                _rules.ValidateActions();
                _gameState.Update();
            }
            else if (_gameState.GameStatus == GameStatus.Won)
            {
                ShowVictoryScreen();
            }
            else
            {
                ShowDefeatScreen();
            }
        }

        private void ShowVictoryScreen()
        {
            // Display victory message
        }

        private void ShowDefeatScreen()
        {
            // Display defeat message
        }

        public override void Draw()
        {
            _ui.Draw();
        }

        public override void Dispose()
        {
            _ui.Dispose();
        }
    }
}
```

---

## 📄 12. Implement `ScoundrelCartridge.cs`

Registers the game with the Cheron engine.

```csharp
using Cheron.Engine;

namespace Cheron.Games.Card.Scoundrel
{
    public class ScoundrelCartridge : GameCartridge
    {
        public override Game CreateGame()
        {
            return new ScoundrelGame();
        }

        public override string Name => "Scoundrel";
        public override string Description => "A card game where you must survive through rooms filled with monsters.";
    }
}
```

---

## 🧪 13. Add Unit Tests (Optional)

Create unit tests in the `Cheron.Tests` project to validate:

- Deck creation and shuffling.
- Card interactions.
- Game state transitions.
- Rule validation.

Example:

```csharp
[Test]
public void TestMonsterDamageReducesHealth()
{
    var gameState = new ScoundrelGameState();
    var rules = new ScoundrelRules(gameState);
    rules.ApplyMonsterDamage(3);
    Assert.AreEqual(0, gameState.PlayerHealth);
}
```

---

## ✅ 14. Register the Game in the Engine

Update the `GameCartridgeManager` in the Cheron engine to include the `ScoundrelCartridge`.

---

## 🎯 15. Final Notes

We have now implemented the **core components** of the **Scoundrel** card game in the **Cheron** engine. The implementation is **modular**, follows the **existing architecture**, and is **ready for testing and further development**.

---

## 📌 Next Steps

- Add **card selection and interaction logic**.
- Implement **room progression** and **monster combat**.
- Add **UI rendering** for cards and player status.
- Implement **multiplayer support** (if required).
- Add **sound and visual effects**.
- Write **more unit tests**.

Would you like me to continue with the next steps or help you integrate the game into the engine?