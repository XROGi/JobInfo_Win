﻿
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Microsoft.AspNet.SignalR.Client.HubConnection connection;

        public Form1()
        {
            InitializeComponent();
            //connection = new HubConnectionBuilder()
            //    .WithUrl("http://localhost:53353/ChatHub")
            //    .Build();

            //connection.Closed += async (error) =>
            //{
            //    await Task.Delay(new Random().Next(0, 5) * 1000);
            //    await connection.StartAsync();
            //};
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
