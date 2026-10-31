namespace TextGame;

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Aliases { get; set; }
    public bool IsTakeable { get; set; }
    public bool IsLightSource { get; set; }
    public bool IsLit { get; set; }
    public Dictionary<string, string> CustomActions { get; set; }

    public Item(string name, string description, bool isTakeable = true)
    {
        Name = name;
        Description = description;
        IsTakeable = isTakeable;
        IsLightSource = false;
        IsLit = false;
        Aliases = new List<string>();
        CustomActions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public void AddAlias(params string[] aliases)
    {
        Aliases.AddRange(aliases);
    }

    public void AddCustomAction(string action, string response)
    {
        CustomActions[action] = response;
    }

    public string? GetCustomActionResponse(string action)
    {
        return CustomActions.TryGetValue(action, out var response) ? response : null;
    }
}
