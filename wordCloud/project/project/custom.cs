using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace project
{
    class custom
    {
        public Point point;
        public Size size;
        public int shape;
        public Brush color;
        public custom(Point i, Size j, int num, Brush col)
        {
            point = i;
            size = j;
            shape = num;
            color = col;
        }
    }
}
