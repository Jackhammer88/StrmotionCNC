using Infrastructure.Resources.Strings;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{

    public class VisibilityMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.All(v => v is bool))
            {
                var result = values.Select(v => (bool)v).Aggregate((b1, b2) => b1 && b2);
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new ArgumentException(CommonStrings.ConverterException);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
