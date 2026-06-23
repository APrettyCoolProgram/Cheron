using System.Windows;
using System.Windows.Media;
using CheronStudio.TekstEngine; // Ensure this namespace is included to access TekstCartridgeWindow

namespace CheronStudio;

public partial class MainWindow : Window
{
    // Ensure proper constructor declaration
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnTekstEngine_Click()
    {
        Background                = System.Windows.Media.Brushes.DarkGray;
        btnTekstEngine.Background = System.Windows.Media.Brushes.LightGray;

        var tekstEngineWindow = new TekstEngineWindow();
        tekstEngineWindow.Show();
    }


    // EVENT HANDLERS

    private void btnTekstEngine_Click(object sender, RoutedEventArgs e) => btnTekstEngine_Click();
}