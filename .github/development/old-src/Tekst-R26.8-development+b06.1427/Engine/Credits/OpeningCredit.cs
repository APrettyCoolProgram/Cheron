// 260619_code
// 260619_documentation

using Tekst.Cartridge;

namespace Tekst.Credits;

/// <summary>Handles displaying the opening credits.</summary>
public static class OpeningCredit
{
    /// <summary>Displays the game title and introductory information from the cartridge data.</summary>
    /// <param name="cartData">The cartridge data containing the title and introductory information.</param>
    public static void GameTitle(CartridgeShell cartData)
    {
        Console.Clear();

        SetColorScheme(cartData.Detail.GameColorScheme);

        DisplayBanner(cartData.Title.BannerContents);

        Console.WriteLine();
        Console.WriteLine(cartData.Title.Story);
        Console.WriteLine(cartData.Title.Instructions);
        Console.WriteLine();
    }

    /// <summary>Displays the banner lines on the console.</summary>
    /// <param name="banner">The list of banner lines to display.</param>
    public static void DisplayBanner(List<string> banner)
    {
        foreach (var line in banner)
        {
            Console.WriteLine(line);
        }
    }

    /// <summary>Sets the console's foreground and background colors based on a color scheme string.</summary>
    /// <param name="colorScheme">A string representing the color scheme in the format "Background,Foreground".</param>
    public static void SetColorScheme(string colorScheme)
    {
        string[] colors = colorScheme.Split(',');

        Console.BackgroundColor = GetConsoleColor(colors[0]);
        Console.ForegroundColor = GetConsoleColor(colors[1]);
    }

    /// <summary>Converts a single-character color code to the corresponding ConsoleColor. </summary>
    /// <param name="colorName">A single-character string representing the color code.</param>
    /// <returns>The corresponding ConsoleColor.</returns>
    public static ConsoleColor GetConsoleColor(string colorName)
    {
        return colorName switch
        {
            "B" => ConsoleColor.Black,
            "U" => ConsoleColor.Blue,
            "C" => ConsoleColor.Cyan,
            "E" => ConsoleColor.Gray,
            "G" => ConsoleColor.Green,
            "M" => ConsoleColor.Magenta,
            "R" => ConsoleColor.Red,
            "W" => ConsoleColor.White,
            "Y" => ConsoleColor.Yellow,
            "DU" => ConsoleColor.DarkBlue,
            "DC" => ConsoleColor.DarkCyan,
            "DE" => ConsoleColor.DarkGray,
            "DG" => ConsoleColor.DarkGreen,
            "DM" => ConsoleColor.DarkMagenta,
            "DR" => ConsoleColor.DarkRed,
            "DY" => ConsoleColor.DarkYellow,
            _ => ConsoleColor.Red,
        };
    }
}