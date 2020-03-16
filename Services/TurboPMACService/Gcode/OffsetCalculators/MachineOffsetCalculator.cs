using Light.GuardClauses;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using System;
using System.Globalization;

namespace ControllerService.Gcode.OffsetCalculators
{
    public static class MachineOffsetCalculator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Правильно создавайте экземпляры исключений аргументов", Justification = "<Ожидание>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public static string ApplyOffsets(string programString, int coordinateSystem, int toolNumber, MachineType machineType, IUserSettingsService userSettings)
        {
            if (string.IsNullOrEmpty(programString)) return programString;
            userSettings.MustNotBeNullReference(nameof(userSettings));

            programString = programString.ToUpper(CultureInfo.InvariantCulture);

            var globalCalculator = new GlobalOffsetCalculator();
            globalCalculator.ApplyOffsets(ref programString, userSettings.Offsets[coordinateSystem]);

            IToolOffsetCalculator calculator;
            switch (machineType)
            {
                case MachineType.Laser:
                    calculator = new LaserToolOffsetCalculator();
                    break;
                case MachineType.Milling:
                    calculator = new MillingToolOffsetCalculator();
                    break;
                case MachineType.Turning:
                    calculator = new TurningToolOffsetCalculator();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Передан несуществующий тип станка");
            }
            return calculator.ApplyOffsets(programString, userSettings.ToolOffsets[toolNumber]);
        }
    }
}
