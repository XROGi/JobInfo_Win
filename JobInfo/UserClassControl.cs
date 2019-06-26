using JobInfo.XROGi_Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo
{
    
    public class UserClassControl : Panel
    {
        public WS_JobInfo.User user;
        public int FotoSize = 20;
        int FotoBox = 42;
        public bool b_ShowUserImage = true;
        public string Text;

        private int Data_Width_Source;
        public int Data_Width;
        public int Data_Height;
        public bool b_selected;

        // Declare a delegate
        public delegate void ValueChangedEventHandler(object sender, int Value);

        [Category("Action")]
        [Description("Fires when the value is changed")]
        public event ValueChangedEventHandler ValueChanged;


        public UserClassControl ( )
        {
            this.Width = FotoBox;
            this.Height = FotoBox;
            b_selected = false;
            // Raise the event
            ValueChanged?.Invoke(this, 0);

        }
        
        public void Calculate_Size(WS_JobInfo.Obj o, int Width)
        {

            //      if (Data_Width_Source == Width) return;

            Data_Width = 0;
            Data_Height = 0;
            Size s = o.MeasureDrawObj(Width - FotoBox, 10000);
            Data_Height = s.Height;
            Data_Width = s.Width;

            if (Data_Width <= 80)
                Data_Width = 80;
            else
                Data_Width = s.Width;
            Data_Width = Data_Width + 25 + 5;

            if (Data_Height < 95)
                Data_Height = 25 + 70;

            Data_Width_Source = Width;
        }
        /*
        private Bitmap User_Foto2_;
        [Category("Flash"),
         Description("The ending color of the bar.")
        ]
        // The public property EndColor accesses endColor.  
        public Bitmap User_Foto2
        {
            get
            {
                return User_Foto2_;
            }
            set
            {
                User_Foto2_ = value;
              
                // The Invalidate method calls the OnPaint method, which redraws   
                // the control.  
                Invalidate();
            }
        }
*/
        private Image User_Foto_;
        [Category("Flash"),
         Description("The ending color of the bar.")
        ]
        // The public property EndColor accesses endColor.  
        public Image User_Foto
        {
            get
            {
                return User_Foto_;
            }
            set
            {
                User_Foto_ = value;
                /* if (baseBackground != null && showGradient)
                 {
                     baseBackground.Dispose();
                     baseBackground = null;
                 }*/
                // The Invalidate method calls the OnPaint method, which redraws   
                // the control.  
                Invalidate();
            }
        }



        private Color endColor = Color.LimeGreen;
        protected override void OnPaint(PaintEventArgs e)
        {

            int Msg_Height = 70;
            int Info_Height = 25;
            Msg_Height = Height - Info_Height;
            base.OnPaint(e);
            Rectangle r = new Rectangle(0, 0, Width, Height);
       //     e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, Width, Height);

            int Msg_X_Start = FotoBox;
       //     Rectangle r_msg = new Rectangle(Msg_X_Start, 0, Width - FotoBox, Msg_Height);
            
                b_ShowUserImage = true;
                Msg_X_Start = FotoBox;
                Rectangle r_image = new Rectangle(0, 0, FotoBox, FotoBox); ;

                r_image.Inflate(-5, -5);

       //         e.Graphics.DrawRectangle(Pens.Gray, r_image);
       //     e.Graphics.DrawLine(Pens.Red, 0, 0, Width, Height);
      //      e.Graphics.DrawLine(Pens.Red, 0, Height, Width, 0);
            e.Graphics.DrawCircle(Pens.Blue, FotoSize, FotoSize, FotoSize);
            /*           if (Width >= 100)
                                    { }
                                    else
                                    {
                                        b_ShowUserImage = false;
                                        r_msg = new Rectangle(0, 0, Width + FotoBox, Msg_Height);
                                    }

                                    //Rectangle r_msg = new Rectangle(Msg_X_Start, 0, Width - FotoBox, Msg_Height); ;
                                    r_msg.Inflate(-3, -2);
                                    e.Graphics.DrawRectangle(Pens.Gray, r_msg);
                                    e.Graphics.DrawString(Text, new Font("Arial", 10), new SolidBrush(Color.Black), r_msg);

                                    Rectangle r_msginfo = new Rectangle(0, Msg_Height, Width, Info_Height); ;
                                    r_msginfo.Inflate(-3, -2);
                                    e.Graphics.DrawRectangle(Pens.Gray, r_msginfo);

                                    Rectangle r_msgtime = new Rectangle(Width - 50, Msg_Height, 50, Info_Height); ;
                                    r_msgtime.Inflate(-7, -4);
                                    e.Graphics.DrawRectangle(Pens.Gray, r_msgtime);*/
            if (User_Foto_ != null) {
                e.Graphics.DrawImageUnscaledAndClipped(User_Foto_, r);

                GraphicsPath g = new GraphicsPath();
                g.AddEllipse(0, 0, 40, 40);
                this.Region = new System.Drawing.Region(g);

            }
            if (b_selected)
            {
                Rectangle r_selection = new Rectangle(0, FotoBox-8, FotoBox, 10); ;
                e.Graphics.FillRectangle(new SolidBrush(Color.Green), r_selection);
            }
            base.OnPaint(e);
        }






        [Category("Flash"),
         Description("The ending color of the bar.")
        ]
        // The public property EndColor accesses endColor.  
        public Color EndColor
        {
            get
            {
                return endColor;
            }
            set
            {
                endColor = value;
                /* if (baseBackground != null && showGradient)
                 {
                     baseBackground.Dispose();
                     baseBackground = null;
                 }*/
                // The Invalidate method calls the OnPaint method, which redraws   
                // the control.  
                Invalidate();
            }
        }
    }
}
