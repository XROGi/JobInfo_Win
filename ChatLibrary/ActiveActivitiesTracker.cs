using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ActiveActivitiesTracker
    {
        private static int sActiveActivities = 0; public static void activityStarted()
        {
            if (sActiveActivities == 0)
            { // TODO: Here is presumably "application level" resume 
            }
            sActiveActivities++;
        }
        public static void activityStopped()
        {
            sActiveActivities--; if (sActiveActivities == 0)
            { // TODO: Here is presumably "application level" pause 
            }
        }
    }
    
}
