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
    public partial class FormInfoMsg : Form
    {
        XMessageCtrl mc;
        Job job;


        public FormInfoMsg()
        {
            InitializeComponent();
            this.KeyPreview = true;
           /* this.KeyPress += (ss, ee) =>

            {

                if (ee.KeyChar == 27) //= escape

                {

              //      Environment.Exit(1);

                }

            };*/
            tabControl1.SelectedTab = tabPage2;
        }

        internal void Set_FormParam(Job job, XMessageCtrl mc)
        {
            try
            {
                #region MyRegion
                this.mc = mc;
                this.job = job;
                //    throw new NotImplementedException();


                DataTable t = new DataTable();
                t.Columns.Add("Наименование параметра");
                t.Columns.Add("Значение параметра");
                CreateParameter(t, "Создатель", mc.MessageObj.userCreater);
                CreateParameter(t, "Дата создания", mc.MessageObj.period.dtc.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                CreateParameter(t, "Длина сообщения, байт", mc.MessageObj.GetText().Length.ToString());
                CreateParameter(t, "ID", mc.MessageObj.ObjId.ToString());
                //        BindingSource SBind = new BindingSource();
                //SBind.DataSource = t;
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = t;
                //   ServersTable.DataSource = SBind;
                dataGridView1.Refresh();

                WS_JobInfo.view_ObjStatus_HistoryInfo [] ret = job.Message_GetStatusHistory((int)mc.MessageObj.ObjId);
                if (ret!=null)
                {
                    dataGridView_shown.AutoGenerateColumns = false;
                    dataGridView_shown.DataSource = ret;
                    dataGridView_shown.Refresh();

                    /*DataTable = new DataTable();
                    t.Columns.Add("Наименование параметра");
                    t.Columns.Add("Значение параметра");
                    CreateParameter(t, "Создатель", mc.MessageObj.userCreater);
                    CreateParameter(t, "Дата создания", mc.MessageObj.period.dtc.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    CreateParameter(t, "Длина сообщения, байт", mc.MessageObj.GetText().Length.ToString());
                    */

                }

                RequestMessageLog(dataGridView_ShowLog);
                #endregion
            }
            catch (Exception)
            {

                
            }

        }

        private void RequestMessageLog(DataGridView dataGridView_ShowLog)
        {
            //job.ge
        }

        private void CreateParameter(DataTable t, string v, string userCreater)
        {
            var r = t.NewRow();
            r.BeginEdit();
            r[0] = v;
            r[1] = userCreater;
            /*
            foreach (DataColumn c in t.Columns)

            {

                r[c.ColumnName] = "Тест"//writing values

            }
            */
            r.EndEdit();
            t.Rows.Add(r);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(mc.MessageObj.GetText());
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(mc._this_bitmap, 0, 0);
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void FormInfoMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void FormInfoMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Escape)
            {
                Close();
            }
        }
    }
}
