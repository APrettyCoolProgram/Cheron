# Implement cartridge-based start screen
## Understanding
The user wants implementation to begin for a new startup screen before Solitaire launches. The screen should use procedural visuals, show a retro console with cartridges, support dragging cartridges (including one labeled Solitaire) into a cartridge slot, and start Solitaire only after pressing power with an approximately 1-second delay.

## Assumptions
- The startup screen should be the initial module launched from `Program.cs`.
- A single startup module can own the transition to `SolitaireGame` without needing immediate `GameHost` redesign.
- Reusing the existing bitmap text utility is acceptable for drawing cartridge labels in this first pass.

## Approach
Create a new `StartScreenGame` module implementing `IGameModule` in a new startup namespace. It will render the warm brown background, console, power button, and multiple cartridges using procedural rectangles and text labels. It will manage pointer drag/drop with snap-to-slot behavior and gate transition on power press + inserted Solitaire cartridge + 1 second delay.

After transition, the module will instantiate and delegate lifecycle/update/draw calls to an internal `SolitaireGame` instance, preserving compatibility with existing host flow. Finally, wire `Program.cs` to launch the start screen module and validate with build and tests.

## Key Files
- src/Games/Startup/StartScreenGame.cs - new startup module, rendering, interaction, and transition delegation.
- src/Program.cs - switch initial module from direct Solitaire to startup screen.

## Risks & Open Questions
- Window title remains host-initialized; post-transition title updates are deferred unless needed.
- Start screen constants are currently aligned to Solitaire’s existing logical size.

## Steps
1. Create startup module skeleton
2. Implement start screen state and cartridge data
3. Implement drag-drop and snap interaction
4. Implement power-button transition to Solitaire with delay
5. Wire Program entrypoint to startup module
6. Validate with build and relevant tests