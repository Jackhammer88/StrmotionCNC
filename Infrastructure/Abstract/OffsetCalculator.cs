using System.Globalization;
using System.Text.RegularExpressions;

namespace Infrastructure.Abstract
{
    public abstract class OffsetCalculator
    {
        protected virtual bool ParseAxis(string axisName, string programString, out double value)
        {
            value = default;
            if (string.IsNullOrEmpty(programString))
                return false;
            programString = programString.ToUpperInvariant();
            Regex regex = new Regex($"[{axisName}]([\\+-]?[0-9]*[\\.]?[0-9]+([0-9]+)?)");
            var axisValue = regex.Match(programString).Value.Replace(axisName, string.Empty);
            return double.TryParse(axisValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }
    }
}
