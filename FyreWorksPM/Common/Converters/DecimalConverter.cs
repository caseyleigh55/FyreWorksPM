using System.Globalization;

namespace FyreWorksPM.Common.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
                return dec.ToString("0.##"); // <-- nice formatting for display/edit

            return value?.ToString() ?? "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value?.ToString(), NumberStyles.Any, culture, out var result))
                return result;

            return 0m;
        }
    }


}
