using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XROGiClassLibrary
{
   public class ExchangeSendEmail
    {
       ExchangeService service;
       StreamingSubscriptionConnection connection;
        bool ? b_CurrentUser = null ;
        public void SendMail(string Subject, string text, string To, bool _b_CurrentUser=false)
        {
            // Create an email message and provide it with connection 
            // configuration information by using an ExchangeService object named service.
            if (_b_CurrentUser)
                service= OpenService();
            else
                service= OpenServiceBot();
            if (service == null) return;

            EmailMessage message = new EmailMessage(service);

            // Set properties on the email message.
            message.Subject = Subject;
            message.Body = text+"<BR/>" +Environment.NewLine  ;

            //if (email != "")
            string[] ff = To.Split(';');
            if (To.Split(';').Count() == 1)
                message.ToRecipients.Add(To);
            else
            {
                foreach (string g in ff)
                {
                    message.ToRecipients.Add(g);
                }
            }

            if (To.ToLower() != "iu.smirnov@ghp.lc" && To.ToLower() != "iu.smirnov")
            {
                message.CcRecipients.Add("iu.smirnov@ghp.lc");
            //    message.CcRecipients.Add("o.artemev@ghp.lc");
            }
            

            //message.ToRecipients.Add(To);
            //message.ToRecipients.Add("e.valuk@ghp.lc");
            //message.ToRecipients.Add("iu.smirnov@ghp.lc");

            //  message.s   IsBodyHtml = false;

            // Send the email message and save a copy.
            // This method call results in a CreateItem call to EWS.
            message.SendAndSaveCopy();

            //   Console.WriteLine("An email with the subject '" + message.Subject + "' has been sent to '" + message.ToRecipients[0] + "' and saved in the SendItems folder.");
        }
        public  ExchangeSendEmail(bool _b_CurrentUser)
        {
            return;
                if (_b_CurrentUser)
                    service = OpenService();
                else
                    service = OpenServiceBot();
           
            
        }

        public ExchangeSendEmail()
        {
            service = OpenService();

        }

        private ExchangeService OpenService()
        {
            if (service == null || b_CurrentUser == false)
            {
                service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);


                service.Credentials = new WebCredentials();
                //service.AutodiscoverUrl("bot@ghp.lc");


                //    service.Credentials = new WebCredentials();
                service.Url = new Uri("https://exch.ghp.lc/EWS/Exchange.asmx");
                b_CurrentUser = true;
            }
            //service.AutodiscoverUrl("iu.smirnov@ghp.lc", RedirectionUrlValidationCallback);
            return service;
        }

        private ExchangeService OpenServiceBot()
        {
      //      return null;
            if (service == null || b_CurrentUser == true )
            {
                service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                service.TraceEnabled=true;

                service.Credentials = new WebCredentials("bot@ghp.lc", "1йфя№УВС");
                service.AutodiscoverUrl("bot@ghp.lc");
           //      service.AutodiscoverUrl("https://Autodiscover.ghp.lc/Autodiscover/Autodiscover.xml");


                //    service.Credentials = new WebCredentials();
//                service.Url = new Uri("https://exch.svod-int.ru/EWS/Exchange.asmx");
                service.Url = new Uri("https://exch.ghp.lc/EWS/Exchange.asmx");
         //       service.
                b_CurrentUser = false;
            }
            
            return service;
            //service.AutodiscoverUrl("iu.smirnov@ghp.lc", RedirectionUrlValidationCallback);

        }
        public void SendMailError(string Subject, string text, string To)
        {
            if (service == null) return;
            To = "iu.smirnov@ghp.lc";
            // Create an email message and provide it with connection 
            // configuration information by using an ExchangeService object named service.
            OpenService();


            EmailMessage message = new EmailMessage(service);

            // Set properties on the email message.
            message.Subject = Subject;
            message.Body = text;

            //if (email != "")
            string[] ff = To.Split(';');
            if (To.Split(';').Count() == 1)
                message.ToRecipients.Add(To);
            else
            {
                foreach (string g in ff)
                {
                    message.ToRecipients.Add(g);
                }
            }
      //      message.ToRecipients.Add("iu.smirnov@ghp.lc");

            message.CcRecipients.Add("iu.smirnov@ghp.lc");
            ///message.CcRecipients.Add("o.artemev@ghp.lc");
            //message.CcRecipients.Add("d.tomshin@ghp.lc");
        //    message.CcRecipients.Add("i.sinycin@ghp.lc");
            //message.CcRecipients.Add("o.artemev@ghp.lc");
            

            //message.ToRecipients.Add(To);
            //message.ToRecipients.Add("e.valuk@ghp.lc");
            //message.ToRecipients.Add("iu.smirnov@ghp.lc");

            //  message.s   IsBodyHtml = false;

            // Send the email message and save a copy.
            // This method call results in a CreateItem call to EWS.
            message.SendAndSaveCopy();

            //   Console.WriteLine("An email with the subject '" + message.Subject + "' has been sent to '" + message.ToRecipients[0] + "' and saved in the SendItems folder.");
        }

        public void AddSubscribeEvent()
        {
            OpenService();
            if (service == null) return;
            StreamingSubscription  ssparam = service.SubscribeToStreamingNotifications(
    new FolderId[] { WellKnownFolderName.Inbox },
    EventType.NewMail,
    EventType.Created,
    EventType.Deleted,
    EventType.Modified,
    EventType.Moved,
    EventType.Copied,
    EventType.FreeBusyChanged);

            // Create a streaming connection to the service object, over which events are returned to the client.
            // Keep the streaming connection open for 30 minutes.
            StreamingSubscriptionConnection connection = new StreamingSubscriptionConnection(service, 30);
            connection.AddSubscription(ssparam);
            connection.OnNotificationEvent += OnNotificationEvent;
            connection.OnDisconnect += OnDisconnect;
            connection.Open();
        }


        public void AddSubscribeEvent(Microsoft.Exchange.WebServices.Data.StreamingSubscriptionConnection.NotificationEventDelegate OnNotificationEvent11)
        {

            //https://github.com/OfficeDev/ews-java-api/issues/249
            //            try
            {
             //   OpenService();
               OpenServiceBot();

                if (service == null) return;

                FolderId[] fol = new FolderId[] { WellKnownFolderName.Inbox };
                /*
                EventType.NewMail,
        EventType.Created,
        EventType.Deleted,
        EventType.Modified,
        EventType.Moved,
        EventType.Copied//,
        EventType.FreeBusyChanged
        );*/
                 



                        StreamingSubscription ssparam = service.SubscribeToStreamingNotifications(
        fol,
        EventType.NewMail//,
     //   EventType.Created,
     //   EventType.Deleted,
    //    EventType.Modified,
    //    EventType.Moved,
    //    EventType.Copied//,
      //  EventType.FreeBusyChanged
        );


             //   StreamingSubscriptionConnection dddd = new StreamingSubscriptionConnection(service, ssparam);

                // Create a streaming connection to the service object, over which events are returned to the client.
                // Keep the streaming connection open for 30 minutes.
                connection = new StreamingSubscriptionConnection(service, 1);
                connection.AddSubscription(ssparam);
                connection.OnNotificationEvent += OnNotificationEvent11;
                connection.OnDisconnect += OnDisconnect;
                connection.Open();
            }
              //  catch (Exception err)
            {


            }

        }

        private void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            try
            {
                if (service == null) return;
                connection.Open();
            }
            catch (Exception err)
            {
                service = null;
                connection = null;
            }
        }

        private void OnNotificationEvent(object sender, NotificationEventArgs args)
        {
            foreach (NotificationEvent ev in args.Events )
            {
                if (ev.EventType == EventType.NewMail)
                {
                    if (ev is ItemEvent)
                    {
                        //The NotificationEvent for an email message is an ItemEvent
                        ItemEvent itemEvent = (ItemEvent)ev;
                        Console.WriteLine("\nItemId:" + itemEvent.ItemId.UniqueId);
                        Item NewItem = Item.Bind(service, itemEvent.ItemId);
                        if (NewItem is EmailMessage)
                        {
                            if (NewItem.Subject !="")
                            {
                                EmailMessage mail = NewItem as EmailMessage;
                                if (mail != null)
                                {
                                    //switch NewItem.Subject 
                                    switch (NewItem.Subject.ToLower())
                                    {
                                        case "ping":
                                            SendMail("pong", DateTime.Now.ToLongTimeString(), mail.From.Address.ToString());// "iu.smirnov@ghp.lc"
                                            break;

                                        default:
                                            SendMail("Error command " + NewItem.Subject, DateTime.Now.ToLongTimeString(), "iu.smirnov@ghp.lc");
                                            break;
                                    }
                                }
                            }
                            Console.WriteLine(NewItem.Subject);
                         
                        }

                    }
                }
            }
        }

        public ExchangeService GetService()
        {
            return service;
        }

      
    }
}
