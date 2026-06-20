// 260619_code
// 260619_documentation

namespace Tekst.Credits;

/// <summary>Handles displaying closing credits and end-of-game information.</summary>
public static class ClosingCredit
{
    /// <summary>Displays a closing message indicating the game has ended.</summary>
    /// <param name="state">The current game state.</param>
    public static void Fin(Tekst.State.GameState state)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"[ Game ended after {state.TurnCount} turn{(state.TurnCount == 1 ? "" : "s")}. ]");
        Console.ResetColor();
    }
}