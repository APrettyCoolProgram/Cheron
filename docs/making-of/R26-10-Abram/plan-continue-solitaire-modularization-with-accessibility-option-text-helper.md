# Continue Solitaire modularization with accessibility option text helper
## Understanding
The user asked to continue the next extraction pass. A safe next step is extracting accessibility option label formatting and option count constants out of `SolitaireGame.cs` to reduce inline string logic and magic numbers.
## Assumptions
- Existing labels and behavior must remain exactly unchanged.
- Keeping helper methods internal static in the same namespace preserves simple usage.
## Approach
Create a new helper file that centralizes accessibility option count/index constants and label formatting for the options modal rows. Update `SolitaireGame.cs` to use these members in keyboard navigation and options rendering loops, removing inline label switch logic and hard-coded `4`/`3` values where they map to option count behavior.
## Key Files
- src/Games/Solitaire/SolitaireAccessibilityOptions.cs - option count constants and label formatting helper.
- src/Games/Solitaire/SolitaireGame.cs - consume helper for navigation math, loops, and option label text.
## Risks & Open Questions
- Minimal risk; compile verification will catch any missed references.
## Steps
1. Create `SolitaireAccessibilityOptions.cs` with option-count constants and option-label formatting helper.
2. Update `SolitaireGame.cs` to use extracted option constants/helper and remove inline label switch logic.
3. Build the solution and resolve any compile issues.