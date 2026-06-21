// 260620_code
// 260620_documentation

namespace Tekst.Catalog;

public static class msg_Credit
{
    public static string GameEnded(int turnCount) =>
        $"{Environment.NewLine}"+
        $"--- Game ended after {turnCount} turn{(turnCount == 1 ? "" : "s")} ---" +
        $"{Environment.NewLine}";
}