using System.Windows;
using CheronStudio.TekstEngine; // Ensure this namespace is included to access TekstCartridgeWindow


namespace CheronStudio;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Ensure proper constructor declaration
    public MainWindow()
    {
        InitializeComponent();
    }


    private void btnTekstEngine_Click()
    {
        // Hide current window (MainWindow) and show TekstCartridge
        this.Hide(); // or this.Hide() if you plan to re-show later.

        // Open the new window:
        var tekstCartridgeWindow = new TekstCartridgeWindow();
        tekstCartridgeWindow.Show();
    }

    private void btnTekstEngine_Click(object sender, RoutedEventArgs e) => btnTekstEngine_Click();
}