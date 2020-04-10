using Ji.Services;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(QrScanningService))]

namespace Ji.Services
{
    public class QrScanningService : IQrScanningService
    {
        public async Task<string> ScanAsync()
        {/*
            var optionsDefault = new MobileBarcodeScanningOptions();
            var optionsCustom = new MobileBarcodeScanningOptions();

            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Scan the QR Code",
                BottomText = "Please Wait",
            };

            var scanResult = await scanner.Scan(optionsCustom);
            if (scanResult ==ZXing.Result 
)
            {
                return "Error Camera Scan";
            }
            return scanResult.Text;
*/
            return "";


        }
    }
}
