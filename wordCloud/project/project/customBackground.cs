using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class customBackground
    {

        public string word { get; set; }
        public Font wordFont { get; set; }
        public Brush color { get; set; }
        public PointF pos { get; set; }

        public customBackground(string word, Font wordFont, Brush color, PointF pos)
        {
            this.word = word;
            this.wordFont = wordFont;
            this.color = color;
            this.pos = pos;
        }

        public customBackground(customBackground word)
        {
            this.word = word.word;
            this.wordFont = word.wordFont;
            this.color = word.color;
            this.pos = word.pos;
        }
    }
}
