using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1
{
    internal class Lines
    {
        private Canvas currentLayer;
        private SolidColorBrush paintBrush;
        private int brushSize;

        private List<Ellipse> ellipses = new List<Ellipse>();
        public Ellipse currentEllipse;

        private Point startVector;
        private Point endVector;
        public Line currentVector;
        public Line selectedVector;
        public bool isMovingStartPoint = false;
        public bool isMovingEndPoint = false;

        public Lines(Canvas layer, SolidColorBrush brush, int brushS)
        {
            currentLayer = layer;
            paintBrush = brush;
            brushSize = brushS;
        }

        public void updateLinesLayer(Canvas layer)
        {
            currentLayer = layer;
        }
        public void updateLinesColor(SolidColorBrush brush)
        {
            paintBrush = brush;
        }
        public void updateLinesSize(int brushS)
        {
            brushSize = brushS;
        }

        public void Start_Brush(object sender, MouseEventArgs e)
        {
            Point startPoint = e.GetPosition(currentLayer);
            currentEllipse = new Ellipse { Width = brushSize, Height = brushSize, Fill = paintBrush, Stroke = paintBrush, };
            Canvas.SetLeft(currentEllipse, startPoint.X - currentEllipse.Width / 2);
            Canvas.SetTop(currentEllipse, startPoint.Y - currentEllipse.Height / 2);

            currentLayer.Children.Add(currentEllipse);
            ellipses.Add(currentEllipse);
        }
        public void Draw_Brush(object sender, MouseEventArgs e)
        {
            if (currentEllipse != null)
            {
                Line connectingLine = new Line
                {
                    Stroke = paintBrush,
                    StrokeThickness = brushSize,
                    X1 = Canvas.GetLeft(currentEllipse) + currentEllipse.Width / 2,
                    Y1 = Canvas.GetTop(currentEllipse) + currentEllipse.Height / 2,
                    X2 = e.GetPosition(currentLayer).X,
                    Y2 = e.GetPosition(currentLayer).Y,
                };

                currentLayer.Children.Add(connectingLine);

                currentEllipse = new Ellipse { Width = brushSize, Height = brushSize, Fill = paintBrush, Stroke = paintBrush, };

                Canvas.SetLeft(currentEllipse, e.GetPosition(currentLayer).X - currentEllipse.Width / 2);
                Canvas.SetTop(currentEllipse, e.GetPosition(currentLayer).Y - currentEllipse.Height / 2);

                currentLayer.Children.Add(currentEllipse);
                ellipses.Add(currentEllipse);
            }
        }
        public void Start_Vector(object sender, MouseEventArgs e)
        {
            Point startPoint = e.GetPosition(currentLayer);
            startPoint = e.GetPosition(currentLayer);
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

            currentLayer.Children.Add(currentVector);

        }

        public void Draw_Vector(object sender, MouseEventArgs e)
        {
            endVector = e.GetPosition(currentLayer);

            if (currentVector != null)
            {
                currentVector.X2 = endVector.X;
                currentVector.Y2 = endVector.Y;
            }
        }
        public void Move_Vector(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(currentLayer);

            foreach (UIElement element in currentLayer.Children)
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
        public bool IsPointCloseToMouse(Point point, Point mousePosition)
        {
            double distance = Math.Sqrt(Math.Pow(point.X - mousePosition.X, 2) + Math.Pow(point.Y - mousePosition.Y, 2));
            return distance <= 20;
        }
        public void Move_Update_Vector(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(currentLayer);

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
    }
}
