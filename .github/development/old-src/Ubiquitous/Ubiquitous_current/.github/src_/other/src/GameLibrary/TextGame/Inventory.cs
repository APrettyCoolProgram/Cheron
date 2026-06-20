namespace TextGame;

/// <summary>
/// Manages a collection of items
/// </summary>
public class Inventory
{
    private readonly List<Item> _items;

    public Inventory()
    {
        _items = new List<Item>();
    }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    public void AddItem(Item item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Add(item);
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    public bool RemoveItem(Item item)
    {
        return _items.Remove(item);
    }

    /// <summary>
    /// Gets an item by name (case-insensitive)
    /// </summary>
    public Item? GetItem(string name)
    {
        return _items.FirstOrDefault(i => 
            i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all items in the inventory
    /// </summary>
    public List<Item> GetAllItems()
    {
        return new List<Item>(_items);
    }

    /// <summary>
    /// Checks if the inventory contains an item
    /// </summary>
    public bool HasItem(string name)
    {
        return _items.Any(i => 
            i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the number of items in the inventory
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Clears all items from the inventory
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}
