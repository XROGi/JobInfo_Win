using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Ji.Models
{
    public class CustomEditor : Editor
    {
        public CustomEditor()
        {
            this.Keyboard = Keyboard.Chat;//    Android.Text.InputTypes.TextVariationShortMessage | Android.Text.InputTypes.ClassText
            
        }
    }
}
