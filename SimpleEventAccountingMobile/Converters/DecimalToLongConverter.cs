using System.Globalization;

namespace SimpleEventAccountingMobile.Converters
{
    internal class DecimalToLongConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
                return dec.ToString("0");
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;
            return null;
        }
    }
}
