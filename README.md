# Cheron

A desktop card game project currently focused on Solitaire, implemented with C# on .NET 10 and MonoGame DesktopGL. Cheron is intended for contributors and players who want a lightweight, code-first game module with clear architecture, variant support, and test coverage.

## 1. Project Header & Description

- **Project Title:** Cheron
- **Short Description:** Cheron is a .NET 10 desktop game project with a modular game host and a Solitaire implementation. It currently includes Klondike and FreeCell variants, with shared orchestration and variant-specific logic separated into dedicated namespaces.
- **Badges:**
  - ![License](https://img.shields.io/badge/License-Apache%202.0-blue)
  - ![Framework](https://img.shields.io/badge/.NET-10.0-purple)

## 2. Key Features

- **Dual Solitaire Variants:** Supports both **Klondike** and **FreeCell** gameplay.
- **Variant Picker Flow:** New game flow includes an in-app prompt to choose the variant.
- **Shared + Variant Architecture:** Common orchestration lives under `Cheron.Games.Solitaire`, with variant-specific behavior in `Cheron.Games.Solitaire.Klondike` and `Cheron.Games.Solitaire.FreeCell`.
- **Input and UX Coverage:** Keyboard and pointer interactions are implemented and tested, including prompt flow behavior.
- **Automated Tests:** xUnit-based test project validates deal setup, prompt behavior, and variant helper logic.

## 3. Tech Stack & Prerequisites

- **Language:** C#
- **Runtime/SDK:** .NET 10 (`net10.0`)
- **Rendering/Game Framework:** MonoGame DesktopGL (`MonoGame.Framework.DesktopGL`)
- **Content Pipeline:** `MonoGame.Content.Builder.Task`
- **Testing:** xUnit + Microsoft.NET.Test.Sdk

Prerequisites:
- .NET 10 SDK
- A desktop environment supported by MonoGame DesktopGL (current project output type is desktop `WinExe`)

## 4. Getting Started / Installation

```bash
git clone https://github.com/APrettyCoolProgram/Cheron.git
cd Cheron
```

Build the solution:

```bash
dotnet build src/Cheron.slnx
```

Run the app:

```bash
dotnet run --project src/Cheron.csproj
```

## 5. Usage Examples

Run the game directly:

```bash
dotnet run --project src/Cheron.csproj
```

Run tests for the Solitaire test project:

```bash
dotnet test src/Solitaire.Tests/Solitaire.Tests.csproj
```

Useful docs:
- Manual index: [docs/man/README.md](docs/man/README.md)
- Solitaire manual: [docs/man/Solitaire.md](docs/man/Solitaire.md)
- API docs index: [docs/api/README.md](docs/api/README.md)

## 6. Architecture & File Structure

```text
Cheron/
├─ src/
│  ├─ Program.cs
│  ├─ Engine/
│  │  └─ GameHost.cs
│  ├─ Games/
│  │  └─ Solitaire/
│  │     ├─ SolitaireGame.cs
│  │     ├─ Klondike/
│  │     └─ FreeCell/
│  └─ Solitaire.Tests/
├─ docs/
│  ├─ man/
│  ├─ api/
│  └─ dev/
└─ README.md
```

## 7. Development & Testing

Build:

```bash
dotnet build src/Cheron.slnx
```

Run all tests in the test project:

```bash
dotnet test src/Solitaire.Tests/Solitaire.Tests.csproj
```

Additional references:
- Development docs: [docs/dev/README.md](docs/dev/README.md)
- Changelog: [docs/CHANGELOG.md](docs/CHANGELOG.md)
- Roadmap: [docs/ROADMAP.md](docs/ROADMAP.md)
- Known issues: [docs/KNOWN-ISSUES.md](docs/KNOWN-ISSUES.md)

## 8. License & Contributions

- **License:** Apache 2.0 ([LICENSE](LICENSE))
- **Contributions:** Use the repository issue tracker and documentation in `docs/` for contribution context. A dedicated `CONTRIBUTING.md` file is not currently present.
