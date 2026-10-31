// 260201_code
// 260201_documentation

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Charon
{
    /// <summary>Dialog that allows the user to pick an RGB color using sliders and presets.</summary>
    public partial class ColorPickerDialog : Window
    {
        /// <summary>The color selected by the user.</summary>
        public Color SelectedColor { get; private set; }

        /// <summary>Initialize the dialog and update the preview to the current slider values.</summary>
        public ColorPickerDialog()
        {
            InitializeComponent();
            UpdateColorPreview();
        }

        /// <summary>Handle slider changes and update numeric labels and the preview color.</summary>
        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RedValue == null || GreenValue == null || BlueValue == null)
            {
                return;
            }

            RedValue.Text   = ((int)RedSlider.Value).ToString();
            GreenValue.Text = ((int)GreenSlider.Value).ToString();
            BlueValue.Text  = ((int)BlueSlider.Value).ToString();

            UpdateColorPreview();
        }

        /// <summary>Update the color preview from the slider values and store the selection.</summary>
        private void UpdateColorPreview()
        {
            byte red   = (byte)RedSlider.Value;
            byte green = (byte)GreenSlider.Value;
            byte blue  = (byte)BlueSlider.Value;

            SelectedColor = Color.FromRgb(red, green, blue);

            ColorPreview.Background = new SolidColorBrush(SelectedColor);
        }

        /// <summary>Apply a preset color to the sliders when a preset button is clicked.</summary>
        private void PresetColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not string colorHex)
            {
                return;
            }

            Color color = (Color)ColorConverter.ConvertFromString(colorHex);

            RedSlider.Value   = color.R;
            GreenSlider.Value = color.G;
            BlueSlider.Value  = color.B;
        }

        /// <summary>Confirm the selection and close the dialog returning true.</summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }

        /// <summary>Cancel the dialog and close returning false.</summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}