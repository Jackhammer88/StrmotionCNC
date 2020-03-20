using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Infrastructure.Resources.Converters
{
    public class TextWrapperConverter : IValueConverter
    {
        private int _textLength = 20;
        private int _textMaxLength = 100;

        public int TextLength
        {
            get => _textLength;
            set
            {
                if (value > 0)
                    _textLength = value;
            }
        }
        public int TextMaxLength
        {
            get => _textMaxLength;
            set
            {
                if (value > TextLength)
                    _textMaxLength = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue)
            {
                var result = RemoveDateTime(sValue);
                if (result.Length > TextLength)
                {
                    return result.Length < TextMaxLength ? result : result.Substring(0, result.Length - 3) + "...";
                }
                else
                    return result.Substring(0, result.Length < TextMaxLength ? result.Length : TextMaxLength);
            }
            return null;
        }

        private string RemoveDateTime(string sValue)
        {
            var length = DateTime.Now.ToString(CultureInfo.InvariantCulture).Length /* + 2*/;
            return new string(sValue.Skip(length).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
