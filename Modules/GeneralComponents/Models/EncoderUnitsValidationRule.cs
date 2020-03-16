using System.Globalization;
using System.Windows.Controls;

namespace GeneralComponents.Models
{
    internal class EncoderUnitsValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out int units))
            {
                if (units > 0 && units <= 10000)
                    return new ValidationResult(true, string.Empty);
                else
                    return new ValidationResult(false, "Out of range.");
            }
            else
                return new ValidationResult(false, "Invalid data");
        }
    }
}
