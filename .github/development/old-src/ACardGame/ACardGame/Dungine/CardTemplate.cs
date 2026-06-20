#region CLASS_HEADER
//   PROJECT: Dungine
//  MODIFIED: 181030
//   AUTHORS: development@aprettycoolprogram.com
// COPYRIGHT: 2018 A Pretty Cool Program
//   LICENSE: Apache License, Version 2.0 [http://www.apache.org/licenses/LICENSE-2.0]
// MORE INFO: http://aprettycoolprogram.com/project/Dungine
#endregion

#region CLASS_NOTES
// The card template object.
//
// I've written up a long, detailed description of how all of this works. You can find it in the User Manual.
#endregion CLASS_NOTES

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace ACardGame.Dungine
{
    public class CardTemplate
    {
        public TemplateDetail Detail { get; set; }
        public TemplateFace Back { get; set; }
        public TemplateFace SideA { get; set; }

        /// <summary>
        /// Builds all of the card templates for a game.
        /// </summary>
        /// <param name="gameInstance">All the game data you need!</param>
        /// <returns>A dictionary of card templates.</returns>
        public static Dictionary<string, Dictionary<string, Texture2D>> BuildAll(GameInstance gameInstance)
        {
            var cardTemplates = new Dictionary<string, Dictionary<string, Texture2D>>();

            foreach (var template in gameInstance.CardTemplates)
            {
                var singleTemplate = BuildSingle(gameInstance, template.Value);
                cardTemplates.Add(template.Key, singleTemplate);
            }

            return cardTemplates;
        }

        /// <summary>
        /// Build a card template for a single template.
        /// </summary>
        /// <param name="gameInstance">All the game data you need!</param>
        /// <param name="templateDefinition">Card template definition.</param>
        /// <returns>A single card template.</returns>
        public static Dictionary<string, Texture2D> BuildSingle(GameInstance gameInstance, string templateDescription)
        {
            var cardDefinition = GetDefinition(gameInstance, templateDescription);

            return new Dictionary<string, Texture2D>
            {
                {"Back",  RenderCardFace(gameInstance, cardDefinition.Detail, cardDefinition.Back)},
                {"SideA", RenderCardFace(gameInstance, cardDefinition.Detail, cardDefinition.SideA)},
            };
        }

        /// <summary>
        /// Render a card face.
        /// </summary>
        /// <returns>A texture.</returns>
        public static Texture2D RenderCardFace(GameInstance gameInstance, TemplateDetail cardDetail, TemplateFace cardFace)
        {
            var renderTarget = NewRenderTarget2D(gameInstance, cardDetail, cardFace);

            gameInstance.GameObject.GraphicsDevice.SetRenderTarget(renderTarget);

            DrawEverything(renderTarget, gameInstance, cardFace);

            gameInstance.GameObject.GraphicsDevice.SetRenderTarget(null);

            return renderTarget;
        }

        public static Texture2D DrawEverything(RenderTarget2D renderTarget, GameInstance gameInstance, TemplateFace cardFace)
        {
            gameInstance.GameObject.GraphicsDevice.Clear(Color.Transparent);

            var spriteBatch = new SpriteBatch(gameInstance.GameObject.GraphicsDevice);
            spriteBatch.Begin();

            DrawBorder(spriteBatch, gameInstance, cardFace);

            spriteBatch.End();

            return renderTarget;
        }

        /// <summary>
        /// Draw the card borders
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch.</param>
        /// <param name="gameInstance">The game information.</param>
        /// <param name="cardFace">The card face to draw.</param>
        private static void DrawBorder(SpriteBatch spriteBatch, GameInstance gameInstance, TemplateFace cardFace)
        {
            spriteBatch.Draw(gameInstance.GameObject.Content.Load<Texture2D>(gameInstance.CardElementLocation + cardFace.BorderExterior.Component),
                             new Rectangle(cardFace.BorderExterior.LocX, cardFace.BorderExterior.LocY, cardFace.BorderExterior.Width, cardFace.BorderExterior.Height),
                             cardFace.BorderExterior.Paint);

            spriteBatch.Draw(gameInstance.GameObject.Content.Load<Texture2D>(gameInstance.CardElementLocation + cardFace.BorderInterior.Component),
                             new Rectangle(cardFace.BorderInterior.LocX, cardFace.BorderInterior.LocY, cardFace.BorderInterior.Width, cardFace.BorderInterior.Height),
                             cardFace.BorderInterior.Paint);
        }

        /// <summary>
        /// Create a new renderTarget.
        /// </summary>
        /// <param name="gameData">Game information.</param>
        /// <param name="cardDetail">Card details.</param>
        /// <param name="cardFace">Card face.</param>
        /// <returns></returns>
        private static RenderTarget2D NewRenderTarget2D(GameInstance gameData, TemplateDetail cardDetail, TemplateFace cardFace)
        {
            return new RenderTarget2D(gameData.GameObject.GraphicsDevice,
                                      cardDetail.Width,
                                      cardDetail.Height,
                                      true,
                                      gameData.GameObject.GraphicsDevice.DisplayMode.Format,
                                      DepthFormat.Depth24);
        }

        /// <summary>
        /// Get the details of a card type.
        /// </summary>
        /// <param name="game">The MonoGame object.</param>
        /// <param name="cardType">The type of card.</param>
        /// <returns>A Card object with the card details.</returns>
        public static CardTemplate GetDefinition(GameInstance gameInstance, string cardType)
        {
            var cardData = File.Exists(gameInstance.CardTemplateLocation + cardType)
                ? File.ReadAllText(gameInstance.CardTemplateLocation + cardType)
                : File.ReadAllText(gameInstance.CardTemplateLocation + "dgn_Error.dgnct");

            return JsonConvert.DeserializeObject<CardTemplate>(cardData);
        }
    }

    /// <summary>
    /// Card template details.
    /// </summary>
    public class TemplateDetail
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Card back.
    /// </summary>
    public class TemplateFace
    {
        public BorderExterior BorderExterior { get; set; }
        public BorderInterior BorderInterior { get; set; }
    }

    /// <summary>
    /// Border exterior.
    /// </summary>
    public class BorderExterior
    {
        public string Component { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int LocX { get; set; }
        public int LocY { get; set; }
        public Color Paint { get; set; }
    }

    /// <summary>
    /// Border interior.
    /// </summary>
    public class BorderInterior
    {
        public string Component { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int LocX { get; set; }
        public int LocY { get; set; }
        public Color Paint { get; set; }
    }

    /// <summary>
    /// Card title.
    /// </summary>
    public class Title
    {
        public string Component { get; set; }
        public Color Paint { get; set; }
    }

    /// <summary>
    /// Card artwork.
    /// </summary>
    public class Artwork
    {
        public string Component { get; set; }
        public Color Paint { get; set; }
    }
}