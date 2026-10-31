// 261031_code
// 261029_documentation

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cheron.Games.Solitaire;

/// <summary>Provides Solitaire's built-in five-by-seven bitmap font and text drawing helpers.</summary>
internal static class SolitaireBitmapFont
{
    /// <summary>Stores seven five-bit bitmap rows for each uppercase letter followed by each decimal digit.</summary>
    /// <remarks>
    /// The first dimension selects A through Z, then 0 through 9. The second selects a glyph row.
    /// Bits four through zero represent pixels from left to right; exclamation marks are generated separately.
    /// </remarks>
    private static readonly int[,] Glyphs =
    {
        { 14, 17, 17, 31, 17, 17, 17 }, { 30, 17, 17, 30, 17, 17, 30 },
        { 14, 17, 16, 16, 16, 17, 14 }, { 30, 17, 17, 17, 17, 17, 30 },
        { 31, 16, 16, 30, 16, 16, 31 }, { 31, 16, 16, 30, 16, 16, 16 },
        { 14, 17, 16, 23, 17, 17, 15 }, { 17, 17, 17, 31, 17, 17, 17 },
        { 14, 4, 4, 4, 4, 4, 14 }, { 7, 2, 2, 2, 18, 18, 12 },
        { 17, 18, 20, 24, 20, 18, 17 }, { 16, 16, 16, 16, 16, 16, 31 },
        { 17, 27, 21, 21, 17, 17, 17 }, { 17, 25, 21, 19, 17, 17, 17 },
        { 14, 17, 17, 17, 17, 17, 14 }, { 30, 17, 17, 30, 16, 16, 16 },
        { 14, 17, 17, 17, 21, 18, 13 }, { 30, 17, 17, 30, 20, 18, 17 },
        { 15, 16, 16, 14, 1, 1, 30 }, { 31, 4, 4, 4, 4, 4, 4 },
        { 17, 17, 17, 17, 17, 17, 14 }, { 17, 17, 17, 17, 17, 10, 4 },
        { 17, 17, 17, 21, 21, 21, 10 }, { 17, 17, 10, 4, 10, 17, 17 },
        { 17, 17, 10, 4, 4, 4, 4 }, { 31, 1, 2, 4, 8, 16, 31 },
        { 14, 17, 19, 21, 25, 17, 14 }, { 4, 12, 4, 4, 4, 4, 14 },
        { 14, 17, 1, 2, 4, 8, 31 }, { 30, 1, 1, 14, 1, 1, 30 },
        { 2, 6, 10, 18, 31, 2, 2 }, { 31, 16, 16, 30, 1, 1, 30 },
        { 14, 16, 16, 30, 17, 17, 14 }, { 31, 1, 2, 4, 8, 8, 8 },
        { 14, 17, 17, 14, 17, 17, 14 }, { 14, 17, 17, 15, 1, 1, 14 }
    };

    /// <summary>Builds a padded atlas of the existing five-by-seven glyphs for smoothly scaled lettering.</summary>
    /// <param name="graphicsDevice">The borrowed device that creates the owned texture.</param>
    /// <returns>The glyph atlas texture.</returns>
    public static Texture2D CreateGlyphTexture(GraphicsDevice graphicsDevice)
    {
        const int atlasWidth = 37 * 7;
        var pixels = new Color[atlasWidth * 9];

        for (var glyphIndex = 0; glyphIndex < 37; glyphIndex++)
        {
            for (var glyphRow = 0; glyphRow < 7; glyphRow++)
            {
                var rowBits = glyphIndex == 36
                    ? (glyphRow < 5 || glyphRow == 6 ? 4 : 0)
                    : Glyphs[glyphIndex, glyphRow];

                for (var glyphColumn = 0; glyphColumn < 5; glyphColumn++)
                {
                    if ((rowBits & (1 << (4 - glyphColumn))) != 0)
                    {
                        pixels[(glyphRow + 1) * atlasWidth + glyphIndex * 7 + glyphColumn + 1] = Color.White;
                    }
                }
            }
        }

        var texture = new Texture2D(graphicsDevice, atlasWidth, 9);

        texture.SetData(pixels);

        return texture;
    }

    /// <summary>Draws uppercase bitmap text, leaving blank character widths for unsupported characters.</summary>
    /// <param name="spriteBatch">The destination sprite batch.</param>
    /// <param name="glyphTexture">The shared glyph atlas texture.</param>
    /// <param name="highContrast">Whether high-contrast mode is currently active.</param>
    /// <param name="text">The text to draw; letters, digits, spaces, and exclamation marks are supported.</param>
    /// <param name="textX">The starting horizontal position in logical board coordinates.</param>
    /// <param name="textY">The starting vertical position in logical board coordinates.</param>
    /// <param name="scale">The size of each glyph pixel in logical board units.</param>
    /// <param name="color">The normal-theme text color.</param>
    /// <param name="preserveColor">Whether to retain contrasting card or button ink in high-contrast mode.</param>
    public static void DrawText(SpriteBatch spriteBatch, Texture2D glyphTexture, bool highContrast, string text, int textX, int textY, int scale, Color color, bool preserveColor = false)
    {
        if (highContrast && !preserveColor)
        {
            color = Color.White;
        }

        foreach (var character in text.ToUpperInvariant())
        {
            if (character == ' ')
            {
                textX += 4 * scale;

                continue;
            }

            var glyphIndex = character switch
            {
                >= 'A' and <= 'Z' => character - 'A',
                >= '0' and <= '9' => character - '0' + 26,
                '!' => 36,
                _ => -1
            };

            if (glyphIndex >= 0)
            {
                spriteBatch.Draw(glyphTexture, new Rectangle(textX, textY, 5 * scale, 7 * scale),
                    new Rectangle(glyphIndex * 7 + 1, 1, 5, 7), color);
            }

            textX += 6 * scale;
        }
    }
}
