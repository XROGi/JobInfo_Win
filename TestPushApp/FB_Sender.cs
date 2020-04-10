using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication4.Classes
{
    public static class FB_Sender
    {
        private static readonly ReaderWriterLockSlim FB_list_Lock = new ReaderWriterLockSlim();
        public static List<FB_user_Send> FB_list = new List<FB_user_Send>();
        public static void FB_list_Add(int userid, string keystr)
        {
            FB_list_Lock.EnterWriteLock();
            try
            { // Удалять нельзя, вдруг несколько телефонов
                //FB_list.RemoveAll(s=>s.userid==)
                FB_list.Add(new FB_user_Send() { userid = userid, Token = keystr });
            }
            finally { FB_list_Lock.ExitWriteLock(); };

        }

        public static void FB_list_Set(FB_user_Send[] arr)
        {
            FB_list_Lock.EnterWriteLock();
            try
            { // Удалять нельзя, вдруг несколько телефонов
                //FB_list.RemoveAll(s=>s.userid==)
                FB_list.Clear();
                FB_list.AddRange(arr);
            }
            finally { FB_list_Lock.ExitWriteLock(); };

        }
       
        public static async Task<string> Send(int userid_WhoSend, int chatid, string MessageCaption, string MessageText)
        {
            string ret="";
            using (JobInfoDataClassesDataContext d = DB_isConnected())
            {
                try
                {
                    if (FB_list.Count == 0)
                        FB_list_Set(d.view_User_ParamFBSet.Select(s => new FB_user_Send() { userid = s.userid.Value, Token = s.ParamValue }).ToArray());
                }
                catch (Exception err)
                {
                    ret+= "Send#1 " + err.Message.ToString()+Environment.NewLine;
                };
                try
                {
                    FB_user_Send[] SendJob =
                d.view_ChatList.Where(s => s.ObjId == chatid
                //                                           && s.UserId != userid_WhoSend
                ).Select(s => new FB_user_Send { ChatId = s.ObjId, Text = MessageText, Caption = MessageCaption + " " + s.ChatName, userid = s.UserId }).ToArray();
                    foreach (FB_user_Send send in SendJob)
                    {
                        try
                        {
                            foreach (string token in FB_list.Where(s => s.userid == send.userid).Select(s => s.Token).ToArray())
                            {
                                try
                                {
                                    await NotifyAsync(token, send.Caption, send.Text);
                                }
                                catch (Exception err)
                                {
                                    ret+= "Send#2 " + err.Message.ToString() + Environment.NewLine;
                                };

                            }
                        }
                        catch (Exception err)
                        {
                            return "Send#3 " + err.Message.ToString();
                        };

                    }
                }
                catch (Exception err)
                {
                   ret+= "Send#4 " + err.Message.ToString() + Environment.NewLine;
                };
            }
            ret+="Send OK";
            return ret;
        }

    //    https://stackoverflow.com/questions/37412963/send-push-to-android-by-c-sharp-using-fcm-firebase-cloud-messaging
        public static async Task<string> NotifyAsync( string to, string title, string body)
        {
            try
            {
                //cloudmessage page see!!!!!!!!!!!!
                var serverKey = string.Format("key={0}", "AAAAOziWWGc:APA91bHbSFqG7mzH_b2intcVElI8ljuytY5vvfqlOp7cr6CT0_xOvwYOVtEmDYy-RrJpbQiOmZP3iw-ogEPPzXeCmsc-n7-2mZZXG2kf1ATDmczjLSa66GELgOGsVPHcGVsTZvba49m5");
                var senderId = string.Format("id={0}", "254352447591");

                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body }
                };

                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {

                    



                httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


                    //HttpClientHandler handler = new HttpClientHandler();
                    //handler.UseDefaultCredentials = true;// .DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    /*   HttpClientHandler handler = new HttpClientHandler()
                       {
                           Proxy = new WebProxy("172.17.10.52:10000")
                           ,Credentials = new NetworkCredential("iu.smirnov", "mou773nitnap*", "ghp.lc"),
                       };handler*/
                    using (var httpClient = new HttpClient())
                    {
                      
                      var result = await httpClient.SendAsync(httpRequest);

                        if (result.IsSuccessStatusCode)
                        {
                            return "OK";
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
        public static string Notify(string to, string title, string body)
        {
            try
            {
                // Get the server key from FCM console
                var serverKey = string.Format("key={0}", "AAAAOziWWGc:APA91bHbSFqG7mzH_b2intcVElI8ljuytY5vvfqlOp7cr6CT0_xOvwYOVtEmDYy-RrJpbQiOmZP3iw-ogEPPzXeCmsc-n7-2mZZXG2kf1ATDmczjLSa66GELgOGsVPHcGVsTZvba49m5");

                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", "254352447591");

                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body },
                //    priority = "high",
                //    content_available = true

                };

                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {





                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


                    //HttpClientHandler handler = new HttpClientHandler();
                    //handler.UseDefaultCredentials = true;// .DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                    /*   HttpClientHandler handler = new HttpClientHandler()
                       {
                           Proxy = new WebProxy("172.17.10.52:10000")
                           ,Credentials = new NetworkCredential("iu.smirnov", "mou773nitnap*", "ghp.lc"),
                       };handler*/
                    using (var httpClient = new HttpClient())
                    {

                        var result =  httpClient.SendAsync(httpRequest);
                        //Task t = SendOutputReportViaInterruptTransfer();
                        result.Wait();
                        return result.Result.StatusCode.ToString();
                        /*
                        if (result.IsSuccessStatusCode)
                        {
                            return "OK";
                        }
                        else
                        {
                            return $"Error sending notification.Status Code: { result.StatusCode}";
                            // Use result.StatusCode to handle failure
                            // Your custom error handler here
                            //         _logger.LogError($"Error sending notification. Status Code: {result.StatusCode}");
                        }*/
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

        private static JobInfoDataClassesDataContext DB_isConnected()
        {
            int nErr = 0;
            JobInfoDataClassesDataContext d = new JobInfoDataClassesDataContext();
            d.CommandTimeout = 60;
            d.Connection.Open();
            while (d == null || d.DatabaseExists() == false || d.Connection.State != System.Data.ConnectionState.Open)
            {


                if (d.Connection.State != System.Data.ConnectionState.Open)
                {
                    if (nErr > 100)
                    {
                        nErr++;
                        throw new Exception("Не установил соедиение с базой данной");
                    }
                }
            }
            return d;
        }
    }
}