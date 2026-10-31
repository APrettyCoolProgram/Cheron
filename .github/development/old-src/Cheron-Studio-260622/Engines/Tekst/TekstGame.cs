// 260622_code
// 260622_documentation

namespace CheronStudio.Engines.Tekst;

public class TekstGame
{
    public TekstDetail Detail { get; set; } = new();
    public TekstTitle Title { get; set; } = new();
    public TekstWinRule WinRule { get; set; } = new();
    public string StartingRoomId { get; set; } = string.Empty;
    public List<TekstRoom> Rooms { get; set; } = [];
}

public class TekstDetail
{
    public string GameTitle { get; set; } = "Untitled Game";
    public string GameAuthor { get; set; } = string.Empty;
    public string GameVersion { get; set; } = "1.0";
    public string GameDescription { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;
    public string GameTypeVariation { get; set; } = string.Empty;
    public string GameGenre { get; set; } = string.Empty;
    public string GameDifficulty { get; set; } = string.Empty;
}

public class TekstTitle
{
    public List<string> BannerContents { get; set; } = [];
    public string Story { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
}

public class TekstWinRule
{
    public string RequiredItemId { get; set; } = string.Empty;
    public string RequiredRoomId { get; set; } = string.Empty;
    public string VictoryText { get; set; } = string.Empty;
}

public class TekstRoom
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<TekstItem> Items { get; set; } = [];
    public List<TekstExit> Exits { get; set; } = [];
}

public class TekstItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool CanTake { get; set; } = true;
}

public class TekstExit
{
    public string Direction { get; set; } = string.Empty;
    public string TargetRoomId { get; set; } = string.Empty;
    public string MoveDescription { get; set; } = string.Empty;
}
