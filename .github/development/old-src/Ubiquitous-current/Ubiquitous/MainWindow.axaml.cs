using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Text;
using TextGame;

namespace Ubiquitous;

public partial class MainWindow : Window
{
    private GameEngine? _game;
    private readonly StringBuilder _outputHistory;

    public MainWindow()
    {
        InitializeComponent();
        _outputHistory = new StringBuilder();
    }

    private void NewGame_Click(object? sender, RoutedEventArgs e)
    {
        StartNewGame();
    }

    private void Help_Click(object? sender, RoutedEventArgs e)
    {
        if (_game != null)
        {
            ProcessCommand("help");
        }
        else
        {
            AppendOutput("\n=== HELP ===\n");
            AppendOutput("Click 'New Game' to start the sample adventure game.\n\n");
            AppendOutput("Once the game starts, you can use these commands:\n");
            AppendOutput("- Movement: north/south/east/west/up/down (or n/s/e/w/u/d)\n");
            AppendOutput("- Items: take [item], drop [item], examine [item]\n");
            AppendOutput("- Look: look or l\n");
            AppendOutput("- Inventory: inventory or i\n");
            AppendOutput("- Score: score\n");
            AppendOutput("- Help: help or ?\n");
            AppendOutput("- Quit: quit, exit, or q\n");
        }
    }

    private void Send_Click(object? sender, RoutedEventArgs e)
    {
        ProcessUserInput();
    }

    private void InputTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _game != null)
        {
            ProcessUserInput();
        }
    }

    private void StartNewGame()
    {
        _outputHistory.Clear();
        
        // Create and start the sample game
        _game = GameBuilder.CreateSampleGame();
        
        AppendOutput("=".PadRight(70, '=') + "\n");
        AppendOutput(_game.GameTitle + "\n");
        AppendOutput("=".PadRight(70, '=') + "\n\n");
        AppendOutput(_game.GameIntro + "\n\n");
        
        _game.Start();
        
        // Show initial room description
        var initialDescription = _game.ProcessCommand("look");
        AppendOutput(initialDescription + "\n");
        
        // Update UI
        GameTitleText.Text = _game.GameTitle;
        StatusText.Text = "Game Started";
        SendButton.IsEnabled = true;
        InputTextBox.IsEnabled = true;
        InputTextBox.Focus();
        
        UpdateStatusBar();
    }

    private void ProcessUserInput()
    {
        if (_game == null || string.IsNullOrWhiteSpace(InputTextBox.Text))
            return;

        var command = InputTextBox.Text.Trim();
        
        // Echo the command
        AppendOutput($"\n> {command}\n");
        
        // Clear input
        InputTextBox.Text = string.Empty;
        
        // Process the command
        ProcessCommand(command);
        
        // Focus back on input
        InputTextBox.Focus();
    }

    private void ProcessCommand(string command)
    {
        if (_game == null)
            return;

        var response = _game.ProcessCommand(command);
        AppendOutput(response + "\n");
        
        UpdateStatusBar();
        
        // Check if game ended
        if (!_game.IsRunning)
        {
            AppendOutput("\n" + "=".PadRight(70, '=') + "\n");
            AppendOutput("GAME OVER\n");
            AppendOutput("=".PadRight(70, '=') + "\n\n");
            AppendOutput($"Final Score: {_game.Player.Score}\n");
            AppendOutput($"Total Moves: {_game.Player.Moves}\n\n");
            AppendOutput("Click 'New Game' to play again.\n");
            
            StatusText.Text = "Game Over";
            SendButton.IsEnabled = false;
            InputTextBox.IsEnabled = false;
        }
    }

    private void UpdateStatusBar()
    {
        if (_game != null)
        {
            ScoreText.Text = $"Score: {_game.Player.Score}";
            MovesText.Text = $"Moves: {_game.Player.Moves}";
        }
    }

    private void AppendOutput(string text)
    {
        _outputHistory.Append(text);
        OutputText.Text = _outputHistory.ToString();
        
        // Auto-scroll to bottom
        OutputScrollViewer.ScrollToEnd();
    }
}
