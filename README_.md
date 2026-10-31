## 1. Project Header & Description

# Cheron

Cheron is a desktop game project built with C# on .NET 10 and MonoGame DesktopGL. It is currently focused on Solitaire and is structured for contributors who want a modular codebase with clear separation between engine hosting and game module logic. The repository includes a startup flow, in-game UX systems, and automated regression tests.

![License](https://img.shields.io/badge/License-Apache%202.0-blue)
![Framework](https://img.shields.io/badge/.NET-10.0-purple)

## 2. Key Features

- **Modular Game Host:** Uses GameHost and IGameModule so startup and gameplay modules remain decoupled.
- **Startup Console Flow:** Supports cartridge drag-and-insert behavior, power gating, and transition into Solitaire.
- **Dual Solitaire Variants:** Includes Klondike and FreeCell with shared orchestration and variant-specific behaviors.
- **Rich UX and Input Paths:** Includes prompt flow, options, help overlay, smoke-pass overlay, and keyboard and pointer controls.
- **Sound State Controls:** Supports SOUND ON and SOUND OFF toggling through both keyboard and pointer interactions.
- **Automated Regression Coverage:** xUnit tests validate variant setup, shortcut routing, modal precedence, and control hit-testing.

## 3. Tech Stack & Prerequisites

- **Language:** C#
- **Runtime / Target Framework:** .NET 10 (net10.0)
- **Game Framework:** MonoGame DesktopGL (MonoGame.Framework.DesktopGL)
- **Content Pipeline:** MonoGame.Content.Builder.Task
- **Testing:** xUnit + Microsoft.NET.Test.Sdk

Prerequisites:
- .NET 10 SDK
- Desktop environment that supports MonoGame DesktopGL

## 4. Getting Started / Installation

~~~bash
git clone https://github.com/APrettyCoolProgram/Cheron.git
cd Cheron
~~~

Build the solution:

~~~bash
dotnet build src/Cheron.slnx
~~~

Run the application:

~~~bash
dotnet run --project src/Cheron.csproj
~~~

## 5. Usage Examples

Run the game:

~~~bash
dotnet run --project src/Cheron.csproj
~~~

Run the Solitaire test suite:

~~~bash
dotnet test src/Solitaire.Tests/Solitaire.Tests.csproj
~~~

Documentation entry points:
- Manual index: [docs/man/README.md](docs/man/README.md)
- Solitaire manual: [docs/man/Solitaire.md](docs/man/Solitaire.md)
- API docs index: [docs/api/README.md](docs/api/README.md)

## 6. Architecture & File Structure

~~~text
Cheron/
├─ src/
│  ├─ Program.cs
│  ├─ Engine/
│  │  ├─ GameHost.cs
│  │  ├─ GameViewport.cs
│  │  ├─ IGameModule.cs
│  │  └─ StartScreenGame.cs
│  ├─ Games/
│  │  └─ Solitaire/
│  │     ├─ SolitaireGame.cs
│  │     ├─ SolitaireInteractionController.cs
│  │     ├─ SolitaireRenderer.cs
│  │     ├─ Klondike/
│  │     └─ FreeCell/
│  └─ Solitaire.Tests/
│     └─ SolitaireVariantPromptTests.cs
├─ docs/
│  ├─ api/
│  ├─ dev/
│  └─ man/
├─ LICENSE
└─ README.md
~~~

## 7. Development & Testing

Build:

~~~bash
dotnet build src/Cheron.slnx
~~~

Run tests:

~~~bash
dotnet test src/Solitaire.Tests/Solitaire.Tests.csproj
~~~

Optional formatting check:

~~~bash
dotnet format whitespace src/Cheron.slnx --verify-no-changes
~~~

Additional references:
- Development docs: [docs/dev/README.md](docs/dev/README.md)
- Changelog: [docs/CHANGELOG.md](docs/CHANGELOG.md)
- Roadmap: [docs/ROADMAP.md](docs/ROADMAP.md)
- Known issues: [docs/KNOWN-ISSUES.md](docs/KNOWN-ISSUES.md)

## 8. License & Contributions

- **License:** Apache 2.0 ([LICENSE](LICENSE))
- **Contributions:** Use repository issues and the docs folder for contribution context. A dedicated CONTRIBUTING.md file is not currently present.
