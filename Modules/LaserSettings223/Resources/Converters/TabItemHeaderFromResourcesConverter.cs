using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LaserSettings.Resources.Converters
{
    public class TabItemHeaderFromResourcesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue)
            {
                var property = typeof(GeneralStrings).GetProperties().SingleOrDefault(p => p.Name.Equals(sValue, StringComparison.Ordinal));
                return property == null ? sValue : (string)property.GetValue(null);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue)
            {

            }
            return null;
        }
    }
}
