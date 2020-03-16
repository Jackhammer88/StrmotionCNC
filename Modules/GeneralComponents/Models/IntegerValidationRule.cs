using System.Globalization;
using System.Windows.Controls;

namespace GeneralComponents.Models
{
    class IntegerValidationRule : ValidationRule
    {
        public IntegerValidationRule()
        {
            MinimalValue = 0;
            MaximalValue = 100;
        }
        public int MinimalValue { get; set; }
        public int MaximalValue { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse((string)value, out int intValue) && intValue >= MinimalValue && intValue <= MaximalValue)
                return new ValidationResult(true, string.Empty);
            return new ValidationResult(false, $"Value must be between {MinimalValue} and {MaximalValue}.");
        }
    }
}
