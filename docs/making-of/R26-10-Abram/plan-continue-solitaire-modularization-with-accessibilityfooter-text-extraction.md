# Continue Solitaire modularization with accessibility/footer text extraction
## Understanding
The user asked to continue refactoring. A safe next step is extracting remaining hard-coded accessibility and footer UI strings from `SolitaireGame.cs` into `SolitaireTextConstants` so UI text is centralized.
## Assumptions
- All string values must remain exactly unchanged.
- `SolitaireGame.cs` already imports `SolitaireTextConstants` statically, so call-site changes are minimal.
## Approach
Extend `SolitaireTextConstants.cs` with the accessibility header/instruction strings and footer status/instruction strings currently embedded in `SolitaireGame.cs`. Then replace literal usages in the options and footer drawing methods with named constants.
## Key Files
- src/Games/Solitaire/SolitaireTextConstants.cs - add extracted accessibility/footer text constants.
- src/Games/Solitaire/SolitaireGame.cs - replace string literals with constants.
## Risks & Open Questions
- Minimal; risk limited to missed literal replacement.
## Steps
1. Add accessibility/options and footer text constants to `SolitaireTextConstants.cs`.
2. Replace corresponding string literals in `SolitaireGame.cs` with extracted constants.
3. Build the solution and resolve any compile issues.