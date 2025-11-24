using FluentAssertions;
using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouterTests
{
    private const int Width = 20;
    private const int Height = 10;
    private const int CreatedRectanglesLimit = 200;
    private const int RandomMinValue = 10;
    private const int RandomMaxValue = 100;
    private const double ExpectedMinDensity = 0.5;

    private Point _center;
    private Size _rectangleSize;
    private CircularCloudLayouter _layouter;
    private Rectangle[] _rectangles = null!;

    [SetUp]
    public void Setup()
    {
        _center = Point.Empty;
        _layouter = new CircularCloudLayouter(_center);
        _rectangleSize = new Size(Width, Height);
    }

    [Test]
    public void PutNextRectangle_OneRectangle_ShouldBeInCenter()
    {
        var rectangle = _layouter.PutNextRectangle(_rectangleSize);

        var centerX = rectangle.Left + rectangle.Width / 2;
        var centerY = rectangle.Top + rectangle.Height / 2;
        var rectCenter = new Point(centerX, centerY);

        rectCenter.Should().Be(_center);
    }

    [Test]
    public void PutNextRectangle_ManyRectanglesWithDifferentSizes_ShouldNotIntersect()
    {
        var random = new Random(0);
        _rectangles = new Rectangle[CreatedRectanglesLimit];

        for (int i = 0; i < CreatedRectanglesLimit; i++)
        {
            var width = random.Next(RandomMinValue, RandomMaxValue);
            var height = random.Next(RandomMinValue, RandomMaxValue);
            var size = new Size(width, height);

            var rectangle = _layouter.PutNextRectangle(size);
            _rectangles[i] = rectangle;
        }

        var rectanglesCount = _rectangles.Length;
        for (int i = 0; i < rectanglesCount; i++)
            for (int j = i + 1; j < rectanglesCount; j++)
            {
                var r1 = _rectangles[i];
                var r2 = _rectangles[j];

                r1.IntersectsWith(r2).Should().BeFalse();
            }
    }

    // Прямоугольники должны лежать как можно плотнее друг к другу
    [Test]
    public void PutNextRectangle_ManyRectanglesWithDifferentSizes_ShouldBeDense()
    {
        var random = new Random(0);
        _rectangles = new Rectangle[CreatedRectanglesLimit];

        for (int i = 0; i < CreatedRectanglesLimit; i++)
        {
            var width = random.Next(RandomMinValue, RandomMaxValue);
            var height = random.Next(RandomMinValue, RandomMaxValue);
            var size = new Size(width, height);

            var rectangle = _layouter.PutNextRectangle(size);
            _rectangles[i] = rectangle;
        }

        var minX = _rectangles.Min(rect => rect.Left);
        var maxX = _rectangles.Max(rect => rect.Right);
        var minY = _rectangles.Min(rect => rect.Top);
        var maxY = _rectangles.Max(rect => rect.Bottom);

        var boundingWidth = maxX - minX;
        var boundingHeight = maxY - minY;
        var boudingArea = boundingWidth * boundingHeight;

        var totalArea = _rectangles.Sum(rect => rect.Width * rect.Height);
        var density = (double)totalArea / boudingArea;

        density.Should().BeGreaterThan(ExpectedMinDensity);
    }
}
