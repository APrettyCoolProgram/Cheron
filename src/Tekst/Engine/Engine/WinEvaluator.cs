// 260619_code
// 260620_documentation

using Tekst.Cartridge;
using Tekst.State;

namespace Tekst.Engine;

/// <summary>Evaluates the win condition described by a <see cref="WinRuleData"/> entryagainst the current <see cref="GameState"/>.</summary>
public static class WinEvaluator
{
    /// <summary>Returns a formatted victory message if the win condition is met,otherwise returns <see langword="null"/>.</summary>
    public static string? Check(WinRuleData rule, GameState state)
    {
        var hasItem = state.Inventory.Any(i => i.Id.Equals(rule.RequiredItemId, StringComparison.OrdinalIgnoreCase));
        var inRoom  = state.CurrentRoom.Id.Equals(rule.RequiredRoomId, StringComparison.OrdinalIgnoreCase);

        if (!hasItem || !inRoom)
        {
            return null;
        }

        state.IsGameWon  = true;
        state.IsGameOver = true;

        return rule.VictoryText.Replace("{0}", state.TurnCount.ToString()).Replace("{1}", state.TurnCount == 1
            ? ""
            : "s");
    }
}