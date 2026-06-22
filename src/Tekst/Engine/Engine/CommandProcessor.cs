// 260621_code
// 260619_documentation

using Tekst.Models;
using Tekst.State;
using Tekst.World;

namespace Tekst.Engine;

/// <summary>Interprets raw player input and mutates <see cref="GameState"/> accordingly.</summary>
/// <remarks>Supports verbs: look, go, take, drop, inventory, examine, quit, help.</remarks>
public class CommandProcessor
{
    private readonly GameWorld _world;

    /// <summary>Initializes a new instance of the <see cref="CommandProcessor"/> class.</summary>
    /// <param name="world">The world data used to resolve room transitions.</param>
    public CommandProcessor(GameWorld world) => _world = world;

    /// <summary>Processes a single line of raw player input.</summary>
    /// <param name="raw">The unparsed command text entered by the player.</param>
    /// <param name="state">The current game state to inspect and mutate.</param>
    /// <returns>The result to present to the player.</returns>
    public CommandResult Process(string raw, GameState state)
    {
        state.TurnCount++;

        var tokens = raw.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length == 0)
        {
            return CommandResult.Say("(Nothing happens.)");
        }

        var verb = tokens[0];
        var noun = tokens.Length > 1
            ? string.Join(' ', tokens[1..])
            : string.Empty;

        return verb switch
        {
            "about" => About(state),
            "look" or "l" => Look(state),
            "go" or "move" => Go(noun, state),
            "north" or "n" => Go("north", state),
            "south" or "s" => Go("south", state),
            "east" or "e" => Go("east", state),
            "west" or "w" => Go("west", state),
            "up" or "u" => Go("up", state),
            "down" or "d" => Go("down", state),
            "take" or "get" => Take(noun, state),
            "drop" => Drop(noun, state),
            "inventory" or "i" or "inv" => ShowInventory(state),
            "examine" or "x" or "ex" or "inspect" => Examine(noun, state),
            "quit" or "exit" or "q" => Quit(state),
            "help" or "h" or "?" => Help(),
            _ => CommandResult.Say($"You don't know how to \"{raw.Trim()}\"."),
        };
    }

    // -------------------------------------------------------------------------
    // Verb handlers
    // -------------------------------------------------------------------------

    private static CommandResult About(GameState state)
    {
        var thing = $"{state.}"
        
        return CommandResult.Say(
            """
            Game details
            ------------
              Name:                  }
              go/move <direction>      - Move in a direction
              get/take <item>          - Pick up an item
              drop <item>              - Drop a carried item
              inventory/inv/i          - List what you're carrying
              examine/x/inspect <item> - Get a detailed look at something
              quit/q                   - Quit the game
              help/h/?                 - Show this list

            Cardinal directions  Vertical directions
            -------------------  -------------------
              north/n              up/u
              south/s              down/d
              east/e
              west/w
            """);
    }
    
    
    /// <summary>Describes the player's current room.</summary>
    /// <param name="state">The current game state.</param>
    /// <returns>A result containing the room description.</returns>
    private static CommandResult Look(GameState state) => CommandResult.Say(DescribeRoom(state.CurrentRoom, state), showRoom: false);

    /// <summary>Moves the player in the specified direction when an exit exists.</summary>
    /// <param name="direction">The direction to travel.</param>
    /// <param name="state">The current game state.</param>
    /// <returns>A result describing the outcome of the movement.</returns>
    private CommandResult Go(string direction, GameState state)
    {
        if (string.IsNullOrEmpty(direction))
        {
            return CommandResult.Say("Go where?");
        }

        var exit = state.CurrentRoom.Exits.FirstOrDefault(e => e.Direction.Equals(direction, StringComparison.OrdinalIgnoreCase));

        if (exit is null)
        {
            return CommandResult.Say("You can't go that way.");
        }

        var destination = _world.GetRoom(exit.TargetRoomId);

        if (destination is null)
        {
            return CommandResult.Say("The way ahead is blocked by an unseen force.");
        }

        state.CurrentRoom = destination;

        var travel = exit.MoveDescription is not null
            ? $"{exit.MoveDescription}\n\n"
            : string.Empty;

        return CommandResult.Say($"{travel}{DescribeRoom(destination, state)}");
    }

    /// <summary>Takes an item from the current room and adds it to the inventory.</summary>
    /// <param name="noun">The item name or identifier to take.</param>
    /// <param name="state">The current game state.</param>
    /// <returns>A result describing the outcome of the action.</returns>
    private static CommandResult Take(string noun, GameState state)
    {
        if (string.IsNullOrEmpty(noun))
        {
            return CommandResult.Say("Take what?");
        }
        var item = FindItem(noun, state.CurrentRoom.Items);

        if (item is null)
        {
            return CommandResult.Say($"You don't see any \"{noun}\" here.");
        }

        if (!item.CanTake)
        {
            return CommandResult.Say($"You can't take the {item.Name}.");
        }

        state.CurrentRoom.Items.Remove(item);
        state.Inventory.Add(item);

        return CommandResult.Say($"You pick up {item.Name}.");
    }

    /// <summary>Drops an item from the inventory into the current room.</summary>
    /// <param name="noun">The item name or identifier to drop.</param>
    /// <param name="state">The current game state.</param>
    /// <returns>A result describing the outcome of the action.</returns>
    private static CommandResult Drop(string noun, GameState state)
    {
        if (string.IsNullOrEmpty(noun))
        {
            return CommandResult.Say("Drop what?");
        }

        var item = FindItem(noun, state.Inventory);

        if (item is null)
        {
            return CommandResult.Say($"You're not carrying any \"{noun}\".");
        }

        state.Inventory.Remove(item);
        state.CurrentRoom.Items.Add(item);

        return CommandResult.Say($"You drop {item.Name}.");
    }

    /// <summary>Lists the items currently carried by the player.</summary>
    /// <param name="state">The current game state.</param>
    /// <returns>A result containing the inventory listing.</returns>
    private static CommandResult ShowInventory(GameState state)
    {
        if (state.Inventory.Count == 0)
        {
            return CommandResult.Say("You are carrying nothing.");
        }

        var list = string.Join("\n  ", state.Inventory.Select(i => $"- {i.Name}"));

        return CommandResult.Say($"You are carrying:\n  {list}");
    }

    /// <summary>Shows the description of an item visible to the player.</summary>
    /// <param name="noun">The item name or identifier to examine.</param>
    /// <param name="state">The current game state.</param>
    /// <returns>A result containing the item's description.</returns>
    private static CommandResult Examine(string noun, GameState state)
    {
        if (string.IsNullOrEmpty(noun))
        {
            return CommandResult.Say("Examine what?");
        }

        var allVisible = state.CurrentRoom.Items.Concat(state.Inventory);
        var item       = FindItem(noun, allVisible);

        if (item is null)
        {
            return CommandResult.Say($"You don't see any \"{noun}\" here.");
        }

        return CommandResult.Say(item.Description);
    }

    /// <summary>Ends the current game session.</summary>
    /// <param name="state">The current game state.</param>
    /// <returns>A result that confirms the game has ended.</returns>
    private static CommandResult Quit(GameState state)
    {
        state.IsGameOver = true;

        return CommandResult.Say("You abandon your quest and step back into the shadows. Farewell.");
    }

    /// <summary>Returns a list of supported commands.</summary>
    /// <returns>A result containing the help text.</returns>
    private static CommandResult Help()
    {
        return CommandResult.Say(
            """
            Commands
            --------
              look/l                   - Describe your surroundings
              go/move <direction>      - Move in a direction
              get/take <item>          - Pick up an item
              drop <item>              - Drop a carried item
              inventory/inv/i          - List what you're carrying
              examine/x/inspect <item> - Get a detailed look at something
              quit/q                   - Quit the game
              help/h/?                 - Show this list

            Cardinal directions  Vertical directions
            -------------------  -------------------
              north/n              up/u
              south/s              down/d
              east/e
              west/w
            """);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>Builds the full textual description of a room at the current moment.</summary>
    /// <param name="room">The room to describe.</param>
    /// <param name="state">The current game state.</param>
    /// <returns>The formatted room description.</returns>
    public static string DescribeRoom(Room room, GameState state)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"[ {room.Title} ]");
        sb.AppendLine(room.Description);

        if (room.Items.Count > 0)
        {
            sb.AppendLine();
            foreach (var item in room.Items)
            {
                sb.AppendLine($"  There is {item.Name} here.");
            }
        }

        if (room.Exits.Count > 0)
        {
            var dirs = string.Join(", ", room.Exits.Select(e => e.Direction));

            sb.AppendLine();
            sb.Append($"Obvious exits: {dirs}.");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>Finds the first item whose identifier or name matches the supplied text.</summary>
    /// <param name="noun">The text to match against available items.</param>
    /// <param name="items">The items to search.</param>
    /// <returns>The matching item, or <see langword="null"/> if no match is found.</returns>
    private static Item? FindItem(string noun, IEnumerable<Item> items) =>
        items.FirstOrDefault(i =>
            i.Id.Equals(noun, StringComparison.OrdinalIgnoreCase) ||
            i.Name.Contains(noun, StringComparison.OrdinalIgnoreCase));
}