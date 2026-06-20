// 260619_code
// 260619_documentation

using Tekst.Cartridge;
using Tekst.Engine;
using Tekst.Models;

namespace Tekst.State;

/// <summary>Mutable snapshot of all runtime game data.</summary>
public class GameState
{
    /// <summary>The room the player is currently standing in.</summary>
    public required Room CurrentRoom { get; set; }

    /// <summary>Items the player is currently carrying.</summary>
    public List<Item> Inventory { get; init; } = [];

    /// <summary>Number of turns the player has taken.</summary>
    public int TurnCount { get; set; }

    /// <summary>Whether the winning condition has been reached.</summary>
    public bool IsGameWon { get; set; }

    /// <summary>Whether the player has chosen to quit.</summary>
    public bool IsGameOver { get; set; }

    public static void MainGameLoop(Tekst.State.GameState state, CommandProcessor processor, CartridgeData cartData)
    {
        while (!state.IsGameOver)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("> ");
            Console.ResetColor();

            var input = Console.ReadLine() ?? string.Empty;
            Console.WriteLine();

            var result = processor.Process(input, state);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(result.Message);
            Console.ResetColor();
            Console.WriteLine();

            var winMessage = WinEvaluator.Check(cartData.WinRule, state);

            if (winMessage is not null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(winMessage);
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }

    public static void OpeningRoom(Tekst.State.GameState state)
    {
        string test = CommandProcessor.DescribeRoom(state.CurrentRoom, state);

        Console.WriteLine(CommandProcessor.DescribeRoom(state.CurrentRoom, state));
        Console.WriteLine();
    }
}