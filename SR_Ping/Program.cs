using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
namespace SR_Ping
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(RunMain).Wait();
            
        }
        private async static void RunMain()
        {
            //await StartConnectionAsync();

        }
        //private static Task StartConnectionAsync()
        //{
        //     //_c = new HubConnectionBuilder()
        //     //   .w
        //}
    }
}
