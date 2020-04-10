//using Android;
//using Android.Content.PM;
using Ji.Models;
using Ji.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
//using Plugin.Permissions;
//using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLib;
using ZXing.Net.Mobile.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    

    public partial class ConnectServerPage : ContentPage
    {
        IChatDataStore c;
        IMyApplicationInfo deviceIn;
        public ConnectServerPage()
        {

            InitializeComponent();
            try
            {
                URLSOAP.Text = "";
                AboutSOAP.Text = "";
                StatusWS.Text = "";
                PrintCurrentStatus();

            }catch (Exception err)
            {

            }
            
        }
        private void RequestCameraPermission()
        {
            const int requestPermissionCode = 0;

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Denied &&
            //    ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Denied &&
            //    ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) !=
            //    Permission.Denied) 
            //    return;

   //         ActivityCompat.RequestPermissions(this, new[]
   //         {
   //   Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage
   //}, requestPermissionCode);
        }

        private void PrintCurrentStatus()
        {
            try
            {

                if (App.ddd.isConnectSetup() == false)
                {
                    ConnectInfo.IsVisible = false;
                    btnExit.IsVisible = false;
                    btnScan.IsEnabled = true;
                }
                else
                {
                    ConnectInfo.IsVisible = true;
                    btnExit.IsVisible = true;

                    URLSOAP.Text = App.ddd.connectInterface.Server_WS;//.WS.Url;
                    btnScan.IsEnabled = false;
                }
                fb_token.Text = App.ddd.connectInterface.Fb_Token;
                StatusWS.Text = App.ddd.statusConect.ToString();
              //  AboutSOAP.Text = App.ddd.SOAP_AboutVersion;
                AboutWS.Text = "";
            }
            catch (Exception ex)
            {

                 
            }
        }

        [Obsolete]
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            try
            {

                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (cameraStatus != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    cameraStatus = results[Permission.Camera];
                    if (cameraStatus != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Камера недоступна", "Разрешение использовать камеру, необходимо для первичной настройки приложения. Без него запуск приложения невозможен", "OK");
                      //  btnScan.IsEnabled = false;
                        return;
                    }
                }


                var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
                txtBarcode.Text = "";

                options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.QR_CODE };
                var scanPage = new ZXingScannerPage(options);
                scanPage.AutoFocus();
                scanPage.OnScanResult += (result) =>
                {
                    // Stop scanning
                    scanPage.IsScanning = false;
                    scanPage.AutoFocus(0, 500);

                    // Pop the page and show the result
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        Navigation.PopModalAsync();
                        //DisplayAlert("Scanned Barcode", result.Text, "OK");

                        /*if (result.Text != "EAAAAJeDQu/AxBpsofsphQsKxzz1F4J93YdDmsxcmsUOGLFLIPmfIPxWbj83qyq8QYsmY4ETt7hFe67Nat3+wdrOoUR7/eQKsi5iL9RMzUImuDmDmmWYjHrUHkjmlf4baneyBw==")
                        {
                        EAAAAIrrlC8oOyhKvcwFSLtVWqCzCzaPEHGHoNLNO3equiAFYHJLEYT5tv2ZYwel+gap4X2m7yrLaDhIjV+MJ2K0W1rbVDvxu7MFyGwhGtg8w1c2EJ9Msi+6nZZTUYAPU85bkQ==
                        EAAAAIrrlC8oOyhKvcwFSLtVWqCzCzaPEHGHoNLNO3equiAFYHJLEYT5tv2ZYwel+gap4X2m7yrLaDhIjV+MJ2K0W1rbVDvxu7MFyGwhGtg8w1c2EJ9Msi+6nZZTUYAPU85bkQ==
                        }*/
                        try
                        {


                            string Data = XCrypt.DecryptStringAES(result.Text, "JobInfo");
                            if (String.IsNullOrEmpty(Data) == false)
                            {

                                string[] dddd = Data.Split('&');
                                if (dddd != null && dddd.Length >= 2
                                    &&
                                    dddd[0].StartsWith("http://")
                                )
                                {//"http://194.190.100.194/xml/&875CACED-96A7-4E9B-BC68-A4C523F63B3E&"
                                 //Application.Current.Properties.ContainsKey("server")

                                    //c.connectInterface = new ConnectInterface() { ,  };
                                    //     txtBarcode.Text = result.Text;
                                    //App.ddd.ClearConnect();

                                    //ConnectInterface connectInterfac = new ConnectInterface();
                                    ////if (Application.Current.Properties.ContainsKey("TokenReqId"))
                                    //connectInterfac.TokenReqId = dddd[1];
                                    ////if (Application.Current.Properties.ContainsKey("Server_SOAP"))
                                    //connectInterfac.Server_Name = dddd[0];
                                    //App.ddd.connectInterface = connectInterfac;
                                    App.ddd.Setup_RegisterNewConnectAsync(dddd[1], dddd[0]);
                                    
                                    App.ddd.SetupParam_Load();

                                    //var alert = new AlertDialog.Builder(this);
                                    //alert.SetView(LayoutInflater.Inflate(Resource.Layout.Modal, null));
                                    //alert.Create().Show();

                                    //         Android.Widget.Toast.MakeText(Android.App.Activity, "Dialog fragment dismissed!", Android.Widget.ToastLength.Short).Show();

                                    PrintCurrentStatus();

                                    Application.Current.MainPage =  new SvodInfMasterDetailPage(); //new MainPage();
                                    //App.MainPage = ;

                                    //     btnScan.IsEnabled = false;
                                    btnScan.Text = "Настроено.";
                                    //btnScan.Text = "Настроено. Перезапустите программу";
                                    txtBarcode.Text = "";
                                 //   var answer = await DisplayActionSheet("Статус", "Настройка выполнена. Приложение перезапустится", "OK");
                     
                                   //  closeApplication();
                                }
                                else

                                {
                                    txtBarcode.Text = "Неверный код подключения.";
                                }
                            }
                        }catch (Exception err)
                        {
                            txtBarcode.Text = "4. Неверный код подключения" + err.Message.ToString()
                            ;
                        }
                       
                    });
                };
             await    Navigation.PushModalAsync(scanPage);


                var device = DeviceInfo.Model;
                var manufacturer = DeviceInfo.Manufacturer;
                var deviceName = DeviceInfo.Name;
                var version = DeviceInfo.VersionString;
                var platform = DeviceInfo.Platform;
                var idiom = DeviceInfo.Idiom;
                var deviceType = DeviceInfo.DeviceType;
                
                    string Mac = "";
                    try
                {
                    var ttttt = NetworkInterface.GetAllNetworkInterfaces();
                    var ni = NetworkInterface.GetAllNetworkInterfaces().OrderBy(intf => intf.NetworkInterfaceType).FirstOrDefault(intf => intf.OperationalStatus == OperationalStatus.Up
                                && (intf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                                || intf.NetworkInterfaceType == NetworkInterfaceType.Ethernet));
                    if (ni != null)
                    {
                        var hw = ni.GetPhysicalAddress();
                        Mac = string.Join(":", (from ma in hw.GetAddressBytes() select ma.ToString("X2")).ToArray());
                    }
                    //1
                    //var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);
                    //if (status != PermissionStatus.Granted)
                    //{
                    //    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);
                    //    //Best practice to always check that the key exists
                    //    if (results.ContainsKey(Permission.Phone))
                    //        status = results[Permission.Phone];
                    //}
                    ////Get Imei
                    //LblImei.Text = "IMEI = " + DependencyService.Get<IServiceImei>().GetImei();

                }
                catch (Exception err)
                {
                    txtBarcode.Text = "2."+ err.Message.ToString();
                }
                //     txtBarcode.Text =  scanPage.Result.Text;

            }
            catch (Exception ex)
            {
                txtBarcode.Text ="1."+ ex.Message.ToString();
                //  throw;
            }
        }
        public void closeApplication()
        {
            try
            {
              App.deviceIn.closeApplication();
            }catch (Exception err)
            {

            }
            //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public void ShowAlert(string str)
        {
          //  DisplayAlert("Alert", "You have been alerted", "OK");
        }
        private void  BtnExit_Clicked22(object sender, EventArgs e)
        {
            //Application.Current.Properties.ContainsKey("server")
            //c.connectInterface = new ConnectInterface() { ,  };
            //     txtBarcode.Text = result.Text;
            App.ddd.ClearConnect();

            //if (App.ddd.connectInterface!=null)
            //{
            //    App.ddd.connectInterface.Server_SOAP = "";
            //    App.ddd.connectInterface.Server_WS = "";
            //    App.ddd.connectInterface.TokenReqId = "";
            //    App.ddd.connectInterface.TokenSeanceId= "";
            //}

         //   var answer = await DisplayActionSheet("Статус", "Настройка сброшена. Приложение будет перезапущено", "OK");

            closeApplication();

        }

        private void BtnExit_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.ddd.ClearConnect();
            }catch (Exception err)
            {

            }
            closeApplication();
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            App.ddd.ClearConnect();
        }
    }
}