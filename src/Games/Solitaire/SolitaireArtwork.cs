// 261028_code
// 261028_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cheron.Games.Solitaire;

/// <summary>Creates and owns the textures used by Solitaire's card-table theme.</summary>
internal sealed class SolitaireArtwork : IDisposable
{
    private readonly List<Texture2D> _textures = [];

    /// <summary>Creates cached artwork on the graphics thread without external assets.</summary>
    /// <param name="graphicsDevice">The borrowed device used to create textures.</param>
    public SolitaireArtwork(GraphicsDevice graphicsDevice)
    {
        Background = CreateTexture(graphicsDevice, 1100, 800, (x, y) =>
        {
            var distance = Vector2.Distance(new Vector2(x, y), new Vector2(510, 270)) / 900f;

            return Color.Lerp(new Color(32, 83, 79), new Color(12, 34, 39), Math.Clamp(distance, 0, 1));
        });

        CardFace = CreateRoundedTexture(graphicsDevice, 78, 108, 8, new Color(253, 250, 241), new Color(207, 216, 207));

        CardBack = CreateTexture(graphicsDevice, 78, 108, (x, y) =>
        {
            var coverage = RoundedCoverage(x, y, 78, 108, 8);
            var isBorder = x < 5 || x > 72 || y < 5 || y > 102;
            var isPattern = ((x + y) % 12 < 2) || ((x - y + 120) % 12 < 2);
            var color = isBorder ? new Color(229, 231, 218)
                : isPattern
                ? new Color(62, 115, 123) : new Color(30, 70, 87);

            return color * coverage;
        });

        CardShadow = CreateTexture(graphicsDevice, 94, 124, (x, y) =>
        {
            var distance = RoundedDistance(x - 8, y - 8, 78, 108, 8);

            return Color.Black * (0.25f * Math.Clamp(1 - distance / 8, 0, 1));
        });

        Slot = CreateRoundedTexture(graphicsDevice, 78, 108, 8, new Color(19, 53, 56), new Color(71, 111, 108));
        Selection = CreateRoundedTexture(graphicsDevice, 84, 114, 10, new Color(236, 195, 106), new Color(255, 227, 161));
        Button = CreateRoundedTexture(graphicsDevice, 132, 44, 9, new Color(235, 205, 143), new Color(251, 226, 176));
        ButtonHover = CreateRoundedTexture(graphicsDevice, 132, 44, 9, new Color(252, 225, 169), new Color(255, 240, 211));
        Banner = CreateRoundedTexture(graphicsDevice, 360, 56, 12, new Color(17, 43, 46), new Color(101, 139, 128));

        var suits = new Texture2D[4];

        for (var suitIndex = 0; suitIndex < suits.Length; suitIndex++)
        {
            var currentSuit = suitIndex;

            suits[suitIndex] = CreateTexture(graphicsDevice, 64, 64, (x, y) =>
            {
                var coveredSamples = 0;

                for (var sampleY = 0; sampleY < 3; sampleY++)
                {
                    for (var sampleX = 0; sampleX < 3; sampleX++)
                    {
                        var position = new Vector2(
                            (x + (sampleX + 0.5f) / 3 - 32) / 28,
                            (y + (sampleY + 0.5f) / 3 - 32) / 28);
                        if (ContainsSuit(position, currentSuit))
                        {
                            coveredSamples++;
                        }
                    }
                }

                return Color.White * (coveredSamples / 9f);
            });
        }

        Suits = suits;
    }

    /// <summary>Gets the softly shaded table background.</summary>
    public Texture2D Background { get; }

    /// <summary>Gets the rounded ivory card face.</summary>
    public Texture2D CardFace { get; }

    /// <summary>Gets the patterned blue card back.</summary>
    public Texture2D CardBack { get; }

    /// <summary>Gets the soft card shadow, including an eight-pixel margin.</summary>
    public Texture2D CardShadow { get; }

    /// <summary>Gets the recessed empty-pile marker.</summary>
    public Texture2D Slot { get; }

    /// <summary>Gets the selection outline drawn behind a card.</summary>
    public Texture2D Selection { get; }

    /// <summary>Gets the new-game button background.</summary>
    public Texture2D Button { get; }

    /// <summary>Gets the highlighted new-game button background.</summary>
    public Texture2D ButtonHover { get; }

    /// <summary>Gets the win-message panel.</summary>
    public Texture2D Banner { get; }

    /// <summary>Gets suit masks in clubs, diamonds, hearts, and spades order.</summary>
    public IReadOnlyList<Texture2D> Suits { get; }

    /// <summary>Releases every texture created by this artwork set.</summary>
    public void Dispose()
    {
        foreach (var texture in _textures)
        {
            texture.Dispose();
        }

        _textures.Clear();
    }

    /// <summary>Creates a rounded panel with a thin border and antialiased edges.</summary>
    /// <param name="graphicsDevice">The borrowed graphics device.</param>
    /// <param name="width">The panel width in pixels.</param>
    /// <param name="height">The panel height in pixels.</param>
    /// <param name="radius">The corner radius in pixels.</param>
    /// <param name="fill">The interior color.</param>
    /// <param name="border">The border color.</param>
    /// <returns>A texture owned by this artwork set.</returns>
    private Texture2D CreateRoundedTexture(GraphicsDevice graphicsDevice, int width, int height, int radius, Color fill, Color border)
    {
        return CreateTexture(graphicsDevice, width, height, (x, y) =>
        {
            var distance = RoundedDistance(x, y, width, height, radius);

            return (distance > -1.5f ? border : fill) * Math.Clamp(0.5f - distance, 0, 1);
        });
    }

    /// <summary>Creates and tracks a texture from a pixel-color function.</summary>
    /// <param name="graphicsDevice">The borrowed graphics device.</param>
    /// <param name="width">The texture width.</param>
    /// <param name="height">The texture height.</param>
    /// <param name="getColor">The function supplying each pixel's premultiplied color.</param>
    /// <returns>A texture owned by this artwork set.</returns>
    private Texture2D CreateTexture(GraphicsDevice graphicsDevice, int width, int height, Func<int, int, Color> getColor)
    {
        var pixels = new Color[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                pixels[y * width + x] = getColor(x, y);
            }
        }

        var texture = new Texture2D(graphicsDevice, width, height);

        _textures.Add(texture);
        texture.SetData(pixels);

        return texture;
    }

    /// <summary>Calculates antialiased coverage for a rounded rectangle.</summary>
    /// <param name="x">The pixel's horizontal coordinate.</param>
    /// <param name="y">The pixel's vertical coordinate.</param>
    /// <param name="width">The rectangle width.</param>
    /// <param name="height">The rectangle height.</param>
    /// <param name="radius">The corner radius.</param>
    /// <returns>A coverage value from zero to one.</returns>
    private static float RoundedCoverage(int x, int y, int width, int height, int radius) =>
        Math.Clamp(0.5f - RoundedDistance(x, y, width, height, radius), 0, 1);

    /// <summary>Measures signed distance from a pixel center to a rounded rectangle's edge.</summary>
    /// <param name="x">The pixel's horizontal coordinate.</param>
    /// <param name="y">The pixel's vertical coordinate.</param>
    /// <param name="width">The rectangle width.</param>
    /// <param name="height">The rectangle height.</param>
    /// <param name="radius">The corner radius.</param>
    /// <returns>Negative distance inside the rectangle and positive distance outside it.</returns>
    private static float RoundedDistance(int x, int y, int width, int height, int radius)
    {
        var offset = new Vector2(Math.Abs(x + 0.5f - width / 2f), Math.Abs(y + 0.5f - height / 2f));

        offset -= new Vector2(width / 2f - radius, height / 2f - radius);

        return Vector2.Max(offset, Vector2.Zero).Length() + Math.Min(Math.Max(offset.X, offset.Y), 0) - radius;
    }

    /// <summary>Tests a normalized point against an original geometric suit silhouette.</summary>
    /// <param name="position">The normalized point within the suit image.</param>
    /// <param name="suitIndex">The suit index in clubs, diamonds, hearts, and spades order.</param>
    /// <returns>Whether the point is inside the suit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="suitIndex"/> does not identify one of the four suits.
    /// </exception>
    private static bool ContainsSuit(Vector2 position, int suitIndex)
    {
        var horizontal = position.X;
        var vertical = position.Y;
        var stem = vertical >= 0.3f && vertical <= 0.9f &&
            Math.Abs(horizontal) <= 0.12f + (vertical - 0.3f) * 0.25f;

        return suitIndex switch
        {
            0 => stem || Vector2.DistanceSquared(position, new Vector2(0, -0.42f)) <= 0.19f ||
                Vector2.DistanceSquared(position, new Vector2(-0.4f, 0.08f)) <= 0.19f ||
                Vector2.DistanceSquared(position, new Vector2(0.4f, 0.08f)) <= 0.19f,
            1 => (Math.Abs(horizontal) / 0.72f) + (Math.Abs(vertical) / 0.95f) <= 1,
            2 => ContainsHeart(horizontal, vertical),
            3 => ContainsHeart(horizontal, -vertical - 0.12f) || stem,
            _ => throw new ArgumentOutOfRangeException(nameof(suitIndex))
        };
    }

    /// <summary>Tests a normalized point against the circles and triangle that form a heart.</summary>
    /// <param name="x">The normalized horizontal coordinate.</param>
    /// <param name="y">The normalized vertical coordinate.</param>
    /// <returns>Whether the point is inside the heart.</returns>
    private static bool ContainsHeart(float x, float y) =>
        Vector2.DistanceSquared(new Vector2(x, y), new Vector2(-0.34f, -0.3f)) <= 0.18f ||
        Vector2.DistanceSquared(new Vector2(x, y), new Vector2(0.34f, -0.3f)) <= 0.18f ||
        (y >= -0.22f && y <= 0.88f && Math.Abs(x) <= (0.88f - y) * 0.69f);
}
