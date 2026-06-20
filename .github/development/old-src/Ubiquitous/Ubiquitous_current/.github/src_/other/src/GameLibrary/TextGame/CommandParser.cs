namespace TextGame;

/// <summary>
/// Command actions that can be performed in the game
/// </summary>
public enum CommandAction
{
    Unknown,
    Move,
    Look,
    Take,
    Drop,
    Inventory,
    Examine,
    Use,
    Help,
    Quit
}

/// <summary>
/// Cardinal directions for navigation
/// </summary>
public enum Direction
{
    North,
    South,
    East,
    West,
    Up,
    Down
}

/// <summary>
/// Represents a parsed command from player input
/// </summary>
public class Command
{
    public CommandAction Action { get; set; }
    public Direction? Direction { get; set; }
    public string? Target { get; set; }
    public string? SecondaryTarget { get; set; }

    public Command(CommandAction action)
    {
        Action = action;
    }
}

/// <summary>
/// Parses natural language commands into game actions
/// </summary>
public class CommandParser
{
    private readonly Dictionary<string, CommandAction> _actionWords;
    private readonly Dictionary<string, Direction> _directionWords;

    public CommandParser()
    {
        _actionWords = new Dictionary<string, CommandAction>(StringComparer.OrdinalIgnoreCase)
        {
            { "go", CommandAction.Move },
            { "move", CommandAction.Move },
            { "walk", CommandAction.Move },
            { "run", CommandAction.Move },
            { "travel", CommandAction.Move },
            { "head", CommandAction.Move },
            { "north", CommandAction.Move },
            { "south", CommandAction.Move },
            { "east", CommandAction.Move },
            { "west", CommandAction.Move },
            { "up", CommandAction.Move },
            { "down", CommandAction.Move },
            { "n", CommandAction.Move },
            { "s", CommandAction.Move },
            { "e", CommandAction.Move },
            { "w", CommandAction.Move },
            { "u", CommandAction.Move },
            { "d", CommandAction.Move },
            
            { "look", CommandAction.Look },
            { "l", CommandAction.Look },
            { "observe", CommandAction.Look },
            
            { "take", CommandAction.Take },
            { "get", CommandAction.Take },
            { "grab", CommandAction.Take },
            { "pick", CommandAction.Take },
            { "pickup", CommandAction.Take },
            
            { "drop", CommandAction.Drop },
            { "discard", CommandAction.Drop },
            { "leave", CommandAction.Drop },
            
            { "inventory", CommandAction.Inventory },
            { "i", CommandAction.Inventory },
            { "inv", CommandAction.Inventory },
            { "items", CommandAction.Inventory },
            
            { "examine", CommandAction.Examine },
            { "x", CommandAction.Examine },
            { "inspect", CommandAction.Examine },
            { "check", CommandAction.Examine },
            { "read", CommandAction.Examine },
            
            { "use", CommandAction.Use },
            { "apply", CommandAction.Use },
            { "activate", CommandAction.Use },
            
            { "help", CommandAction.Help },
            { "?", CommandAction.Help },
            { "commands", CommandAction.Help },
            
            { "quit", CommandAction.Quit },
            { "exit", CommandAction.Quit },
            { "q", CommandAction.Quit }
        };

        _directionWords = new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
        {
            { "north", Direction.North },
            { "n", Direction.North },
            { "south", Direction.South },
            { "s", Direction.South },
            { "east", Direction.East },
            { "e", Direction.East },
            { "west", Direction.West },
            { "w", Direction.West },
            { "up", Direction.Up },
            { "u", Direction.Up },
            { "down", Direction.Down },
            { "d", Direction.Down }
        };
    }

    /// <summary>
    /// Parses a player input string into a Command object
    /// </summary>
    public Command? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var words = input.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLower())
            .ToList();

        if (words.Count == 0)
            return null;

        // Remove common filler words
        var fillerWords = new HashSet<string> { "the", "a", "an", "at", "to", "on", "in" };
        words = words.Where(w => !fillerWords.Contains(w)).ToList();

        if (words.Count == 0)
            return null;

        var firstWord = words[0];

        // Try to match action
        if (!_actionWords.TryGetValue(firstWord, out var action))
            return null;

        var command = new Command(action);

        // Special handling for movement commands
        if (action == CommandAction.Move)
        {
            // Check if first word is a direction
            if (_directionWords.TryGetValue(firstWord, out var direction))
            {
                command.Direction = direction;
            }
            // Check if second word is a direction
            else if (words.Count > 1 && _directionWords.TryGetValue(words[1], out direction))
            {
                command.Direction = direction;
            }
        }

        // Handle "look at [item]" pattern
        if (action == CommandAction.Look && words.Count > 1)
        {
            command.Action = CommandAction.Examine;
            command.Target = string.Join(" ", words.Skip(1));
        }

        // Extract target for other commands
        if (action == CommandAction.Take || 
            action == CommandAction.Drop || 
            action == CommandAction.Examine || 
            action == CommandAction.Use)
        {
            if (words.Count > 1)
            {
                command.Target = string.Join(" ", words.Skip(1));
            }
        }

        return command;
    }

    /// <summary>
    /// Adds a custom action word to the parser
    /// </summary>
    public void AddActionWord(string word, CommandAction action)
    {
        _actionWords[word] = action;
    }

    /// <summary>
    /// Adds a custom direction word to the parser
    /// </summary>
    public void AddDirectionWord(string word, Direction direction)
    {
        _directionWords[word] = direction;
    }
}
