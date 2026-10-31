// 261031_code
// 261031_documentation

using System;
using Microsoft.Xna.Framework;

namespace Cheron.Games.Memory;

/// <summary>Defines the color palette used to build Memory's tile artwork.</summary>
internal static class MemoryColors
{
    /// <summary>The background color used while clearing the device before drawing the table texture.</summary>
    public static readonly Color TableBackground = new(30, 20, 32);

    /// <summary>The eight distinct solid colors used for matching tile-front pairs on smaller boards.</summary>
    public static readonly Color[] TileFronts =
    [
        new Color(220, 60, 60),
        new Color(60, 150, 220),
        new Color(90, 200, 100),
        new Color(230, 200, 60),
        new Color(190, 90, 220),
        new Color(240, 140, 50),
        new Color(60, 210, 200),
        new Color(230, 110, 170)
    ];

    /// <summary>Gets the given number of distinct tile-front colors, reusing the curated palette first.</summary>
    /// <param name="pairCount">The number of distinct colors needed, one per matching pair.</param>
    /// <returns>An array of exactly <paramref name="pairCount"/> distinct colors.</returns>
    public static Color[] GetPairColors(int pairCount)
    {
        var colors = new Color[pairCount];
        for (var index = 0; index < pairCount; index++)
        {
            colors[index] = index < TileFronts.Length ? TileFronts[index] : CreateProceduralColor(index);
        }

        return colors;
    }

    /// <summary>Creates an additional, visually distinct color beyond the curated palette using an evenly spaced hue.</summary>
    /// <param name="index">The zero-based pair index, used to derive a unique hue.</param>
    private static Color CreateProceduralColor(int index)
    {
        const float goldenAngle = 137.50776f;
        var hue = (index * goldenAngle) % 360f;
        return FromHsv(hue, 0.62f, 0.85f);
    }

    /// <summary>Converts an HSV color to an RGB <see cref="Color"/>.</summary>
    /// <param name="hue">The hue in degrees, from 0 to 360.</param>
    /// <param name="saturation">The saturation, from 0 to 1.</param>
    /// <param name="value">The value/brightness, from 0 to 1.</param>
    private static Color FromHsv(float hue, float saturation, float value)
    {
        var chroma = value * saturation;
        var huePrime = hue / 60f;
        var x = chroma * (1 - MathF.Abs(huePrime % 2 - 1));
        var m = value - chroma;

        var (r, g, b) = huePrime switch
        {
            < 1 => (chroma, x, 0f),
            < 2 => (x, chroma, 0f),
            < 3 => (0f, chroma, x),
            < 4 => (0f, x, chroma),
            < 5 => (x, 0f, chroma),
            _ => (chroma, 0f, x)
        };

        return new Color(r + m, g + m, b + m);
    }
}
