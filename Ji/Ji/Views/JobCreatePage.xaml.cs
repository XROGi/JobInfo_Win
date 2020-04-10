using Ji.Models;
using Ji.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobCreatePage : ContentPage
    {
        JobViewModel model;

        //  public ObservableCollection<string> Items { get; set; }
     
        public JobCreatePage( ObjMsg obj)
        {
            InitializeComponent();
            model =new JobViewModel(obj);
            BindingContext = model;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //if (e.Item == null)
            //    return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            ////Deselect Item
            //((ListView)sender).SelectedItem = null;
        }

        private async void OnUserAdd(object sender, EventArgs e)
        {
            GroupChat _newGroup = new GroupChat(model.NewMsg);
            UsersAddViewPage employeesServices = new UsersAddViewPage(_newGroup);
            employeesServices.OnUsersSelected += EmployeesServices_OnUsersSelected;
            //    (Application.Current as App).MainPage = new NavigationPage(employeesServices); // or other navigation methods


            //https://docs.microsoft.com/ru-ru/xamarin/xamarin-forms/app-fundamentals/navigation/hierarchical
            NavigationPage.SetBackButtonTitle(employeesServices, "Название группы");
            await  Navigation.PushAsync(employeesServices); ;
           


        }

        private void EmployeesServices_OnUsersSelected(UserChat[] selected_UserChat)
        {
            if (selected_UserChat != null)
                model.SetUserList(selected_UserChat);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (model.bBusy == false)
            {
                if (model.NewMsg.ObjId > 0)
                {
                }
                else
                {
                    if (String.IsNullOrEmpty(model.NewMsg.Text.Trim()))
                    {
                        await DisplayAlert("Выбор данных", "Введите название для задачи", "ОК");
                        return;

                    }

                    model.bBusy = true;
                //    createbtn.IsEnabled = !model.bBusy;
                    int[] users = model.GetUserList();
                    
                    ObjJob job = new ObjJob();
                    job.JobChat = model.NewMsg;
                    job.isJobNative = model.b_JobNative;
                    job.isJobSvod= model.b_JobSvod;
                    job.DateFinish = model.DateEnd;
                    job.Msgs.Add(model.Msg);
                    job.SetUsersIn(users);
                    job.JobType = MsgObjType.Job;


                    App.ddd.OnReciveChatAppend += Ddd_OnReciveChatAppend;
                    App.ddd.SR.OnUnknownError += SR_OnUnknownError;  ;

                    //      await App.ddd.SR.Request_Chat_Create(0, (int)job.JobType, job.JobChat.Text, job.JobChat.Description, job.UsersIn.ToArray()).ConfigureAwait(false);
                    await App.ddd.SR.Request_Job_Create
                        (
                            0, 
                            (int)model.JobClassTypeId,
                            job.JobChat.Text, 
                            job.JobChat.Description,
                            job.UsersIn.ToArray(),
                            job.isJobNative,
                            job.isJobSvod,
                            job.Msgs.Select(s=>s.ObjId).ToArray()
                        )
                        .ConfigureAwait(false);

                    //model.NewMsg;
                }
            }
        }

        private void SR_OnUnknownError(string Error)
        {
            model.bBusy = false; 
            App.ddd.SR.OnUnknownError -= SR_OnUnknownError;
        }

        private async void Ddd_OnReciveChatAppend(int chatId, Obj objtemp)
        {
            try
            {
                App.ddd.OnReciveChatAppend -= Ddd_OnReciveChatAppend;
                App.ddd.SR.OnUnknownError  -= SR_OnUnknownError;

                model.bBusy = false;
                createbtn.IsEnabled = !model.bBusy;
                try
                {
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());  
                }
                catch (Exception err)
                {
                }
            }
            catch (Exception err)
            {
            }
        }
 
    }
}
