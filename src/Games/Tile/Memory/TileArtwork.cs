// 261031_code
// 261031_documentation

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cheron.Games.Tile.Memory;

/// <summary>Creates and owns the textures used by Tile's modern rounded-tile theme.</summary>
/// <remarks>Mirrors the rounded-panel, soft-shadow approach used by <c>SolitaireArtwork</c> for a consistent look.</remarks>
internal sealed class TileArtwork : IDisposable
{
    private readonly List<Texture2D> _textures = [];

    /// <summary>Creates cached artwork on the graphics thread without external assets.</summary>
    /// <param name="graphicsDevice">The borrowed device used to create textures.</param>
    /// <param name="boardWidth">The logical board width used to size the background texture.</param>
    /// <param name="boardHeight">The logical board height used to size the background texture.</param>
    /// <param name="tileSize">The side length, in pixels, of each square tile texture.</param>
    /// <param name="cornerRadius">The corner radius, in pixels, applied to every rounded tile.</param>
    /// <param name="frontColors">The solid front-face colors, one per matching pair.</param>
    /// <param name="buttonWidth">The width, in pixels, of a toolbar button texture.</param>
    /// <param name="buttonHeight">The height, in pixels, of a toolbar button texture.</param>
    /// <param name="buttonCornerRadius">The corner radius, in pixels, applied to buttons and overlay panels.</param>
    /// <param name="panelWidth">The width, in pixels, of the centered overlay panel texture.</param>
    /// <param name="panelHeight">The height, in pixels, of the centered overlay panel texture.</param>
    public TileArtwork(GraphicsDevice graphicsDevice, int boardWidth, int boardHeight, int tileSize, int cornerRadius,
        IReadOnlyList<Color> frontColors, int buttonWidth, int buttonHeight, int buttonCornerRadius,
        int panelWidth, int panelHeight)
    {
        Background = CreateTexture(graphicsDevice, boardWidth, boardHeight, (x, y) =>
        {
            var distance = Vector2.Distance(new Vector2(x, y), new Vector2(boardWidth / 2f, boardHeight * 0.3f)) /
                (boardWidth + boardHeight);

            return Color.Lerp(new Color(44, 30, 46), TileColors.TableBackground, Math.Clamp(distance * 1.6f, 0, 1));
        });

        TileShadow = CreateTexture(graphicsDevice, tileSize + 16, tileSize + 16, (x, y) =>
        {
            var distance = RoundedDistance(x - 8, y - 8, tileSize, tileSize, cornerRadius);

            return Color.Black * (0.3f * Math.Clamp(1 - distance / 8, 0, 1));
        });

        TileBack = CreateRoundedTexture(graphicsDevice, tileSize, tileSize, cornerRadius,
            new Color(120, 32, 42), new Color(176, 70, 82));

        var frontTextures = new Texture2D[frontColors.Count];
        for (var index = 0; index < frontColors.Count; index++)
        {
            var baseColor = frontColors[index];
            var borderColor = Color.Lerp(baseColor, Color.White, 0.35f);
            frontTextures[index] = CreateRoundedTexture(graphicsDevice, tileSize, tileSize, cornerRadius, baseColor, borderColor);
        }

        TileFronts = frontTextures;

        FocusOutline = CreateRoundedTexture(graphicsDevice, tileSize + 12, tileSize + 12, cornerRadius + 4,
            new Color(0, 0, 0, 0), new Color(255, 221, 120));

        Button = CreateRoundedTexture(graphicsDevice, buttonWidth, buttonHeight, buttonCornerRadius,
            new Color(96, 48, 90), new Color(160, 96, 150));
        ButtonHover = CreateRoundedTexture(graphicsDevice, buttonWidth, buttonHeight, buttonCornerRadius,
            new Color(130, 68, 118), new Color(210, 140, 190));

        Banner = CreateRoundedTexture(graphicsDevice, panelWidth, panelHeight, buttonCornerRadius + 4,
            new Color(28, 20, 32), new Color(176, 96, 150));
    }

    /// <summary>Gets the softly shaded table background.</summary>
    public Texture2D Background { get; }

    /// <summary>Gets the soft tile shadow, including an eight-pixel margin.</summary>
    public Texture2D TileShadow { get; }

    /// <summary>Gets the rounded maroon tile back.</summary>
    public Texture2D TileBack { get; }

    /// <summary>Gets the rounded solid-color tile fronts, indexed by pair color id.</summary>
    public IReadOnlyList<Texture2D> TileFronts { get; }

    /// <summary>Gets the glowing outline drawn around the keyboard-focused tile.</summary>
    public Texture2D FocusOutline { get; }

    /// <summary>Gets the toolbar button background.</summary>
    public Texture2D Button { get; }

    /// <summary>Gets the highlighted toolbar button background.</summary>
    public Texture2D ButtonHover { get; }

    /// <summary>Gets the centered overlay panel used for the help screen and win celebration.</summary>
    public Texture2D Banner { get; }

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

            return (distance > -2.5f ? border : fill) * Math.Clamp(0.5f - distance, 0, 1);
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
}
