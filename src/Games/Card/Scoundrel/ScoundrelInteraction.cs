using System;
using System.Collections.Generic;

namespace Cheron.Games.Card.Scoundrel
{
    /// <summary>
    /// Handles user interaction for the Scoundrel game.
    /// </summary>
    public class ScoundrelInteraction
    {
        /// <summary>
        /// Handles user input for the game.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="input">The user input.</param>
        public void HandleInput(ScoundrelGameState gameState, string input)
        {
            // Implementation for handling user input
            switch (input.ToLower())
            {
                case "draw":
                    DrawCard(gameState);
                    break;
                case "fight":
                    FightMonster(gameState);
                    break;
                case "use potion":
                    UsePotion(gameState);
                    break;
                case "equip weapon":
                    EquipWeapon(gameState);
                    break;
                case "quit":
                    gameState.GameStatus = ScoundrelGameStatus.Lost;
                    break;
                default:
                    // Handle unknown input
                    break;
            }
        }

        /// <summary>
        /// Draws a card from the dungeon pile.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        private void DrawCard(ScoundrelGameState gameState)
        {
            // Implementation for drawing a card from the dungeon pile
            if (gameState.RoomState.DungeonPile.Count > 0)
            {
                ScoundrelCard card = gameState.RoomState.DungeonPile[0];
                gameState.RoomState.DungeonPile.RemoveAt(0);
                gameState.RoomState.Cards.Add(card);
            }
        }

        /// <summary>
        /// Fights a monster in the room.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        private void FightMonster(ScoundrelGameState gameState)
        {
            // Implementation for fighting a monster
            if (gameState.RoomState.Cards.Count > 0)
            {
                ScoundrelCard monster = gameState.RoomState.Cards[0];
                int monsterValue = ScoundrelRules.GetValue(monster);

                if (ScoundrelRules.CanWeaponFight(monsterValue, gameState.LastWeaponKillValue))
                {
                    int damage = ScoundrelRules.GetMonsterDamage(monsterValue, gameState.EquippedWeapon);
                    gameState.Health -= damage;

                    if (gameState.Health <= 0)
                    {
                        gameState.GameStatus = ScoundrelGameStatus.Lost;
                    }
                    else
                    {
                        gameState.RoomState.Cards.RemoveAt(0);
                        gameState.RoomState.DiscardPile.Add(monster);
                        gameState.LastWeaponKillValue = monsterValue;
                    }
                }
                else
                {
                    // Weapon cannot fight this monster
                }
            }
        }

        /// <summary>
        /// Uses a potion to heal the player.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        private void UsePotion(ScoundrelGameState gameState)
        {
            // Implementation for using a potion
            if (gameState.RoomState.Cards.Count > 0)
            {
                ScoundrelCard potion = gameState.RoomState.Cards[0];
                int potionValue = ScoundrelRules.GetValue(potion);
                gameState.Health = ScoundrelRules.Heal(gameState.Health, potionValue, 100);
                gameState.RoomState.Cards.RemoveAt(0);
                gameState.RoomState.DiscardPile.Add(potion);
            }
        }

        /// <summary>
        /// Equips a weapon.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        private void EquipWeapon(ScoundrelGameState gameState)
        {
            // Implementation for equipping a weapon
            if (gameState.RoomState.Cards.Count > 0)
            {
                ScoundrelCard weapon = gameState.RoomState.Cards[0];
                if (weapon.GetCardType() == ScoundrelCardType.Weapon)
                {
                    gameState.EquippedWeapon = weapon;
                    gameState.RoomState.Cards.RemoveAt(0);
                    gameState.RoomState.DiscardPile.Add(weapon);
                }
            }
        }
    }
}