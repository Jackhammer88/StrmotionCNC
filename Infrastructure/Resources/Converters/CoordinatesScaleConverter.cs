using Infrastructure.Resources.Strings;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public class CoordinatesScaleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return ((double)value) / 100;
            }
            throw new ArgumentException(CommonStrings.ConverterException);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
