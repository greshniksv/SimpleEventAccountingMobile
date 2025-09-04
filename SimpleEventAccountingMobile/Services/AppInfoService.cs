using SimpleEventAccountingMobile.Services.Interfaces;

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
