using SimpleEventAccountingMobile.Services.Interfaces;
using System.Globalization;

namespace SimpleEventAccountingMobile.Services
{
    public class LanguageService : ILanguageService
    {
        private CultureInfo _currentCulture;

        public CultureInfo CurrentCulture => _currentCulture;
        public event EventHandler<CultureInfo>? CultureChanged;

        public List<CultureInfo> SupportedLanguages { get; } =
        [
            new CultureInfo("en"),
            new CultureInfo("ru")
        ];

        public LanguageService()
        {
            var savedLanguage = Preferences.Get("AppLanguage", CultureInfo.CurrentCulture.Name);
            _currentCulture = new CultureInfo(savedLanguage);
        }

        public void SetLanguage(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            _currentCulture = cultureInfo;

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Сохраняем настройку
            Preferences.Set("AppLanguage", culture);

            CultureChanged?.Invoke(this, cultureInfo);
        }
    }
}
