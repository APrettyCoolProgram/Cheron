using System.Configuration;
using System.Data;
using System.Windows;

namespace Ubiquitus
{
    /// <summary>
    /// Represents the main application class for the Ubiquitus WPF application.
    /// Handles application-level events and initialization.
    /// </summary>
    /// <remarks>
    /// This class serves as the entry point for the WPF application and manages
    /// the application lifecycle, including startup, shutdown, and unhandled exception handling.
    /// </remarks>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event.
        /// Displays the splash screen before showing the main window.
        /// </summary>
        /// <param name="e">The startup event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Show splash screen
            var splashWindow = new SplashWindow();
            splashWindow.Show();

            // Create main window but don't show it yet
            var mainWindow = new MainWindow();

            // When splash closes, show main window
            splashWindow.Closed += (sender, args) =>
            {
                mainWindow.Show();
            };
        }
    }
}
