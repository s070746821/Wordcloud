using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class Word
    {
        public string actualWord { get; }
        public int count { get; set; }
        public bool used = false;
        public Position place { get; set; }
        public int fontSize { get; set; }

        public Word(string aWord)
        {
            actualWord = aWord;
            count = 1;
            place = new Position();
            fontSize = 1;
        }

        public void increaseCount()
        {
            count++;
            return;
        }

        public bool checkWord(string aWord)
        {
            if (aWord == actualWord) return true;
            else return false;
        }
    }

}
