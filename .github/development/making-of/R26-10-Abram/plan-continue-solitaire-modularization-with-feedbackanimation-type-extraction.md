# Continue Solitaire modularization with feedback/animation type extraction
## Understanding
The user asked to continue modularization. The next low-risk reduction is extracting small nested type declarations (`FeedbackSound`, `CardPresentation`, and `CardTransition`) from `SolitaireGame.cs` into dedicated files without changing behavior.
## Assumptions
- These types are only used within the Solitaire namespace and can be `internal`.
- Extracting declarations only will not alter game logic or rendering behavior.
## Approach
Create new files for the feedback enum and animation record structs, preserving XML docs. Remove corresponding nested declarations from `SolitaireGame.cs` so it uses the extracted types directly.
## Key Files
- src/Games/Solitaire/SolitaireFeedbackSound.cs - extracted sound outcome enum.
- src/Games/Solitaire/SolitaireAnimationTypes.cs - extracted card presentation/transition records.
- src/Games/Solitaire/SolitaireGame.cs - remove nested type declarations.
## Risks & Open Questions
- Compile-time visibility issues are possible if any type remains private-only; full build validates.
## Steps
1. Create files for `FeedbackSound`, `CardPresentation`, and `CardTransition` extracted from `SolitaireGame.cs`.
2. Remove nested declarations for those types from `SolitaireGame.cs`.
3. Build the solution and fix any compile issues.