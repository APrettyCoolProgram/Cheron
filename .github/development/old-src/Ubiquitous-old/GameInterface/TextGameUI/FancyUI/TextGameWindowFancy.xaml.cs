using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using TextGame;
using TextGame.Models;
using System.Collections.Generic;
using System.Linq;
using Ubiquitous.GameInterface.Audio;

namespace Ubiquitous.GameInterface.TextGameUI.FancyUI
{
    /// <summary>
    /// Interaction logic for TextGameWindowFancy.xaml. Represents the enhanced Fancy UI window.
    /// Fancy TextGames UI - enhanced interface with status panel, inventory display, and colored/flashing text support.
    /// </summary>
    public partial class TextGameWindowFancy : Window
    {
        private Game? _game;
        private readonly List<string> _commandHistory;
        private int _historyIndex;
        private CartridgeData? _cartridgeMetadata;
        private FantasyMusicGenerator? _musicGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextGameWindowFancy"/> class.
        /// </summary>
        public TextGameWindowFancy()
        {
            InitializeComponent();
            _commandHistory = new List<string>();
            _historyIndex = -1;
            
            InitializeGame();
            CommandInput.Focus();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextGameWindowFancy"/> class with a starting room.
        /// </summary>
        /// <param name="startingRoom">The starting room for the game</param>
        public TextGameWindowFancy(Room startingRoom)
        {
            InitializeComponent();
            _commandHistory = new List<string>();
            _historyIndex = -1;
            
            InitializeGame(startingRoom);
            CommandInput.Focus();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextGameWindowFancy"/> class with a starting room and metadata.
        /// </summary>
        /// <param name="startingRoom">The starting room for the game</param>
        /// <param name="metadata">The cartridge metadata containing theme information</param>
        public TextGameWindowFancy(Room startingRoom, CartridgeData metadata)
        {
            InitializeComponent();
            _commandHistory = new List<string>();
            _historyIndex = -1;
            _cartridgeMetadata = metadata;
            
            ApplyTheme();
            InitializeGame(startingRoom);
            InitializeMusic();
            CommandInput.Focus();
            
            // Subscribe to window closing event for cleanup
            this.Closing += TextGameWindowFancy_Closing;
        }

        /// <summary>
        /// Applies theme-specific styling based on the cartridge metadata
        /// </summary>
        private void ApplyTheme()
        {
            if (_cartridgeMetadata?.Genre?.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) == true)
            {
                ApplyFantasyTheme();
            }
        }

        /// <summary>
        /// Applies fantasy-themed colors and styling to the UI
        /// </summary>
        private void ApplyFantasyTheme()
        {
            // Window background - deep forest green/purple
            this.Background = new SolidColorBrush(Color.FromRgb(20, 15, 35));
            
            // Main borders - magical purple/gold
            var borders = new[] 
            { 
                FindName("StatusBorder"), 
                FindName("OutputBorder"), 
                FindName("InventoryBorder"),
                FindName("HistoryBorder")
            };
            
            foreach (var borderObj in borders)
            {
                if (borderObj is Border border)
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(35, 25, 55));
                    border.BorderBrush = new SolidColorBrush(Color.FromRgb(147, 112, 219)); // Medium purple
                }
            }
            
            // Output area - darker mystical background
            if (FindName("OutputScrollViewer") is ScrollViewer outputScroll)
            {
                outputScroll.Background = new SolidColorBrush(Color.FromRgb(25, 15, 40));
            }
            
            if (FindName("GameOutputText") is RichTextBox outputText)
            {
                outputText.Background = new SolidColorBrush(Color.FromRgb(25, 15, 40));
                outputText.Foreground = new SolidColorBrush(Color.FromRgb(220, 200, 255)); // Soft lavender
            }
            
            // Section headers - golden mystical color
            if (FindName("SectionHeaderStyle") is Style headerStyle)
            {
                headerStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(255, 215, 100))));
            }
            
            // Location and status cards - enchanted purple
            var statusCards = new[] 
            { 
                FindName("LocationCard"), 
                FindName("ExitsCard"), 
                FindName("StatsCard") 
            };
            
            foreach (var cardObj in statusCards)
            {
                if (cardObj is Border card)
                {
                    card.Background = new SolidColorBrush(Color.FromRgb(45, 35, 70));
                    card.BorderBrush = new SolidColorBrush(Color.FromRgb(186, 85, 211)); // Medium orchid
                }
            }
            
            // Text colors for fantasy theme
            var textElements = new[] 
            { 
                FindName("LocationText"), 
                FindName("ExitsText"), 
                FindName("MovesText"),
                FindName("ScoreText"), 
                FindName("ItemCountText"), 
                FindName("CommandHistoryText"),
                FindName("InventoryCountText")
            };
            
            foreach (var textObj in textElements)
            {
                if (textObj is TextBlock text)
                {
                    text.Foreground = new SolidColorBrush(Color.FromRgb(220, 200, 255));
                }
            }
            
            // Labels in golden color
            var labels = new[] 
            { 
                FindName("LocationLabel"), 
                FindName("ExitsLabel"), 
                FindName("StatsLabel") 
            };
            
            foreach (var labelObj in labels)
            {
                if (labelObj is TextBlock label)
                {
                    label.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 100));
                }
            }
            
            // Command input - mystical theme
            if (FindName("CommandInput") is TextBox commandInput)
            {
                commandInput.Background = new SolidColorBrush(Color.FromRgb(25, 15, 40));
                commandInput.Foreground = new SolidColorBrush(Color.FromRgb(220, 200, 255));
                commandInput.BorderBrush = new SolidColorBrush(Color.FromRgb(147, 112, 219));
                commandInput.CaretBrush = new SolidColorBrush(Color.FromRgb(255, 215, 100));
            }
            
            // Command prompt - golden
            if (FindName("CommandPrompt") is TextBlock prompt)
            {
                prompt.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 100));
            }
            
            // Send button - magical purple gradient
            if (FindName("SendButton") is Button sendButton)
            {
                var gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(1, 1);
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(147, 112, 219), 0.0));
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(186, 85, 211), 1.0));
                sendButton.Background = gradientBrush;
            }
            
            // Inventory cards - enchanted styling
            UpdateInventoryTheme();
            
            // Title update
            this.Title = $"Ubiquitous - {_cartridgeMetadata?.Title ?? "Fantasy Adventure"}";
        }

        /// <summary>
        /// Updates inventory display with fantasy theme
        /// </summary>
        private void UpdateInventoryTheme()
        {
            if (FindName("InventoryPanel") is StackPanel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is Border card)
                    {
                        card.Background = new SolidColorBrush(Color.FromRgb(45, 35, 70));
                        card.BorderBrush = new SolidColorBrush(Color.FromRgb(186, 85, 211));
                        
                        if (card.Child is StackPanel stackPanel)
                        {
                            foreach (var textChild in stackPanel.Children)
                            {
                                if (textChild is TextBlock textBlock)
                                {
                                    if (textBlock.FontWeight == FontWeights.Bold)
                                    {
                                        textBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 100));
                                    }
                                    else
                                    {
                                        textBlock.Foreground = new SolidColorBrush(Color.FromRgb(220, 200, 255));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the game with a sample adventure
        /// </summary>
        private void InitializeGame()
        {
            _game = GameBuilder.BuildDefaultGame();
            _game.OutputGenerated += OnGameOutput;
            
            UpdateStatus();
            UpdateInventory();
        }

        /// <summary>
        /// Initializes the game with a specific starting room
        /// </summary>
        /// <param name="startingRoom">The starting room for the game</param>
        private void InitializeGame(Room startingRoom)
        {
            _game = new Game();
            var player = new Player();
            _game.Initialize(startingRoom, player);
            _game.OutputGenerated += OnGameOutput;
            _game.Start();
            
            UpdateStatus();
            UpdateInventory();
        }

        /// <summary>
        /// Handles game output events
        /// </summary>
        private void OnGameOutput(string text)
        {
            Dispatcher.Invoke(() =>
            {
                AppendColoredText(text);
                OutputScrollViewer.ScrollToEnd();
            });
        }

        /// <summary>
        /// Appends text to the output with support for color codes and flashing
        /// Color codes: [COLOR:color_name]text[/COLOR]
        /// Flashing: [FLASH]text[/FLASH]
        /// </summary>
        private void AppendColoredText(string text)
        {
            var paragraph = GameOutputText.Document.Blocks.FirstOrDefault() as Paragraph;
            if (paragraph == null)
            {
                paragraph = new Paragraph();
                GameOutputText.Document.Blocks.Add(paragraph);
            }

            if (paragraph.Inlines.Count > 0)
            {
                paragraph.Inlines.Add(new LineBreak());
            }

            var segments = ParseTextWithFormatting(text);
            foreach (var segment in segments)
            {
                var run = new Run(segment.Text);

                if (segment.Color != null)
                {
                    run.Foreground = new SolidColorBrush(segment.Color.Value);
                }
                else
                {
                    run.Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204));
                }

                if (segment.IsFlashing)
                {
                    ApplyFlashingAnimation(run);
                }

                paragraph.Inlines.Add(run);
            }
        }

        /// <summary>
        /// Parses text with formatting codes
        /// </summary>
        private List<TextSegment> ParseTextWithFormatting(string text)
        {
            var segments = new List<TextSegment>();
            var currentPos = 0;

            while (currentPos < text.Length)
            {
                var colorStart = text.IndexOf("[COLOR:", currentPos, StringComparison.OrdinalIgnoreCase);
                var flashStart = text.IndexOf("[FLASH]", currentPos, StringComparison.OrdinalIgnoreCase);

                var nextTag = -1;
                var isColor = false;
                var isFlash = false;

                if (colorStart != -1 && (flashStart == -1 || colorStart < flashStart))
                {
                    nextTag = colorStart;
                    isColor = true;
                }
                else if (flashStart != -1)
                {
                    nextTag = flashStart;
                    isFlash = true;
                }

                if (nextTag == -1)
                {
                    if (currentPos < text.Length)
                    {
                        segments.Add(new TextSegment { Text = text.Substring(currentPos) });
                    }
                    break;
                }

                if (nextTag > currentPos)
                {
                    segments.Add(new TextSegment { Text = text.Substring(currentPos, nextTag - currentPos) });
                }

                if (isColor)
                {
                    var colorNameStart = nextTag + 7;
                    var colorNameEnd = text.IndexOf(']', colorNameStart);
                    if (colorNameEnd != -1)
                    {
                        var colorName = text.Substring(colorNameStart, colorNameEnd - colorNameStart);
                        var colorEnd = text.IndexOf("[/COLOR]", colorNameEnd, StringComparison.OrdinalIgnoreCase);
                        if (colorEnd != -1)
                        {
                            var coloredText = text.Substring(colorNameEnd + 1, colorEnd - colorNameEnd - 1);
                            segments.Add(new TextSegment 
                            { 
                                Text = coloredText, 
                                Color = GetColorFromName(colorName) 
                            });
                            currentPos = colorEnd + 8;
                        }
                        else
                        {
                            currentPos = colorNameEnd + 1;
                        }
                    }
                    else
                    {
                        currentPos = nextTag + 1;
                    }
                }
                else if (isFlash)
                {
                    var flashEnd = text.IndexOf("[/FLASH]", nextTag, StringComparison.OrdinalIgnoreCase);
                    if (flashEnd != -1)
                    {
                        var flashText = text.Substring(nextTag + 7, flashEnd - nextTag - 7);
                        segments.Add(new TextSegment 
                        { 
                            Text = flashText, 
                            IsFlashing = true 
                        });
                        currentPos = flashEnd + 8;
                    }
                    else
                    {
                        currentPos = nextTag + 1;
                    }
                }
            }

            return segments;
        }

        /// <summary>
        /// Gets a color from a color name string
        /// </summary>
        private Color? GetColorFromName(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => Color.FromRgb(255, 85, 85),
                "green" => Color.FromRgb(85, 255, 85),
                "blue" => Color.FromRgb(85, 170, 255),
                "yellow" => Color.FromRgb(255, 255, 85),
                "cyan" => Color.FromRgb(85, 255, 255),
                "magenta" => Color.FromRgb(255, 85, 255),
                "white" => Color.FromRgb(255, 255, 255),
                "gray" or "grey" => Color.FromRgb(128, 128, 128),
                "orange" => Color.FromRgb(255, 165, 85),
                "purple" => Color.FromRgb(170, 85, 255),
                _ => null
            };
        }

        /// <summary>
        /// Applies flashing animation to a text run
        /// </summary>
        private void ApplyFlashingAnimation(Run run)
        {
            var colorAnimation = new ColorAnimation
            {
                From = Color.FromRgb(204, 204, 204),
                To = Color.FromRgb(50, 50, 50),
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            var brush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            run.Foreground = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        /// <summary>
        /// Updates the status panel
        /// </summary>
        private void UpdateStatus()
        {
            if (_game?.Player == null) return;

            Dispatcher.Invoke(() =>
            {
                LocationText.Text = _game.Player.CurrentRoom?.Name ?? "Unknown";
                MovesText.Text = _game.Player.MoveCount.ToString();
                ScoreText.Text = _game.Player.Score.ToString();
                
                var itemCount = _game.Player.Inventory.GetAllItems().Count;
                ItemCountText.Text = itemCount.ToString();
                
                var exits = _game.Player.CurrentRoom?.GetAvailableExits();
                if (exits != null && exits.Count > 0)
                {
                    ExitsText.Text = string.Join(", ", exits);
                }
                else
                {
                    ExitsText.Text = "None";
                }
            });
        }

        /// <summary>
        /// Updates the inventory panel
        /// </summary>
        private void UpdateInventory()
        {
            if (_game?.Player == null) return;

            Dispatcher.Invoke(() =>
            {
                InventoryPanel.Children.Clear();

                var items = _game.Player.Inventory.GetAllItems();
                
                InventoryCountText.Text = items.Count.ToString();

                if (items.Count == 0)
                {
                    var emptyText = new TextBlock
                    {
                        Text = "Empty",
                        FontFamily = new FontFamily("Consolas"),
                        FontSize = 12,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    
                    // Apply theme-appropriate color
                    if (_cartridgeMetadata?.Genre?.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        emptyText.Foreground = new SolidColorBrush(Color.FromRgb(120, 100, 150));
                    }
                    else
                    {
                        emptyText.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));
                    }
                    
                    InventoryPanel.Children.Add(emptyText);
                }
                else
                {
                    foreach (var item in items)
                    {
                        var card = new Border
                        {
                            BorderThickness = new Thickness(1),
                            CornerRadius = new CornerRadius(4),
                            Padding = new Thickness(10),
                            Margin = new Thickness(0, 0, 0, 8)
                        };
                        
                        // Apply theme-appropriate colors
                        if (_cartridgeMetadata?.Genre?.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            card.Background = new SolidColorBrush(Color.FromRgb(45, 35, 70));
                            card.BorderBrush = new SolidColorBrush(Color.FromRgb(186, 85, 211));
                        }
                        else
                        {
                            card.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                            card.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                        }

                        var stackPanel = new StackPanel();

                        var nameText = new TextBlock
                        {
                            Text = item.Name.ToUpper(),
                            FontFamily = new FontFamily("Consolas"),
                            FontSize = 12,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 4)
                        };
                        
                        // Apply theme-appropriate colors
                        if (_cartridgeMetadata?.Genre?.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            nameText.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 100));
                        }
                        else
                        {
                            nameText.Foreground = new SolidColorBrush(Color.FromRgb(86, 156, 214));
                        }

                        var descText = new TextBlock
                        {
                            Text = item.Description,
                            FontFamily = new FontFamily("Consolas"),
                            FontSize = 10,
                            TextWrapping = TextWrapping.Wrap
                        };
                        
                        // Apply theme-appropriate colors
                        if (_cartridgeMetadata?.Genre?.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            descText.Foreground = new SolidColorBrush(Color.FromRgb(220, 200, 255));
                        }
                        else
                        {
                            descText.Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204));
                        }
                        
                        stackPanel.Children.Add(nameText);
                        stackPanel.Children.Add(descText);
                        card.Child = stackPanel;
                        InventoryPanel.Children.Add(card);
                    }
                }
            });
        }

        /// <summary>
        /// Handles the Send button click
        /// </summary>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessCommand();
        }

        /// <summary>
        /// Handles key down events in the command input
        /// </summary>
        private void CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessCommand();
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                NavigateCommandHistory(-1);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                NavigateCommandHistory(1);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Processes the current command
        /// </summary>
        private void ProcessCommand()
        {
            var command = CommandInput.Text.Trim();
            if (string.IsNullOrEmpty(command))
                return;

            _commandHistory.Add(command);
            _historyIndex = _commandHistory.Count;

            if (!string.IsNullOrEmpty(CommandHistoryText.Text))
            {
                CommandHistoryText.Text += "\n";
            }
            CommandHistoryText.Text += $"> {command}";
            HistoryScrollViewer.ScrollToEnd();

            CommandInput.Clear();

            if (command.Equals("quit", StringComparison.OrdinalIgnoreCase) || 
                command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                _game?.Stop();
                Close();
                return;
            }

            _game?.ProcessInput(command);
            
            UpdateStatus();
            UpdateInventory();
        }

        /// <summary>
        /// Navigates through command history
        /// </summary>
        private void NavigateCommandHistory(int direction)
        {
            if (_commandHistory.Count == 0)
                return;

            _historyIndex += direction;

            if (_historyIndex < 0)
                _historyIndex = 0;
            else if (_historyIndex >= _commandHistory.Count)
            {
                _historyIndex = _commandHistory.Count;
                CommandInput.Text = string.Empty;
                return;
            }

            CommandInput.Text = _commandHistory[_historyIndex];
            CommandInput.SelectionStart = CommandInput.Text.Length;
        }

        /// <summary>
        /// Initializes fantasy background music if appropriate
        /// </summary>
        private void InitializeMusic()
        {
            // Start fantasy music if this is a Fantasy game using Fancy UI
            if (_cartridgeMetadata?.Genre?.Equals("Fantasy", StringComparison.OrdinalIgnoreCase) == true &&
                _cartridgeMetadata?.UIType?.Equals("Fancy", StringComparison.OrdinalIgnoreCase) == true)
            {
                _musicGenerator = new FantasyMusicGenerator();
                _musicGenerator.Start();
            }
        }

        /// <summary>
        /// Handles window closing event to clean up resources
        /// </summary>
        private void TextGameWindowFancy_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop and dispose of music generator
            _musicGenerator?.Dispose();
            _musicGenerator = null;
        }

        /// <summary>
        /// Helper class for text formatting
        /// </summary>
        private class TextSegment
        {
            public string Text { get; set; } = string.Empty;
            public Color? Color { get; set; }
            public bool IsFlashing { get; set; }
        }
    }
}
