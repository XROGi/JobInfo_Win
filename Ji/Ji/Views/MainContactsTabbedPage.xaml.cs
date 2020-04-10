using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ji.Droid;
using Ji.Models;
using Ji.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;


namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainContactsTabbedPage : Xamarin.Forms.TabbedPage
    {
        public MainContactsTabbedPage()
        {
            InitializeComponent();
            InitTabs();
        }
        private void InitTabs()
        {
            Children.Clear();
            //_= On<Android>().SetBarItemColor(Color.Red);
            //_ = On<Android>().SetBarSelectedItemColor(Color.White);
            //SelectedTabColor = Color.White;
            //BarTextColor = Color.FromHex("#66FFFFFF");

            //Children.Add(new ItemsPage() { IconImageSource = "todo48.png", BackgroundImageSource = "todo48.png", HeightRequest = 96  , Title="Мне"}); 
            //Children.Add(new ContactsPage() { IconImageSource = "user48.png", Title = "От меня" });

            Children.Add(new ContactsPage("Favorite") { Title = "Избранные"  ,BackgroundImageSource = "Background.png" });
            Children.Add(new ContactsPage("All") { Title = "Все", BackgroundImageSource = "Background.png" });
        }
        private void FilterButtonClick(object sender, EventArgs e)
        {
            //FilterPanel
            JobsViewPage job = CurrentPage as JobsViewPage;
            if (job != null)
            {
                job.Switch_HidePanel();
            }
        }
    }
}