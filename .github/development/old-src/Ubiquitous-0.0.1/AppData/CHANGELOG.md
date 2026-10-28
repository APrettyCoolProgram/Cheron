# Changelog

All notable changes to the Ubiquitus project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Comprehensive XML documentation across entire solution
  - App.xaml.cs with startup sequence documentation
  - MainWindow.xaml.cs with UI documentation
  - SplashScreen.xaml.cs with timer and window behavior documentation
  - AssemblyInfo.cs with theme configuration documentation
  - GameEngine.cs with complete API documentation
  - Room.cs with property and method documentation
  - Item.cs with item behavior documentation
  - GameBuilder.cs with game loading documentation
  - GameCartridge.cs with data model documentation
  - CartridgeLoader.cs with loading system documentation
- XML documentation file generation enabled for both projects
  - Ubiquitus.xml generated in build output
  - TextGame.xml generated in build output
- API Documentation in markdown format (AppData/Documentation/Generated/API-Documentation.md)
- Generated XML documentation files copied to AppData/Documentation/Generated/

### Changed
- Project files updated to generate XML documentation files
- Build configuration optimized with documentation generation

### Maintenance
- Cleaned build artifacts (obj/ and bin/ directories)
- Verified build process with comprehensive documentation
- Organized documentation in AppData/Documentation/Generated/

## [0.0.1] - 2025-12-19

### Added
- CHANGELOG.md file to track all version changes
- Solution README.md with project overview and documentation
- Initial WPF application structure for Ubiquitus
- Splash screen with Ubiquitus logo (ubiquitus-v1_1024x1024.png)
  - 3-second display duration
  - Borderless, centered window
  - Transparent background
  - High-quality bitmap scaling
- Main application window (MainWindow.xaml)
- Custom application startup sequence
  - Splash screen displays first
  - Main window opens after splash screen closes
- TextGame library (v0.1.0) for text adventure game functionality
  - Core game engine with command parser
  - Room navigation system
  - Inventory management system
  - Item interaction system
  - Event-driven output architecture
- TextGame JSON cartridge system
  - Data models for game serialization (GameCartridge, RoomData, ItemData)
  - CartridgeLoader for loading games from JSON files
  - Support for metadata extraction
  - Cartridge discovery functionality
- Sample game cartridges in JSON format
  - haunted-mansion.json (fantasy adventure)
  - space-station.json (sci-fi exploration)
  - lost-temple.json (treasure hunting adventure)
- Comprehensive documentation
  - TextGame library README
  - JSON Cartridge system README
  - Cartridge creation guide
  - Console example for TextGame usage
- Development infrastructure
  - .gitignore configuration
  - Development blog (devblog.md)
  - .NET 10.0 targeting
  - WPF application framework

### Changed
- Cartridge files relocated from `GameLibrary/TextGame/Cartridge/` to `Cartridge/`
  - Better separation of library code and application data
  - Centralizes game content within application structure

### Fixed
- Build configuration issues
  - Added GameLibrary/** exclusions to Ubiquitus.csproj
  - Resolved duplicate type definition errors
  - Fixed assembly attribute conflicts
  - Ensured proper project reference handling

### Technical Details
- **Platform**: .NET 10.0 Windows
- **Framework**: WPF (Windows Presentation Foundation)
- **Project Type**: WinExe
- **Language Features**: C# with nullable reference types and implicit usings enabled
- **Dependencies**: TextGame library (v0.1.0)

---

## Version History Summary

| Version | Date | Highlights |
|---------|------|------------|
| Unreleased | In Progress | XML documentation, API documentation, solution optimization |
| 0.0.1 | 2025-12-19 | Initial release with splash screen, TextGame library, and JSON cartridge system |

---

## Notes

### Version Numbering
- **MAJOR.MINOR.PATCH** format (Semantic Versioning)
- **MAJOR**: Incompatible API changes
- **MINOR**: Backward-compatible functionality additions
- **PATCH**: Backward-compatible bug fixes

### Change Categories
- **Added**: New features
- **Changed**: Changes in existing functionality
- **Deprecated**: Soon-to-be removed features
- **Removed**: Removed features
- **Fixed**: Bug fixes
- **Security**: Security vulnerability fixes
- **Maintenance**: Code quality, optimization, and documentation improvements
