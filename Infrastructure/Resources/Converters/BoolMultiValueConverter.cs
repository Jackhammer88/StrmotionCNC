using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Infrastructure.Resources.Converters
{
    public class BoolMultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        public bool And { get; set; }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            if (And)
            {
                bool result = true;
                foreach (object item in values)
                {
                    if (item is bool bItem)
                        result = result && bItem;
                    else
                        throw new InvalidCastException();
                }
                return result;
            }
            else
            {
                bool result = false;
                foreach (object item in values)
                {
                    if (item is bool bItem)
                        result = result || bItem;
                    else
                        throw new InvalidCastException();
                }
                return result;
            }
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
