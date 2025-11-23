using FluentAssertions;
using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouterTests
{
    private const int Width = 20;
    private const int Height = 10;

    private Point _center;
    private Size _rectangleSize;
    private CircularCloudLayouter _layouter;

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
        const int rectanglesLimit = 200;
        const int randomMinValue = 10;
        const int randomMaxValue = 100;
        
        var random = new Random(0);
        var rectangles = new Rectangle[rectanglesLimit];

        for (int i = 0; i < rectanglesLimit; i++)
        {
            var width = random.Next(randomMinValue, randomMaxValue);
            var height = random.Next(randomMinValue, randomMaxValue);
            var size = new Size(width, height);

            var rectangle = _layouter.PutNextRectangle(size);
            rectangles[i] = rectangle;
        }

        var rectanglesCount = rectangles.Length;
        for (int i = 0; i < rectanglesCount; i++)
            for (int j = i + 1; j < rectanglesCount; j++)
            {
                var r1 = rectangles[i];
                var r2 = rectangles[j];

                r1.IntersectsWith(r2).Should().BeFalse();
            }
    }
}
