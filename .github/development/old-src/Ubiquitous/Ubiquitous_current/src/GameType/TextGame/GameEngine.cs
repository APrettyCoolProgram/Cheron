namespace TextGame;

public class GameEngine
{
    public Player Player { get; private set; }
    public bool IsRunning { get; private set; }
    public string GameTitle { get; set; }
    public string GameIntro { get; set; }
    private Dictionary<string, Action<string[]>> Commands { get; set; }

    public GameEngine(Room startingRoom, string title = "Text Adventure", string intro = "Welcome to the adventure!")
    {
        Player = new Player(startingRoom);
        GameTitle = title;
        GameIntro = intro;
        IsRunning = false;
        Commands = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase);
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        Commands["go"] = Go;
        Commands["move"] = Go;
        Commands["walk"] = Go;
        Commands["north"] = args => Go(new[] { "north" });
        Commands["south"] = args => Go(new[] { "south" });
        Commands["east"] = args => Go(new[] { "east" });
        Commands["west"] = args => Go(new[] { "west" });
        Commands["up"] = args => Go(new[] { "up" });
        Commands["down"] = args => Go(new[] { "down" });
        Commands["n"] = args => Go(new[] { "north" });
        Commands["s"] = args => Go(new[] { "south" });
        Commands["e"] = args => Go(new[] { "east" });
        Commands["w"] = args => Go(new[] { "west" });
        Commands["u"] = args => Go(new[] { "up" });
        Commands["d"] = args => Go(new[] { "down" });
        
        Commands["take"] = Take;
        Commands["get"] = Take;
        Commands["pick"] = Take;
        Commands["pickup"] = Take;
        
        Commands["drop"] = Drop;
        Commands["put"] = Drop;
        
        Commands["look"] = Look;
        Commands["l"] = Look;
        Commands["examine"] = Examine;
        Commands["x"] = Examine;
        
        Commands["inventory"] = Inventory;
        Commands["i"] = Inventory;
        
        Commands["use"] = Use;
        Commands["light"] = Light;
        Commands["turn"] = Turn;
        
        Commands["help"] = Help;
        Commands["?"] = Help;
        
        Commands["quit"] = Quit;
        Commands["exit"] = Quit;
        Commands["q"] = Quit;
        
        Commands["score"] = Score;
    }

    public string ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "What?";
        }

        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0];
        var args = parts.Skip(1).ToArray();

        if (Commands.TryGetValue(command, out var action))
        {
            try
            {
                var response = CaptureCommandResponse(() => action(args));
                Player.Moves++;
                return response;
            }
            catch (Exception ex)
            {
                return $"Error executing command: {ex.Message}";
            }
        }

        return $"I don't understand '{command}'. Type 'help' for available commands.";
    }

    private string _commandResponse = "";

    private string CaptureCommandResponse(Action action)
    {
        _commandResponse = "";
        action();
        return _commandResponse;
    }

    private void Output(string message)
    {
        _commandResponse += message;
    }

    private void Go(string[] args)
    {
        if (args.Length == 0)
        {
            Output("Go where?");
            return;
        }

        if (!Enum.TryParse<Direction>(args[0], true, out var direction))
        {
            Output($"I don't know how to go '{args[0]}'.");
            return;
        }

        if (!Player.CurrentRoom.Exits.TryGetValue(direction, out var nextRoom) || nextRoom == null)
        {
            Output("You can't go that way.");
            return;
        }

        Player.CurrentRoom = nextRoom;
        Look(Array.Empty<string>());
    }

    private void Take(string[] args)
    {
        if (args.Length == 0)
        {
            Output("Take what?");
            return;
        }

        var itemName = string.Join(" ", args);
        var item = Player.CurrentRoom.FindItem(itemName);

        if (item == null)
        {
            Output($"There is no '{itemName}' here.");
            return;
        }

        if (!item.IsTakeable)
        {
            Output($"You can't take the {item.Name}.");
            return;
        }

        Player.CurrentRoom.RemoveItem(item);
        Player.AddToInventory(item);
        Output($"Taken: {item.Name}");
    }

    private void Drop(string[] args)
    {
        if (args.Length == 0)
        {
            Output("Drop what?");
            return;
        }

        var itemName = string.Join(" ", args);
        var item = Player.FindItemInInventory(itemName);

        if (item == null)
        {
            Output($"You don't have a '{itemName}'.");
            return;
        }

        Player.RemoveFromInventory(item);
        Player.CurrentRoom.AddItem(item);
        Output($"Dropped: {item.Name}");
    }

    private void Look(string[] args)
    {
        Output(Player.CurrentRoom.GetDescription(Player.HasLightSource()));
    }

    private void Examine(string[] args)
    {
        if (args.Length == 0)
        {
            Look(args);
            return;
        }

        var itemName = string.Join(" ", args);
        
        var item = Player.FindItemInInventory(itemName) ?? Player.CurrentRoom.FindItem(itemName);

        if (item == null)
        {
            Output($"You don't see any '{itemName}' here.");
            return;
        }

        Output(item.Description);
    }

    private void Inventory(string[] args)
    {
        Output(Player.GetInventoryDescription());
    }

    private void Use(string[] args)
    {
        if (args.Length == 0)
        {
            Output("Use what?");
            return;
        }

        var itemName = string.Join(" ", args);
        var item = Player.FindItemInInventory(itemName);

        if (item == null)
        {
            Output($"You don't have a '{itemName}'.");
            return;
        }

        var response = item.GetCustomActionResponse("use");
        Output(response ?? $"You can't use the {item.Name} that way.");
    }

    private void Light(string[] args)
    {
        if (args.Length == 0)
        {
            Output("Light what?");
            return;
        }

        var itemName = string.Join(" ", args);
        var item = Player.FindItemInInventory(itemName);

        if (item == null)
        {
            Output($"You don't have a '{itemName}'.");
            return;
        }

        if (!item.IsLightSource)
        {
            Output($"You can't light the {item.Name}.");
            return;
        }

        if (item.IsLit)
        {
            Output($"The {item.Name} is already lit.");
            return;
        }

        item.IsLit = true;
        Output($"The {item.Name} is now lit.");
    }

    private void Turn(string[] args)
    {
        if (args.Length < 2)
        {
            Output("Turn what on/off?");
            return;
        }

        var action = args[0].ToLower();
        var itemName = string.Join(" ", args.Skip(1));
        var item = Player.FindItemInInventory(itemName);

        if (item == null)
        {
            Output($"You don't have a '{itemName}'.");
            return;
        }

        if (!item.IsLightSource)
        {
            Output($"You can't turn the {item.Name} on or off.");
            return;
        }

        if (action == "on")
        {
            if (item.IsLit)
            {
                Output($"The {item.Name} is already on.");
                return;
            }
            item.IsLit = true;
            Output($"The {item.Name} is now on.");
        }
        else if (action == "off")
        {
            if (!item.IsLit)
            {
                Output($"The {item.Name} is already off.");
                return;
            }
            item.IsLit = false;
            Output($"The {item.Name} is now off.");
        }
        else
        {
            Output("Turn on or off?");
        }
    }

    private void Help(string[] args)
    {
        Output(@"Available Commands:
  Movement: go/move/walk [direction], or just use: north/south/east/west/up/down (n/s/e/w/u/d)
  Items: take/get [item], drop [item]
  Interaction: examine/x [item], use [item], light [item], turn on/off [item]
  Information: look/l, inventory/i, score
  System: help/?, quit/exit/q");
    }

    private void Quit(string[] args)
    {
        IsRunning = false;
        Output("Thanks for playing!");
    }

    private void Score(string[] args)
    {
        Output($"Score: {Player.Score} | Moves: {Player.Moves}");
    }

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }
}
