using Infrastructure.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public class LaserSettingsComboBoxToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string sParameter)
            {
                if (value is Gas gValue)
                {
                    return (int)gValue;
                }
                if (value is Burning bValue)
                {
                    return (int)bValue;
                }
                if (value is bool boolValue)
                {
                    return boolValue ? 0 : 1;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int iValue && parameter is string sParameter)
            {
                if (sParameter.Equals("Gas", StringComparison.Ordinal))
                {
                    return (Gas)iValue;
                }
                else if (sParameter.Equals("Burning", StringComparison.Ordinal))
                {
                    return (Burning)iValue;
                }
                else if (sParameter.Equals("RoundedOn", StringComparison.Ordinal))
                {
                    return iValue == 0;
                }
            }
            return null;
        }
    }
}
