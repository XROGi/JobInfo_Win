using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLib;
using ZXing;

namespace JobInfo
{
    public partial class FormQRRequest : Form
    {
        private int userid;

        public FormQRRequest()
        {
            InitializeComponent();
        }

        private void FormQRRequest_Load(object sender, EventArgs e)
        {
            DomainDataClassesDataContext d = new DomainDataClassesDataContext();
            proc_TokenReq_AddResult [] res =  d.proc_TokenReq_Add(userid).ToArray();
            if (res != null && res.Length>=1)
            {
                string newid = res[0].Column1;
                BarcodeWriter b = new BarcodeWriter()
                { Format = BarcodeFormat.QR_CODE };
                string req =  newid+ "&";
                //            string req = "http://jobinfo.svodint-ru/ji/xml/&09A5EEAA-E57B-4422-82CA-65B2AC7DE565";
                string req2 = XCrypt.EncryptStringAES(req, Application.ProductName);
                b.Options.Width = 250;
                b.Options.Height = 250;
                pictureBox1.Image = b.Write(req2);
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        internal void SetUsedId(int v)
        {
            userid = v;
        }
    }
}
