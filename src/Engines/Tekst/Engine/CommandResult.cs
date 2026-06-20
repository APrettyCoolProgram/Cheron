// 260619_code
// 260619_documentation

namespace Tekst.Engine;

/// <summary>The outcome of processing a single player command.</summary>
public class CommandResult
{
    /// <summary>Narrative text to display to the player.</summary>
    public required string Message { get; init; }

    /// <summary>Whether the room description should be printed after this command.</summary>
    public bool ShowRoomDescription { get; init; }

    /// <summary> Convenience method for creating a CommandResult with a message and optional room description flag.</summary>
    /// <param name="message">The message to display to the player.</param>
    /// <param name="showRoom">Whether to show the room description after this command.</param>
    /// <returns>A new CommandResult instance.</returns>
    public static CommandResult Say(string message, bool showRoom = false) =>
        new() { Message = message, ShowRoomDescription = showRoom };
}
