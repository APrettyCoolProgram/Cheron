// 260806_code
// 260806_documentation

using CheronEngine.Trove;

namespace CheronTekst;

internal static class Program
{
    /// <summary>Entry method for CheronTekst.</summary>
    /// <param name="args">Command-line arguments.</param>
    internal static void Main(string[] args)
    {
        ValidateArguments(args);
    }

    /// <summary>Validates the command-line arguments.</summary>
    /// <param name="args">Command-line arguments.</param>
    private static void ValidateArguments(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine(UsrMsg.MissingGameName);

            return;
        }
    }
}