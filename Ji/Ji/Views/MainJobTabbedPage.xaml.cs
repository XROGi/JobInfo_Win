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
    public partial class MainJobTabbedPage : TabbedPage
    {
        public MainJobTabbedPage()
        {
            
            InitializeComponent();
            InitTabs();
        }

        private void InitTabs()
        {
            Children.Clear();

            //Children.Add(new ItemsPage() { IconImageSource = "todo48.png", BackgroundImageSource = "todo48.png", HeightRequest = 96  , Title="Мне"}); 
            //Children.Add(new ContactsPage() { IconImageSource = "user48.png", Title = "От меня" });
            
            Children.Add(new JobsViewPage( new List<xFilterAttribute>() { xFilterAttribute.JobMy}) {  Title = "Мне" });
            Children.Add(new JobsViewPage(new List<xFilterAttribute>() { xFilterAttribute.JobFromMy }) {  Title = "От меня" });


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