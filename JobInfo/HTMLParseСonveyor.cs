using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo
{
  
    public class HTMLParseСonveyor : Queue
    {
        static string DebugPath = "c:\\temp\\chat\\";
        int Width;
        int Height;
        private WebBrowser wb;

        public delegate void OnCompleatConvertDelegate(MessageLive ml, Bitmap result);
        public event OnCompleatConvertDelegate OnCompleatConvert = delegate { };

        public delegate void OnCompleatFinishDelegate();
        public event OnCompleatFinishDelegate OnCompleatFinish = delegate { };
        string css;
        public const int const_MinHeighText = 50; // минимальная шмрмна сообщения в пикселях
        public HTMLParseСonveyor(int _Width, int _Height)
        {
            Width = _Width; Height = _Height;
            wb = new WebBrowser();
            wb.Width = Width;
            wb.BackColor = Color.LightGray;
            wb.Height = Height;// wb.Height+200;
            wb.ScrollBarsEnabled = false;

            wb.MinimumSize = new Size(50, 50);
            //                MaximumSize = s;
            //      wb.DocumentCompleted += wb_DocumentCompleted;
            wb.DocumentText = "0";
            //wb.DocumentText =  
            //      wb.Document.OpenNew(true);
            //  //       
            css = "img {max-width: 40px;height:auto;max-height:40px;}";
        }
       
        public void DoNow()
        {
            while (this.Count > 0)
            {
                WS_JobInfo.Obj o = null;
                object ob = Dequeue();
                if (ob != null)
                {
                    MessageLive mc = ob as MessageLive;
                    if (mc != null)
                        o = mc.obj;
                    else 
                        o = ob as WS_JobInfo.Obj;

                    if (o != null)
                    {

                    }
                    string HTML = XParse.ParseMessageToHtml(o.xml);
         //           HTML = HTML.Replace("\n", "<BR/>");
                    
                    // 222 MC!!!!!!
                    //      wb.Navigate("about:blank");
                    //       wb.Refresh();
                    wb.Document.OpenNew(true);
                    wb.DocumentText="";

                    wb.Document.Write(HTML);
                    if (wb.Document.Body != null)
                    {
                        var images1 = wb.Document.Body.GetElementsByTagName("img");
                        if (images1.Count > 0)
                        {
                            HtmlElement el = images1[0];
                        }
                    }else

                    {

                    }
                    wb.Refresh();

                    if (wb.Document.Body != null)
                    {
                        var rrrrrrr = wb.Document.Body.GetElementsByTagName("div");
                        if (rrrrrrr.Count > 0)
                        {
                            Rectangle r1111 = rrrrrrr[0].OffsetRectangle;// wb.Document.Body.ScrollRectangle;
                            if (r1111.Height == 0)
                                r1111.Height = const_MinHeighText;

                            Rectangle r = new Rectangle(0, 0, r1111.Left + r1111.Width + 10, r1111.Top + r1111.Height + 10);//  wb.Document.Body.ScrollRectangle;
                            Rectangle r_Wb = wb.Document.Body.ScrollRectangle;
                            Bitmap bitmap = new Bitmap(r1111.Left + r1111.Width + 1, r1111.Top + r1111.Height + 1);

                            //77 MC !!!!
                            wb.DrawToBitmap(bitmap, r);

                            try
                            {
                                Bitmap eeeee = bitmap.Clone(r1111, bitmap.PixelFormat);
                                Graphics g = Graphics.FromImage(eeeee);

                                //           if (Directory.Exists(DebugPath))
                                //                 eeeee.Save(DebugPath + "\\1.png", System.Drawing.Imaging.ImageFormat.Png);
                                OnCompleatConvert(mc, eeeee);
                            }
                            catch (Exception err)
                            {
                                //              E(err);
                                OnCompleatConvert(mc, null);
                            }

                        }
                        else

                            OnCompleatConvert(mc, null);

                    }
                    else
                        OnCompleatConvert(mc, null);

                }

                }
            OnCompleatFinish();
        }
   



      

    }
}
