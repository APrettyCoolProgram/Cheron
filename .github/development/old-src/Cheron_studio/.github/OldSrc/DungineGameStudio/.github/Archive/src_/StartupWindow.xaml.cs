using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DungineStudio
{
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void CreateTextGame_Click(object sender, MouseButtonEventArgs e)
        {
            OpenMainWindow(createNew: true);
        }

        private void OpenExisting_Click(object sender, MouseButtonEventArgs e)
        {
            OpenMainWindow(createNew: false);
        }

        private void OpenMainWindow(bool createNew)
        {
            var mainWindow = new MainWindow();

            if (createNew)
            {
                // Trigger the New Cartridge command after the window loads
                mainWindow.Loaded += (s, e) =>
                {
                    var viewModel = mainWindow.DataContext as ViewModels.MainViewModel;
                    if (viewModel?.NewGameCommand.CanExecute(null) == true)
                    {
                        viewModel.NewGameCommand.Execute(null);
                    }
                };
            }

            mainWindow.Show();
            this.Close();
        }

        private void GameTypeCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
            }
        }

        private void GameTypeCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7"));
            }
        }
    }
}
