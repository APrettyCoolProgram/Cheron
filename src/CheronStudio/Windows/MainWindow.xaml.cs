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
        Background                = Brushes.DarkGray;
        btnTekstEngine.Background = Brushes.LightGray;

        var tekstEngineWindow = new TekstEngineWindow();
        tekstEngineWindow.Show();
    }


    // EVENT HANDLERS

    private void btnTekstEngine_Click(object sender, RoutedEventArgs e) => btnTekstEngine_Click();
}