using JobInfo;
using JobInfo.XROGi_Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JI_WPF
{
    public  class ChatDataStore
    {
        public User user = null;
        public List<ChatControl> chats = new List<ChatControl>();
        public ObservableCollection<XUserMy> users=  new ObservableCollection<XUserMy>();

        public int Selectet_ConnectInterfaceId;
        public List<ConnectInterface> connections = new List<ConnectInterface>();

        public delegate void OnUser_UpdateDelegate(ChatDataStore sender);
        public event OnUser_UpdateDelegate OnUser_Update;//= delegate { };

        Job job;
        internal ConnectInterface GetFirstConnectInterface()
        {
            if (Selectet_ConnectInterfaceId < connections.Count)
            {
              return  connections.Where(s=>s.Id==Selectet_ConnectInterfaceId).FirstOrDefault();
            }
            return null;
        }

        internal void ConnectBegin()
        {
            ConnectToServer(GetFirstConnectInterface());
        }

        private void ConnectToServer(ConnectInterface connectInterface)
        {
            Setup.MachineName = Environment.MachineName;
            Setup.UserLogin = Environment.UserName;

            string t = "";
              job = new Job(connectInterface.Server_SOAP);

            //       jobsArxive.Add(job);
            job.OnConneced += Job_OnConneced; ;
            job.OnDisconneced += Job_OnDisconneced; ;
            job.OnTokenRecive += Job_OnTokenRecive; ;
            job.OnMsgRecive += Job_OnMsgRecive; ;
            job.OnChatListChanged += Job_OnChatListChanged; ;

            job.OnJobClassEvent += Job_OnJobClassEvent; ;
            job.OnChatUpdateRecive += Job_OnChatUpdateRecive; ;
            job.OnSocketSend += Job_OnSocketSend; ;
            job.OnBackWorkBegin += Job_OnBackWorkBegin; ;
            //            mp.SetJob(job);
            job.OnUser_GetListAllAsync += Job_OnUser_GetListAllAsync; ;
            job.OnPingPong += Job_OnPingPong; ;

            //            job.OnConneced += mp.On_Job_Conneced;
            //            job.OnTokenRecive += mp.On_Job_TokenRecive;
            //            job.OnDisconneced += mp.On_Job_Disconneced;
            //            job.OnChatUpdateRecive += mp.On_Job_ChatUpdateRecive;
            //            job.OnChatEvent += ForTree_OnChatEvent;
            //            job.onMessage_GetListID += mp.onMessage_GetListIDMethod;
            //            job.Message_ReciveListObjId += mp.Message_ReciveListObjIdMethod;
            //            job.OnPingPong += OnJob_PingPong;

            job.ConnectToServer(Setup.MachineName, Setup.UserLogin);
            /*            string returl = job.GetServerName();
            if (returl != prif && AntiRecurse != 1)
            {
                Setup.URLServer = returl;
                try
                {
                    AntiRecurse = 1;
                    Job_Run(Setup.URLServer);
                }
                catch (Exception err)
                {

                }
            }
            AntiRecurse = 0;*/
        }

        private void Job_OnPingPong(Job job, DateTime Ping, DateTime Pong)
        {
        }

        private void Job_OnUser_GetListAllAsync(object sender, JobInfo.WS_JobInfo.User_GetListAllCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result != null)
                {
                  foreach(XUserMy u in    e.Result.Select(s => new XUserMy() { Name = s.UserName , Description = s.positions.FirstOrDefault()?.Position , OU =  s.positions.FirstOrDefault()?.Subdiv }))
                    {
                        users.Add(u);
                    }
                }
            }
            if (OnUser_Update!=null)
                    OnUser_Update(this);
            
        }

        private void Job_OnBackWorkBegin(string Message)
        {
        }

        private void Job_OnSocketSend(Exception err, string data)
        {
        }

        private void Job_OnChatUpdateRecive(Job _job, string cmd, long chatid, long msgid)
        {
        }

        private void Job_OnJobClassEvent(Job _job, string FunctionName)
        {
        }

        private void Job_OnChatListChanged(Job _job)
        {
        }

        private void Job_OnMsgRecive(Job _job, string cmd, long chatid, long msgid)
        {
        }

        private void Job_OnTokenRecive(Job _job)
        {
            try
            {
         //       ContentMenu_Devices(job);
                if (_job != null && _job.this_device != null)
                    I("Recive Token " + _job.this_device.TokenId.ToString());
             //   if (debug)
                {
                    //Text = _job.this_device.TokenId.ToString();
                    //tokenToolStripMenuItem.Text = _job.this_device.TokenId.ToString();
                    //serverConnectedToolStripMenuItem.Text = Setup.URLServer;
                    //mashineNameToolStripMenuItem.Text = Setup.MachineName;
                    //userLoginToolStripMenuItem.Text = Setup.UserLogin;
                }

                try
                {
                    job.UserGetSelf();
                }
                catch (Exception err)
                {

                }


                if (_job.this_device.TokenId != -1)
                {
             //       ShowInfoBallon("Новый статус", "Подключен к серверу");
                    I("Подключен к серверу");
                    //    if (up.users == null)
                      job.GetUsers();
                    
              
                    //int selectedchat = job.Chat_GetSelected();
                    //FindAndSelectInTreeChatById(selectedchat);
               //     job.Spravka_GetStatus();
                    //         job.GetUsers();
                }

                //       throw new NotImplementedException();
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); }
            catch (Exception err) { E(err); }
        }

        private void Job_OnDisconneced(Job _job, Exception err)
        {
        }

        private void Job_OnConneced(Job _job)
        {
            try
            {
                I("Подключен к серверу.Идентификация...");
                job.ConnectToServer(Setup.MachineName, Setup.UserLogin);
              
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err);  }
            catch (Exception err) { E(err); }
        }

        private void E(ChatDisconnectedException err)
        {
           
        }

        private void E(ChatWsFunctionException err)
        {
           
        }
        private void E(Exception err)
        {

        }

        private void I(string v)
        {
          
        }
    }
}
