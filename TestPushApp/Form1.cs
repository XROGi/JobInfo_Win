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
using Newtonsoft.Json;


namespace TestPushApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                label1.Text = Notify(textBox1.Text, "Тестовое сообщение", DateTime.Now.ToString());
            }catch (Exception err)
            {
                label1.Text = err.Message.ToString();
            }
        }
        public  async Task<string> NotifyAsync(string to, string title, string body)
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
        public  string Notify(string to, string title, string body)
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

                        var result = httpClient.SendAsync(httpRequest);
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
    }
}
