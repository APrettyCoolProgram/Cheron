<div align="center">

  <h1>Cheron: Scratchpad</h1>

</div>


You are an experienced software developer with a deep understanding of various programming languages, frameworks, and development methodologies, and user interfaces.

You will be building a GUI that will allow the user to:

* Load an existing Tekst cartridge and view/modify the contents
* Create a new Tekst cartridge
* Save changes made to the *.tekst file within the TekstEngineWindow back to the cartridge folder.

When the user clicks the "Load" button on the TekstEngineWindow, the application should open a folder dialog allowing the user to select folder containing Tekst cartridge data.

Tekst cartridge data contains all of the necessary data that a Tekst engine game requires to run.

The most important file in a cartridge folder is the *.tekst file, and it must be present for the Tekst engine to recognize and load the cartridge correctly.

Once the cartridge folder is selected and the *.tekst file is found, the application should load the content of the *.tekst file into the TekstEngineWindow - using the existing Tab Control -, allowing the user to view and edit the contents of the Tekst cartridge.


If needed, the next step is a proper Rooms/Items/Exits visual editor instead of editing the Rooms array as raw JSON.



* Delete a Tekst cartridge by removing its folder and all associated files.
* Rename a Tekst cartridge by changing the name of its folder and updating any references within the *.tekst file accordingly.
* Export a Tekst cartridge by creating a compressed archive of the cartridge folder, allowing it to be shared or backed up easily.
* Import a Tekst cartridge by extracting a compressed archive into a folder, allowing it to be loaded and edited within the TekstEngineWindow.
* View the contents of a Tekst cartridge by opening the *.tekst file within the TekstEngineWindow, allowing the user to read and understand the structure and content of the cartridge.
* Close the TekstEngineWindow, allowing the user to exit the editing interface and return to the main application window without making any changes to the loaded Tekst cartridge.