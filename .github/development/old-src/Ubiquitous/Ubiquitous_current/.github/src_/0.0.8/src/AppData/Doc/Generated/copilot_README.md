# Ubiquitous

**A Modern Game Engine Built on .NET 10 and WPF**

[![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/Framework-WPF-blue)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Version](https://img.shields.io/badge/Version-0.0.2-green)](https://github.com/APrettyCoolProgram/ubiquitous/releases/tag/v0.0.2)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Documentation](#documentation)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

**Ubiquitous** is an early-stage game engine built using Windows Presentation Foundation (WPF) and targeting .NET 10. The project is designed with extensibility, maintainability, and modern software development practices in mind.

### Current Status

The project is in **active development** at version **0.0.2**. Current functionality includes:
- Application initialization and startup
- Splash screen with automatic version detection
- Main application window framework
- Text-based game engine library (TextGame)
- Comprehensive XML documentation with single-line summaries
- Clean, optimized codebase

### Vision

Ubiquitous aims to become a full-featured game engine providing:
- Scene and state management
- Asset loading and management
- Input handling
- Rendering system
- Physics integration
- Audio management
- Extensible plugin architecture

---

## Features

### Current Features (v0.0.2)

? **WPF Application Framework**
- Modern .NET 10 implementation
- Clean MVVM-ready architecture
- Nullable reference types enabled

? **Splash Screen**
- Transparent window with application logo
- Automatic version detection and display
- 8-second timer with smooth transition

? **Main Window**
- Text game integration with command input/output
- Command history navigation (up/down arrows)
- Sample adventure implementation
- Standard and fancy UI options

? **MainWindowFancy Enhanced UI**
- Status panel displaying location, moves, and score
- Visual inventory display with item cards
- Color-coded text support ([COLOR:name]text[/COLOR])
- Flashing text animations ([FLASH]text[/FLASH])
- Modern dark theme with syntax highlighting colors

? **TextGame Library**
- Command parser with natural language support
- Room system with directional navigation
- Item system with properties and events
- Player inventory management
- World management system
- Extensible game engine foundation

? **Documentation**
- Comprehensive XML documentation for all code
- Architecture documentation with diagrams
- Simplified API documentation
- Development roadmap
- Task logging system

? **Build System**
- .NET 10 SDK project
- Automated version management
- Resource embedding
- Multi-project solution support

### Upcoming Features (v0.0.3+)

?? UI consolidation and optimization  
?? Enhanced game save/load functionality  
?? Settings/preferences system  
?? Configuration management  
?? Logging framework  

---

## Getting Started

### Prerequisites

To build and run Ubiquitous, you need:

- **Operating System:** Windows 10/11 (WPF requirement)
- **.NET SDK:** .NET 10.0 or higher
  - Download: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- **IDE (Optional but Recommended):**
  - Visual Studio 2025 or later
  - Visual Studio Code with C# extension
  - JetBrains Rider

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/APrettyCoolProgram/ubiquitous.git
   cd ubiquitous
   ```

2. **Navigate to the source directory:**
   ```bash
   cd src
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Build the project:**
   ```bash
   dotnet build
   ```

### Running the Application

**Option 1: Using .NET CLI**
```bash
cd src
dotnet run
```

**Option 2: Using Visual Studio**
1. Open `src/Ubiquitous.csproj` in Visual Studio
2. Press `F5` or click the "Start" button

**Option 3: Run the executable directly**
```bash
cd src/bin/Debug/net10.0-windows
./Ubiquitous.exe
```

### Expected Behavior

When you run the application:
1. A splash screen appears with the Ubiquitous logo
2. The version number is displayed (e.g., "Version 0.0.2")
3. After 8 seconds, the main window appears
4. The splash screen closes automatically

---

## Project Structure

```
ubiquitous/
??? src/                          # Source code directory
?   ??? AppData/                  # Application data
?   ?   ??? Doc/                  # Documentation
?   ?   ?   ??? Generated/        # Auto-generated docs
?   ?   ?       ??? copilot_ARCHITECTURE.md
?   ?   ?       ??? copilot_API.md
?   ?   ?       ??? copilot_README.md
?   ?   ?       ??? copilot_ROADMAP.md
?   ?   ??? PromptLog/            # Prompt logs
?   ?   ?   ??? README.md
?   ?   ??? TaskLog/              # Development task logs
?   ?       ??? README.md
?   ?       ??? [date-time].md    # Individual task logs
?   ??? GameLibrary/              # Game engine libraries
?   ?   ??? TextGame/             # Text-based game engine
?   ?       ??? Game.cs           # Main game controller
?   ?       ??? CommandParser.cs  # Natural language parser
?   ?       ??? Room.cs           # Room and World classes
?   ?       ??? Item.cs           # Item and Inventory classes
?   ?       ??? Player.cs         # Player character
?   ?       ??? README.md         # TextGame documentation
?   ?       ??? TextGame.csproj   # TextGame project file
?   ??? Resources/                # Application resources
?   ?   ??? ubiquitus-splash_512x512.png
?   ??? App.xaml                  # Application definition
?   ??? App.xaml.cs               # Application logic
?   ??? MainWindow.xaml           # Main window UI
?   ??? MainWindow.xaml.cs        # Main window logic
?   ??? SplashScreen.xaml         # Splash screen UI
?   ??? SplashScreen.xaml.cs      # Splash screen logic
?   ??? AssemblyInfo.cs           # Assembly configuration
?   ??? Ubiquitous.csproj         # Project file
??? CHANGELOG.md                  # Version history
??? ROADMAP.md                    # Development roadmap
??? README.md                     # This file
??? LICENSE                       # License information
```

---

## Documentation

Comprehensive documentation is available in the `AppData/Doc/Generated/` directory:

### Architecture Documentation
**File:** `AppData/Doc/Generated/copilot_ARCHITECTURE.md`

Detailed architectural overview including:
- Component relationships and diagrams
- Startup flow sequence diagrams
- Class hierarchy
- Technology stack
- Design patterns
- Future architecture extensions

### API Documentation
**File:** `AppData/Doc/Generated/copilot_API.md`

Simplified API reference covering:
- All public classes (App, SplashScreen, MainWindow)
- TextGame library API
- Methods and properties
- Usage examples
- Code samples
- Best practices

### Roadmap
**File:** `AppData/Doc/Generated/copilot_ROADMAP.md`

Development roadmap detailing:
- Completed milestones
- Upcoming features
- Planned versions
- Future considerations

### Task Logs
**Directory:** `AppData/TaskLog/`

Development session logs documenting:
- Tasks completed
- Changes made
- Version updates
- Technical decisions

---

## Roadmap

### Version 0.0.2 (Current) ?
- Splash screen implementation
- Version management system
- TextGame library foundation
- Complete XML documentation
- Single-line XML documentation summaries
- Optimized and cleaned codebase
- Build artifacts removed

### Version 0.0.3 (Next Release)
- Text game UI integration
- Game save/load functionality
- Settings system
- Enhanced documentation

### Version 0.1.0 (Planned)
- Additional game types support
- Advanced input handling
- Scene management
- State persistence

### Version 0.2.0 (Future)
- Graphical game engine capabilities
- Asset pipeline
- Performance optimizations
- Comprehensive test coverage

See [copilot_ROADMAP.md](AppData/Doc/Generated/copilot_ROADMAP.md) for complete details.

---

## Development

### Building from Source

```bash
# Clone repository
git clone https://github.com/APrettyCoolProgram/ubiquitous.git
cd ubiquitous/src

# Build
dotnet build

# Run
dotnet run
```

### Development Requirements

- C# 13 language features
- .NET 10.0 SDK
- Windows 10/11 for WPF support
- Visual Studio 2025+ (recommended)

### Code Style

The project follows standard C# conventions:
- XML documentation for all public APIs
- Nullable reference types enabled
- Implicit usings for common namespaces
- XAML for UI definition
- Code-behind for UI logic

### Version Management

Version numbers are managed in project files:
- `Ubiquitous.csproj` - Main application version
- `GameLibrary/TextGame/TextGame.csproj` - TextGame library version

All projects synchronized at version 0.0.2.

---

## Contributing

Contributions are welcome! Here's how you can help:

### Reporting Issues

Found a bug or have a feature request?
1. Check existing issues: [Issues](https://github.com/APrettyCoolProgram/ubiquitous/issues)
2. Create a new issue with detailed information

### Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow existing code style and conventions
- Add XML documentation for new public APIs
- Update relevant documentation files
- Test your changes thoroughly
- Update CHANGELOG.md with your changes

---

## Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Application framework |
| WPF | (included in .NET 10) | UI framework |
| C# | 13 | Programming language |
| XAML | 2006 | UI markup language |

### Dependencies

Currently, the project has **no external NuGet dependencies** and relies solely on .NET and WPF framework features.

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Contact and Support

- **Repository:** [https://github.com/APrettyCoolProgram/ubiquitous](https://github.com/APrettyCoolProgram/ubiquitous)
- **Issues:** [https://github.com/APrettyCoolProgram/ubiquitous/issues](https://github.com/APrettyCoolProgram/ubiquitous/issues)
- **Author:** A Pretty Cool Program

---

## Acknowledgments

- Built with .NET 10 and Windows Presentation Foundation
- Follows semantic versioning principles
- Documentation format inspired by Keep a Changelog

---

## Status and Updates

**Current Version:** 0.0.2  
**Last Updated:** December 20, 2025  
**Status:** Active Development  
**Next Release:** 0.0.3 (Planned)

---

*Ubiquitous is in early development. Features and APIs are subject to change.*
