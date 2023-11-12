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
using System.Xml.Linq;
using Microsoft.Win32;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private string desktopPath = "";
        private String drawingMethod = "";
        private int toolboxHeight = 100;
        private int brushSize = 5;

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

        private Point startSphere;
        private Ellipse currentSphereDepth;
        private Ellipse currentSphereWidth;
        private Ellipse currentSphereCircle;
        private double sphereRadiusX = 5;
        private double sphereRadiusY = 1;
        private double ratioSphere = 1;

        private Point startCylinder;
        private Ellipse currentCylinderTop;
        private Ellipse currentCylinderBottom;
        private Rectangle currentCylinderBody;
        private double cylinderRadiusX = 5;
        private double cylinderRadiusY = 1;
        private double cylinderHeight = 10;
        private double ratioCylinder = 1;

        private Point startCuboid;
        private Polygon currentCuboidLeftSide;
        private Polygon currentCuboidRightSide;
        private Polygon currentCuboidTop;
        private Polygon currentCuboidBottom;
        private double cuboidWidth = 12;
        private double cuboidDepth = 3;
        private double cuboidHeight = 6;

        private Point startPyramid;
        private Polygon currentPyramidBase;
        private Polygon currentPyramidSide1;
        private Polygon currentPyramidSide2;
        private Polygon currentPyramidSide3;
        private Polygon currentPyramidSide4;
        private double pyramidWidth = 12;
        private double pyramidHeight = 6;

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
                paintSurface.Children.Remove(element);
            }
        }

        private void Start_Sphere(object sender, MouseEventArgs e)
        {
            sphereRadiusX = 5;
            sphereRadiusY = 1;
            ratioSphere = sphereRadiusX / sphereRadiusY;

            double cursorX = e.GetPosition(paintSurface).X - (sphereRadiusX * 2);
            double cursorY = e.GetPosition(paintSurface).Y - sphereRadiusX * 2 - sphereRadiusY;

            startSphere = new Point(cursorX, cursorY);

            currentSphereDepth = new Ellipse { Width = sphereRadiusY, Height = sphereRadiusX * 2, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };
            Canvas.SetLeft(currentSphereDepth, startSphere.X + sphereRadiusX);
            Canvas.SetTop(currentSphereDepth, startSphere.Y);
            currentSphereWidth = new Ellipse { Width = sphereRadiusX * 2, Height = sphereRadiusY, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };
            Canvas.SetLeft(currentSphereWidth, startSphere.X);
            Canvas.SetTop(currentSphereWidth, startSphere.Y + sphereRadiusX);
            currentSphereCircle = new Ellipse { Width = sphereRadiusX * 2, Height = sphereRadiusX * 2, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };
            Canvas.SetLeft(currentSphereCircle, startSphere.X);
            Canvas.SetTop(currentSphereCircle, startSphere.Y);

            paintSurface.Children.Add(currentSphereDepth);
            paintSurface.Children.Add(currentSphereWidth);
            paintSurface.Children.Add(currentSphereCircle);
        }

        private void Draw_Sphere(object sender, MouseEventArgs e)
        {
            if (currentSphereDepth != null)
            {
                Point endPoint = Mouse.GetPosition(paintSurface);
                if (sphereRadiusX != 0)
                {
                    ratioSphere = sphereRadiusY / sphereRadiusX;
                }
                sphereRadiusX = Math.Abs(endPoint.X - startSphere.X);
                sphereRadiusY = ratioSphere * sphereRadiusX;

                if (Math.Min(startSphere.Y, endPoint.Y) > 0)
                {
                    currentSphereWidth.Width = sphereRadiusX;
                    currentSphereWidth.Height = sphereRadiusY * 2;
                    currentSphereDepth.Width = sphereRadiusX * ratioSphere;
                    currentSphereDepth.Height = sphereRadiusY / ratioSphere;
                    currentSphereCircle.Width = sphereRadiusX;
                    currentSphereCircle.Height = sphereRadiusX;

                    Canvas.SetTop(currentSphereDepth, Math.Min(startSphere.Y, endPoint.Y));
                    Canvas.SetLeft(currentSphereDepth, Math.Min(startSphere.X, endPoint.X) + sphereRadiusX / 2 - sphereRadiusY / 2);
                    Canvas.SetTop(currentSphereWidth, Math.Min(startSphere.Y, endPoint.Y) + sphereRadiusX / 2 - sphereRadiusY);
                    Canvas.SetLeft(currentSphereWidth, Math.Min(startSphere.X, endPoint.X));
                    Canvas.SetTop(currentSphereCircle, Math.Min(startSphere.Y, endPoint.Y));
                    Canvas.SetLeft(currentSphereCircle, Math.Min(startSphere.X, endPoint.X));
                }
            }
        }

        private void Start_Cylinder(object sender, MouseEventArgs e)
        {
            cylinderRadiusX = 5;
            cylinderRadiusY = 1;
            ratioCylinder = cylinderRadiusX / cylinderRadiusY;

            double cursorX = e.GetPosition(paintSurface).X - (cylinderRadiusX * 2);
            double cursorY = e.GetPosition(paintSurface).Y - cylinderHeight + cylinderRadiusY;

            startCylinder = new Point(cursorX, cursorY);

            currentCylinderTop = new Ellipse { Width = cylinderRadiusX * 2, Height = cylinderRadiusY, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush};
            Canvas.SetLeft(currentCylinderTop, startCylinder.X);
            Canvas.SetTop(currentCylinderTop, startCylinder.Y);
            currentCylinderBottom = new Ellipse { Width = cylinderRadiusX * 2, Height = cylinderRadiusY, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush};
            Canvas.SetLeft(currentCylinderBottom, startCylinder.X);
            Canvas.SetTop(currentCylinderBottom, startCylinder.Y + cylinderHeight - cylinderRadiusY);
            currentCylinderBody = new Rectangle { Width = cylinderRadiusX * 2, Height = cylinderHeight, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush};
            Canvas.SetLeft(currentCylinderBody, startCylinder.X);
            Canvas.SetTop(currentCylinderBody, startCylinder.Y);

            paintSurface.Children.Add(currentCylinderTop);
            paintSurface.Children.Add(currentCylinderBottom);
            paintSurface.Children.Add(currentCylinderBody);
        }

        private void Draw_Cylinder(object sender, MouseEventArgs e)
        {
            if (currentCylinderTop != null)
            {
                Point endPoint = Mouse.GetPosition(paintSurface);
                if (cylinderRadiusX != 0)
                {
                    ratioCylinder = cylinderRadiusY / cylinderRadiusX;
                }
                cylinderRadiusX = Math.Abs(endPoint.X - startCylinder.X);
                cylinderRadiusY = ratioCylinder * cylinderRadiusX;

                if (endPoint.Y - cylinderRadiusY * 2 > 0 && startCylinder.Y - cylinderRadiusY * 2 > 0)
                {
                    currentCylinderTop.Width = currentCylinderBottom.Width = currentCylinderBody.Width = cylinderRadiusX;
                    currentCylinderTop.Height = cylinderRadiusY * 2;
                    currentCylinderBottom.Height = cylinderRadiusY * 2;

                    currentCylinderBody.Height = Math.Abs(endPoint.Y - startCylinder.Y);
                    Canvas.SetTop(currentCylinderTop, Math.Min(startCylinder.Y, endPoint.Y) - cylinderRadiusY * 2);
                    Canvas.SetLeft(currentCylinderTop, Math.Min(startCylinder.X, endPoint.X));
                    Canvas.SetTop(currentCylinderBottom, Math.Max(startCylinder.Y, endPoint.Y) - cylinderRadiusY * 2);
                    Canvas.SetLeft(currentCylinderBottom, Math.Min(startCylinder.X, endPoint.X));
                    Canvas.SetTop(currentCylinderBody, Math.Min(startCylinder.Y, endPoint.Y) - cylinderRadiusY);
                    Canvas.SetLeft(currentCylinderBody, Math.Min(startCylinder.X, endPoint.X));
                }
            }
        }

        private void Start_Cuboid(object sender, MouseEventArgs e)
        {
            cuboidWidth = 12;
            cuboidDepth = 3;
            cuboidHeight = 6;
            double cursorX = e.GetPosition(paintSurface).X - (cuboidWidth);
            double cursorY = e.GetPosition(paintSurface).Y - cuboidHeight;

            startCuboid = new Point(cursorX, cursorY);
            PointCollection leftSidePoints = new PointCollection
            {
                new Point(startCuboid.X, startCuboid.Y),
                new Point(startCuboid.X + cuboidDepth, startCuboid.Y),
                new Point(startCuboid.X + cuboidDepth, startCuboid.Y + cuboidHeight),
                new Point(startCuboid.X, startCuboid.Y + cuboidHeight)
            };
            currentCuboidLeftSide = new Polygon { Points = leftSidePoints, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection rightSidePoints = new PointCollection
            {
                new Point(startCuboid.X + cuboidWidth, startCuboid.Y),
                new Point(startCuboid.X + cuboidWidth + cuboidDepth, startCuboid.Y),
                new Point(startCuboid.X + cuboidWidth + cuboidDepth, startCuboid.Y + cuboidHeight),
                new Point(startCuboid.X + cuboidWidth, startCuboid.Y + cuboidHeight)
            };
            currentCuboidRightSide = new Polygon { Points = rightSidePoints, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection topPoints = new PointCollection
            {
                new Point(startCuboid.X + cuboidDepth, startCuboid.Y - cuboidDepth),
                new Point(startCuboid.X + cuboidWidth + cuboidDepth, startCuboid.Y - cuboidDepth),
                new Point(startCuboid.X + cuboidWidth, startCuboid.Y),
                new Point(startCuboid.X, startCuboid.Y)
            };
            currentCuboidTop = new Polygon { Points = topPoints, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection bottomPoints = new PointCollection
            {
                new Point(startCuboid.X + cuboidDepth, startCuboid.Y - cuboidDepth + cuboidHeight),
                new Point(startCuboid.X + cuboidWidth + cuboidDepth, startCuboid.Y - cuboidDepth + cuboidHeight),
                new Point(startCuboid.X + cuboidWidth, startCuboid.Y + cuboidHeight),
                new Point(startCuboid.X, startCuboid.Y + cuboidHeight)
            };
            currentCuboidBottom = new Polygon { Points = bottomPoints, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            paintSurface.Children.Add(currentCuboidLeftSide);
            paintSurface.Children.Add(currentCuboidRightSide);
            paintSurface.Children.Add(currentCuboidTop);
            paintSurface.Children.Add(currentCuboidBottom);
        }

        private void Draw_Cuboid(object sender, MouseEventArgs e)
        {
            if (currentCuboidLeftSide != null && currentCuboidTop != null)
            {
                Point endPoint = Mouse.GetPosition(paintSurface);
                cuboidWidth = Math.Abs(endPoint.X - startCuboid.X);
                cuboidDepth = cuboidWidth * 0.4;
                cuboidHeight = Math.Abs(endPoint.Y - startCuboid.Y);

                if (Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth > 0 && Math.Min(startCuboid.X, endPoint.X) + cuboidWidth + cuboidDepth < paintSurface.Width)
                {
                    PointCollection leftSidePoints = currentCuboidLeftSide.Points;
                    leftSidePoints[0] = new Point(Math.Min(startCuboid.X, endPoint.X), Math.Min(startCuboid.Y, endPoint.Y));
                    leftSidePoints[1] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth);
                    leftSidePoints[2] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) + cuboidHeight - cuboidDepth);
                    leftSidePoints[3] = new Point(Math.Min(startCuboid.X, endPoint.X), Math.Min(startCuboid.Y, endPoint.Y) + cuboidHeight);

                    PointCollection rightSidePoints = currentCuboidRightSide.Points;
                    rightSidePoints[0] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth, Math.Min(startCuboid.Y, endPoint.Y));
                    rightSidePoints[1] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth);
                    rightSidePoints[2] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) + cuboidHeight - cuboidDepth);
                    rightSidePoints[3] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth, Math.Min(startCuboid.Y, endPoint.Y) + cuboidHeight);

                    PointCollection topPoints = currentCuboidTop.Points;
                    topPoints[0] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth);
                    topPoints[1] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth);
                    topPoints[2] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth, Math.Min(startCuboid.Y, endPoint.Y));
                    topPoints[3] = new Point(Math.Min(startCuboid.X, endPoint.X), Math.Min(startCuboid.Y, endPoint.Y));

                    PointCollection bottomPoints = currentCuboidBottom.Points;
                    bottomPoints[0] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth + cuboidHeight);
                    bottomPoints[1] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth + cuboidDepth, Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth + cuboidHeight);
                    bottomPoints[2] = new Point(Math.Min(startCuboid.X, endPoint.X) + cuboidWidth, Math.Min(startCuboid.Y, endPoint.Y) + cuboidHeight);
                    bottomPoints[3] = new Point(Math.Min(startCuboid.X, endPoint.X), Math.Min(startCuboid.Y, endPoint.Y) + cuboidHeight);
                }
            }
        }

        private void Start_Pyramid(object sender, MouseEventArgs e)
        {
            pyramidWidth = 12;
            pyramidHeight = 6;
            double cursorX = e.GetPosition(paintSurface).X - (pyramidWidth / 2);
            double cursorY = e.GetPosition(paintSurface).Y - pyramidHeight;

            startPyramid = new Point(cursorX, cursorY);

            PointCollection basePoints = new PointCollection
            {
                new Point(startPyramid.X, startPyramid.Y),
                new Point(startPyramid.X + pyramidWidth, startPyramid.Y),
                new Point(startPyramid.X + pyramidWidth, startPyramid.Y + pyramidHeight),
                new Point(startPyramid.X, startPyramid.Y + pyramidHeight)
            };
            currentPyramidBase = new Polygon { Points = basePoints, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection side1Points = new PointCollection
            {
                startPyramid,
                new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight),
                new Point(startPyramid.X + pyramidWidth, startPyramid.Y)
            };
            currentPyramidSide1 = new Polygon { Points = side1Points, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection side2Points = new PointCollection
            {
                new Point(startPyramid.X + pyramidWidth, startPyramid.Y),
                new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight),
                new Point(startPyramid.X + pyramidWidth, startPyramid.Y + pyramidHeight)
            };
            currentPyramidSide2 = new Polygon { Points = side2Points, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection side3Points = new PointCollection
            {
                new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight),
                new Point(startPyramid.X, startPyramid.Y + pyramidHeight),
                new Point(startPyramid.X + pyramidWidth, startPyramid.Y + pyramidHeight)
            };
            currentPyramidSide3 = new Polygon { Points = side3Points, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            PointCollection side4Points = new PointCollection
            {
                new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight),
                new Point(startPyramid.X, startPyramid.Y),
                new Point(startPyramid.X, startPyramid.Y + pyramidHeight)
            };
            currentPyramidSide4 = new Polygon { Points = side4Points, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };

            paintSurface.Children.Add(currentPyramidBase);
            paintSurface.Children.Add(currentPyramidSide1);
            paintSurface.Children.Add(currentPyramidSide2);
            paintSurface.Children.Add(currentPyramidSide3);
            paintSurface.Children.Add(currentPyramidSide4);
        }

        private void Draw_Pyramid(object sender, MouseEventArgs e)
        {
            if (currentPyramidBase != null)
            {
                Point endPoint = Mouse.GetPosition(paintSurface);
                pyramidWidth = Math.Abs(endPoint.X - startPyramid.X);
                pyramidHeight = Math.Abs(endPoint.Y - startPyramid.Y);

                if (startPyramid.Y - pyramidHeight > 0 && startPyramid.X + pyramidWidth < paintSurface.Width)
                {
                    PointCollection basePoints = currentPyramidBase.Points;
                    basePoints[0] = new Point(startPyramid.X, startPyramid.Y);
                    basePoints[1] = new Point(startPyramid.X + pyramidWidth, startPyramid.Y);
                    basePoints[2] = new Point(startPyramid.X + pyramidWidth, startPyramid.Y + pyramidHeight);
                    basePoints[3] = new Point(startPyramid.X, startPyramid.Y + pyramidHeight);

                    PointCollection side1Points = currentPyramidSide1.Points;
                    side1Points[0] = startPyramid;
                    side1Points[1] = new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight);
                    side1Points[2] = new Point(startPyramid.X + pyramidWidth, startPyramid.Y);

                    PointCollection side2Points = currentPyramidSide2.Points;
                    side2Points[0] = new Point(startPyramid.X + pyramidWidth, startPyramid.Y);
                    side2Points[1] = new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight);
                    side2Points[2] = new Point(startPyramid.X + pyramidWidth, startPyramid.Y + pyramidHeight);

                    PointCollection side3Points = currentPyramidSide3.Points;
                    side3Points[0] = new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight);
                    side3Points[1] = new Point(startPyramid.X, startPyramid.Y + pyramidHeight);
                    side3Points[2] = new Point(startPyramid.X + pyramidWidth, startPyramid.Y + pyramidHeight);

                    PointCollection side4Points = currentPyramidSide4.Points;
                    side4Points[0] = new Point(startPyramid.X + (pyramidWidth / 2), startPyramid.Y - pyramidHeight);
                    side4Points[1] = new Point(startPyramid.X, startPyramid.Y);
                    side4Points[2] = new Point(startPyramid.X, startPyramid.Y + pyramidHeight);
                }
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
                else if (drawingMethod == "sphere")
                {
                    Start_Sphere(sender, e);
                }
                else if (drawingMethod == "pyramid")
                {
                    Start_Pyramid(sender, e);
                }
                else if(drawingMethod == "cylinder")
                {
                    Start_Cylinder(sender, e);
                }
                else if (drawingMethod == "cuboid")
                {
                    Start_Cuboid(sender, e);
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
                        else if (drawingMethod == "sphere")
                        {
                            Draw_Sphere(sender, e);
                        }
                        else if (drawingMethod == "pyramid")
                        {
                            Draw_Pyramid(sender, e);
                        }
                        else if (drawingMethod == "cylinder")
                        {
                            Draw_Cylinder(sender, e);
                        }
                        else if (drawingMethod == "cuboid")
                        {
                            Draw_Cuboid(sender, e);
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
