using System.Windows;

namespace Ubiquitus
{
    /// <summary>
    /// Main window of the Ubiquitus application
    /// </summary>
    /// <remarks>
    /// This is the primary user interface window for the Ubiquitus text adventure game player.
    /// Currently displays a placeholder interface. Future versions will include:
    /// - Terminal-style text display for game output
    /// - Command input interface for player commands
    /// - Cartridge selection and management UI
    /// - Game state controls (save/load, restart, etc.)
    /// </remarks>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        /// <remarks>
        /// Calls InitializeComponent() to load the XAML-defined user interface.
        /// The window is shown automatically after the splash screen closes.
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}