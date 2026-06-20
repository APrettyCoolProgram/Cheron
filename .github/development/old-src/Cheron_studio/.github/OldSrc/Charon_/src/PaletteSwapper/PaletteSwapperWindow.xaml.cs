// 260207_code
// 260207_documentation

using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Charon.PaletteSwapper;
/// <summary>
/// Interaction logic for PaletteSwapperWindow.xaml
/// </summary>
public partial class PaletteSwapperWindow : Window
{
    public PaletteSwapperWindow()
    {
        InitializeComponent();
    }

    /*
     * EVENTS
     */

    private void LoadOriginalImageClicked()
    {
        var openOriginalImage = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
            Title  = "Select an Image File"
        };

        if (openOriginalImage.ShowDialog() == true)
        {
            try
            {
                imgOriginal.Source = new BitmapImage(new Uri(openOriginalImage.FileName));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /*
     * EVENT HANDLERS
     */

    /// <summary>
    /// Handle the Load button click event to load an image file
    /// </summary>
    private void btnLoadOriginalImage_Click(object sender, RoutedEventArgs e) => LoadOriginalImageClicked();
}