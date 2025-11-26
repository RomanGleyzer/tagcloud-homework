using TagsCloudVisualization;
using System.Drawing;
using CloudLayouterVisualizer;

var layouter = new CircularCloudLayouter(new Point(0, 0));
var rectangles = new List<Rectangle>();

for (int i = 0; i < 50; i++)
{
    rectangles.Add(layouter.PutNextRectangle(new Size(30, 20)));
}

var visualizer = new CloudVisualizer(rectangles, "cloud.png", 10, 1);
visualizer.Visualize();