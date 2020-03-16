using Infrastructure.Resources.Strings;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Infrastructure.Resources.Converters
{
    public class BoolInverseConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Process(value);
        }

        private static object Process(object value)
        {
            if (value is bool)
                return !(bool)value;
            throw new ArgumentException(CommonStrings.ConverterException);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Process(value);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
