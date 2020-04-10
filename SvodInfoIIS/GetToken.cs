using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SvodInfoIIS 
{
    public class GetToken:Hub
    {

        public void UserOnline(int UserId)
        {
            object[] data = new object[] { UserId };
            //Clients.All.SendCoreAsync("UserOnline", data);
            Clients.Others.SendCoreAsync("UserOnline", data);
        }
        //public void UserOnline(int UserId)
        //{
        //    object[] data = new object[] { UserId };
        //    Clients.All.SendCoreAsync("UserOnline", data);
        //}
    }
   // https://localhost:44369/weatherforecast
}
