using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouter(Point center)
{
    private const double SpiralStep = 0.2;
    private const int ExpansionRate = 2;

    private readonly List<Rectangle> _createdRectangles = [];

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (_createdRectangles.Count == 0)
        {
            var startRectangle = CreateRectangle(rectangleSize, center.X, center.Y);

            _createdRectangles.Add(startRectangle);
            return startRectangle;
        }

        var _currentSpiralStep = 0.0;
        while (true)
        {
            _currentSpiralStep += SpiralStep;

            var spiralRadius = ExpansionRate * _currentSpiralStep;
            var candidateX = (int)(center.X + spiralRadius * Math.Cos(_currentSpiralStep));
            var candidateY = (int)(center.Y + spiralRadius * Math.Sin(_currentSpiralStep));

            var candidate = CreateRectangle(rectangleSize, candidateX, candidateY);
            if (_createdRectangles.Any(candidate.IntersectsWith))
                continue;

            var currentRectangle = candidate;
            while (true)
            {
                var centerX = currentRectangle.X + currentRectangle.Width / 2;
                var centerY = currentRectangle.Y + currentRectangle.Height / 2;

                var stepX = Math.Sign(center.X - centerX);
                var stepY = Math.Sign(center.Y - centerY);

                if (stepX == 0 && stepY == 0)
                    break;

                var movedCenterX = centerX + stepX;
                var movedCenterY = centerY + stepY;

                var moved = CreateRectangle(rectangleSize, movedCenterX, movedCenterY);

                if (_createdRectangles.Any(moved.IntersectsWith))
                    break;

                currentRectangle = moved;
            }

            _createdRectangles.Add(currentRectangle);
            return currentRectangle;
        }
    }

    private static Rectangle CreateRectangle(Size size, int centerX, int centerY)
    {
        var left = centerX - size.Width / 2;
        var top = centerY - size.Height / 2;
        return new Rectangle(left, top, size.Width, size.Height);
    }
}
