using System.Drawing;
namespace project
{
    public class Position
    {
        public SizeF size { get; set; }
        public PointF pos { get; set; }

        public Position()
        {
            size = new SizeF(0, 0);
            pos = new PointF(0, 0);
        }

        public Position(SizeF s, float x, float y)
        {
            size = new SizeF(s.Width + 10, s.Height + 10);
            pos = new PointF(x + 5, y + 5);
        }
    }
}