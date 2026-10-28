namespace TextGame;

public class GameBuilder
{
    private Room? _startingRoom;
    private string _title = "Text Adventure";
    private string _intro = "Welcome to the adventure!";

    public GameBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public GameBuilder WithIntro(string intro)
    {
        _intro = intro;
        return this;
    }

    public GameBuilder WithStartingRoom(Room room)
    {
        _startingRoom = room;
        return this;
    }

    public GameEngine Build()
    {
        if (_startingRoom == null)
        {
            throw new InvalidOperationException("Starting room must be set before building the game.");
        }

        return new GameEngine(_startingRoom, _title, _intro);
    }

    public static GameEngine CreateSampleGame()
    {
        var assembly = typeof(Cartridge.GameCartridge).Assembly;
        var resourceName = "Cartridge.Games.TheAbandonedMansion.json";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        var jsonContent = reader.ReadToEnd();
        var cartridge = Cartridge.CartridgeLoader.LoadCartridgeFromJsonContent(jsonContent);
        
        if (cartridge == null)
        {
            throw new InvalidOperationException("Failed to load game cartridge.");
        }

        return CartridgeBuilder.BuildGameFromCartridge(cartridge);
    }
}
