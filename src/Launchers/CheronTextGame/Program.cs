// 260620_code
// 260620_documentation

using Tekst.Cartridge;

/// <summary> entry point of the application.</summary>
public static class Program
{
    /// <summary>Main method where the execution starts.</summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        /* TODO Eventually move this to a common cartridge class.
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
        /* Separating these so it's easier to understand what is what.
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