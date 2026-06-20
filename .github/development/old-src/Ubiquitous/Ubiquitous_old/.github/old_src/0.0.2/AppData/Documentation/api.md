# Ubiquitus API Documentation

**Version**: 0.0.2  
**Generated**: December 2025  
**Target Framework**: .NET 10.0-windows

---

## Table of Contents

1. [Overview](#overview)
2. [Namespace: Ubiquitus](#namespace-ubiquitus)
   - [App Class](#app-class)
   - [SplashWindow Class](#splashwindow-class)
   - [MainWindow Class](#mainwindow-class)
3. [Assembly Configuration](#assembly-configuration)

---

## Overview

Ubiquitus is a WPF-based game engine application for running text adventure games. This API documentation covers the core application classes and their functionality.

### Key Features

- **WPF Framework**: Modern Windows desktop application
- **Splash Screen**: Branded startup experience with version display
- **Custom Startup**: Managed application initialization sequence
- **Text Adventure Support**: Game engine for interactive fiction (planned)
- **Extensible Architecture**: Designed for future expansion
- **.NET 10 Targeting**: Latest framework features

---

## Namespace: Ubiquitus

The primary namespace containing all application classes.

### App Class

**Inheritance**: `System.Windows.Application`

#### Description

Represents the main application class for the Ubiquitus WPF application. Handles application-level events and initialization.

#### Remarks

This class serves as the entry point for the WPF application and manages the application lifecycle, including startup, shutdown, and unhandled exception handling. In version 0.0.2, it implements a custom startup sequence that displays a splash screen before showing the main window.

#### Class Declaration

```csharp
public partial class App : Application
```

#### Properties

Inherits all properties from `System.Windows.Application`.

#### Methods

Inherits all methods from `System.Windows.Application`.

##### OnStartup(StartupEventArgs)

Overrides the default startup behavior to implement a custom startup sequence with splash screen.

**Signature**:
```csharp
protected override void OnStartup(StartupEventArgs e)
```

**Parameters**:
- `e` (StartupEventArgs): The startup event arguments

**Behavior**:
1. Creates and shows the SplashWindow
2. Creates the MainWindow instance (but doesn't show it)
3. Waits for SplashWindow to close
4. Shows MainWindow when splash closes

**Usage Example**:
```csharp
// This method is called automatically by WPF
// Override in App.xaml.cs to customize startup
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    var splashWindow = new SplashWindow();
    splashWindow.Show();
    
    var mainWindow = new MainWindow();
    splashWindow.Closed += (sender, args) =>
    {
        mainWindow.Show();
    };
}
```

#### Events

Inherits all events from `System.Windows.Application`:
- `Startup` - Occurs when the application starts
- `Exit` - Occurs when the application exits
- `SessionEnding` - Occurs when the user ends the Windows session

#### Example

The App class is defined in XAML with startup configuration:

```xml
<Application x:Class="Ubiquitus.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
         
    </Application.Resources>
</Application>
```

**Note**: The `StartupUri` property is not used in v0.0.2 because startup is handled manually via the `OnStartup` override.

---

### SplashWindow Class

**Inheritance**: `System.Windows.Window`

#### Description

Represents the splash screen window displayed during application startup. Shows the application logo and version information for 3 seconds or until clicked.

#### Remarks

The splash screen provides a professional startup experience with the following features:
- Displays the Ubiquitus logo
- Shows version information retrieved from assembly attributes
- Automatically closes after 3 seconds
- Can be manually closed by clicking anywhere on the window
- Uses a DispatcherTimer for automatic closure

#### Class Declaration

```csharp
public partial class SplashWindow : Window
```

#### Constructor

##### SplashWindow()

Initializes a new instance of the SplashWindow class. Sets up the splash screen with version information and configures the auto-close timer.

**Signature**:
```csharp
public SplashWindow()
```

**Behavior**:
1. Calls `InitializeComponent()` to load XAML
2. Retrieves and displays version number from assembly
3. Creates a DispatcherTimer set to 3 seconds
4. Starts the timer for automatic closure

**Usage Example**:
```csharp
var splash = new SplashWindow();
splash.Show(); // Display splash screen
// Automatically closes after 3 seconds or when clicked
```

#### Properties

Inherits all properties from `System.Windows.Window`:
- `Title` - Gets or sets the window title
- `Width` - Gets or sets the window width
- `Height` - Gets or sets the window height
- `WindowStyle` - Set to `None` for borderless display
- `WindowStartupLocation` - Set to `CenterScreen`
- `AllowsTransparency` - Set to `true` for transparent background
- `Background` - Set to `Transparent`

#### Methods

##### SetVersionNumber()

Private method that retrieves the version from the assembly and updates the version text display.

**Signature**:
```csharp
private void SetVersionNumber()
```

**Behavior**:
- Retrieves assembly version using Reflection
- Formats version as "Version X.Y.Z"
- Updates the VersionText TextBlock

##### Timer_Tick(object?, EventArgs)

Private event handler that closes the splash screen when the timer expires.

**Signature**:
```csharp
private void Timer_Tick(object? sender, EventArgs e)
```

**Behavior**:
1. Stops the timer
2. Closes the window

##### Window_MouseDown(object, MouseButtonEventArgs)

Private event handler that allows users to close the splash screen by clicking.

**Signature**:
```csharp
private void Window_MouseDown(object sender, MouseButtonEventArgs e)
```

**Parameters**:
- `sender` (object): The source of the event
- `e` (MouseButtonEventArgs): Mouse button event data

**Behavior**:
1. Stops the timer (preventing duplicate close)
2. Closes the window

Also inherits all methods from `System.Windows.Window`:
- `Show()` - Opens the window
- `Close()` - Closes the window

#### XAML Configuration

The SplashWindow is defined in XAML with borderless, transparent styling:

```xml
<Window x:Class="Ubiquitus.SplashWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Ubiquitus"
        Width="512"
        Height="512"
        WindowStyle="None"
        AllowsTransparency="True"
        Background="Transparent"
        WindowStartupLocation="CenterScreen"
        MouseDown="Window_MouseDown">
    <Grid>
        <Image Source="AppData/Image/Logo/ubiquitus-splash_512x512.png"
               Stretch="Uniform"
               RenderOptions.BitmapScalingMode="HighQuality"/>
        <TextBlock x:Name="VersionText"
                   Text="Version 0.0.2"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Bottom"
                   Margin="0,0,0,20"
                   FontSize="16"
                   Foreground="White"/>
    </Grid>
</Window>
```

---

### MainWindow Class

**Inheritance**: `System.Windows.Window`

#### Description

Represents the main window of the Ubiquitus application. Provides the primary user interface for interacting with the game engine.

#### Remarks

This window serves as the main container for the application's UI elements and handles user interactions, game display, and command input.

#### Class Declaration

```csharp
public partial class MainWindow : Window
```

#### Constructor

##### MainWindow()

Initializes a new instance of the MainWindow class. Sets up the window components and initializes the UI.

**Signature**:
```csharp
public MainWindow()
```

**Usage Example**:
```csharp
var mainWindow = new MainWindow();
mainWindow.Show();
```

#### Properties

Inherits all properties from `System.Windows.Window`:
- `Title` - Gets or sets the window title
- `Width` - Gets or sets the window width
- `Height` - Gets or sets the window height
- `Content` - Gets or sets the content of the window

#### Methods

Inherits all methods from `System.Windows.Window`:
- `Show()` - Opens the window
- `ShowDialog()` - Opens the window as a modal dialog
- `Close()` - Closes the window

#### XAML Configuration

The MainWindow is defined in XAML:

```xml
<Window x:Class="Ubiquitus.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MainWindow"
        Width="800"
        Height="450">
    <Grid />
</Window>
```

---

## Assembly Configuration

### ThemeInfo Attribute

The application uses the `ThemeInfo` assembly attribute to configure theme resource dictionaries.

#### Declaration

```csharp
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]
```

#### Parameters

1. **First Parameter** (`ResourceDictionaryLocation.None`):
   - Specifies where theme-specific resource dictionaries are located
   - `None` indicates no theme-specific resources are used
   - WPF will not search for theme-specific resources in separate assemblies

2. **Second Parameter** (`ResourceDictionaryLocation.SourceAssembly`):
   - Specifies where the generic resource dictionary is located
   - `SourceAssembly` means the dictionary is in the same assembly
   - Used as fallback when resources aren't found in page or application dictionaries

#### Purpose

This configuration tells WPF's resource resolution system where to find resources:
- Theme-specific resources: Not used (None)
- Generic resources: Located in the source assembly
- Fallback chain: Page ? Application ? Generic dictionary

---

## Architecture Notes

### Application Startup Sequence

1. **App.xaml.cs** instantiates the Application
2. **OnStartup override** executes custom startup logic
3. **SplashWindow** created and displayed
4. **Version number** retrieved and shown
5. **3-second timer** starts (or wait for click)
6. **MainWindow** created in background
7. **SplashWindow closes** after timer/click
8. **MainWindow displays** with specified dimensions (800x450)

### Design Patterns

- **Partial Classes**: XAML and code-behind are split using partial classes
- **Code-Behind Pattern**: Event handling and logic in .cs files
- **XAML Definition**: UI structure defined declaratively in .xaml files

### File Structure

```
Ubiquitus/
??? App.xaml               (Application XAML definition)
??? App.xaml.cs            (Application code-behind with custom startup)
??? SplashWindow.xaml      (Splash screen XAML definition)
??? SplashWindow.xaml.cs   (Splash screen code-behind with timer)
??? MainWindow.xaml        (Main window XAML definition)
??? MainWindow.xaml.cs     (Main window code-behind)
??? AssemblyInfo.cs        (Assembly-level attributes)
??? Ubiquitus.csproj       (Project configuration)
```

---

## Future Expansion

The current architecture is designed to support:

### Planned Features
- **Game Engine Integration**: TextGame library integration
- **Cartridge System**: JSON-based game loading
- **Save/Load System**: Game state persistence
- **UI Enhancements**: Terminal-style display, themes
- **Command System**: Text input and parsing

### Extension Points
- **MainWindow.xaml**: Can be extended with game UI controls
- **App.xaml**: Can define application-wide resources and styles
- **Event Handlers**: Can be added to MainWindow for game logic

---

## Dependencies

### Framework
- **.NET 10.0-windows**: Target framework
- **Windows Presentation Foundation (WPF)**: UI framework

### NuGet Packages
Currently no external NuGet packages are required.

### References
- `System.Windows`
- `System.Windows.Controls`
- `System.Windows.Data`
- `System.Windows.Documents`
- `System.Windows.Input`
- `System.Windows.Media`
- `System.Windows.Navigation`
- `System.Windows.Shapes`

---

## Build Configuration

### Project Settings
- **Output Type**: WinExe (Windows application)
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled
- **UseWPF**: true
- **Documentation File**: Generated automatically

### Generated Files
- **Ubiquitus.xml**: XML documentation file
- **Ubiquitus.exe**: Compiled executable

---

## See Also

- [CHANGELOG.md](../../CHANGELOG.md) - Version history and changes
- [ROADMAP.md](../../ROADMAP.md) - Planned features and enhancements
- [Microsoft WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)

---

**Document Version**: 1.0  
**Last Updated**: December 2025
