using System.Text.Json;

namespace Cartridge;

public static class CartridgeLoader
{
    public static object LoadFromJson(string jsonFilePath)
    {
        var jsonContent = File.ReadAllText(jsonFilePath);
        return LoadFromJsonContent(jsonContent);
    }

    public static object LoadFromJsonContent(string jsonContent)
    {
        var cartridge = JsonSerializer.Deserialize<GameCartridge>(jsonContent);
        
        if (cartridge == null)
        {
            throw new InvalidOperationException("Failed to deserialize game cartridge.");
        }

        return cartridge;
    }

    public static GameCartridge? LoadCartridgeFromJson(string jsonFilePath)
    {
        var jsonContent = File.ReadAllText(jsonFilePath);
        return JsonSerializer.Deserialize<GameCartridge>(jsonContent);
    }

    public static GameCartridge? LoadCartridgeFromJsonContent(string jsonContent)
    {
        return JsonSerializer.Deserialize<GameCartridge>(jsonContent);
    }
}
