// 260213_code
// 260213_documentation

using System.Reflection;
using System.Windows;
using Cheron.PaletteSwapper;

namespace Cheron;

/// <summary>Entry class for Cheron.</summary>
public partial class MainWindow : Window
{
    /// <summary>Entry method for Cheron.</summary>
    public MainWindow()
    {
        InitializeComponent();

        StartApp();
    }

    /// <summary>Application startup tasks.</summary>
    private void StartApp()
    {
        lblCheronVersion.Content = $"Version {Assembly.GetEntryAssembly().GetName().Version.ToString()}";
    }

    /* EVENTS */

    /// <summary>Opens the palette swapper window and hides the main window.</summary>
    private void PaletteSwapperClicked()
    {
        var paletteSwapperWindow = new PaletteSwapperWindow();
        paletteSwapperWindow.Show();
        Hide();
    }

    /* EVENT HANDLERS */
    private void PaletteSwapper_Click(object sender, RoutedEventArgs e) => PaletteSwapperClicked();
}