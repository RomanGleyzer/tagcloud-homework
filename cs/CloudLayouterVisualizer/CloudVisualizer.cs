using System.Drawing;

namespace CloudLayouterVisualizer;

public class CloudVisualizer(List<Rectangle> rectangles, string filePath, double scale, int padding)
{
    public void Visualize()
    {
        var minX = rectangles.Min(rect => rect.Left);
        var minY = rectangles.Min(rect => rect.Top);
        var maxX = rectangles.Max(rect => rect.Right);
        var maxY = rectangles.Max(rect => rect.Bottom);

        var cloudWidth = maxX - minX;
        var cloudHeight = maxY - minY;

        var scaleWidth = cloudWidth * scale;
        var scaleHeight = cloudHeight * scale;

        var imageWidth = (int)Math.Ceiling(scaleWidth + 2 * padding);
        var imageHeight = (int)Math.Ceiling(scaleHeight + 2 * padding);

        using var bitmap = new Bitmap(imageWidth, imageHeight);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.Clear(Color.White);

        using var fillBrush = new SolidBrush(Color.Orange);
        using var textBrush = new SolidBrush(Color.Black);
        using var font = new Font("Arial", 8);
        using var pen = new Pen(Color.Black, 1);

        foreach (var rect in rectangles)
        {
            var x = (float)((rect.X - minX) * scale + padding);
            var y = (float)((rect.Y - minY) * scale + padding);
            var width = (float)(rect.Width * scale);
            var height = (float)(rect.Height * scale);

            var imageRect = new RectangleF(x, y, width, height);

            graphics.FillRectangle(fillBrush, imageRect);
            graphics.DrawRectangle(pen, imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height);
        }

        bitmap.Save(filePath);
    }
}
