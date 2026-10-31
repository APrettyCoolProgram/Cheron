<div align="center">

  <img src="../logo/Cheron-Logo-512x512.png" alt="Repository Template">

  <h1>Development Notes</h1>

</div>


***

## Before release

- Are there any other tests we should create for the %Component% component?

Let's do a pass over all of the project code, and using the guidelines in `.github/agents/AGENT-CSharp-DotNet-Janitor.md`, `.github/agents/AGENT-CSharp-ExpertDeveloper.md`, and `.github/agents/AGENT-MonoGame-Engine.md` and:
- Confirm all project source code is human readable
- Confirm all variables/methods/etc. have descriptive names
- Confirm source code formatting is correct

- Confirm all source code in the entire solution has accurate XML documentation, according to the guidelines in `.github/agents/AGENT-CSharp-XmlDocumentation.md`

- Update the repository README.md according to the guidelines in `.github/agents/AGENT-Repository-Readme.md`

- Update the `docs/rn/rn26-10.md` release notes file according to the guidelines in `.github/agents/AGENT-ReleaseNotes.md`

***

## Start of new release

Let's do a pass over all of the project code, and using the guidelines in `.github/agents/AGENT-CSharp-DotNet-Janitor.md`, `.github/agents/AGENT-CSharp-ExpertDeveloper.md`, and `.github/agents/AGENT-MonoGame-Engine.md` and:
- Redundant code is refactored
- Unnecessary code is removed
