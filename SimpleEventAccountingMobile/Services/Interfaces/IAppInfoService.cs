using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IAppInfoService
    {
        string GetAppVersion();
        string GetAppName();
        string GetBuildNumber();
    }
}
