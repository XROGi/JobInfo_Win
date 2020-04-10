using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserListTabbedPage : TabbedPage
    {
        Models.GroupChat chat;
        public UserListTabbedPage(Models.GroupChat _chat)
        {
            InitializeComponent();
            chat = _chat;
            //var navigationPage = new NavigationPage(new NewItemPage());
            //navigationPage.IconImageSource = "schedule.png";
            //navigationPage.Title = "Schedule";
            Children.Clear();

            //            Children.Add(new GroupSelectedListViewPage() { IconImageSource = "todo48.png", Title = "Выбранные" }); ;
            bool b_OnlySelcted = true;
            Children.Add(new UsersSelectPage(chat, b_OnlySelcted) { IconImageSource = "todo48.png", Title = "Выбранные" }); ;
            Children.Add(new UsersSelectPage(chat,false) { IconImageSource = "user48.png", Title = "Все" }); ;

            //            Children.Add(navigationPage);

            //          < ContentPage Title = "Все"   IconImageSource = "/*user48.png*/" />
            //< ContentPage Title = "Выбранные" IconImageSource = "todo48.png" />
            //      < ContentPage Title = "Другое" IconImageSource = "settings64.png" />



        }
        protected override void OnDisappearing()
        {
            foreach (var t in Children)
            {
                if (t is UsersSelectPage us)
                {
                    us.Close();
                }
            }
            base.OnDisappearing();
        }
    }
}