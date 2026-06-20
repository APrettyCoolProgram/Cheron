// 260619_code
// 260619_documentation

namespace Tekst.Cartridge;

/// <summary>Root deserialization target for a <c>.cart</c> JSON file.</summary>
public class CartridgeData
{
    /// <summary>Gets or sets the title data displayed at startup.</summary>
    public TitleData Title { get; set; } = new();

    /// <summary>Gets or sets the game title shown in the banner.</summary>
    public string GameTitle { get; set; } = string.Empty;

    /// <summary>Gets or sets the author of the game.</summary>
    public string GameAuthor { get; set; } = "---";

    /// <summary>Gets or sets the game version.</summary>
    public string GameVersion { get; set; } = string.Empty;

    /// <summary>Gets or sets a short description of the game.</summary>
    public string GameDescription { get; set; } = string.Empty;

    /// <summary>Gets or sets the engine used to run the game.</summary>
    public string GameEngine { get; set; } = string.Empty;

    /// <summary>Gets or sets the type of game.</summary>
    public string GameType { get; set; } = string.Empty;

    /// <summary>Gets or sets the single-character variation of the game type.</summary>
    public string GameTypeVariation { get; set; } = "A";

    /// <summary>Gets or sets the foreground,background color scheme.</summary>
    public string GameColorScheme { get; set; } = "B,W";

    /// <summary>Gets or sets the genre of the game.</summary>
    public string GameGenre { get; set; } = string.Empty;

    /// <summary>Gets or sets the difficulty level of the game.</summary>
    public string GameDifficulty { get; set; } = string.Empty;

    /// <summary>Gets or sets the text game type.</summary>
    public string TextGameType { get; set; } = string.Empty;

    /// <summary>Gets or sets the rule that determines when the game is won.</summary>
    public WinRuleData WinRule { get; set; } = new();

    /// <summary>Gets or sets the identifier of the room where the player begins.</summary>
    public string StartingRoomId { get; set; } = string.Empty;

    /// <summary>Gets or sets the rooms defined in the cartridge.</summary>
    public List<RoomData> Rooms { get; set; } = [];
}

/// <summary>Defines the title screen content for the game.</summary>
public class TitleData
{
    /// <summary>Gets or sets the banner lines shown on the title screen.</summary>
    public List<string> Banner { get; set; } = [];

    /// <summary>Gets or sets the introductory text displayed when the game starts.</summary>
    public string Intro { get; set; } = string.Empty;

    /// <summary>Gets or sets the help prompt shown to the player.</summary>
    public string HelpPrompt { get; set; } = string.Empty;
}

/// <summary>Defines the victory requirements for the game.</summary>
public class WinRuleData
{
    /// <summary>Gets or sets the identifier of the item required to win.</summary>
    public string RequiredItemId { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the room where the win condition must be met.</summary>
    public string RequiredRoomId { get; set; } = string.Empty;

    /// <summary>
    /// Victory message shown when the win condition is met.
    /// Supports <c>{0}</c> = turn count, <c>{1}</c> = "s" or "" for plural.
    /// </summary>
    public string VictoryText { get; set; } = string.Empty;
}

/// <summary>Defines a room in the game world.</summary>
public class RoomData
{
    /// <summary>Gets or sets the room identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the room title shown to the player.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the room description shown to the player.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the items available in the room.</summary>
    public List<ItemData> Items { get; set; } = [];

    /// <summary>Gets or sets the exits available from the room.</summary>
    public List<ExitData> Exits { get; set; } = [];
}

/// <summary>Defines an item that can appear in a room or inventory.</summary>
public class ItemData
{
    /// <summary>Gets or sets the item identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the item display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the item description shown to the player.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the item can be taken by the player.</summary>
    public bool CanTake { get; set; } = true;
}

/// <summary>Defines a directional exit from one room to another.</summary>
public class ExitData
{
    /// <summary>Gets or sets the direction label used to reference the exit.</summary>
    public string Direction { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the target room.</summary>
    public string TargetRoomId { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional text shown when the player moves through the exit.</summary>
    public string? MoveDescription { get; set; }
}