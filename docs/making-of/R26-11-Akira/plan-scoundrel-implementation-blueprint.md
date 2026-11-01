# Scoundrel Implementation Blueprint
## Understanding
Produce a detailed, source-grounded technical document for implementing Scoundrel in Cheron without changing gameplay code. The document must map the existing engine and card-game lifecycle, translate the supplied rules, propose concrete additions, integration work, and tests.

## Assumptions
- The deliverable is a new or updated Markdown document in the workspace; no Scoundrel implementation files will be created.
- Existing source and tests are authoritative for engine conventions; ambiguities in the Scoundrel rules will be recorded rather than resolved speculatively.
- The supplied Copilot instructions require internal test seams to use `InternalsVisibleTo("Solitaire.Tests")` and `Test*` members where applicable.

## Approach
Inspect the solution and relevant project layouts, then trace a representative Solitaire cartridge from registration through the engine loop, input, rendering, completion, and disposal. Read the Scoundrel rules and compare each mechanic against reusable card abstractions and test conventions. Create a standalone Markdown blueprint in the Scoundrel documentation area with exact references, proposed files/classes, state transitions, pseudocode, integration instructions, and a deterministic test matrix.

## Key Files
- `src/Cheron.Engine/` - engine lifecycle, rendering, input, cartridge registration, and asset conventions.
- `src/Cheron/Games/Card/` - shared card models, deck/container behavior, and rendering utilities.
- `src/Cheron/Games/Card/Solitaire/` - representative card-game and cartridge lifecycle.
- `docs/making-of/R26-11-Akira/Card.Scoundrel/Scoundrel-Instructions.md` - authoritative game rules.
- `src/Cheron.Tests/` - test framework and existing game-test conventions.

## Risks & Open Questions
- The provided test-instructions mention `Solitaire.Tests`, which may differ from the actual `Cheron.Tests` project specified in the request; document and flag any mismatch found.
- Rules may omit decisions needed for code behavior; list those precisely instead of inventing outcomes.

## Steps
1. Inspect solution and project structure to locate engine, shared-card, Solitaire, test, and rules artifacts.
2. Trace the engine and representative Solitaire game lifecycle through initialization, updates, input, rendering, completion, and disposal.
3. Analyze reusable card abstractions and existing testing conventions relevant to Scoundrel.
4. Translate the Scoundrel rules into mechanics, state transitions, and documented ambiguities.
5. Design the Scoundrel architecture, file layout, lifecycle integration, and implementation sequence using observed conventions.
6. Author the source-grounded Scoundrel implementation blueprint in the documentation area.
7. Validate document references and workspace changes, then finalize the plan.