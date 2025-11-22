using FluentAssertions;
using FluentAssertions.Execution;
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
    public void PutNextRectangle_FirstCall_ShouldBeInCenter()
    {
        var rectangle = _layouter.PutNextRectangle(_rectangleSize);

        using (new AssertionScope())
        {
            var expectedLeft = _center.X - _rectangleSize.Width / 2;
            var expectedTop = _center.Y - _rectangleSize.Height / 2;

            rectangle.Left.Should().Be(expectedLeft);
            rectangle.Top.Should().Be(expectedTop);
        }
    }
}
