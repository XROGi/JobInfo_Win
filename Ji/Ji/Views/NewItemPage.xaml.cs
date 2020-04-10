using System;
using System.ComponentModel;

using Xamarin.Forms;

using Ji.Models;

namespace Ji.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class NewItemPage : ContentPage
    {
        public GroupChat Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();

            Item = new GroupChat
            {
                Text = ""
                //, Description = ""
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", Item);
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
         //   var mi = ((MenuItem)sender);
       //     if (mi != null)
            {
           //     GroupChat g = mi.CommandParameter as GroupChat;

                //if (g != null)
                {
                    try
                    {
                        GroupChat newGroup = new GroupChat() { Text = GroupName.Text, Description = GroupInfo.Text };

                        //Navigation.PushAsync(new UsersSelectPage(GroupName.Text, GroupInfo.Text));
                        //https://forums.xamarin.com/discussion/38437/silent-remove-of-page-in-navigation-stack
                        UsersSelectPage p = new UsersSelectPage(newGroup);
                        NavigationPage.SetBackButtonTitle(p, "Название группы");
                        Navigation.PushAsync( p); ;
          //              Navigation.RemovePage(this);
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }
    }
}