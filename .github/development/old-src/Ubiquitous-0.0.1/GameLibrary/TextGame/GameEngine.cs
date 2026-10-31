namespace TextGame;

/// <summary>
/// Core game engine for text-based adventure games
/// </summary>
/// <remarks>
/// The GameEngine class manages all game state including the current room,
/// player inventory, and game running status. It processes player commands,
/// handles room navigation, item interactions, and generates text output
/// through an event-driven architecture.
/// </remarks>
public class GameEngine
{
    private Room _currentRoom;
    private Dictionary<string, Room> _rooms;
    private Dictionary<string, Item> _inventory;
    private bool _isRunning;

    /// <summary>
    /// Event raised when the game generates output text
    /// </summary>
    /// <remarks>
    /// Subscribe to this event to receive all game output including room descriptions,
    /// command responses, error messages, and help text. This allows for flexible
    /// output handling (console, GUI, logging, etc.).
    /// </remarks>
    public event EventHandler<string>? OutputGenerated;

    /// <summary>
    /// Initializes a new instance of the GameEngine class
    /// </summary>
    /// <remarks>
    /// Creates empty room and inventory dictionaries and sets the game state to not running.
    /// Call Start() with a starting room to begin gameplay.
    /// </remarks>
    public GameEngine()
    {
        _rooms = new Dictionary<string, Room>();
        _inventory = new Dictionary<string, Item>();
        _isRunning = false;
        _currentRoom = null!;
    }

    /// <summary>
    /// Starts the game at the specified starting room
    /// </summary>
    /// <param name="startingRoom">The room where the game begins</param>
    /// <remarks>
    /// Sets the current room, marks the game as running, displays a welcome message,
    /// and describes the starting room. This should be called before processing any commands.
    /// </remarks>
    public void Start(Room startingRoom)
    {
        _currentRoom = startingRoom;
        _isRunning = true;
        Output("Welcome to the Text Adventure Game!");
        Output("");
        DescribeCurrentRoom();
    }

    /// <summary>
    /// Processes a player command
    /// </summary>
    /// <param name="input">The command string entered by the player</param>
    /// <remarks>
    /// Parses the input string and executes the appropriate action. Supported commands include:
    /// go/move, look, take/get, inventory/inv, use, examine, help, quit/exit.
    /// Commands are case-insensitive and multi-word item names are supported.
    /// </remarks>
    public void ProcessCommand(string input)
    {
        if (!_isRunning)
        {
            Output("Game is not running. Please start a new game.");
            return;
        }

        var command = input.Trim().ToLower().Split(' ');
        var action = command[0];

        switch (action)
        {
            case "go":
            case "move":
                if (command.Length > 1)
                    Move(command[1]);
                else
                    Output("Go where? Specify a direction (north, south, east, west).");
                break;

            case "look":
                DescribeCurrentRoom();
                break;

            case "take":
            case "get":
                if (command.Length > 1)
                    TakeItem(string.Join(" ", command.Skip(1)));
                else
                    Output("Take what?");
                break;

            case "inventory":
            case "inv":
                ShowInventory();
                break;

            case "use":
                if (command.Length > 1)
                    UseItem(string.Join(" ", command.Skip(1)));
                else
                    Output("Use what?");
                break;

            case "examine":
                if (command.Length > 1)
                    ExamineItem(string.Join(" ", command.Skip(1)));
                else
                    Output("Examine what?");
                break;

            case "help":
                ShowHelp();
                break;

            case "quit":
            case "exit":
                _isRunning = false;
                Output("Thanks for playing!");
                break;

            default:
                Output($"I don't understand '{input}'. Type 'help' for available commands.");
                break;
        }
    }

    /// <summary>
    /// Moves the player in the specified direction
    /// </summary>
    /// <param name="direction">The direction to move (north, south, east, west, up, down)</param>
    /// <remarks>
    /// Checks if the current room has an exit in the specified direction.
    /// If valid, moves to the new room and describes it. Otherwise, displays an error message.
    /// </remarks>
    private void Move(string direction)
    {
        if (_currentRoom.Exits.TryGetValue(direction, out var nextRoom))
        {
            _currentRoom = nextRoom;
            Output($"You move {direction}.");
            Output("");
            DescribeCurrentRoom();
        }
        else
        {
            Output($"You can't go {direction} from here.");
        }
    }

    /// <summary>
    /// Displays the description of the current room
    /// </summary>
    /// <remarks>
    /// Outputs the room name, description, list of visible items, and available exits.
    /// This is called when entering a room or when the player uses the 'look' command.
    /// </remarks>
    private void DescribeCurrentRoom()
    {
        Output($"=== {_currentRoom.Name} ===");
        Output(_currentRoom.Description);

        if (_currentRoom.Items.Any())
        {
            Output("");
            Output("You see:");
            foreach (var item in _currentRoom.Items.Values)
            {
                Output($"  - {item.Name}");
            }
        }

        if (_currentRoom.Exits.Any())
        {
            Output("");
            Output("Exits: " + string.Join(", ", _currentRoom.Exits.Keys));
        }
    }

    /// <summary>
    /// Attempts to take an item from the current room
    /// </summary>
    /// <param name="itemName">The name of the item to take</param>
    /// <remarks>
    /// Searches for the item in the current room (case-insensitive).
    /// If found and takeable, moves it to inventory. Otherwise, displays appropriate error message.
    /// </remarks>
    private void TakeItem(string itemName)
    {
        var item = _currentRoom.Items.Values.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
        if (item != null && item.CanTake)
        {
            _currentRoom.Items.Remove(item.Name);
            _inventory[item.Name] = item;
            Output($"You take the {item.Name}.");
        }
        else if (item != null && !item.CanTake)
        {
            Output($"You can't take the {item.Name}.");
        }
        else
        {
            Output($"There is no {itemName} here.");
        }
    }

    /// <summary>
    /// Displays the player's current inventory
    /// </summary>
    /// <remarks>
    /// Lists all items currently carried by the player.
    /// If inventory is empty, displays an appropriate message.
    /// </remarks>
    private void ShowInventory()
    {
        if (_inventory.Any())
        {
            Output("You are carrying:");
            foreach (var item in _inventory.Values)
            {
                Output($"  - {item.Name}");
            }
        }
        else
        {
            Output("Your inventory is empty.");
        }
    }

    /// <summary>
    /// Uses an item from the player's inventory
    /// </summary>
    /// <param name="itemName">The name of the item to use</param>
    /// <remarks>
    /// Checks if the item exists in inventory and displays its use description.
    /// The item remains in inventory after use (unless removed by game logic).
    /// </remarks>
    private void UseItem(string itemName)
    {
        if (_inventory.TryGetValue(itemName, out var item))
        {
            Output($"You use the {item.Name}. {item.UseDescription}");
        }
        else
        {
            Output($"You don't have a {itemName}.");
        }
    }

    /// <summary>
    /// Examines an item in detail
    /// </summary>
    /// <param name="itemName">The name of the item to examine</param>
    /// <remarks>
    /// Searches for the item in both inventory and current room.
    /// Displays the item's detailed description if found.
    /// </remarks>
    private void ExamineItem(string itemName)
    {
        var item = _inventory.Values.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower()) 
                   ?? _currentRoom.Items.Values.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());

        if (item != null)
        {
            Output(item.Description);
        }
        else
        {
            Output($"There is no {itemName} here or in your inventory.");
        }
    }

    /// <summary>
    /// Displays the help message with available commands
    /// </summary>
    /// <remarks>
    /// Lists all supported commands with brief descriptions of their usage.
    /// This is shown when the player types 'help'.
    /// </remarks>
    private void ShowHelp()
    {
        Output("Available commands:");
        Output("  go/move [direction] - Move in a direction (north, south, east, west)");
        Output("  look - Look around the current room");
        Output("  take/get [item] - Take an item");
        Output("  inventory/inv - Show your inventory");
        Output("  use [item] - Use an item from your inventory");
        Output("  examine [item] - Examine an item closely");
        Output("  help - Show this help message");
        Output("  quit/exit - Quit the game");
    }

    /// <summary>
    /// Sends output to subscribers of the OutputGenerated event
    /// </summary>
    /// <param name="message">The message to output</param>
    /// <remarks>
    /// Internal method used to generate all game output through the event system.
    /// </remarks>
    private void Output(string message)
    {
        OutputGenerated?.Invoke(this, message);
    }

    /// <summary>
    /// Gets a value indicating whether the game is currently running
    /// </summary>
    /// <value>true if the game is running; otherwise, false</value>
    /// <remarks>
    /// The game is running after Start() is called and until the player quits.
    /// </remarks>
    public bool IsRunning => _isRunning;
}
