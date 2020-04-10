using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.ClassSR
{
    public partial class ViewGetUserParameters
    {
        public int ParamId { get; set; }
        public int? UserId { get; set; }
        public int? ObjId { get; set; }
        public int PeriodId { get; set; }
        public int SgParamClass { get; set; }
        public int SgParamVid { get; set; }
        public string ParamValue { get; set; }
        public int SgValueType { get; set; }
    }
}
