using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo.XROGi_Class
{

    public class RoundButton : Button
    {
      


    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            try
            {

                base.OnPaint(e);
                GraphicsPath grPath = new GraphicsPath();
                grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
                this.Region = new System.Drawing.Region(grPath);
             //   e.Graphics.FillPath(  ( this.BackColor, grPath);
          //   
            }
            catch (Exception err)
            {


            }
        }

         
    }
   
}
