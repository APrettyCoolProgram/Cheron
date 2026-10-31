// 260201_code
// 260201_documentation

using System.Windows;

namespace Charon
{
    /// <summary>Advanced palette swapping with three-column workflow (Original ? In-Process ? Final).</summary>
    public partial class PaletteSwapper : Window
    {
        //    // Image buffers for workflow
        //    private WriteableBitmap? _originalBitmap;
        //    private WriteableBitmap? _inProcessBitmap;

        //    // Color selection state
        //    private Color? _pickedSourceColor;
        //    private Color? _replacementColor;

        //    // Eyedropper tool state
        //    private bool _isEyedropperActive = false;

        //    // Zoom state for each column
        //    private double _originalZoomScale = 1.0;
        //    private double _inProcessZoomScale = 1.0;

        //    private const double ZoomFactor = 1.25;
        //    private const double MaxZoom = 8.0;
        //    private const double MinScale = 1.0;

        //    // User zoom multipliers
        //    private double _userOriginalZoomFactor = 1.0;
        //    private double _userInProcessZoomFactor = 1.0;

        //    /// <summary>Create a new PaletteSwapper window and wire up UI handlers.</summary>
        //    public PaletteSwapper()
        //    {
        //        InitializeComponent();

        //        ToleranceSlider.ValueChanged += ToleranceSlider_ValueChanged;
        //    }

        //    /// <summary>Handle tolerance slider changes and update in real-time.</summary>
        //    private void ToleranceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //    {
        //        if (ToleranceValue != null)
        //            ToleranceValue.Text = $"{(int)e.NewValue}%";

        //        // Apply swap in real-time if both colors are selected
        //        if (_originalBitmap != null && _pickedSourceColor != null && _replacementColor != null)
        //        {
        //            ApplyColorSwap();
        //        }
        //    }

        //    /// <summary>Show an OpenFileDialog and load the chosen image into the Original column.</summary>
        //    private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        //    {
        //        var dialog = new OpenFileDialog
        //        {
        //            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*"
        //        };

        //        if (dialog.ShowDialog() != true)
        //            return;

        //        try
        //        {
        //            var bitmapImage = new BitmapImage(new Uri(dialog.FileName));
        //            _originalBitmap = new WriteableBitmap(bitmapImage);
        //            OriginalImage.Source = _originalBitmap;

        //            // Reset zoom
        //            this.Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                _userOriginalZoomFactor = 1.0;
        //                SetOriginalScale(_userOriginalZoomFactor);
        //            }), System.Windows.Threading.DispatcherPriority.Loaded);

        //            _pickedSourceColor = null;
        //            _replacementColor = null;
        //            _inProcessBitmap = null;

        //            InProcessImage.Source = null;

        //            EyedropColor.Background = Brushes.Transparent;
        //            ToColor.Background = Brushes.Transparent;

        //            UpdateButtonStates();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }

        //    /// <summary>Activate the eyedropper tool to pick a color from the original image.</summary>
        //    private void EyedropperButton_Click(object sender, RoutedEventArgs e)
        //    {
        //        if (_originalBitmap == null)
        //        {
        //            MessageBox.Show("Please load an image first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }

        //        _isEyedropperActive = true;
        //        OriginalImage.Cursor = Cursors.Cross;
        //    }

        //    /// <summary>Handle mouse click on original image to pick a color with eyedropper.</summary>
        //    private void OriginalImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //    {
        //        if (!_isEyedropperActive || _originalBitmap == null)
        //            return;

        //        Point position = e.GetPosition(OriginalImage);

        //        if (TryGetBitmapCoordinates(OriginalImage, _originalBitmap, position, out int bitmapX, out int bitmapY))
        //        {
        //            _pickedSourceColor = GetPixelColor(_originalBitmap, bitmapX, bitmapY);

        //            EyedropColor.Background = new SolidColorBrush(_pickedSourceColor.Value);

        //            _isEyedropperActive = false;
        //            OriginalImage.Cursor = Cursors.Arrow;

        //            // If target color is already selected, apply the swap immediately
        //            if (_replacementColor != null)
        //            {
        //                ApplyColorSwap();
        //            }

        //            UpdateButtonStates();
        //        }
        //    }

        //    /// <summary>Open color picker dialog to choose the replacement color.</summary>
        //    private void ChooseColorButton_Click(object sender, RoutedEventArgs e)
        //    {
        //        if (_pickedSourceColor == null)
        //        {
        //            MessageBox.Show("Please select a source color first using the eyedropper.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }

        //        var dialog = new ColorPickerDialog();
        //        if (dialog.ShowDialog() == true)
        //        {
        //            _replacementColor = dialog.SelectedColor;

        //            ToColor.Background = new SolidColorBrush(_replacementColor.Value);

        //            // Immediately apply the swap to show in-process image
        //            ApplyColorSwap();

        //            UpdateButtonStates();
        //        }
        //    }

        //    /// <summary>Perform the actual color swap operation.</summary>
        //    private void ApplyColorSwap()
        //    {
        //        if (_originalBitmap == null || _pickedSourceColor == null || _replacementColor == null)
        //            return;

        //        double tolerance = ToleranceSlider.Value / 100.0;

        //        _inProcessBitmap = _originalBitmap.Clone();
        //        SwapColors(_inProcessBitmap, _pickedSourceColor.Value, _replacementColor.Value, tolerance);
        //        InProcessImage.Source = _inProcessBitmap;

        //        //StatusText.Text = $"Color swap applied with {(int)ToleranceSlider.Value}% tolerance";

        //        UpdateButtonStates();
        //    }

        //    // MoveToFinalButton removed with Final column; saving uses in-process image directly.

        //    /// <summary>Save the final image to disk.</summary>
        //    private void SaveButton_Click(object sender, RoutedEventArgs e)
        //    {
        //        // Save the in-process image since that's the working image
        //        if (_inProcessBitmap == null)
        //            return;

        //        var dialog = new SaveFileDialog
        //        {
        //            Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
        //            DefaultExt = "png"
        //        };

        //        if (dialog.ShowDialog() != true)
        //            return;

        //        try
        //        {
        //            using var fileStream = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Create);

        //            BitmapEncoder encoder = dialog.FilterIndex switch
        //            {
        //                2 => new JpegBitmapEncoder(),
        //                3 => new BmpBitmapEncoder(),
        //                _ => new PngBitmapEncoder()
        //            };

        //            encoder.Frames.Add(BitmapFrame.Create(_inProcessBitmap));
        //            encoder.Save(fileStream);

        //            //StatusText.Text = $"Image saved to {dialog.FileName}";
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }

        //    /// <summary>Reset all images and state.</summary>
        //    private void ResetAllButton_Click(object sender, RoutedEventArgs e)
        //    {
        //        _originalBitmap = null;
        //        _inProcessBitmap = null;
        //        _pickedSourceColor = null;
        //        _replacementColor = null;

        //        OriginalImage.Source = null;
        //        InProcessImage.Source = null;

        //        EyedropColor.Background = Brushes.Transparent;
        //        ToColor.Background = Brushes.Transparent;

        //        UpdateButtonStates();
        //    }

        //    /// <summary>Update button enabled states based on current workflow state.</summary>
        //    private void UpdateButtonStates()
        //    {
        //        bool hasInProcess = _inProcessBitmap != null;

        //        if (SaveButton != null)
        //            SaveButton.IsEnabled = hasInProcess;
        //    }

        //    /// <summary>Get a pixel color from a bitmap at the specified coordinates.</summary>
        //    private Color GetPixelColor(WriteableBitmap bitmap, int x, int y)
        //    {
        //        bitmap.Lock();
        //        try
        //        {
        //            unsafe
        //            {
        //                IntPtr pBackBuffer = bitmap.BackBuffer;
        //                int stride = bitmap.BackBufferStride;
        //                byte* pPixel = (byte*)pBackBuffer + y * stride + x * 4;

        //                byte b = pPixel[0];
        //                byte g = pPixel[1];
        //                byte r = pPixel[2];
        //                byte a = pPixel[3];

        //                return Color.FromArgb(a, r, g, b);
        //            }
        //        }
        //        finally
        //        {
        //            bitmap.Unlock();
        //        }
        //    }

        //    /// <summary>Replace pixels close to a source color with the target color within a tolerance.</summary>
        //    private void SwapColors(WriteableBitmap bitmap, Color sourceColor, Color targetColor, double tolerance)
        //    {
        //        bitmap.Lock();
        //        try
        //        {
        //            unsafe
        //            {
        //                IntPtr pBackBuffer = bitmap.BackBuffer;
        //                int stride = bitmap.BackBufferStride;
        //                int width = bitmap.PixelWidth;
        //                int height = bitmap.PixelHeight;

        //                for (int y = 0; y < height; y++)
        //                {
        //                    byte* row = (byte*)pBackBuffer + y * stride;
        //                    for (int x = 0; x < width; x++)
        //                    {
        //                        byte* pixel = row + x * 4;

        //                        byte b = pixel[0];
        //                        byte g = pixel[1];
        //                        byte r = pixel[2];

        //                        double distance = ColorDistance(r, g, b, sourceColor.R, sourceColor.G, sourceColor.B);
        //                        double maxDistance = 255 * Math.Sqrt(3);
        //                        double normalizedDistance = distance / maxDistance;

        //                        if (normalizedDistance <= tolerance)
        //                        {
        //                            double ratio = 1.0 - (normalizedDistance / tolerance);

        //                            int newR = (int)(r + (targetColor.R - sourceColor.R) * ratio);
        //                            int newG = (int)(g + (targetColor.G - sourceColor.G) * ratio);
        //                            int newB = (int)(b + (targetColor.B - sourceColor.B) * ratio);

        //                            pixel[0] = (byte)Math.Clamp(newB, 0, 255);
        //                            pixel[1] = (byte)Math.Clamp(newG, 0, 255);
        //                            pixel[2] = (byte)Math.Clamp(newR, 0, 255);
        //                        }
        //                    }
        //                }
        //            }

        //            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
        //        }
        //        finally
        //        {
        //            bitmap.Unlock();
        //        }
        //    }

        //    /// <summary>Compute Euclidean distance between two RGB colors.</summary>
        //    private double ColorDistance(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        //    {
        //        int dr = r1 - r2;
        //        int dg = g1 - g2;
        //        int db = b1 - b2;
        //        return Math.Sqrt(dr * dr + dg * dg + db * db);
        //    }

        //    /// <summary>Translate a mouse position over the Image control to bitmap pixel coordinates.</summary>
        //    private bool TryGetBitmapCoordinates(Image imageControl, WriteableBitmap bitmap, Point position, out int bx, out int by)
        //    {
        //        bx = 0;
        //        by = 0;
        //        if (bitmap == null || imageControl.Source == null)
        //            return false;

        //        double imgWidth = imageControl.ActualWidth;
        //        double imgHeight = imageControl.ActualHeight;
        //        if (imgWidth <= 0 || imgHeight <= 0)
        //            return false;

        //        double bmpW = bitmap.PixelWidth;
        //        double bmpH = bitmap.PixelHeight;

        //        double scale = Math.Min(imgWidth / bmpW, imgHeight / bmpH);
        //        double displayW = bmpW * scale;
        //        double displayH = bmpH * scale;

        //        double offsetX = (imgWidth - displayW) / 2.0;
        //        double offsetY = (imgHeight - displayH) / 2.0;

        //        double px = (position.X - offsetX) / scale;
        //        double py = (position.Y - offsetY) / scale;

        //        int ix = (int)Math.Floor(px);
        //        int iy = (int)Math.Floor(py);

        //        if (ix < 0 || iy < 0 || ix >= bitmap.PixelWidth || iy >= bitmap.PixelHeight)
        //            return false;

        //        bx = ix;
        //        by = iy;
        //        return true;
        //    }

        //    #region Zoom Controls - Original Column

        //    /// <summary>Increase zoom for the original image.</summary>
        //    // Removed plus/minus button handlers; using slider instead.

        //    /// <summary>Handle slider changes for original image zoom.</summary>
        //    private void OriginalZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //    {
        //        // Map slider value directly to scale (slider min/max = 1..8)
        //        double newScale = Math.Clamp(e.NewValue, MinScale, MaxZoom);
        //        _userOriginalZoomFactor = newScale / MinScale;
        //        SetOriginalScale(newScale);
        //    }

        //    /// <summary>Reset the original image zoom back to fit-to-view.</summary>
        //    private void OrigFit_Click(object sender, RoutedEventArgs e)
        //    {
        //        _userOriginalZoomFactor = 1.0;
        //        SetOriginalScale(MinScale * _userOriginalZoomFactor);

        //        // Reset any zoom sliders to the fit value
        //        ResetZoomSliders();
        //    }

        //    /// <summary>Apply zoom scale to original image.</summary>
        //    private void SetOriginalScale(double scale)
        //    {
        //        _originalZoomScale = Math.Clamp(scale, MinScale, MaxZoom);

        //        if (OriginalImage.LayoutTransform is ScaleTransform st)
        //        {
        //            st.ScaleX = _originalZoomScale;
        //            st.ScaleY = _originalZoomScale;
        //        }
        //        else
        //        {
        //            OriginalImage.LayoutTransform = new ScaleTransform(_originalZoomScale, _originalZoomScale);
        //        }
        //    }

        //    /// <summary>Handle mouse wheel zoom for original image.</summary>
        //    private void OriginalScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //    {
        //        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //        {
        //            var sv = OriginalScrollViewer;
        //            Point mousePos = e.GetPosition(sv);
        //            double prev = _originalZoomScale;
        //            double newUserZoom = e.Delta > 0 ? _userOriginalZoomFactor * ZoomFactor : _userOriginalZoomFactor / ZoomFactor;
        //            _userOriginalZoomFactor = Math.Clamp(newUserZoom, MinScale / Math.Max(MinScale, 1e-6), MaxZoom / Math.Max(MinScale, 1e-6));
        //            double newScale = Math.Clamp(MinScale * _userOriginalZoomFactor, MinScale, MaxZoom);

        //            double relativeX = mousePos.X + sv.HorizontalOffset;
        //            double relativeY = mousePos.Y + sv.VerticalOffset;

        //            SetOriginalScale(newScale);

        //            double scaleRatio = newScale / prev;
        //            sv.ScrollToHorizontalOffset((relativeX * scaleRatio) - mousePos.X);
        //            sv.ScrollToVerticalOffset((relativeY * scaleRatio) - mousePos.Y);
        //            e.Handled = true;
        //        }
        //    }

        //    #endregion

        //    #region Zoom Controls - In-Process Column

        //    /// <summary>Increase zoom for the in-process image.</summary>
        //    private void InProcessZoomIn_Click(object sender, RoutedEventArgs e)
        //    {
        //        _userInProcessZoomFactor *= ZoomFactor;
        //        SetInProcessScale(MinScale * _userInProcessZoomFactor);
        //    }

        //    /// <summary>Decrease zoom for the in-process image.</summary>
        //    private void InProcessZoomOut_Click(object sender, RoutedEventArgs e)
        //    {
        //        _userInProcessZoomFactor /= ZoomFactor;
        //        SetInProcessScale(MinScale * _userInProcessZoomFactor);
        //    }

        //    /// <summary>Reset the in-process image zoom back to fit-to-view.</summary>
        //    private void InProcessFit_Click(object sender, RoutedEventArgs e)
        //    {
        //        _userInProcessZoomFactor = 1.0;
        //        SetInProcessScale(MinScale * _userInProcessZoomFactor);

        //        // Reset any zoom sliders to the fit value
        //        ResetZoomSliders();
        //    }

        //    /// <summary>Reset zoom-related sliders (if present) to the fit-to-view value.</summary>
        //    private void ResetZoomSliders()
        //    {
        //        // Original zoom slider
        //        if (OriginalZoomSlider != null)
        //        {
        //            OriginalZoomSlider.Value = MinScale;
        //        }

        //        // In-process zoom slider (optional; may not exist)
        //        if (this.FindName("InProcessZoomSlider") is Slider inSlider)
        //        {
        //            inSlider.Value = MinScale;
        //        }
        //    }

        //    /// <summary>Apply zoom scale to in-process image.</summary>
        //    private void SetInProcessScale(double scale)
        //    {
        //        _inProcessZoomScale = Math.Clamp(scale, MinScale, MaxZoom);

        //        if (InProcessImage.LayoutTransform is ScaleTransform st)
        //        {
        //            st.ScaleX = _inProcessZoomScale;
        //            st.ScaleY = _inProcessZoomScale;
        //        }
        //        else
        //        {
        //            InProcessImage.LayoutTransform = new ScaleTransform(_inProcessZoomScale, _inProcessZoomScale);
        //        }
        //    }

        //    /// <summary>Handle mouse wheel zoom for in-process image.</summary>
        //    private void InProcessScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //    {
        //        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //        {
        //            var sv = InProcessScrollViewer;
        //            Point mousePos = e.GetPosition(sv);
        //            double prev = _inProcessZoomScale;
        //            double newUserZoom = e.Delta > 0 ? _userInProcessZoomFactor * ZoomFactor : _userInProcessZoomFactor / ZoomFactor;
        //            _userInProcessZoomFactor = Math.Clamp(newUserZoom, MinScale / Math.Max(MinScale, 1e-6), MaxZoom / Math.Max(MinScale, 1e-6));
        //            double newScale = Math.Clamp(MinScale * _userInProcessZoomFactor, MinScale, MaxZoom);

        //            double relativeX = mousePos.X + sv.HorizontalOffset;
        //            double relativeY = mousePos.Y + sv.VerticalOffset;

        //            SetInProcessScale(newScale);

        //            double scaleRatio = newScale / prev;
        //            sv.ScrollToHorizontalOffset((relativeX * scaleRatio) - mousePos.X);
        //            sv.ScrollToVerticalOffset((relativeY * scaleRatio) - mousePos.Y);
        //            e.Handled = true;
        //        }
        //    }

        //    #endregion

        //    // Final column removed; related handlers and fields have been dropped.
        //}

        ///// <summary>Converts null to Visibility.Visible and non-null to Visibility.Collapsed.</summary>
        //public class NullToVisibilityConverter : IValueConverter
        //{
        //    public static NullToVisibilityConverter Instance { get; } = new NullToVisibilityConverter();

        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        return value == null ? Visibility.Visible : Visibility.Collapsed;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
