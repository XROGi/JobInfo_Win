using System;
using System.Globalization;
using Xamarin.Forms;

namespace Ji.Models
{
    public class HtmlSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var html = new HtmlWebViewSource();

            if (value != null)
            {
                html.Html = value.ToString();
            }

            return html;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class HtmlSourceConverterGetImageOnly : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ew = "https://www.rec.uk.com/__data/assets/image/0003/316083/REC-Web-Header-Banners-Compliance-test.jpg";
            /*var html = new HtmlWebViewSource();

            if (value != null)
            {
                html.Html = ew;
            }
            */
          return   new Uri(ew);//, UriKind.Relative
            //return html;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
