using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    class pictures
    {
        public Image image { get; set; }
        public int area { get; set; }

        public pictures(Image pic, int are)
        {
            image = pic;
            area = are;
        }
    }
}
