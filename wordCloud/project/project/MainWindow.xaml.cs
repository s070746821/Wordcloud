using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Word> words = new List<Word>();
        List<Word> blackList = new List<Word>();
        List<System.Drawing.Brush> brushes = new List<System.Drawing.Brush>();

        bool[] colors = new bool[8];

        string filename;
        string backFilename;

        public MainWindow()
        {
            InitializeComponent();
            button.IsDefault = true;

            System.Drawing.FontFamily[] fontList = new InstalledFontCollection().Families;

            //gets every font on the system
            for (int f = 0; f < fontList.Length; f++)
                fontsList.Items.Add(fontList[f].Name);

            //gets every color on the system
            //these colors are only for background

            List<string> list = new List<string>();

            foreach (var color in Enum.GetValues(typeof(KnownColor)))
                list.Add(color.ToString());

            list.Add("Custom Background");
            list.Sort();
            list = list.Distinct().ToList();

            foreach (string i in list)
                bgColor.Items.Add(i);


            //adds shape to 
            shapeList.Items.Add("Square");
            shapeList.Items.Add("Circle");
            shapeList.Items.Add("Triangle");
            shapeList.Items.Add("Heart");
            shapeList.Items.Add("Custom");

            capitalization.Items.Add("Unchanged");
            capitalization.Items.Add("ALL CAPS");
            capitalization.Items.Add("all lower");

            //sets default values of comboboxes
            shapeList.SelectedIndex = shapeList.Items.IndexOf("Square");
            bgColor.SelectedIndex = bgColor.Items.IndexOf("White");
            fontsList.SelectedIndex = fontsList.Items.IndexOf("Arial");
            capitalization.SelectedIndex = capitalization.Items.IndexOf("Unchanged");

            //all the words of the blacklist
            blackList.Add(new Word("the"));
            blackList.Add(new Word("of"));
            blackList.Add(new Word("were"));
            blackList.Add(new Word("a"));
            blackList.Add(new Word("by"));
            blackList.Add(new Word("in"));
            blackList.Add(new Word("is"));
            blackList.Add(new Word("and"));
            blackList.Add(new Word("to"));
            blackList.Add(new Word("it"));
            blackList.Add(new Word("are"));
            blackList.Add(new Word("was"));
            blackList.Add(new Word("for"));
            blackList.Add(new Word("on"));
            blackList.Add(new Word("or"));
            blackList.Add(new Word("which"));
            blackList.Add(new Word("but"));
            blackList.Add(new Word("him"));
            blackList.Add(new Word("has"));
            blackList.Add(new Word("her"));
            blackList.Add(new Word("hers"));
            blackList.Add(new Word("his"));
            blackList.Add(new Word("that"));
            blackList.Add(new Word("with"));
            blackList.Add(new Word("its"));
            blackList.Add(new Word("be"));
            blackList.Add(new Word("have"));
            blackList.Add(new Word("not"));
            blackList.Add(new Word("he"));
            blackList.Add(new Word("I"));
            blackList.Add(new Word("as"));
            blackList.Add(new Word("you"));
            blackList.Add(new Word("do"));
            blackList.Add(new Word("at"));
            blackList.Add(new Word("by"));
            blackList.Add(new Word("from"));
            blackList.Add(new Word("they"));
            blackList.Add(new Word("we"));
            blackList.Add(new Word("say"));
            blackList.Add(new Word("an"));
            blackList.Add(new Word("my"));
            blackList.Add(new Word("all"));
            blackList.Add(new Word("would"));
            blackList.Add(new Word("should"));
            blackList.Add(new Word("there"));
            blackList.Add(new Word("what"));
            blackList.Add(new Word("their"));
            blackList.Add(new Word("so"));
            blackList.Add(new Word("if"));
            blackList.Add(new Word("about"));
            blackList.Add(new Word("them"));
            blackList.Add(new Word("your"));
            blackList.Add(new Word("some"));
            blackList.Add(new Word("come"));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            byte caps = 0;
            if (capitalization.SelectedItem.ToString() == "ALL CAPS")
                caps = 1;
            else if (capitalization.SelectedItem.ToString() == "all lower")
                caps = 2;

            brushes.Clear();
            words.Clear();
            //delims to split text by
            //Starts new Dialog, and gets the return value

            string[] input = new string[1];
            using (var form = new TextInput())
            {
                var result = form.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    input = form.values;            //values preserved after close
                }
                else
                {
                    System.Windows.MessageBox.Show("You left the TextBox Empty!");
                    return;
                }

            }
            //End of Dialog


            //Starts new thread to display splashscreen
            bool wordExists;
            bool blackListExist;
            //add first word to List<Word> words
            //words.Add(new Word(input[0]));
            //loop through rest of words of input text
            for (int i = 0; i < input.Length; i++)
            {
                blackListExist = false;
    
                foreach(Word t in blackList)
                {
                    if (String.Equals(input[i],t.actualWord, StringComparison.OrdinalIgnoreCase) || input[i].Length <=2)
                    {
                        blackListExist = true;
                        break;
                    }
                }

                if (!blackListExist)
                {
                    //assume input[i] doesnt exist in List<Word> words
                    wordExists = false;
                    //loop through all Word objects in List<Word> words
                    foreach (Word j in words)
                    {
                        //if input[i] is equal to the actualWord element of j
                        //j is a Word in List<Word> words
                        if (j.actualWord == input[i])
                        {
                            //if the strings match, increment the count of j
                            //set wordExists to true and break out of foreach loop
                            j.count++;
                            wordExists = true;
                            break;
                        }
                    }
                    //if input[i] did not match any actualWord variable in List<Word> words
                    if (!wordExists)
                    {
                        //create new entry for List<Word> words
                        string temp = input[i];
                        if (caps == 1)
                        {
                            temp = temp.ToUpper();
                        }
                        else if (caps == 2)
                        {
                            temp = temp.ToLower();
                        }
                        words.Add(new Word(temp));
                    }
                }
            }

            //checks for all colors used for words
            if (checkBox.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Black);
            if (checkBox1.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Gray);
            if (checkBox2.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Orange);
            if (checkBox3.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Red);
            if (checkBox4.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Purple);
            if (checkBox5.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Blue);
            if (checkBox6.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Green);
            if (checkBox7.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.Yellow);
            if (checkBox8.IsChecked == true)
                brushes.Add(System.Drawing.Brushes.White);

            //if they have no color selected, makes black default
            if (brushes.Count == 0)
            {
                checkBox.IsChecked = true;
                brushes.Add(System.Drawing.Brushes.Black);
            }

            //gets shape to be used

            if (shapeList.SelectedItem.ToString() != "Custom")
                filename = AppDomain.CurrentDomain.BaseDirectory;

            if (shapeList.SelectedItem.ToString() == "Square")
                filename += "\\square.png";
            else if (shapeList.SelectedItem.ToString() == "Triangle")
                filename += "\\triangle.png";
            else if (shapeList.SelectedItem.ToString() == "Circle")
                filename += "\\circle.png";
            else if (shapeList.SelectedItem.ToString() == "Heart")
                filename += "\\heart.png";

            bool transparent = false;
            System.Drawing.Color color = System.Drawing.Color.White;
            
            if (trans.IsChecked == true)
            {
                transparent = true;
            }

            if (bgColor.SelectedItem.ToString() != "Custom Background")
            {
                color = System.Drawing.Color.FromName(bgColor.SelectedItem.ToString());
                backFilename = null;
            }

            int maxCount = ((int)Math.Sqrt(int.Parse(widthtext.Text) * int.Parse(heighttext.Text))) / 7;
            //Console.WriteLine(maxCount);
            if (words.Count > maxCount)
            {
                words.RemoveRange(maxCount, words.Count - maxCount);
            }
            else
            {
                int tempCount = 0;
                while (words.Count < maxCount)
                {
                    words.Add(new Word(words[tempCount++].actualWord));
                }
            }

            byte fontStyle = 0;
            if (bold.IsChecked == true && ital.IsChecked == true)
                fontStyle = 3;
            else if (ital.IsChecked == true)
                fontStyle = 2;
            else if (bold.IsChecked == true)
                fontStyle = 1;


            int scale = 1;

            if (times1.IsChecked == true)
                scale = 1;
            else if (times2.IsChecked == true)
                scale = 2;
            else if (times3.IsChecked == true)
                scale = 3;
            else if (times4.IsChecked == true)
                scale = 4;

            //this is where the tranistion to the winform happens
            //as soon as the object is created, the calculations begin
            //first arg is the list of words
            //second arg is the name of the font
            //third arg is shape
            //forth arg is list of colors used for words
            //fifth arg is width but make sure its the height value
            //sixth arg is height but make sure its the width value
            //seventh arg is background color

            main pic = new main(words, fontsList.SelectedItem.ToString(), brushes, int.Parse(heighttext.Text), int.Parse(widthtext.Text), color, filename, fontStyle, transparent, scale, backFilename);

            while (pic.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                pic = new main(words, fontsList.SelectedItem.ToString(), brushes, int.Parse(heighttext.Text), int.Parse(widthtext.Text), color, filename, fontStyle, transparent, scale, backFilename);
            }


            //draw pic = new draw(int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            //pic.ShowDialog();

        }

        private void customImage()
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Image Files (*.png, *.jpg, *.bmp, *.gif)|*.png;*.jpg;*.bmp;*.gif";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            customLabel.Content = ((FileStream)myStream).Name;
                            filename = customLabel.Content.ToString();
                            widthtext.Text = System.Drawing.Image.FromStream(myStream).Size.Width.ToString();
                            heighttext.Text = System.Drawing.Image.FromStream(myStream).Size.Height.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else
                shapeList.SelectedIndex = shapeList.Items.IndexOf("Square");
        }

        private void shapeList_DropDownClosed(object sender, EventArgs e)
        {
            if (shapeList.SelectedItem.ToString() == "Custom")
            {
                System.Windows.MessageBox.Show("Only the color black counts as the shape.\nEverything else is ignored.");
                customImage();
            }
            else
                customLabel.Content = "";
        }

        private void bgColor_DropDownClosed(object sender, EventArgs e)
        {
            if (bgColor.SelectedItem.ToString() == "Custom Background")
            {
                System.Windows.MessageBox.Show("Background is not resized to fit dimensions.");
                getCustomBack();
            }
            else
                customBack.Content = "";
        }

        private void getCustomBack()
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Image Files (*.png, *.jpg, *.bmp, *.gif)|*.png;*.jpg;*.bmp;*.gif";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            customBack.Content = ((FileStream)myStream).Name;
                            backFilename = customBack.Content.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else
                bgColor.SelectedIndex = bgColor.Items.IndexOf("White");
        }
    }
}
