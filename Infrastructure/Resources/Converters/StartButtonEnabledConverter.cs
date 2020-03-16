using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public sealed class StartButtonEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            if (values[0] is bool isAuto && values[1] is bool isPressed)
                return isAuto && !isPressed;
            return true;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
