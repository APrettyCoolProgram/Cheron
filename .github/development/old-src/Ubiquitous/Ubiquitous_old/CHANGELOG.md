# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.0.2] - 2025-12-20

### Added
- Splash screen window with application logo
- Automatic version display on splash screen
- 8-second splash screen timer with automatic transition to main window
- Transparent background for splash screen
- Comprehensive XML documentation for all classes and methods
- TextGame library with complete game engine features:
  - Command parser with natural language support
  - Room system with directional exits
  - Item system with properties and events
  - Player system with inventory management
  - World management for organizing rooms
- MainWindow with game selection menu
- CartridgeListWindow for game cartridge selection
- TextGameWindow for basic text game UI
- TextGameWindowFancy for enhanced text game UI with status panel and inventory display

### Changed
- Application startup now shows splash screen before main window
- Version numbers standardized to 0.0.2 across all projects
- All XML documentation summaries converted to single-line format

### Optimized
- Removed unused using directives from App.xaml.cs and MainWindow.xaml.cs
- Cleaned up build artifacts (obj/bin directories)
- Removed backup temporary files
- Improved code organization and documentation
- Complete XML documentation coverage

## [0.0.1] - 2025-12-20

### Added
- Initial project structure
- WPF application framework
- MainWindow and App classes
- AppData directory structure for documentation and logs

[Unreleased]: https://github.com/APrettyCoolProgram/ubiquitous/compare/v0.0.2...HEAD
[0.0.2]: https://github.com/APrettyCoolProgram/ubiquitous/compare/v0.0.1...v0.0.2
[0.0.1]: https://github.com/APrettyCoolProgram/ubiquitous/releases/tag/v0.0.1
