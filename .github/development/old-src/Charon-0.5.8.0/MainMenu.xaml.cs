// 260201_code
// 260201_documentation

using System.Reflection;
using System.Windows;

namespace Charon
{
    /// <summary>Main menu window offering entry points to Charon features.</summary>
    public partial class MainMenu : Window
    {
        /// <summary>Initialize the main menu and its UI.</summary>
        public MainMenu()
        {
            InitializeComponent();

            lblVersion.Content = $"Version {Assembly.GetEntryAssembly().GetName().Version.ToString()}";
        }

        internal void openPaletteSwapper()
        {
            var paletteSwapperWindow = new PaletteSwapper();
            paletteSwapperWindow.Owner = this;
            paletteSwapperWindow.Show();
        }

        /* EVENT HANDLERS */
        private void OpenPaletteSwapper_Click(object sender, RoutedEventArgs e) => openPaletteSwapper();
    }
}