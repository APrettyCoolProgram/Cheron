#region CLASS_HEADER
//   PROJECT: Dungine
//  MODIFIED: 181030
//   AUTHORS: development@aprettycoolprogram.com
// COPYRIGHT: 2018 A Pretty Cool Program
//   LICENSE: Apache License, Version 2.0 [http://www.apache.org/licenses/LICENSE-2.0]
// MORE INFO: http://aprettycoolprogram.com/project/Dungine
#endregion

#region CLASS_NOTES
// Development tools for Dungine.
#endregion CLASS_NOTES

using System;

namespace ACardGame.Dungine
{
    internal class Tools
    {
        /// <summary>
        /// Do some special things, then exits the game.
        /// </summary>
        /// <param name="gameInstance">All sorts of information about this game.</param>
        public static void DevelopmentMode(GameInstance gameInstance)
        {
            /* DevelopmentMode = 0: Development Mode disabled.
             * DevelopmentMode = 1: The game framework is created.
             * DevelopmentMode = 2: The game framework and an empty JSON files are created.
             */
            if (gameInstance.DevelopmentMode >= 2)
            {
                CreateEmptyJSON("devmode2.json");
            }

            Environment.Exit(0);
        }

        /// <summary>
        /// Create an empty card template file.
        /// </summary>
        /// <param name="filename"></param>
        public static void CreateEmptyJSON(string filename)
        {
            // [TODO] This object is not accurate, since the final object definition hasn't been determined yet.
            var card = new CardTemplate()
            {
                Detail = new TemplateDetail(),
                Back = new TemplateFace(),
                SideA = new TemplateFace(),
            };

            Platform.WindowsOS.WriteJSON(@"./AppData/TemporaryData/" + filename, card);
        }
    }
}
