# 🎯 Implement Spider solitaire variant

## Understanding
Add a new Spider variant to the existing Solitaire module so it can be selected from the new-game prompt and played through the same shared game orchestration. The implementation should fit current architecture (variant enum + behavior strategies) and preserve existing variants.

## Assumptions
- Spider should be integrated as a third `SolitaireVariant` alongside Klondike and FreeCell.
- Initial implementation should be functional and testable within current shared renderer/interaction system, without rewriting the whole Solitaire engine.
- Existing tests must continue to pass and new Spider-specific tests should be added for coverage.

## Approach
I will add a `Spider` variant behavior under a new `Games/Solitaire/Spider` folder, with dedicated deal and tableau move rules. Then I will wire Spider into variant selection, prompt rendering/hit testing, and variant-cycling keyboard behavior. Because current prompt/layout/text are hard-coded for two variants, I will extend those points to support three selectable options while preserving the existing UX style.

I will also adjust variant-specific labels/help logic to include Spider and add tests for prompt selection and Spider deal shape/rules. Finally, I will run build and tests to validate no regressions.

## Key Files
- src/Games/Solitaire/SolitaireVariant.cs - add Spider enum value.
- src/Games/Solitaire/SolitaireVariantExtensions.cs - replace two-way toggle with cycling helpers.
- src/Games/Solitaire/SolitaireGame.cs - register Spider behavior and variant-specific help handling.
- src/Games/Solitaire/SolitaireRenderer.cs - render Spider option in prompt and variant label in toolbar.
- src/Games/Solitaire/SolitaireInteractionController.cs - pointer/keyboard prompt selection behavior for three variants.
- src/Games/Solitaire/SolitaireLayout.cs - add Spider prompt bounds.
- src/Games/Solitaire/SolitaireTextConstants.cs - add Spider labels/help lines.
- src/Games/Solitaire/Spider/* - new Spider behavior/rules/deal helpers.
- src/Cheron.Tests/SolitaireVariantPromptTests.cs - update/add tests for three-variant prompt behavior.
- src/Cheron.Tests/SolitaireVariantHelpersTests.cs - add Spider helper/deal tests.

## Risks & Open Questions
- Current shared foundation/win orchestration is Klondike/FreeCell-centric; Spider is adapted within this framework and may not cover every classic Spider rule yet.
- Prompt geometry changes must keep click targets non-overlapping and readable.

**Progress**: 100% [██████████]

**Last Updated**: 2026-11-01 16:49:13

## 📝 Plan Steps
- ✅ **Add Spider variant model and behavior registration**
- ✅ **Implement Spider deal and tableau move rules helpers**
- ✅ **Extend prompt layout/text/constants for Spider option**
- ✅ **Update renderer and interaction logic for three-variant selection**
- ✅ **Update variant help/label logic and test hooks**
- ✅ **Add and update tests for Spider selection and deal behavior**
- ✅ **Build and run tests to validate the implementation**

