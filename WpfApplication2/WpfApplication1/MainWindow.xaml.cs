using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private List<Ellipse> ellipses = new List<Ellipse>();
        private Ellipse currentEllipse;
        private SolidColorBrush paintBrush = new SolidColorBrush(Colors.Black);

        private Rectangle currentRectangle;
        private Point startRectangle;
        private Ellipse currentCircle;
        private Point startCircle;

        private Point startVector;
        private Point endVector;
        private Line currentVector;
        private Line selectedVector;
        private bool isMovingStartPoint = false;
        private bool isMovingEndPoint = false;

        private string desktopPath = "";
        private String drawingMethod = "";
        private int toolboxHeight = 100;
        private int brushSize = 5;


        public MainWindow()
        {       
            InitializeComponent();
            drawingMethod = "brush";
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        //Save and load
        private void Save(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(paintSurface.ActualHeight.ToString());
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)paintSurface.ActualWidth, (int)paintSurface.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(paintSurface);
            
            var crop = new CroppedBitmap(renderBitmap, new Int32Rect(0, 100, (int)paintSurface.ActualWidth, (int)(paintSurface.ActualHeight - 100)));
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(crop));

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var fs = new FileStream(openFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
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
                    paintSurface.Children.Add(loadedImage);
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
                    paintBrush = new SolidColorBrush(buttonBrush.Color);
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
        private void Start_Rectangle(object sender, RoutedEventArgs e)
        {
            if (e is MouseButtonEventArgs mouseEvent && mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                startRectangle = Mouse.GetPosition(paintSurface);
                currentRectangle = new Rectangle{Width = 0, Height = 0, Fill = paintBrush, Stroke = paintBrush,};

                Canvas.SetLeft(currentRectangle, startRectangle.X);
                Canvas.SetTop(currentRectangle, startRectangle.Y);

                paintSurface.Children.Add(currentRectangle);
            }
        }
        private void Draw_Rectangle(object sender, MouseEventArgs e)
        {
            if (currentRectangle != null)
            {
                Point endPoint = Mouse.GetPosition(paintSurface);

                currentRectangle.Width = Math.Abs(endPoint.X - startRectangle.X);
                currentRectangle.Height = Math.Abs(endPoint.Y - startRectangle.Y);

                Canvas.SetLeft(currentRectangle, Math.Min(startRectangle.X, endPoint.X));
                Canvas.SetTop(currentRectangle, Math.Min(startRectangle.Y, endPoint.Y));
            }
        }

        private void Start_Circle(object sender, RoutedEventArgs e)
        {
            startCircle = Mouse.GetPosition(paintSurface);
            currentCircle = new Ellipse{Width = 0, Height = 0, Fill = paintBrush, Stroke = paintBrush,};

            Canvas.SetLeft(currentCircle, startCircle.X);
            Canvas.SetTop(currentCircle, startCircle.Y);

            paintSurface.Children.Add(currentCircle);
        }
        private void Draw_Circle(object sender, MouseEventArgs e)
        {
            if (currentCircle != null)
            {
                Point endPoint = Mouse.GetPosition(paintSurface);

                currentCircle.Width = Math.Abs(endPoint.X - startCircle.X);
                currentCircle.Height = Math.Abs(endPoint.Y - startCircle.Y);

                Canvas.SetLeft(currentCircle, Math.Min(startCircle.X, endPoint.X));
                Canvas.SetTop(currentCircle, Math.Min(startCircle.Y, endPoint.Y));
            }
        }

        private void Start_Vector(object sender, MouseEventArgs e)
        {
            Point startPoint = e.GetPosition(paintSurface);
            startPoint = e.GetPosition(paintSurface);
            endVector = startPoint;

            currentVector = new Line
            {
                Stroke = paintBrush,
                StrokeThickness = brushSize,
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endVector.X,
                Y2 = endVector.Y,
            };
            currentVector.Tag = "Vector";

            paintSurface.Children.Add(currentVector);

        }

        private void Draw_Vector(object sender, MouseEventArgs e)
        {
            endVector = e.GetPosition(paintSurface);

            if (currentVector != null)
            {
                currentVector.X2 = endVector.X;
                currentVector.Y2 = endVector.Y;
            }
        }

        private void Move_Vector(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(paintSurface);

            foreach (UIElement element in paintSurface.Children)
            {
                if (element is Line line)
                {
                    bool isVector = line.Tag != null && line.Tag.ToString() == "Vector";
                    if (isVector)
                    {
                        Point startPoint = new Point(line.X1, line.Y1);
                        Point endPoint = new Point(line.X2, line.Y2);

                        if (IsPointCloseToMouse(mousePosition, startPoint))
                        {
                            selectedVector = line;
                            isMovingStartPoint = true;
                            isMovingEndPoint = false;
                            return;
                        }
                        else if (IsPointCloseToMouse(mousePosition, endPoint))
                        {
                            selectedVector = line;
                            isMovingStartPoint = false;
                            isMovingEndPoint = true;
                            return;
                        }
                    }
                }
            }
        }


        private bool IsPointCloseToMouse(Point point, Point mousePosition)
        {
            double distance = Math.Sqrt(Math.Pow(point.X - mousePosition.X, 2) + Math.Pow(point.Y - mousePosition.Y, 2));
            return distance <= 20;
        }

        private void Move_Update_Vector(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(paintSurface);

            if (isMovingStartPoint)
            {
                selectedVector.X1 = mousePosition.X;
                selectedVector.Y1 = mousePosition.Y;
            }
            else if (isMovingEndPoint)
            {
                selectedVector.X2 = mousePosition.X;
                selectedVector.Y2 = mousePosition.Y;
            }
        }

        private void Start_Brush(object sender, MouseEventArgs e)
        {
            Point startPoint = e.GetPosition(paintSurface);
            currentEllipse = new Ellipse { Width = brushSize, Height = brushSize, Fill = paintBrush, Stroke = paintBrush, };
            Canvas.SetLeft(currentEllipse, startPoint.X - currentEllipse.Width / 2);
            Canvas.SetTop(currentEllipse, startPoint.Y - currentEllipse.Height / 2);

            paintSurface.Children.Add(currentEllipse);
            ellipses.Add(currentEllipse);
        }

        private void Draw_Brush(object sender, MouseEventArgs e)
        {
            if (currentEllipse != null)
            {
                Line connectingLine = new Line
                {
                    Stroke = paintBrush,
                    StrokeThickness = brushSize,
                    X1 = Canvas.GetLeft(currentEllipse) + currentEllipse.Width / 2,
                    Y1 = Canvas.GetTop(currentEllipse) + currentEllipse.Height / 2,
                    X2 = e.GetPosition(paintSurface).X,
                    Y2 = e.GetPosition(paintSurface).Y,          
                };

                paintSurface.Children.Add(connectingLine);

                currentEllipse = new Ellipse {Width = brushSize, Height = brushSize, Fill = paintBrush, Stroke = paintBrush,};

                Canvas.SetLeft(currentEllipse, e.GetPosition(paintSurface).X - currentEllipse.Width / 2);
                Canvas.SetTop(currentEllipse, e.GetPosition(paintSurface).Y - currentEllipse.Height / 2);

                paintSurface.Children.Add(currentEllipse);
                ellipses.Add(currentEllipse);
            }
        }

        private void EraseElements(Point position)
        {
            List<UIElement> elementsToRemove = new List<UIElement>();

            foreach (UIElement element in paintSurface.Children)
            {
                if (element is Ellipse || element is Line)
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

                    double distance = Math.Sqrt(Math.Pow(elementPosition.X - position.X, 2) + Math.Pow(elementPosition.Y - position.Y, 2));
                    if (distance <= brushSize)
                    {
                        elementsToRemove.Add(element);
                    }
                }
            }

            foreach (UIElement element in elementsToRemove)
            {
                paintSurface.Children.Remove(element);
            }
        }


        //Mouse
        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (drawingMethod == "vector" && e.RightButton == MouseButtonState.Pressed)
            {
                Move_Vector(sender, e);
            }
            else if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (drawingMethod == "eraser")
                {
                    EraseElements(e.GetPosition(paintSurface));
                }
                if (drawingMethod == "rectangle")
                {
                    Start_Rectangle(sender, e);
                }
                else if (drawingMethod == "circle")
                {
                    Start_Circle(sender, e);
                }
                else if(drawingMethod == "vector")
                {
                    Start_Vector(sender, e);
                }
                else if(drawingMethod == "brush")
                {
                    Start_Brush(sender, e);
                }
            }
        }

        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (selectedVector != null)
            {
                Move_Update_Vector(sender, e);
            }
            else
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.GetPosition(paintSurface).Y >= 0)
                    {
                        if (drawingMethod == "eraser")
                        {
                            EraseElements(e.GetPosition(paintSurface));
                        }
                        else if (drawingMethod == "rectangle")
                        {
                            Draw_Rectangle(sender, e);
                        }
                        else if (drawingMethod == "circle")
                        {
                            Draw_Circle(sender, e);
                        }
                        else if (drawingMethod == "vector")
                        {
                            Draw_Vector(sender, e);
                        }
                        else if (drawingMethod == "brush")
                        {
                            Draw_Brush(sender, e);
                        }
                    }
                }
            }
        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (selectedVector != null)
            {
                selectedVector = null;
                isMovingStartPoint = false;
                isMovingEndPoint = false;
            }
            else
            {
                currentEllipse = null;
                currentVector = null;
            }
        }

    }
}
