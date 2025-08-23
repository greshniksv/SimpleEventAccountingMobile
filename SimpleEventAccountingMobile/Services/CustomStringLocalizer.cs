using Microsoft.Extensions.Localization;
using System.Globalization;
using SimpleEventAccountingMobile.Resources;

namespace SimpleEventAccountingMobile.Services
{
    public class CustomStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, Strings.ResourceManager.GetString(name) ?? "ERROR");

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }
    }
}
