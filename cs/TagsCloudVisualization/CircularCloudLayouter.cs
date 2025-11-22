using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouter(Point center)
{
    private readonly List<Rectangle> _createdRectangles = [];

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (_createdRectangles.Count == 0)
        {
            var startRectangle = CreateRectangle(rectangleSize, center.X, center.Y);

            _createdRectangles.Add(startRectangle);
            return startRectangle;
        }
        
        return new Rectangle();
    }

    private static Rectangle CreateRectangle(Size size, int centerX, int centerY)
    {
        var rectangleX = centerX - size.Width / 2;
        var rectangleY = centerY - size.Height / 2;

        return new Rectangle(rectangleX, rectangleY, size.Width, size.Height);
    }
}
