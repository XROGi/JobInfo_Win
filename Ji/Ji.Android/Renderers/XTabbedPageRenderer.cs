using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using System.ComponentModel;
 
using Ji.Droid.Renderers.TabbedSample.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Ji.Controls;

[assembly: ExportRenderer(typeof(XTabbedPage), typeof(CustomTabbedPageRenderer))]

namespace Ji.Droid.Renderers
{
namespace TabbedSample.Droid.Renderers
    {
        public class CustomTabbedPageRenderer : TabbedPageRenderer
        {
            //https://www.andrewhoefling.com/Blog/Post/xamarin-forms-a-better-tabbed-page-renderer
            private bool _isConfigured = false;
            private ViewPager _pager;
            private TabLayout _layout;

            public CustomTabbedPageRenderer(Context context) : base(context) { }

            protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                base.OnElementPropertyChanged(sender, e);

                _pager = (ViewPager)ViewGroup.GetChildAt(0);
                _layout = (TabLayout)ViewGroup.GetChildAt(1);

                var control = (XTabbedPage)sender;
                Android.Graphics.Color selectedColor;
                Android.Graphics.Color unselectedColor;
                if (control != null)
                {
                    selectedColor = control.SelectedIconColor.ToAndroid();
                    unselectedColor = control.UnselectedIconColor.ToAndroid();
                }
                else
                {
                    selectedColor = new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.tabBarSelected));
                    unselectedColor = new Android.Graphics.Color(ContextCompat.GetColor(Context, Resource.Color.tabBarUnselected));
                }

                for (int i = 0; i < _layout.TabCount; i++)
                {
                    var tab = _layout.GetTabAt(i);
                     
                    var icon = tab.Icon;
                    if (icon != null)
                    {
                        var color = tab.IsSelected ? selectedColor : unselectedColor;
                        icon = Android.Support.V4.Graphics.Drawable.DrawableCompat.Wrap(icon);
                        icon.SetColorFilter(color, PorterDuff.Mode.SrcIn);
                    }
                }
            }
        }
    }
}