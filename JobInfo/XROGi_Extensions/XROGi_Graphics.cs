using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
using System.Xml.Linq;
using System.Xml.XPath;

namespace JobInfo.XROGi_Extensions
{
    
    public static class GraphicsChatExtensions
    {

        static Font Font_MessageText = new Font("Arial", 10);
        static public Size MeasureDrawObj(this WS_JobInfo.Obj o, int Width, int Height)
        {
            Size s = new Size(Width - 140, Height);

            // Cast the sender object back to ListBox type.
            // Get the string contained in each item.
            /*
            string itemString = (string)theListBox.Items[e.Index];

            // Split the string at the " . "  character.
            string[] resultStrings = itemString.Split('.');
            */
            // If the string contains more than one period, increase the 
            // height by ten pixels; otherwise, increase the height by 
            // five pixels.

            String MsgText = "";
            bool b_smile = false;
            bool b_text = false;
            bool b_image = false;

            {
                b_text = o.isText();
                b_smile = o.isSmile();
                b_image = o.isImage();
                if (b_text) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                if (b_smile) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                if (b_image) MsgText = "Картинка";
            }
            WebBrowser wb = new WebBrowser();
            wb.Width = s.Width;
            wb.BackColor = Color.LightGray;
            wb.Height = s.Width;// wb.Height+200;
            wb.ScrollBarsEnabled = false;

            wb.MinimumSize = new Size(50, 50);
            //                MaximumSize = s;
            wb.DocumentCompleted += wb_DocumentCompleted;
            wb.DocumentText = "0";
            //wb.DocumentText =  
            //      wb.Document.OpenNew(true);
            //  //       
            if (true)
            {
                wb.Document.Write("<html><body><div style='background-color:lightblue;float:left;margin:10px;'>" + o.xml + "</div></body></html>");// o.xml;);
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
            wb.Refresh();

            //      Size web_size = wb.Document.ScrollRectangle.Size;// wb.GetPreferredSize(s);
            //GetElementsByTagName
            var rrrrrrr = wb.Document.Body.GetElementsByTagName("div");
            Rectangle r1111 = rrrrrrr[0].OffsetRectangle;// wb.Document.Body.ScrollRectangle;
                                                         //       Rectangle r = wb.Document.Body.ScrollRectangle;

            //wb.DrawToBitmap(bitmap, new Rectangle(0, 0, r.Width, r.Height));
            //          r.Top = r1111.Top;

            //wb.Document.Body.ScrollRectangle.Width
           //            Rectangle r = new Rectangle(r1111.Left, r1111.Top, wb.Document.Body.ScrollRectangle.Width, wb.Document.Body.ScrollRectangle.Height);//  wb.Document.Body.ScrollRectangle;
           Rectangle r = new Rectangle(0,0,r1111.Left + r1111.Width+10, r1111.Top + r1111.Height+10);//  wb.Document.Body.ScrollRectangle;
            Bitmap bitmap = new Bitmap(r1111.Left + r1111.Width+1, r1111.Top + r1111.Height+1);
            wb.DrawToBitmap(bitmap, r);
            
            Bitmap eeeee = bitmap.Clone(r1111, bitmap.PixelFormat); ;

    //        eeeee.Save("c:\\temp\\chat\\1.jpg", System.Drawing.Imaging.ImageFormat.Png);
            wb.Dispose();
            {
                s.Height = eeeee.Height;
                s.Width = eeeee.Width;
       
                return s;
            }
            if (b_text)
            {

                Rectangle rec = new Rectangle(0, 0, s.Width, s.Height);
                rec.Inflate(-3, -3);
                TextFormatFlags flags =
                TextFormatFlags.HorizontalCenter |
     TextFormatFlags.NoPadding | TextFormatFlags.WordBreak |
     TextFormatFlags.EndEllipsis;
                
                
                    string[] lines = MsgText.Split('\r');

              
                Size size = TextRenderer.MeasureText(MsgText, Font_MessageText, rec.Size, flags);
                if (MsgText.StartsWith("{\\rtf1\\"))
                {
                    s.Width = size.Width;
                    //richTextBox1.Width = e.ItemWidth;
                    /*richTextBox1.Rtf = o.Tag.GetText();
                    Graphics g = Graphics.FromHwnd(richTextBox1.Handle);
                    SizeF f = g.MeasureString(MsgText, richTextBox1.Font);
                    e.ItemHeight = (int)f.Height;
                    richTextBox1.Width = (int)(f.Width) + 5;
                    */
                }
                if (size.Height <= 25)
                    s.Height = 25;
                else
                    s.Height = size.Height;
                s.Width = size.Width+70;
                if (s.Width<=250)
                {
                    s.Width = 250;
                }
         //      s.Height += 10;
            }
          
            if (b_smile)
            {
                s.Height = 35;
                s.Width = 250;
            }

            if (b_image)
            {
                s.Height = 150;
                s.Width  = 150;
            }
            
            return s;


            //e.ItemHeight += e.Index * 10;


        }
        static public Bitmap ParseHTML_GetImage(this WS_JobInfo.Obj o, int Width, int Height)
        {

            Size s = new Size(Width - 140, Height);

            // Cast the sender object back to ListBox type.
            // Get the string contained in each item.
            /*
            string itemString = (string)theListBox.Items[e.Index];

            // Split the string at the " . "  character.
            string[] resultStrings = itemString.Split('.');
            */
            // If the string contains more than one period, increase the 
            // height by ten pixels; otherwise, increase the height by 
            // five pixels.

            String MsgText = "";
            bool b_smile = false;
            bool b_text = false;
            bool b_image = false;

            {
                b_text = o.isText();
                b_smile = o.isSmile();
                b_image = o.isImage();
                if (b_text) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                if (b_smile) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                if (b_image) MsgText = "Картинка";
            }
            WebBrowser wb = new WebBrowser();
            wb.Width = s.Width;
          
            wb.BackColor = Color.LightGray;
            wb.Height = s.Width;// wb.Height+200;
            wb.ScrollBarsEnabled = false;

            wb.MinimumSize = new Size(50, 50);
            //                MaximumSize = s;
            wb.DocumentCompleted += wb_DocumentCompleted;
            wb.DocumentText = "0";
            //wb.DocumentText =  
            //      wb.Document.OpenNew(true);
            //  //       
            string css = "img {max-width: 40px;height:auto;max-height:40px;}";
            if (true)
            {
            //           wb.Document.Write("<html><body><div style='background-color:lightblue;float:left;margin:10px;'>" + o.xml + "</div></body></html>");// o.xml;);
                
                //  https://codepen.io/josy-star/pen/xwdddP


                wb.Document.Write("<html><head>"+css+"</head><body><div style='float:left;margin:10px;'>" + o.xml + "</div></body></html>");// o.xml;);
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
            if (images1.Count>0)
            {
                //     HTMLElement img = images[0] HTMLImageElement;
                HtmlElement el = images1[0];
         //       el.Style = "max-width:40px;height:auto;max-height:40px;";
/* Закоментировал из за "D:\\Work\\icon\\pause.png               
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
                    el.SetAttribute("style",  "max-width:40px;height:auto;max-height:40px;");
                    el.Id = "TestXROGi";
                }
*/
            }
            wb.Refresh();
            var rrrrrrr = wb.Document.Body.GetElementsByTagName("div");
            Rectangle r1111 = rrrrrrr[0].OffsetRectangle;// wb.Document.Body.ScrollRectangle;

            //<img ver =\"1.0\" fileid=\"5b4842c4-a751-4c2a-a211-b0dc945068f1\" />

            //wb.DrawToBitmap(bitmap, new Rectangle(0, 0, r.Width, r.Height));
            //          r.Top = r1111.Top;

            //wb.Document.Body.ScrollRectangle.Width
            //            Rectangle r = new Rectangle(r1111.Left, r1111.Top, wb.Document.Body.ScrollRectangle.Width, wb.Document.Body.ScrollRectangle.Height);//  wb.Document.Body.ScrollRectangle;
            Rectangle r = new Rectangle(0, 0, r1111.Left + r1111.Width + 10, r1111.Top + r1111.Height + 10);//  wb.Document.Body.ScrollRectangle;
            Bitmap bitmap = new Bitmap(r1111.Left + r1111.Width + 1, r1111.Top + r1111.Height + 1);
            wb.DrawToBitmap(bitmap, r);

            Bitmap eeeee = bitmap.Clone(r1111, bitmap.PixelFormat); 

//            eeeee.Save("c:\\temp\\chat\\1.jpg", System.Drawing.Imaging.ImageFormat.Png);
            wb.Dispose();
            return eeeee;
        }

        private static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection elems = ((WebBrowser)sender)
        .Document.GetElementsByTagName("text");
            foreach (HtmlElement elem in elems)
            {
                // Do Some Stuff
            }
        }
        /*
<root><text ver="1.0">В состав группы добавлен(a):РоботАвтоИнформатор</text><user userid="14722">РоботАвтоИнформатор</user></root>
<text>мар 14 2019  2:04PM</text>
<text>фев 22 2019  9:02AM</text>
<root><img ver="1.0" fileid="3f23713f-39ab-46a6-8f97-420513808a69" /><text ver="1.0">Изображение</text></root>
<text>фев 22 2019  9:01AM</text>
<root><text ver="1.0">Новый чат [22.02.2019 9:01:27]</text></root>
<root><text ver="1.0">Создан чат</text></root>
<root><text ver="1.0">Тест1</text></root>
<root><text ver="1.0">тест2</text></root>         
             */

        public enum XTypeStringInfo { Text_String, Text_XML, Description_String, Description_XML }
        public static String GetString(this WS_JobInfo.Obj o,  XTypeStringInfo xt)
        {
            switch (xt)
            {
                case XTypeStringInfo.Text_String:
                    return o.GetText();
                case XTypeStringInfo.Text_XML:
                    return o.GetXML();
                case XTypeStringInfo.Description_String:
                    return o.GetText();
                case XTypeStringInfo.Description_XML:
                    return o.GetText();
            }
            return "Error WS_JobInfo.Obj.GetString";
        }
        public static String GetText(this WS_JobInfo.Obj o)
        {
            try
            {
                if (o==null)
                {
                    return "";
                }
                if ((o.xml==null))
                {
                    return "Нет названия для обьекта obkid="+o.ObjId.ToString();
                }

                if (!(o.xml.Trim().StartsWith("<")))
                {
                    return o.xml;
                }
                TextReader tr = new StringReader(o.xml);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;
         /*       string out_text = xDocument.ToString();
                if (!out_text.StartsWith("<root>"))
                {
                    out_text = "<root>" + out_text + "</root>";
            //        return out_text;
                }
                */
          //      return out_text;

                var varText = from x in xDocument.XPathSelectElements("//text") select x.Value;
                if (varText.Any())
                {

                    string txt = varText.First().ToString();
                    /*if (!txt.StartsWith("<root>"))
                    {
                        txt = "<root>" + txt + "</root>";
                    }*/
                    return txt;
                }
                else
                {
                    var varSmile = from x in xDocument.XPathSelectElements("//smile") select x.Value;
                    if (varSmile.Any())
                    {
                        return varSmile.First().ToString();
                    }

                }
            }
            catch (Exception err)
            {
                return "Ошибка представления текста[" + o.xml + "]";
            }
            return "Тип не определён[" + o.xml + "]";
        }
        public static String GetXML(this WS_JobInfo.Obj o)
        {
            try
            {
                if (o == null)
                {
                    throw new Exception("Значение данных объекта " + o.ObjId.ToString() + " не содержит XML данных77");
                }
                if ((o.xml == null))
                {
                    throw new Exception("Значение данных объекта " + o.ObjId.ToString() + " не содержит XML данных");
                }

                if (!(o.xml.Trim().StartsWith("<")))
                {
                    throw new Exception("Значение данных объекта " + o.ObjId.ToString() + " не содержит XML данных");
                    //return o.xml;
                }
                TextReader tr = new StringReader(o.xml);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;
                string out_text = xDocument.ToString();
                if (!out_text.StartsWith("<root>"))
                {
                    out_text = "<root>" + out_text + "</root>";
                    //        return out_text;
                }
                else
                {
                    return o.xml;
                }

                //      return out_text;

                var varText = from x in xDocument.XPathSelectElements("//text") select x.Value;
                if (varText.Any())
                {

                    string txt = varText.First().ToString();
                    if (!txt.StartsWith("<root>"))
                    {
                        txt = "<root>" + txt + "</root>";
                    }
                    return txt;
                }
                else
                {
                    var varSmile = from x in xDocument.XPathSelectElements("//smile") select x.Value;
                    if (varSmile.Any())
                    {
                        return varSmile.First().ToString();
                    }

                }
            }
            catch (Exception err)
            {
                return "Ошибка представления текста[" + o.xml + "]";
            }
            return "Тип не определён[" + o.xml + "]";
        }

        public static String [] GetFiles(this WS_JobInfo.Obj o)
        {
            try
            {
                
                if (!(o.xml.Trim().StartsWith("<")))
                {
          //          return o.xml;
                }
                TextReader tr = new StringReader(o.xml);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;

                var varImg = from x in xDocument.XPathSelectElements("//img") select x.Attribute("fileid").Value;
                string [] ret = varImg.ToArray();
                return ret;
            }
            catch (Exception err)
            {
                //        return "Ошибка представления текста[" + o.xml + "]";
                return null;
            }
            return null;
        }

        public static bool isText(this WS_JobInfo.Obj o)
        {
            try
            {

                TextReader tr = new StringReader(o.xml);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;

                var varText = from x in xDocument.XPathSelectElements("//text") select x.Value;
                if (varText.Any())
                {
                    return true;
                }
                else
                {
                    var varSmile = from x in xDocument.XPathSelectElements("//smile") select x.Value;
                    if (varSmile.Any())
                    {
                        return false;
                    }

                }
            }
            catch (Exception err)
            {
                return false;
            }
            return false;
        }
        public static bool isSmile(this WS_JobInfo.Obj o)
        {
            try
            {

                TextReader tr = new StringReader(o.xml);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;

                
                    var varSmile = from x in xDocument.XPathSelectElements("//smile") select x.Value;
                    if (varSmile.Any())
                    {
                        return true;
                    }

              
            }
            catch (Exception err)
            {
                return false;
            }
            return false;
        }
        public static bool isImage(this WS_JobInfo.Obj o)
        {
            try
            {
                if (o==null)
                {
                    return false;
                }
                TextReader tr = new StringReader(o.xml);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;


                var varSmile = from x in xDocument.XPathSelectElements("//img") select x.Value;
                if (varSmile.Any())
                {
                    return true;
                }


            }
            catch (Exception err)
            {
                return false;
            }
            return false;
        }

    }
    public static class PanelExtension
    {
        public static void ScrollDown(this Panel p, int pos)
        {
            //pos passed in should be positive
            using (Control c = new Control() { Parent = p, Height = 1, Top = p.ClientSize.Height + pos })
            {
                p.ScrollControlIntoView(c);
            }
        }
        public static void ScrollUp(this Panel p, int pos)
        {
            //pos passed in should be negative
            using (Control c = new Control() { Parent = p, Height = 1, Top = pos })
            {
                p.ScrollControlIntoView(c);
            }
        }
    }

    public static class GraphicsExtensions
    {
        static Font Font_MessageText = new Font("Arial", 10);
        static int OtstupMsg = 35;
        public static void DrawChat(this Graphics g, WS_JobInfo.Obj o,  Rectangle  Bounds , DrawItemState State
           //, Pen pen,float centerX, float centerY, float radius
            )
        {
           /* g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);

            
           */

            /*
1.          //   https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.textrenderer?redirectedfrom=MSDN&view=netframework-4.7.2
             Size len = TextRenderer.MeasureText(Control.Text, Control.Font);
             TextRenderer.DrawText(e.Graphics, "Regular Text", this.Font,  new Point(10, 10), SystemColors.ControlText);
             //Используй эту перегрузку DrawString. Там можно указать прямоугольник, в который вписывать текст
             public void DrawString (string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF layoutRectangle);
2.         //Твой рисунок
                Bitmap newImage = new Bitmap(...);
                //Твой шрифт
                Font f = new Font(...);
                //График для рисунка
                Graphics g= Graphics.FromImage(newImage);
                //Твой текст
                string s = "Hello";
                //Размер шрифта
                Size size = g.MeasureString(s,f);

             */

            // If the item is the selected item, then draw the rectangle
            // filled in blue. The item is selected when a bitwise And  
            // of the State property and the DrawItemState.Selected 
            // property is true.





            Rectangle rec = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            g.FillRectangle(Brushes.LightGray, Bounds);
            rec.Inflate(-3, -3);
            String MsgText = "";
            MsgText = o.GetText();// + "\r\nid=" + o.ObjId.ToString();


            
            Size size = TextRenderer.MeasureText(MsgText, Font_MessageText, rec.Size, TextFormatFlags.WordBreak);

            Brush back = Brushes.White;
        

            //else
            {
                back = Brushes.Beige;
                //e.Graphics.FillRectangle(back, e.Bounds);
                /*
                switch (o.type)
                {
                    case MsgType.chat:
                        back = Brushes.LightGreen;

                        break;
                        
                    case MsgType.msg:
                        {
                        */
                 
                            back = Brushes.White;
                        //    OnMessageShow(job, o);
/*
                            //                            rec = new Rectangle(rec.X + OtstupMsg, rec.Y, rec.Width - OtstupMsg, rec.Height);
                            //  rec = new Rectangle(rec.X + OtstupMsg, rec.Y, size.Width, size.Height);
                        }
                        break;
                }

                */
                // Otherwise, draw the rectangle filled in beige.



            }

            if ((State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //    size.Height = rec.Height;
                //    size.Width = rec.Width;
                //e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds);
                back = Brushes.LightBlue;
            }


            rec = new Rectangle(rec.X + OtstupMsg, rec.Y, size.Width, size.Height);
            g.FillRectangle(back, rec);
            // Draw a rectangle in blue around each item.


            //Rectangle fullR = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height - 2);

           g.DrawRectangle(Pens.Blue, rec);

            List<Point> Income = new List<Point>();
            //Income.Add(new Point(e.Bounds.X, e.Bounds.Y));
            int Ugol = 5;

            #region MyRegion
            /*

 //    Control control = panel1;
     int radius = 30;
     using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
     {

         path.AddLine(radius, 0, fullR.Width - radius, 0);
         path.AddArc(fullR.Width - radius, 0, radius, radius, 270, 90);
         path.AddLine(fullR.Width, radius, fullR.Width, fullR.Height - radius);
         path.AddArc(fullR.Width - radius, fullR.Height - radius, radius, radius, 0, 90);
         path.AddLine(fullR.Width - radius, fullR.Height, radius, fullR.Height);
         path.AddArc(0, fullR.Height - radius, radius, radius, 90, 90);
         path.AddLine(0, fullR.Height - radius, 0, radius);
         path.AddArc(0, 0, radius, radius, 180, 90);
         using (SolidBrush brush = new SolidBrush(Color.Purple))
         {
             e.Graphics.FillPath(brush, path);
         }
     }
     */
            /*      Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Y));
               //   Income.Add(new Point(e.Bounds.X+Ugol, e.Bounds.Y));
                  Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Height - 5));
                  //Income.Add(new Point(e.Bounds.Width - 2, e.Bounds.Width - 2));
                  //Income.Add(new Point(e.Bounds.Width - 2, e.Bounds.Y));
                  ////Income.Add(new Point(e.Bounds.X, e.Bounds.Y));


                  //Income.Add(new Point(e.Bounds.X , e.Bounds.Y+Ugol));
                  //Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Y));

                  e.Graphics.DrawPolygon(Pens.Red, Income.ToArray());
                  */
            // Draw the text in the item.
            /*e.Graphics.DrawString(listBox1.Items[e.Index].ToString(),
                this.Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
            */
            #endregion
            TextRenderer.DrawText(g, MsgText, Font_MessageText, rec, Color.Blue, TextFormatFlags.WordBreak);


            // Draw the focus rectangle around the selected item.
            //   e.DrawFocusRectangle();

            try
            {
                if ( o.links != null && o.links.Count() > 0)
                {
                    
                    int SizeCircle = 12;
                    g.FillCircle(Brushes.Red, rec.X - 15, rec.Y + SizeCircle, SizeCircle);
                    g.DrawCircle(Pens.White, rec.X - 15, rec.Y + SizeCircle, SizeCircle);
                    Font Font_MessageText = new Font("Arial", 10);
                    TextRenderer.DrawText(g, o.links.First().objid_to.ToString(), Font_MessageText, new Point(rec.X - SizeCircle - 15, rec.Y + 5), Color.Blue, TextFormatFlags.WordBreak);

                    //     e.Graphics.cir.DrawRectangle(Pens.Blue, rec);
                }
            }
            catch (Exception err)
            {

            }


            #region MyRegion

            /**
             
               var o = listBox1.Items[e.Index] as Obj;
            Color backColor = Color.Green;
           // if (o.id == "5")
            {
                backColor = Color.Gray;
                e.DrawBackground();

                // Draw the picture.
                //http://csharphelper.com/blog/2014/11/make-an-owner-drawn-listbox-with-pictures-in-c/
                /*      float scale = PictureHeight / planet.Picture.Height;
                      RectangleF source_rect = new RectangleF(
                          0, 0, planet.Picture.Width, planet.Picture.Height);
                      float picture_width = scale * planet.Picture.Width;
                      RectangleF dest_rect = new RectangleF(
                          e.Bounds.Left + ItemMargin, e.Bounds.Top + ItemMargin,
                          picture_width, PictureHeight);
                      e.Graphics.DrawImage(planet.Picture, dest_rect,
                          source_rect, GraphicsUnit.Pixel);

                      // See if the item is selected.
                      Brush br;
                      if ((e.State & DrawItemState.Selected) ==
                          DrawItemState.Selected)
                          br = SystemBrushes.HighlightText;
                      else
                          br = new SolidBrush(e.ForeColor);

                      // Find the area in which to put the text.
                
      */
            /*  // Draw the text.

              float x = e.Bounds.Left + 6 + 3 * 6;
              float y = e.Bounds.Top + 6;
              float width = e.Bounds.Right - 6 - x;
              float height = e.Bounds.Bottom - 6 - y;
              RectangleF layout_rect = new RectangleF(x, y, width, height);

              Brush br;
              if ((e.State & DrawItemState.Selected) ==
                  DrawItemState.Selected)
                  br = SystemBrushes.HighlightText;
              else
                  br = new SolidBrush(e.ForeColor);


              string txt = o.id + '\n' + o.type;
              e.Graphics.DrawString(txt, this.Font, br, layout_rect);


              // Draw the focus rectangle if appropriate.
              e.DrawFocusRectangle();
          }

               */
            #endregion

        }
        public static void DrawMessage(this Graphics g, WS_JobInfo.Obj o, Rectangle Bounds, DrawItemState State , ImageList il 
          //, Pen pen,float centerX, float centerY, float radius
          )
        {
            int OtstupMsg = 35;
            /* g.DrawEllipse(pen, centerX - radius, centerY - radius,
                           radius + radius, radius + radius);


            */

            /*
1.          //   https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.textrenderer?redirectedfrom=MSDN&view=netframework-4.7.2
             Size len = TextRenderer.MeasureText(Control.Text, Control.Font);
             TextRenderer.DrawText(e.Graphics, "Regular Text", this.Font,  new Point(10, 10), SystemColors.ControlText);
             //Используй эту перегрузку DrawString. Там можно указать прямоугольник, в который вписывать текст
             public void DrawString (string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF layoutRectangle);
2.         //Твой рисунок
                Bitmap newImage = new Bitmap(...);
                //Твой шрифт
                Font f = new Font(...);
                //График для рисунка
                Graphics g= Graphics.FromImage(newImage);
                //Твой текст
                string s = "Hello";
                //Размер шрифта
                Size size = g.MeasureString(s,f);

             */

            // If the item is the selected item, then draw the rectangle
            // filled in blue. The item is selected when a bitwise And  
            // of the State property and the DrawItemState.Selected 
            // property is true.



            Size size = o.MeasureDrawObj(Bounds.Width, Bounds.Height);
            Rectangle rec = new Rectangle(Bounds.X, Bounds.Y, size.Width, size.Height);
            Rectangle rec1 = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            g.FillRectangle(Brushes.AliceBlue, Bounds);
            rec.Inflate(-3, -3);
            Brush back = Brushes.White;
            String MsgText = "";
            MsgText = o.GetText();//+ "\r\nid=" + o.ObjId.ToString();
            bool b_text = o.isText();
            bool b_smile = o.isSmile();
            if (b_text)
            {
                if (MsgText.StartsWith("{\\rtf1\\"))
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.Rtf = MsgText;
                    rtb.Update(); // Ensure RTB fully painted
                    Bitmap bmp = new Bitmap(rtb.Width, rtb.Height);

                    g.DrawRtfText(MsgText, rec); ;
                    return;
                    /*
                    using (g = Graphics.FromImage(bmp))
                    {
                        g..CopyFromScreen(rtb.PointToScreen(Point.Empty), Point.Empty, rec.Size);
                        return;
                    }*/
                    //      return bmp;
                }
            }
     //       Size size = TextRenderer.MeasureText(MsgText, new Font("Arial", 10), rec.Size, TextFormatFlags.WordBreak);


            //else
            {
//                back = Brushes.Beige;
                //e.Graphics.FillRectangle(back, e.Bounds);
                /*
                switch (o.type)
                {
                    case MsgType.chat:
                        back = Brushes.LightGreen;

                        break;
                        
                    case MsgType.msg:
                        {
                        */

                back = Brushes.White;
                //    OnMessageShow(job, o);
                /*
                                            //                            rec = new Rectangle(rec.X + OtstupMsg, rec.Y, rec.Width - OtstupMsg, rec.Height);
                                            //  rec = new Rectangle(rec.X + OtstupMsg, rec.Y, size.Width, size.Height);
                                        }
                                        break;
                                }

                                */
                // Otherwise, draw the rectangle filled in beige.



            }

            //DrawItemState.Selected 
           /* if ((State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                //    size.Height = rec.Height;
                //    size.Width = rec.Width;
                //e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds);
                back = Brushes.LightBlue;
            }*/

            if ((State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //    size.Height = rec.Height;
                //    size.Width = rec.Width;
                //e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds);
                back = Brushes.LightBlue;
            }



            Rectangle rec_1 = new Rectangle(rec.X + OtstupMsg, rec.Y, size.Width, size.Height);
            //g.FillRectangle(back, rec);

           // if (!b_smile)
            {
                g.DrawRoundedRectangleFill(new Pen(back).Color, rec_1.X, rec_1.Y, rec_1.Height, rec_1.Width, 10);
                g.DrawRoundedRectangle(Color.LightSkyBlue, rec_1.X, rec_1.Y, rec_1.Height, rec_1.Width, 10);
            }
            //            return;
            List<Point> Income = new List<Point>();
            int Ugol = 5;

            #region MyRegion
            /*

 //    Control control = panel1;
     int radius = 30;
     using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
     {

         path.AddLine(radius, 0, fullR.Width - radius, 0);
         path.AddArc(fullR.Width - radius, 0, radius, radius, 270, 90);
         path.AddLine(fullR.Width, radius, fullR.Width, fullR.Height - radius);
         path.AddArc(fullR.Width - radius, fullR.Height - radius, radius, radius, 0, 90);
         path.AddLine(fullR.Width - radius, fullR.Height, radius, fullR.Height);
         path.AddArc(0, fullR.Height - radius, radius, radius, 90, 90);
         path.AddLine(0, fullR.Height - radius, 0, radius);
         path.AddArc(0, 0, radius, radius, 180, 90);
         using (SolidBrush brush = new SolidBrush(Color.Purple))
         {
             e.Graphics.FillPath(brush, path);
         }
     }
     */
            /*      Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Y));
               //   Income.Add(new Point(e.Bounds.X+Ugol, e.Bounds.Y));
                  Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Height - 5));
                  //Income.Add(new Point(e.Bounds.Width - 2, e.Bounds.Width - 2));
                  //Income.Add(new Point(e.Bounds.Width - 2, e.Bounds.Y));
                  ////Income.Add(new Point(e.Bounds.X, e.Bounds.Y));


                  //Income.Add(new Point(e.Bounds.X , e.Bounds.Y+Ugol));
                  //Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Y));

                  e.Graphics.DrawPolygon(Pens.Red, Income.ToArray());
                  */
            // Draw the text in the item.
            /*e.Graphics.DrawString(listBox1.Items[e.Index].ToString(),
                this.Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
            */
            #endregion


            if (b_text)
            {
                TextFormatFlags flags =
               TextFormatFlags.HorizontalCenter |
    TextFormatFlags.NoPadding | TextFormatFlags.WordBreak |
    TextFormatFlags.EndEllipsis;

                TextRenderer.DrawText(g, MsgText, Font_MessageText, rec_1, Color.Blue, flags);
            }
            if (b_smile)
            {
                int left = 18;
            //    g.DrawCircle(Pens.Green, rec_1.X+15 , rec_1.Y + left, left - 5);
                g.DrawImage(il.Images[MsgText], rec_1.X+9 , rec_1.Y+9 );
            }

            // Draw the focus rectangle around the selected item.
            //   e.DrawFocusRectangle();

            #region Нижняя строка: часы, автор

            if (o.period.dtc.HasValue)
            {
                int DownDelta = 2;
                Rectangle rec_id = new Rectangle(rec_1.X + 12, rec_1.Y + size.Height + DownDelta, 50, 50);

                TextRenderer.DrawText(g, o.ObjId.ToString(), new Font("Arial", 8), rec_id, Color.Gray, TextFormatFlags.WordBreak);


                Rectangle rec_fio = new Rectangle(rec_1.X + 55, rec_1.Y + size.Height + DownDelta, 150, 50);
                if (o.userCreater != null)
                    TextRenderer.DrawText(g, o.userCreater.ToString(), new Font("Arial", 8), rec_fio, Color.Gray, TextFormatFlags.WordBreak);
                else
                    TextRenderer.DrawText(g, "uid=" + o.userid.ToString(), new Font("Arial", 8), rec_fio, Color.Gray, TextFormatFlags.WordBreak);

                Rectangle rec_datetime = new Rectangle(0, rec_1.Y + size.Height + DownDelta, 50, 50);
                //Rectangle rec_datetime = new Rectangle(rec_1.X + size.Width - 35, rec_1.Y + size.Height + DownDelta, 50, 50);
                TextRenderer.DrawText(g, o.period.dtc.Value.ToString("HH:mm"), new Font("Arial", 8), rec_datetime, Color.Gray, TextFormatFlags.WordBreak);

                Rectangle rec_size = new Rectangle(300 , rec_1.Y + size.Height + DownDelta, 150, 50);
                //Rectangle rec_datetime = new Rectangle(rec_1.X + size.Width - 35, rec_1.Y + size.Height + DownDelta, 50, 50);
                string ss = rec.X.ToString() + "," + rec.Y.ToString() + "," + rec.Width.ToString() + "," + rec.Height.ToString();

                TextRenderer.DrawText(g, ss , new Font("Arial", 8), rec_size, Color.Red, TextFormatFlags.WordBreak);




            }

            #endregion
            try
            {
                if (o.links != null && o.links.Count() > 0)
                {

                    int SizeCircle = 12;
                    g.FillCircle(Brushes.LightPink, rec_1.X - 15, rec_1.Y + SizeCircle, SizeCircle);
                    g.DrawCircle(Pens.White, rec_1.X - 15, rec_1.Y + SizeCircle, SizeCircle);
                    TextRenderer.DrawText(g, o.links.First().objid_to.ToString(), new Font("Arial", 8), new Point(rec_1.X - SizeCircle - 12, rec_1.Y + 7), Color.Blue, TextFormatFlags.WordBreak);

                    //     e.Graphics.cir.DrawRectangle(Pens.Blue, rec);
                }
            }
            catch (Exception err)
            {

            }


            #region MyRegion

            /**
             
               var o = listBox1.Items[e.Index] as Obj;
            Color backColor = Color.Green;
           // if (o.id == "5")
            {
                backColor = Color.Gray;
                e.DrawBackground();

                // Draw the picture.
                //http://csharphelper.com/blog/2014/11/make-an-owner-drawn-listbox-with-pictures-in-c/
                /*      float scale = PictureHeight / planet.Picture.Height;
                      RectangleF source_rect = new RectangleF(
                          0, 0, planet.Picture.Width, planet.Picture.Height);
                      float picture_width = scale * planet.Picture.Width;
                      RectangleF dest_rect = new RectangleF(
                          e.Bounds.Left + ItemMargin, e.Bounds.Top + ItemMargin,
                          picture_width, PictureHeight);
                      e.Graphics.DrawImage(planet.Picture, dest_rect,
                          source_rect, GraphicsUnit.Pixel);

                      // See if the item is selected.
                      Brush br;
                      if ((e.State & DrawItemState.Selected) ==
                          DrawItemState.Selected)
                          br = SystemBrushes.HighlightText;
                      else
                          br = new SolidBrush(e.ForeColor);

                      // Find the area in which to put the text.
                
      */
            /*  // Draw the text.

              float x = e.Bounds.Left + 6 + 3 * 6;
              float y = e.Bounds.Top + 6;
              float width = e.Bounds.Right - 6 - x;
              float height = e.Bounds.Bottom - 6 - y;
              RectangleF layout_rect = new RectangleF(x, y, width, height);

              Brush br;
              if ((e.State & DrawItemState.Selected) ==
                  DrawItemState.Selected)
                  br = SystemBrushes.HighlightText;
              else
                  br = new SolidBrush(e.ForeColor);


              string txt = o.id + '\n' + o.type;
              e.Graphics.DrawString(txt, this.Font, br, layout_rect);


              // Draw the focus rectangle if appropriate.
              e.DrawFocusRectangle();
          }

               */
            #endregion

        }
        public static void DrawMessage_SourceChat(this Graphics g, WS_JobInfo.Obj o, Rectangle Bounds, DrawItemState State
            //, Pen pen,float centerX, float centerY, float radius
            )
        {
            /* g.DrawEllipse(pen, centerX - radius, centerY - radius,
                           radius + radius, radius + radius);


            */

            /*
1.          //   https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.textrenderer?redirectedfrom=MSDN&view=netframework-4.7.2
             Size len = TextRenderer.MeasureText(Control.Text, Control.Font);
             TextRenderer.DrawText(e.Graphics, "Regular Text", this.Font,  new Point(10, 10), SystemColors.ControlText);
             //Используй эту перегрузку DrawString. Там можно указать прямоугольник, в который вписывать текст
             public void DrawString (string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF layoutRectangle);
2.         //Твой рисунок
                Bitmap newImage = new Bitmap(...);
                //Твой шрифт
                Font f = new Font(...);
                //График для рисунка
                Graphics g= Graphics.FromImage(newImage);
                //Твой текст
                string s = "Hello";
                //Размер шрифта
                Size size = g.MeasureString(s,f);

             */

            // If the item is the selected item, then draw the rectangle
            // filled in blue. The item is selected when a bitwise And  
            // of the State property and the DrawItemState.Selected 
            // property is true.





            Rectangle rec = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            g.FillRectangle(Brushes.AliceBlue, Bounds);
            rec.Inflate(-3, -3);
            String MsgText = "";
            MsgText = o.xml;// + "\r\nid=" + o.ObjId.ToString();

            //o.period.dtc;

            Size size = TextRenderer.MeasureText(MsgText, Font_MessageText , rec.Size, TextFormatFlags.WordBreak);

            Brush back = Brushes.White;


            //else
            {
               
                //e.Graphics.FillRectangle(back, e.Bounds);
                /*
                switch (o.type)
                {
                    case MsgType.chat:
                        back = Brushes.LightGreen;

                        break;
                        
                    case MsgType.msg:
                        {
                        */

                back = Brushes.Thistle;
                //    OnMessageShow(job, o);
                /*
                                            //                            rec = new Rectangle(rec.X + OtstupMsg, rec.Y, rec.Width - OtstupMsg, rec.Height);
                                            //  rec = new Rectangle(rec.X + OtstupMsg, rec.Y, size.Width, size.Height);
                                        }
                                        break;
                                }

                                */
                // Otherwise, draw the rectangle filled in beige.



            }

            if ((State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //    size.Height = rec.Height;
                //    size.Width = rec.Width;
                //e.Graphics.FillRectangle(Brushes.CornflowerBlue, e.Bounds);
                back = Brushes.Thistle ;
            }


            rec = new Rectangle(rec.X + OtstupMsg, rec.Y, size.Width, size.Height);
            g.FillRectangle(back, rec);
            // Draw a rectangle in blue around each item.


            //Rectangle fullR = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height - 2);

            g.DrawRectangle(Pens.LightSkyBlue, rec);

            List<Point> Income = new List<Point>();
            //Income.Add(new Point(e.Bounds.X, e.Bounds.Y));
            int Ugol = 5;

            #region MyRegion
            /*

 //    Control control = panel1;
     int radius = 30;
     using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
     {

         path.AddLine(radius, 0, fullR.Width - radius, 0);
         path.AddArc(fullR.Width - radius, 0, radius, radius, 270, 90);
         path.AddLine(fullR.Width, radius, fullR.Width, fullR.Height - radius);
         path.AddArc(fullR.Width - radius, fullR.Height - radius, radius, radius, 0, 90);
         path.AddLine(fullR.Width - radius, fullR.Height, radius, fullR.Height);
         path.AddArc(0, fullR.Height - radius, radius, radius, 90, 90);
         path.AddLine(0, fullR.Height - radius, 0, radius);
         path.AddArc(0, 0, radius, radius, 180, 90);
         using (SolidBrush brush = new SolidBrush(Color.Purple))
         {
             e.Graphics.FillPath(brush, path);
         }
     }
     */
            /*      Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Y));
               //   Income.Add(new Point(e.Bounds.X+Ugol, e.Bounds.Y));
                  Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Height - 5));
                  //Income.Add(new Point(e.Bounds.Width - 2, e.Bounds.Width - 2));
                  //Income.Add(new Point(e.Bounds.Width - 2, e.Bounds.Y));
                  ////Income.Add(new Point(e.Bounds.X, e.Bounds.Y));


                  //Income.Add(new Point(e.Bounds.X , e.Bounds.Y+Ugol));
                  //Income.Add(new Point(e.Bounds.X + Ugol, e.Bounds.Y));

                  e.Graphics.DrawPolygon(Pens.Red, Income.ToArray());
                  */
            // Draw the text in the item.
            /*e.Graphics.DrawString(listBox1.Items[e.Index].ToString(),
                this.Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
            */
            #endregion
            TextRenderer.DrawText(g, MsgText, new Font("Arial", 10), rec, Color.Blue, TextFormatFlags.WordBreak);


            // Draw the focus rectangle around the selected item.
            //   e.DrawFocusRectangle();

            if (o.period.dtc.HasValue)
            {
                Rectangle rec_datetime = new Rectangle(rec.X + size.Width - 30, rec.Y + size.Height - 12, 50, 50);
                TextRenderer.DrawText(g, o.period.dtc.Value.ToString("HH:MM"), new Font("Arial", 8), rec_datetime, Color.Gray, TextFormatFlags.WordBreak);
            }

            try
            {
                if (o.links != null && o.links.Count() > 0)
                {

                    int SizeCircle = 12;
                    g.FillCircle(Brushes.Red, rec.X - 15, rec.Y + SizeCircle, SizeCircle);
                    g.DrawCircle(Pens.White, rec.X - 15, rec.Y + SizeCircle, SizeCircle);
                    TextRenderer.DrawText(g, o.links.First().objid_to.ToString(), new Font("Arial", 8), new Point(rec.X - SizeCircle - 12, rec.Y + 7), Color.Blue, TextFormatFlags.WordBreak);

                    //     e.Graphics.cir.DrawRectangle(Pens.Blue, rec);
                }
            }
            catch (Exception err)
            {

            }


            #region MyRegion

            /**
             
               var o = listBox1.Items[e.Index] as Obj;
            Color backColor = Color.Green;
           // if (o.id == "5")
            {
                backColor = Color.Gray;
                e.DrawBackground();

                // Draw the picture.
                //http://csharphelper.com/blog/2014/11/make-an-owner-drawn-listbox-with-pictures-in-c/
                /*      float scale = PictureHeight / planet.Picture.Height;
                      RectangleF source_rect = new RectangleF(
                          0, 0, planet.Picture.Width, planet.Picture.Height);
                      float picture_width = scale * planet.Picture.Width;
                      RectangleF dest_rect = new RectangleF(
                          e.Bounds.Left + ItemMargin, e.Bounds.Top + ItemMargin,
                          picture_width, PictureHeight);
                      e.Graphics.DrawImage(planet.Picture, dest_rect,
                          source_rect, GraphicsUnit.Pixel);

                      // See if the item is selected.
                      Brush br;
                      if ((e.State & DrawItemState.Selected) ==
                          DrawItemState.Selected)
                          br = SystemBrushes.HighlightText;
                      else
                          br = new SolidBrush(e.ForeColor);

                      // Find the area in which to put the text.
                
      */
            /*  // Draw the text.

              float x = e.Bounds.Left + 6 + 3 * 6;
              float y = e.Bounds.Top + 6;
              float width = e.Bounds.Right - 6 - x;
              float height = e.Bounds.Bottom - 6 - y;
              RectangleF layout_rect = new RectangleF(x, y, width, height);

              Brush br;
              if ((e.State & DrawItemState.Selected) ==
                  DrawItemState.Selected)
                  br = SystemBrushes.HighlightText;
              else
                  br = new SolidBrush(e.ForeColor);


              string txt = o.id + '\n' + o.type;
              e.Graphics.DrawString(txt, this.Font, br, layout_rect);


              // Draw the focus rectangle if appropriate.
              e.DrawFocusRectangle();
          }

               */
            #endregion

        }
        public static void DrawCircle(this Graphics g, Pen pen,
                                      float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        //https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp
        public static void DrawRoundedRectangle(this Graphics g, Color BoxColor, Rectangle r, int CornerRadius)
        {
            DrawRoundedRectangle(g, BoxColor, r.X, r.Y, r.Height, r.Width, CornerRadius);
        }
        public static void DrawRoundedRectangle(this Graphics g, Color BoxColor, int XPosition, int YPosition, int Height, int Width, int CornerRadius)
        {
            // Bitmap NewBitmap = new Bitmap(Image, Image.Width, Image.Height);
            //            using (Graphics NewGraphics = g)
            {
                using (Pen BoxPen = new Pen(BoxColor))
                {
                    using (GraphicsPath Path = new GraphicsPath())
                    {
                        Path.AddLine(XPosition + CornerRadius, YPosition, XPosition + Width - (CornerRadius * 2), YPosition);
                        Path.AddArc(XPosition + Width - (CornerRadius * 2), YPosition, CornerRadius * 2, CornerRadius * 2, 270, 90);
                        Path.AddLine(XPosition + Width, YPosition + CornerRadius, XPosition + Width, YPosition + Height - (CornerRadius * 2));
                        Path.AddArc(XPosition + Width - (CornerRadius * 2), YPosition + Height - (CornerRadius * 2), CornerRadius * 2, CornerRadius * 2, 0, 90);
                        Path.AddLine(XPosition + Width - (CornerRadius * 2), YPosition + Height, XPosition + CornerRadius, YPosition + Height);
                        Path.AddArc(XPosition, YPosition + Height - (CornerRadius * 2), CornerRadius * 2, CornerRadius * 2, 90, 90);
                        Path.AddLine(XPosition, YPosition + Height - (CornerRadius * 2), XPosition, YPosition + CornerRadius);
                        Path.AddArc(XPosition, YPosition, CornerRadius * 2, CornerRadius * 2, 180, 90);
                        Path.CloseFigure();
                        g.DrawPath(BoxPen, Path);
                    }
                }
            }
           // return NewBitmap;
        }
        public static void DrawRoundedRectangleFill(this Graphics g, Color BoxColor,Rectangle r, int CornerRadius)
        {
            DrawRoundedRectangleFill(g, BoxColor, r.X, r.Y, r.Height, r.Width, CornerRadius);
        }


        public static Image FixedSize(this Graphics g, Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static GraphicsPath PathRoundedRectangleFill(this Graphics g,  int XPosition, int YPosition, int Height, int Width, int CornerRadius)
        {
            
            {

                // using (
                GraphicsPath Path = new GraphicsPath();//)
                    {
                        Path.AddLine(XPosition + CornerRadius, YPosition, XPosition + Width - (CornerRadius * 2), YPosition);
                        Path.AddArc(XPosition + Width - (CornerRadius * 2), YPosition, CornerRadius * 2, CornerRadius * 2, 270, 90);
                        Path.AddLine(XPosition + Width, YPosition + CornerRadius, XPosition + Width, YPosition + Height - (CornerRadius * 2));
                        Path.AddArc(XPosition + Width - (CornerRadius * 2), YPosition + Height - (CornerRadius * 2), CornerRadius * 2, CornerRadius * 2, 0, 90);
                        Path.AddLine(XPosition + Width - (CornerRadius * 2), YPosition + Height, XPosition + CornerRadius, YPosition + Height);
                        Path.AddArc(XPosition, YPosition + Height - (CornerRadius * 2), CornerRadius * 2, CornerRadius * 2, 90, 90);
                        Path.AddLine(XPosition, YPosition + Height - (CornerRadius * 2), XPosition, YPosition + CornerRadius);
                        Path.AddArc(XPosition, YPosition, CornerRadius * 2, CornerRadius * 2, 180, 90);
                        Path.CloseFigure();
                    return Path;
                    }
               
            }
            // return NewBitmap;
        }


        public static void DrawRoundedRectangleFill(this Graphics g, Color BoxColor, int XPosition, int YPosition, int Height, int Width, int CornerRadius)
        {
            // Bitmap NewBitmap = new Bitmap(Image, Image.Width, Image.Height);
            //            using (Graphics NewGraphics = g)
            {
                using (SolidBrush BoxPen =   new SolidBrush(BoxColor))
                {
                    using (GraphicsPath Path = new GraphicsPath())
                    {
                        Path.AddLine(XPosition + CornerRadius, YPosition, XPosition + Width - (CornerRadius * 2), YPosition);
                        Path.AddArc(XPosition + Width - (CornerRadius * 2), YPosition, CornerRadius * 2, CornerRadius * 2, 270, 90);
                        Path.AddLine(XPosition + Width, YPosition + CornerRadius, XPosition + Width, YPosition + Height - (CornerRadius * 2));
                        Path.AddArc(XPosition + Width - (CornerRadius * 2), YPosition + Height - (CornerRadius * 2), CornerRadius * 2, CornerRadius * 2, 0, 90);
                        Path.AddLine(XPosition + Width - (CornerRadius * 2), YPosition + Height, XPosition + CornerRadius, YPosition + Height);
                        Path.AddArc(XPosition, YPosition + Height - (CornerRadius * 2), CornerRadius * 2, CornerRadius * 2, 90, 90);
                        Path.AddLine(XPosition, YPosition + Height - (CornerRadius * 2), XPosition, YPosition + CornerRadius);
                        Path.AddArc(XPosition, YPosition, CornerRadius * 2, CornerRadius * 2, 180, 90);
                        Path.CloseFigure();
                        g.FillPath(BoxPen, Path);
                    }
                }
            }
            // return NewBitmap;
        }
    }
    public static class ResourceExtensions
    {
        public static MemoryStream GetMemoryStream(this ResourceManager resourceManager, String name)
        {
            object resource = resourceManager.GetObject(name);

            if (resource is byte[])
            {
                return new MemoryStream((byte[])resource);
            }
            else
            {
                throw new System.InvalidCastException("The specified resource is not a binary resource.");
            }
        }
    }
}
