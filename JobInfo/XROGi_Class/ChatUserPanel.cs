using JobInfo.XROGi_Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo.XROGi_Class
{
    public class ChatUserPanel :Control
    {
        public UserLive live;
        public WS_JobInfo.User u;
        Image User_Foto;
        public int UserId;
        

        public Chat personal_chatid;

        public bool b_FotoSkipped = false;
        public bool Selected { get; internal set; }

        public delegate void onUser_NeedFotoDelegate(ChatUserPanel sender);
        public event onUser_NeedFotoDelegate onUser_NeedFoto = delegate { };

        public delegate void onMessages_GetCountNotReadDelegate(ChatUserPanel sender);
        public event onMessages_GetCountNotReadDelegate OnMessages_GetCountNotRead = delegate { };


        public ChatUserPanel(WS_JobInfo.User _u)
        {
            u = _u;
            UserId = u.UserId;
        }

        public WS_JobInfo.User GetUser()
        {
            return u ;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                Graphics g = e.Graphics;
                PaintUserV1(g,false);
            }
            catch (Exception err)
            {

            }
        }
        internal void PaintUserV1(Graphics g, bool b_Online, int CountNewMsg = 0)
        {
            try
            {
                if (Selected)
                    g.DrawRoundedRectangleFill(Color.LightYellow, new Rectangle(Left + 5, Top + 5, Width - 5, Height - 5), 10);
                //else
                g.DrawRoundedRectangleFill(Color.White, new Rectangle(Left + 5, Top + 5, 64, 64), 10);
                if (User_Foto == null)
                {
                    Image i = u.GetFoto();
                    if (i == null)
                    {
                        onUser_NeedFoto(this);
                        i = u.GetFoto();
                    }
                    if (i != null)
                    {
                        Image User_Foto = ScaleImage(u.GetFoto(), 56, 56);
                        if (User_Foto != null)
                        {
                            this.Region = new System.Drawing.Region(g.PathRoundedRectangleFill(Left + 5, Top + 5, 64, 64, 10));
                            g.DrawImage(User_Foto, Left + 12, Top + 10);
                        }
                    }
                    if (b_Online)
                    {
                        g.FillCircle(Brushes.Green, Left + 7, Top + 7, 5);
                    }

                }


                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 8);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                g.DrawString(u.UserName, drawFont, drawBrush, Left + 75, Top + 5, new StringFormat(StringFormatFlags.NoWrap));
                string d = u.positions.FirstOrDefault()?.Position;
                if (d != null)
                    g.DrawString(d, drawFont, drawBrush, Left + 75, Top + 15, new StringFormat(StringFormatFlags.NoWrap));

                g.DrawRoundedRectangle(Color.Gray, new Rectangle(Left + 5, Top + 5, 64, 64), 10);

                OnMessages_GetCountNotRead(this);
       //         live.c
                if (live.statistic != null)
                {

                    if (live.statistic.CountNew > 0)
                    {
                        string ggg = live.statistic.CountNew.ToString();// DateTime.Now.Second.ToString();



                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(ggg, new Font("Arial", 8));
                        g.DrawRoundedRectangleFill(Color.Gray, new Rectangle(Left + 55, Top + 55, Convert.ToInt32(stringSize.Width) + 5, 20), 4);
                        //          g.DrawRoundedRectangle(Color.White, new Rectangle(Left + 55, Top + 55, Convert.ToInt32(stringSize.Width) + 5, 20), 4);
                        g.DrawString(ggg , new Font("Arial", 8), new SolidBrush(Color.White), Left + 58, Top + 58);
                    }
                }
            }
            catch (Exception err
           )
            {

            }
        }
            internal void PaintUserV1(Graphics g, bool b_Online)
        {
            PaintUserV1(g, b_Online, 0);
            
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            if (image != null)
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
            return null;
        }
        
    }
}
