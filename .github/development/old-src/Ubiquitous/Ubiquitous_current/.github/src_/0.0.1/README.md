# Ubiquitus

A modern WPF application for playing interactive text adventure games, powered by the TextGame library.

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![License](https://img.shields.io/badge/license-TBD-yellow)

## Overview

Ubiquitus is a desktop application that provides a graphical interface for playing text-based adventure games. It leverages the TextGame library to offer a modern take on classic interactive fiction, supporting JSON-based game cartridges that can be easily created and shared.

## Features

- **Modern WPF Interface**: Clean, professional user interface built with Windows Presentation Foundation
- **Splash Screen**: Eye-catching startup experience with the Ubiquitus logo
- **TextGame Integration**: Full integration with the TextGame library for rich interactive fiction gameplay
- **JSON Cartridge Support**: Play games defined in simple JSON files
- **Multiple Game Formats**: Comes with sample cartridges including:
  - Haunted Mansion (fantasy adventure)
  - Space Station (sci-fi exploration)
  - Lost Temple (treasure hunting)

## System Requirements

- **Operating System**: Windows 10/11 (or later)
- **.NET Runtime**: .NET 10.0 or higher
- **Framework**: WPF support required

## Quick Start

### Running the Application

1. **Clone the repository**:
   ```bash
   git clone https://github.com/APrettyCoolProgram/ubiquitous
   cd ubiquitous/src
   ```

2. **Build the project**:
   ```bash
   dotnet build Ubiquitus.csproj
   ```

3. **Run the application**:
   ```bash
   dotnet run --project Ubiquitus.csproj
   ```

### Playing Games

The application includes sample cartridges located in the `Cartridge/` directory:
- `haunted-mansion.json` - Explore a mysterious mansion
- `space-station.json` - Navigate a sci-fi space station
- `lost-temple.json` - Discover ancient treasures

## Project Structure

```
src/
??? Ubiquitus.csproj              # Main application project
??? MainWindow.xaml[.cs]          # Main application window
??? App.xaml[.cs]                 # Application startup logic
??? SplashScreen.xaml[.cs]        # Splash screen window
??? AppData/                      # Application data and resources
?   ??? CHANGELOG.md              # Version history
?   ??? Devblog/                  # Development documentation
?   ?   ??? devblog.md
?   ??? Documentation/            # Generated documentation
?   ??? Image/                    # Image resources
?       ??? Logo/
?           ??? ubiquitus-v1_1024x1024.png
??? Cartridge/                    # Game cartridge JSON files
?   ??? haunted-mansion.json
?   ??? space-station.json
?   ??? lost-temple.json
??? GameLibrary/                  # Game engine library
    ??? TextGame/                 # TextGame library project
        ??? TextGame.csproj
        ??? GameEngine.cs         # Core game logic
        ??? Room.cs               # Room definitions
        ??? Item.cs               # Item definitions
        ??? GameBuilder.cs        # Game construction helpers
        ??? Models/               # Data models
        ?   ??? GameCartridge.cs
        ??? Loaders/              # Cartridge loading
            ??? CartridgeLoader.cs
```

## TextGame Library

The TextGame library provides the core functionality for interactive fiction gameplay:

### Features
- **Command Parser**: Natural language command processing
- **Room Navigation**: Move between connected locations
- **Inventory System**: Pick up, drop, and use items
- **Item Interactions**: Examine and interact with objects
- **Event-Driven Architecture**: Clean separation between game logic and UI

### Supported Commands
- `go/move [direction]` - Navigate (north, south, east, west, up, down)
- `look` - Examine current location
- `take/get [item]` - Pick up items
- `inventory/inv` - View carried items
- `use [item]` - Use items from inventory
- `examine [item]` - Get detailed descriptions
- `help` - Display available commands
- `quit/exit` - End game session

For detailed documentation, see [TextGame Library README](GameLibrary/TextGame/README.md).

## Creating Custom Cartridges

Games are defined in JSON format, making it easy to create custom adventures without coding.

### Minimal Example

```json
{
  "name": "My Adventure",
  "description": "A custom text adventure",
  "author": "Your Name",
  "version": "1.0.0",
  "startingRoomId": "start",
  "rooms": [
    {
      "id": "start",
      "name": "Starting Room",
      "description": "You are in a simple room.",
      "exits": {},
      "itemIds": []
    }
  ],
  "items": []
}
```

For a complete guide to creating cartridges, see [Cartridge Creation Guide](GameLibrary/TextGame/Cartridge/README.md).

## Development

### Building from Source

```bash
# Build the entire solution
dotnet build

# Build just the TextGame library
dotnet build GameLibrary/TextGame/TextGame.csproj

# Build just the Ubiquitus application
dotnet build Ubiquitus.csproj
```

### Project Configuration

The Ubiquitus project is configured with:
- **.NET 10.0** targeting
- **WPF** framework enabled
- **Nullable reference types** enabled
- **Implicit usings** enabled
- **Assembly versioning**: 0.0.1

### Dependencies

The Ubiquitus application references:
- **TextGame** (v0.1.0) - Local project reference

## Version History

See [CHANGELOG.md](AppData/CHANGELOG.md) for detailed version history and release notes.

**Current Version**: 0.0.1 (Initial Release)

## Documentation

- **[CHANGELOG](AppData/CHANGELOG.md)** - Version history and release notes
- **[ROADMAP](AppData/ROADMAP.md)** - Future development plans and priorities
- **[Development Blog](AppData/Devblog/devblog.md)** - Detailed development notes and decisions
- **[API Documentation](AppData/Documentation/Generated/API-Documentation.md)** - Complete API reference
- **[TextGame Library Documentation](GameLibrary/TextGame/README.md)** - Library API and usage
- **[Cartridge System Guide](GameLibrary/TextGame/Cartridge/README.md)** - Creating custom games

### Generated Documentation

The project automatically generates XML documentation files during build:
- `Ubiquitus.xml` - Application API documentation
- `TextGame.xml` - TextGame library API documentation

These files are copied to `AppData/Documentation/Generated/` for reference and tool integration.

## Architecture

### Application Flow

```
Application Start
    ?
SplashScreen.xaml (3 seconds)
    ?
MainWindow.xaml
    ?
[Game UI Implementation - Future]
```

### Component Diagram

```
???????????????????????????????????????
?         Ubiquitus (WPF App)         ?
?  ?????????????     ???????????????  ?
?  ? Splash    ? ?   ? Main Window ?  ?
?  ? Screen    ?     ?  (Game UI)  ?  ?
?  ?????????????     ???????????????  ?
???????????????????????????????????????
              ?
???????????????????????????????????????
?      TextGame Library (v0.1.0)      ?
?  ???????????????  ????????????????  ?
?  ? GameEngine  ?  ? Cartridge    ?  ?
?  ?             ?  ? Loader       ?  ?
?  ? - Commands  ?  ?              ?  ?
?  ? - Navigation?  ? - JSON Parse ?  ?
?  ? - Inventory ?  ? - Game Build ?  ?
?  ???????????????  ????????????????  ?
???????????????????????????????????????
              ?
???????????????????????????????????????
?       JSON Cartridges (.json)       ?
?  ???????????? ???????????? ???????? ?
?  ? Haunted  ? ?  Space   ? ? Lost ? ?
?  ? Mansion  ? ? Station  ? ?Temple? ?
?  ???????????? ???????????? ???????? ?
???????????????????????????????????????
```

## Roadmap

### Planned Features
- [ ] Complete main window UI implementation
- [ ] Game selection interface
- [ ] In-game text display and input
- [ ] Save/load game progress
- [ ] Cartridge management UI
- [ ] Settings and preferences
- [ ] Dark/light theme support
- [ ] Syntax highlighting for commands
- [ ] Game history/transcript saving

### Future Enhancements
- [ ] Multiplayer/networked gameplay
- [ ] Sound effects and music support
- [ ] Achievements system
- [ ] Built-in cartridge editor
- [ ] Online cartridge repository
- [ ] Mod/plugin support

## Contributing

*Contribution guidelines coming soon*

## License

*License to be determined*

## Authors

- **Chris Banwarth** - Initial development

## Acknowledgments

- Inspired by classic text adventure games like Zork
- Built with modern .NET and WPF technologies
- Community feedback and testing

## Contact & Support

- **Repository**: [https://github.com/APrettyCoolProgram/ubiquitous](https://github.com/APrettyCoolProgram/ubiquitous)
- **Issues**: [GitHub Issues](https://github.com/APrettyCoolProgram/ubiquitous/issues)

---

*Built with .NET 10.0 and WPF | December 2025*
