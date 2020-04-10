using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo
{
    public class Test_Class
    {
        public string Name;
        public string Text;

        public Test_Class()
        {
            Name = "Test";
            Text = DateTime.Now.ToString();

        }

        public Test_Class(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}
