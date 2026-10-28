using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ubiquitous
{
    /// <summary>
    /// Interaction logic for MainWindowFancy.xaml. Represents the enhanced Fancy UI window.
    /// </summary>
    public partial class MainWindowFancy : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowFancy"/> class.
        /// </summary>
        public MainWindowFancy()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Send button click
        /// </summary>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessCommand();
        }

        /// <summary>
        /// Handles key down events in the command input
        /// </summary>
        private void CommandInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessCommand();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Processes the current command
        /// </summary>
        private void ProcessCommand()
        {
            var command = CommandInput.Text.Trim();
            if (string.IsNullOrEmpty(command))
                return;

            if (!string.IsNullOrEmpty(CommandHistoryText.Text))
            {
                CommandHistoryText.Text += "\n";
            }
            CommandHistoryText.Text += $"> {command}";

            CommandInput.Clear();

            // TODO: Wire up to game engine
            var paragraph = GameOutputText.Document.Blocks.FirstOrDefault() as Paragraph;
            if (paragraph == null)
            {
                paragraph = new Paragraph();
                GameOutputText.Document.Blocks.Add(paragraph);
            }

            if (paragraph.Inlines.Count > 0)
            {
                paragraph.Inlines.Add(new LineBreak());
            }

            var run = new Run($"You entered: {command}")
            {
                Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204))
            };
            paragraph.Inlines.Add(run);

            OutputScrollViewer.ScrollToEnd();
        }
    }
}
