using System.IO;
using System.Windows;
using System.Windows.Controls;
using CheronStudio.Engines.Tekst;
using Microsoft.Win32;

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
        var dialog = new OpenFileDialog
        {
            Title           = "Open Game",
            Filter          = "Tekst Game Files (*.tekst)|*.tekst|All Files (*.*)|*.*",
            DefaultExt      = ".tekst",
            CheckFileExists = true,
            CheckPathExists = true,
        };

        if (dialog.ShowDialog(this) != true)
            return;

        OpenTekstFile(dialog.FileName);
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        if (ActiveEditor() is not TekstGameEditor editor)
        {
            AppendOutput("Nothing to save.");
            return;
        }

        if (editor.Save())
        {
            UpdateTabHeader(editor);
            AppendOutput($"Saved: {editor.FilePath}");
            StatusText.Text = "Saved.";
        }
    }

    private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
    {
        if (ActiveEditor() is not TekstGameEditor editor)
        {
            AppendOutput("Nothing to save.");
            return;
        }

        if (editor.SaveAs())
        {
            UpdateTabHeader(editor);
            AppendOutput($"Saved as: {editor.FilePath}");
            StatusText.Text = "Saved.";
        }
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
        OutputPanel.Visibility   = visible ? Visibility.Visible : Visibility.Collapsed;
        OutputRow.Height         = visible ? new GridLength(150) : new GridLength(0);
        OutputSplitterRow.Height = visible ? new GridLength(4)   : new GridLength(0);
    }

    private void MenuResetLayout_Click(object sender, RoutedEventArgs e)
    {
        NavigatorColumn.Width    = new GridLength(220);
        OutputRow.Height         = new GridLength(150);
        OutputSplitterRow.Height = new GridLength(4);

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

    // ── Tekst engine ──────────────────────────────────────────────────────────

    private void OpenTekstFile(string filePath)
    {
        // If already open, just activate that tab.
        foreach (TabItem existing in WorkspaceTabControl.Items)
        {
            if (existing.Content is TekstGameEditor existingEditor &&
                string.Equals(existingEditor.FilePath, filePath, StringComparison.OrdinalIgnoreCase))
            {
                WorkspaceTabControl.SelectedItem = existing;
                return;
            }
        }

        try
        {
            var editor = new TekstGameEditor();
            editor.Load(filePath);
            editor.DirtyChanged += (_, _) => UpdateTabHeader(editor);

            var tab = new TabItem
            {
                Header  = Path.GetFileName(filePath),
                Content = editor,
                Tag     = filePath,
            };

            WorkspaceTabControl.Items.Add(tab);
            WorkspaceTabControl.SelectedItem = tab;

            PopulateNavigatorForFile(filePath);
            ProjectStatusText.Text = Path.GetFileName(filePath);
            AppendOutput($"Opened: {filePath}");
            StatusText.Text = "Ready";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not open the file.\n\nDetails: {ex.Message}",
                "Error Opening File",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            AppendOutput($"Error: {ex.Message}");
        }
    }

    private void PopulateNavigatorForFile(string filePath)
    {
        ProjectNavigator.Items.Clear();

        var root = new TreeViewItem
        {
            Header     = Path.GetFileName(filePath),
            Tag        = filePath,
            IsExpanded = true,
        };

        var dir = Path.GetDirectoryName(filePath);
        if (dir is not null && Directory.Exists(dir))
        {
            foreach (var f in Directory.EnumerateFiles(dir, "*.tekst"))
            {
                root.Items.Add(new TreeViewItem
                {
                    Header = Path.GetFileName(f),
                    Tag    = f,
                });
            }
        }

        ProjectNavigator.Items.Add(root);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private TekstGameEditor? ActiveEditor()
    {
        if (WorkspaceTabControl.SelectedItem is TabItem tab)
            return tab.Content as TekstGameEditor;
        return null;
    }

    private void UpdateTabHeader(TekstGameEditor editor)
    {
        foreach (TabItem tab in WorkspaceTabControl.Items)
        {
            if (tab.Content == editor)
            {
                var name = editor.FilePath is not null
                    ? Path.GetFileName(editor.FilePath)
                    : "Untitled";
                tab.Header = editor.IsDirty ? $"{name} •" : name;
                return;
            }
        }
    }

    private void AppendOutput(string message)
    {
        OutputTextBox.Text += $"\n{message}";
        OutputTextBox.ScrollToEnd();
        StatusText.Text = message;
    }
}
