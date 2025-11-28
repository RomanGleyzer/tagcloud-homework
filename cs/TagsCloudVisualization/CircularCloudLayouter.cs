using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouter(Point center)
{
    // Шага 0.1 радиан достаточно для плотного наполнения спирали прямоугольниками
    private const double SpiralAngleStep = 0.1;

    // Радиус спирали будет расти не слишнком медленно и не слишком быстро
    private const double ExpansionRate = 1.0;

    private readonly List<Rectangle> _createdRectangles = [];

    private double _currentAngle;

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (_createdRectangles.Count == 0)
        {
            var rectangle = CreateRectangle(rectangleSize, center.X, center.Y);
            _createdRectangles.Add(rectangle);
            return rectangle;
        }

        var candidate = GetFirstNonIntersectingRectangle(rectangleSize);
        var movedToCenter = MoveRectangleToCenter(candidate);

        _createdRectangles.Add(movedToCenter);
        return movedToCenter;
    }

    private Rectangle GetFirstNonIntersectingRectangle(Size rectangleSize)
    {
        while (true)
        {
            var pointOnSpiral = GetNextSpiralPoint();
            var candidate = CreateRectangle(rectangleSize, pointOnSpiral.X, pointOnSpiral.Y);

            if (!_createdRectangles.Any(candidate.IntersectsWith))
                return candidate;
        }
    }

    private Rectangle MoveRectangleToCenter(Rectangle candidate)
    {
        var current = candidate;

        while (true)
        {
            var centerX = current.X + current.Width / 2;
            var centerY = current.Y + current.Height / 2;

            var stepX = Math.Sign(center.X - centerX);
            var stepY = Math.Sign(center.Y - centerY);

            if (stepX == 0 && stepY == 0)
                break;

            var movedCenterX = centerX + stepX;
            var movedCenterY = centerY + stepY;

            var moved = CreateRectangle(current.Size, movedCenterX, movedCenterY);

            if (_createdRectangles.Any(moved.IntersectsWith))
                break;

            current = moved;
        }

        return current;
    }

    private Point GetNextSpiralPoint()
    {
        _currentAngle += SpiralAngleStep;

        var radius = ExpansionRate * _currentAngle;

        var x = center.X + (int)(radius * Math.Cos(_currentAngle));
        var y = center.Y + (int)(radius * Math.Sin(_currentAngle));

        return new Point(x, y);
    }

    private static Rectangle CreateRectangle(Size size, int centerX, int centerY)
    {
        var left = centerX - size.Width / 2;
        var top = centerY - size.Height / 2;
        return new Rectangle(left, top, size.Width, size.Height);
    }
}
