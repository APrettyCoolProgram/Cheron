using System;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Card.Scoundrel
{
    /// <summary>
    /// Contains layout constants and rectangle factories for Scoundrel.
    /// </summary>
    public static class ScoundrelLayout
    {
        /// <summary>
        /// Gets the dimensions of the game board.
        /// </summary>
        public static readonly Point BoardSize = new Point(1100, 800);

        /// <summary>
        /// Gets the rectangle for the top bar.
        /// </summary>
        public static Rectangle TopBar => new Rectangle(0, 0, BoardSize.X, 100);

        /// <summary>
        /// Gets the rectangle for the dungeon pile.
        /// </summary>
        public static Rectangle DungeonPile => new Rectangle(50, 120, 200, 100);

        /// <summary>
        /// Gets the rectangle for the discard pile.
        /// </summary>
        public static Rectangle DiscardPile => new Rectangle(50, 240, 200, 100);

        /// <summary>
        /// Gets the rectangle for the room cards.
        /// </summary>
        public static Rectangle RoomCards => new Rectangle(300, 120, 800, 600);

        /// <summary>
        /// Gets the rectangle for the equipped weapon slot.
        /// </summary>
        public static Rectangle EquippedWeaponSlot => new Rectangle(950, 120, 150, 100);

        /// <summary>
        /// Gets the rectangle for the health panel.
        /// </summary>
        public static Rectangle HealthPanel => new Rectangle(950, 240, 150, 100);

        /// <summary>
        /// Gets the rectangle for the status panel.
        /// </summary>
        public static Rectangle StatusPanel => new Rectangle(950, 360, 150, 100);

        /// <summary>
        /// Gets the rectangle for the action panel.
        /// </summary>
        public static Rectangle ActionPanel => new Rectangle(950, 480, 150, 100);

        /// <summary>
        /// Gets the rectangle for the footer.
        /// </summary>
        public static Rectangle Footer => new Rectangle(0, 700, BoardSize.X, 100);

        /// <summary>
        /// Gets the rectangle for the terminal overlay.
        /// </summary>
        public static Rectangle TerminalOverlay => new Rectangle(0, 0, BoardSize.X, BoardSize.Y);
    }
}