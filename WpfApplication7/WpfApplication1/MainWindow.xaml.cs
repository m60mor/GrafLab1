using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Emgu;
using EmguCV = Emgu.CV;
using EmguStruct = Emgu.CV.Structure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Emit;
using Emgu.CV.Flann;

namespace WpfApplication1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string desktopPath = "";
        private string drawingMethod = "";
        private int toolboxHeight = 100;
        private int brushSize = 5;

        private Shapes shapes;
        private Lines lines;

        private ObservableCollection<Canvas> layers = new ObservableCollection<Canvas>();
        public ObservableCollection<Canvas> Layers
        {
            get { return layers; }
            set
            {
                layers = value;
                OnPropertyChanged(nameof(Layers));
                OnPropertyChanged(nameof(LayerIndexes));
            }
        }
        public List<int> LayerIndexes => Layers.Select((canvas, index) => index + 1).ToList();
        private Canvas currentLayer;
        private int layerIndex = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {       
            InitializeComponent();
            AddLayer();
            drawingMethod = "brush";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(baseDirectory))));
            string imagePath = System.IO.Path.Combine(projectDirectory, "images", "brush.png");
            
            EmguCV.Image<EmguStruct.Gray, byte> img = new EmguCV.Image<EmguStruct.Gray, byte>(imagePath);
            EmguCV.Image<EmguStruct.Gray, byte> customImg = ApplyLinearFilter(img);
            //EmguCV.CvInvoke.Imshow("Image", customImg);
            //EmguCV.CvInvoke.WaitKey(0);

            shapes = new Shapes(currentLayer, new SolidColorBrush(Colors.Black));
            lines = new Lines(currentLayer, new SolidColorBrush(Colors.Black), brushSize);

            DataContext = this;
        }

        private void LayerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLayer = layers[comboBoxLayers.SelectedIndex];
            lines.updateLinesLayer(currentLayer);
            shapes.updateShapesLayer(currentLayer);
        }

        private void AddLayer()
        {
            Canvas newLayer = new Canvas();
            newLayer.Background = new SolidColorBrush(Colors.Transparent);
            Layers.Add(newLayer);
            paintSurface.Children.Add(newLayer);
            currentLayer = newLayer;

            Layers = new ObservableCollection<Canvas>(Layers);
            comboBoxLayers.SelectedIndex = Layers.Count - 1;
        }

        private void CreateNewLayer(object sender, RoutedEventArgs e)
        {
            AddLayer();
        }

        private void RemoveLayer(object sender, RoutedEventArgs e)
        {
            if (Layers.Count > 1)
            {
                paintSurface.Children.Remove(layers.ElementAt(comboBoxLayers.SelectedIndex));
                layers.RemoveAt(comboBoxLayers.SelectedIndex);
                comboBoxLayers.SelectedIndex = 0;
                Layers = new ObservableCollection<Canvas>(Layers);
            }
        }

        private void ToggleLayers(object sender, RoutedEventArgs e)
        {
            layerIndex = comboBoxLayers.SelectedIndex;
            if (layers.ElementAt(layerIndex).Visibility == Visibility.Visible)
            {
                layers.ElementAt(layerIndex).Visibility = Visibility.Hidden;
            }
            else
            {
                layers.ElementAt(layerIndex).Visibility = Visibility.Visible;
            }
        }

        private EmguCV.Image<EmguStruct.Gray, byte> ApplyLinearFilter(EmguCV.Image<EmguStruct.Gray, byte> inputImage)
        {
            float[,] linearFilterMatrix = new float[,]
            {
        { 2, 2, 2 },
        { 1, 1, 2 },
        { 1, 2, 1 }
            };
            float factor = 1.0f / 9.0f;
            for (int i = 0; i < linearFilterMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < linearFilterMatrix.GetLength(1); j++)
                {
                    linearFilterMatrix[i, j] *= factor;
                }
            }
            EmguCV.ConvolutionKernelF convolutionKernel = new EmguCV.ConvolutionKernelF(linearFilterMatrix);

            EmguCV.Image<EmguStruct.Gray, byte> resultImage = new EmguCV.Image<EmguStruct.Gray, byte>(inputImage.Size);
            EmguCV.CvInvoke.Filter2D(inputImage, resultImage, convolutionKernel, new System.Drawing.Point(-1, -1));

            return resultImage;
        }

        private void Rgb_To_Hsv(object sender, RoutedEventArgs e)
        {
            if (rgbToHsvUserControl.Visibility == Visibility.Collapsed)
            {
                rgbToHsvUserControl.Visibility = Visibility.Visible;
            }
            else
            {
                rgbToHsvUserControl.Visibility = Visibility.Collapsed;
            }
        }

        //Save and load
        private void Save(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)paintSurface.ActualWidth, (int)paintSurface.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(paintSurface);

            var crop = new CroppedBitmap(renderBitmap, new Int32Rect(0, 100, (int)paintSurface.ActualWidth, (int)(paintSurface.ActualHeight - 100)));
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(crop));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Nie można zapisać pliku: {ex.Message}");
                }
            }
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Image loadedImage = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                try
                {
                    using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = fileStream;
                        bitmapImage.EndInit();
                    }

                    loadedImage.Source = bitmapImage;
                    currentLayer.Children.Add(loadedImage);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Nie można wczytać pliku: {ex.Message}");
                }
            }
        }

        //brush options
        private void Change_Color(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                SolidColorBrush buttonBrush = clickedButton.Background as SolidColorBrush;
                if (buttonBrush != null)
                {
                    lines.updateLinesColor(buttonBrush);
                    shapes.updateShapesColor(buttonBrush);
                }
            }
        }

        private void Change_Brush_Size(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Content is TextBlock textBlock)
                {
                    int fontSize = (int)textBlock.FontSize;
                    brushSize = fontSize - 9;
                    lines.updateLinesSize(brushSize);
                }
            }
        }

        //Drawing methods
        private void Drawing_Method(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                drawingMethod = button.Name;
            }
        }

        private void EraseElements(Point position)
        {
            List<UIElement> elementsToRemove = new List<UIElement>();

            foreach (UIElement element in currentLayer.Children)
            {
                if (element is Ellipse || element is Line || element is Rectangle)
                {
                    Point elementPosition;

                    if (element is Ellipse)
                    {
                        Ellipse ellipse = element as Ellipse;
                        elementPosition = new Point(Canvas.GetLeft(ellipse) + ellipse.Width / 2, Canvas.GetTop(ellipse) + ellipse.Height / 2);
                    }
                    else if (element is Line)
                    {
                        Line line = element as Line;
                        elementPosition = new Point((line.X1 + line.X2) / 2, (line.Y1 + line.Y2) / 2);
                    }
                    else if (element is Rectangle)
                    {
                        Rectangle rectangle = element as Rectangle;
                        elementPosition = new Point(Canvas.GetLeft(rectangle) + rectangle.Width / 2, Canvas.GetTop(rectangle) + rectangle.Height / 2);
                    }

                    double distance = Math.Sqrt(Math.Pow(elementPosition.X - position.X, 2) + Math.Pow(elementPosition.Y - position.Y, 2));
                    if (distance <= brushSize)
                    {
                        elementsToRemove.Add(element);
                    }
                }
            }

            foreach (UIElement element in elementsToRemove)
            {
                currentLayer.Children.Remove(element);
            }
        }

        //Mouse
        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (drawingMethod == "vector" && e.RightButton == MouseButtonState.Pressed)
            {
                lines.Move_Vector(sender, e);
            }
            else if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (drawingMethod == "eraser")
                {
                    EraseElements(e.GetPosition(currentLayer));
                }
                else if (drawingMethod == "brush")
                {
                    lines.Start_Brush(sender, e);
                }
                else if (drawingMethod == "vector")
                {
                    lines.Start_Vector(sender, e);
                }
                if (drawingMethod == "rectangle")
                {
                    shapes.Start_Rectangle(sender, e);
                }
                else if (drawingMethod == "circle")
                {
                    shapes.Start_Circle(sender, e);
                }
                else if (drawingMethod == "sphere")
                {
                    shapes.Start_Sphere(sender, e);
                }
                else if (drawingMethod == "pyramid")
                {
                    shapes.Start_Pyramid(sender, e);
                }
                else if(drawingMethod == "cylinder")
                {
                    shapes.Start_Cylinder(sender, e);
                }
                else if (drawingMethod == "cuboid")
                {
                    shapes.Start_Cuboid(sender, e);
                }
            }
        }

        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (lines.selectedVector != null)
            {
                lines.Move_Update_Vector(sender, e);
            }
            else
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.GetPosition(currentLayer).Y >= 0)
                    {
                        if (drawingMethod == "eraser")
                        {
                            EraseElements(e.GetPosition(currentLayer));
                        }
                        else if (drawingMethod == "brush")
                        {
                            lines.Draw_Brush(sender, e);
                        }
                        else if (drawingMethod == "vector")
                        {
                            lines.Draw_Vector(sender, e);
                        }
                        else if (drawingMethod == "rectangle")
                        {
                            shapes.Draw_Rectangle(sender, e);
                        }
                        else if (drawingMethod == "circle")
                        {
                            shapes.Draw_Circle(sender, e);
                        }
                        else if (drawingMethod == "sphere")
                        {
                            shapes.Draw_Sphere(sender, e);
                        }
                        else if (drawingMethod == "pyramid")
                        {
                            shapes.Draw_Pyramid(sender, e);
                        }
                        else if (drawingMethod == "cylinder")
                        {
                            shapes.Draw_Cylinder(sender, e);
                        }
                        else if (drawingMethod == "cuboid")
                        {
                            shapes.Draw_Cuboid(sender, e);
                        }
                    }
                }
            }
        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (lines.selectedVector != null)
            {
                lines.selectedVector = null;
                lines.isMovingStartPoint = false;
                lines.isMovingEndPoint = false;
            }
            else
            {
                lines.currentEllipse = null;
                lines.currentVector = null;
            }
        }
    }
}
