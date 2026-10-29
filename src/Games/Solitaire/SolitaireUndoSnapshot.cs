// 261028_code
// 261028_documentation

namespace Cheron.Games.Solitaire;

/// <summary>Stores one card reference and its face state independently of subsequent mutations.</summary>
/// <param name="Card">The original card identity.</param>
/// <param name="FaceUp">The saved visibility of the card face.</param>
internal readonly record struct SavedCard(Card Card, bool FaceUp);

/// <summary>Stores every pile's order, face states, and move count before a successful action.</summary>
/// <param name="Piles">Stock, waste, tableau, and foundation contents in enumeration order.</param>
/// <param name="MoveCount">The move count before the action.</param>
internal sealed record BoardSnapshot(SavedCard[][] Piles, int MoveCount);
