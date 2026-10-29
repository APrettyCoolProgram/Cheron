// 261029_code
// 261029_documentation

using Microsoft.Xna.Framework;

namespace Cheron.Games.Solitaire;

/// <summary>Describes a card's position and visibility before a logical operation.</summary>
/// <param name="Bounds">The full card rectangle in board coordinates.</param>
/// <param name="FaceUp">Whether the card face was visible.</param>
internal readonly record struct CardPresentation(Rectangle Bounds, bool FaceUp);

/// <summary>Describes the endpoints and delay of one presentation-only card transition.</summary>
/// <param name="Start">The source position and face state.</param>
/// <param name="End">The destination position and face state.</param>
/// <param name="Delay">The delay before this transition begins, in seconds.</param>
internal readonly record struct CardTransition(CardPresentation Start, CardPresentation End, float Delay);
