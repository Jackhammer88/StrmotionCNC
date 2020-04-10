using Light.GuardClauses;
using Infrastructure.Abstract;
using Infrastructure.Interfaces.CNCControllerService;
using System.Globalization;

namespace ControllerService.Gcode.OffsetCalculators
{
    public class MillingToolOffsetCalculator : OffsetCalculator, IToolOffsetCalculator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public string ApplyOffsets(string programString, IToolOffsetData toolOffset)
        {
            toolOffset.MustNotBeNullReference(nameof(toolOffset));
            if (string.IsNullOrEmpty(programString)) return programString;

            //Remove bad -0.000
            programString = programString.Replace("-0.000", "0").Replace("0.000", "0");

            if (ParseAxis("X", programString, out double xValue))
                programString = programString.Replace($"X{xValue.ToString(CultureInfo.InvariantCulture)}", $"X{(xValue + (toolOffset.ToolDiameter / 2)).ToString(CultureInfo.InvariantCulture)}");
            if (ParseAxis("I", programString, out double iValue))
                programString = programString.Replace($"I{iValue.ToString(CultureInfo.InvariantCulture)}", $"I{(iValue + (toolOffset.ToolDiameter / 2)).ToString(CultureInfo.InvariantCulture)}");

            if (ParseAxis("Y", programString, out double yValue))
                programString = programString.Replace($"Y{yValue.ToString(CultureInfo.InvariantCulture)}", $"Y{(yValue + (toolOffset.ToolDiameter / 2)).ToString(CultureInfo.InvariantCulture)}");
            if (ParseAxis("J", programString, out double jValue))
                programString = programString.Replace($"J{jValue.ToString(CultureInfo.InvariantCulture)}", $"J{(jValue + (toolOffset.ToolDiameter / 2)).ToString(CultureInfo.InvariantCulture)}");

            if (ParseAxis("Z", programString, out double zValue))
                programString = programString.Replace($"Z{zValue.ToString(CultureInfo.InvariantCulture)}", $"Z{(zValue + (toolOffset.ToolLength)).ToString(CultureInfo.InvariantCulture)}");
            if (ParseAxis("K", programString, out double kValue))
                programString = programString.Replace($"K{kValue.ToString(CultureInfo.InvariantCulture)}", $"K{(kValue + (toolOffset.ToolLength)).ToString(CultureInfo.InvariantCulture)}");

            return programString;
        }
    }
}
