namespace TextGame;

/// <summary>
/// Builder class for creating and initializing text-based games with predefined scenarios
/// </summary>
public class GameBuilder
{
    /// <summary>
    /// Builds a default game with a sample adventure scenario
    /// </summary>
    /// <returns>A fully initialized and started Game instance</returns>
    public static Game BuildDefaultGame()
    {
        var game = new Game();
        
        var startRoom = CreateEntranceHall();
        var library = CreateLibrary();
        var garden = CreateGarden();
        
        ConnectRooms(startRoom, library, garden);
        PopulateRooms(startRoom, library, garden);
        
        var player = new Player();
        game.Initialize(startRoom, player);
        game.Start();
        
        return game;
    }
    
    /// <summary>
    /// Creates the entrance hall room
    /// </summary>
    private static Room CreateEntranceHall()
    {
        return new Room("The Entrance Hall", 
            "You stand in a grand entrance hall. Dust motes dance in the light streaming through high windows. " +
            "The air smells of old wood and forgotten memories.");
    }
    
    /// <summary>
    /// Creates the library room
    /// </summary>
    private static Room CreateLibrary()
    {
        return new Room("The Library",
            "Towering bookshelves line the walls, filled with ancient tomes. " +
            "A large oak desk sits in the center of the room.");
    }
    
    /// <summary>
    /// Creates the garden room
    /// </summary>
    private static Room CreateGarden()
    {
        return new Room("The Garden",
            "A beautiful but overgrown garden stretches before you. " +
            "Roses and ivy compete for space along crumbling stone walls.");
    }
    
    /// <summary>
    /// Connects rooms with directional exits
    /// </summary>
    private static void ConnectRooms(Room entranceHall, Room library, Room garden)
    {
        entranceHall.AddExit("north", library);
        library.AddExit("south", entranceHall);
        entranceHall.AddExit("east", garden);
        garden.AddExit("west", entranceHall);
    }
    
    /// <summary>
    /// Populates rooms with items
    /// </summary>
    private static void PopulateRooms(Room entranceHall, Room library, Room garden)
    {
        var key = new Item("key", "A small brass key with intricate engravings.", isPickupable: true);
        var book = new Item("book", "An old leather-bound journal with yellowed pages.", isPickupable: true);
        var desk = new Item("desk", "A solid oak desk with several drawers.", isPickupable: false);
        var rose = new Item("rose", "A beautiful red rose, still fresh despite the garden's decay.", isPickupable: true);

        entranceHall.AddItem(key);
        library.AddItem(book);
        library.AddItem(desk);
        garden.AddItem(rose);
    }
}
