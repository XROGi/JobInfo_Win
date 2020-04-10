using Ji.Droid;
using Ji.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Essentials;

namespace Ji.Services
{
    [XmlRoot("Device", Namespace = "http://localhost/xrogi", IsNullable = false)]
    public class Device
    {
        /* 
          <device name = "PARK-IT-PC444" devicetype="Microsoft Windows NT 6.2.9200.0&#x9;Win32NT" TokenId="-1" Token_Counter="2">
          <period dtb = "0001-01-01T00:00:00" dte="0001-01-01T00:00:00" dtc="2019-07-24T09:35:41.8368326+03:00" dtd="0001-01-01T00:00:00" />
          </device>

        alter procedure [dbo].[proc_TokenReq_IsApply] @TokenID varchar(255)  ,@DeviceName varchar(255) , @DeviceType varchar(255) , @DeviceUID varchar(255) , @OSVersion varchar(255) , @CS int, @OtherParams varchar(max) 
         ,'Blackview BV6000S_RU'
 ,'Phone'
 ,''
 ,'Android 7.0'
 ,0
 ,null
        */
        [XmlAttribute(AttributeName = "name")]
        public string name;/*manufacturer+' '+DeviceInfo.Model*/
        [XmlAttribute(AttributeName = "devicetype")]
        public string devicetype;
        [XmlAttribute(AttributeName = "IMEI")]
        public string IMEI;
        [XmlAttribute(AttributeName = "OSVersion")]
        public string OSVersion;
        [XmlAttribute(AttributeName = "OtherParams")]
        public string OtherParams;// split /n
        [XmlAttribute(AttributeName = "TokenId")]
        public string TokenId;
        [XmlAttribute(AttributeName = "Token_Counter")]
        long _Token_Counter;
        public long Token_Counter { get { _Token_Counter++; return _Token_Counter; } set { _Token_Counter = 0; } }
        public Period period;
        
        
        public void Fill(IMyApplicationInfo deviceIn)
        {

            try
            {
                
                var device = DeviceInfo.Model;
                var manufacturer = DeviceInfo.Manufacturer;
                var deviceName = DeviceInfo.Name;
                var version = DeviceInfo.VersionString;
                var platform = DeviceInfo.Platform;
                var idiom = DeviceInfo.Idiom;
                var deviceType_ = DeviceInfo.DeviceType;

                name = (manufacturer + " " + device).Trim();
                devicetype = idiom.ToString()  ;// deviceType_.ToString();
                OSVersion = (platform + " " + version).Trim();

                string Mac = "";
                try
                {
                    IMEI =  deviceIn.Get_AndroidMAC();//

                    /*
                    var ttttt = NetworkInterface.GetAllNetworkInterfaces();
                    var ni = NetworkInterface.GetAllNetworkInterfaces().OrderBy(intf => intf.NetworkInterfaceType).FirstOrDefault(intf => intf.OperationalStatus == OperationalStatus.Up
                                && (intf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                                || intf.NetworkInterfaceType == NetworkInterfaceType.Ethernet));
                    if (ni != null)
                    {
                        var hw = ni.GetPhysicalAddress();
                        Mac = string.Join(":", (from ma in hw.GetAddressBytes() select ma.ToString("X2")).ToArray());
                        IMEI = Mac;
                    }
                    */
                }
                catch (Exception err)
                {
                }
            }
            catch (Exception err)
            { }
        }

        internal string GetXMLString()
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Device));

            string xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, this);
                    xml = sww.ToString(); // Your XML
                }
            }
            return xml;
        }
    }
}
