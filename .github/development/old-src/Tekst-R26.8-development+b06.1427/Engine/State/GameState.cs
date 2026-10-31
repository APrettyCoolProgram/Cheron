// 260620_code
// 260620_documentation

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

    /// <summary>
    /// Main game loop that continues until the game is over. On each iteration, it prompts the player for input,
    /// processes the input, and updates the game state accordingly.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="processor">The command processor responsible for handling player input.</param>
    /// <param name="cartData">The cartridge data containing game rules and content.</param>
    public static void MainGameLoop(GameState state, CommandProcessor processor, CartridgeShell cartData)
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

    /// <summary>Describes the current room to the player, including its name, description, items present, and available exits.</summary>
    /// <param name="state">The current game state.</param>
    public static void OpeningRoom(GameState state)
    {
        Console.WriteLine(CommandProcessor.DescribeRoom(state.CurrentRoom, state));
        Console.WriteLine();
    }
}