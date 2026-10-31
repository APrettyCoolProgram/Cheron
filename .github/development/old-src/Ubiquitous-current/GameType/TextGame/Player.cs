namespace TextGame;

public class Player
{
    public Room CurrentRoom { get; set; }
    public List<Item> Inventory { get; set; }
    public int Score { get; set; }
    public int Moves { get; set; }

    public Player(Room startingRoom)
    {
        CurrentRoom = startingRoom;
        Inventory = new List<Item>();
        Score = 0;
        Moves = 0;
    }

    public void AddToInventory(Item item)
    {
        Inventory.Add(item);
    }

    public void RemoveFromInventory(Item item)
    {
        Inventory.Remove(item);
    }

    public Item? FindItemInInventory(string itemName)
    {
        return Inventory.FirstOrDefault(i => 
            i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase) ||
            i.Aliases.Any(a => a.Equals(itemName, StringComparison.OrdinalIgnoreCase)));
    }

    public bool HasLightSource()
    {
        return Inventory.Any(i => i.IsLightSource && i.IsLit);
    }

    public string GetInventoryDescription()
    {
        if (!Inventory.Any())
        {
            return "You are empty-handed.";
        }

        return "You are carrying:\n  " + string.Join("\n  ", Inventory.Select(i => i.Name));
    }
}
