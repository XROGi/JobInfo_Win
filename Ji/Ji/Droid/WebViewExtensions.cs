using Xamarin.Forms;

namespace Ji.Droid
{
    public static class WebViewExtensions
    {
        public static void ResizeToContent(this WebView webView)
        {
         /*   var heightString = webView.InvokeScript("eval", new[] { "document.body.scrollHeight.toString()" });
            int height;
            if (int.TryParse(heightString, out height))
            {
                webView.Height = height;
            }

            var widthString = webView.InvokeScript("eval", new[] { "document.body.scrollWidth.toString()" });
            int width;
            if (int.TryParse(widthString, out width))
            {
                webView.Width = width;
            }
            */
        }
    }
}
