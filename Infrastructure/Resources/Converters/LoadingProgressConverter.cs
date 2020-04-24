using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public class LoadingProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                switch (progress)
                {
                    case double subValue when subValue <= 1:
                        return string.Empty;
                    case double subValue when subValue < 10:
                        return "Logging subsystem loading";
                    case double subValue when subValue < 20:
                        return "Controller service loading";
                    case double subValue when subValue < 30:
                        return "General components module loading";
                    case double subValue when subValue < 40:
                        return "Messaging module loading";
                    case double subValue when subValue < 50:
                        return "Motor info module loading";
                    case double subValue when subValue < 99:
                        return "Almost done";
                    case 100:
                        return "Done";
                    default:
                        return string.Empty;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
