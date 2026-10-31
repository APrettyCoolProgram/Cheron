// 261029_code
// 261029_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Centralizes fixed values used by Solitaire's win celebration particle burst.</summary>
internal static class SolitaireCelebrationConstants
{
    /// <summary>The number of particles emitted by the celebration burst.</summary>
    internal const int ParticleCount = 28;

    /// <summary>The horizontal origin of the celebration burst.</summary>
    internal const int OriginX = 550;

    /// <summary>The vertical origin of the celebration burst.</summary>
    internal const int OriginY = 410;

    /// <summary>The base horizontal offset from the origin before speed is applied.</summary>
    internal const int BaseHorizontalOffset = 80;

    /// <summary>The initial horizontal speed of the first particle.</summary>
    internal const int BaseHorizontalSpeed = 42;

    /// <summary>The horizontal speed increase per particle index.</summary>
    internal const int HorizontalSpeedStep = 5;

    /// <summary>The base upward offset applied before gravity-like acceleration.</summary>
    internal const int BaseRise = 145;

    /// <summary>The number of particles in one repeating rise-variation cycle.</summary>
    internal const int RiseVariationCycle = 7;

    /// <summary>The upward offset increase per rise-variation step.</summary>
    internal const int RiseVariationStep = 18;

    /// <summary>The downward acceleration term for particle motion.</summary>
    internal const int VerticalAcceleration = 100;

    /// <summary>The left culling bound for celebration particles.</summary>
    internal const int MinX = 32;

    /// <summary>The right culling bound for celebration particles.</summary>
    internal const int MaxX = 1062;

    /// <summary>The top culling bound for celebration particles.</summary>
    internal const int MinY = 82;

    /// <summary>The bottom culling bound for celebration particles.</summary>
    internal const int MaxY = 714;

    /// <summary>The width of each rendered celebration particle.</summary>
    internal const int ParticleWidth = 4;

    /// <summary>The height of each rendered celebration particle.</summary>
    internal const int ParticleHeight = 6;
}
