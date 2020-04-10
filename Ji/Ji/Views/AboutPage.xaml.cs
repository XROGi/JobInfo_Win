using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Essentials;
using System;

namespace Ji.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            try
            {
                VersionTracking.Track();
                var currentVersion = VersionTracking.CurrentVersion;
                var currentBuild = VersionTracking.CurrentBuild;
                Version.Text = currentVersion+"."+ currentBuild;// +
            }catch (Exception err)
            {

            }
            /*
            var assembliesToInclude = new List<Assembly>
{
    typeof(CachedImage).GetTypeInfo().Assembly,
    typeof(FFImageLoading.Forms.Platform.CachedImageRenderer).GetTypeInfo().Assembly
};

            Xamarin.Forms.Forms.Init(e, assembliesToInclude);
            */
        }
    }
}