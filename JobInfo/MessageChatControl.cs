using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Class;
using JobInfo.XROGi_Extensions;

namespace JobInfo
{
    public enum MessageRegion { xrNone, xrFoto, xrFIO, xrTime, xrMessage, xrImage, xrInfo, xrFreePoints , xrLike, xrDisLike, xrWorkBtn};
    public enum MessagePosition { xrOther, xrCener, xrMyMessage};
    public class XMessageCtrl : Panel
    {
        const int MinHeighText = 50;

        public MessagePosition Position = MessagePosition.xrOther;
        public ImageList ImageListMsg { get; set; }
        public bool b_ShowUserImage = true;
        public bool b_ImageMsg = false;
        public string Text;
        public string HTML;

        public WS_JobInfo.User user;
        public int Full_Width = 0;
        private int Data_Width_Source;
        public int Data_Width;
        public int Data_Height;
        public int FotoBox = 50;
        int LeftStart = 0;
        int Position_LeftStart;
        WS_JobInfo.Obj o;
        internal Bitmap _this_bitmap;
        internal Bitmap _this_bitmap_html;

        internal WebBrowser wb;

        public delegate void OnMessageNeedReDrawelegate(XMessageCtrl sender);
        public event OnMessageNeedReDrawelegate OnNeedRedraw = delegate { };


    //    XMessageCtrl Replay = null;
        List<object> Data = new List<object>();
        public int chatId;

        public WS_JobInfo.Obj MessageObj
        {
            get { return o; }
            set { o = value; _objid = (int)o?.ObjId; }
        }
        int _objid = 0;
        public  int Id
        {
            get { return _objid; }
            set { }
        }
        /*
                public string Name
                {
                    get { return "msg_"+_objid.ToString(); }
                    set { }
                }
                */
        public List<Tuple<Image, Image, string>> files = new List<Tuple<Image, Image, string>>();


        int Msg_Height = 70;
        int Info_Height = 25;

        List<MessageElementDraw> elements = new List<MessageElementDraw>();

        Rectangle r;
      //  Rectangle r_msg;
   //     Rectangle r_msginfo;
  //      Rectangle r_msgtime;
//        Rectangle r_FIO;
   //     Rectangle r_image;
        
        
   //     Rectangle r_like;
   //     Rectangle r_dislike;
      //  Rectangle r_work;

        public delegate void OnMessageShownEventHandler(object sender);
        public delegate void OnClickMessageEventHandler(object sender);
        public delegate void OnClickMessageRegionEventHandler(XMessageCtrl sender, MouseEventArgs e, MessageRegion region);

        [Category("Action")]
        [Description("Fires when the value is changed")]
        public event OnMessageShownEventHandler OnMessageShownEvent;

        [Category("Action")]
        [Description("Возникает при нажатии на сообщение")]
        public event OnClickMessageEventHandler OnClickMessageEvent;

        [Category("Action")]
        [Description("Возникает при нажатии на сообщение")]
        public event OnClickMessageRegionEventHandler OnClickMessageRegionEvent;

        public XMessageCtrl ()
        {
            Visible = true;
    //        Replay = null;
            chatId = 0;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
          
            base.OnMouseDoubleClick(e);

            MessageRegion mr = GetRegion(e.X, e.Y);
            OnClickMessageRegionEvent?.Invoke(this, e, mr);
            /*if (r_image.Contains(e.X, e.Y))
            {
                OnClickMessageRegionEvent?.Invoke(this, e, MessageRegion.xrFoto);
            }
            if (r_msg.Contains(e.X, e.Y))
            {
                if (o.isImage())
                {
                    b_ImageMsg = true;
                    OnClickMessageRegionEvent?.Invoke(this, e, MessageRegion.xrImage);
                }
                else
                {
                    OnClickMessageRegionEvent?.Invoke(this, e, MessageRegion.xrMessage);
                }
            }
            if (r_msgtime.Contains(e.X, e.Y))
            {
                OnClickMessageRegionEvent?.Invoke(this, e, MessageRegion.xrTime);
            }
            if (r_FIO.Contains(e.X, e.Y))
            {
                OnClickMessageRegionEvent?.Invoke(this, e, MessageRegion.xrFIO);
            }
            if (r_msginfo.Contains(e.X, e.Y))
            {
                OnClickMessageRegionEvent?.Invoke(this, e, MessageRegion.xrInfo);
            }
            OnClickMessageRegionEvent?.Invoke(this,e, MessageRegion.xrFreePoints);*/
        }

        public MessageRegion GetRegion(int x, int y)
        {
          /*  if (r_image.Contains(x, y))
            {
                return MessageRegion.xrFoto;
            }*/
        /*    if (r_msg.Contains(x, y))
            {
                if (o.isImage())
                {
                  
                    return  MessageRegion.xrImage;
                }
                else
                {
                    return  MessageRegion.xrMessage;
                }
            }*/
            //if (r_msgtime.Contains(x, y))
            //{
            //    return MessageRegion.xrTime;
            //}
           /* if (r_FIO.Contains(x, y))
            {
                return MessageRegion.xrFIO;
            }*/
            //if (r_msginfo.Contains(x, y))
            //{
            //    return MessageRegion.xrInfo;
            //}
            
            foreach (var v in elements)
            {
                if (v.AreaPosition.Contains(x, y))
                {
                    return v.RegionType;
                }
            }

            
            return MessageRegion.xrFreePoints;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

         public Bitmap ParseHTML_GetImage(WS_JobInfo.Obj o, int Width, int Height)
        {
            if (o.ObjId == 168032)
            {

            }

//            Size s = new Size(Width - 140, Height);
            Size s = new Size(300, Height);
            if (Text == string.Empty)
                Text = o.GetString(GraphicsChatExtensions.XTypeStringInfo.Text_XML);

            bool b_smile = false;
            bool b_text = false;
            bool b_image = false;

            {
                b_text = o.isText();
                b_smile = o.isSmile();
                b_image = o.isImage();
                b_ImageMsg = b_image;
                //if (b_text) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                //if (b_smile) MsgText = o.GetText(); // ws_o.xml;// + "\r\nid=" + ws_o.ObjId.ToString();
                
            }

            
            if (wb==null)
                wb = new WebBrowser();
            wb.Width = s.Width;

            wb.BackColor = Color.LightGray;
            wb.Height = s.Width;// wb.Height+200;
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
                if (o.ObjId==64614)
                {

                }
               HTML = XParse.ParseMessageToHtml(o.xml);
               wb.Document.Write(HTML);// o.xml;);


                if ( (Environment.UserName == "iu.smirnov" ))//&& Control.ModifierKeys == Keys.Shift)
                {
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
                    r1111.Height = MinHeighText;

                //<img ver =\"1.0\" fileid=\"5b4842c4-a751-4c2a-a211-b0dc945068f1\" />

                //wb.DrawToBitmap(bitmap, new Rectangle(0, 0, r.Width, r.Height));
                //          r.Top = r1111.Top;

                //wb.Document.Body.ScrollRectangle.Width
                //            Rectangle r = new Rectangle(r1111.Left, r1111.Top, wb.Document.Body.ScrollRectangle.Width, wb.Document.Body.ScrollRectangle.Height);//  wb.Document.Body.ScrollRectangle;
                Rectangle r = new Rectangle(0, 0, r1111.Left + r1111.Width + 10, r1111.Top + r1111.Height + 10);//  wb.Document.Body.ScrollRectangle;
                Rectangle r_Wb = wb.Document.Body.ScrollRectangle;
                Bitmap bitmap = new Bitmap(r1111.Left + r1111.Width + 1, r1111.Top + r1111.Height + 1);
                wb.DrawToBitmap(bitmap, r);

                try
                {
                    Bitmap eeeee = bitmap.Clone(r1111, bitmap.PixelFormat);
                
                
      //          eeeee.Save("c:\\temp\\chat\\1.jpg", System.Drawing.Imaging.ImageFormat.Png);
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




        public void Calculate_Size()
        {
            
            if (o.ObjId == 32976 || o.ObjId == 27693)
            {

            }
            //      if (Data_Width_Source == Width) return;
            
            Data_Width = 0;
            Data_Height = 0;
        

            _this_bitmap_html = ParseHTML_GetImage(o,Full_Width - FotoBox, 10000);
            if (_this_bitmap_html==null)
            {

            }
            if (_this_bitmap_html != null)
            {
                Size s = _this_bitmap_html.Size;// o.MeasureDrawObj(Width - FotoBox, 10000);
                Data_Height = s.Height + 4;
                Data_Width = s.Width + 4;

                if (Data_Width <= 80)
                    Data_Width = 180;
                else
                    Data_Width = s.Width;

                Data_Width = Data_Width + 25 + 5;

                if (Data_Height < 75)
                {
                    Data_Height = 75;
                }

                Data_Height = 25 + Data_Height;//20190315MinHeighText

                Data_Width_Source = Full_Width;
                Height = Data_Height;
            }
        }

        internal void SetWidth(int width)
        {
            if (Full_Width > width)
                _this_bitmap = null;

            Full_Width = width;
            Width = width - 30;//; c.Data_Width;
            
        }
        //internal void SetObj(Job job, WS_JobInfo.Obj o, Size s)
        internal void SetObj(Job job, WS_JobInfo.Obj o )
        {
            Tag = o;
            Text = "";
            //Text = o.GetText();
            if (Environment.UserName == "iu.smirnov" && (Control.ModifierKeys == Keys.Shift ))
                Text += " id=" + o.ObjId.ToString();
            MessageObj = o;
            this.o = o;
            LoadContainFiles(job,1); // 1- icon
            Calculate_Size();

            //9            c.ContextMenuStrip = context_msg;
            //       c.Top = maxH;
            //c.Top;
            Height = Data_Height;
            //           maxH += c.Height + 25 + 5;
//9            c.ContextMenuStrip = context_msg;
      //20190417?????      OnMessageShownEvent += OnMessageShownEvent;
            //20170521 ???? OnClickMessageRegionEvent += OnClickMessageRegionEvent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                PrepareRegions();
                PrepareRegionsVers_2Image();
                PaintMessageV1(g,false);
            }catch (Exception err)
            {

            }
        }

        private void PaintMessageV1(Graphics g_screen, bool b_RegenImage)
        {
            //using (
            if (r.Height == 0)
            {
                return;
            }
                    Graphics g=null;
           if (_this_bitmap == null || b_RegenImage==true)
            {
                //if (r.Height == 0)
                {
                    _this_bitmap = new Bitmap(r.Width, r.Height);
                    g = Graphics.FromImage(_this_bitmap);
                }
            }
            else
            {
                ///              _this_bitmap.Save("c:\\temp\\chat\\image" + this.MessageObj.ObjId.ToString() + ".jpg");

                if (g_screen != null)
                    g_screen.DrawImage(_this_bitmap, r.X+ Position_LeftStart, r.Y);
                return;
            }
          /*   */

        //    _this_bitmap = new Bitmap(r.Width, r.Height);
       //     g = Graphics.FromImage(_this_bitmap);
            //            g = Graphics.FromImage(_this_bitmap);

            //      g.DrawImage(_this_bitmap, r.X, r.Y);
            //                 return;

            if (this.Region == null)
            {

                // Bitmap _this_bitmap = new Bitmap(r.Width, r.Height);
                //g = Graphics.FromImage(_this_bitmap);

                //g = e.Graphics;

                //)
                {
                    this.Region = new System.Drawing.Region(g.PathRoundedRectangleFill(r.X + Position_LeftStart, r.Y, r.Height, r.Width, 10));
                    //   g.Dispose();
                }
            }





            o = Tag as WS_JobInfo.Obj;
            //this.Parent.Bounds;
     //20190305       if (this.Top <= this.Parent.Bounds.Height)
            {
                //    Calculate_Size()

                //20190227        base.OnPaint(e);

                //8 r = new Rectangle(0, 0, Data_Width + FotoBox, Height);
                //e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, Width, Height);
                g.DrawRoundedRectangleFill(Color.White, r.X, r.Y, r.Height, r.Width, 10);

                /*     g.FillRectangle(Brushes.Red, 0, 0, _this_bitmap.Width, _this_bitmap .Height);
                     g.FillRectangle(Brushes.White, 0, 10, 200, 10);
                     */
                //         e.Graphics.DrawImage(_this_bitmap, r.X, r.Y);
                //    _this_bitmap.Save("c:\\temp\\chat\\image.jpg");

                // int Msg_X_Start = FotoBox;
                //8    r_msg = new Rectangle(Msg_X_Start, 0, r.Width - FotoBox, Msg_Height);

                if (Position != MessagePosition.xrMyMessage)
                {
                    if (b_ShowUserImage && user != null && user.foto != null)
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(user.foto))
                            {
                                Image f = ScaleImage(Image.FromStream(ms), 40, 40);
                                g.DrawImage(f, GetPoint("r_image"));
                            }
                        }
                        catch (Exception err)
                        {
                            g.DrawRoundedRectangle(Color.Red, GetRect  ("r_image"), 10);
                        }
                    }
                    else
                    {
                        //нет фото
                        g.DrawRoundedRectangle(Color.LightGray, GetRect("r_image"), 10);
                    }
                }
                else
                {
                    /*
                    {
                        g.DrawRoundedRectangle(Color.LightGray, r_image, 10);
                    }*/
                    ;
                 //   ImageListMsg.Draw(g, r_like.Location, ImageListMsg.Images.IndexOfKey("like"));
                 //   ImageListMsg.Draw(g, r_dislike.Location, ImageListMsg.Images.IndexOfKey("dislike"));
                    string[] drarbtn = new string[] { "r_like", "r_dislike", "r_work" };
                    foreach (string ss in drarbtn)
                    {
                        var elem = elements.Where(s => s.Name == ss).First();
                        ImageListMsg.Draw(g, elem.AreaPosition.Location, ImageListMsg.Images.IndexOfKey(elem.ImageKey));
                    }
                }



                //         e.Graphics.DrawRectangle(Pens.Gray, r_msg);
                Rectangle r_msg8 = GetRect("r_msg");

                g.DrawImage( _this_bitmap_html, r_msg8.Location.X + 1, r_msg8.Location.Y + 1);
                //_this_bitmap_html.Save("c:\\temp\\eeeeee.bmp");
                //g.DrawString(Text, new Font("Arial", 10), new SolidBrush(Color.Black), r_msg);
                g.DrawRoundedRectangle(Color.Gray, r_msg8.X, r_msg8.Y, r_msg8.Height, r_msg8.Width, 6);
                //g.DrawString("--------------------------------", new Font("Arial", 8), new SolidBrush(Color.Gray), GetPoint("r_msg"));

                if (o.isImage())
                {
                    //   e.Graphics.DrawRoundedRectangleFill(Color.Yellow, r_msg, 5);
                    foreach (Tuple<Image, Image, string> f in files)
                    {
                        Image image = f.Item1 as Image;
                        Image imageicon = f.Item2 as Image;
                        string imageguid = f.Item3 as string;
                        if (imageicon != null)
                        {
                            Image im2 = g.FixedSize(imageicon, r_msg8.Width, r_msg8.Height);
                            g.DrawImage(im2, r_msg8);
                        }
                        else
                        if (image != null)
                        {
                            Image im2 = g.FixedSize(image, r_msg8.Width, r_msg8.Height);
                            g.DrawImage(im2, r_msg8);
                        }
                        /*
                        if (imageicon != null)
                            g.DrawImage(imageicon, r_msg8);
                        else
                       if (image != null)
                            g.DrawImage(image, r_msg8);*/
                        
                    }
                }
         //       g.DrawRoundedRectangleFill(Color.LightYellow, GetRect("r_msginfo"), 5);
                g.DrawRoundedRectangle(Color.LightGray, GetRect("r_msginfo"), 5);
                //          e.Graphics.DrawRectangle(Pens.Gray, r_msgtime);

                if (o != null && o.period != null && o.period.dtc.HasValue)
                {
           //         if (o.period.dtc.Value.Date==DateTime.Now.Date)
                        g.DrawString(o.period.dtc.Value.ToString("HH:mm"), new Font("Arial", 8), new SolidBrush(Color.Gray), GetPoint("r_msgtime"));
             /*       else
                        g.DrawString(o.period.dtc.Value.ToString("MM-dd HH:mm"), new Font("Arial", 8), new SolidBrush(Color.Gray), r_msgtime);
                        */
                }
                if (o != null && o.userCreater != null)
                {
                    g.DrawString(o.userCreater, new Font("Arial", 8), new SolidBrush(Color.Gray), GetPoint("r_FIO"));
                }

                //                 _this_bitmap.Save("c:\\temp\\chat\\image" + this.MessageObj.ObjId.ToString() + ".jpg");

                if (g_screen != null)
                    g_screen.DrawImage(_this_bitmap, r.X+ Position_LeftStart, r.Y);
                try
                {
                    OnMessageShownEvent?.Invoke(this);
                    if (OnMessageShownEvent == null)
                    {

                    }
                }
                catch (Exception err)
                {
                    err.XROGi(MethodBase.GetCurrentMethod().DeclaringType.FullName);
                }
            }
        }
        public static Image FixedSize( Image imgPhoto, int Width, int Height)
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
            grPhoto.Clear(Color.White );
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        private PointF GetPoint(string v)
        {
            Rectangle rr = GetRect(v);
            if (rr==null)
            {

            }
            return rr.Location;
         //   return elements.Where(s => s.Name == v).SingleOrDefault().AreaPosition.Location;
           // return null;
        }
        private Rectangle GetRect(string v)
        {
            var rrrr = elements.Where(s => s.Name == v).SingleOrDefault();
        if (rrrr != null)
            return rrrr.AreaPosition;

            throw new Exception(v + " не найден");
    //     return null;
            // return null;
        }
        private void PrepareRegions()
        {
            elements.Clear();


           if (Position== MessagePosition.xrMyMessage)
            {
                LeftStart = Width - Data_Width - FotoBox;
            }
            Msg_Height = 70;
            Info_Height = 25;
            Msg_Height = Height - Info_Height;
            r = new Rectangle(LeftStart, 0, Data_Width + FotoBox, Height);
            int Msg_X_Start = LeftStart+FotoBox;


         
         

            if (Width >= 100)
            {
                Rectangle r_msg = new Rectangle(Msg_X_Start, 0, r.Width - FotoBox, Msg_Height);
                r_msg.Inflate(-3, -2);
                elements.Add(new MessageElementDraw()
                {
                    Name = "r_msg",
                    AreaPosition = r_msg,// new Rectangle(LeftStart, FotoBox + 5, 16, 16),
                    ImageKey = "msg",
                    Text = "Область сообщения",
                    Visible = true,
                    RegionType = MessageRegion.xrMessage
                });

                b_ShowUserImage = true;
            }
            else
            {
                Rectangle r_msg = new Rectangle(LeftStart, 0, Width + FotoBox, Msg_Height);
                elements.Add(new MessageElementDraw()
                {
                    Name = "r_msg",
                    AreaPosition = r_msg,// new Rectangle(LeftStart, FotoBox + 5, 16, 16),
                    ImageKey = "msg",
                    Text = "Область сообщения",
                    Visible = true,
                    RegionType = MessageRegion.xrMessage
                });

                b_ShowUserImage = false;
            }

          //  r_image.Inflate(-5, -5);
            elements.Add(new MessageElementDraw()
            {
                Name = "r_image",
                AreaPosition = new Rectangle(LeftStart, 0, FotoBox, FotoBox),// new Rectangle(LeftStart, FotoBox + 5, 16, 16),
                ImageKey = "image",
                Text = "Область фото",
                Visible = b_ShowUserImage,
                RegionType = MessageRegion.xrImage
            });

            //   r_like = new Rectangle(LeftStart, FotoBox + 5, 16, 16); ;
            //    r_dislike = new Rectangle(LeftStart+18,  FotoBox + 5, 16, 16); ;
            // r_work = new Rectangle(LeftStart + 18+18, FotoBox + 5, 16, 16); ;


            elements.Add(new MessageElementDraw()
            {
                Name = "r_like",
                AreaPosition = new Rectangle(LeftStart, FotoBox + 5, 16, 16),
                ImageKey = "like",
                Text = "Кнопка понравилось",
                Visible = true,
                RegionType = MessageRegion.xrLike
            });
            elements.Add(new MessageElementDraw()
            {
                Name = "r_dislike",
                AreaPosition = new Rectangle(LeftStart + 18, FotoBox + 5, 16, 16),
                ImageKey = "dislike",
                Text = "Кнопка не понравилось",
                Visible = true,
                RegionType = MessageRegion.xrDisLike
            });

            elements.Add(new MessageElementDraw()
            {
                Name = "r_work",
                AreaPosition = new Rectangle(LeftStart + 5, 5, 16, 16),
//                AreaPosition = new Rectangle(LeftStart + 18 + 18, FotoBox + 5, 16, 16),
                ImageKey = "work",
                Text = "Кнопка создание задачи",
                Visible = true,
                RegionType = MessageRegion.xrWorkBtn
            });
            Rectangle r1 = (new Rectangle(LeftStart, Msg_Height, Data_Width + FotoBox, Info_Height));
            r1.Inflate(-3, -2);
            elements.Add(new MessageElementDraw()
            {
                Name = "r_msginfo",
                AreaPosition = r1 ,
                ImageKey = "msginfo",
                Text = "Область информации о сообщении",
                Visible = true,
                RegionType = MessageRegion.xrInfo
            });
             

       //    
           /* r_msginfo = new Rectangle(LeftStart, Msg_Height, Data_Width + FotoBox, Info_Height); ;
            r_msginfo.Inflate(-3, -2);
            */
            int WidthTime = 50;
            Rectangle r_msgtime = new Rectangle(LeftStart+Data_Width + FotoBox - WidthTime, Msg_Height, WidthTime, Info_Height); ;
            r_msgtime.Inflate(-7, -4);
            elements.Add(new MessageElementDraw()
            {
                Name = "r_msgtime",
                AreaPosition = r_msgtime,
                ImageKey = "r_msgtime",
                Text = "Область времени",
                Visible = true,
                RegionType = MessageRegion.xrTime
            });

            Rectangle r_FIO = new Rectangle(LeftStart, Msg_Height, Data_Width + FotoBox - WidthTime, Info_Height); ;
            r_FIO.Inflate(-7, -4);
            elements.Add(new MessageElementDraw()
            {
                Name = "r_FIO",
                AreaPosition = r_FIO,
                ImageKey = "fio",
                Text = "Область ФИО",
                Visible = true,
                RegionType = MessageRegion.xrFIO
            });
            Rectangle r_msginfo = new Rectangle(LeftStart, Msg_Height, Data_Width + FotoBox, Info_Height); ;
            //r_msginfo.Inflate(-3, -2);

            elements.Add(new MessageElementDraw()
            {
                Name = "msginfo",
                AreaPosition = r_msginfo,
                ImageKey = "msginfo",
                Text = "Область ФИО",
                Visible = true,
                RegionType = MessageRegion.xrInfo
            });

           
            //r_msginfo.Inflate(-3, -2);

            // if (this.Region == null)


        }

        private void PrepareRegionsVers_2Image()
        {

            if (Position == MessagePosition.xrMyMessage)
            {
                 Position_LeftStart = Width - Data_Width - FotoBox;
                LeftStart = 0;
            }
             
            Msg_Height = Msg_Height; //70
            Info_Height = 25;
            Msg_Height = Height - Info_Height;
            r = new Rectangle(LeftStart, 0, Data_Width + FotoBox, Height);
            int Msg_X_Start = LeftStart + FotoBox;
            /*
            if (Width >= 100)
            {
                r_msg = new Rectangle(Msg_X_Start, 0, r.Width - FotoBox, Msg_Height);
                r_msg.Inflate(-3, -2);
                b_ShowUserImage = true;
            }
            else
            {
                r_msg = new Rectangle(LeftStart, 0, Width + FotoBox, Msg_Height);
                b_ShowUserImage = false;
            }*/
            //      r_image = new Rectangle(LeftStart, 0, FotoBox, FotoBox); ;
            //      r_image.Inflate(-5, -5);

            //       r_msginfo = new Rectangle(LeftStart, Msg_Height, Data_Width + FotoBox, Info_Height); ;
            //r_msginfo.Inflate(-3, -2);
            int WidthTime = 50;
       //     r_msgtime = new Rectangle(LeftStart + Data_Width + FotoBox - WidthTime, Msg_Height, WidthTime, Info_Height); ;
        //    r_msgtime.Inflate(-7, -4);
         //   r_FIO = new Rectangle(LeftStart, Msg_Height, Data_Width + FotoBox - WidthTime, Info_Height); ;
         //   r_FIO.Inflate(-7, -4);

            // if (this.Region == null)

            Width = r.Width;
            Height = r.Height;
        }
        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        internal void LoadContainFiles(Job job, int TypeFile )
        {
            if (o.isImage())
            {
         //       MsgText = "идёт загрузка...";
                string[] file = o.GetFiles();
                if (file != null)
                {
                    foreach (string guid in file)
                    {
                        if (guid.Trim() != "")
                        {
                            Image img = null; //job.Message_Image_Get((int)o.ObjId, guid,0); // плохо. нет иконки

                            //if (imgicon == null)
                            if ((files.Where(s=>s.Item3== guid && s.Item2!=null).Any()==false))
                            {
                                //         var events = job.GetType().GetEvents();//.OnMessage_GetFileAsync
                   //             job.OnMessage_GetFileAsync += Job_OnMessage_GetFileAsync; ;


                                job.Message_GetFile((int)o.ObjId, guid, TypeFile); //   иконка
                            }else
                            {

                            }
                             
                            /*
                            if (img != null || null != imgicon)
                            {
                                files.Add(new Tuple<Image, Image, string>(img, imgicon, guid));
                            }
                            else
                            {
                                //вообще нет ничего
                            }*/

                        }
                    }
                }
            }

        }

        private void Job_OnMessage_GetFileAsync(asyncReturn_GetFile ret, int ObjId, byte[] data)
        {
           
        }

        

        internal void RefreshPrepare()
        {
            PrepareRegionsVers_2Image();
            bool b_RegenImage = true;
            PrepareRegions();
            PaintMessageV1(null, b_RegenImage);// рисуем в Bitmap
            OnNeedRedraw(this);
        }

        internal void AddParentMsg(XMessageCtrl mc)
        {
            Data.Add(mc);
            //Replay = mc;
      //      throw new NotImplementedException();
        }

        internal void AddText(string v)
        {
            Data.Add(v);
         //   throw new NotImplementedException();
        }

        internal void SetCurrntcChat(long v)
        {
            chatId =(int) v;
        }

        internal string Data2String()
        {
            string ret = String.Empty;
  //          ret = "<root>";

            foreach (object ob in Data)
                {
                    switch (ob)
                    {

                        case  string s:
                            {
                            ret += "<text  ver=\"1.0\">" + s + "</text>";
                            break;
                            }
                    case XMessageCtrl reply:
                        {
                            ret += "<reply ver=\"1.0\" id=\""+ reply._objid.ToString()+ "\">" + reply.Data2String() + "</reply>";
                            break;
                        }
                    case Image image:
                            {
                                break;
                            }
                    default:
                        ret += "<text  ver=\"1.0\">??? тип данных в сообщении не определён. ???Data2String." + "</text>";
                        break;
                    }
                }
            ret += "<text  ver=\"1.0\">" + o.GetText() + "</text>";
 //           ret += "</root>";
            return ret;
        }
    }
}
