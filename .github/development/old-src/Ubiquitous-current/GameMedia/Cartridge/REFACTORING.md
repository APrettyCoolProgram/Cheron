# Cartridge Project Refactoring

## Overview
Successfully moved all cartridge-related logic from the TextGame library to a new dedicated Cartridge class library project.

## Changes Made

### New Structure Created

#### 1. GameMedia Folder
Created new root folder `GameMedia` to house game-related assets.

#### 2. Cartridge Project (`GameMedia/Cartridge/`)
New .NET 10 class library project containing:

**Files:**
- `GameCartridge.cs` - Data models for cartridge structure
  - `GameCartridge` - Top-level game data
  - `RoomData` - Room definitions
  - `ItemData` - Item definitions
  - `ExitData` - Room connections
  - `CustomActionData` - Item actions
  
- `CartridgeLoader.cs` - JSON serialization/deserialization utilities
  - `LoadFromJson()` - Load from file path
  - `LoadFromJsonContent()` - Load from JSON string
  - `LoadCartridgeFromJson()` - Return GameCartridge object
  - `LoadCartridgeFromJsonContent()` - Return GameCartridge object from string

- `Games/TheAbandonedMansion.json` - Sample game cartridge
- `Games/README.md` - Documentation for cartridge format

**Project Configuration:**
- Target: .NET 10
- Version: 0.0.2
- Embedded resources: `Games\*.json`

### TextGame Project Updates

#### New Files:
- `CartridgeBuilder.cs` - Converts GameCartridge data to TextGame objects
  - `BuildGameFromCartridge()` - Constructs GameEngine from cartridge

#### Modified Files:
- `GameBuilder.cs` - Updated `CreateSampleGame()` to use embedded cartridge
- `TextGame.csproj` - Added project reference to Cartridge project

#### Removed Files:
- `GameCartridge.cs` - Moved to Cartridge project
- `CartridgeLoader.cs` - Moved to Cartridge project  
- `Cartridge/` folder - Moved to GameMedia/Cartridge/Games/

### Solution Updates
- Added Cartridge project to Ubiquitous.sln

## Architecture

```
???????????????????
?   Ubiquitous    ? (Avalonia GUI)
?                 ?
?  MainWindow.cs  ????
???????????????????  ?
                     ?references
                     ?
???????????????????????????????
?         TextGame            ? (Game Engine)
?                             ?
?  GameEngine.cs              ?
?  GameBuilder.cs             ?
?  CartridgeBuilder.cs        ?????????
?  Room.cs, Item.cs, etc.     ?       ?
???????????????????????????????       ?references
                                      ?
                          ?????????????????????
                          ?     Cartridge     ? (Data Layer)
                          ?                   ?
                          ?  GameCartridge.cs ?
                          ?  CartridgeLoader  ?
                          ?  Games/*.json     ?
                          ?????????????????????
```

## Benefits

1. **Separation of Concerns**
   - Data models isolated in Cartridge project
   - Game logic stays in TextGame project
   - Clear boundaries between layers

2. **Reusability**
   - Cartridge library can be used by other game engines
   - JSON format is engine-agnostic

3. **Maintainability**
   - Easier to modify cartridge format without affecting game engine
   - Clear project structure

4. **Extensibility**
   - Easy to add new game formats or loaders
   - Can add validation, schema checking, etc. to Cartridge project

5. **Testing**
   - Cartridge data can be tested independently
   - Mock cartridges easy to create for testing

## Usage

### Loading a Game Cartridge

```csharp
// From file
var cartridge = Cartridge.CartridgeLoader.LoadCartridgeFromJson("path/to/game.json");
var game = TextGame.CartridgeBuilder.BuildGameFromCartridge(cartridge);

// From embedded resource (as used in CreateSampleGame)
var assembly = typeof(Cartridge.GameCartridge).Assembly;
var stream = assembly.GetManifestResourceStream("Cartridge.Games.TheAbandonedMansion.json");
using var reader = new StreamReader(stream);
var jsonContent = reader.ReadToEnd();
var cartridge = Cartridge.CartridgeLoader.LoadCartridgeFromJsonContent(jsonContent);
var game = TextGame.CartridgeBuilder.BuildGameFromCartridge(cartridge);
```

### Creating a New Game

1. Create JSON file in `GameMedia/Cartridge/Games/`
2. File automatically embeds as resource
3. Load using CartridgeLoader
4. Build game using CartridgeBuilder

## Project Dependencies

```
Ubiquitous
  ??? TextGame
        ??? Cartridge
```

All three projects target .NET 10 and have version 0.0.2.

## Build Status
? All projects build successfully
? Cartridge project compiles independently
? TextGame properly references Cartridge
? Ubiquitous GUI works with new structure
