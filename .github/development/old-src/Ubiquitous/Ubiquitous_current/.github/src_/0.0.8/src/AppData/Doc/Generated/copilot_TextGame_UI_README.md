# TextGame UI

This directory contains user interface implementations for the TextGame library.

## Directory Structure

```
GameInterface/TextGameUI/
??? BasicUI/
?   ??? TextGameWindow.xaml
?   ??? TextGameWindow.xaml.cs
??? FancyUI/
    ??? TextGameWindowFancy.xaml
    ??? TextGameWindowFancy.xaml.cs
```

## UI Implementations

### BasicUI

**Location:** `GameInterface/TextGameUI/BasicUI/`

A simplified black and white interface for text adventures.

**Features:**
- Clean, minimalist design
- Game output display
- Command history panel
- Command input with history navigation (Up/Down arrows)
- Monospace font (Consolas) for retro feel

**Class:** `Ubiquitous.GameInterface.TextGameUI.BasicUI.TextGameWindow`

### FancyUI

**Location:** `GameInterface/TextGameUI/FancyUI/`

An enhanced interface with modern styling and additional features.

**Features:**
- Dark theme with Visual Studio Code-inspired colors
- Three-panel layout:
  - Game output (with rich text formatting)
  - Status panel (location, moves, score)
  - Inventory panel (visual item cards)
- Command history panel
- Color-coded text support using `[COLOR:name]text[/COLOR]` tags
- Flashing text support using `[FLASH]text[/FLASH]` tags
- Animated hover effects

**Class:** `Ubiquitous.GameInterface.TextGameUI.FancyUI.TextGameWindowFancy`

## Color Tags (FancyUI)

The FancyUI supports the following color tags in game text:

- `red`
- `green`
- `blue`
- `yellow`
- `cyan`
- `magenta`
- `white`
- `gray` / `grey`
- `orange`
- `purple`

**Example:**
```
"You found a [COLOR:red]red key[/COLOR]!"
"[FLASH]DANGER![/FLASH] The door is locked."
```

## Usage

To launch a TextGame UI window:

```csharp
// Basic UI
var basicWindow = new Ubiquitous.GameInterface.TextGameUI.BasicUI.TextGameWindow();
basicWindow.Show();

// Fancy UI
var fancyWindow = new Ubiquitous.GameInterface.TextGameUI.FancyUI.TextGameWindowFancy();
fancyWindow.Show();
```

## Game Initialization

Both UI implementations include sample game initialization with:
- Three connected rooms (Entrance Hall, Library, Garden)
- Four items (key, book, desk, rose)
- Basic navigation commands

The initialization code can be customized or replaced with your own game setup.

## Future Enhancements

Potential improvements for TextGame UI:
- Configurable themes
- Save/load game state
- Custom font selection
- Sound effect integration
- Map display
- Achievement tracking
- Multiplayer support
