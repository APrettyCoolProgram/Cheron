using System.Windows;
using Ubiquitous.GameInterface.CartridgeSelection;
using Ubiquitous.GameInterface.CartridgeCreator;
using Ubiquitous.GameInterface.TextGameUI.BasicUI;
using Ubiquitous.GameInterface.TextGameUI.FancyUI;

namespace Ubiquitous
{
    /// <summary>Interaction logic for MainWindow.xaml - represents the primary application window with game selection menu</summary>
    public partial class MainWindow : Window
    {
        /// <summary>Initializes a new instance of the MainWindow class</summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Handles the Play button click - opens cartridge selection window</summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var cartridgeListWindow = new CartridgeListWindow();
            cartridgeListWindow.Show();
            Close();
        }

        /// <summary>Handles the Create button click - opens cartridge creation window</summary>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var cartridgeCreatorWindow = new CartridgeCreatorWindow();
            cartridgeCreatorWindow.Show();
            Close();
        }

        /// <summary>Handles the Settings button click - placeholder for future settings window</summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Settings coming soon!",
                "Settings",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>Handles the Basic Text Game button click</summary>
        private void BasicTextGameButton_Click(object sender, RoutedEventArgs e)
        {
            var basicWindow = new TextGameWindow();
            basicWindow.Show();
            Close();
        }

        /// <summary>Handles the Fancy Text Game button click</summary>
        private void FancyTextGameButton_Click(object sender, RoutedEventArgs e)
        {
            var fancyWindow = new TextGameWindowFancy();
            fancyWindow.Show();
            Close();
        }
    }
}
