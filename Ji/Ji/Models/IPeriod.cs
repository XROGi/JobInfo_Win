using System;

namespace Ji.Models
{
    public interface IPeriod
    {
        
            DateTime dtb { get; set; }
            DateTime dte { get; set; }
            DateTime dtc { get; set; }
            DateTime dtd { get; set; }
         
        /*     public IPeriod()
             {
                 dtc = DateTime.Now;
             }
      /*   */
             int PeriodId { get; set; }


        bool IsFinish();
        bool FinishNow();

      
    }
}
