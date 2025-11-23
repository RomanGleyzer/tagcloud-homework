using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouter(Point center)
{
    private const double SpiralStep = 0.5;
    private const int ExpansionRate = 5;

    private readonly List<Rectangle> _createdRectangles = [];

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (_createdRectangles.Count == 0)
        {
            var startRectangle = CreateRectangle(size: rectangleSize, x: center.X, y: center.Y, useRounding: false);

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

            var candidate = CreateRectangle(size: rectangleSize, x: candidateX, y: candidateY, useRounding: true);

            if (!_createdRectangles.Any(candidate.IntersectsWith))
            {
                _createdRectangles.Add(candidate);
                return candidate;
            }
        }
    }

    private static Rectangle CreateRectangle(Size size, int x, int y, bool useRounding)
    {
        int left, top;
        if (useRounding)
        {
            left = (int)Math.Round(x - size.Width / 2.0);
            top = (int)Math.Round(y - size.Height / 2.0);
        }
        else
        {
            left = x - size.Width / 2;
            top = y - size.Height / 2;
        }

        return new Rectangle(left, top, size.Width, size.Height);
    }
}
