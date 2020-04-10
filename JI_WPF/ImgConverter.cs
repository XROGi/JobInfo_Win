using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace JI_WPF
{
    public class ImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {///https://stackoverflow.com/questions/16035300/make-wpf-image-load-async
            if (value != null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                
                bi.UriSource = new Uri("http://www.gravatar.com/avatar.php?gravatar_id=1", UriKind.Absolute);  
                 
                bi.EndInit();
                return bi;
            }
            else
            {
                return null;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
