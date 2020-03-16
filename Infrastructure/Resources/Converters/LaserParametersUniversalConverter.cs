using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Infrastructure.Resources.Converters
{
    public class LaserParametersUniversalConverter : MarkupExtension, IValueConverter
    {
        public LaserParametersUniversalConverter()
        {
            UpperTreshold = float.MaxValue;
            LowerTreshold = float.MinValue;
        }
        public double UpperTreshold { get; set; }
        public double LowerTreshold { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float fValue) return Math.Round(fValue, 1);
            if (value is double dValue) return Math.Round(dValue, 1);
            if (value == null) return null;
            return new ValidationResult(false, "Decimal number format error");
        }

        private bool AmongThreshold(double fValue)
        {
            if (UpperTreshold > LowerTreshold) return fValue <= UpperTreshold && fValue >= LowerTreshold;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string fValue)
            {
                if (fValue.Length == 0) return null;

                if (double.TryParse(fValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double diValue))
                {
                    if (AmongThreshold(diValue))
                        return Math.Round(diValue, 1);
                    else
                        return new ValidationResult(false, "Outside of a threshold range.");
                }
                if (double.TryParse(fValue, NumberStyles.Float, CultureInfo.CurrentCulture, out double dcValue))
                {
                    if (AmongThreshold(dcValue))
                        return Math.Round(dcValue, 1);
                    else
                        return new ValidationResult(false, "Outside of a threshold range.");
                }
            }
            return new ValidationResult(false, "Decimal number format error");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
