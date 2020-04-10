using Ji.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Ji.ViewModels
{
    public class SetupViewModel : BaseSetupViewModel
    { 
        public SetupAppParam Params 
        { 
            get;
            set; 
        }
        string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
       

        public SetupViewModel()
        {
            Params = new SetupAppParam();
            Params.b_ShowExceptionText = true;
            Params.PropertyChanged += Params_PropertyChanged;
         
          //  Content = new StackLayout
            /*{
                Children = {
                    new Label { Text = "Welcome to Xamarin.Forms!" }
                }
            };*/

        }

        private void Params_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }
    }
}