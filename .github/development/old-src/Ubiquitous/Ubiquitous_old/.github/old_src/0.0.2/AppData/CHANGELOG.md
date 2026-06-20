# Changelog

All notable changes to the Ubiquitus project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.0.2] - 2025-12-19

### Added
- Splash screen functionality
  - Displays during application startup
  - Shows Ubiquitus logo and version information
  - 3-second auto-close timer
  - Click-to-close functionality
  - Version number dynamically retrieved from assembly
- Custom application startup sequence
  - Splash screen displays before main window
  - Main window appears after splash screen closes
  - Override of `OnStartup` method in App.xaml.cs

### Changed
- Application startup flow modified to show splash screen first
- StartupUri removed from App.xaml in favor of manual window management

### Maintenance
- Version numbers updated to 0.0.2 across the solution
- Documentation updated for new version
- Code optimizations applied

## [0.0.1] - 2025-12-19

### Added
- Comprehensive XML documentation across entire solution
  - App.xaml.cs with startup sequence documentation
  - MainWindow.xaml.cs with UI documentation
  - SplashWindow.xaml.cs with timer and window behavior documentation
  - AssemblyInfo.cs with theme configuration documentation
- XML documentation file generation enabled
  - Ubiquitus.xml generated in build output
- API Documentation in markdown format (AppData/Documentation/api.md)
- CHANGELOG.md file to track all version changes
- Solution README.md with project overview and documentation
- Initial WPF application structure for Ubiquitus
- Main application window (MainWindow.xaml)
- Development infrastructure
  - .gitignore configuration
  - Development blog entries
  - .NET 10.0 targeting
  - WPF application framework

### Technical Details
- **Platform**: .NET 10.0 Windows
- **Framework**: WPF (Windows Presentation Foundation)
- **Project Type**: WinExe
- **Language Features**: C# with nullable reference types and implicit usings enabled

---

## Version History Summary

| Version | Date | Highlights |
|---------|------|------------|
| 0.0.2 | 2025-12-19 | Splash screen implementation, custom startup sequence |
| 0.0.1 | 2025-12-19 | Initial release with XML documentation and WPF framework |

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

