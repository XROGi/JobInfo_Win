using System.Threading.Tasks;

namespace Ji.Services
{
    interface IQrScanningService
    {
      
            Task<string> ScanAsync();
       
    }
}
