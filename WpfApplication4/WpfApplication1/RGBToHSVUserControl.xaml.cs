// RGBToHSVUserControl.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication1
{
    public partial class RGBToHSVUserControl : UserControl
    {
        public RGBToHSVUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(RGBToHSVUserControl), new PropertyMetadata(Colors.Black));

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RedTextBox.Text, out int red) && int.TryParse(GreenTextBox.Text, out int green) && int.TryParse(BlueTextBox.Text, out int blue))
            {
                double hue, saturation, value;
                RGBToHSV(red, green, blue, out hue, out saturation, out value);

                HsvResultText.Text = $"HSV: Hue={hue:F2}, Saturation={saturation:F2} \n Value={value:F2}";
                Color rgbColor = Color.FromRgb((byte)red, (byte)green, (byte)blue);
                SelectedColor = rgbColor;

                if (Application.Current.MainWindow != null)
                {
                    Button colorChangeButton = (Button)Application.Current.MainWindow.FindName("ColorChangeButton");
                    if (colorChangeButton != null)
                    {
                        colorChangeButton.Background = new SolidColorBrush(rgbColor);
                    }
                }
            }
            else
            {
                HsvResultText.Text = "Invalid input. Please enter valid numeric values.";
            }
        }

        private void RGBToHSV(int red, int green, int blue, out double hue, out double saturation, out double value)
        {
            double R = red / 255.0;
            double G = green / 255.0;
            double B = blue / 255.0;
            double Cmax = Math.Max(Math.Max(R, G), B);
            double Cmin = Math.Min(Math.Min(R, G), B);
            double delta = Cmax - Cmin;

            if (delta == 0)
            {
                hue = 0;
            }
            else if (Cmax == R)
            {
                hue = 60 * ((G - B) / delta % 6);
            }
            else if (Cmax == G)
            {
                hue = 60 * ((B - R) / delta + 2);
            }
            else
            {
                hue = 60 * ((R - G) / delta + 4);
            }

            hue = (hue + 360) % 360;
            saturation = (Cmax == 0) ? 0 : (delta / Cmax);
            value = Cmax;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }


        private void NumericInputOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text + e.Text, out int value))
            {
                if (textBox.Text == "0")
                {
                    e.Handled = true;
                    textBox.Text = e.Text;
                    textBox.CaretIndex = textBox.Text.Length;
                }
                else 
                {
                    e.Handled = value < 0 || value >= 256;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}