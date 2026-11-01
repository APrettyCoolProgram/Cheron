# 🎯 Make Scoundrel Playable End-to-End

## Understanding
The Scoundrel cartridge launches, but gameplay is non-functional. The user wants all core areas fixed: input/actions, room/deck progression, health/weapon/potion mechanics, and win/loss outcomes.

## Assumptions
- The immediate goal is a working playable baseline inside the existing MonoGame module, not final polish.
- Existing standalone Scoundrel helper files can be corrected and reused where practical.
- Keyboard-first controls are acceptable for now as long as they are clearly rendered and game rules are enforced.

## Approach
I will implement deterministic rule-correct core gameplay in `ScoundrelGame` while fixing helper types (`ScoundrelCard`, `ScoundrelRules`, deck handling assumptions) to match the documented Scoundrel rules. The game will support selecting one of four room slots and actions to fight barehanded/with weapon, equip weapon, drink potion, avoid room (with non-consecutive restriction), and auto room progression with carry-over card semantics.

I will keep rendering/input in `ScoundrelGame` for minimal invasive changes and avoid introducing new engine abstractions. Then I will validate by building and running targeted tests/build checks.

## Key Files
- `src/Games/Card/Scoundrel/ScoundrelGame.cs` - main lifecycle, input handling, state transitions, rendering feedback.
- `src/Games/Card/Scoundrel/ScoundrelCard.cs` - card classification currently incorrect; needs suit-based typing.
- `src/Games/Card/Scoundrel/ScoundrelRules.cs` - rule methods need correct Scoundrel logic.
- `src/Games/Card/Scoundrel/ScoundrelDeckBuilder.cs` - deck composition/order behavior must align with gameplay flow.

## Risks & Open Questions
- Final-room behavior is ambiguous in the source rules; I will implement a practical fallback: when fewer than 4 cards remain, resolve all currently available room cards to finish.
- Existing Scoundrel helper classes may be partially unused; changes will prioritize compile-safe behavior and active gameplay path.

**Progress**: 100% [██████████]

**Last Updated**: 2026-11-01 23:45:11

## 📝 Plan Steps
- ✅ **Inspect current Scoundrel files to map incorrect rule/model implementations.**
- ✅ **Correct card typing and rules helpers to reflect suit-based Scoundrel mechanics.**
- ✅ **Rework Scoundrel game state flow in `ScoundrelGame` for room creation, resolve-three carry-over, and avoid-room restrictions.**
- ✅ **Implement actionable input handling and rule-driven card resolution (fight/equip/potion) with win/loss updates.**
- ✅ **Update rendering/status text to expose controls, selected card, invalid actions, and terminal states.**
- ✅ **Build and verify compile/runtime readiness, then report what works and remaining constraints.**

