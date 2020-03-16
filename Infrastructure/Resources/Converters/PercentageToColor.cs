using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Infrastructure.Resources.Converters
{
    public class PercentageToColor : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                if (progress >= 0 && progress < 33)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F7D18F"));
                if (progress >= 33 && progress < 80)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5892E"));
                if (progress >= 80)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EA4030"));
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
            }
            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
