# 🎯 Add Yukon Variant Back to Solitaire New Game Picker

## Understanding
The Yukon variant is missing from the in-game New Game chooser. The user expects Yukon to be selectable and playable like other Solitaire variants.
## Assumptions
- Yukon should be a supported fourth `SolitaireVariant` in the current architecture.
- The prior Yukon files were removed because they used an incompatible design; re-adding Yukon must follow the existing strategy-based variant behavior model.
- Minimal safe implementation is to add Yukon through `ISolitaireVariantBehavior` and existing shared orchestrator hooks, plus UI prompt wiring.
## Approach
I will reintroduce Yukon as a first-class variant by extending the enum, prompt cycling helpers, prompt layout/render/input, and variant registration in `SolitaireGame`. I will add new Yukon gameplay helpers under `src/Games/Solitaire/Yukon` that conform to the current architecture (static gameplay helper + behavior adapter), including Yukon deal rules and tableau move legality.

I will then update variant-specific labels/help content and adjust tests to cover Yukon prompt selection and Yukon deal layout. Finally I will run build and relevant test project validation.
## Key Files
- src/Games/Solitaire/SolitaireVariant.cs - add Yukon enum member.
- src/Games/Solitaire/SolitaireVariantExtensions.cs - include Yukon in prompt navigation and helper methods.
- src/Games/Solitaire/SolitaireGame.cs - register Yukon behavior and help content.
- src/Games/Solitaire/SolitaireLayout.cs - add Yukon prompt option bounds.
- src/Games/Solitaire/SolitaireRenderer.cs - render Yukon option and selected state in prompt; toolbar variant label.
- src/Games/Solitaire/SolitaireInteractionController.cs - pointer selection for Yukon option.
- src/Games/Solitaire/SolitaireTextConstants.cs - Yukon labels/help text.
- src/Games/Solitaire/Yukon/*.cs - Yukon behavior and gameplay logic.
- src/Cheron.Tests/SolitaireVariantPromptTests.cs - add/adjust prompt and deal tests for Yukon.
- src/Cheron.Tests/SolitaireVariantHelpersTests.cs - add Yukon helper/deal tests.
## Risks & Open Questions
- Yukon move semantics vary by ruleset; implementation will use common Yukon expectations (no stock/freecells, any face-up group may move if lead card fits destination, any card to empty column).
- Prompt layout must remain readable when expanded from 3 to 4 options.

**Progress**: 100% [██████████]

**Last Updated**: 2026-11-01 18:16:49

## 📝 Plan Steps
- ✅ **Extend core variant contracts and cycling helpers for Yukon.**
- ✅ **Implement Yukon gameplay/behavior classes compatible with the current strategy architecture.**
- ✅ **Wire Yukon into game orchestration, prompt layout, rendering, interaction, and text resources.**
- ✅ **Update and add tests for Yukon prompt selection and Yukon deal/help behavior.**
- ✅ **Build and run Solitaire-focused tests to validate Yukon integration.**

