using System.Windows;

namespace Ubiquitus
{
    /// <summary>
    /// Interaction logic for App.xaml - Main application class
    /// </summary>
    /// <remarks>
    /// Handles the application startup sequence, including displaying the splash screen
    /// before showing the main window.
    /// </remarks>
    public partial class App : Application
    {
        /// <summary>
        /// Handles the application startup event
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Startup event arguments</param>
        /// <remarks>
        /// This method is called when the application starts. It creates and displays
        /// the splash screen, then waits for it to close before showing the main window.
        /// The splash screen displays for 3 seconds (configured in SplashScreen.xaml.cs).
        /// </remarks>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var splashScreen = new SplashScreen();
            splashScreen.Show();
            
            splashScreen.Closed += (s, args) =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            };
        }
    }
}
