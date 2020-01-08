using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace project
{
    public partial class main : Form
    {

        BackgroundWorker bw = new BackgroundWorker();

        loadingScreen load = new loadingScreen();

        Stopwatch s = new Stopwatch();

        int pictureWidth;
        int pictureHeight;

        int originalWidth;
        int originalHeight;

        //this array is used for most checks
        //if a word cant fit inside the array then it cant fit in the picture
        byte[,] picSize;
        bool[,] blankMap;
        PointF nextStart = new PointF();
        Random r = new Random();

        int area = 0;
        int blankArea = 0;

        PictureBox picture = new PictureBox();
        Bitmap flag;
        Graphics flagGraphics;

        string fontString;

        List<Brush> colors;

        Color bgColor;

        int tempWordCount = 0;

        List<Word> words;

        List<pictures> pictures = new List<pictures>();

        FontStyle fontstyle;

        int index = 0;

        bool transparent = false;
        bool wordSize = false;
        int scale = 1;

        List<customBackground> wordsToResize = new List<customBackground>();

        Image backGround = null;

        public main(List<Word> wordss, string font, List<Brush> brushes, int picWid, int picHit, Color color, string filename, byte fontStyle, bool transparent, int scale, string customBack)
        {
            if (customBack != null)
            {
                backGround = Image.FromFile(customBack);
                wordSize = true;
            }

            if (scale >= 2)
            {
                this.scale = scale;
                wordSize = true;
            }

            this.transparent = transparent;

            if (fontStyle == 0)
                fontstyle = FontStyle.Regular;
            else if (fontStyle == 1)
                fontstyle = FontStyle.Bold;
            else if (fontStyle == 2)
                fontstyle = FontStyle.Italic;
            else if (fontStyle == 3)
                fontstyle = FontStyle.Bold | FontStyle.Italic;

            this.words = wordss;

            this.PreviewKeyDown += Main_PreviewKeyDown;

            picture.Click += Picture_Click;

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;

            //this is where most of the global variables are initialized
            bgColor = color;

            originalHeight = picHit;
            originalWidth = picWid;

            pictureWidth = (picWid / 10) / scale;
            pictureHeight = (picHit / 10) /scale;

            picSize = new byte[pictureWidth, pictureHeight];
            blankMap = new bool[pictureWidth, pictureHeight];

            flag = new Bitmap(pictureHeight * 10, pictureWidth * 10);
            
            fontString = font;
            colors = brushes;

            picture.Size = new Size(pictureHeight * 10, pictureWidth * 10);
            this.Controls.Add(picture);
            flagGraphics = Graphics.FromImage(flag);

            //sets blankpic
            //blankpic is the initial unfilled array

            customImage(filename);

            flagGraphics.Clear(bgColor);

            //flagGraphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            flagGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            //flagGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //flagGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //flagGraphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            //flagGraphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            //sorts the list based on the word counts
            words = words.OrderByDescending(o => o.count).ToList();

            //stopwatch start
            s.Start();

            if (bw.IsBusy != true)
                bw.RunWorkerAsync();

            if (load.ShowDialog() == false)
            {
                bw.CancelAsync();
            }
            //variable to keep best picture

            //Console.WriteLine(words.Count);

            //Console.WriteLine(s.ElapsedMilliseconds);

            //stop stopwatch
            s.Stop();
            s.Restart();

            ToolTip tt = new ToolTip();
            tt.SetToolTip(picture, "Click for options");


            //Console.WriteLine(bestarea);

            //((Bitmap)bestpic).MakeTransparent(Color.White);
            //set image to the bestpic

            //flag.Save("test.jpeg");

            //picture.Image.Save("test.jpeg");

            InitializeComponent();

            //sets form size to be slightly bigger than picture size
            this.Size = new Size(pictureHeight * 10 * scale + 20, pictureWidth * 10 * scale + 45);

        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.Close();
            }
            else
            {
                picture.Size = new Size(originalHeight, originalWidth);
                //pictures[0].image = ResizeImage(pictures[0].image, 2000, 2000);
                this.Text = string.Format("Image #" + (index + 1));
                this.Opacity = 100;
                picture.Image = pictures[0].image;
                MessageBox.Show("Use arrow keys to navigate images.\nClick on image for image options.", "Image");
            }
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            imageOptions img = new imageOptions();
            img.Title = img.textBox.Text  = string.Format("Image" + (index + 1));
            img.image = pictures[index].image;
            var temp = img.ShowDialog();
            if (temp == true)
            {
                if (img.sameSet)
                {
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }
                else
                {
                    this.DialogResult = DialogResult.No;
                    this.Close();
                }
            }
        }

        private void Main_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Left)
            {
                if (--index < 0)
                    index = 9;
                picture.Image = pictures[index].image;
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (++index > 9)
                    index = 0;
                picture.Image = pictures[index].image;
            }
            this.Text = string.Format("Image #" + (index + 1));
            //Console.WriteLine(index);

        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int q = 0; q < 50; q++)
            {
                //Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64);
                //Console.WriteLine(worker.CancellationPending);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                wordsToResize.Clear();

                tempWordCount = 0;
                //reset everything each run
                //picSize = new int[pictureWidth, pictureHeight];
                flag = new Bitmap(originalHeight, originalWidth);

                picture.Size = new Size(originalHeight, originalWidth);
                this.Controls.Add(picture);
                flagGraphics = Graphics.FromImage(flag);

                flagGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                //reset array
                setPicSize();

                flagGraphics.Clear(bgColor);

                //assigns size based on word count
                int prevArea;
                while (area <= (pictureHeight * pictureWidth * 8 / 10))
                {
                    prevArea = area;
                    assignSize(words, 1);

                    //shuffles words randomly
                    //algorithm randomly shuffles the list of words
                    //then starts from the top to the bottom trying to plave the words
                    //this makes the words be placed randomly
                    shuffle(words);

                    //place words is where the magic happens
                    placeWords(words);

                    //Console.Write(area + " ");

                    //basically algorithm places a 2 in the picSize array
                    //if it finds a really small gap ie. smallest word height
                    //is 2 so if it runs into a gap of 1 it places a 2
                    //in picSize array
                    //this basically resets all 2s in picSize back to 0
                    for (int i = 0; i < pictureWidth; i++)
                        for (int j = 0; j < pictureHeight; j++)
                            if (picSize[i, j] == 2)
                                picSize[i, j] = 0;

                    if (prevArea == area)
                    {
                        List<Word> temp = new List<Word>();

                        foreach (Word i in words)
                        {
                            lastRun(i);
                            if (prevArea != area)
                            {
                                //Console.Write("Fit");
                                break;
                            }
                        }
                        if (prevArea == area)
                        {
                            //Console.Write("still not fit");
                            break;
                        }
                    }
                }
                //assign smaller size

                assignSize(words, 3);
                shuffle(words);

                placeWords(words);

                //Console.Write(area + " ");

                for (int i = 0; i < pictureWidth; i++)
                    for (int j = 0; j < pictureHeight; j++)
                        if (picSize[i, j] == 2)
                            picSize[i, j] = 0;

                //assign smallest size
                assignSize(words, 4);
                shuffle(words);

                //last run attempts to place these smallest words into gaps
                foreach (Word i in words)
                    lastRun(i);
                //Console.WriteLine(area + " ");

                //Console.WriteLine(area);

                if (wordSize)
                {
                    resizeWords();
                }

                if (transparent)
                    flag.MakeTransparent(bgColor);

                //if area this run is better than previous best run
                //Console.WriteLine(area);

                if (pictures.Count < 10)
                    pictures.Add(new pictures(flag, area));
                else if (pictures[9].area < area)
                {
                    //Console.WriteLine("new best");
                    //flagGraphics.DrawString(q.ToString(), new Font("Arial", 20), Brushes.Black, 0, 0);
                    pictures.RemoveAt(9);
                    pictures.Add(new pictures(flag, area));
                    //Console.WriteLine(bestarea);
                    //Console.WriteLine(tempWordCount);
                }
                
                pictures = pictures.OrderByDescending(o => o.area).ToList();
                //foreach (var i in pictures)
                //    Console.Write(i.area + " ");
                //Console.WriteLine(q);
                //flagGraphics.DrawString(q.ToString(), new Font("Arial", 10), Brushes.Black, PointF.Empty);
                worker.ReportProgress((q * 2) + 2);
            }
            /*
                for (int i = 0; i < pictureHeight; i++)
                {
                    for (int j = 0; j < pictureWidth; j++)
                    {
                        if (blankMap[j, i] == true)
                            Console.Write("1");
                        else
                            Console.Write("0");
                    }
                    Console.WriteLine();
                }
                */
        }

        private void resizeWords()
        {
            flag = new Bitmap(originalHeight, originalWidth);

            picture.Size = new Size(originalHeight, originalWidth);
            this.Controls.Add(picture);
            flagGraphics = Graphics.FromImage(flag);

            flagGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            flagGraphics.Clear(bgColor);

            if (backGround != null)
                flagGraphics.DrawImageUnscaledAndClipped(backGround, new Rectangle(0,0,originalHeight, originalWidth));

            foreach (var i in wordsToResize)
            {
                Font tempfont = new Font(i.wordFont.Name, i.wordFont.Size * scale);

                flagGraphics.DrawString(i.word, tempfont, i.color, i.pos.X * scale, i.pos.Y * scale);
            }

            return;
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            load.setProgressBar(e.ProgressPercentage);
        }

        private void setPicSize()
        {
            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    if (blankMap[j, i] == true)
                        picSize[j, i] = 1;
                    else
                        picSize[j, i] = 0;
                }
            }
            area = blankArea;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("download button click");
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("generate button click");
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        //function first checks if the specified area is out of bounds
        //then function checks the specified area given in the array
        //and returns false if a value other than 0 is found
        //then function checks actual bitmap if entire space is emtpy
        //if the area passes these checks then it returns true
        bool checkArea(int x, int y, int width, int height)
        {
            if (x + width >= pictureHeight || y + height >= pictureWidth)
                return false;

            //check picSize
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    if (picSize[j, i] == 1 || picSize[j,i] == 2)
                        return false;
                }
            }

            //check actual bitmap
            for (int i = x * 10; i < (x + width) * 10; i++)
            {
                for (int j = y * 10; j < (y + height) * 10; j++)
                {
                    //flag.SetPixel(i, j, Color.Black);
                    if (flag.GetPixel(i, j).ToArgb() != bgColor.ToArgb())
                    {
                        //Console.Write(flag.GetPixel(i, j));
                        return false;
                    }
                }
            }

            //flagGraphics.FillRectangle(Brushes.Black, x * 10, y * 10, width * 10, height * 10);

            return true;
        }

        //this function finds the next position for the last run function
        //it searches for space that can fit at least a 4 x 2 word
        //searches each column from top to bottom at a time from left to right
        //returns a point if valid point is found
        PointF findLastRun()
        {
            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    if (picSize[j, i] == 0 && blankMap[j,i] == false)
                    {
                        //3 from bottom or 3 / 2 / 1 gap
                        if (!checkArea(i, j, 4, 2))
                        {
                            picSize[j, i] = 2;
                        }
                        else
                        {
                            return new PointF(i * 10, j * 10);
                        }
                    }
                }
            }
            return new PointF();
        }

        //this function makes program alternate between placeWord and placeWordRight
        void placeWords(List<Word> words)
        {
            bool temp = false;

            for (int i = 0; i < words.Count; i++)
            {
                if (temp)
                    placeWord(words[i]);
                else
                    placeWordRight(words[i]);
                temp = !temp;
            }

            /*
            for (int i = 0; i < 70; i++)
            {
                for (int j = 0; j < 70; j++)
                {
                    tempout += picSize[i, j];
                }
                Console.WriteLine(tempout);
                tempout = "";
            }
            */

    }

        //this functions places the smallest words to fill remaining gaps
        //works by trying to place words from left
        void lastRun(Word word)
        {
            Font wordFont;
            //List<Position> positions = new List<Position>();
            float modifyY;
            float modifyX;
            float fontSize;

            int hit;
            int wit;
            SizeF siz;

            //loops through entire list of words


            nextStart = findLastRun();
            //fontSize = words[i].fontSize * 10;
            //Console.WriteLine(nextStart);

            //sets font to 10
            fontSize = 10;
            wordFont = new Font(fontString, fontSize, fontstyle);

            //this is the size of the actual word plus a small border
            siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);

            //the idea behind this is that the program has basically ignored
            //all gaps of size 20 up to this point
            //now in order to fill these gaps, it must find a fontsize that
            //the word heights are less than 20 so they can fit inside
            //size 20
            while (siz.Height > 20)
            {
                fontSize -= 1;
                wordFont = new Font(fontString, fontSize, fontstyle);
                siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);
            }
                
            //basically divides the height and width of the word
            //and rounds up no matter what
            //the assumption is basically if a word is 
            //9.9 or 9.1 pixels tall, both will fit into
            //a 10 pixel gap
            hit = (int)((siz.Height + 10) / 10);
            wit = (int)((siz.Width + 10) / 10);

            //Console.WriteLine(siz.Width + " " + siz.Height);

            //checks bitmap if word can fit
            if (checkFit(nextStart, hit, wit))
            {
                tempWordCount++;
                //if word fits, it places it

                //by default draw string has a bit gap surrounding the word
                //this makes the gap around the word smaller
                word.place = new Position(flagGraphics.MeasureString(word.actualWord, wordFont), nextStart.X, nextStart.Y);
                modifyY = word.place.size.Height * 0.14f;
                modifyX = fontSize * 0.2f;
                word.place.size = new SizeF(word.place.size.Width - (fontSize * 0.45f), word.place.size.Height * 0.6f);

                //modifyY = words[i].place.size.Height * 0.14f;
                //modifyX = fontSize * 0.2f;
                //words[i].place.size = new SizeF(words[i].place.size.Width - (fontSize * 0.45f), words[i].place.size.Height * 0.6f);

                //fills picSize array 
                fillArray(word.place.size, word.place.pos);

                //Console.WriteLine(words[i].place.size + " at " + words[i].place.pos);

                PointF tempPoint = new PointF(word.place.pos.X - modifyX, word.place.pos.Y - modifyY);
                Brush tempColor = randColor();
                flagGraphics.DrawString(word.actualWord, wordFont, tempColor, tempPoint);

                if (wordSize)
                    wordsToResize.Add(new customBackground(word.actualWord, wordFont, tempColor, tempPoint));

                //temp
                //flagGraphics.DrawRectangle(new Pen(Color.Blue), new Rectangle((int)words[i].place.pos.X, (int)words[i].place.pos.Y, (int)words[i].place.size.Width, (int)words[i].place.size.Height));
                word.used = true;
                modifyX = 0;
                modifyY = 0;
                //Console.WriteLine("lastfit");
            }
            else
            {
                //if word doesnt fit
                word.place = new Position(new SizeF(0, 0), 0, 0);
            }
            
        }

        //look at last run for more detailed explanation
        //this function attemps to place words on left side
        void placeWord(Word word)
        {

            Font wordFont;
            //List<Position> positions = new List<Position>();
            float modifyY;
            float modifyX;
            float fontSize;

            int hit;
            int wit;
            SizeF siz;

            nextStart = findNextPos();
            fontSize = word.fontSize * 10;
            wordFont = new Font(fontString, fontSize, fontstyle);

            siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);

            while (fontSize <= 20 && siz.Height > 30)
            {
                fontSize -= 1;
                wordFont = new Font(fontString, fontSize, fontstyle);
                siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);
            }

            hit = (int)((siz.Height + 10) / 10);
            wit = (int)((siz.Width + 10) / 10);

            //if word cant fit make it smaller and try again
            if (!checkFit(nextStart, hit, wit) && fontSize > 20)
            {
                //Console.WriteLine("make it smaller");
                fontSize -= 10;
                wordFont = new Font(fontString, fontSize, fontstyle);

                siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);
                hit = (int)((siz.Height + 10) / 10);
                wit = (int)((siz.Width + 10) / 10);
            }

            if (checkFit(nextStart, hit, wit))
            {
                tempWordCount++;
                word.place = new Position(flagGraphics.MeasureString(word.actualWord, wordFont), nextStart.X, nextStart.Y);
                modifyY = word.place.size.Height * 0.14f;
                modifyX = fontSize * 0.2f;
                word.place.size = new SizeF(word.place.size.Width - (fontSize * 0.45f), word.place.size.Height * 0.6f);
                //modifyY = words[i].place.size.Height * 0.14f;
                //modifyX = fontSize * 0.2f;
                //words[i].place.size = new SizeF(words[i].place.size.Width - (fontSize * 0.45f), words[i].place.size.Height * 0.6f);
                fillArray(word.place.size, word.place.pos);
                //Console.WriteLine(words[i].place.size + " at " + words[i].place.pos);
                PointF tempPoint = new PointF(word.place.pos.X - modifyX, word.place.pos.Y - modifyY);
                Brush tempColor = randColor();
                flagGraphics.DrawString(word.actualWord, wordFont, tempColor, tempPoint);

                if (wordSize)
                    wordsToResize.Add(new customBackground(word.actualWord, wordFont, tempColor, tempPoint));
                
                //temp
                //flagGraphics.DrawRectangle(new Pen(Color.Blue), new Rectangle((int)words[i].place.pos.X, (int)words[i].place.pos.Y, (int)words[i].place.size.Width, (int)words[i].place.size.Height));
                word.used = true;
                modifyX = 0;
                modifyY = 0;
            }
            else
            {
                word.place = new Position(new SizeF(0, 0), 0, 0);
                if (fontSize <= 20)
                {
                    lastRun(word);
                }
            }
        }

        //look at last run and placeWord for more detailed explanation
        //this function attemps to place words on right side
        void placeWordRight(Word word)
        {

            Font wordFont;
            //List<Position> positions = new List<Position>();
            float modifyY;
            float modifyX;
            float fontSize;

            int hit;
            int wit;
            SizeF siz;

            
            nextStart = findRightNextPos();
            fontSize = word.fontSize * 10;
            wordFont = new Font(fontString, fontSize, fontstyle);
            siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);

            while (fontSize <= 20 && siz.Height > 30)
            {
                fontSize -= 1;
                wordFont = new Font(fontString, fontSize, fontstyle);
                siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);
            }

            hit = (int)((siz.Height + 10) / 10);
            wit = (int)((siz.Width + 10) / 10);

            //Console.WriteLine(nextStart + " " + wit);

            float tempX = nextStart.X;

            nextStart = new PointF(tempX - (wit * 10 - 10), nextStart.Y);

            //Console.WriteLine(nextStart);

            if (!checkFit(nextStart, hit, wit) && fontSize > 20)
            {

                //Console.WriteLine("make it smaller");
                fontSize -= 10;
                wordFont = new Font(fontString, fontSize, fontstyle);

                siz = new SizeF(flagGraphics.MeasureString(word.actualWord, wordFont).Width - (fontSize * 0.45f) + 10, flagGraphics.MeasureString(word.actualWord, wordFont).Height * 0.6f + 10);
                hit = (int)((siz.Height + 10) / 10);
                wit = (int)((siz.Width + 10) / 10);
                nextStart = new PointF(tempX - (wit * 10 - 10), nextStart.Y);
            }

            if (checkFit(nextStart, hit, wit))
            {
                tempWordCount++;
                word.place = new Position(flagGraphics.MeasureString(word.actualWord, wordFont), nextStart.X, nextStart.Y);
                modifyY = word.place.size.Height * 0.14f;
                modifyX = fontSize * 0.2f;
                word.place.size = new SizeF(word.place.size.Width - (fontSize * 0.45f), word.place.size.Height * 0.6f);
                //modifyY = words[i].place.size.Height * 0.14f;
                //modifyX = fontSize * 0.2f;
                //words[i].place.size = new SizeF(words[i].place.size.Width - (fontSize * 0.45f), words[i].place.size.Height * 0.6f);
                fillArray(word.place.size, word.place.pos);
                //Console.WriteLine(words[i].place.size + " at " + words[i].place.pos);
                PointF tempPoint = new PointF(word.place.pos.X - modifyX, word.place.pos.Y - modifyY);
                Brush tempColor = randColor();
                flagGraphics.DrawString(word.actualWord, wordFont, tempColor, tempPoint);

                if (wordSize)
                    wordsToResize.Add(new customBackground(word.actualWord, wordFont, tempColor, tempPoint));
                //temp
                //flagGraphics.DrawRectangle(new Pen(Color.Blue), new Rectangle((int)words[i].place.pos.X, (int)words[i].place.pos.Y, (int)words[i].place.size.Width, (int)words[i].place.size.Height));
                word.used = true;
                modifyX = 0;
                modifyY = 0;
            }
            else
            {
                word.place = new Position(new SizeF(0, 0), 0, 0);
                if (fontSize <= 20)
                {
                    lastRun(word);
                }
            }
        }

        //this function selects a random color from the list of colors
        //provided by the user (from the check boxes)
        //the color returned is used to draw the string
        Brush randColor()
        {
            int temp = r.Next(0, 512) % colors.Count;

            return colors[temp];
        }

        //function fills picSize array which is used to keep
        //track of used and unused locations
        //also increases area
        void fillArray(SizeF size, PointF point)
        {
            int xPoint = (int)(point.X / 10);
            int yPoint = (int)(point.Y / 10);
            int xLimit = (int)((size.Width + 10) / 10) + xPoint;
            int yLimit = (int)((size.Height + 10) / 10) + yPoint;

            for (int i = xPoint; i < xLimit; i++)
            {
                for(int j = yPoint; j < yLimit; j++)
                {
                    
                    picSize[j, i] = 1;
                    area++;
                }
            }
        }
        
        //same as findLastRun but uses a gap of 5 x 3 for check area
        PointF findNextPos()
        {
            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    if (picSize[j, i] == 0 && blankMap[j, i] == false)
                    {
                        //3 from bottom or 3 / 2 / 1 gap
                        if (!checkArea(i, j, 5, 3))
                        {
                            picSize[j, i] = 2;
                        }
                        else
                        {
                            return new PointF(i * 10, j * 10);
                        }
                    }
                }
            }
            return new PointF();
        }

        //finds nect position from the right
        //i will probably end up changing part of this function
        PointF findRightNextPos()
        {
            int temp;
            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    temp = pictureHeight - 1 - i;
                    if (picSize[j, temp] == 0 && blankMap[j, temp] == false)
                    {
                        //3 from bottom or 3 / 2 / 1 gap
                        //this if statement is the part i will eventually change
                        if (j >= pictureWidth - 3 || picSize[j + 1, temp] == 1 || picSize[j + 2, temp] == 1
                            || temp <= 5 || picSize[j, temp - 1] == 1 || picSize[j, temp - 2] == 1 || picSize[j, temp - 3] == 1 || picSize[j, temp - 4] == 1 || picSize[j, temp - 5] == 1)
                        {
                            picSize[j, i] = 2;
                        }
                        else
                        {
                            return new PointF(temp * 10, j * 10);
                        }
                    }
                }
            }
            return new PointF();
        }

        //function checks if given heght and width fit within 
        //the picSize array at point start
        bool checkFit(PointF start, int height, int width)
        {
            int startx = (int)(start.X / 10);
            int starty = (int)(start.Y / 10);

            if (startx + width > pictureHeight)
            {
                //Console.WriteLine("outofbounds");
                return false;
            }
            if (starty + height > pictureWidth)
            {
                //Console.WriteLine("outofbounds size {0}", height);
                return false;
            }

            //outofbounds
            if (startx < 0)
                return false;

            //checks if width and height fit inside picSize array
            for (int i = startx; i < startx + width; i++)
            {
                for (int j = starty; j < starty + height; j++)
                {
                    if (picSize[j,i] == 1)
                    {
                        //Console.WriteLine("Cant fit size {0}", height);
                        return false;
                    }
                }
            }

            return true;
        }

        //assigns sizes to words
        //sizes depend on the run number
        //first run cuts words list into seven parts
        //each word in each part is given the same size
        //size varies from 8 to 2
        //example:
        //list of size 21 gets cut into 7 sections of 3
        //first section of 3, the three words get size 8
        //second section of 3, the three words get size 7
        //and so on
        //this works since 
        //
        //second run just assigns all word sizes of 4
        //third run assigns word sizes 2
        //fourth run assigns word sizes 1
        //each size eventually gets multiplied by 10
        private void assignSize(List<Word> tempWords, int run)
        {
            if (run == 1)
            {
                int div = tempWords.Count / 7;
                for (int i = 0; i < 6; i++)
                {
                    loopSize(i, div, i * div, tempWords);
                }
            }
            else if (run == 2)
            {
                foreach(Word temp in tempWords)
                {
                    temp.fontSize = 4;
                }
            }
            else if (run == 3)
            {
                foreach (Word temp in tempWords)
                {
                    temp.fontSize = 2;
                }
            }
            else if (run == 3)
            {
                foreach (Word temp in tempWords)
                {
                    temp.fontSize = 1;
                }
            }

        }

        //loops through each section of words giving each word same size
        private void loopSize(int val, int times, int start, List<Word> temp)
        {
            for(int i = start; i < start + times; i++)
            {
                temp[i].fontSize = (8 - val);
            }
        }

        //function shuffles list of words randomly
        public void shuffle(List<Word> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                Word value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        //custom image from user is specified
        //must be a png,bmp,gif or jpg
        //must be a white background
        //foreground must be black
        private void customImage(string filename)
        {
            Bitmap temp = (Bitmap)Image.FromFile(filename);
            int tempCount;
            float xSection = (float)temp.Size.Width / (float)(pictureHeight);
            float ySection = (float)temp.Size.Height / (float)(pictureWidth);
            int areaSection = (int)(xSection * ySection) / 2;

            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    tempCount = 0;
                    for (int x = 0; x < xSection; x++)
                    {
                        for (int y = 0; y < ySection; y++)
                        {
                            if (temp.GetPixel((int)(i * xSection + x), (int)(j * ySection + y)).ToArgb() == Color.Black.ToArgb())
                                tempCount++;
                        }
                    }
                    if (tempCount < areaSection)
                    {
                        blankMap[j, i] = true;
                        blankArea++;
                    }
                    //temp.SetPixel(i, j, Color.Black);
                }
            }
        }


        //function for circle image
        public void circle()
        {
            flagGraphics.FillEllipse(Brushes.Black, new Rectangle(0, 0, pictureHeight, pictureWidth));
            picture.Image = flag;
            
            for (int i = 0; i < pictureHeight; i++)
            {
                for(int j = 0; j < pictureWidth; j++)
                {
                    if (!(flag.GetPixel(i, j).R == 0))
                    {
                        //flag.SetPixel(i, j, Color.Blue);
                        blankMap[j, i] = true;
                        blankArea++;
                    }
                }
            }
            flagGraphics.Clear(bgColor);
            /*
            int tempI;
            int tempJ;
            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    tempI = pictureHeight / 2 - i;
                    tempJ = pictureWidth / 2 - j;
                    if (Math.Sqrt(tempI * tempI + tempJ * tempJ) > pictureHeight / 2)
                    {
                        picSize[j, i] = 1;
                        area++;
                    }
                }
            }
            */
        }

        //function for triangle image
        public void triangle()
        {
            Point[] points = new Point[3];
            points[0] = new Point(pictureHeight / 2, 0);
            points[1] = new Point(pictureHeight, pictureWidth);
            points[2] = new Point(0, pictureWidth);
            flagGraphics.FillPolygon(Brushes.Black, points);
            picture.Image = flag;

            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    if (!(flag.GetPixel(i, j).R == 0))
                    {
                        //flag.SetPixel(i, j, Color.Blue);
                        blankMap[j, i] = true;
                        blankArea++;
                    }
                }
            }
            flagGraphics.Clear(bgColor);

            /*
            int count;
            for (int i = 0; i < pictureHeight / 2; i++)
            {
                count = pictureHeight / 2 - i;
                for (int j = 0; j < count; j++)
                {
                    //picSize[j, i * 2] = 1;
                    //picSize[j, i * 2 + 1] = 1;
                    //picSize[69 - j, i * 2] = 1;
                    //picSize[69 - j, i * 2 + 1] = 1;
                    area += 4;
                    picSize[i * 2, j] = 1;
                    picSize[i * 2 + 1, j] = 1;
                    picSize[i * 2, pictureHeight - 1 - j] = 1;
                    picSize[i * 2 + 1, pictureHeight - 1 - j] = 1;
                }
            }
            */
        }

        //function for square image
        public void square()
        {
            for (int i = 0; i < pictureHeight; i++)
            {
                for (int j = 0; j < pictureWidth; j++)
                {
                    blankMap[j, i] = false;
                }
            }
        }
    }
}
