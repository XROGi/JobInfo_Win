using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ji.Views
{

    public class SvodInfMasterDetailPageMasterMenuItem
    {
        public SvodInfMasterDetailPageMasterMenuItem()
        {
            TargetType = typeof(SvodInfMasterDetailPageMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}