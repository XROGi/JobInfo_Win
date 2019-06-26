using System.Drawing;

namespace JobInfo.XROGi_Class
{
    internal class XDraw_VisualData1
    {
        internal XMessageCtrl mc;
        internal XMessageCtrl newmsg;

        public Bitmap bitmap_beforeEdit { get; internal set; }
        public int AlphaStart { get; internal set; }
        public int AlphaStop { get; internal set; }
        public int Delta { get; internal set; }
        public float Value { get; internal set; }
    }
}