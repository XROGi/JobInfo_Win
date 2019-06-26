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
    public partial class FormDebug : Form
    {
        public delegate void LockalAddDelegate( string Msg);
        public event LockalAddDelegate LockalAddEvent = delegate { };

        Queue<string> q = new Queue<string>();

        public Delegate LockalAdd { get
                ; private set;
        }

        public FormDebug()
        {
            InitializeComponent();
            LockalAddEvent += LockalAddMethod;
        }

        private void LockalAddMethod(string Msg)
        {
            if (isShown)
            {
                listBox1.Items.Add(Msg);
            }
            else
            {
                q.Enqueue(Msg);
                while (q.Count >= 150)
                {
                    q.Dequeue();
                }
            }
        }

        internal void AddMsg(string v)
        {
            try
            {
                try
                {
                    if (Handle!=null)
                        BeginInvoke(LockalAddEvent, v);
                    else
                    {
                        listBox1.Items.Add(v);
                    }
                }
                catch (Exception err)
                {
                    listBox1.Items.Add(v);
               //     listBox1.Items.Add(err.Message.ToString());
                }
            }catch
            {

            }
        }

        private void вФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                const string sPath = @"c:\temp\save.txt";

                System.IO.StreamWriter SaveFile = new System.IO.StreamWriter(sPath);
                foreach (var item in listBox1.Items)
                {
                    SaveFile.WriteLine(item);
                }

                SaveFile.Close();

                MessageBox.Show("Programs saved!");
            }catch (Exception err)
            {

            }
            */
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void КопироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(
            listBox1.SelectedItem.ToString()
            );
        }

        bool isShown = false;
        private void FormDebug_Shown(object sender, EventArgs e)
        {
            isShown = true;
            while (q.Count>0)
            {
                string s = q.Dequeue(); ;
                listBox1.Items.Add(s);
            }
            
        }
    }
}
