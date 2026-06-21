using System.Windows;

namespace DungineStudio.Views
{
    public partial class AddExitDialog : Window
    {
        public string Direction { get; set; } = string.Empty;
        public string TargetLocationId { get; set; } = string.Empty;

        public AddExitDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Direction = DirectionTextBox.Text;
            TargetLocationId = TargetTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
