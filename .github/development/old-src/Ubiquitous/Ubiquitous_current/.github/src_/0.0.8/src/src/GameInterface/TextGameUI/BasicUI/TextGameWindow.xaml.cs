using System.Windows;
using System.Windows.Input;
using TextGame;
using System.Collections.Generic;

namespace Ubiquitous.GameInterface.TextGameUI.BasicUI
{
    /// <summary>
    /// Interaction logic for TextGameWindow.xaml. Represents the primary application window.
    /// Basic TextGames UI - simplified black and white interface.
    /// </summary>
    public partial class TextGameWindow : Window
    {
        private Game? _game;
        private readonly List<string> _commandHistory;
        private int _historyIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextGameWindow"/> class.
        /// </summary>
        public TextGameWindow()
        {
            InitializeComponent();
            _commandHistory = new List<string>();
            _historyIndex = -1;
            
            InitializeGame();
            CommandInput.Focus();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextGameWindow"/> class with a starting room.
        /// </summary>
        /// <param name="startingRoom">The starting room for the game</param>
        public TextGameWindow(Room startingRoom)
        {
            InitializeComponent();
            _commandHistory = new List<string>();
            _historyIndex = -1;
            
            InitializeGame(startingRoom);
            CommandInput.Focus();
        }

        /// <summary>
        /// Initializes the game with a sample adventure
        /// </summary>
        private void InitializeGame()
        {
            _game = GameBuilder.BuildDefaultGame();
            _game.OutputGenerated += OnGameOutput;
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
        }

        /// <summary>
        /// Handles game output events
        /// </summary>
        private void OnGameOutput(string text)
        {
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(GameOutputText.Text))
                {
                    GameOutputText.Text += "\n";
                }
                GameOutputText.Text += text;

                OutputScrollViewer.ScrollToEnd();
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

            CommandInput.Clear();

            if (command.Equals("quit", StringComparison.OrdinalIgnoreCase) || 
                command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                _game?.Stop();
                Close();
                return;
            }

            _game?.ProcessInput(command);
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
    }
}
