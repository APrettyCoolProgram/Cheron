# Ubiquitous Architecture Documentation

## Table of Contents
- [Overview](#overview)
- [Application Architecture](#application-architecture)
- [Component Relationships](#component-relationships)
- [Startup Flow](#startup-flow)
- [Project Structure](#project-structure)
- [Class Hierarchy](#class-hierarchy)
- [TextGame Library Architecture](#textgame-library-architecture)
- [Technology Stack](#technology-stack)

## Overview

Ubiquitous is a modern game engine built on WPF and .NET 10. The architecture follows a traditional WPF MVVM-ready pattern with a clean separation of concerns and emphasis on maintainability. The solution includes a text-based game engine library (TextGame) that provides a foundation for interactive fiction and text adventure games.

**Current Version:** 0.0.2

## Application Architecture

```mermaid
graph TB
    subgraph "Ubiquitous Game Engine"
        A[App.xaml/App.xaml.cs<br/>Application Entry Point]
        B[SplashScreen.xaml/SplashScreen.xaml.cs<br/>Startup Screen]
        C[MainWindow.xaml/MainWindow.xaml.cs<br/>Primary Window]
        D[Resources<br/>Images & Assets]
        E[AppData<br/>Documentation & Logs]
        F[TextGame Library<br/>Game Engine]
    end
    
    A -->|Startup| B
    B -->|Timer: 8 seconds| C
    B -->|Displays| D
    A -.->|References| E
    C -.->|Future: Uses| F
    
    style A fill:#e1f5ff
    style B fill:#fff4e1
    style C fill:#e8f5e9
    style D fill:#f3e5f5
    style E fill:#fce4ec
    style F fill:#ffe0b2
```

## Component Relationships

```mermaid
graph LR
    subgraph "UI Layer"
        A[App]
        B[SplashScreen]
        C[MainWindow]
    end
    
    subgraph "Game Engine Layer"
        F[TextGame.Game]
        G[TextGame.CommandParser]
        H[TextGame.World]
    end
    
    subgraph "Data Layer"
        D[AppData/Doc]
        E[AppData/PromptLog]
    end
    
    subgraph "Resources"
        I[Images]
        J[Configuration]
    end
    
    A -->|Creates| B
    B -->|Transitions to| C
    A -->|MainWindow Property| C
    B -->|Reads Version| J
    B -->|Displays| I
    C -.->|Future: Uses| F
    F -->|Uses| G
    F -->|Manages| H
    C -.->|Future: Uses| D
    
    style A fill:#bbdefb
    style B fill:#fff9c4
    style C fill:#c8e6c9
    style F fill:#ffcc80
    style G fill:#ffb74d
    style H fill:#ffa726
```

## Startup Flow

```mermaid
sequenceDiagram
    participant User
    participant App
    participant SplashScreen
    participant DispatcherTimer
    participant MainWindow
    
    User->>App: Launch Application
    App->>App: Initialize Application
    App->>SplashScreen: Create & Show
    SplashScreen->>SplashScreen: SetVersionText()
    Note over SplashScreen: Read Assembly Version<br/>(0.0.2.0)
    SplashScreen->>DispatcherTimer: Start (8 seconds)
    Note over SplashScreen,DispatcherTimer: Display splash screen<br/>with logo and version
    DispatcherTimer-->>SplashScreen: Timer_Tick (after 8s)
    SplashScreen->>DispatcherTimer: Stop Timer
    SplashScreen->>MainWindow: Create & Show
    SplashScreen->>App: Set MainWindow
    SplashScreen->>SplashScreen: Close()
    MainWindow->>User: Display Main Window
```

## Project Structure

```mermaid
graph TD
    Root[ubiquitous/]
    Src[src/]
    Docs[Docs/]
    
    Root --> Src
    Root --> Docs
    Root --> CHANGELOG[CHANGELOG.md]
    Root --> ROADMAP[ROADMAP.md]
    Root --> README[README.md]
    
    Src --> AppData[AppData/]
    Src --> GameLib[GameLibrary/]
    Src --> Resources[Resources/]
    Src --> XAML[XAML Files]
    Src --> CS[C# Files]
    Src --> Proj[Ubiquitous.csproj]
    
    GameLib --> TextGame[TextGame/]
    TextGame --> GameCS[Game.cs]
    TextGame --> ParserCS[CommandParser.cs]
    TextGame --> RoomCS[Room.cs]
    TextGame --> ItemCS[Item.cs]
    TextGame --> PlayerCS[Player.cs]
    TextGame --> TextProj[TextGame.csproj]
    
    AppData --> DocFolder[Doc/]
    AppData --> PromptLog[PromptLog/]
    
    DocFolder --> Generated[Generated/]
    
    Resources --> Images[ubiquitus-splash_512x512.png]
    
    XAML --> AppXAML[App.xaml]
    XAML --> MainXAML[MainWindow.xaml]
    XAML --> SplashXAML[SplashScreen.xaml]
    
    CS --> AppCS[App.xaml.cs]
    CS --> MainCS[MainWindow.xaml.cs]
    CS --> SplashCS[SplashScreen.xaml.cs]
    CS --> Assembly[AssemblyInfo.cs]
    
    Docs --> Architecture[Architecture.md]
    Docs --> API[API.md]
    
    style Root fill:#e3f2fd
    style Src fill:#f3e5f5
    style GameLib fill:#ffe0b2
    style TextGame fill:#ffcc80
    style AppData fill:#fff3e0
    style Resources fill:#e8f5e9
```

## Class Hierarchy

```mermaid
classDiagram
    Application <|-- App
    Window <|-- SplashScreen
    Window <|-- MainWindow
    
    class Application {
        <<WPF Framework>>
        +Current MainWindow
    }
    
    class App {
        +Partial class
        +Entry point
    }
    
    class Window {
        <<WPF Framework>>
        +Show()
        +Close()
    }
    
    class SplashScreen {
        -DispatcherTimer timer
        +SplashScreen()
        -SetVersionText()
        -Timer_Tick(sender, e)
    }
    
    class MainWindow {
        +MainWindow()
    }
    
    App --> SplashScreen : Creates on startup
    SplashScreen --> MainWindow : Creates after delay
    App --> MainWindow : Sets as Current.MainWindow
```

## TextGame Library Architecture

### TextGame Component Diagram

```mermaid
graph TB
    subgraph "TextGame Library"
        G[Game<br/>Main Controller]
        CP[CommandParser<br/>Input Processing]
        W[World<br/>Room Manager]
        R[Room<br/>Location]
        I[Item<br/>Objects]
        Inv[Inventory<br/>Collection]
        P[Player<br/>Character]
    end
    
    G -->|Uses| CP
    G -->|Manages| W
    G -->|Manages| P
    W -->|Contains| R
    R -->|Contains| I
    R -->|Connected to| R
    P -->|Has| Inv
    P -->|In| R
    Inv -->|Contains| I
    
    style G fill:#ff9800
    style CP fill:#ffa726
    style W fill:#ffb74d
    style R fill:#ffcc80
    style I fill:#ffe0b2
    style Inv fill:#fff3e0
    style P fill:#ffab91
```

### TextGame Class Hierarchy

```mermaid
classDiagram
    class Game {
        -CommandParser _parser
        -World _world
        -Player _player
        -bool _isRunning
        +Initialize(Room, Player)
        +Start()
        +World: World
        +Player: Player
    }
    
    class CommandParser {
        -Dictionary~string, CommandAction~ _actionWords
        -Dictionary~string, Direction~ _directionWords
        +Parse(string): Command
        +AddActionWord(string, CommandAction)
        +AddDirectionWord(string, Direction)
    }
    
    class Command {
        +CommandAction Action
        +Direction? Direction
        +string? Target
        +string? SecondaryTarget
    }
    
    class World {
        -List~Room~ _rooms
        +AddRoom(Room)
        +GetRoom(string): Room
        +GetAllRooms(): List~Room~'
        +RemoveRoom(Room)
    }
    
    class Room {
        -Dictionary~Direction, Room~ _exits
        -List~Item~ _items
        +string Name
        +string Description
        +bool IsVisited
        +AddExit(Direction, Room)
        +AddTwoWayExit(Direction, Room)
        +GetExit(Direction): Room
        +AddItem(Item)
        +RemoveItem(Item)
        +GetItem(string): Item
    }
    
    class Item {
        +string Name
        +string Description
        +List~string~ Aliases
        +bool IsPickupable
        +bool IsUsable
        +Dictionary~string, object~ Properties
        +event EventHandler~ItemUseEventArgs~ Used
        +AddAlias(string)
        +SetProperty(string, object)
        +GetProperty~T~(string): T
    }
    
    class Inventory {
        -List~Item~ _items
        -int _capacity
        +int Capacity
        +int Count
        +AddItem(Item): bool
        +RemoveItem(Item): bool
        +GetItem(string): Item
        +GetAllItems(): List~Item~'
        +HasItem(Item): bool
        +Clear()
    }
    
    class Player {
        +string Name
        +Inventory Inventory
        +Room CurrentRoom
        +Dictionary~string, object~ Properties
        +SetProperty(string, object)
        +GetProperty~T~(string): T
        +HasProperty(string): bool
    }
    
    Game --> CommandParser
    Game --> World
    Game --> Player
    CommandParser --> Command
    World --> Room
    Room --> Item
    Player --> Inventory
    Player --> Room
    Inventory --> Item
```

### Game Flow Diagram

```mermaid
sequenceDiagram
    participant User
    participant Game
    participant CommandParser
    participant Player
    participant Room
    participant Item
    
    User->>Game: Start()
    Game->>Game: DisplayCurrentLocation()
    Game->>User: Show room description
    User->>Game: Input command
    Game->>CommandParser: Parse(input)
    CommandParser-->>Game: Command object
    Game->>Game: ExecuteCommand()
    
    alt Move Command
        Game->>Room: GetExit(direction)
        Room-->>Game: Next room
        Game->>Player: Set CurrentRoom
    else Take Command
        Game->>Room: GetItem(name)
        Room-->>Game: Item
        Game->>Room: RemoveItem(item)
        Game->>Player: Inventory.AddItem(item)
    else Use Command
        Game->>Player: Inventory.GetItem(name)
        Player-->>Game: Item
        Game->>Item: OnUse(player, room)
    end
    
    Game->>Game: DisplayCurrentLocation()
    Game->>User: Show updated state
```

## Component Details

### App Component
```mermaid
graph LR
    A[App.xaml] -->|Defines| B[StartupUri]
    A -->|Maps to| C[App.xaml.cs]
    B -->|Points to| D[SplashScreen.xaml]
    C -->|Inherits| E[Application Class]
    
    style A fill:#e1f5ff
    style C fill:#bbdefb
```

### SplashScreen Component
```mermaid
graph TB
    A[SplashScreen.xaml]
    B[SplashScreen.xaml.cs]
    
    A -->|UI Definition| C[Window Properties]
    A -->|Displays| D[Image Element]
    A -->|Displays| E[TextBlock Version]
    
    B -->|Constructor| F[InitializeComponent]
    B -->|Constructor| G[SetVersionText]
    B -->|Constructor| H[Timer Setup]
    
    F --> I[XAML Parsing]
    G --> J[Assembly.GetExecutingAssembly]
    H --> K[DispatcherTimer]
    K -->|After 8s| L[Timer_Tick]
    L --> M[Create MainWindow]
    L --> N[Close SplashScreen]
    
    style A fill:#fff9c4
    style B fill:#fff59d
```

### MainWindow Component
```mermaid
graph LR
    A[MainWindow.xaml]
    B[MainWindow.xaml.cs]
    
    A -->|UI Definition| C[Window Layout]
    B -->|Constructor| D[InitializeComponent]
    
    D --> E[XAML Parsing]
    
    style A fill:#c8e6c9
    style B fill:#a5d6a7
```

## Technology Stack

```mermaid
graph TB
    subgraph "Framework"
        A[.NET 10]
        B[WPF - Windows Presentation Foundation]
    end
    
    subgraph "Language"
        C[C# 13]
        D[XAML]
    end
    
    subgraph "Features"
        E[ImplicitUsings]
        F[Nullable Reference Types]
        G[DispatcherTimer]
        H[Reflection]
        I[LINQ]
        J[Events & Delegates]
    end
    
    A --> B
    A --> C
    B --> D
    C --> E
    C --> F
    B --> G
    C --> H
    C --> I
    C --> J
    
    style A fill:#1976d2,color:#fff
    style B fill:#388e3c,color:#fff
    style C fill:#7b1fa2,color:#fff
    style D fill:#d32f2f,color:#fff
```

## Design Patterns

### Current Implementation

1. **Code-Behind Pattern**: Each XAML view has a corresponding code-behind file for UI logic
2. **Separation of Concerns**: Clear separation between UI (XAML) and logic (C#)
3. **Timer-Based Transitions**: Using DispatcherTimer for automatic screen transitions
4. **Resource Management**: Embedded resources for images and assets
5. **Version Management**: Centralized version information in project files
6. **Command Pattern**: TextGame uses command pattern for parsing and executing player actions
7. **Repository Pattern**: World class acts as a repository for Room objects
8. **Observer Pattern**: Item.Used event allows for flexible item behavior
9. **Dictionary-Based Lookup**: Fast command and direction resolution

### Future Considerations

The architecture is designed to support:
- MVVM (Model-View-ViewModel) pattern implementation
- Dependency injection for better testability
- Service layer for game engine functionality
- Event aggregation for loose coupling
- Repository pattern for data access

## Data Flow

```mermaid
graph LR
    A[User Input] -->|Event| B[UI Layer]
    B -->|Commands| C[Logic Layer]
    C -->|Game Actions| D[TextGame Engine]
    D -->|Updates| E[Game State]
    E -->|Notifications| B
    B -->|Renders| F[Display]
    
    style A fill:#ffccbc
    style B fill:#c8e6c9
    style C fill:#bbdefb
    style D fill:#ffcc80
    style E fill:#f0f4c3
    style F fill:#d1c4e9
```

## Build and Deployment

```mermaid
graph TB
    A[Source Code] -->|dotnet build| B[Compilation]
    B --> C[Assembly Generation]
    C --> D[Resource Embedding]
    D --> E[Executable .exe]
    D --> F[TextGame.dll]
    
    E --> G[Debug Output]
    E --> H[Release Output]
    
    G --> I[bin/Debug/net10.0-windows/]
    H --> J[bin/Release/net10.0-windows/]
    
    style A fill:#e3f2fd
    style E fill:#c8e6c9
    style F fill:#ffcc80
    style G fill:#fff9c4
    style H fill:#ffccbc
```

## Version Information Flow

```mermaid
graph LR
    A[Ubiquitous.csproj] -->|Version Property| B[Assembly Metadata]
    A -->|AssemblyVersion| B
    A -->|FileVersion| B
    
    C[TextGame.csproj] -->|Version Property| D[TextGame Metadata]
    
    B -->|Assembly.GetExecutingAssembly| E[SplashScreen.SetVersionText]
    E -->|Formats| F[Display Text]
    F -->|Binds to| G[VersionText.Text]
    
    style A fill:#fff3e0
    style B fill:#f3e5f5
    style C fill:#ffcc80
    style D fill:#ffb74d
    style E fill:#e1f5ff
    style F fill:#e8f5e9
    style G fill:#fce4ec
```

## Future Architecture Extensions

```mermaid
graph TB
    A[Current: WPF App + TextGame]
    
    B[Game Engine Core]
    C[Rendering System]
    D[Physics Engine]
    E[Input Management]
    F[Asset Management]
    G[Scene Management]
    H[Audio System]
    
    A -.->|v0.1.0+| B
    B --> C
    B --> D
    B --> E
    B --> F
    B --> G
    B --> H
    
    style A fill:#c8e6c9
    style B fill:#bbdefb
```

---

**Version:** 0.0.2  
**Last Updated:** December 20, 2025  
**Status:** Active Development
