using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo
{
    public partial class FormEnterText : Form
    {
        public FormEnterText()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text.Length >= 5)
                {
                    richTextBox1.Focus();
                }
                else
                    MessageBox.Show("Название чата должно содержать ек менее  символов");
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

            private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter && (e.Control || e.Alt))
                {
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    Close();
                }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        internal string GetText()
        {
            return textBox1.Text;
        }

        internal int GetSgnTypeId()
        {
            return 7;
        }

        internal string GetComment()
        {
            return richTextBox1.Text;
        }
    }
}
