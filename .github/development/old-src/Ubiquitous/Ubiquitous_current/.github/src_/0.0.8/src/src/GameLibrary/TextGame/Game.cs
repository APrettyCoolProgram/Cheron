namespace TextGame;

/// <summary>Main game controller that manages the game state and game loop</summary>
public class Game
{
    private readonly CommandParser _parser;
    private readonly World _world;
    private Player? _player;
    private bool _isRunning;

    /// <summary>Event raised when the game outputs text</summary>
    public event Action<string>? OutputGenerated;

    public Game()
    {
        _parser = new CommandParser();
        _world = new World();
        _isRunning = false;
    }

    /// <summary>Initializes the game with a starting room and player</summary>
    public void Initialize(Room startingRoom, Player player)
    {
        _world.AddRoom(startingRoom);
        _player = player;
        _player.CurrentRoom = startingRoom;
    }

    /// <summary>Starts the game</summary>
    public void Start()
    {
        if (_player == null)
        {
            throw new InvalidOperationException("Game must be initialized before starting.");
        }

        _isRunning = true;
        OnGameStart();
        DisplayCurrentLocation();
    }

    /// <summary>Processes a player command and returns the response</summary>
    public void ProcessInput(string input)
    {
        if (!_isRunning || string.IsNullOrWhiteSpace(input))
            return;

        var command = _parser.Parse(input);
        
        if (command == null)
        {
            Output("I don't understand that command.");
            return;
        }

        ExecuteCommand(command);
    }

    /// <summary>Stops the game</summary>
    public void Stop()
    {
        _isRunning = false;
        OnGameEnd();
    }

    private void ExecuteCommand(Command command)
    {
        if (_player == null) return;

        switch (command.Action)
        {
            case CommandAction.Move:
                HandleMove(command);
                break;
            case CommandAction.Look:
                HandleLook(command);
                break;
            case CommandAction.Take:
                HandleTake(command);
                break;
            case CommandAction.Drop:
                HandleDrop(command);
                break;
            case CommandAction.Inventory:
                HandleInventory();
                break;
            case CommandAction.Examine:
                HandleExamine(command);
                break;
            case CommandAction.Use:
                HandleUse(command);
                break;
            case CommandAction.Help:
                HandleHelp();
                break;
            case CommandAction.Quit:
                _isRunning = false;
                break;
            default:
                Output("I don't understand that command.");
                break;
        }
    }

    private void HandleMove(Command command)
    {
        if (_player == null || _player.CurrentRoom == null) return;

        if (command.Direction == null)
        {
            Output("Which direction do you want to go?");
            return;
        }

        var nextRoom = _player.CurrentRoom.GetExit(command.Direction.Value);
        
        if (nextRoom == null)
        {
            Output("You can't go that way.");
            return;
        }

        _player.CurrentRoom = nextRoom;
        _player.MoveCount++;
        Output($"You head {command.Direction.Value.ToString().ToLower()}.");
        DisplayCurrentLocation();
    }

    private void HandleLook(Command command)
    {
        if (_player?.CurrentRoom == null) return;

        if (command.Target == null)
        {
            DisplayCurrentLocation();
        }
        else
        {
            HandleExamine(command);
        }
    }

    private void HandleTake(Command command)
    {
        if (_player?.CurrentRoom == null) return;

        if (command.Target == null)
        {
            Output("What do you want to take?");
            return;
        }

        var item = _player.CurrentRoom.GetItem(command.Target);
        
        if (item == null)
        {
            Output($"There is no {command.Target} here.");
            return;
        }

        if (!item.IsPickupable)
        {
            Output($"You can't take the {item.Name}.");
            return;
        }

        _player.CurrentRoom.RemoveItem(item);
        _player.Inventory.AddItem(item);
        Output($"You take the {item.Name}.");
    }

    private void HandleDrop(Command command)
    {
        if (_player?.CurrentRoom == null) return;

        if (command.Target == null)
        {
            Output("What do you want to drop?");
            return;
        }

        var item = _player.Inventory.GetItem(command.Target);
        
        if (item == null)
        {
            Output($"You don't have a {command.Target}.");
            return;
        }

        _player.Inventory.RemoveItem(item);
        _player.CurrentRoom.AddItem(item);
        Output($"You drop the {item.Name}.");
    }

    private void HandleInventory()
    {
        if (_player == null) return;

        var items = _player.Inventory.GetAllItems();
        
        if (items.Count == 0)
        {
            Output("You are not carrying anything.");
            return;
        }

        Output("You are carrying:");
        foreach (var item in items)
        {
            Output($"  - {item.Name}");
        }
    }

    private void HandleExamine(Command command)
    {
        if (_player?.CurrentRoom == null) return;

        if (command.Target == null)
        {
            Output("What do you want to examine?");
            return;
        }

        // Check inventory first
        var item = _player.Inventory.GetItem(command.Target);
        
        // Then check room
        item ??= _player.CurrentRoom.GetItem(command.Target);

        if (item == null)
        {
            Output($"You don't see a {command.Target} here.");
            return;
        }

        Output(item.Description);
    }

    private void HandleUse(Command command)
    {
        if (_player?.CurrentRoom == null) return;

        if (command.Target == null)
        {
            Output("What do you want to use?");
            return;
        }

        var item = _player.Inventory.GetItem(command.Target);
        
        if (item == null)
        {
            Output($"You don't have a {command.Target}.");
            return;
        }

        if (!item.IsUsable)
        {
            Output($"You can't use the {item.Name} right now.");
            return;
        }

        item.OnUse(_player, _player.CurrentRoom);
        Output(item.UseDescription);
    }

    private void HandleHelp()
    {
        Output("\nAvailable commands:");
        Output("  Movement: go [direction], north, south, east, west, n, s, e, w");
        Output("  Observation: look, examine [item], look at [item]");
        Output("  Inventory: take [item], drop [item], inventory, i");
        Output("  Actions: use [item]");
        Output("  Other: help, quit");
    }

    private void DisplayCurrentLocation()
    {
        if (_player?.CurrentRoom == null) return;

        Output($"\n{_player.CurrentRoom.Name}");
        Output(_player.CurrentRoom.Description);

        var exits = _player.CurrentRoom.GetAvailableExits();
        if (exits.Count > 0)
        {
            Output($"\nExits: {string.Join(", ", exits)}");
        }

        var items = _player.CurrentRoom.GetAllItems();
        if (items.Count > 0)
        {
            Output("\nYou can see:");
            foreach (var item in items)
            {
                Output($"  - {item.Name}");
            }
        }
    }

    protected virtual void OnGameStart()
    {
        Output("=== Text Adventure Game ===\n");
        Output("Type 'help' for a list of commands.\n");
    }

    protected virtual void OnGameEnd()
    {
        Output("\nThanks for playing!");
    }

    /// <summary>Outputs text through the OutputGenerated event</summary>
    private void Output(string text)
    {
        OutputGenerated?.Invoke(text);
    }

    public World World => _world;
    public Player? Player => _player;
    public bool IsRunning => _isRunning;
}
