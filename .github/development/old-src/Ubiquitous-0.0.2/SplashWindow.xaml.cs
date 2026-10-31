using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ubiquitus
{
    /// <summary>
    /// Represents the splash screen window displayed during application startup.
    /// Shows the application logo and version information.
    /// </summary>
    /// <remarks>
    /// The splash screen automatically closes after a set duration or when clicked by the user.
    /// Version information is retrieved from the assembly attributes.
    /// </remarks>
    public partial class SplashWindow : Window
    {
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashWindow"/> class.
        /// Sets up the splash screen with version information and auto-close timer.
        /// </summary>
        public SplashWindow()
        {
            InitializeComponent();
            SetVersionNumber();
            
            // Auto-close after 7 seconds
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(7)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Sets the version number text from the assembly version.
        /// </summary>
        private void SetVersionNumber()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
            {
                VersionText.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";
            }
        }

        /// <summary>
        /// Handles the timer tick event to close the splash screen automatically.
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
            Close();
        }

        /// <summary>
        /// Handles mouse down events to allow users to close the splash screen by clicking.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The mouse button event arguments.</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
            Close();
        }
    }
}
