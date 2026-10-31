<div align="center">

  <img src=".github/logo/Cheron-Logo-512x512.png" alt="Cheron">

  ![RELEASE](https://img.shields.io/badge/release-26.10-teal)&nbsp;
  ![STAGE](https://img.shields.io/badge/Development-red)&nbsp; <!-- Alpha = Red, Beta = Yellow, Stable = Green -->
  ![.NET](https://img.shields.io/badge/.NET-10.0-purple)
  ![MonoGame](https://shields.io)
  ![Platform](https://img.shields.io/badge/platform-windows-lightgrey)&nbsp;

</div>

***

<h6 align="center">

  [MANUAL](docs/man/README.md)&nbsp;&bull;&nbsp;[CHANGELOG](docs/CHANGELOG.md)&nbsp;&bull;&nbsp;[ROADMAP](docs/ROADMAP.md)&nbsp;&bull;&nbsp;[KNOWN ISSUES](docs/KNOWN-ISSUES.md)

</h6>

***

| CONTENTS                                    |
|---------------------------------------------|
| [About Cheron](#about-Cheron) |
| [How it works](#how-it-works)               |
| [Getting started](#getting-started)         |
| [Installing](#installing)                   |
| [Usage](#usage)                             |
| [Acknowledgements](#acknowledgements)       |
| [Related projects](#related-projects)       |
| [License](#license)                         |

***

# About Cheron

Cheron is:

1. **A game engine**  
The *idea* is to create a game engine that can be used to create and play a variety of different game types.

2. **An experiment**  
I'm using AI to build Cheron. Why? [Because](./making-of/README.md)




Cheron is a desktop card game built with C# and MonoGame. It currently includes
Klondike and FreeCell Solitaire, hosted by a small game engine that separates
the game lifecycle from individual game modules.

![License](https://img.shields.io/badge/License-Apache%202.0-blue)


## Features

- Modular game host and game-module lifecycle.
- Start screen that transitions into Solitaire.
- Klondike and FreeCell variants.
- Keyboard and pointer controls, help, and options.
- xUnit regression tests for Solitaire behavior.

## Requirements

- .NET 10 SDK.
- A desktop environment supported by MonoGame DesktopGL.

## Build and run

Clone the repository and enter its directory:

```shell
git clone https://github.com/APrettyCoolProgram/Cheron.git
cd Cheron
```

Build the solution:

```shell
dotnet build src/Cheron.slnx
```

Run the game:

```shell
dotnet run --project src/Cheron.csproj
```

## Tests

Run the Solitaire test suite:

```shell
dotnet test src/Solitaire.Tests/Solitaire.Tests.csproj
```

## Project layout

```text
src/
├── Engine/             Game host, module contract, and start screen
├── Games/Solitaire/    Shared Solitaire systems and Klondike/FreeCell variants
├── Solitaire.Tests/    xUnit tests
├── Program.cs          Application entry point
└── Cheron.csproj
```

## Documentation

- [Manual](docs/man/README.md)
- [Solitaire guide](docs/man/Solitaire.md)
- [API documentation](docs/api/README.md)
- [Development notes](docs/dev/README.md)

## License

Cheron is distributed under the [Apache 2.0 License](LICENSE).
