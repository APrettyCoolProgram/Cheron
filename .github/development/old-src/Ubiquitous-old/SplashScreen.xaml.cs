using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ubiquitous
{
    /// <summary>Represents the application splash screen displayed during startup with logo, version information, and 8-second auto-transition to main window</summary>
    public partial class SplashScreen : Window
    {
        private DispatcherTimer? _timer;

        /// <summary>Initializes a new instance of the SplashScreen class, setting up version display and configuring the timer for automatic transition</summary>
        public SplashScreen()
        {
            InitializeComponent();
            SetVersionText();
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(8)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        /// <summary>Sets the version text displayed on the splash screen by reading the assembly version in format "Version X.Y.Z"</summary>
        private void SetVersionText()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = $"Version {version?.Major}.{version?.Minor}.{version?.Build}";
        }

        /// <summary>Handles mouse click on the splash screen to allow early dismissal</summary>
        /// <param name="sender">The window that was clicked.</param>
        /// <param name="e">Event data for the mouse down event.</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TransitionToMainWindow();
        }

        /// <summary>Handles the timer tick event to transition from the splash screen to the main window</summary>
        /// <param name="sender">The timer that triggered the event.</param>
        /// <param name="e">Event data for the timer tick.</param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            TransitionToMainWindow();
        }

        /// <summary>Transitions from the splash screen to the main window by stopping the timer, creating and showing the main window, and closing the splash screen</summary>
        private void TransitionToMainWindow()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
                _timer = null;
            }
            
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            
            Close();
        }
    }
}
