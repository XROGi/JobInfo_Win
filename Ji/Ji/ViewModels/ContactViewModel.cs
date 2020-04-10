using Ji.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Ji.ViewModels
{
    

    public class ContactViewModel : BaseViewModel
    {
        public UserChat Item { get; set; }
        public ContactViewModel(UserChat item = null)
        {
            //Title = item?..Text;
            Item = item;
        }

        internal void Close()
        {

            Item = null;
        }
    }
}