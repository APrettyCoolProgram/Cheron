#region CLASS_HEADER
//   PROJECT: Dungine
//  MODIFIED: 181030
//   AUTHORS: development@aprettycoolprogram.com
// COPYRIGHT: 2018 A Pretty Cool Program
//   LICENSE: Apache License, Version 2.0 [http://www.apache.org/licenses/LICENSE-2.0]
// MORE INFO: http://aprettycoolprogram.com/project/Dungine
#endregion

#region CLASS_NOTES
// Game information for Dungine.
#endregion CLASS_NOTES

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace ACardGame.Dungine
{
    /// <summary>
    /// GameData object.
    /// </summary>
    public class GameInstance
    {
        public Game GameObject { get; set; }
        public GraphicsDeviceManager GraphicsObject { get; set; }
        public string GameName { get; set; }
        public string GameVersion { get; set; }
        public string CardElementLocation { get; set; }
        public string CardDefinitionLocation { get; set; }
        public string CardTemplateLocation { get; set; }
        public Dictionary<string, string> CardTemplates { get; set; }
        public bool GameIsFullscreen { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public Color CanvasPrimaryColor { get; set; }
        public string DungineVersion { get; set; }
        public int DevelopmentMode { get; set; }


        /// <summary>
        /// Load important data about this game.
        /// </summary>
        /// <param name="game">Game object.</param>
        /// <param name="graphics">Graphics object.</param>
        /// <returns>The GameData object with lots of cool information!</returns>
        public static GameInstance Load(Game game, GraphicsDeviceManager graphics)
        {
            const string gameDataFile = "./AppData/game-data.json";
            var gameData = JsonConvert.DeserializeObject<GameInstance>(File.ReadAllText(gameDataFile));

            gameData.GameObject = game;
            gameData.GraphicsObject = graphics;

            return gameData;
        }
    }
}
