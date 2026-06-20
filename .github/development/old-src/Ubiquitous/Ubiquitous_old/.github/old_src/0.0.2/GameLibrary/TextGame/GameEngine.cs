namespace TextGame;

/// <summary>
/// Core game engine for text-based adventure games.
/// </summary>
public class GameEngine
{
    private readonly Dictionary<string, Room> _rooms = new();
    private readonly Dictionary<string, Item> _items = new();
    private readonly Player _player = new();
    private bool _isRunning;

    /// <summary>
    /// Event raised when the game outputs text to the player.
    /// </summary>
    public event EventHandler<string>? OutputGenerated;

    /// <summary>
    /// Gets the current player state.
    /// </summary>
    public Player Player => _player;

    /// <summary>
    /// Gets whether the game is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Initializes the game with rooms and items.
    /// </summary>
    /// <param name="rooms">Dictionary of rooms in the game world.</param>
    /// <param name="items">Dictionary of items in the game world.</param>
    /// <param name="startingRoomId">The ID of the room where the player starts.</param>
    public void Initialize(Dictionary<string, Room> rooms, Dictionary<string, Item> items, string startingRoomId)
    {
        _rooms.Clear();
        _items.Clear();
        
        foreach (var room in rooms)
        {
            _rooms.Add(room.Key, room.Value);
        }
        
        foreach (var item in items)
        {
            _items.Add(item.Key, item.Value);
        }

        _player.CurrentRoomId = startingRoomId;
        _player.Inventory.Clear();
        _player.Score = 0;
        _player.MoveCount = 0;
        _isRunning = true;

        DisplayRoom();
    }

    /// <summary>
    /// Processes a player command.
    /// </summary>
    /// <param name="command">The command string entered by the player.</param>
    public void ProcessCommand(string command)
    {
        if (!_isRunning || string.IsNullOrWhiteSpace(command))
            return;

        var parts = command.ToLower().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return;

        var verb = parts[0];
        var noun = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;

        switch (verb)
        {
            case "go":
            case "move":
            case "walk":
            case "n":
            case "s":
            case "e":
            case "w":
            case "north":
            case "south":
            case "east":
            case "west":
                HandleMove(verb == "go" || verb == "move" || verb == "walk" ? noun : verb);
                break;

            case "look":
            case "l":
                if (string.IsNullOrEmpty(noun))
                    DisplayRoom();
                else
                    ExamineItem(noun);
                break;

            case "examine":
            case "x":
                ExamineItem(noun);
                break;

            case "take":
            case "get":
            case "pick":
                TakeItem(noun);
                break;

            case "drop":
                DropItem(noun);
                break;

            case "inventory":
            case "i":
                ShowInventory();
                break;

            case "use":
                UseItem(noun);
                break;

            case "quit":
            case "exit":
                _isRunning = false;
                Output("Thanks for playing!");
                break;

            case "help":
            case "?":
                ShowHelp();
                break;

            default:
                Output("I don't understand that command. Type 'help' for a list of commands.");
                break;
        }
    }

    /// <summary>
    /// Handles player movement between rooms.
    /// </summary>
    private void HandleMove(string direction)
    {
        var currentRoom = GetCurrentRoom();
        if (currentRoom == null)
            return;

        var dir = direction switch
        {
            "n" => "north",
            "s" => "south",
            "e" => "east",
            "w" => "west",
            _ => direction
        };

        if (currentRoom.Exits.TryGetValue(dir, out var nextRoomId))
        {
            _player.CurrentRoomId = nextRoomId;
            _player.MoveCount++;
            DisplayRoom();
        }
        else
        {
            Output("You can't go that way.");
        }
    }

    /// <summary>
    /// Displays the current room's description.
    /// </summary>
    private void DisplayRoom()
    {
        var room = GetCurrentRoom();
        if (room == null)
            return;

        Output($"\n{room.Name}");
        Output(room.Description);

        if (room.Items.Count > 0)
        {
            Output("\nYou can see:");
            foreach (var itemId in room.Items)
            {
                if (_items.TryGetValue(itemId, out var item))
                {
                    Output($"  - {item.Name}");
                }
            }
        }

        if (room.Exits.Count > 0)
        {
            Output($"\nExits: {string.Join(", ", room.Exits.Keys)}");
        }

        room.IsVisited = true;
    }

    /// <summary>
    /// Examines an item in the current room or inventory.
    /// </summary>
    private void ExamineItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Output("Examine what?");
            return;
        }

        var room = GetCurrentRoom();
        if (room == null)
            return;

        var item = FindItemByName(itemName, room.Items.Concat(_player.Inventory).ToList());
        if (item != null)
        {
            Output(item.Description);
        }
        else
        {
            Output($"You don't see any '{itemName}' here.");
        }
    }

    /// <summary>
    /// Takes an item from the current room.
    /// </summary>
    private void TakeItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Output("Take what?");
            return;
        }

        var room = GetCurrentRoom();
        if (room == null)
            return;

        var item = FindItemByName(itemName, room.Items);
        if (item == null)
        {
            Output($"There is no '{itemName}' here.");
            return;
        }

        if (!item.IsCollectable)
        {
            Output($"You can't take the {item.Name}.");
            return;
        }

        room.Items.Remove(item.Id);
        _player.AddItem(item.Id);
        Output($"You take the {item.Name}.");
    }

    /// <summary>
    /// Drops an item from inventory into the current room.
    /// </summary>
    private void DropItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Output("Drop what?");
            return;
        }

        var item = FindItemByName(itemName, _player.Inventory);
        if (item == null)
        {
            Output($"You don't have a '{itemName}'.");
            return;
        }

        var room = GetCurrentRoom();
        if (room == null)
            return;

        _player.RemoveItem(item.Id);
        room.Items.Add(item.Id);
        Output($"You drop the {item.Name}.");
    }

    /// <summary>
    /// Uses an item from the player's inventory.
    /// </summary>
    private void UseItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Output("Use what?");
            return;
        }

        var item = FindItemByName(itemName, _player.Inventory);
        if (item == null)
        {
            Output($"You don't have a '{itemName}'.");
            return;
        }

        if (!item.IsUsable)
        {
            Output($"You can't use the {item.Name}.");
            return;
        }

        Output($"You use the {item.Name}.");
    }

    /// <summary>
    /// Shows the player's inventory.
    /// </summary>
    private void ShowInventory()
    {
        if (_player.Inventory.Count == 0)
        {
            Output("You are not carrying anything.");
            return;
        }

        Output("You are carrying:");
        foreach (var itemId in _player.Inventory)
        {
            if (_items.TryGetValue(itemId, out var item))
            {
                Output($"  - {item.Name}");
            }
        }
    }

    /// <summary>
    /// Shows the help text with available commands.
    /// </summary>
    private void ShowHelp()
    {
        Output("\nAvailable Commands:");
        Output("  Movement: go [direction], north/n, south/s, east/e, west/w");
        Output("  Actions: look/l, examine [item], take [item], drop [item], use [item]");
        Output("  Info: inventory/i, help/?");
        Output("  Game: quit/exit");
    }

    /// <summary>
    /// Gets the current room the player is in.
    /// </summary>
    private Room? GetCurrentRoom()
    {
        return _rooms.TryGetValue(_player.CurrentRoomId, out var room) ? room : null;
    }

    /// <summary>
    /// Finds an item by name from a list of item IDs.
    /// </summary>
    private Item? FindItemByName(string name, List<string> itemIds)
    {
        foreach (var itemId in itemIds)
        {
            if (_items.TryGetValue(itemId, out var item) && 
                item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// Outputs text to the player through the OutputGenerated event.
    /// </summary>
    private void Output(string text)
    {
        OutputGenerated?.Invoke(this, text);
    }
}
