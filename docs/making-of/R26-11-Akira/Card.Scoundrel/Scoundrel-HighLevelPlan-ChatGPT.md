# Scoundrel Card Game – Architecture and Implementation Guide

## Objective

Create a detailed, step-by-step technical document that other AI models and developers can use to implement the **Scoundrel** card game in the Cheron game engine.

The document must serve as a practical implementation blueprint. It should provide enough architectural detail, source code references, implementation descriptions, and pseudocode that another AI model can implement the game without having to repeat the initial codebase investigation.

**Important:** This task is to analyze the existing codebase and produce an implementation guide. Do not implement the game itself unless explicitly requested.

---

## 1. Understand the Existing Cheron Game Engine Codebase

Review the existing source code located at:

`D:/Repositories/GitHub/APCP/Public/Cheron/src/`

Develop a thorough understanding of the following areas:

### 1.1 Game Engine Core (`Cheron.Engine`)

Examine the engine's architecture, including:

- Core engine classes and interfaces.
- Game lifecycle and initialization.
- Game update loop.
- Rendering and drawing systems.
- Input handling and event processing.
- Game state management.
- Scene, screen, or component management, where applicable.
- Asset and resource management.
- Game registration and cartridge loading.
- Any other engine systems that a card game would need to interact with.

Identify the relevant classes, interfaces, base classes, and extension points.

### 1.2 Base Card Game Logic (`Cheron.Games.Card`)

Examine the shared card game implementation, including:

- Card representations and properties.
- Card suits, ranks, and types.
- Deck construction and management.
- Card collections and containers.
- Card positioning and layout.
- Card rendering.
- Card interaction and movement.
- Shared card game functionality.

Determine which functionality can be reused directly by Scoundrel and which functionality requires game-specific implementation.

### 1.3 Existing Card Game Implementations

Examine representative implementations of existing card games, particularly:

`Cheron.Games.Card.Solitaire.*`

Develop an understanding of their:

- Directory and namespace organization.
- Class structures and inheritance.
- Game initialization.
- Game state management.
- Game-specific rules and logic.
- UI components and rendering.
- Input handling.
- Engine integration.
- Cartridge implementation and registration.

**Important:** Trace at least one representative card game through its complete lifecycle, from instantiation and initialization through gameplay, rendering, completion, and disposal.

Use actual implementations as architectural references rather than assuming how the engine works.

### 1.4 Codebase Investigation Requirements

Throughout the investigation:

- Reference actual source filenames, class names, namespaces, and method signatures.
- Identify existing functionality that can be reused.
- Identify functionality that must be implemented specifically for Scoundrel.
- Identify relevant design conventions, naming conventions, and coding patterns.
- Distinguish between existing functionality and proposed new functionality.

Do not invent existing classes, interfaces, methods, properties, or engine capabilities.

If a proposed component does not currently exist, explicitly identify it as a new addition.

---

## 2. Review the Scoundrel Game Rules

Study the Scoundrel rules document:

`D:/Repositories/GitHub/APCP/Public/Cheron/docs/making-of/R26-11-Akira/Card.Scoundrel/Scoundrel-Instructions.md`

Develop a complete understanding of the game.

Identify and document:

- Game description and objective.
- Deck composition and card removal.
- Card types and their properties.
- Game setup and initialization.
- Game phases and turn structure.
- Available player actions.
- Monster combat mechanics.
- Weapon mechanics and restrictions.
- Potion mechanics and restrictions.
- Health management.
- Win and loss conditions.
- Special rules and exceptions.
- Any other mechanics described in the rules document.

Translate the rules into an explicit set of game mechanics and state transitions.

Where a rule is ambiguous or incomplete, document the ambiguity rather than inventing a rule.

---

## 3. Scoundrel Game Architecture Overview

Design the Scoundrel architecture using the existing Cheron engine conventions.

Reference existing implementations in:

`Cheron.Games.Card.Solitaire.*`

Follow existing design patterns, naming conventions, inheritance structures, and architectural conventions wherever appropriate.

### 3.1 Namespace and Directory Structure

Define the proposed namespace:

`Cheron.Games.Card.Scoundrel`

Define the complete proposed directory and file structure under:

`Cheron/Games/Card/Scoundrel/`

Explain the purpose and responsibilities of each directory and file.

### 3.2 Game Classes and Methods

Identify all required classes, interfaces, structures, enumerations, and other architectural components.

For each component, document:

- Proposed name.
- Namespace.
- Purpose and responsibilities.
- Base class or implemented interfaces.
- Important properties and fields.
- Important methods and their responsibilities.
- Dependencies on other components.
- Relationship to existing engine classes.

Mirror existing card game implementations where appropriate.

Avoid unnecessary abstractions or introducing architectural patterns that are inconsistent with the existing engine.

### 3.3 Game State and Data Model

Describe the Scoundrel game state, including:

- Player health.
- Remaining deck.
- Current room.
- Available room cards.
- Equipped weapon.
- Weapon-related combat state.
- Potion usage state.
- Game status.
- Other state required by the rules.

Identify which data structures should be shared with the base card game implementation and which should be game-specific.

### 3.4 Separation of Responsibilities

Clearly distinguish between:

- Game rules and mechanics.
- Game state and data management.
- Player actions and interaction handling.
- UI and card rendering.
- Engine lifecycle and integration.

Where practical, game rules should be testable independently of rendering and user input.

---

## 4. Set Up the Scoundrel Framework

Describe the steps required to establish the initial Scoundrel project structure.

Include:

- Required directories.
- Required source files.
- Namespace declarations.
- Class declarations.
- Required base classes and interfaces.
- Project configuration changes, if necessary.
- References to existing engine components.

Provide an implementation order that respects dependencies between components.

Do not duplicate existing engine functionality unnecessarily.

---

## 5. Implement the Scoundrel Game

Provide a detailed implementation blueprint covering all aspects of the game.

### 5.1 Game Initialization

Describe:

- Game instantiation.
- Initial game state creation.
- Deck construction.
- Card removal.
- Deck shuffling.
- Initial player health.
- Initial game status.

### 5.2 Board and Game Setup

Describe:

- Card collections.
- Room initialization.
- Card positioning.
- Player status display.
- Weapon and potion representation.
- Board layout.

### 5.3 Game Loop and State Management

Explain how Scoundrel interacts with the existing engine update loop.

Describe:

- Game updates.
- State transitions.
- Player input processing.
- Turn progression.
- Room management.
- Game completion.

### 5.4 Card Interactions and Game Mechanics

Provide detailed implementation descriptions for:

- Selecting cards.
- Fighting monsters.
- Applying monster damage.
- Using weapons.
- Tracking weapon restrictions and combat history.
- Using potions.
- Applying healing.
- Managing rooms.
- Drawing and resolving cards.
- Handling invalid player actions.

Ensure that all mechanics accurately reflect the supplied Scoundrel rules.

### 5.5 UI and Rendering

Describe how the Scoundrel interface should be implemented using existing Cheron card game UI conventions.

Cover:

- Card rendering.
- Card positioning.
- Player health display.
- Weapon display.
- Potion display.
- Room display.
- Player interaction feedback.
- Game-over and victory screens.

Reference actual existing UI components and rendering implementations.

### 5.6 Pseudocode

Provide pseudocode for all proposed classes and their important methods.

Pseudocode should be detailed enough to establish:

- Method responsibilities.
- Input parameters.
- Return values.
- State modifications.
- Dependencies.
- Error and invalid-action handling.
- Interactions with other components.

Where appropriate, include example C# method signatures that follow existing Cheron conventions.

### 5.7 Implementation Dependencies

Provide an ordered implementation roadmap.

For each step, identify:

- Components that must already exist.
- Components being introduced.
- Dependencies on other implementation steps.
- Expected results.
- How to verify that the step is complete.

---

## 6. Integrate Scoundrel with the Cheron Game Engine

Describe all changes required to integrate Scoundrel into the existing engine.

Include:

- Required engine modifications.
- Game registration.
- Cartridge creation.
- Cartridge initialization.
- Asset and resource loading.
- Input and rendering integration.
- Compatibility with existing engine systems.
- Any required configuration or project file changes.

Create a new Scoundrel cartridge following the conventions of existing Cheron cartridges.

Identify every existing file that must be modified and every new file that must be created.

Avoid unnecessary modifications to shared engine components.

---

## 7. Create Unit and Integration Tests

Create a comprehensive testing plan for the Scoundrel implementation.

Tests should be located in the existing `Cheron.Tests` project:

`src/Cheron.Tests/CheronTests.csproj`

Cover the following areas:

### 7.1 Game Rules

- Deck composition.
- Card removal.
- Deck shuffling.
- Monster combat.
- Weapon restrictions.
- Potion usage.
- Health calculations.
- Win/loss conditions.

### 7.2 Player Actions

- Valid actions.
- Invalid actions.
- Illegal card selections.
- Invalid weapon usage.
- Invalid potion usage.
- Room progression.

### 7.3 State Transitions

- Initial game state.
- Room initialization.
- Room resolution.
- Turn progression.
- Game completion.
- Victory.
- Defeat.

### 7.4 Edge Cases

Test unusual but valid and invalid game situations, including:

- Player health reaching zero.
- Player health exceeding its maximum, if applicable under the rules.
- Empty deck.
- Final room resolution.
- Weapon restrictions involving previously defeated monsters.
- Attempting actions after game completion.

### 7.5 Testing Requirements

- Follow existing `Cheron.Tests` conventions and testing frameworks.
- Use deterministic decks or controlled random seeds wherever possible.
- Avoid relying on random card distributions for rule validation.
- Test game logic independently of rendering wherever practical.
- Include integration tests for engine interaction where appropriate.

Provide example test cases and pseudocode.

---

## 8. Final Documentation Requirements

The completed document must be a comprehensive, technically detailed implementation guide.

Include:

1. Architecture overview.
2. Codebase investigation findings.
3. Scoundrel rules breakdown.
4. Proposed directory and file structure.
5. Class and component descriptions.
6. Method responsibilities and signatures.
7. Game state and data models.
8. Implementation sequence.
9. Pseudocode.
10. Engine integration instructions.
11. Testing strategy.
12. Outstanding questions, assumptions, and unresolved issues.

### Accuracy and Quality Requirements

- Base all architectural recommendations on actual Cheron source code.
- Reference relevant source files and existing implementations throughout the document.
- Distinguish clearly between existing code and proposed new code.
- Follow established Cheron coding conventions.
- Avoid unnecessary abstractions and duplication.
- Do not invent functionality or silently assume undocumented behavior.
- Ensure the implementation plan accounts for the complete game lifecycle.
- Ensure every Scoundrel rule is accounted for in the proposed implementation.
- Prefer concrete implementation guidance over generic architectural descriptions.