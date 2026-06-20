namespace TextGame;

/// <summary>
/// Represents the player's inventory and state.
/// </summary>
public class Player
{
    /// <summary>
    /// Gets or sets the player's current location (room ID).
    /// </summary>
    public string CurrentRoomId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of item IDs in the player's inventory.
    /// </summary>
    public List<string> Inventory { get; set; } = new();

    /// <summary>
    /// Gets or sets the player's score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the number of moves the player has made.
    /// </summary>
    public int MoveCount { get; set; }

    /// <summary>
    /// Adds an item to the player's inventory.
    /// </summary>
    /// <param name="itemId">The ID of the item to add.</param>
    public void AddItem(string itemId)
    {
        if (!Inventory.Contains(itemId))
        {
            Inventory.Add(itemId);
        }
    }

    /// <summary>
    /// Removes an item from the player's inventory.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove.</param>
    /// <returns>True if the item was removed, false if it wasn't in inventory.</returns>
    public bool RemoveItem(string itemId)
    {
        return Inventory.Remove(itemId);
    }

    /// <summary>
    /// Checks if the player has a specific item in their inventory.
    /// </summary>
    /// <param name="itemId">The ID of the item to check for.</param>
    /// <returns>True if the item is in inventory, false otherwise.</returns>
    public bool HasItem(string itemId)
    {
        return Inventory.Contains(itemId);
    }
}
