using Light.GuardClauses;
using Infrastructure.Abstract;
using Infrastructure.Interfaces.CNCControllerService;
using System.Globalization;

namespace ControllerService.Gcode.OffsetCalculators
{
    public class TurningToolOffsetCalculator : OffsetCalculator, IToolOffsetCalculator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public string ApplyOffsets(string programString, IToolOffsetData toolOffset)
        {
            toolOffset.MustNotBeNullReference(nameof(toolOffset));
            if (string.IsNullOrEmpty(programString)) return programString;

            if (ParseAxis("X", programString, out double xValue))
                programString = programString.Replace($"X{xValue.ToString(CultureInfo.InvariantCulture)}", $"X{(xValue + toolOffset.ToolDiameter).ToString(CultureInfo.InvariantCulture)}");
            if (ParseAxis("I", programString, out double iValue))
                programString = programString.Replace($"I{iValue.ToString(CultureInfo.InvariantCulture)}", $"I{(iValue + toolOffset.ToolDiameter).ToString(CultureInfo.InvariantCulture)}");
            if (ParseAxis("Z", programString, out double zValue))
                programString = programString.Replace($"Z{zValue.ToString(CultureInfo.InvariantCulture)}", $"Z{(zValue + toolOffset.ToolLength).ToString(CultureInfo.InvariantCulture)}");
            if (ParseAxis("K", programString, out double kValue))
                programString = programString.Replace($"K{kValue.ToString(CultureInfo.InvariantCulture)}", $"K{(kValue + toolOffset.ToolLength).ToString(CultureInfo.InvariantCulture)}");
            return programString;

        }
    }
}
