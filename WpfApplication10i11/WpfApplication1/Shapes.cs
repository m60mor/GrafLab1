using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace WpfApplication1
{
    internal class Shapes
    {
        private Canvas currentLayer;
        private SolidColorBrush paintBrush;

        private Rectangle currentRectangle;
        private Point startRectangle;

        private Ellipse currentCircle;
        private Point startCircle;

        public Point startTriangle;
        public Polygon currentTriangle;
        private double triangleWidth = 12;
        private double triangleHeight = 6;

        public Point startArrow;
        public Polygon currentArrow;
        private double arrowWidth = 12;
        private double arrowHeight = 6;

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
        public Shapes(Canvas layer, SolidColorBrush brush)
        {
            currentLayer = layer;
            paintBrush = brush;
        }

        public void updateShapesLayer(Canvas layer)
        {
            currentLayer = layer;
        }

        public void updateShapesColor(SolidColorBrush brush)
        {
            paintBrush = brush;
        }

        public void Start_Rectangle(object sender, RoutedEventArgs e)
        {
            if (e is MouseButtonEventArgs mouseEvent && mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                startRectangle = Mouse.GetPosition(currentLayer);
                currentRectangle = new Rectangle { Width = 0, Height = 0, Fill = paintBrush, Stroke = paintBrush, };

                Canvas.SetLeft(currentRectangle, startRectangle.X);
                Canvas.SetTop(currentRectangle, startRectangle.Y);

                currentLayer.Children.Add(currentRectangle);
            }
        }
        public void Draw_Rectangle(object sender, MouseEventArgs e)
        {
            if (currentRectangle != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);

                currentRectangle.Width = Math.Abs(endPoint.X - startRectangle.X);
                currentRectangle.Height = Math.Abs(endPoint.Y - startRectangle.Y);

                Canvas.SetLeft(currentRectangle, Math.Min(startRectangle.X, endPoint.X));
                Canvas.SetTop(currentRectangle, Math.Min(startRectangle.Y, endPoint.Y));
            }
        }

        public void Start_Circle(object sender, RoutedEventArgs e)
        {
            startCircle = Mouse.GetPosition(currentLayer);
            currentCircle = new Ellipse { Width = 0, Height = 0, Fill = paintBrush, Stroke = paintBrush, };

            Canvas.SetLeft(currentCircle, startCircle.X);
            Canvas.SetTop(currentCircle, startCircle.Y);

            currentLayer.Children.Add(currentCircle);
        }
        public void Draw_Circle(object sender, MouseEventArgs e)
        {
            if (currentCircle != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);

                currentCircle.Width = Math.Abs(endPoint.X - startCircle.X);
                currentCircle.Height = Math.Abs(endPoint.Y - startCircle.Y);

                Canvas.SetLeft(currentCircle, Math.Min(startCircle.X, endPoint.X));
                Canvas.SetTop(currentCircle, Math.Min(startCircle.Y, endPoint.Y));
            }
        }

        public void Start_Triangle(object sender, MouseEventArgs e)
        {
            double cursorX = e.GetPosition(currentLayer).X;
            double cursorY = e.GetPosition(currentLayer).Y;

            startTriangle = new Point(cursorX, cursorY);
            PointCollection trianglePoints = new PointCollection
            {
                new Point(startTriangle.X, startTriangle.Y),
                new Point(startTriangle.X, startTriangle.Y),
                new Point(startTriangle.X, startTriangle.Y)
            };
            currentTriangle = new Polygon { Points = trianglePoints, Fill = paintBrush, Stroke = paintBrush };
            currentLayer.Children.Add(currentTriangle);
        }
        public void Draw_Triangle(object sender, MouseEventArgs e)
        {
            triangleWidth = 2;
            triangleHeight = 1;
            if (currentTriangle != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);
                triangleWidth = Math.Abs(endPoint.X - startTriangle.X);
                triangleHeight = Math.Abs(endPoint.Y - startTriangle.Y);

                if (startTriangle.Y - triangleHeight > 0 && startTriangle.X < currentLayer.Width)
                {
                    PointCollection trianglePoints = currentTriangle.Points;
                    trianglePoints[0] = new Point(startTriangle.X + (triangleWidth / 2), startTriangle.Y - triangleHeight);
                    trianglePoints[1] = new Point(startTriangle.X, startTriangle.Y + triangleHeight);
                    trianglePoints[2] = new Point(startTriangle.X + triangleWidth, startTriangle.Y + triangleHeight);
                }
            }
        }

        public void Start_Arrow(object sender, MouseEventArgs e)
        {
            double cursorX = e.GetPosition(currentLayer).X;
            double cursorY = e.GetPosition(currentLayer).Y;

            startArrow = new Point(cursorX, cursorY);
            PointCollection arrowPoints = new PointCollection
            {
                new Point(startArrow.X, startArrow.Y),
                new Point(startArrow.X, startArrow.Y),
                new Point(startArrow.X, startArrow.Y),
                new Point(startArrow.X, startArrow.Y),
                new Point(startArrow.X, startArrow.Y),
                new Point(startArrow.X, startArrow.Y),
                new Point(startArrow.X, startArrow.Y)
            };
            currentArrow = new Polygon { Points = arrowPoints, Fill = paintBrush, Stroke = paintBrush };
            currentLayer.Children.Add(currentArrow);
        }
        public void Draw_Arrow(object sender, MouseEventArgs e)
        {
            arrowWidth = 2;
            arrowHeight = 1;
            if (currentArrow != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);
                arrowWidth = Math.Abs(endPoint.X - startArrow.X);
                arrowHeight = Math.Abs(endPoint.Y - startArrow.Y);

                if (startArrow.Y - arrowHeight > 0 && startArrow.X < currentLayer.Width)
                {
                    PointCollection arrowPoints = currentArrow.Points;
                    arrowPoints[0] = new Point(startArrow.X + (arrowWidth / 2), startArrow.Y - arrowHeight);
                    arrowPoints[1] = new Point(startArrow.X, startArrow.Y);
                    arrowPoints[2] = new Point(startArrow.X + (arrowWidth / 3), startArrow.Y);
                    arrowPoints[3] = new Point(startArrow.X + (arrowWidth / 3), startArrow.Y + arrowHeight);
                    arrowPoints[4] = new Point(startArrow.X + 2 * (arrowWidth / 3), startArrow.Y + arrowHeight);
                    arrowPoints[5] = new Point(startArrow.X + 2 * (arrowWidth / 3), startArrow.Y);
                    arrowPoints[6] = new Point(startArrow.X + arrowWidth, startArrow.Y);
                }
            }
        }

        public void Start_Sphere(object sender, MouseEventArgs e)
        {
            sphereRadiusX = 5;
            sphereRadiusY = 1;
            ratioSphere = sphereRadiusX / sphereRadiusY;

            double cursorX = e.GetPosition(currentLayer).X - (sphereRadiusX * 2);
            double cursorY = e.GetPosition(currentLayer).Y - sphereRadiusX * 2 - sphereRadiusY;

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

            currentLayer.Children.Add(currentSphereDepth);
            currentLayer.Children.Add(currentSphereWidth);
            currentLayer.Children.Add(currentSphereCircle);
        }

        public void Draw_Sphere(object sender, MouseEventArgs e)
        {
            if (currentSphereDepth != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);
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

        public void Start_Cylinder(object sender, MouseEventArgs e)
        {
            cylinderRadiusX = 5;
            cylinderRadiusY = 1;
            ratioCylinder = cylinderRadiusX / cylinderRadiusY;

            double cursorX = e.GetPosition(currentLayer).X - (cylinderRadiusX * 2);
            double cursorY = e.GetPosition(currentLayer).Y - cylinderHeight + cylinderRadiusY;

            startCylinder = new Point(cursorX, cursorY);

            currentCylinderTop = new Ellipse { Width = cylinderRadiusX * 2, Height = cylinderRadiusY, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };
            Canvas.SetLeft(currentCylinderTop, startCylinder.X);
            Canvas.SetTop(currentCylinderTop, startCylinder.Y);
            currentCylinderBottom = new Ellipse { Width = cylinderRadiusX * 2, Height = cylinderRadiusY, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };
            Canvas.SetLeft(currentCylinderBottom, startCylinder.X);
            Canvas.SetTop(currentCylinderBottom, startCylinder.Y + cylinderHeight - cylinderRadiusY);
            currentCylinderBody = new Rectangle { Width = cylinderRadiusX * 2, Height = cylinderHeight, Fill = new SolidColorBrush(Colors.Transparent), Stroke = paintBrush };
            Canvas.SetLeft(currentCylinderBody, startCylinder.X);
            Canvas.SetTop(currentCylinderBody, startCylinder.Y);

            currentLayer.Children.Add(currentCylinderTop);
            currentLayer.Children.Add(currentCylinderBottom);
            currentLayer.Children.Add(currentCylinderBody);
        }

        public void Draw_Cylinder(object sender, MouseEventArgs e)
        {
            if (currentCylinderTop != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);
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

        public void Start_Cuboid(object sender, MouseEventArgs e)
        {
            cuboidWidth = 12;
            cuboidDepth = 3;
            cuboidHeight = 6;
            double cursorX = e.GetPosition(currentLayer).X - (cuboidWidth);
            double cursorY = e.GetPosition(currentLayer).Y - cuboidHeight;

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

            currentLayer.Children.Add(currentCuboidLeftSide);
            currentLayer.Children.Add(currentCuboidRightSide);
            currentLayer.Children.Add(currentCuboidTop);
            currentLayer.Children.Add(currentCuboidBottom);
        }

        public void Draw_Cuboid(object sender, MouseEventArgs e)
        {
            if (currentCuboidLeftSide != null && currentCuboidTop != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);
                cuboidWidth = Math.Abs(endPoint.X - startCuboid.X);
                cuboidDepth = cuboidWidth * 0.4;
                cuboidHeight = Math.Abs(endPoint.Y - startCuboid.Y);

                if (Math.Min(startCuboid.Y, endPoint.Y) - cuboidDepth > 0 && Math.Min(startCuboid.X, endPoint.X) + cuboidWidth + cuboidDepth < currentLayer.Width)
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

        public void Start_Pyramid(object sender, MouseEventArgs e)
        {
            pyramidWidth = 12;
            pyramidHeight = 6;
            double cursorX = e.GetPosition(currentLayer).X - (pyramidWidth / 2);
            double cursorY = e.GetPosition(currentLayer).Y - pyramidHeight;

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

            currentLayer.Children.Add(currentPyramidBase);
            currentLayer.Children.Add(currentPyramidSide1);
            currentLayer.Children.Add(currentPyramidSide2);
            currentLayer.Children.Add(currentPyramidSide3);
            currentLayer.Children.Add(currentPyramidSide4);
        }

        public void Draw_Pyramid(object sender, MouseEventArgs e)
        {
            if (currentPyramidBase != null)
            {
                Point endPoint = Mouse.GetPosition(currentLayer);
                pyramidWidth = Math.Abs(endPoint.X - startPyramid.X);
                pyramidHeight = Math.Abs(endPoint.Y - startPyramid.Y);

                if (startPyramid.Y - pyramidHeight > 0 && startPyramid.X + pyramidWidth < currentLayer.Width)
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
    }
}
