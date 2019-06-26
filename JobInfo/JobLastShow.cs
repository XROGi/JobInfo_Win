using System;

namespace JobInfo
{
    internal class JobLastShow
    {
        private int LastStopPosition;
        private int LastShowPosition;

        public JobLastShow(int _LastStopPosition, int _LastShowPosition)
        {
            this.LastStopPosition = _LastStopPosition;
            this.LastShowPosition = _LastShowPosition;
        }

        internal int Get_LastStopPosition()
        {
            return LastStopPosition;
        }
    }
}