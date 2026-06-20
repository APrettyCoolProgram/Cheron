using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextGame.Loaders;
using TextGame.Models;
using Ubiquitous.GameInterface.TextGameUI.BasicUI;
using Ubiquitous.GameInterface.TextGameUI.FancyUI;

namespace Ubiquitous.GameInterface.CartridgeSelection
{
    /// <summary>
    /// Displays a list of available game cartridges for selection
    /// </summary>
    public partial class CartridgeListWindow : Window
    {
        private List<CartridgeItem> _allCartridges = new();
        private string _currentFilter = string.Empty;

        /// <summary>
        /// Represents a cartridge item in the UI list
        /// </summary>
        public class CartridgeItem
        {
            public string Title { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string UIType { get; set; } = string.Empty;
            public string Difficulty { get; set; } = string.Empty;
            public string Genre { get; set; } = string.Empty;
            public string FilePath { get; set; } = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the CartridgeListWindow
        /// </summary>
        public CartridgeListWindow()
        {
            InitializeComponent();
            LoadCartridges();
        }

        /// <summary>
        /// Loads all cartridge files from the Cartridge directory
        /// </summary>
        private void LoadCartridges()
        {
            var cartridgeDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cartridge");
            _allCartridges.Clear();

            if (Directory.Exists(cartridgeDirectory))
            {
                var cartridgeFiles = Directory.GetFiles(cartridgeDirectory, "*.json", SearchOption.AllDirectories);

                foreach (var filePath in cartridgeFiles)
                {
                    try
                    {
                        var metadata = CartridgeLoader.LoadCartridgeMetadata(filePath);
                        if (metadata != null)
                        {
                            _allCartridges.Add(new CartridgeItem
                            {
                                Title = metadata.Title,
                                Author = metadata.Author,
                                Version = metadata.Version,
                                Description = metadata.Description,
                                Type = metadata.Type,
                                UIType = metadata.UIType,
                                Difficulty = metadata.Difficulty,
                                Genre = metadata.Genre,
                                FilePath = filePath
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error loading cartridge '{Path.GetFileName(filePath)}': {ex.Message}",
                            "Load Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }

            if (_allCartridges.Count == 0)
            {
                _allCartridges.Add(new CartridgeItem
                {
                    Title = "No Cartridges Found",
                    Author = "System",
                    Version = "N/A",
                    Description = $"No .json files found in the Cartridge directory: {cartridgeDirectory}",
                    FilePath = string.Empty
                });
            }

            UpdateGameTypeButtons();
            ApplyFilter();
        }

        /// <summary>
        /// Updates the visibility of game type buttons based on available games
        /// </summary>
        private void UpdateGameTypeButtons()
        {
            // Get unique game types from loaded cartridges
            var gameTypes = _allCartridges
                .Where(c => !string.IsNullOrEmpty(c.Type))
                .Select(c => c.Type)
                .Distinct()
                .ToList();

            // Show/hide TextGame button based on availability
            if (TextGameButton != null)
            {
                TextGameButton.Visibility = gameTypes.Contains("TextGame") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Applies the current filter to the cartridge list
        /// </summary>
        private void ApplyFilter()
        {
            IEnumerable<CartridgeItem> filteredCartridges;

            if (string.IsNullOrEmpty(_currentFilter))
            {
                // Show all cartridges
                filteredCartridges = _allCartridges;
            }
            else
            {
                // Filter by type
                filteredCartridges = _allCartridges.Where(c => c.Type == _currentFilter);
            }

            CartridgeListControl.ItemsSource = filteredCartridges.ToList();
        }

        /// <summary>
        /// Handles game type button clicks to filter the cartridge list
        /// </summary>
        private void GameTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Update the current filter
                _currentFilter = button.Tag?.ToString() ?? string.Empty;

                // Update button styles to show active state
                UpdateButtonStyles(button);

                // Apply the filter
                ApplyFilter();
            }
        }

        /// <summary>
        /// Updates button styles to highlight the active filter
        /// </summary>
        private void UpdateButtonStyles(Button activeButton)
        {
            // Reset all buttons to default style
            AllGamesButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F3F46"));
            if (TextGameButton.Visibility == Visibility.Visible)
            {
                TextGameButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F3F46"));
            }

            // Highlight the active button
            activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC"));
        }

        /// <summary>
        /// Handles clicking on a cartridge item to launch the game
        /// </summary>
        private void CartridgeItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string filePath && !string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Load the cartridge metadata to check UIType
                    var metadata = CartridgeLoader.LoadCartridgeMetadata(filePath);
                    if (metadata == null)
                    {
                        MessageBox.Show(
                            "Failed to load cartridge metadata.",
                            "Load Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    // Load the starting room
                    var startingRoom = CartridgeLoader.LoadFromJson(filePath);
                    if (startingRoom != null)
                    {
                        // Determine which UI to use based on UIType
                        Window gameWindow;
                        if (metadata.UIType?.Equals("Fancy", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            gameWindow = new TextGameWindowFancy(startingRoom, metadata);
                        }
                        else
                        {
                            // Default to Basic UI for "Basic" or any other value
                            gameWindow = new TextGameWindow(startingRoom);
                        }

                        gameWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to load the game cartridge.",
                            "Load Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error launching game: {ex.Message}",
                        "Launch Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Handles the Back button click to return to the main window
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
