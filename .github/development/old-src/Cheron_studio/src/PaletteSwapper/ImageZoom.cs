// 260309_code
// 260309_documentation

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cheron.PaletteSwapper;

/// <summary>Provides zooming and panning for an <see cref="Image"/> inside a <see cref="Border"/>.</summary>
internal class ImageZoom
{
    /// <summary>The image control that will be zoomed and panned.</summary>
    private readonly Image _image;

    /// <summary>The container that holds the image and constrains panning.</summary>
    private readonly Border _container;

    /// <summary>The scale transform applied to the image for zooming.</summary>
    private ScaleTransform? _scaleTransform;

    /// <summary>The translate transform applied to the image for panning.</summary>
    private TranslateTransform? _translateTransform;

    /// <summary>The origin of the current pan operation.</summary>
    private Point _origin;

    /// <summary>The pointer start position for a pan operation.</summary>
    private Point _start;

    /// <summary>Indicates whether a pan operation is currently active.</summary>
    private bool _isPanning;

    /// <summary>The current zoom factor.</summary>
    private double _currentZoom = 1.0;

    /// <summary>Minimum allowed zoom factor (<c>1.0</c>).</summary>
    private const double ZoomMin = 1.0;

    /// <summary>Maximum allowed zoom factor (<c>3.0</c>).</summary>
    private const double ZoomMax = 3.0;

    /// <summary>Increment used when zooming with the mouse wheel (<c>0.1</c>).</summary>
    private const double ZoomStep = 0.1;

    /// <summary>Raised when the zoom factor changes.</summary>
    /// <remarks>
    /// The <c>double</c> event argument contains the new zoom factor.
    /// </remarks>
    public event EventHandler<double>? ZoomChanged;

    /// <summary>Initializes a new instance of the <see cref="ImageZoom"/> class.</summary>
    /// <param name="image">The <see cref="Image"/> control to enable zooming for.</param>
    /// <param name="container">The <see cref="Border"/> that constrains panning and receives mouse input.</param>
    /// <remarks>Sets up the image and container references and initializes event handlers.</remarks>
    public ImageZoom(Image image, Border container)
    {
        _image     = image;
        _container = container;

        Initialize();
    }

    /// <summary>Initializes transforms and wires up mouse event handlers for the image.</summary>
    /// <remarks>Called by the constructor to set up transforms and mouse event subscriptions.</remarks>
    private void Initialize()
    {
        _scaleTransform     = new ScaleTransform();
        _translateTransform = new TranslateTransform();

        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(_scaleTransform);
        transformGroup.Children.Add(_translateTransform);

        _image.RenderTransform       = transformGroup;
        _image.RenderTransformOrigin = new Point(0.5, 0.5);

        _image.MouseWheel          += OnMouseWheel;
        _image.MouseLeftButtonDown += OnMouseLeftButtonDown;
        _image.MouseLeftButtonUp   += OnMouseLeftButtonUp;
        _image.MouseMove           += OnMouseMove;
        _image.MouseLeave          += OnMouseLeave;
    }

    /// <summary>Handles mouse wheel events to adjust the zoom factor.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="mWheelEventArgs">The mouse wheel event arguments.</param>
    /// <remarks>Increases or decreases the zoom level based on the mouse wheel delta.</remarks>
    private void OnMouseWheel(object sender, MouseWheelEventArgs mWheelEventArgs)
    {
        if (_scaleTransform == null)
        {
            return;
        }

        double zoom = mWheelEventArgs.Delta > 0
            ? ZoomStep
            : -ZoomStep;

        double newZoom = _currentZoom + zoom;

        newZoom = Math.Max(ZoomMin, Math.Min(ZoomMax, newZoom));

        if (Math.Abs(newZoom - _currentZoom) < 0.001)
        {
            return;
        }

        _currentZoom           = newZoom;
        _scaleTransform.ScaleX = _currentZoom;
        _scaleTransform.ScaleY = _currentZoom;

        ConstrainPan();
        ZoomChanged?.Invoke(this, _currentZoom);

        mWheelEventArgs.Handled = true;
    }

    /// <summary>Begins a pan operation when the left mouse button is pressed.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="mButtonEventArg">The mouse button event arguments.</param>
    /// <remarks>Captures the mouse and records the starting position for panning.</remarks>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs mButtonEventArg)
    {
        if (_image.IsMouseCaptured)
        {
            return;
        }

        _image.CaptureMouse();
        _start        = mButtonEventArg.GetPosition(_container);
        _origin       = new Point(_translateTransform.X, _translateTransform.Y);
        _isPanning    = true;
        _image.Cursor = Cursors.Hand;
    }

    /// <summary>Ends a pan operation when the left mouse button is released.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="mButtonEventArg">The mouse button event arguments.</param>
    /// <remarks>Releases the mouse capture and ends the panning operation.</remarks>
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mButtonEventArg)
    {
        if (!_image.IsMouseCaptured)
        {
            return;
        }

        _image.ReleaseMouseCapture();
        _isPanning    = false;
        _image.Cursor = Cursors.Arrow;
    }

    /// <summary>Handles mouse movement during a pan operation to translate the image.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="mEventArg">The mouse event arguments.</param>
    /// <remarks>Updates the image translation based on mouse movement while panning.</remarks>
    private void OnMouseMove(object sender, MouseEventArgs mEventArg)
    {
        if (!_isPanning || _translateTransform == null)
        {
            return;
        }

        var currentPosition = mEventArg.GetPosition(_container);
        var delta           = currentPosition - _start;

        _translateTransform.X = _origin.X + delta.X;
        _translateTransform.Y = _origin.Y + delta.Y;

        ConstrainPan();
    }

    /// <summary>Cancels an active pan operation when the mouse leaves the image area.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="mEventArg">The mouse event arguments.</param>
    /// <remarks>Ensures panning is stopped if the mouse leaves the image region.</remarks>
    private void OnMouseLeave(object sender, MouseEventArgs mEventArg)
    {
        if (_isPanning)
        {
            _image.ReleaseMouseCapture();
            _isPanning    = false;
            _image.Cursor = Cursors.Arrow;
        }
    }

    /// <summary>Constrains the translation so the image cannot be panned beyond the visible bounds.</summary>
    /// <remarks>Keeps the image centered and prevents panning outside the container.</remarks>
    private void ConstrainPan()
    {
        if (_translateTransform == null || _scaleTransform == null)
        {
            return;
        }

        var scaledWidth     = _image.ActualWidth * _currentZoom;
        var scaledHeight    = _image.ActualHeight * _currentZoom;
        var containerWidth  = _container.ActualWidth;
        var containerHeight = _container.ActualHeight;

        var maxOffsetX = Math.Max(0, (scaledWidth - containerWidth) / 2);
        var maxOffsetY = Math.Max(0, (scaledHeight - containerHeight) / 2);

        _translateTransform.X = Math.Max(-maxOffsetX, Math.Min(maxOffsetX, _translateTransform.X));
        _translateTransform.Y = Math.Max(-maxOffsetY, Math.Min(maxOffsetY, _translateTransform.Y));
    }

    /// <summary>Resets the zoom factor and translation to the default (no zoom, centered).</summary>
    /// <remarks>Sets zoom to 1.0 and centers the image in the container.</remarks>
    public void ResetZoom()
    {
        if (_scaleTransform == null || _translateTransform == null)
        {
            return;
        }

        _currentZoom           = 1.0;
        _scaleTransform.ScaleX = 1.0;
        _scaleTransform.ScaleY = 1.0;
        _translateTransform.X  = 0;
        _translateTransform.Y  = 0;

        ZoomChanged?.Invoke(this, _currentZoom);
    }

    /// <summary>Gets the current zoom factor.</summary>
    /// <remarks>Returns the current zoom level applied to the image.</remarks>
    /// <returns>The current zoom factor.</returns>
    public double GetCurrentZoom() => _currentZoom;

    /// <summary>Sets the zoom factor to the specified value, clamped to the allowed range.</summary>
    /// <param name="zoom">The desired zoom factor; value will be clamped to the interval defined by <c>ZoomMin</c> and <c>ZoomMax</c>.</param>
    /// <remarks>Adjusts the zoom and clamps it to the allowed range, updating the image transform.</remarks>
    public void SetZoom(double zoom)
    {
        if (_scaleTransform == null)
        {
            return;
        }

        zoom                   = Math.Max(ZoomMin, Math.Min(ZoomMax, zoom));
        _currentZoom           = zoom;
        _scaleTransform.ScaleX = zoom;
        _scaleTransform.ScaleY = zoom;
        ConstrainPan();

        ZoomChanged?.Invoke(this, _currentZoom);
    }
}