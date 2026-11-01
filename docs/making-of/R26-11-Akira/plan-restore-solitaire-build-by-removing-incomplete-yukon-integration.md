# Restore Solitaire Build by Removing Incomplete Yukon Integration
## Understanding
The solution fails to build because partially added Yukon variant code does not match the current Solitaire architecture and references a non-existent `SolitaireVariant.Yukon` enum value. The goal is to get the workspace compiling again with minimal, safe changes.
## Assumptions
- Yukon is not currently a supported production variant because `SolitaireVariant` only includes Klondike, FreeCell, and Spider.
- The Yukon files are incomplete scaffolding and are not referenced by the active game flow.
- Minimal-risk fix is to remove incomplete Yukon-specific compile units and clean variant extension/start-screen references.
## Approach
I will remove the broken Yukon implementation files under `src/Games/Solitaire/Yukon` and clean accidental Yukon references in shared files that are causing enum and duplicate-method errors. I will also fix `src/Games/Solitaire/StartScreenGame.cs` to stop referencing Yukon if that file remains in the project.

After edits, I will run a full build and resolve any residual compile issues directly related to these changes.
## Key Files
- src/Games/Solitaire/SolitaireVariantExtensions.cs - contains duplicate method and invalid Yukon enum references.
- src/Games/Solitaire/StartScreenGame.cs - includes unsupported Yukon in variant list.
- src/Games/Solitaire/Yukon/*.cs - incomplete incompatible implementation causing most compile failures.
## Risks & Open Questions
- If Yukon support was intentionally in-progress, removing these files defers that feature.
- If any external code expects the removed Yukon files, additional references may surface on rebuild.
## Steps
1. Clean invalid Yukon references in shared variant extension helpers.
2. Remove incomplete Yukon implementation files from the Solitaire project.
3. Fix leftover unsupported Yukon references in ancillary Solitaire files.
4. Rebuild solution to verify compile health.
5. Address any residual build errors introduced or uncovered by the cleanup.