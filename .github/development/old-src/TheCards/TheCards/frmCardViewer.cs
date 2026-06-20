// Card viewer form [151123]

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheCards
{
    public partial class frmCardViewer : Form
    {
        /****** START GLOBALS ******************************************************************************************/
        bool DEVMODE = true;                                                      // Permenant, only used for dev
        int curCard = 0;                                                          // Temp, start cards at 0
        List<Panel> cardBorders = new List<Panel>();                              // Temp, for card borders
        List<PictureBox> cardImages = new List<PictureBox>();                     // Temp, for card images
        Dictionary<string, string> appSetting = new Dictionary<string, string>(); // Temp, for global use settings
        string gameName = "mtg";                                                  // Temp, game name
        /****** END GLOBALS ********************************************************************************************/

        /// <summary>Initialize the card viewer form.</summary>
        /// <remarks>Gets the settings for everything right away.</remarks>
        public frmCardViewer()
        {
            InitializeComponent();
            SetApplicationPreferences();
        }

        /// <summary>Gets various settings from various sources.</summary>
        /// <remarks>
        /// Overview
        /// --------
        /// Get settings for The Cards, and the current game, from various sources.
        /// 
        /// Local variables
        /// ---------------
        /// thecardsPath: the filepath to data for The Cards
        /// generalSettings: general default settings for The Cards
        /// gameSettings: default settings for the current game
        /// 
        /// Notes
        /// -----
        /// [A] Put the contents of the general and game settings into dictionaries, then put some calculated values
        ///     into a dictionary.
        /// [B] Combine all three dictionaries into a single dictionary.
        /// </remarks>
        private void SetApplicationPreferences()
        {
            string generalPrefsPath = AppPath() + @"\general.settings";                                             //[A]
            var generalPrefs = DoFile.ToDictionary(generalPrefsPath, '=', true, true, '#');

            string viewPrefsPath = AppPath() + @"\games\" + gameName + @"\viewer.settings";                         //[B]
            var viewPrefs = DoFile.ToDictionary(viewPrefsPath, '=', true, true, '#');

            int borderWidth = int.Parse(viewPrefs["borderWidth"]);                                                  //[C]
            int borderHeight = int.Parse(viewPrefs["borderHeight"]);
            int viewerWidth = pnlViewer.Width;
            int viewerHeight = pnlViewer.Height;
            int columns = viewerWidth / borderWidth;
            int rows = viewerHeight / borderHeight;
            int bufferWidth = ((viewerWidth - (rows * borderWidth)) / (viewerWidth / borderWidth));
            int bufferHeight = ((viewerHeight - (columns * borderHeight)) / (viewerHeight / borderHeight));

            Dictionary<string, string> viewStatus = new Dictionary<string, string>                                  //[D]
            {
                {"viewerWidth", viewerWidth.ToString() },
                {"viewerHeight", viewerHeight.ToString() },
                {"rows", rows.ToString() },
                {"columns", columns.ToString() },
                {"bufferWidth", bufferWidth.ToString() },
                {"bufferHeight", bufferHeight.ToString() }
            };

            var dictionariesToCombine = new List<Dictionary<string, string>> { generalPrefs, viewPrefs, viewStatus }; //[B]
            appSetting = DoDictionary.CombineStringDictionaries(dictionariesToCombine);
        }

        /// <summary>Get the path for The Cards.</summary>
        /// <returns>The path for The Cards.</returns>
        /// <remarks>This will eventually be removed once the portable version is up and running.</remarks>
        private string AppPath()
        {
            if (DEVMODE)
            {
                return (DoCommon.GetPathAPCP() + @"thecards\"); // DEV -- APCP folder
            }
            else
            {
                return "CODE GOES HERE";                        // LIVE -- Portable
            }
        }

        /// <summary>Display a number of cards.</summary>
        /// <param name="startAt">The location of the card array to start at.</param>
        /// <param name="direction">Direction [forward/backward/none].</param>
        /// <remarks>
        /// [A] Constrain the starting place.
        /// [X] Loop through the number of cards on the page (rows * columns).
        /// [X] If going foward a page, make sure we're 
        /// </remarks>
        private void FillCardSlots(bool forward)
        {
            int maxSlots = int.Parse(appSetting["rows"]) * int.Parse(appSetting["columns"]);                    //[A]

            for (var cardSlot = 0; cardSlot <= maxSlots - 1; cardSlot++)                                        //[B]
            {
                if (forward)
                {
                    if (curCard >= maxSlots)
                    {
                        cardBorders[cardSlot].Controls.Remove(cardImages[curCard - maxSlots]);                  //[C] Remove previous PictureBox when not first page
                    }
                    else
                    {
                        cardBorders[cardSlot].Controls.Remove(cardImages[curCard]);                             //[D] Remove previous PictureBox when not first page
                    }

                    if (curCard < cardImages.Count)
                    {
                        cardBorders[cardSlot].Controls.Add(cardImages[curCard]);                                //[E] Create a new PictureBox
                    }

                }
                else
                {
                    if (curCard <= maxSlots)
                    {
                        cardBorders[cardSlot].Controls.Remove(cardImages[curCard]);
                    }

                    if (curCard >= 0)
                    {
                        cardBorders[cardSlot].Controls.Add(cardImages[curCard]);                                    // Create a new PictureBox
                    }
                }
                curCard++;
            }
        }

        /// <summary>User clicks the test button - DEV</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// [A] Put the set CSV into a list, then create an array for the cards whose size equals the size of that list
        /// [B] Loop through all of the cards in the set
        /// [C] Create a new card object at this element
        /// [D] Split the current card line in the CSV at the delimeter, then loop through all of the substrings and
        ///     apply the value to the card object ID
        /// [E] Variables for the view area such as number of rows/columns, border sizes, locations, etc. Currently
        ///     hardcoded, but will eventually be in the game configuration file
        /// [F] Loop through the number of rows and columns and create the cards
        /// [G] Some additional card stuff
        /// [H] Create the panels
        /// [I] Loop through the number of boxes we will have - dev set at 3 - and create a new picturebox for each
        /// [J] Actually display the cards
        /// </remarks>
        private void TEST_Click(object sender, EventArgs e)
        {
            string setDatabaseFile = AppPath() + @"games\mtg\sets\8E\8E.card.database";                             //[A]
            var gameSet = DoFile.ToList(setDatabaseFile, true, true, '#');
            Card[] setArray = new Card[gameSet.Count];                                                              // OR...create a dictionary that has keys from a Card object, and match with the gameSet array...THEN put that into an array

            for (int i = 0; i < gameSet.Count; i++)                                                                 //[B]
            {
                setArray[i] = new Card();                                                                           //[C]
                string[] words = gameSet[i].Split('¤');                                                             //[D]

                foreach (string word in words)                                                                      // Change to dictionary?
                {
                    setArray[i].Name = words[0];
                    setArray[i].Set = words[1];
                    setArray[i].SetCode = words[2];
                    setArray[i].ID = words[3];
                    setArray[i].Type = words[4];
                    setArray[i].Attack = words[5];
                    setArray[i].Defense = words[6];
                    setArray[i].Allegiance = words[7];
                    setArray[i].SpecificCost = words[8];
                    setArray[i].GenericCost = words[9];
                    setArray[i].Artist = words[10];
                    setArray[i].Flavor = words[11];
                    setArray[i].Color = words[12];
                    setArray[i].GeneratedResource = words[13];
                    setArray[i].Number = words[14];
                    setArray[i].Rarity = words[15];
                    setArray[i].Back = words[16];
                    setArray[i].Ability = words[17];
                    setArray[i].FrontA = "";
                    setArray[i].FrontB = "";
                    setArray[i].FrontC = "";
                }
            }

            int rows = int.Parse(appSetting["rows"]);
            int columns = int.Parse(appSetting["columns"]);
            int borderWidth = int.Parse(appSetting["borderWidth"]);
            int borderHeight =int.Parse(appSetting["borderHeight"]);
            int bufferWidth = int.Parse(appSetting["bufferWidth"]);
            int bufferHeight = int.Parse(appSetting["bufferHeight"]);
            int[] slotLocX = new int[rows];
            int[] slotLocY = new int[columns];

            for (int rowSlot = 0; rowSlot < rows ; rowSlot++)
            {
                if (rowSlot == 0)
                {
                    slotLocX[rowSlot] = bufferWidth;
                }
                else
                {
                    slotLocX[rowSlot] = ((borderWidth * rowSlot) + (bufferWidth * rowSlot));
                }
            }

            for (int columnSlot = 0; columnSlot < columns; columnSlot++)
            {
                if (columnSlot == 0)
                {
                    slotLocY[columnSlot] = bufferHeight;
                }
                else
                {
                    slotLocY[columnSlot] = ((borderHeight * columnSlot) + (bufferHeight * columnSlot));
                }
            }

            int locX, locY;                                                                                         //[F]

            for (int currentRow = 0; currentRow < rows; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < columns; currentColumn++)
                {
                    locX = slotLocX[currentColumn];
                    locY = slotLocY[currentRow];

                    Panel cardPanel = new Panel                                                                     //[G]
                    {
                        Name = "cardPanel_r" + currentRow + "c" + currentColumn,                                      // border location
                        Size = new Size(int.Parse(appSetting["borderWidth"]), int.Parse(appSetting["borderHeight"])),                                                           // Size of border
                        Location = new Point(slotLocX[currentColumn], slotLocY[currentRow]),   // Location of border, -1 to offset arrays starting a 0
                        BackgroundImage = Properties.Resources.borderWhite,                                // Border color
                        BackgroundImageLayout = ImageLayout.Zoom                                                // Type of image layout
                    };
                    cardBorders.Add(cardPanel);                                                                   // Add to the panel list
                }
            }

            foreach (Panel p in cardBorders)                                                                      //[H]
            {
                pnlViewer.Controls.Add(p);
            }

            for (int i = 1; i <= (setArray.Length - 1); i++)                                                    //[I]
            {
                PictureBox picture = new PictureBox
                {
                    Name = "pictureBox" + i,
                    Size = new Size(312, 445),
                    Location = new Point(2, 2),               // Change programatically
                    BorderStyle = BorderStyle.None,
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                picture.ImageLocation = AppPath() + @"games\mtg\sets\8E\100\" + setArray[i].Name + ".jpg";
                cardImages.Add(picture);
            }

            FillCardSlots(true); // force                                                                                    //[J]

        }

        /// <summary>User clicks page forward button</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// ADD
        /// </remarks>
        private void btnPageForward_Click(object sender, EventArgs e)
        {
            if ((curCard + Int32.Parse(appSetting["numberOfCardsPerPage"])) <= cardImages.Count)
            {
                curCard += Int32.Parse(appSetting["numberOfCardsPerPage"]);
            }
            else
            {
                curCard = cardImages.Count;
            }

            FillCardSlots(true);
        }

        /// <summary>User clicks page back button</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// [B] revert the page
        /// </remarks>
        private void btnPageBack_Click(object sender, EventArgs e)
        {
            if ((curCard - Int32.Parse(appSetting["numberOfCardsPerPage"])) >= 0)
            {
                curCard -= Int32.Parse(appSetting["numberOfCardsPerPage"]);
            }
            else
            {
                curCard = 0;
            }

            FillCardSlots(false); // If it's less than that, go forward
        }

        private void pnlCardViewerContainer_SizeChanged(object sender, EventArgs e)
        {
            if (appSetting.Count != 0)
            {
                int panelX = pnlViewer.Width;
                int panelY = pnlViewer.Height;
                int rows = panelX / int.Parse(appSetting["borderX"]);
                int cols = panelY / int.Parse(appSetting["borderY"]);
                int bufferX = (panelX - (rows * int.Parse(appSetting["borderX"]))) / rows;
                int bufferY = (panelY - (cols * int.Parse(appSetting["borderY"]))) / cols;

                appSetting["panelX"] = panelX.ToString();
                appSetting["panelY"] = panelY.ToString();
                appSetting["rows"] = rows.ToString();
                appSetting["cols"] = rows.ToString();
                appSetting["bufferX"] = bufferX.ToString();
                appSetting["bufferY"] = bufferY.ToString();
            }
        }

        /// <summary>User clicks exit button to close the form</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e) // RENAME CLOSE
        {
            Application.Exit(); // DEV -- Easier to exit
            //Dispose();        // LIVE -- Properly close
        }
    }
}