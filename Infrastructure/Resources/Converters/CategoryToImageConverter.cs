using Prism.Logging;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Infrastructure.Resources.Converters
{
    public class CategoryToImageConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Category cat)
            {
                switch (cat)
                {
                    case Category.Debug:
                        return new Uri("pack://application:,,,/Infrastructure;component/Resources/icons/information.png");
                    case Category.Exception:
                        return new Uri("pack://application:,,,/Infrastructure;component/Resources/icons/error.png");
                    case Category.Info:
                        return new Uri("pack://application:,,,/Infrastructure;component/Resources/icons/information.png");
                    case Category.Warn:
                        return new Uri("pack://application:,,,/Infrastructure;component/Resources/icons/warning.png");
                    default:
                        return new Uri(string.Empty);
                }
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
