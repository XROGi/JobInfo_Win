using Ji.ClassSR;
using Ji.Droid;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParkingPassPage : ContentPage
    {
        ParkResult res;
        public ParkingPassPage()
        {
            InitializeComponent();
            PassNumber.Text = ""; Status.Text = "Билет не введён";
           Result.IsVisible = false;//ViewStates.Invisible
            Compleat.IsVisible = false;//ViewStates.Invisible
            App.ddd.SR.OnResultReciveParkingStep1 += SR_OnResultReciveParkingStep1;
            App.ddd.SR.OnResultReciveParkingStep2 += SR_OnResultReciveParkingStep2;
            //objectView.SetValue(IsVisibleProperty, false); // the view is GONE, not invisible
            //objectView.SetValue(IsVisibleProperty, true);

        }

        private void SR_OnResultReciveParkingStep2(string Number, string jsonResult)
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                //if (OnWSReciveNewChatStatistic != null)
                //    OnWSReciveNewChatStatistic.Invoke(this, chat);

          //      if (Number == PassNumber.Text)
                {
                    // jsonResult
                    if ((!String.IsNullOrEmpty(jsonResult)) || (!(jsonResult.StartsWith("id=null"))))
                    {
                        try
                        {
                            ViewPpsStep2Results dd = System.Text.Json.JsonSerializer.Deserialize<ViewPpsStep2Results>(jsonResult);
                            if (dd != null)
                            {
                                StatusStep2.Text = dd.Result;
                                DateOut.Text = dd.DateOut.ToString();
                                btnStep2.IsVisible = false;
                            }
                            else
                            {
                                ClearDataElements();
                                ResultatVisible(false);
                            }
                            StatusStep2.Text = String.IsNullOrEmpty(jsonResult) ? "Не найдено" : dd.Result;
                            Result.IsVisible = true;
                        }
                        catch (Exception err)
                        {
                        }

                    }
                    else
                    {
                        StatusStep2.Text = DateTime.Now.ToString("HH:mm:ss") + " Билет не найден";
                        ClearDataElements();
                        ResultatVisible(false);
                    }
                }
            });
        }
        ViewPpsStep1Results step1;
        private void SR_OnResultReciveParkingStep1(string Number, string jsonResult)
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                //if (OnWSReciveNewChatStatistic != null)
                //    OnWSReciveNewChatStatistic.Invoke(this, chat);

                if (Number == PassNumber.Text)
                {
                    // jsonResult
                    if ((!String.IsNullOrEmpty(jsonResult)) && (!(jsonResult.StartsWith("id=null"))))
                    {
                        try
                        {
                          
                            step1 = System.Text.Json.JsonSerializer.Deserialize<ViewPpsStep1Results>(jsonResult);
                            if (step1 != null)
                            {
                                btnStep2.IsVisible = true;
                                DateIn.Text = step1.DateIn.ToString();
                                // Place_In.Text = dd.Place_In;
                                ResultatVisible(true);
                                TestParamCancel();
                            }
                            else
                            {
                                ClearDataElements();
                                ResultatVisible(false);
                            }
                            Status.Text = String.IsNullOrEmpty(jsonResult) ? "Не найдено" : step1.Result;
                            Result.IsVisible = true;
                        }
                        catch (Exception err)
                        {
                        }

                    }
                    else
                    {
                        Status.Text = DateTime.Now.ToString("HH:mm:ss")+ " Билет не найден"; 
                        ClearDataElements();
                        ResultatVisible(false);
                    }
                }
            });

        
        }

        private async void BtnStep1_Clicked(object sender, EventArgs e)
        {
            try
            {
                Compleat.IsVisible = false;
                btnStep2.IsVisible = false;
                Status.Text = "";
                bool b_error = false;

                try
                {
                    if (PassNumber.Text.Length == 5)
                        PassNumber.Text = "0" + PassNumber.Text;

                    if (PassNumber.Text.Length == 6)
                    {
                        //  res = App.ddd.Parking_Step1(PassNumber.Text);
                        await App.ddd.SR.ReciveParkingStep1(PassNumber.Text).ConfigureAwait(false);
                        return;
                    }
                    else
                    {
                        Status.Text = "Введите 6 символов номера";
                        b_error = true;
                    }

                }
                catch (Exception err)
                {
                    Status.Text = err.Message.ToString();
                    ClearDataElements();
                    ResultatVisible(false);
                    b_error = true;
                }
                Result.IsVisible = false;
                if (b_error == false)
                {
                    if (res != null)
                    {
                        ClearDataElements();
                        if (res.Status == "Выезд возможен")
                        {
                            btnStep2.IsVisible = true;
                            DateIn.Text = res.DateIn.ToString();
                            Place_In.Text = res.Place_In;
                            ResultatVisible(true);
                        }
                        else
                        {
                            ClearDataElements();
                            ResultatVisible(false);
                        }
                        Status.Text = String.IsNullOrEmpty(res.Status) ? "Статус не определён" : res.Status;

                        Result.IsVisible = true;//ViewStates.Invisible
                                                //    PassNumber.Text = res.ParkingNumber;
                        if (res.Status.Contains("Выезд возможен"))
                        {
                        }
                        else
                        {
                            Status.Text = "Билет не найден";
                            ClearDataElements();
                            ResultatVisible(false);
                        }
                    }
                }
            
               
            }catch (Exception err )
            {
                Status.Text = "Сервис недоступен";
                ClearDataElements();
            }
        }

        private void ClearDataElements()
        {
            DateIn.Text = "";
            Out_Who.Text = "";
            Out_CarType.Text = "";
            Out_CarNumber.Text = "";
            Out_Reason.Text = "";
            //       PassNumber.Text = "";
            Place_In.Text = "";
            StatusStep2.Text = "";
            DateOut.Text = "";
        //    btnStep2.IsEnabled = false;
            TestParamCancel();
        }
        private void ResultatVisible(bool val)
        {
             DateIn.IsVisible= val;
            Out_Who.IsVisible = val;
            Out_CarType.IsVisible = val;
            Out_CarNumber.IsVisible = val;
            Out_Reason.IsVisible = val;
            //       PassNumber.Text = "";
            Place_In.IsVisible = val;


            DateInL.IsVisible = val;
            Place_InL.IsVisible = val;
            Out_WhoL.IsVisible = val;
            Out_CarTypeL.IsVisible = val;
            Out_CarNumberL.IsVisible = val;
            Out_ReasonL.IsVisible = val;

   
        }

        private async void BtnStep2_Clicked(object sender, EventArgs e)
        {
            Compleat.IsVisible = true;
            btnStep2.IsVisible = false;

            //            exec[proc_Propusk_Step2] 4, 'F000B7E8-132D-4A61-81D6-50B4A771B6FD', 'проверка','КИА','К975КК93','Проверка Android App'

            
            //ParkResult res2 = 
         //   await App.ddd.SR.ReciveParkingStep2("F000B7E8-132D-4A61-81D6-50B4A771B6FD", "проверка", "КИА", "К975КК93", "Проверка Android App");
            await App.ddd.SR.ReciveParkingStep2(step1.ParkingNumber, Out_Who.Text, Out_CarType.Text, Out_CarNumber.Text, Out_Reason.Text);



        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            Result.IsVisible = false;//ViewStates.Invisible
            Compleat.IsVisible = false;//ViewStates.Invisible
            
            PassNumber.Text = "";
        }

        private void TestValues(object sender, TextChangedEventArgs e)
        {
            TestParamCancel();
         
        }

        private void TestParamCancel()
        {
            if (
                  Out_CarType.Text.Trim().Length == 0
              || Out_Who.Text.Trim().Length == 0
              || Out_CarNumber.Text.Trim().Length == 0
              || Out_Reason.Text.Trim().Length == 0
              )
                btnStep2.IsEnabled = false;
            else
                btnStep2.IsEnabled = true;
        }
    }
}

