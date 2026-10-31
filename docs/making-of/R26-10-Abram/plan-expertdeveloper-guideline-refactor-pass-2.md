# 🎯 ExpertDeveloper Guideline Refactor - Pass 2

## Understanding
Apply another full-project refactor pass on `src` to further align with `.github/agents/AGENT-CSharp-ExpertDeveloper.md`, with low-risk readability/maintainability improvements and safe bug fixes only.

## Assumptions
- Prior pass already handled visibility and several nullability fixes.
- The highest remaining impact is in Solitaire UI string consistency and repeated magic values.
- No test project exists, so validation will rely on build success.

## Approach
Focus on high-confidence, behavior-preserving changes in engine and Solitaire files: remove null-forgiving fallback in selection transfer, extract repeated gameplay/UI literals into named constants, and standardize repeated text/labels through centralized constants to reduce duplication and improve maintainability. Keep all gameplay behavior unchanged.

## Key Files
- src/Games/Solitaire/SolitaireGame.cs - core constants and shared labels.
- src/Games/Solitaire/SolitaireGame.Controls.cs - hint and toolbar text duplication.
- src/Games/Solitaire/SolitaireGame.Accessibility.cs - repeated settings/footer labels.
- src/Games/Solitaire/SolitaireGame.Keyboard.cs - repeated rank/label text building.
- src/Engine/GameViewport.cs - small readability/guard refinements.

## Risks & Open Questions
- Some repeated strings are intentionally distinct UX phrasing; over-consolidation could reduce clarity.
- Broad text edits across partial files can introduce typos; build validation is required.

**Progress**: 100% [██████████]

**Last Updated**: 2026-10-29 00:36:48

## 📝 Plan Steps
- ✅ **Inspect remaining target files and confirm exact refactor points.**
- ✅ **Refactor shared constants and nullability-safe flow in core Solitaire file.**
- ✅ **Refactor duplicated label/hint text usage across Solitaire partial files.**
- ✅ **Apply small readability refinements in engine file(s) where safe.**
- ✅ **Build the solution and resolve any compilation issues.**
- ✅ **Summarize completed refactors and residual recommendations.**

