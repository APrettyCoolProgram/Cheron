using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DungineStudio.ViewModels;

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
            DataContext = new MainViewModel();
        }

        private void ChooseBoxArt_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel vm)
            {
                var cmd = vm.ChooseBoxArtCommand;
                if (cmd != null && cmd.CanExecute(null))
                    cmd.Execute(null);
            }
        }

        private void ChooseLabelArt_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel vm)
            {
                var cmd = vm.ChooseLabelArtCommand;
                if (cmd != null && cmd.CanExecute(null))
                    cmd.Execute(null);
            }
        }
    }
}