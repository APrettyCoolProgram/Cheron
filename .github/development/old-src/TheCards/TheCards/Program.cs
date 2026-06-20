/* The Cards ALPHA 24
 *
 * LICENSE TO GO HERE
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheCards
{
    static class Program
    {
        /// <summary>The main entry point for the application.</summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new frmCardViewer());   // Go directly to the viewer -- DEV
            //Application.Run(new frmTheCardsMainMenu()); // This is commented out to make the development of VIEWER easier -- LIVE
        }
    }
}
