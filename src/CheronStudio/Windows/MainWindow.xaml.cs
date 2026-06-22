using System.Windows;
using System.Windows.Controls;

namespace CheronStudio;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // ── File ──────────────────────────────────────────────────────────────────

    private void MenuNewGame_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("New game workflow — not yet implemented.");
        StatusText.Text = "New game...";
    }

    private void MenuOpenGame_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Open game workflow — not yet implemented.");
        StatusText.Text = "Open game...";
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Save — not yet implemented.");
        StatusText.Text = "Saved.";
    }

    private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Save As — not yet implemented.");
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

    // ── View ──────────────────────────────────────────────────────────────────

    private void MenuViewProjectNavigator_Click(object sender, RoutedEventArgs e)
    {
        bool visible = MenuItemProjectNavigator.IsChecked;
        NavigatorPanel.Visibility    = visible ? Visibility.Visible : Visibility.Collapsed;
        NavigatorSplitter.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        NavigatorColumn.Width        = visible ? new GridLength(220) : new GridLength(0);
    }

    private void MenuViewOutput_Click(object sender, RoutedEventArgs e)
    {
        bool visible = MenuItemOutput.IsChecked;
        OutputPanel.Visibility      = visible ? Visibility.Visible : Visibility.Collapsed;
        OutputRow.Height            = visible ? new GridLength(150) : new GridLength(0);
        OutputSplitterRow.Height    = visible ? new GridLength(4)   : new GridLength(0);
    }

    private void MenuResetLayout_Click(object sender, RoutedEventArgs e)
    {
        NavigatorColumn.Width     = new GridLength(220);
        OutputRow.Height          = new GridLength(150);
        OutputSplitterRow.Height  = new GridLength(4);

        NavigatorPanel.Visibility    = Visibility.Visible;
        NavigatorSplitter.Visibility = Visibility.Visible;
        OutputPanel.Visibility       = Visibility.Visible;

        MenuItemProjectNavigator.IsChecked = true;
        MenuItemOutput.IsChecked           = true;
    }

    // ── Game ──────────────────────────────────────────────────────────────────

    private void MenuRun_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Run — not yet implemented.");
        StatusText.Text = "Running...";
    }

    private void MenuBuild_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Build — not yet implemented.");
        StatusText.Text = "Building...";
    }

    private void MenuGameProperties_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Game Properties — not yet implemented.");
    }

    // ── Tools ─────────────────────────────────────────────────────────────────

    private void MenuPreferences_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Preferences — not yet implemented.");
    }

    // ── Help ──────────────────────────────────────────────────────────────────

    private void MenuDocumentation_Click(object sender, RoutedEventArgs e)
    {
        AppendOutput("Documentation — not yet implemented.");
    }

    private void MenuAbout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Cheron Studio\n\nGame development for the Cheron ecosystem.",
            "About Cheron Studio",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void AppendOutput(string message)
    {
        OutputTextBox.Text += $"\n{message}";
        OutputTextBox.ScrollToEnd();
        StatusText.Text = message;
    }
}