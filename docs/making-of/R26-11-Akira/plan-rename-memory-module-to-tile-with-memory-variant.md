# 🎯 Rename Memory module to Tile with Memory variant

## Understanding
The user wants the current standalone Memory game type restructured so the game type is Tile, and Memory becomes a variant of Tile. They specifically chose a full rename of the Memory module to a Tile module.

## Assumptions
- The Tile game type currently has a single variant (Memory).
- Existing gameplay behavior should stay unchanged; this is a structural and naming refactor.
- Public behavior visible from the start screen should now present Tile instead of Memory.

## Approach
I will rename the Memory game module artifacts (namespace, class/type names, and folder/file names) from Memory* to Tile* while introducing an explicit `TileVariant` concept with `Memory` as the current variant. Then I will update all references in engine wiring and tests to use the new Tile naming and ensure build/tests still pass.

This will touch core game files under `src/Games/Memory`, startup wiring in `src/Engine/StartScreenGame.cs`, and test files under `src/Cheron.Tests`. I will keep logic equivalent and minimize behavioral changes.

## Key Files
- src/Engine/StartScreenGame.cs - start-screen cartridge label/type and game module launch mapping.
- src/Games/Memory/MemoryGame.cs - primary game module class to become Tile game with variant support.
- src/Games/Memory/*.cs - supporting constants/layout/artwork/tile types to rename to Tile nomenclature.
- src/Cheron.Tests/*.cs - test imports and type references to align with renamed module.

## Risks & Open Questions
- Full file/folder renames can miss references if any path-based assumptions exist.
- Introducing variant naming should not accidentally change UI strings beyond intended Tile/Memory taxonomy.

**Progress**: 100% [██████████]

**Last Updated**: 2026-11-01 16:31:27

## 📝 Plan Steps
- ✅ **Baseline Memory module and startup/test references**
- ✅ **Rename game module types/namespaces from Memory to Tile**
- ✅ **Introduce Tile variant model with Memory as default variant**
- ✅ **Update engine/start-screen wiring to Tile game type**
- ✅ **Update tests and file/folder names to match Tile structure**
- ✅ **Build and run relevant tests to validate refactor**

