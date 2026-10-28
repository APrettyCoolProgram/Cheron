using System.Windows;
using System.Windows.Threading;

namespace Ubiquitus
{
    /// <summary>
    /// Splash screen window displayed on application startup
    /// </summary>
    /// <remarks>
    /// Displays the Ubiquitus logo (ubiquitus-v1_1024x1024.png) for 3 seconds
    /// before automatically closing. The splash screen provides a professional
    /// startup experience and allows time for application initialization.
    /// </remarks>
    public partial class SplashScreen : Window
    {
        /// <summary>
        /// Initializes a new instance of the SplashScreen class
        /// </summary>
        /// <remarks>
        /// Sets up a DispatcherTimer that automatically closes the splash screen
        /// after 3 seconds. The timer is started immediately upon construction.
        /// The window is borderless, centered on screen, and displays the application logo.
        /// </remarks>
        public SplashScreen()
        {
            InitializeComponent();
            
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                Close();
            };
            timer.Start();
        }
    }
}
