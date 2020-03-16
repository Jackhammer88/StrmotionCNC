using Infrastructure.Resources.Strings;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public class BoolToMachineStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool state)
            {
                if (state)
                    return CodesStrings.MachineStateOn;
                else
                    return CodesStrings.MachineStateOff;
            }
            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
