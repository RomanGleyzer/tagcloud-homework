using CloudLayouterVisualizer;
using FluentAssertions;
using NUnit.Framework.Interfaces;
using System.Drawing;

namespace TagsCloudVisualization;

public class CircularCloudLayouterTests
{
    private const int CreatedRectanglesLimit = 200;

    private static readonly Size DefaultRectangleSize = new(30, 20);

    // Минимум половина ограничивающего прямоугольника должна быть занята прямоугольниками
    private const double ExpectedMinDensity = 0.5;

    // Допустимое значение смещения центра облака (не более 20 процентов от размера облака)
    private const double MaxAllowedCenterOffsetRatio = 0.2;
    
    // Облако не должно быть сильно вытянуто по вертикали и горизонтали. Ожидаем круг, а не овал
    // 0.75 < ожидаемое соотношение сторон < 1.25
    private const double MinAllowedAspectRatio = 0.75;
    private const double MaxAllowedAspectRatio = 1.25;

    private const string FailureImagesDirName = "TagCloudFailures";
    private const string FailureImagesFileExtension = "png";
    private const string FileNameTimestampFormat = "yyyyMMdd_HHmmssfff";
    private const string GuidFormat = "N";
    private const int VisualizationScale = 5;
    private const int VisualizationPadding = 10;

    private Point _center;
    private CircularCloudLayouter _layouter;

    [SetUp]
    public void SetUp()
    {
        _center = Point.Empty;
        _layouter = new CircularCloudLayouter(_center);
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
            return;

        try
        {
            var ctx = TestContext.CurrentContext;

            if (_layouter.CreatedRectangles.Count == 0)
            {
                TestContext.Out.WriteLine("Тест завершился с ошибкой до создания прямоугольников. Визуализация облака не была сохранена");
                return;
            }

            var outDir = Path.Combine(ctx.WorkDirectory, FailureImagesDirName);
            Directory.CreateDirectory(outDir);

            var fileName = $"{DateTime.UtcNow.ToString(FileNameTimestampFormat)}_{Guid.NewGuid().ToString(GuidFormat)}.{FailureImagesFileExtension}";
            var filePath = Path.Combine(outDir, fileName);

            new CloudVisualizer(_layouter.CreatedRectangles, filePath, VisualizationScale, VisualizationPadding).Visualize();

            TestContext.Out.WriteLine($"Визуализация облака сохранена в файл: {filePath} (тест: {ctx.Test.Name})");
        }
        catch (Exception e)
        {
            TestContext.Out.WriteLine($"Не удалось сохранить визуализацию облака: {e.Message}");
        }
    }

    private Rectangle[] CreateRandomRectangles(int count)
    {
        var rectangles = new Rectangle[count];

        for (var i = 0; i < count; i++)
            rectangles[i] = _layouter.PutNextRectangle(DefaultRectangleSize);

        return rectangles;
    }

    private static Rectangle GetBoundingRectangle(Rectangle[] rectangles)
    {
        var left = rectangles.Min(rect => rect.Left);
        var top = rectangles.Min(rect => rect.Top);
        var right = rectangles.Max(rect => rect.Right);
        var bottom = rectangles.Max(rect => rect.Bottom);

        return Rectangle.FromLTRB(left, top, right, bottom);
    }

    private static Point GetRectangleCenter(Rectangle rectangle)
    {
        var centerX = rectangle.Left + rectangle.Width / 2;
        var centerY = rectangle.Top + rectangle.Height / 2;
        return new Point(centerX, centerY);
    }

    [Test]
    public void PutNextRectangle_FirstRectangle_ShouldBeInCenter()
    {
        var rectangle = _layouter.PutNextRectangle(DefaultRectangleSize);

        var rectangleCenter = GetRectangleCenter(rectangle);

        rectangleCenter.Should().Be(_center);
    }

    [Test]
    public void PutNextRectangle_ManyRectangles_DoNotIntersect()
    {
        var rectangles = CreateRandomRectangles(CreatedRectanglesLimit);

        for (var i = 0; i < rectangles.Length; i++)
            for (var j = i + 1; j < rectangles.Length; j++)
                rectangles[i].IntersectsWith(rectangles[j]).Should().BeFalse();
    }

    // Облако должно быть достаточно плотным
    [Test]
    public void PutNextRectangle_ManyRectangles_ShouldBeDense()
    {
        var rectangles = CreateRandomRectangles(CreatedRectanglesLimit);

        var totalArea = rectangles.Sum(rect => rect.Width * rect.Height);

        var boundingRectangle = GetBoundingRectangle(rectangles);
        var boundingArea = boundingRectangle.Width * boundingRectangle.Height;

        var density = (double)totalArea / boundingArea;

        density.Should().BeGreaterThan(ExpectedMinDensity);
    }

    // Облако растет относительно центра
    [Test]
    public void PutNextRectangle_ManyRectangles_BoundingRectangleContainsCenter()
    {
        var rectangles = CreateRandomRectangles(CreatedRectanglesLimit);

        var boundingRectangle = GetBoundingRectangle(rectangles);

        boundingRectangle.Contains(_center).Should().BeTrue();
    }

    // Облако из прямоугольников получается в форме круга
    [Test]
    public void PutNextRectangle_ManyRectangles_ShapeHasCircularAspectRatio()
    {
        var rectangles = CreateRandomRectangles(CreatedRectanglesLimit);

        var boundingRectangle = GetBoundingRectangle(rectangles);

        var aspectRatio = (double)boundingRectangle.Width / boundingRectangle.Height;

        aspectRatio.Should().BeInRange(MinAllowedAspectRatio, MaxAllowedAspectRatio);
    }

    // Центр облака не должен сильно сместиться
    [Test]
    public void PutNextRectangle_ManyRectangles_CenterDoesNotShiftMuch()
    {
        var rectangles = CreateRandomRectangles(CreatedRectanglesLimit);

        var boundingRectangle = GetBoundingRectangle(rectangles);
        var boundingCenter = GetRectangleCenter(boundingRectangle);

        var dx = boundingCenter.X - _center.X;
        var dy = boundingCenter.Y - _center.Y;

        var distanceFromRequiredCenter = Math.Sqrt(dx * dx + dy * dy);
        
        var maxSize = Math.Max(boundingRectangle.Width, boundingRectangle.Height);
        
        var centerOffsetRatio = distanceFromRequiredCenter / maxSize;

        centerOffsetRatio.Should().BeLessThanOrEqualTo(MaxAllowedCenterOffsetRatio);
    }
}
