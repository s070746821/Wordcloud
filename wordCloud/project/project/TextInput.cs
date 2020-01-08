using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project
{
    public partial class TextInput : Form
    {


        public string[] values;
        public TextInput()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            char[] delims = { ' ', '.', ',', '\t', '(', ')' };
            values = InputBox.Text.Split(delims);

            if (string.IsNullOrWhiteSpace(InputBox.Text))
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else this.DialogResult = DialogResult.OK;
            this.Close();


        }
    }
}
