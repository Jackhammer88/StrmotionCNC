using Light.GuardClauses;
using Infrastructure.Abstract;
using Infrastructure.Interfaces.UserSettingService;
using System.Globalization;
using System.Linq;
using System;

namespace ControllerService.Gcode.OffsetCalculators
{
    public class GlobalOffsetCalculator : OffsetCalculator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public void ApplyOffsets(ref string programString, IOffsetData offsets)
        {
            if (string.IsNullOrEmpty(programString)) return;
            offsets.MustNotBeNullReference(nameof(offsets));

            programString = programString.ToUpper(CultureInfo.InvariantCulture);

            var props = offsets.GetType().GetProperties().Where(p => p.Name.Length == 1 && !p.Name.Contains("I") && !p.Name.Contains("J") && !p.Name.Contains("K")).OrderBy(p => p.Name);

            foreach (var prop in props)
            {
                var axisOffsetValue = (double)prop.GetValue(offsets);
                if (ParseAxis(prop.Name, programString, out double tValue))
                    ApplyOffsetsToAxis(ref programString, prop.Name, tValue, axisOffsetValue);
            }
        }
        private static void ApplyOffsetsToAxis(ref string programString, string axisName, double oldValue, double offsetValue)
        {
            if (oldValue == 0)
                programString = programString.Replace("-0.000", "0");

            programString = programString.Replace($"{axisName}{oldValue.ToString(CultureInfo.InvariantCulture)}", $"{axisName}{RoundValue(oldValue + offsetValue, 3)}");
        }

        private static object RoundValue(double value, int accuracy)
        {
            return Math.Round(value, accuracy, MidpointRounding.AwayFromZero).ToString("F3", CultureInfo.InvariantCulture);
        }
    }
}
