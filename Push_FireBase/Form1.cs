using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Push_FireBase
{
    public partial class Form1 : Form
    {
        JiDataClassesDataContext d ;
        public Form1()
        {
            InitializeComponent();
        }
        int Test1 = 0;
        private async void  button1_Click(object sender, EventArgs e)
        {
            foreach (string key in textBox1.Text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                if (string.IsNullOrEmpty(key)) continue;
                Test1++;
                
                    string caption = "Тест"+Test1.ToString();
                if (Test1 > 10) Test1 = 0;
                string text = DateTime.Now.ToLongTimeString();
                string ret = await NotifyAsync(key, caption, text);
                if (ret== "Key NotRegistered")
                {

                }
                AddLog(ret);
            }
        }

        private void AddLog(string ret)
        {
            listBox2.Items.Add($"[{DateTime.Now.ToLongTimeString()}]\t{ret}");
        }

        //    https://stackoverflow.com/questions/37412963/send-push-to-android-by-c-sharp-using-fcm-firebase-cloud-messaging
        public static async Task<string> NotifyAsync(string to, string title, string body)
        {
            try
            {
            
                //if (FirebaseAuth.DefaultInstance == null)
                //{
                //    FirebaseApp.Create(new AppOptions()
                //    {
                //        Credential = GoogleCredential.FromFile(@"C:\XROGi\FB\SvodInf\svodinf-firebase-adminsdk-atlfn-4c0bbf542a.json"),
                //    });
                //}
              
                //   FirebaseApp.Create("https://svodinf.firebaseio.com");
                //FirebaseApp.Create(new AppOptions()
                //{
                //    Credential = GoogleCredential.GetApplicationDefault(),
                //    ServiceAccountId = "my-client-id@my-project-id.iam.gserviceaccount.com",
                //});


       //         var decoded = await FirebaseAuth.DefaultInstance.ListUsersAsync()// .VerifyIdTokenAsync(to);

       //         var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(to);
     //           var uid = decoded.Uid;
     /*             */

                string jsonBody = "";

                FB_Setup fbsetup = new FB_Setup();

                /*Ji*/
/*                fbsetup.PostURL = "https://fcm.googleapis.com/fcm/send";
                fbsetup.PostContent = "application/json";
                fbsetup.ServerKeyHeader = "AAAAOziWWGc:APA91bHbSFqG7mzH_b2intcVElI8ljuytY5vvfqlOp7cr6CT0_xOvwYOVtEmDYy-RrJpbQiOmZP3iw-ogEPPzXeCmsc-n7-2mZZXG2kf1ATDmczjLSa66GELgOGsVPHcGVsTZvba49m5";
                fbsetup.SenderId = "254352447591";
                fbsetup.Priority = "high";
                fbsetup.Sound = "default";
                */
                /*SvodInf*/
                fbsetup.PostURL = "https://fcm.googleapis.com/fcm/send";
                fbsetup.PostContent = "application/json";
                fbsetup.ServerKey = "AAAAp1WNZKE:APA91bEOsZ-1QcV4qTUifEd7lBecKcw9i_xE6kUO93y-GAiKZNln2LFHHNeC3TNaYNi2PyA2bWhBVt7K4ecQNeaO0x6EZZDKg0zYtVKWt0A5EnaR8iF2UYIevREBLwZkbhPi4bIzqp6i";
                fbsetup.SenderId = "718694868129";
                fbsetup.Priority = "high";
                fbsetup.Sound = "default";


                bool res = await isValidKey(to, fbsetup);

                jsonBody = fbsetup.GetJson(to, title, body);

                /*
                //cloudmessage page see!!!!!!!!!!!!
                var serverKey = string.Format("key={0}", fbsetup.ServerKey);
                var senderId = string.Format("id={0}", fbsetup.SenderId);

                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body, sound = fbsetup.Sound }
                   ,
                    priority = fbsetup.Priority 
                   ,
                    content_available = true
                };

                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);
                */
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, fbsetup.PostURL))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", fbsetup.ServerKeyHeader );
                    httpRequest.Headers.TryAddWithoutValidation("Sender", fbsetup.SenderIdHeader);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, fbsetup.PostContent);


                    //HttpClientHandler handler = new HttpClientHandler();
                    //handler.UseDefaultCredentials = true;// .DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    /*   HttpClientHandler handler = new HttpClientHandler()
                       {
                           Proxy = new WebProxy("172.17.10.52:10000")
                           ,Credentials = new NetworkCredential("iu.login", "pass*", "ghp.lc"),
                       };handler*/
                    using (var httpClient = new HttpClient())
                    {

                        var result = await httpClient.SendAsync(httpRequest);
                        string rep = await result.Content.ReadAsStringAsync();
                        dynamic stuff =  JObject.Parse(rep);
                      
                        if (result.IsSuccessStatusCode)
                        {
                            if (stuff.success == "1")
                            {
                                return "OK";
                            }
                            else
                            {
                                string error = stuff.results.ToString(); 
                                if (error.Contains("NotRegistered"))
                                {
                                    UnregisterKey(to);
                                    return "Key NotRegistered";
                                }
                            }
                            
                        }
                        else
                        {
                            return $"Error sending notification.Status Code: { result.StatusCode}";
                            // Use result.StatusCode to handle failure
                            // Your custom error handler here
                            //         _logger.LogError($"Error sending notification. Status Code: {result.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception thrown in Notify Service: {ex}";
                //       _logger.LogError($"Exception thrown in Notify Service: {ex}");
            }

            return "";
        }

        private static void UnregisterKey(string to)
        {
           // AddLog("Невалидный ключ =>\t" + to ) ;
        }

        private async static Task<bool> isValidKey(string to, FB_Setup fbsetup  )
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, fbsetup.PostURL))
            {
                httpRequest.Headers.TryAddWithoutValidation("Authorization", fbsetup.ServerKeyHeader);
                httpRequest.Headers.TryAddWithoutValidation("Sender", fbsetup.SenderIdHeader);
                httpRequest.Content = new StringContent("{\"registration_ids\":[\""+to+"\"]}", Encoding.UTF8, fbsetup.PostContent);


                //HttpClientHandler handler = new HttpClientHandler();
                //handler.UseDefaultCredentials = true;// .DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                /*   HttpClientHandler handler = new HttpClientHandler()
                   {
                       Proxy = new WebProxy("172.17.10.52:10000")
                       ,Credentials = new NetworkCredential("iu.login", "pass*", "ghp.lc"),
                   };handler*/
                using (var httpClient = new HttpClient())
                {

                    var result = await httpClient.SendAsync(httpRequest);
                    string rep = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                        // Use result.StatusCode to handle failure
                        // Your custom error handler here
                        //         _logger.LogError($"Error sending notification. Status Code: {result.StatusCode}");
                    }
                }
                return false;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            SendNow();

        }

        int LastSendId = 0;
        private async  void SendNow()
        {
            if (d == null)
                d = new JiDataClassesDataContext();
            foreach (tbl_FB_SendLog send in d.tbl_FB_SendLog.Where(s => s.FB_SendLogDateSend == null && s.FB_SendLogId > LastSendId ).ToArray())
            {
                try
                {
                    LastSendId = send.FB_SendLogId;
                    string msg;
                    try
                    {
                        XmlDocument dd = new XmlDocument();
                        dd.LoadXml(send.Body);
                        msg =dd.DocumentElement.InnerText;
                    }
                    catch (Exception err)
                    {
                        msg = "текст смотреть в программе";
                    }
                    string ret = await NotifyAsync(send.key, send.Title, msg);
                    if (ret == "OK")
                    {
                        d.proc_FB_SendCommited(send.FB_SendLogId);
                    }
                    if (ret == "Key NotRegistered")
                    {
                        d.proc_User_ParamFBClear(send.userid, send.key);
                    }
                    AddLog(ret);
                }catch (Exception err)
                {
                    AddLog("SendNow error => " + err.Message.ToString()); ;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                     SendNow();
        }

        private async void timer2_Tick(object sender, EventArgs e)
        {
            foreach (string key in textBox1.Text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(key)) continue;
                Test1++;

                string caption = "Тест" + Test1.ToString();
                if (Test1 > 10) Test1 = 0;
                string text = DateTime.Now.ToLongTimeString();
                string ret = await NotifyAsync(key, caption, text);

                AddLog(ret);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            timer2.Enabled = checkBox2.Checked;
        }
    }
}
