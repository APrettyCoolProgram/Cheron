# Copilot Instructions

## General Guidelines

- For documentation and design work, *generally* use a local LLM.
- For implementation tasks, focus on using GitHub Copilot.

## Project Guidelines

- For Cheron, internal game module testability is enabled via `InternalsVisibleTo("Solitaire.Tests")` in Cheron.csproj plus `Test*`-prefixed internal methods/properties added directly to game classes (e.g., SolitaireGame, MemoryGame) to expose state and simulate input for xUnit tests, rather than using reflection or changing production visibility.