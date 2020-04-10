using JobInfo.XROGi_Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo
{
    class BrowserToImage    
    {
        WebBrowser wb;
        BrowserToImage  ()
        {
            wb = new WebBrowser();
        }
        public  Bitmap ParseHTML_GetImage(WS_JobInfo.Obj o, int Width, int Height, XMessageCtrl c)
        {
            if (o.ObjId == 168032)
            {
            }

            Size s = new Size(Width - 140, Height);
            //       Size s = new Size(300, Height);
            if (c.Text == string.Empty)
                c.Text = o.GetString(GraphicsChatExtensions.XTypeStringInfo.Text_XML);

            bool b_smile = false;
            bool b_text = false;
            bool b_image = false;
            {
                b_text = o.isText();
                b_smile = o.isSmile();
                b_image = o.isImage();
                c.b_ImageMsg = b_image;
                //if (b_text) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                //if (b_smile) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();

            }

       //     WebBrowser wb;
            //    if (wb==null)
       //     wb = new WebBrowser();
            wb.Width = s.Width;

            wb.BackColor = Color.LightGray;
            wb.Height = s.Height;// wb.Height+200;
            wb.ScrollBarsEnabled = false;

            wb.MinimumSize = new Size(50, 50);
            //                MaximumSize = s;
            //      wb.DocumentCompleted += wb_DocumentCompleted;
            wb.DocumentText = "0";
            //wb.DocumentText =  
            //      wb.Document.OpenNew(true);
            //  //       
            string css = "img {max-width: 40px;height:auto;max-height:40px;}";
            if (true)
            {
                //           wb.Document.Write("<html><body><div style='background-color:lightblue;float:left;margin:10px;'>" + o.xml + "</div></body></html>");// o.xml;);

                //  https://codepen.io/josy-star/pen/xwdddP
                if (o.ObjId == 64614)
                {

                }
                c.HTML = XParse.ParseMessageToHtml(o.xml);


                // 222 MC!!!!!!
                wb.Document.Write(c.HTML);// o.xml;);


                if ((Environment.UserName == "iu.smirnov"))//&& Control.ModifierKeys == Keys.Shift)
                {

                    if (true)
                    {
                        //         File.WriteAllText("c:\\temp\\drop.html", HTML);
                    }
                    /*                    File.WriteAllText("C:\\temp\\chat\\msg\\" + o.ObjId.ToString() + "_0.html", Text );
                                        File.WriteAllText("C:\\temp\\chat\\msg\\" + o.ObjId.ToString() + "_1.html", html );
                                        File.WriteAllText("C:\\temp\\chat\\msg\\" + o.ObjId.ToString() + "_2.html", wb.Document.Body.Parent.OuterHtml, Encoding.GetEncoding(wb.Document.Encoding));
                      */
                }

                //wb.Document.Write("<html><head>" + css + "</head><body><div style='float:left;margin:10px;'>" + o.xml + "</div></body></html>");// o.xml;);
            }
            else
            {

                wb.Navigate(@"C:\Temp\chat\1.html");
                while (wb.ReadyState != WebBrowserReadyState.Complete)
                {
                    //Debug.WriteLine("Loading loop..");
                    Application.DoEvents();
                }
            }
            //           wb.Refresh();

            //      Size web_size = wb.Document.ScrollRectangle.Size;// wb.GetPreferredSize(s);
            //GetElementsByTagName
            //       Rectangle r = wb.Document.Body.ScrollRectangle;
            var images1 = wb.Document.Body.GetElementsByTagName("img");
            if (images1.Count > 0)
            {
                //     HTMLElement img = images[0] HTMLImageElement;
                HtmlElement el = images1[0];
                //       el.Style = "max-width:40px;height:auto;max-height:40px;";
                /* Закоментировал блок из за файла на Д. Не пойму зачем и что это. Тест??

                               byte[] AsBytes = File.ReadAllBytes("D:\\Work\\icon\\pause.png");
                               String AsBase64String = Convert.ToBase64String(AsBytes);
                               string src = "data:image/png;base64," + AsBase64String;
                               string file = el.GetAttribute("fileid");
                               string ver = el.GetAttribute("ver");
                               if (ver == "1.0")
                               {
                                   //      o.i
                                   //if (images["file"])
                                   //         images[0].SetAttribute("src", @"http://cyberstatic.net/images/misc/tick.png");
                                   el.SetAttribute("src", src);
                                   el.SetAttribute("height", "40");
                                   el.SetAttribute("style", "max-width:40px;height:auto;max-height:40px;");
                                   el.Id = "TestXROGi";
                               }
               */
            }
            wb.Refresh();
            var rrrrrrr = wb.Document.Body.GetElementsByTagName("div");
            if (rrrrrrr.Count > 0)
            {
                Rectangle r1111 = rrrrrrr[0].OffsetRectangle;// wb.Document.Body.ScrollRectangle;
                if (r1111.Height == 0)
                    r1111.Height = XMessageCtrl.MinHeighText;

                //<img ver =\"1.0\" fileid=\"5b4842c4-a751-4c2a-a211-b0dc945068f1\" />

                //wb.DrawToBitmap(bitmap, new Rectangle(0, 0, r.Width, r.Height));
                //          r.Top = r1111.Top;

                //wb.Document.Body.ScrollRectangle.Width
                //            Rectangle r = new Rectangle(r1111.Left, r1111.Top, wb.Document.Body.ScrollRectangle.Width, wb.Document.Body.ScrollRectangle.Height);//  wb.Document.Body.ScrollRectangle;
                Rectangle r = new Rectangle(0, 0, r1111.Left + r1111.Width + 10, r1111.Top + r1111.Height + 10);//  wb.Document.Body.ScrollRectangle;
                Rectangle r_Wb = wb.Document.Body.ScrollRectangle;
                Bitmap bitmap = new Bitmap(r1111.Left + r1111.Width + 1, r1111.Top + r1111.Height + 1);


                //77 MC !!!!
                wb.DrawToBitmap(bitmap, r);

                try
                {
                    Bitmap eeeee = bitmap.Clone(r1111, bitmap.PixelFormat);
                    Graphics g = Graphics.FromImage(eeeee);
                    /*RectangleF rectf = new RectangleF(50, 0, 150, 150);

                    g.DrawString("id="+ _objid.ToString(), new Font("Tahoma", 8), Brushes.Red, rectf);
                    */
                    //                   eeeee.Save("c:\\temp\\chat\\1.jpg", System.Drawing.Imaging.ImageFormat.Png);
                    //     if (true)
                    //         wb.Dispose();

                    return eeeee;
                }
                catch (Exception err)
                {
                    //              E(err);
                }
                return null;
            }
            else
                return null;
        }


    }
}
