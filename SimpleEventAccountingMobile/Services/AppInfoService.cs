using SimpleEventAccountingMobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEventAccountingMobile.Services
{
    public class AppInfoService : IAppInfoService
    {
        public string GetAppVersion()
        {
            return AppInfo.Current.VersionString;
        }

        public string GetAppName()
        {
            return AppInfo.Current.Name;
        }

        public string GetBuildNumber()
        {
            return AppInfo.Current.BuildString;
        }
    }
}
