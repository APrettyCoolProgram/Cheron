# Cheron - Making Of 001

Create an agent file that will help me vibe code a game engine using MonoGame.

```text
I've created an AGENTS.md file tailored to help you vibe-code a reusable game engine using MonoGame. It's designed to guide an AI coding agent such as Codex or another coding assistant to work like a senior game-engine developer and practical pair programmer.

What's included

1. Desktop-first development

Targets Windows and macOS. Explicitly excludes mobile development and emphasizes compatibility with the MonoGame project template and graphics backend you actually choose.

2. Practical engine architecture

Guides the agent through rendering, input, scenes, asset management, timing, UI, audio, and other potential engine subsystems without forcing you to implement everything upfront.

3. Build and test discipline

Requires the agent to inspect the repository, make focused changes, run appropriate .NET builds and tests, and report actual results rather than claiming success without verification.

4. Protect your work

Prevents unauthorized destructive Git operations, unnecessary rewrites, and unrequested dependencies. Encourages reversible changes and clear explanations of trade-offs.

5. Vibe-coding workflow

Keeps the process collaborative and iterative: understand the task, make a small change, verify it, explain what happened, and move on to the next milestone.

How to use it
1. Create or open your MonoGame project in your preferred IDE.
1. 
2. Place AGENTS.md in the repository's root directory, alongside your solution or main project directory.
3. Start your coding agent in that repository and ask it to read and follow the instructions in AGENTS.md.
The file is written as general agent guidance rather than instructions for a particular AI tool.
My recommendation for your first milestone
Don't start by asking the agent to build an entire engine. Start with a minimal foundation that runs:
1. A working MonoGame desktop application.
2. A clean game loop with separate update and draw responsibilities.
3. Basic input handling.
4. A simple scene or game-state mechanism.
5. A small rendering helper and a way to load assets.
Once that foundation builds and runs on your machine, expand it incrementally.

The guiding principle: build an engine to support the games you want to make, rather than spending months building an engine before you can make a game.

```