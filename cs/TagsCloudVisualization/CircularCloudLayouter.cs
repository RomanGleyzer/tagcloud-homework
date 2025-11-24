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
            var rectangle = CreateRectangle(rectangleSize, center.X, center.Y);
            _createdRectangles.Add(rectangle);
            return rectangle;
        }

        var _currentSpiralStep = 0.0;
        while (true)
        {
            _currentSpiralStep += SpiralStep;

            var candidate = CreateCandidate(center, rectangleSize, _currentSpiralStep);
            if (_createdRectangles.Any(candidate.IntersectsWith))
                continue;

            var currentRectangle = HandleCandidateMovingToCenter(rectangleSize, candidate);

            _createdRectangles.Add(currentRectangle);
            return currentRectangle;
        }
    }

    private static Rectangle CreateCandidate(Point center, Size size, double currentSpiralStep)
    {
        var spiralRadius = ExpansionRate * currentSpiralStep;
        var candidateX = (int)(center.X + spiralRadius * Math.Cos(currentSpiralStep));
        var candidateY = (int)(center.Y + spiralRadius * Math.Sin(currentSpiralStep));

        return CreateRectangle(size, candidateX, candidateY);
    }

    private Rectangle HandleCandidateMovingToCenter(Size size, Rectangle candidate)
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

            var moved = CreateRectangle(size, movedCenterX, movedCenterY);

            if (_createdRectangles.Any(moved.IntersectsWith))
                break;

            current = moved;
        }

        return current;
    }

    private static Rectangle CreateRectangle(Size size, int centerX, int centerY)
    {
        var left = centerX - size.Width / 2;
        var top = centerY - size.Height / 2;
        return new Rectangle(left, top, size.Width, size.Height);
    }
}
