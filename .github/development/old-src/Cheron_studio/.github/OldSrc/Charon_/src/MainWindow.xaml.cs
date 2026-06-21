// 260207_code
// 260207_documentation

using System.Reflection;
using System.Windows;
using Charon.PaletteSwapper;

namespace Charon;

/// <summary>
/// Entry class for Charon
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        StartApp();
    }

    private void StartApp()
    {
        lblCheronVersion.Content = $"Version {Assembly.GetEntryAssembly().GetName().Version.ToString()}";
    }

    /*
     * EVENTS
     */

    private void PaletteSwapperClicked()
    {
        var paletteSwapperWindow = new PaletteSwapperWindow();
        paletteSwapperWindow.Show();
        this.Hide();
    }

    /*
     * EVENT HANDLERS
     */

    /// <summary>
    /// Handle the PaletteSwapper button click event
    /// </summary>
    private void PaletteSwapper_Click(object sender, RoutedEventArgs e) => PaletteSwapperClicked();
}