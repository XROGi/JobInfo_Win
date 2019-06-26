using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using CefSharp;
//using CefSharp.WinForms;
using JobInfo.XROGi_Extensions;


namespace JobInfo.XROGi_Class
{
    enum XChatMessagePanelMode { ShowMesages, EditOneMessage,SelectedMessages};


    /// <summary>
    /// Класс показывающий все сообщения чата
    /// </summary>
    class ChatMessagesPanel : Control
    {
        bool ChromiumDebug = true;
        public XChatMessagePanelMode Mode;
        public bool b_boss = false;
        int CountPreCashe = 30;
        Job job;
        Chat chat;
        public ImageList ImageListMsg { get; set; }

        public bool DebugInfoDraw { get; set; }
        public bool bShowBtnGoToListEnd = true;
        public bool bShowBtnGoToListNewMessages = true;

        public ContextMenuStrip ContextMenuStripMessage { get; set; }
        

        /*Отсупы сообщений*/
        int SpaceMessagesHeight = 15;
        /*Отсупы Прокрутки*/
        int SpaceScrollLeft = 15;
        int SpaceScrollMidleLine = 5;

        int SpaceScrollFirst5 = 15;

        int ButtonRightWidthDelta = 30; // гширина кнопки вниз 

        ShowMessagePosition LastMoveTo;
        int ShowMessage_Top = 0;
        /*Region занимаемая область
         object1  - описанеи типа региона
         object2  - Обьект региона (сообщение)
             */
        List<Tuple<Region, object>> regions = new List<Tuple<Region, object>>();

        int _ShownMessage_Bottom=0;
        int ShownMessage_Bottom
        {
            get { return _ShownMessage_Bottom; }
            set { _ShownMessage_Bottom = value; }
        } //ID = ObjId
        int _ShownMessage_Bottom_Index = -1;
        int ShownMessage_Bottom_Index
        {
            get { return _ShownMessage_Bottom_Index; }
            set { 
                if (value==-1)
                {

                }
                _ShownMessage_Bottom_Index = value;
            }

        }


        int ShowBottomDeltaPosition = 0; // Указывает насколько нижнее сообщение в панели смещено вниз отностительно нижнего угла. 
        int ShowBottomDeltaPositionAddons = 2; // Указывает на сколько сместить за одну итеррацию
        int ShowBottomDeltaPositionTimeMs = 20; // Указывает на сколько сместить за одну итеррацию 
        int ShowBottomDeltaAlphaTimeMs = 20; //Скорость  погасание прозрачности


        List<UserClassControl> users;
 //       List<UserClassControl> users_add;
        SortedList<int, XMessageCtrl> messages;
        List<Control> showObject = new List<Control>();

        List<int> Order_RequestMessageText = new List<int>();
        enum ShowMessagePosition { LastInTop, LastOnBootom };


        Timer timer = new Timer();

        Timer timer_ModeShowList2EditMessage = new Timer();

        public delegate void OnMouseMessageClickDelegate(XMessageCtrl sender, ChatPanelElement el, MessageRegion mr);
        public event OnMouseMessageClickDelegate OnMouseMessageClick = delegate { };





        public ChatMessagesPanel()
        {
            // job = _job;
            Mode = XChatMessagePanelMode.ShowMesages;
            Color1 = Color.YellowGreen;
            Color2 = Color.LightGreen;
            Angle = 30;
            messages = new SortedList<int, XMessageCtrl>();
            DoubleBuffered = true;
            timer.Interval = ShowBottomDeltaPositionTimeMs;
            timer.Tick += onTimer_MoveMessage;
            timer.Stop();

            timer_ModeShowList2EditMessage.Interval = ShowBottomDeltaAlphaTimeMs;
            timer_ModeShowList2EditMessage.Tick += onTimer_ModeShowList2EditMessage;
            timer_ModeShowList2EditMessage.Stop();
        }

        private void onTimer_MoveMessage(object sender, EventArgs e)
        {
            /*
            ShowBottomDeltaPosition += ShowBottomDeltaPositionAddons;
            if (ShowBottomDeltaPosition >= 1500)
            {
                timer.Stop();
                ShowBottomDeltaPosition = 0;
                ShowBottomDeltaPositionAddons = 2;
            }
            Refresh();*/
        }

        protected override void OnResize(EventArgs eventargs)
        {
           
            //       if (Full_Width != Width)
            
            int _Width = this.Width; ;
            {
                try
                {
                    foreach (XMessageCtrl mc in showObject.Cast<XMessageCtrl>().ToArray())
                    {
                        if (mc.Full_Width > _Width)
                        {
                            mc.SetWidth(_Width);
                            mc.Calculate_Size();
                            mc.RefreshPrepare();
                            //     MessageChatControl_RefreshImage(this.Size, (int)mc.MessageObj.ObjId, mc.MessageObj);
                        }
                    }

                    foreach (XMessageCtrl mc in messages.Select(s => s.Value))
                    {
                        if (mc.Full_Width > _Width)
                        {
                            mc.SetWidth(_Width);
                            mc.Calculate_Size();
                            mc.RefreshPrepare();
                        }
                    };
                }
                catch (Exception err)
                {

                }
                
            }
            base.OnResize(eventargs);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            foreach (var region in regions)
            {
                Region r = region.Item1;
                var rr = r.GetRegionData();
                if (r.IsVisible(e.Location))
                {
                    //onMouseObjectMove();
                }
               
            }
           
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            try
            {
                base.OnMouseDown(e);
                bool b_refresh = false;
                foreach (var region in regions)
                {
                    Region r = region.Item1;
                    var rr = r.GetRegionData();
                    if (r.IsVisible(e.Location))
                    {
                        switch (region.Item2)
                        {
                            case ChatPanelElement el:
                                if (el.ElementType == ChatPanelElementType.btnLastShown)
                                {
                                    Display_Message(chat.statistic.LastShownObjId, ShowMessagePosition.LastOnBootom);
                                    b_refresh = true;
                                }
                                if (el.ElementType == ChatPanelElementType.btnEndMessage)
                                {
                                    Display_Message(chat.statistic.LastObjId, ShowMessagePosition.LastOnBootom);
                                    b_refresh = true;
                                }
                                if (el.ElementType == ChatPanelElementType.btnScrollPoint)
                                {
                                    Display_Message(el.GetTagInt(), ShowMessagePosition.LastOnBootom);
                                    b_refresh = true;
                                }

                                if (el.ElementType == ChatPanelElementType.pnlMessageTextRegion)
                                {
                                    XMessageCtrl mc = el.Get_Tag_MessageChatControl();

                                    MessageRegion mr = MessageRegion.xrNone;
                                    if (mc != null)
                                    {
                                        mr = mc.GetRegion(e.X - mc.Left, e.Y - mc.Top);

                                        if (mr == MessageRegion.xrMessage)
                                        {
                                            if (mc.wb!=null)
                                            {
                                                if (mc.wb != null)
                                                {
                                                    mc.wb.ScriptErrorsSuppressed = true;
           /* if (ChromiumDebug == false)
                                                    {
                                                        CefSettings settings = new CefSettings();
                                                        ChromiumWebBrowser chromeBrowser;
                                                        try
                                                        {
                                                            if (Cef.IsInitialized != true)
                                                                Cef.Initialize(settings);
                                                        }
                                                        catch (Exception err)
                                                        {

                                                        }
                                                        // Create a browser component

                                                        //                                                    chromeBrowser = new ChromiumWebBrowser("https://www.google.com/");
                                                        chromeBrowser = new ChromiumWebBrowser();
                                                        chromeBrowser.LoadHtml(mc.HTML);
                                                        chromeBrowser.Left = mc.Left;
                                                        chromeBrowser.Top = mc.Top;
                                                        chromeBrowser.Height = mc.Height;
                                                        chromeBrowser.Width = mc.Width;
                                                        // Add it to the form and fill it to the form window.
                                                        this.Controls.Add(chromeBrowser);
                                                        chromeBrowser.Anchor = AnchorStyles.Top;//.Dock = DockStyle.Fill;

                                                        //
                                                        //var webView = new WebView("http://play.spotify.com", new CefSharp.BrowserSettings());
                                                        //this.Controls.Add(webView);
                                                        //webView.Dock = DockStyle.Fill;
                                                        //webView.Address = "http://play.spotify.com";
                                                        //SPHeader p = new SPHeader();
                                                        //this.Controls.Add(p);
                                                        //p.Dock = DockStyle.Top;

                                                        //p.MouseMove += p_MouseMove;
                                                        //p.MouseDoubleClick += p_MouseDoubleClick;

                                                        //SPFooter footer = new SPFooter();
                                                        //this.Controls.Add(footer);
                                                        //footer.Dock = DockStyle.Bottom;
                                                        //
                                                        //        this.Controls.Clear();
                                                        //         mc.wb.Left = 0 + 15;
                                                        //         mc.wb.Top = 15;
                                                        //         this.Controls.Add(mc.wb);
                                                    }
*/
                                                }
                                            }
                                        }
                                         
                                    }

                                    if (e.Button == MouseButtons.Left && e.Clicks == 2)
                                    {

                                        OnMouseMessageClick(el.Get_Tag_MessageChatControl(), el, mr);
                                        break;
                                    }
                                    if (e.Button == MouseButtons.Right)
                                    {
                                        //          MessageRegion mr = mc.GetRegion(e.X - mc.Left, e.Y - mc.Top);
                                        Point loc = this.PointToClient(Cursor.Position);
                                        Point yyy = this.PointToClient(Point.Empty);
                                        int x = mc.PointToClient(e.Location).X; ;
                                        int y = mc.PointToClient(e.Location).Y;
                                        ContextMenuStripMessage.Show(mc, -yyy.X + x, -yyy.Y + y);
                                    }
                                    //                                b_refresh = true;
                                }
                                break;


                            case XMessageCtrl mc:
                                if (e.Button == MouseButtons.Right)
                                {
                                    MessageRegion mr = mc.GetRegion(e.X - mc.Left, e.Y - mc.Top);
                                    ContextMenuStripMessage.Show(mc, Cursor.Position);
                                }
                                if (e.Button == MouseButtons.Left && e.Clicks == 2)
                                {
                                    //  mc.
                                    //  OnMouseMessageClick(mc, null);
                                }
                                break;
                            default:
                                break;
                        }




                        //onMouseObjectMove();
                    }

                }
                if (b_refresh) Refresh(); ;
            }catch (Exception err)
            {
                E(err);
            }
        }

        private void E(Exception err)
        {
            
        }

        public Color Color1 { get; set; }
        public Color Color2 { get; set; }

        public float Angle { get; set; }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                                                                       Color1,
                                                                       Color2,
                                                                       Angle))
            {
                pevent.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        internal void Close()
        {
            //    throw new NotImplementedException();
            CloseChatOnPanel();
            regions?.Clear();
            showObject?.Clear();
            users?.Clear();
            messages?.Clear();
            Order_RequestMessageText?.Clear();
            job = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            base.OnPaint(e);
            if (Mode == XChatMessagePanelMode.ShowMesages)
                DrawFrame(g);
            if (Mode == XChatMessagePanelMode.EditOneMessage)
            {
                regions.Clear();
                showObject.Clear();
                DrawBitmapAlpha(g, vd);
                
            }
        }

        private void DrawBitmapAlpha(Graphics g, XDraw_VisualData1 vd)
        {
            ColorMatrix cm = new ColorMatrix();

            if (vd.Value + vd.Delta == vd.AlphaStop)
            {
             //   vd.Value = vd.AlphaStop;
            }
            else
                vd.Value = vd.Value + vd.Delta;

            cm.Matrix33 = 1.0f * vd.Value / 100;
            //* 0.55f;
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            g.DrawImage(vd.bitmap_beforeEdit, new Rectangle(0,0, Width, Height), 0, 0, Width, Height, GraphicsUnit.Pixel, ia);

            g.DrawImage(vd.mc._this_bitmap, vd.mc.Left, vd.mc.Top);

        }

        private void DrawFrame(Graphics g)
        {
            try
            {
                //Анимированй gif
                //https://stackoverflow.com/questions/8292710/c-sharp-winforms-drawimage-without-losing-animation
                this.Controls.Clear();
             regions.Clear();
                showObject.Clear();
                Pen pen_ten = new Pen(Color.Blue);
                Pen pen_zero = new Pen(Color.Green);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                //https://stackoverflow.com/questions/34872659/anti-aliasing-for-regions-in-c-sharp-and-alpha-masking
                


                if (chat != null && chat.MessMessageArray != null)
                {
                    if (b_boss)
                        g.DrawString("chatid=" + chat.chatId.ToString() + " " + chat.MessMessageArray.Count.ToString() + " " + DateTime.Now.ToLongTimeString(), new Font("Arial", 8), new SolidBrush(Color.Gray), 0, 0);
                }
                else
                    g.DrawString("Нет соединения с чатом" + " " + DateTime.Now.ToLongTimeString(), new Font("Arial", 8), new SolidBrush(Color.Gray), 0, 0);

                ;
                if (chat == null)
                {
                    return;
                }
                /*
                 Область рисования сообщений
                */
                int index = 0;
                #region  Область рисования сообщений
                if (LastMoveTo == ShowMessagePosition.LastOnBootom) //Рисуем снизу вверх
                {
                    index = messages.IndexOfKey(ShownMessage_Bottom);
                    if (messages.Count > 0 && index >= 0)
                    {
                        int index_min = index - 20 < 0 ? 0 : index - 20;
                        int step = 100;
                        int _H = ShowBottomDeltaPosition;
                        while (true)
                        {
                            if (index < 0) break; // дошли до -1первого сообщения
                            if (step-- < 0) break; //защита

                            var t91 = messages.Keys[index];
                            XMessageCtrl mc = messages.Values[index];

                            _H += mc.Height;// высота сообщения

                            int LeftPosition = 0;
                            int TopPosition = this.Height - _H;
                            if (TopPosition + mc.Height <= 0)
                                break;// Выходит за пределы экрана
                            showObject.Add(mc); // список выведенных сообщений
                            if (mc.Position == MessagePosition.xrMyMessage)
                            {
                                LeftPosition = Width - mc.Data_Width - mc.FotoBox;
                            }
                            int msg_Top = this.Height - _H;
                            if ((msg_Top < 0)) // если выходит выше экрана, то нарисовать полупрозрачным
                            {

                                ColorMatrix cm = new ColorMatrix();
                                cm.Matrix33 = 0.25f;
                                ImageAttributes ia = new ImageAttributes();
                                ia.SetColorMatrix(cm);
                                /*
                                PointF ulCorner1 = new PointF(0, 0);
                                PointF urCorner1 = new PointF(0, 100.0F);
                                PointF llCorner1 = new PointF(100, 100F);
                                PointF[] destPara1 = { ulCorner1, urCorner1, llCorner1 };
                                */
                                //20190311canvas.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);


                                int left = this.Left + 7;
                                if (mc.Position == MessagePosition.xrMyMessage)
                                {
                                    left = this.Width - mc._this_bitmap.Width - 7;
                                }
                                left = this.Left;
                                Rectangle rrrr = new Rectangle(left, msg_Top, ((Bitmap)mc._this_bitmap).Width, ((Bitmap)mc._this_bitmap).Height);
                                PointF[] destPara1 = GetPoints(rrrr);
                                mc.Left = rrrr.Left;
                                mc.Top = rrrr.Top;
                                g.DrawImage(mc._this_bitmap, destPara1, rrrr, GraphicsUnit.Pixel, ia);
                            }
                            else
                            {


                                int left = this.Left + 7;
                                if (mc.Position == MessagePosition.xrMyMessage)
                                {
                                    left = this.Width - mc._this_bitmap.Width - 7;
                                }

                                regions.Add(new Tuple<Region, object>
                                      (
                                          new Region(new Rectangle(left, TopPosition, mc._this_bitmap.Width, mc._this_bitmap.Height))
                                          , new ChatPanelElement(ChatPanelElementType.pnlMessageTextRegion, mc)
                                      )
                                      );
                                mc.Left = left;
                                mc.Top = TopPosition;
                                g.DrawImage(mc._this_bitmap, left, TopPosition);
                                /*if (mc.wb!=null)
                                {
                                    mc.wb.Left = left + 15;
                                    mc.wb.Top = TopPosition;
                                    this.Controls.Add(mc.wb);
                                }*/
                                //7        job.Message_Shown(chat.chatId, mc.MessageObj.ObjId);
                            }

                            index--;
                            _H += SpaceMessagesHeight; //
                        }


                    }
                    else
                    { //нет загруженных сообщений. Вывод преджзагрузки квадраты
                        index = chat.MessMessageArray.IndexOfKey(ShownMessage_Bottom);
                        if (index >= 0) //стартовое сообщение в хэш табл
                        {
                            int index_min = index - 20 < 0 ? 0 : index - 20;

                            var t9 = chat.MessMessageArray.Keys[index_min];
                            var msgs = chat.MessMessageArray.Where(s => s.Key >= t9 && s.Key <= ShownMessage_Bottom).OrderByDescending(s => s.Key).OrderBy(s => s.Key).ToArray();
                            int bottom = this.Height;

                            int _H = 0;


                            for (int i = 0; i < msgs.Length; i++)
                            {
                                _H += 30;
                                int id = msgs[i].Key;
                                if (messages?.ContainsKey(id) == false)
                                {
                                    _H += 70;
                                    if ((this.Height - _H) < 0)
                                    {

                                    }
                                    else
                                    {
                                        g.DrawRectangle(new Pen(Color.Gray, 2), this.Left, this.Height - _H, 120, 50);
                                        string print = "идёт загрузка..";
                                        if (b_boss) print += "\r\n\r\n" + "id=" + id.ToString();

                                        g.DrawString(print, new Font("Arial", 8), new SolidBrush(Color.Gray), this.Left + 10, this.Height - _H);
                                    }
                                }
                                else
                                { // невероятно 
                                    var mc = messages?[id];
                                    _H += mc.Height;// высота сообщения
                                    g.DrawImage(messages?[id]._this_bitmap, this.Left, this.Height - _H);
                                    int w = ((Bitmap)messages?[id]._this_bitmap).Width;
                                    if ((this.Height - _H < 0))
                                    {
                                        ColorMatrix cm = new ColorMatrix();
                                        cm.Matrix33 = 0.15f;
                                        ImageAttributes ia = new ImageAttributes();
                                        ia.SetColorMatrix(cm);

                                        Rectangle rrrr = new Rectangle(this.Left, this.Height - _H, ((Bitmap)mc._this_bitmap).Width, ((Bitmap)mc._this_bitmap).Height);
                                        PointF[] destPara1 = GetPoints(rrrr);
                                        g.DrawImage(mc._this_bitmap, destPara1, rrrr, GraphicsUnit.Pixel, ia);
                                        /*
                                           PointF ulCorner1 = new PointF(0, 0);
                                           PointF urCorner1 = new PointF(0, 100.0F);
                                           PointF llCorner1 = new PointF(100, 100F);
                                           PointF[] destPara1 = { ulCorner1, urCorner1, llCorner1 };
                                        */
                                        //20190311canvas.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
                                        //    Rectangle rrrr = new Rectangle(this.Left, this.Height - _H, ((Bitmap)messages?[id]._this_bitmap).Width, ((Bitmap)messages?[id]._this_bitmap).Height);
                                        //    e.Graphics.DrawImage(messages?[id]._this_bitmap, destPara1 , rrrr , GraphicsUnit.Pixel, ia);
                                    }

                                }
                            }
                        }
                    }



                }
                #endregion


                bool b_ShowDownScroll = true;
                bool b_ShowTopScroll = true;
                bool b_ShowBtnScroll = true;

                /*
                 Область рисования верхней прокрутки
                */
                #region  Область рисования верхней прокрутки
                if (b_ShowTopScroll)
                {
                    int SpaceScrollMidleLine = 15;
                    int SpaceScrollTopYLine = 15;
                    int lineEnd = Width - SpaceScrollLeft;
                    g.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollTopYLine, lineEnd, SpaceScrollTopYLine);
                    /*Рисует точки отчёта*/
                    if (chat?.MessMessageArray.Count > 0)
                    {
                        index = chat.MessMessageArray.IndexOfKey(ShownMessage_Bottom);
                        //7e.Graphics.DrawString(index.ToString(), new Font("Arial", 8), new SolidBrush(Color.Gray), Width - 50, 70);
                        int indexUsed = 0;
                        if (index >= 0) //стартовое сообщение в хэш табл
                        {
                            //   if (index > 100)
                            {
                                int PositionX = SpaceScrollMidleLine;
                                for (int i = 1; i <= 10; i++)
                                {
                                    if (index < indexUsed) break;
                                    //  
                                    g.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollMidleLine, Width - SpaceScrollLeft, SpaceScrollMidleLine);

                                    Rectangle recTen = new Rectangle(PositionX - 2, SpaceScrollTopYLine - 2, 4, 4);
                                    if ((index - indexUsed) % 10 == 0)
                                    {

                                        g.FillRectangle(new SolidBrush(Color.Green), recTen);
                                    }
                                    else
                                        g.DrawRectangle(pen_ten, recTen);

                                    Rectangle rec10 = new Rectangle(PositionX - SpaceScrollMidleLine / 2, SpaceScrollMidleLine - SpaceScrollMidleLine / 2, SpaceScrollMidleLine, SpaceScrollMidleLine);
                                    int objid = chat.MessMessageArray.ElementAt(indexUsed).Key;
                                    regions.Add(new Tuple<Region, object>(new Region(rec10), new ChatPanelElement(ChatPanelElementType.btnScrollPoint, objid)));

                                    PositionX += SpaceScrollMidleLine;
                                    indexUsed += 1;
                                    if (PositionX >= lineEnd) break;

                                }

                                for (int i = 1; i <= 10; i++)
                                {
                                    if (index < indexUsed) break;
                                    //                            e.Graphics.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollMidleLine, Width - SpaceScrollLeft, SpaceScrollMidleLine);
                                    g.DrawCircle(new Pen(Color.Blue), PositionX, SpaceScrollMidleLine, 3);

                                    if ((index - indexUsed) % 100 <= 9)
                                    {
                                        g.FillCircle(new SolidBrush(Color.Blue), PositionX, SpaceScrollMidleLine, 3);
                                    }

                                    Rectangle rec100 = new Rectangle(PositionX - SpaceScrollMidleLine / 2, SpaceScrollMidleLine - SpaceScrollMidleLine / 2, SpaceScrollMidleLine, SpaceScrollMidleLine);

                                    // Region r_circle = GetRegionCircle(PositionX, SpaceScrollMidleLine, 5);
                                    int objid = chat.MessMessageArray.ElementAt(indexUsed).Key;
                                    regions.Add(new Tuple<Region, object>(new Region(rec100) /*r_circle*/, new ChatPanelElement(ChatPanelElementType.btnScrollPoint, objid)));

                                    PositionX += SpaceScrollMidleLine;
                                    indexUsed += 10;
                                    if (PositionX >= lineEnd) break;
                                }
                                for (int i = 1; i <= 10; i++)
                                {
                                    for (int j = 1; j <= 10; j++)
                                    {
                                        indexUsed += 100;
                                        if (index < indexUsed) break;


                                        //                            e.Graphics.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollMidleLine, Width - SpaceScrollLeft, SpaceScrollMidleLine);

                                        g.DrawCircle(new Pen(Color.Red), PositionX, SpaceScrollMidleLine, 3);
                                        if ((index - indexUsed) % 1000 <= 100)
                                        {
                                            g.FillCircle(new SolidBrush(Color.Blue), PositionX, SpaceScrollMidleLine, 3);
                                        }

                                        Rectangle rec1000 = new Rectangle(PositionX - SpaceScrollMidleLine / 2, SpaceScrollMidleLine - SpaceScrollMidleLine / 2, SpaceScrollMidleLine, SpaceScrollMidleLine);
                                        //                                    Region r_circle = GetRegionCircle(PositionX, SpaceScrollMidleLine, 5);
                                        int objid = chat.MessMessageArray.ElementAt(indexUsed).Key;
                                        regions.Add(new Tuple<Region, object>(new Region(rec1000) /*r_circle*/, new ChatPanelElement(ChatPanelElementType.btnScrollPoint, objid)));


                                        PositionX += SpaceScrollMidleLine;
                                        if (PositionX >= lineEnd) break;
                                    }
                                    if (PositionX >= lineEnd) break;
                                }
                            }

                        }
                    }
                }
                #endregion



                /*
                 Область рисования нижней прокрутки
                */
                #region    Область рисования нижней прокрутки
                if (b_ShowDownScroll)
                {
                    int SpaceScrollMidleLine = 15;
                    int SpaceScrollDownYLine = this.Height - 5;

                    int lineEnd = SpaceScrollLeft + ButtonRightWidthDelta; //+для симметрии
                    int lineStart = Width - lineEnd - ButtonRightWidthDelta;
                    g.DrawLine(new Pen(Color.LightBlue), lineStart, SpaceScrollDownYLine, lineEnd, SpaceScrollDownYLine);
                    int index_max = chat.MessMessageArray.Count() - 1;
                    /*Рисует точки отчёта*/
                    if (chat?.MessMessageArray.Count > 0)
                    {
                        index = chat.MessMessageArray.IndexOfKey(ShownMessage_Bottom);
                        //                        e.Graphics.DrawString(index.ToString(), new Font("Arial", 8), new SolidBrush(Color.Gray), Width - 50, 70);
                        int indexUsed = 0;
                        if (index >= 0) //стартовое сообщение в хэш табл
                        {
                            //   if (index > 100)
                            {
                                int PositionX = lineStart;
                                for (int i = 1; i <= 10; i++)
                                {
                                    if (index + indexUsed >= index_max)
                                        break;
                                    //  
                                    //7                                    e.Graphics.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollMidleLine, Width - SpaceScrollLeft, SpaceScrollMidleLine);

                                    Rectangle recTen = new Rectangle(PositionX - 2, SpaceScrollDownYLine - 2, 4, 4);
                                    if ((index + indexUsed) % 10 == 0)
                                    {

                                        g.FillRectangle(new SolidBrush(Color.Green), recTen);
                                    }
                                    else
                                        g.DrawRectangle(pen_ten, recTen);

                                    Rectangle rec10 = new Rectangle(PositionX - SpaceScrollMidleLine / 2, SpaceScrollMidleLine - SpaceScrollMidleLine / 2, SpaceScrollMidleLine, SpaceScrollMidleLine);
                                    int objid = chat.MessMessageArray.ElementAt(index + indexUsed).Key;
                                    regions.Add(new Tuple<Region, object>(new Region(rec10), new ChatPanelElement(ChatPanelElementType.btnScrollPoint, objid)));

                                    PositionX -= SpaceScrollMidleLine;
                                    indexUsed += 1;
                                    if (PositionX <= lineEnd) break;

                                }


                                for (int i = 1; i <= 10; i++)
                                {
                                    if (index + indexUsed >= index_max)
                                        break;
                                    //                            e.Graphics.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollMidleLine, Width - SpaceScrollLeft, SpaceScrollMidleLine);
                                    g.DrawCircle(new Pen(Color.Blue), PositionX, SpaceScrollDownYLine, 3);

                                    if ((index + indexUsed) % 100 <= 9)
                                    {
                                        g.FillCircle(new SolidBrush(Color.Blue), PositionX, SpaceScrollDownYLine, 3);
                                    }

                                    Rectangle rec100 = new Rectangle(PositionX - SpaceScrollMidleLine / 2, SpaceScrollDownYLine - SpaceScrollMidleLine / 2, SpaceScrollMidleLine, SpaceScrollMidleLine);

                                    // Region r_circle = GetRegionCircle(PositionX, SpaceScrollMidleLine, 5);
                                    int objid = chat.MessMessageArray.ElementAt(index + indexUsed).Key;
                                    regions.Add(new Tuple<Region, object>(new Region(rec100) /*r_circle*/, new ChatPanelElement(ChatPanelElementType.btnScrollPoint, objid)));

                                    PositionX -= SpaceScrollMidleLine;
                                    indexUsed += 10;
                                    if (PositionX <= lineEnd) break;
                                }


                                for (int i = 1; i <= 10; i++)
                                {
                                    for (int j = 1; j <= 10; j++)
                                    {
                                        indexUsed += 100;
                                        if (index + indexUsed >= index_max) break;


                                        //                            e.Graphics.DrawLine(new Pen(Color.LightBlue), SpaceScrollLeft, SpaceScrollMidleLine, Width - SpaceScrollLeft, SpaceScrollMidleLine);


                                        if ((index + indexUsed) % 1000 <= 100)
                                        {
                                            g.FillCircle(new SolidBrush(Color.Blue), PositionX, SpaceScrollDownYLine, 3);
                                        }
                                        else
                                            g.DrawCircle(new Pen(Color.Red), PositionX, SpaceScrollDownYLine, 3);

                                        Rectangle rec1000 = new Rectangle(PositionX - SpaceScrollMidleLine / 2, SpaceScrollDownYLine - SpaceScrollMidleLine / 2, SpaceScrollMidleLine, SpaceScrollMidleLine);
                                        //                                    Region r_circle = GetRegionCircle(PositionX, SpaceScrollMidleLine, 5);
                                        int objid = chat.MessMessageArray.ElementAt(index + indexUsed).Key;
                                        regions.Add(new Tuple<Region, object>(new Region(rec1000) /*r_circle*/, new ChatPanelElement(ChatPanelElementType.btnScrollPoint, objid)));


                                        PositionX -= SpaceScrollMidleLine;
                                        if (PositionX <= lineEnd) break;
                                    }
                                    if (PositionX <= lineEnd) break;
                                }

                            }
                        }
                    }
                }
                #endregion
                /*
                 Область рисования кнопок прокрутки. Кнопка к первомк непрочтённому.
                */

                #region Область рисования кнопок прокрутки. Кнопка к первомк непрочтённому.
                if (bShowBtnGoToListEnd)
                    if (chat?.statistic != null
                        && chat.statistic.LastObjId != ShownMessage_Bottom
                        )
                    {
                        Point CenterButton = new Point(this.Width - ButtonRightWidthDelta, this.Height - 30);

                        g.FillCircle(new SolidBrush(Color.White), CenterButton.X, CenterButton.Y, 20);
                        g.DrawCircle(new Pen(Color.LightGray), CenterButton.X, CenterButton.Y, 20);


                        Region r_circle = GetRegionCircle(CenterButton.X, CenterButton.Y, 20);


                        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddEllipse(CenterButton.X - 20, CenterButton.Y - 20, 40, 40);
                        //Creating the region from the circle path  

                        regions.Add(new Tuple<Region, object>
                                           (
                                               new Region(path)
                                               , new ChatPanelElement(ChatPanelElementType.btnEndMessage)
                                           )
                                           );

                        //SolidBrush sb = new SolidBrush(Color.LightGray);
                        Pen pen = new Pen(Color.LightGray);
                        pen.Width = 4;
                        g.DrawLines(pen, new Point[] { new Point(CenterButton.X - 7, CenterButton.Y - 3), new Point(CenterButton.X, CenterButton.Y + 5), new Point(CenterButton.X + 7, CenterButton.Y - 3) });
                    }
                #endregion
                /*
                 Область рисования кнопок прокрутк (новые сообщения)
                */
                #region Область рисования кнопок прокрутк (новые сообщения)
                if (b_ShowBtnScroll)
                    if (bShowBtnGoToListNewMessages)
                    {
                        if (chat?.statistic != null
                            && chat.statistic.CountNew > 0
                            && chat.statistic.LastObjId != ShownMessage_Bottom //условие заменить в онwheel 
                            )
                        {
                            SizeF s = g.MeasureString(chat.statistic.CountNew.ToString(), new Font("Arial", 8));
                            Point ElipsePoint = new Point(this.Width - ButtonRightWidthDelta - ((int)s.Width / 2) - 5, this.Height - 60 - 5);
                            SolidBrush pen = new SolidBrush(Color.DodgerBlue);
                            Point CenterButton = new Point(this.Width - ButtonRightWidthDelta - ((int)s.Width / 2), this.Height - 60);
                            g.FillEllipse(pen, ElipsePoint.X, ElipsePoint.Y, (int)s.Width + 10, (int)s.Height + 10);
                            g.DrawString(chat.statistic.CountNew.ToString(), new Font("Arial", 8), new SolidBrush(Color.White), CenterButton.X, CenterButton.Y);

                            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                            path.AddEllipse(ElipsePoint.X, ElipsePoint.Y, (int)s.Width + 10, (int)s.Height + 10);
                            //Creating the region from the circle path  

                            Region r_circle = GetRegionCircle(ElipsePoint.X, ElipsePoint.Y, (int)s.Width + 10, (int)s.Height + 10);

                            regions.Add(new Tuple<Region, object>
                                               (
                                                  r_circle
                                                   , new ChatPanelElement(ChatPanelElementType.btnLastShown)
                                               )
                                               );


                            /*                    e.Graphics.FillCircle(new SolidBrush(Color.White), CenterButton.X, CenterButton.Y, 20);
                                                e.Graphics.DrawCircle(new Pen(Color.LightGray), CenterButton.X, CenterButton.Y, 20);
                                                //SolidBrush sb = new SolidBrush(Color.LightGray);
                                                Pen pen = new Pen(Color.LightGray);
                                                pen.Width = 4;
                                                e.Graphics.DrawLines(pen, new Point[] { new Point(CenterButton.X - 7, CenterButton.Y - 3), new Point(CenterButton.X, CenterButton.Y + 5), new Point(CenterButton.X + 7, CenterButton.Y - 3) });*/
                        }
                        else
                        {

                        }


                    }
                #endregion

                stopwatch.Stop();
                var elapsed_time = stopwatch.ElapsedMilliseconds;

                g.DrawString("▲" + elapsed_time.ToString(), new Font("Arial", 8), new SolidBrush(Color.Gray), Width - 50, 0);
            }
            catch (Exception err)
            {
                g.DrawString(err.Message.ToString(), new Font("Arial", 8), new SolidBrush(Color.Gray), 0, 0);
            }
        }

        internal void onMessage_GetListID(int ChatId, int[] MessageArray)
        {
            chat?.MessageArrayAddRange(MessageArray);
        }

        internal void Chat_UnSelect()
        {
            CloseChatOnPanel();
            messages.Clear();
            regions.Clear();
            if (chat != null)
            {
                chat.OnChatEvent -= OnChatEvent;
                chat = null;
            }
            Refresh();
        
        }

        private Region GetRegionCircle(int x, int y, int v)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(x-v, y-v, v, v);
            return new Region(path);
        }

        private Region GetRegionCircle(int x, int y, int v1, int v2)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(x, y, v1, v2);
            return new Region(path);
        }
        /*private Region GetRegionCircle(int x, int y, int radius)
        {
           
        }*/

        private PointF[] GetPoints(RectangleF rectangle)
        {
            return new PointF[3]
            {
            new PointF(rectangle.Left, rectangle.Top),
            new PointF(rectangle.Right, rectangle.Top),
            new PointF(rectangle.Left, rectangle.Bottom)
            };
          /*  return new PointF[4]
            {
            new PointF(rectangle.Left, rectangle.Top),
            new PointF(rectangle.Right, rectangle.Top),
            new PointF(rectangle.Right, rectangle.Bottom),
            new PointF(rectangle.Left, rectangle.Bottom)
     
            };*/
        }


        /*
        protected void OnPaint_v0(PaintEventArgs e)
        {//https://stackoverflow.com/questions/34872659/anti-aliasing-for-regions-in-c-sharp-and-alpha-masking
            base.OnPaint(e);
            if (chat != null && chat.MessMessageArray != null)

                e.Graphics.DrawString(chat.MessMessageArray.Count.ToString(), new Font("Arial", 8), new SolidBrush(Color.Gray), 0, 0);
            else
                e.Graphics.DrawString("Нет соединения с чатом", new Font("Arial", 8), new SolidBrush(Color.Gray), 0, 0);

            ;
            if (LastMoveTo == ShowMessagePosition.LastOnBootom) //Рисуем снизу вверх
            {
                if (chat.MessMessageArray.Count > 0)
                {
                    int index = chat.MessMessageArray.IndexOfKey(ShownMessage_Bottom);
                    int index_min = index - 20 < 0 ? 0 : index - 20;

                    var t9 = chat.MessMessageArray.Keys[index_min];
                    var msgs = chat.MessMessageArray.Where(s => s.Key >= t9 && s.Key <= ShownMessage_Bottom).OrderByDescending(s => s.Key).ToArray();
                    int bottom = this.Height;

                    int _H = 0;


                    for (int i = 0; i <= msgs.Length - 1; i++)
                    {
                        _H += 30;
                        int id = msgs[i].Key;
                        if (messages?.ContainsKey(id) == false)
                        {
                            _H += 70;
                            if ((this.Height - _H) < 0)
                            {

                            }
                            else
                            {
                                e.Graphics.DrawRectangle(new Pen(Color.Gray, 2), this.Left, this.Height - 80, 120, 50);
                                e.Graphics.DrawString("идёт загрузка..", new Font("Arial", 8), new SolidBrush(Color.Gray), this.Left + 10, this.Height - _H);
                            }
                        }
                        else
                        {
                            //    messages?[id]
                            var cc = messages?[id];
                            _H += cc.Height;// высота сообщения
                            e.Graphics.DrawImage(messages?[id]._this_bitmap, this.Left, this.Height - _H);

                        }
                    }
                }
                else
                {

                }

            }
        }
        */
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                base.OnMouseWheel(e);
                //   if (e.Delta<0)
                if (chat != null)
                {
                    if (chat.statistic.LastObjId < ShownMessage_Bottom)
                    {
                        job.Chat_GetLastStatistic(chat);
                    }
                     //      if (ShownMessage_Bottom_Index - (e.Delta / 120) >= 0)
                    {
                        //                    Display_MessageIndex(ShownMessage_Bottom_Index - (e.Delta / 120), ShowMessagePosition.LastOnBootom);



                        //    ShownMessage_Bottom_Index = ShownMessage_Bottom_Index - (e.Delta / 120);


                        int getindex = chat.MessMessageArray.IndexOfKey(ShownMessage_Bottom);
                        if (getindex < 0)
                        {
                            // Сообщения не загружены
                            job.Chat_GetLastStatistic(chat);
                            job.Message_GetListIDsNow(chat);
                            getindex = chat.MessMessageArray.IndexOfKey(ShownMessage_Bottom);
                        }
                        bool moveDown = true;
                        if (e.Delta < 0)
                        {
                            chat.Refresh_Statistic(job);
                            moveDown = false;
                        }
                        getindex -= (e.Delta / 120);
                        if (chat.MessMessageArray.Count == 0)
                        {

                        }
                        if (getindex >= 0 && chat.MessMessageArray.Count > getindex)
                        {
                            int t = chat.MessMessageArray.ElementAt(getindex).Key;
                            Display_Message(t, ShowMessagePosition.LastOnBootom);
                            if (timer.Enabled)
                            {
                                if (moveDown)
                                    ShowBottomDeltaPositionAddons += 10;
                                else
                                    ShowBottomDeltaPositionAddons -= 10;
                            }
                            else
                            {
                                timer.Stop();
                                timer.Start();
                            }
                        }
                        Refresh();
                    }
                }
            }catch (Exception err)
            {

            }
        }


        internal void Chat_Select(ref Chat _chat)
        {
            if (_chat == null)
            {
                CloseChatOnPanel();
                messages.Clear();
                regions.Clear();
                if (chat != null)
                {
                    chat.OnChatEvent -= OnChatEvent;
                    chat = null;
                }
                Refresh();
                return;
            }
            if (chat != null && chat.ObjId.ObjId != _chat.ObjId.ObjId)
            {
                CloseChatOnPanel();
                messages.Clear();
                regions.Clear();

                chat.OnChatEvent -= OnChatEvent;
                chat = null;
            }
            chat = _chat;
            Text = chat.Text;
            chat.OnChatEvent += OnChatEvent;

            job?.Chat_GetLastStatistic(chat);

            if (chat?.MessMessageArray?.Count == 0)
            {
                try
                {
                   job?.Message_GetListIDsNow(chat);
                } catch (Exception err)
                {// СИТУАЦИЮ ОТЛОВИТЬ! Возникает пока не подписанный чат кликаешь точно. Не удалять брэйкпоинт пока не разберешся. !!!---!!!

                }

            }
            else
            {
    //            job?.Chat_GetLastStatistic(chat);//job.Chat_GetMyStatistic(chat.chatId);

            }

     //20190619 Двойной вызов       job.Chat_GetMyStatistic(chat.chatId);
            if (chat.statistic != null)
            {
                Display_Message(chat.statistic.LastShownObjId, ShowMessagePosition.LastOnBootom);
            }
            else
            {

            }
            Refresh();
            //if (chat.MessMessageArray)
        }

        private void OnChatEvent(Chat chat, ChatEventType et, object Tag)
        {
            switch (et)
            {
                
                case ChatEventType.onFilterChanged:
                    {/*Все сообщения, сообщения с пометкой на исполнение, файлы и вложения*/
                      break;
                    }
                case ChatEventType.onErrorEvent:
                    {
                break;
            }
                case ChatEventType.onUnknownEvent:
                    {
                        break;
                    }
                case ChatEventType.onChatStatisticChanged:
                    {
                        break;
                    }
                    
                case ChatEventType.onMessagesListUpdate:
                    {

                        Display_Message(chat.statistic.LastShownObjId, ShowMessagePosition.LastOnBootom);

                        break;
                    }
                case ChatEventType.onMessagesReciverUpdate:
                    {

                        //          Display_Message(chat.statistic.LastShownObjId, ShowMessagePosition.LastOnBootom);
                       /*Получили текс сообщения. Удаляем запрос на текст и обновляем картинку*/
                        foreach (var m in (Tag as WS_JobInfo.Obj []))
                        {
                       
                            if (m != null)
                            {
                                try
                                {
                                 Order_RequestMessageText.RemoveAll(s=>s ==(int)m.ObjId);
                                 MessageChatControl_RefreshImage(this.Size, (int)m.ObjId, m);
                                }
                                catch (Exception err)
                                {
                      //              E(err);
                                }

                            }
                        } /**/
                        Invalidate();
     //                   Refresh();//После загрузки реаьного сообщения, отрисовать.
                        break;
                    }

            }
        }

        private void Display_MessageIndex(int indexMessMessageArray)
        {
            int index = indexMessMessageArray;
         //     ShownMessage_Bottom = chat.MessMessageArray[indexMessMessageArray].IndexOfKey(lastShownObjId); ;
            if (index >= 0)
            {

                #region Предзагрузка сообщений
                int index_min = index - CountPreCashe < 0 ? 0 : index - CountPreCashe;
                int index_max = index + CountPreCashe >= chat.MessMessageArray.Count ? chat.MessMessageArray.Count - 1 : index + CountPreCashe;
                var t9 = chat.MessMessageArray.Keys[index_min];
                var t11 = chat.MessMessageArray.Keys[index_max];
                var msgs = chat.MessMessageArray.Where(s => s.Key >= t9 && s.Key <= t11).ToArray();

                //Если сообщениявыгруженного нет, делаем запрос
                var keys = msgs.Where(s => s.Value == null).ToArray();
                if (keys.Length > 0)
                {
                    if (keys.Length <= 100)
                    {

                        /*int min_req = keys.Min(s => s.Key);
                        int new_max = min_req + 20 > chat.MessMessageArray.Count ? 
                            chat.MessMessageArray.ElementAt(chat.MessMessageArray.Count - 1).Key : min_req + 20;
//                        int new_max = min_req + 20 > chat.MessMessageArray.Count ? chat.MessMessageArray.Count - 1
//                            chat.MessMessageArray.ElementAt(chat.MessMessageArray.Count - 1).Key : min_req + 20;
                        //int[] ddd = chat.MessMessageArray.Where(s => s.Key >= min_req && s.Key <= new_max).Select(s => s.Key).ToArray();
                        */
                        int[] new_req = GetNewMessageInRequest(keys.Select(s=>s.Key).ToArray());
                        
                        {
                            if (new_req.Length > 0)
                            {
                                if (new_req.Length<10)
                                {
                                    int n = new_req.Max();
                                    List<int> req = new List<int>();
                                    req.AddRange(new_req);
                                    if (n < ShownMessage_Bottom)
                                    {//перемещеаемся в начало списка
                                        new_req = GetNewMessageInRequest(chat.MessMessageArray.Select(s=>s.Key).Where(s => s<= n).OrderByDescending(s => s).Take(CountPreCashe).ToArray());
                                        req.AddRange(new_req);
                                    }
                                    else
                                    {//перемещеаемся в конец списка
                                        new_req = GetNewMessageInRequest(chat.MessMessageArray.Select(s => s.Key).Where(s => s >= n).OrderBy(s => s).Take(CountPreCashe).ToArray());
                                        req.AddRange(new_req);
                                    }

                                    job.Message_GetListArray(chat.chatId, req.ToArray());

                                }
                                else
                                    job.Message_GetListArray(chat.chatId, new_req);
                            }
                            
                        }
                    }
                    else
                    {
                        int[] ddd = keys.Select(s => s.Key).ToArray();
                        int[] new_req = GetNewMessageInRequest(ddd);
                        {
                            if (new_req.Length > 0)
                                job.Message_GetListArray(chat.chatId, new_req);
                        }

                //        job.Message_GetListArray(chat.chatId, new_req);
                    }
                } 
                #endregion
 //         if (messages != null)
                {
                    foreach (var m in msgs)
                    {
                        if (m.Value != null)
                        {
                            try
                            {
                                MessageChatControl_RefreshImage(this.Size, (int)m.Value.ObjId, m.Value);
                            }
                            catch (Exception err)
                            {
                            }

                        }
                    }

                }


            }
        }

        private int[] GetNewMessageInRequest(int[] ddd)
        {
            
                int [] ret = ddd.Except(Order_RequestMessageText).ToArray();
            if (ret.Length>0)
                Order_RequestMessageText.AddRange(ret);
            return ret;
        }

        private async void MessageChatControl_RefreshImage(Size size, int objId, WS_JobInfo.Obj value)
        {
            if (messages?.ContainsKey(objId) == false)
            {
                XMessageCtrl c = new XMessageCtrl();
                c.ImageListMsg = ImageListMsg;
                c.chatId =   this.chat.chatId;
                ;
                c.OnNeedRedraw += C_OnNeedRedraw;
                c.user = job.GetUser(value.userid);
                if (c.user.UserId == job.GetMyUserId())
                    c.Position = MessagePosition.xrMyMessage;

                c.ImageListMsg = ImageListMsg;

                c.SetWidth(size.Width);
                c.SetObj(job, value);
                c.RefreshPrepare();
          


                if (messages.ContainsKey(objId) == false)
                    messages.Add(objId, c);

                if (this.ClientRectangle.IntersectsWith(c.ClientRectangle))
                {
                    /*
                    if (showObject.Contains(c) == false)
                        showObject.Add(c);*/
                }

            }
        }
        private void MessageChatControl_RefreshImage(Size size, XMessageCtrl c)
        {
            c.SetWidth(size.Width);
            c.RefreshPrepare();
        }

        private void C_OnNeedRedraw(XMessageCtrl sender)
        {
            Refresh();
        }

        public void Show_Message(int chatid, int ShowMessage_ObjId)
        {
            if (chat==null)
            {

            }
            else
            if (chat.chatId==chatid)
            {
                job.Message_GetListIDsNow(chat);

                Display_Message(ShowMessage_ObjId, ShowMessagePosition.LastOnBootom);
            }
        }
        private void Display_Message(int lastShownObjId, ShowMessagePosition lastOnBootom)
        {
            if (lastShownObjId<0)
            {
                return;
            }
            LastMoveTo = lastOnBootom;
            if (lastOnBootom == ShowMessagePosition.LastOnBootom)
            {
                ShownMessage_Bottom = lastShownObjId;
            }
            if (lastOnBootom == ShowMessagePosition.LastInTop)
            {
                ShowMessage_Top = lastShownObjId;
            }

            int index = chat.MessMessageArray.IndexOfKey(lastShownObjId);
            if (index >= 0)
            {
                Display_MessageIndex(index);
                job.Message_Shown(chat.chatId, lastShownObjId);
            }
            else
            {//Нет указаного сообщения в списке
                job.Chat_GetLastStatistic(chat);
            }
        }



        /// <summary>
        /// Просит отобразить панель в состоянии ObjId верхнее сообщение на экране. 
        /// Если сообщение последнее или нет наполнения
        /// </summary>
        /// <param name="ObjId"></param>
        private void Display_MessageToTop(int ObjId , ShowMessagePosition position)
        {
            var index = chat?.MessMessageArray.IndexOfKey(ObjId);
            if(index.HasValue)
            {

            }
        }

        /// <summary>
        /// Крсиво схлопнуть панель сообщений
        /// </summary>
        private void CloseChatOnPanel()
        {
            
        }

        internal void SetJob(Job _job)
        {
            this.job = _job;
            job.OnMessage_GetFileAsync += Job_OnMessage_GetFileAsync;
        }

        private void Job_OnMessage_GetFileAsync(WS_JobInfo.asyncReturn_GetFile ret, int ObjId, byte[] data)
        {
            XMessageCtrl mc =  messages.Where(s => s.Key == ret.InParam_ObjId).FirstOrDefault().Value;
            if (mc != null)
            {
                if (mc.MessageObj.ObjId == ret.InParam_ObjId)
                {
                    Image img = byteArrayToImage(data);
                    if (ret.TypeRequest == 0)
                        if (!mc.files.Where(s => s.Item3 == ret.Guid && s.Item2 != null).Any())
                            mc.files.Add(new Tuple<Image, Image, string>(null, img, ret.Guid));
                        else
                        {

                        }
                    if (ret.TypeRequest == 1)
                        if (!mc.files.Where(s => s.Item3 == ret.Guid && s.Item1 != null).Any())
                            mc.files.Add(new Tuple<Image, Image, string>(img, null, ret.Guid));
                        else
                        {

                        }
                    mc.RefreshPrepare();
                    mc.Refresh();
                }
            }
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null && byteArrayIn.Length > 0)
            {
                try
                {
                    Image returnImage;
                    MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                    ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                    returnImage = Image.FromStream(ms, true);//Exception occurs here
                    return returnImage;
                }
                catch { }
            }

            return null;
        }
        internal void On_Job_Disconneced(Job _job, Exception err)
        {
        //    this.BeginInvoke(Chat_Select,null);
            
        }

        internal void On_Job_TokenRecive(Job _job)
        {
           
        }

        internal void On_Job_Conneced(Job _job)
        {
            
        }

        internal void On_Job_ChatUpdateRecive(Job _job, string cmd, long chatid, long msgid)
        {
            try
            {
                if (cmd != "CHATLEAVE")
                    job?.Message_GetListIDsNow((int)chatid);
                else
                {
               //     this.BeginInvoke(Chat_Select, null);
                 //   Chat_Select(null);
                }
            }catch (Exception err)
            {

            }
        }


        internal XDraw_VisualData1 GetEditMessage()
        {
            return vd;
        }

        XDraw_VisualData1 vd;
        internal void SetMode(XMessageCtrl mc, XChatMessagePanelMode _mode)
        {
            vd = new XDraw_VisualData1();
            
            Mode = _mode; // XChatMessagePanelMode.ShowMesages
            if (Mode == XChatMessagePanelMode.EditOneMessage)
            {
                Bitmap bitmap_beforeEdit = new Bitmap(Width, Height);
                using (var g = Graphics.FromImage(bitmap_beforeEdit))
                {
                   DrawFrame(g);
                   //bitmap_beforeEdit.Save("c:\\temp\\chat\\bef.bmp");
                   vd.bitmap_beforeEdit = bitmap_beforeEdit;
                    vd.AlphaStart = 70;
                    vd.AlphaStop = 15;
                    vd.Delta= -1;
                    vd.Value = 70;
                    timer_ModeShowList2EditMessage.Enabled = true;
                    vd.mc = mc;

                    vd.newmsg = new XMessageCtrl();
                    
                    vd.newmsg.SetCurrntcChat(mc.chatId);
                    vd.newmsg.AddParentMsg(mc);



                    //           XMessageCtrl mc = context_msg.SourceControl as XMessageCtrl;
                    //mc.SetCurrntcChat(GetCurrentChatId());
                    //string data = mc.MessageObj.GetText();
                    //Clipboard.SetText(            data            );/**/
/*
                    newmsg.AddText(textBox1.Text);// "Ответ на соообщение"
              //      mp.SetMode(mc, XChatMessagePanelMode.EditOneMessage);
                    job.Message_Replay(newmsg);
                    */

                }
                
            }

        }

        private void onTimer_ModeShowList2EditMessage(object sender, EventArgs e)
        {
            Refresh();
            //Bitmap bitmap_beforeEdit;
        }

    }
}
