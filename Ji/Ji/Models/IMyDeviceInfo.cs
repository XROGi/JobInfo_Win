using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.Models
{
    public interface IMyApplicationInfo
    {
        //pid= Process.MyPid().ToString(), clientname = Android.App.Application.Context.PackageName
         string Get_MyPid();
         string Get_clientname();
         string Get_clientvers();
         void closeApplication();
         string Get_AndroidMAC();
    }
}
