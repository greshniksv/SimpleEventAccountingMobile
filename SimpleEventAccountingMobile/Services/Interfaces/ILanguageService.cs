using System.Globalization;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface ILanguageService
    {
        CultureInfo CurrentCulture { get; }
        event EventHandler<CultureInfo> CultureChanged;
        void SetLanguage(string culture);
        List<CultureInfo> SupportedLanguages { get; }
    }
}
