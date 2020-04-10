namespace Ji.Models
{
    public class XParam
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public XParam(string ss)
        {
            string[] s = ss.Split('\t'); ; ;
            ParamName = s[0];
            ParamValue = s[1];
        }
    }
}