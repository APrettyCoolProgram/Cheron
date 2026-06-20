using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DungineStudio.GameType.TextBased;

namespace DungineStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateTextGame_Click(object sender, RoutedEventArgs e)
        {
            // Hide the main window and open the text-based game window.
            this.Hide();

            var textWindow = new TextBasedWindow();
            // When the text window closes, show the main window again.
            textWindow.Closed += (_, __) => this.Show();
            textWindow.Show();
        }
    }
}