using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public class AxisSelectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && parameter is string positive)
            {
                var adString = string.CompareOrdinal(positive, "-") == 0 ? "-" : "+";
                return $"{str}{adString}";
            }
            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
