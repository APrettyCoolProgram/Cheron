# Quick Start: Integrating Cartridge System

## For UI Developers

This guide shows how to integrate cartridge loading into your UI.

## Basic Integration (Recommended)

### Load Default Cartridge

```csharp
private void InitializeGame()
{
    // Load the default game cartridge
    Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
    
    if (startingRoom == null)
    {
        MessageBox.Show("Failed to load game. Using sample game instead.");
        startingRoom = GameBuilder.BuildSampleGame();
    }
    
    // Initialize game as before
    var player = new Player("Hero");
    _game = new Game();
    _game.Initialize(startingRoom, player);
    _game.OutputGenerated += OnGameOutput;
    _game.Start();
}
```

## Advanced Integration (With Selector)

### Let User Choose Cartridge

```csharp
private void InitializeGame()
{
    // Find all available cartridges
    var cartridges = GameBuilder.FindCartridges("Cartridges");
    
    if (cartridges.Count == 0)
    {
        MessageBox.Show("No cartridges found. Using sample game.");
        var sampleRoom = GameBuilder.BuildSampleGame();
        StartGame(sampleRoom);
        return;
    }
    
    // Show selection dialog (implement based on your UI)
    string selectedCartridge = ShowCartridgeSelector(cartridges);
    
    // Load selected cartridge
    Room? startingRoom = GameBuilder.LoadFromCartridge(selectedCartridge);
    
    if (startingRoom != null)
    {
        StartGame(startingRoom);
    }
    else
    {
        MessageBox.Show("Failed to load cartridge.");
    }
}

private void StartGame(Room startingRoom)
{
    var player = new Player("Hero");
    _game = new Game();
    _game.Initialize(startingRoom, player);
    _game.OutputGenerated += OnGameOutput;
    _game.Start();
}
```

## Cartridge Selector Example

### Simple ComboBox Selector

```csharp
private string ShowCartridgeSelector(List<string> cartridges)
{
    var dialog = new Window
    {
        Title = "Select Adventure",
        Width = 300,
        Height = 200
    };
    
    var comboBox = new ComboBox
    {
        ItemsSource = cartridges.Select(Path.GetFileNameWithoutExtension)
    };
    
    // ... add buttons and handle selection
    
    return cartridges[comboBox.SelectedIndex];
}
```

## Error Handling

### Robust Loading with Fallbacks

```csharp
private Room LoadGameWithFallback()
{
    // Try to load default cartridge
    var room = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
    if (room != null) return room;
    
    // Try any other cartridge
    var cartridges = GameBuilder.FindCartridges("Cartridges");
    foreach (var cartridge in cartridges)
    {
        room = GameBuilder.LoadFromCartridge(cartridge);
        if (room != null) return room;
    }
    
    // Fall back to hardcoded sample game
    return GameBuilder.BuildSampleGame();
}
```

## Migration Path

### Update Existing UI Code

**Before:**
```csharp
private void InitializeGame()
{
    var entrance = new Room("Entrance Hall", "...");
    var library = new Room("Library", "...");
    // ... lots of room/item creation code
    
    var player = new Player("Hero");
    _game = new Game();
    _game.Initialize(entrance, player);
    _game.Start();
}
```

**After:**
```csharp
private void InitializeGame()
{
    var startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
    
    var player = new Player("Hero");
    _game = new Game();
    _game.Initialize(startingRoom, player);
    _game.Start();
}
```

## Important Notes

1. **Cartridge Location**: Cartridges are in `Cartridges/` folder relative to executable
2. **Error Handling**: Always check if LoadFromCartridge returns null
3. **Console Output**: Errors are written to Console - consider redirecting
4. **Backward Compatibility**: BuildSampleGame() still works as before
5. **Thread Safety**: CartridgeLoader is stateless and thread-safe

## Complete Example for BasicUI

```csharp
// GameInterface/TextGameUI/BasicUI/TextGameWindow.xaml.cs

private void InitializeGame()
{
    // Load from cartridge
    Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
    
    if (startingRoom == null)
    {
        // Fallback to hardcoded game
        startingRoom = GameBuilder.BuildSampleGame();
    }
    
    // Initialize game (unchanged)
    var player = new Player("Hero");
    _game = new Game();
    _game.Initialize(startingRoom, player);
    _game.OutputGenerated += OnGameOutput;
    _game.Start();
}
```

## Complete Example for FancyUI

```csharp
// GameInterface/TextGameUI/FancyUI/TextGameWindowFancy.xaml.cs

private void InitializeGame()
{
    // Load from cartridge
    Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
    
    if (startingRoom == null)
    {
        // Fallback to hardcoded game
        startingRoom = GameBuilder.BuildSampleGame();
    }
    
    // Initialize game (unchanged)
    var player = new Player("Hero");
    _game = new Game();
    _game.Initialize(startingRoom, player);
    _game.OutputGenerated += OnGameOutput;
    _game.Start();
    
    // Update UI (unchanged)
    UpdateRoomDisplay();
}
```

## Need More Help?

- See `Cartridges/README.md` for creating custom games
- See `GameLibrary/TextGame/CARTRIDGE_SYSTEM.md` for technical details
- See `GameLibrary/TextGame/CARTRIDGE_IMPLEMENTATION.md` for full overview

## Summary

**Minimal Change:**
Replace hardcoded room creation with:
```csharp
Room? startingRoom = GameBuilder.LoadFromCartridge("Cartridges/HauntedMansion.json");
```

Everything else stays the same!
