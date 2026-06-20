namespace TextGame;

public class Room
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<Direction, Room?> Exits { get; set; }
    public List<Item> Items { get; set; }
    public bool IsLit { get; set; }
    public string DarkDescription { get; set; }

    public Room(string name, string description, bool isLit = true)
    {
        Name = name;
        Description = description;
        IsLit = isLit;
        DarkDescription = "It is pitch black. You are likely to be eaten by a grue.";
        Exits = new Dictionary<Direction, Room?>();
        Items = new List<Item>();
    }

    public void AddExit(Direction direction, Room room)
    {
        Exits[direction] = room;
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

    public Item? FindItem(string itemName)
    {
        return Items.FirstOrDefault(i => 
            i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase) ||
            i.Aliases.Any(a => a.Equals(itemName, StringComparison.OrdinalIgnoreCase)));
    }

    public string GetDescription(bool hasLight)
    {
        if (!IsLit && !hasLight)
        {
            return DarkDescription;
        }

        var description = $"{Name}\n{Description}";

        if (Items.Any())
        {
            description += "\n\nYou can see:";
            foreach (var item in Items)
            {
                description += $"\n  - {item.Name}";
            }
        }

        if (Exits.Any())
        {
            description += "\n\nObvious exits: " + string.Join(", ", Exits.Keys.Select(d => d.ToString().ToLower()));
        }

        return description;
    }
}
