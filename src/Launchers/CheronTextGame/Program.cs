// 260619_code
// 260619_documentation

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

        /* Separating these so it's easier to understand what is what.
         */
        Tekst.World.GameWorld          _;
        Tekst.State.GameState?         state;
        Tekst.Engine.CommandProcessor? processor;
        CartridgeData?                 cartData;

        (_, state, processor, cartData) = CartridgeLoader.Load($"{args[0]}.tekst");

        Tekst.Credits.OpeningCredit.GameTitle(cartData);

        Tekst.State.GameState.OpeningRoom(state);

        Tekst.State.GameState.MainGameLoop(state, processor, cartData);

        Tekst.Credits.ClosingCredit.Fin(state);
    }
}