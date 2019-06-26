using JobInfo.XROGi_Extensions;
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
    public partial class FormChatInfo : Form
    {
        Job job;
        XROGi_Class.Chat o;
        public FormChatInfo()
        {
            InitializeComponent();
        }

        internal void ShowChat(Job _job, XROGi_Class.Chat _o)
        {
            if (_o != null)
            {
                //XROGi_Class.Chat c = _job.Chats.Where(s => s.ObjId.ObjId == _o).FirstOrDefault();
                textBox1.Text = _o.ObjId.GetText();
                //"Новое имя"
                job = _job;
                o = _o;
            }
            else
            {
                MessageBox.Show("не выбран чат");
            }
         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                job.Chat_Rename(o, textBox1.Text);

                Close();


           /* }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); ChatDisconnected(); }
            catch (Exception err) { E(err); }
            */

        }catch (Exception err)
            {
                if (err.InnerException != null)
                {
                    string f = err.InnerException.ToString();
                    MessageBox.Show("Возникла ошибка во время переименования. Возможно Для этого вида чата, переименование не поддерживается");
                }
                else
                {
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
