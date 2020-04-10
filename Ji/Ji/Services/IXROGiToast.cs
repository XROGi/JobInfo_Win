using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.Services
{
    public interface IXROGiToast
    {
         void LongAlert(string message);
          void ShortAlert(string message);
    }
}
