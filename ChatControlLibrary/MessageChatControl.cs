using JobInfo.XROGi_Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatControlLibrary
{
    public class MessageChatControl : Panel
    {
        public bool b_ShowUserImage = true;
        public string Text;

        public int Data_Width;
        public int Data_Height;
        /*public void Resize(WS_JobInfo.Obj o , int Width)
        {
            Data_Width = 0;
            Data_Height = 0;
        }*/

        protected override void OnPaint(PaintEventArgs e)
        {
            int FotoBox = 50;
            int Msg_Height = 70;
            int Info_Height = 25;
            Msg_Height = Height - Info_Height;
            base.OnPaint(e);
            Rectangle r = new Rectangle(0, 0, Width, Height);
            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, Width, Height);

            e.Graphics.DrawRoundedRectangle(Color.Gray, 0, 0, Width, Height,10);

            int Msg_X_Start = FotoBox;
            Rectangle r_msg = new Rectangle(Msg_X_Start, 0, Width - FotoBox, Msg_Height);
            if (Width >= 100)
            {
                b_ShowUserImage = true;
                Msg_X_Start = FotoBox;
                Rectangle r_image = new Rectangle(0, 0, FotoBox, FotoBox); ;

                r_image.Inflate(-5, -5);

                e.Graphics.DrawRectangle(Pens.Gray, r_image);
                
                
            }
            else
            {
                b_ShowUserImage = false;
                r_msg = new Rectangle(0, 0, Width+ FotoBox, Msg_Height);
            }
            
            //Rectangle r_msg = new Rectangle(Msg_X_Start, 0, Width - FotoBox, Msg_Height); ;
            r_msg.Inflate(-3, -2);
            e.Graphics.DrawRectangle(Pens.Gray, r_msg);
            Font Font_MessageText = new Font("Arial", 10);
            e.Graphics.DrawString(Text, Font_MessageText, new SolidBrush(Color.Black), r_msg);  

            Rectangle r_msginfo = new Rectangle(0, Msg_Height, Width, Info_Height); ;
            r_msginfo.Inflate(-3, -2);
            e.Graphics.DrawRectangle(Pens.Gray, r_msginfo);

            Rectangle r_msgtime = new Rectangle(Width-50, Msg_Height, 50, Info_Height); ;
            r_msgtime.Inflate(-7, -4);
            e.Graphics.DrawRectangle(Pens.Gray, r_msgtime);
        }
    }
}
