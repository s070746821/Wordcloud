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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace project
{
    /// <summary>
    /// Interaction logic for imageOptions.xaml
    /// </summary>
    public partial class imageOptions : Window
    {

        public bool sameSet { get; set; }

        public System.Drawing.Image image { get; set; }

        public imageOptions()
        {
            InitializeComponent();
            this.comboBox.Items.Add(".png");
            this.comboBox.Items.Add(".jpg");
            this.comboBox.Items.Add(".gif");
            this.comboBox.Items.Add(".bmp");
            this.comboBox.SelectedIndex = this.comboBox.Items.IndexOf(".png");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string temp = textBox.Text;
            if (temp.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                System.Windows.MessageBox.Show("Invalid filename");
            }
            else
            {
                string downloads = KnownFolders.GetPath(KnownFolder.Downloads);
                downloads += "\\" + textBox.Text + comboBox.SelectedItem.ToString();
                image.Save(downloads);
                System.Windows.MessageBox.Show("Image Downloaded");
                this.Close();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            sameSet = true;
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            sameSet = false;
            this.Close();
        }
    }
}
