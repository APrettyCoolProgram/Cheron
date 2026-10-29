<div align="center">

  <img src="../logo/Cheron-Logo-512x512.png" alt="Repository Template">

  <h1>Development Notes</h1>

</div>


As we implement the new start screen, keep in mind any non-game specific logic (e.g., "Solitaire") should be in Cheron


Currently Cheron launches directly into a game of Solitaire.
Since Cheron will be used for multiple game types, let's modify the start screen as follows:
•	The start screen should have a warm light brown background
•	In the center, show a 1970's video game console with a cartridge port and a power button (that is off by default)
•	To the side of the console, have a number of cartridges lying around
•	One of those cartridges says "Solitaire"
•	The user can click and drag the Solitaire cartridge to the cartridge port, at which point it will "snap" into place
•	If the user presses the "Power" button, a game of Solitaire will start.
What do you think about this idea?


Strong idea. It fits the multi-game direction and gives Cheron a clear identity as a game hub.
Pros:
•	Good thematic UX for “engine + cartridges = games”.
•	Creates a natural, extensible game selection model.
•	Lets Solitaire remain a “cartridge” without changing its core logic.
Main implementation considerations:
•	Add a launcher module/state before game modules.
•	Build simple drag/drop + snap zones for cartridge interaction.
•	Add a power-state gate: only launch when cartridge inserted + power on.
•	Keep launcher assets/logic separate from Solitaire to avoid coupling.
Recommended rollout:
1.	Static launcher scene (background, console, cartridges, power button).
2.	Cartridge drag/drop with snapping.
3.	Power button behavior and Solitaire handoff.
4.	Visual polish (1970s styling, animations, sound cues).
So: yes, this is a solid direction and worth doing.

