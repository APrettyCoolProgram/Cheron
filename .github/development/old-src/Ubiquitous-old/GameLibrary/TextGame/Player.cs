namespace TextGame;

/// <summary>Represents the player's inventory and state</summary>
public class Player
{
    /// <summary>Gets or sets the player's current room</summary>
    public Room? CurrentRoom { get; set; }

    /// <summary>Gets the player's inventory</summary>
    public Inventory Inventory { get; }

    /// <summary>Gets or sets the player's score</summary>
    public int Score { get; set; }

    /// <summary>Gets or sets the number of moves the player has made</summary>
    public int MoveCount { get; set; }

    /// <summary>Initializes a new instance of the Player class</summary>
    public Player()
    {
        Inventory = new Inventory();
        Score = 0;
        MoveCount = 0;
    }
}
