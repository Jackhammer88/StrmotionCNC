using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Infrastructure.Resources.Converters
{
    public class SizeConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            if (values[0] is double spHeight && values[1] is double vbHeight && values[2] is int count)
            {
                if (count == 0)
                    return 0;
                return (spHeight - vbHeight) / count;
            }
            return 0;
            //throw new InvalidOperationException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
