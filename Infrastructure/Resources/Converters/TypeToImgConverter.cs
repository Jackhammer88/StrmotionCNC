using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public sealed class TypeToImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is FileInfo ? new Uri("pack://application:,,,/Infrastructure;component/Resources/icons/file-icon.png") : new Uri("pack://application:,,,/Infrastructure;component/Resources/icons/folder-icon.png");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
