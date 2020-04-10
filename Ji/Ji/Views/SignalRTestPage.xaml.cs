using Ji.ViewModels;
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
    public partial class SignalRTestPage : ContentPage
    {
        SignalRViewModel viewModel;
        public SignalRTestPage()
        {
            InitializeComponent();
            viewModel = new SignalRViewModel();
            this.BindingContext = viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.Connect();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await viewModel.Disconnect();
        }
    }
}