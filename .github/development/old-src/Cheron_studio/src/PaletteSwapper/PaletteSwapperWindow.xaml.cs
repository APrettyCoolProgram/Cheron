// 260309_code
// 260309_documentation

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Cheron.PaletteSwapper;

/// <summary>Entry class for Palette Swapper.</summary>
public partial class PaletteSwapperWindow : Window
{
    /// <summary>Responsible for handling zoom and pan functionality for the original image.</summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>Allows users to interactively zoom in and out of the image and pan it within the container.</item>
    ///         <item>ImageZoom class raises events when the zoom level changes, which are handled to update the UI accordingly.</item>
    ///         <item>Initialized in the StartApplet method and used throughout the class to manage image interactions.</item>
    ///     </list>
    /// </remarks>
    private ImageZoom? _imageZoom;

    /// <summary>Flag used to prevent recursive updates between the zoom slider and the ImageZoom control.</summary>
    /// <remarks>
    ///     When the zoom level is changed programmatically (e.g., via the ImageZoom control), this flag is set to true
    ///     to indicate that the slider is being updated as a result of a zoom change event. This prevents the
    ///     OriginalZoomValueChanged event handler from responding to changes that are not user-initiated, avoiding
    ///     potential infinite loops or unintended side effects. Once the slider update is complete, the flag is reset
    ///     to false, allowing user interactions with the slider to function normally.
    /// </remarks>
    private bool _isUpdatingSlider;

    /// <summary>The currently selected "from" color for palette swapping.</summary>
    private Color? _fromColor;

    /// <summary>Entry method for the Palette Swapper.</summary>
    public PaletteSwapperWindow()
    {
        InitializeComponent();

        StartApplet();
    }

    /// <summary>Start logic for Palette Swapper.</summary>
    private void StartApplet()
    {
        InitializeImageZoom();
    }

    /// <summary>Initialize the image zoom functionality.</summary>
    private void InitializeImageZoom()
    {
        _imageZoom              = new ImageZoom(imgOriginal, brdrOriginalImage);
        _imageZoom.ZoomChanged += OnZoomChanged;
    }

    /// <summary>Zoom slider is moved.</summary>
    /// <param name="sender">The source of the zoom change event.</param>
    /// <param name="zoom">The new zoom level to apply.</param>
    private void OnZoomChanged(object? sender, double zoom)
    {
        _isUpdatingSlider              = true;
        sldrOriginalZoom.Value         = zoom;
        lblOriginalZoomPercent.Content = $"{(int)(zoom * 100)}%";
        _isUpdatingSlider              = false;
    }

    /// <summary>When the user clicks the Load Original Image button.</summary>
    /// <remarks>This method opens a file dialog to select an image file and loads it into the original image control.</remarks>
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

    /// <summary>Opens a color picker dialog and stores the selected "from" color.</summary>
    /// <remarks>Updates <c>btnFromColor</c> background to reflect the chosen color.</remarks>
    private void ChooseFromColor()
    {
        using var dialog = new System.Windows.Forms.ColorDialog { FullOpen = true };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var c                   = dialog.Color;
            _fromColor              = Color.FromArgb(c.A, c.R, c.G, c.B);
            btnFromColor.Background = new SolidColorBrush(_fromColor.Value);
        }
    }

    /* EVENTS */

    /// <summary>Zoom value has been changed.</summary>
    /// <param name="sender">The source of the zoom change event.</param>
    /// <param name="eventArgs">The event arguments containing the new zoom value.</param>
    private void OriginalZoomValueChanged(object sender, RoutedPropertyChangedEventArgs<double> eventArgs)
    {
        if (_isUpdatingSlider || _imageZoom == null)
        {
            return;
        }

        _imageZoom.SetZoom(eventArgs.NewValue);
    }



    /* EVENT HANDLERS*/
    private void ResetOriginalZoomClicked() => _imageZoom.ResetZoom();
    private void btnLoadOriginalImage_Click(object sender, RoutedEventArgs e) => LoadOriginalImageClicked();
    private void sldrOriginalZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> eventArgs) => OriginalZoomValueChanged(sender, eventArgs);
    private void btnResetOriginalZoom_Click(object sender, RoutedEventArgs e) => ResetOriginalZoomClicked();
    private void btnFromColor_Click(object sender, RoutedEventArgs e) => ChooseFromColor();

    private void btnToColor_Click(object sender, RoutedEventArgs e)
    {

    }
}