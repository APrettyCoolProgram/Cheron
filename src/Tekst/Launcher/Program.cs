// 260620_code
// 2606201_documentation

using Tekst.Cartridge;

/// <summary> entry point of the application.</summary>
/// <remarks>
/// This is the launcher for Tekst engine games, and it only does a few basic things:
/// <list type="number">
/// <item>1. Validates the command-line arguments.</item>
/// <item>2. Loads the game cartridge.</item>
/// <item>3. Displays the opening credits.</item>
/// <item>4. Starts the main game loop.</item>
/// <item>5. Displays the closing credits.</item>
/// </list>
/// </remarks>
public static class Program
{
    /// <summary>Main method where the execution starts.</summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        /* TODO Eventually move this to a common cartridge class?
         */
        if (args.Length == 0)
        {
            Console.WriteLine("\nMissing game name\n");
            return;
        }

        Launch(args[0]);

    }

    /// <summary>Launches the game by loading the cartridge, displaying the opening credits, and starting the main game loop.</summary>
    /// <param name="gameName">The name of the game to launch.</param>
    private static void Launch(string gameName)
    {
        /* Separating these for now so it's easier to understand what is what.
         */
        Tekst.World.GameWorld          _;
        Tekst.State.GameState?         state;
        Tekst.Engine.CommandProcessor? processor;
        CartridgeData?                 cartData;

        (_, state, processor, cartData) = CartridgeLoader.Load($"{gameName}.tekst");

        Tekst.Credits.OpeningCredit.GameTitle(cartData);

        Tekst.State.GameState.OpeningRoom(state);

        Tekst.State.GameState.MainGameLoop(state, processor, cartData);

        Tekst.Credits.ClosingCredit.Fin(state);
    }
}