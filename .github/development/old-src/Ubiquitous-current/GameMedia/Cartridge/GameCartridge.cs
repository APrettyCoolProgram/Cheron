namespace Cartridge;

public class GameCartridge
{
    public string Title { get; set; } = string.Empty;
    public string Intro { get; set; } = string.Empty;
    public List<RoomData> Rooms { get; set; } = new();
    public List<ItemData> Items { get; set; } = new();
    public string StartingRoomId { get; set; } = string.Empty;
}

public class RoomData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsLit { get; set; } = true;
    public List<ExitData> Exits { get; set; } = new();
    public List<string> Items { get; set; } = new();
}

public class ExitData
{
    public string Direction { get; set; } = string.Empty;
    public string TargetRoomId { get; set; } = string.Empty;
}

public class ItemData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Aliases { get; set; } = new();
    public bool IsTakeable { get; set; }
    public bool IsLightSource { get; set; }
    public List<CustomActionData> CustomActions { get; set; } = new();
}

public class CustomActionData
{
    public string Verb { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
}
