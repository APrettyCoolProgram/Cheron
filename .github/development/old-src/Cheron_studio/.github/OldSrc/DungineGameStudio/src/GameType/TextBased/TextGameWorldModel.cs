// 260130_code
// 260130_documentation

namespace DungineStudio.GameType.TextBased;

public class TextGameWorldModel
{
    public string StartLocationId { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public List<Location> Locations { get; set; } = new();
}

public class Location
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Dictionary<string, string> Exits { get; set; } = new();

    public List<Item> Items { get; set; } = new();

    public override string ToString() => $"{Id} - {Name}";
}

public class Item
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool? IsPortable { get; set; }

    public bool? IsContainer { get; set; }

    public List<Item>? Contents { get; set; }

    public List<string>? Aliases { get; set; }

    public override string ToString() => $"{Id} - {Name}";
}
