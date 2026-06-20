#region CLASS_HEADER
//   PROJECT: Dungine
//  MODIFIED: 181029
//   AUTHORS: development@aprettycoolprogram.com
// COPYRIGHT: 2018 A Pretty Cool Program
//   LICENSE: Apache License, Version 2.0 [http://www.apache.org/licenses/LICENSE-2.0]
// MORE INFO: http://aprettycoolprogram.com/project/Dungine
#endregion

#region CLASS_NOTES
// Game screen things.
#endregion CLASS_NOTES

using Microsoft.Xna.Framework.Graphics;

namespace ACardGame.Dungine
{
    internal class GameScreen
    {
        /// <summary>
        /// Set the game fullscreen or window resolution.
        /// </summary>
        /// <param name="gameInstance">GameData object.</param>
        public static void SetResolution(GameInstance gameInstance)
        {
            if (gameInstance.GameIsFullscreen)
            {
                gameInstance.GraphicsObject.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                gameInstance.GraphicsObject.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                gameInstance.GraphicsObject.IsFullScreen = true;
            }
            else
            {
                gameInstance.GraphicsObject.PreferredBackBufferWidth = gameInstance.ScreenWidth;
                gameInstance.GraphicsObject.PreferredBackBufferHeight = gameInstance.ScreenHeight;
            }

            gameInstance.GraphicsObject.ApplyChanges();
        }
    }
}
