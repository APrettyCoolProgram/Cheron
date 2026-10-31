# 🎯 Project-wide C# ExpertDeveloper Refactor

## Understanding
Apply a broad refactor across production code and tests to align with `.github/agents/AGENT-CSharp-ExpertDeveloper.md`, allowing safe bug fixes and including project/config updates when needed. Keep the codebase compiling and tests passing while improving design consistency, error handling, async practices, and test quality.

## Assumptions
- The repo currently builds and has runnable tests.
- Existing architecture should be preserved unless a guideline-driven cleanup is clearly beneficial.
- "Safe bug fixes" means behavior changes only when correcting obvious defects with low regression risk.
- Refactor should remain incremental and validated continuously even though delivery is requested as one pass.

## Approach
I will first inventory projects, key config files, and source/test layout, then identify guideline mismatches (visibility, null checks, exception usage, async/cancellation flow, localization strings, test conventions). Next, I will apply targeted edits in batches across production and test code, prioritizing low-risk consistency changes and clear correctness fixes. I will then run build and tests, fix regressions, and provide a concise summary of all applied updates.

## Key Files
- `.github/agents/AGENT-CSharp-ExpertDeveloper.md` - source guidelines driving the refactor.
- `src/**/*.cs` - production code refactor targets.
- `tests/**/*.cs` (or equivalent test projects) - test quality and convention alignment.
- `Directory.Build.*`, `Directory.Packages.props`, `global.json`, `*.csproj` - configuration and language/tooling constraints.

## Risks & Open Questions
- Large-scope refactor can introduce regressions even with conservative edits.
- Some guideline rules may conflict with existing project conventions; project conventions will be prioritized where conflicts appear.
- Test runtime or external dependencies may limit full validation.

**Progress**: 100% [██████████]

**Last Updated**: 2026-10-29 00:31:03

## 📝 Plan Steps
- ✅ **Inventory solution structure and guideline-relevant configuration.**
- ✅ **Identify concrete refactor candidates across production code and tests.**
- ✅ **Apply production-code refactors for design, safety, and async/error-handling consistency.**
- ✅ **Apply test-code and configuration refactors aligned with the guideline.**
- ✅ **Build and run tests, then fix any issues introduced by refactor.**
- ✅ **Summarize completed changes and remaining recommendations.**

