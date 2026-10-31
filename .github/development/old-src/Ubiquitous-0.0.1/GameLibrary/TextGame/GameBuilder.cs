using TextGame.Loaders;

namespace TextGame;

/// <summary>
/// Utility class for building and loading text adventure games
/// </summary>
/// <remarks>
/// Provides static methods to:
/// - Load games from JSON cartridge files
/// - Build a sample game for testing and demonstration
/// - Discover available cartridges in a directory
/// This class serves as the main entry point for game creation and loading.
/// </remarks>
public class GameBuilder
{
    /// <summary>
    /// Loads a game from a JSON cartridge file.
    /// </summary>
    /// <param name="cartridgePath">Path to the JSON cartridge file</param>
    /// <returns>Starting room or null if load fails</returns>
    /// <remarks>
    /// This method delegates to CartridgeLoader.LoadFromJson() to handle
    /// JSON deserialization and game graph construction.
    /// </remarks>
    public static Room? LoadFromCartridge(string cartridgePath)
    {
        return CartridgeLoader.LoadFromJson(cartridgePath);
    }

    /// <summary>
    /// Builds the default sample game (Haunted Mansion).
    /// This method is kept for backward compatibility.
    /// </summary>
    /// <returns>The starting room of the sample game</returns>
    /// <remarks>
    /// Creates a hardcoded game with:
    /// - 4 rooms: Entrance Hall, Library, Garden, Cellar
    /// - 4 items: key, lamp, statue, book
    /// This method demonstrates programmatic game creation and provides
    /// a quick way to test the game engine without loading a cartridge.
    /// </remarks>
    public static Room BuildSampleGame()
    {
        var entrance = new Room("Entrance Hall", 
            "You are standing in a grand entrance hall. The marble floor gleams in the dim light.");
        
        var library = new Room("Library", 
            "You are in a dusty library. Books line the walls from floor to ceiling.");
        
        var garden = new Room("Garden", 
            "You are in a peaceful garden. Flowers bloom everywhere and birds chirp softly.");
        
        var cellar = new Room("Cellar", 
            "You are in a dark, damp cellar. The air smells of mold and old wine.");

        entrance.AddExit("north", library);
        entrance.AddExit("east", garden);
        entrance.AddExit("down", cellar);

        library.AddExit("south", entrance);
        garden.AddExit("west", entrance);
        cellar.AddExit("up", entrance);

        var key = new Item("key", "A small brass key with intricate engravings.", true, "The key fits into a lock somewhere.");
        var lamp = new Item("lamp", "An old oil lamp. It might still work.", true, "The lamp flickers to life, illuminating the area.");
        var statue = new Item("statue", "A heavy marble statue of a lion.", false, "The statue is too heavy to move.");
        var book = new Item("book", "An ancient tome with strange symbols on the cover.", true, "As you open the book, the symbols seem to shimmer.");

        entrance.AddItem(key);
        library.AddItem(book);
        library.AddItem(statue);
        garden.AddItem(lamp);

        return entrance;
    }

    /// <summary>
    /// Finds all cartridge files in the specified directory.
    /// </summary>
    /// <param name="cartridgeDirectory">Directory to search for cartridges</param>
    /// <returns>List of cartridge file paths</returns>
    /// <remarks>
    /// Searches recursively for all .json files in the specified directory.
    /// Useful for implementing cartridge browsing and selection UI.
    /// If the directory doesn't exist, it will be created.
    /// </remarks>
    public static List<string> FindCartridges(string cartridgeDirectory)
    {
        return CartridgeLoader.FindCartridges(cartridgeDirectory);
    }
}
