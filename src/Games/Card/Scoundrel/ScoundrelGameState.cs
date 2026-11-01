using System;
using System.Collections.Generic;

namespace Cheron.Games.Card.Scoundrel
{
    /// <summary>
    /// Represents the state of the Scoundrel game.
    /// </summary>
    public class ScoundrelGameState
    {
        /// <summary>
        /// Gets or sets the current health of the player.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets the equipped weapon.
        /// </summary>
        public ScoundrelCard? EquippedWeapon { get; set; }

        /// <summary>
        /// Gets or sets the value of the last monster killed by the equipped weapon.
        /// </summary>
        public int? LastWeaponKillValue { get; set; }

        /// <summary>
        /// Gets or sets the current room state.
        /// </summary>
        public ScoundrelRoomState RoomState { get; set; }

        /// <summary>
        /// Gets or sets the current game status.
        /// </summary>
        public ScoundrelGameStatus GameStatus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoundrelGameState"/> class.
        /// </summary>
        public ScoundrelGameState()
        {
            Health = 100;
            EquippedWeapon = null;
            LastWeaponKillValue = null;
            RoomState = new ScoundrelRoomState();
            GameStatus = ScoundrelGameStatus.NotStarted;
        }
    }

    /// <summary>
    /// Represents the state of the room in Scoundrel.
    /// </summary>
    public class ScoundrelRoomState
    {
        /// <summary>
        /// Gets or sets the list of cards in the room.
        /// </summary>
        public List<ScoundrelCard> Cards { get; set; }

        /// <summary>
        /// Gets or sets the list of cards in the dungeon pile.
        /// </summary>
        public List<ScoundrelCard> DungeonPile { get; set; }

        /// <summary>
        /// Gets or sets the list of cards in the discard pile.
        /// </summary>
        public List<ScoundrelCard> DiscardPile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoundrelRoomState"/> class.
        /// </summary>
        public ScoundrelRoomState()
        {
            Cards = new List<ScoundrelCard>();
            DungeonPile = new List<ScoundrelCard>();
            DiscardPile = new List<ScoundrelCard>();
        }
    }
}