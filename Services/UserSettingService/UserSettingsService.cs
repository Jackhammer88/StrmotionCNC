using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserSettingService
{
    public class UserSettingsService : IUserSettingsService
    {
        public UserSettingsService()
        {
            //Offsets
            LoadGlobalOffsets();
            //Tool offsets
            LoadToolOffsets();
        }
        private void LoadToolOffsets()
        {
            string[] toolOffsetNames = { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" };
            int tCounter = 0;
            foreach (var name in toolOffsetNames)
            {
                var settingsTOProps = typeof(AppUserSettings).GetProperties().Where(p => p.Name.Contains(name) && p.Name.Length == (name.Length + 1)).ToList();
                var toPropLength = typeof(ToolOffsetData).GetProperty("ToolLength");
                var toSettingsPropL = settingsTOProps.First(p => p.Name.Equals($"{name}L", StringComparison.Ordinal));
                var toLVal = toSettingsPropL.GetValue(AppUserSettings.Default);
                toPropLength.SetValue(ToolOffsets[tCounter], toLVal);
                var toPropDiameter = typeof(ToolOffsetData).GetProperty("ToolDiameter");
                var toSettingsPropD = settingsTOProps.First(p => p.Name.Equals($"{name}D", StringComparison.Ordinal));
                var toDVal = toSettingsPropD.GetValue(AppUserSettings.Default);
                toPropDiameter.SetValue(ToolOffsets[tCounter], toDVal);
                tCounter++;
            }
        }
        private void LoadGlobalOffsets()
        {
            string[] offsetNames = { "G54", "G55", "G56", "G57", "G58", "G59" };
            string[] helperCodes = { "I", "J", "K" };
            int counter = 0;
            foreach (var offsetName in offsetNames)
            {
                var settingsProperties = typeof(AppUserSettings).GetProperties().Where(p => p.Name.Contains(offsetName) && !helperCodes.Contains(p.Name)).ToList();
                foreach (var prop in typeof(OffsetData).GetProperties().Where(p => p.Name.Length == 1 && !helperCodes.Contains(p.Name)))
                {
                    var tempProperty = settingsProperties.First(p => p.Name.Equals($"{offsetName}{prop.Name}", StringComparison.Ordinal));
                    var tempPropertyValue = tempProperty.GetValue(AppUserSettings.Default);
                    prop.SetValue(Offsets[counter], tempPropertyValue);
                }
                counter++;
            }
        }
        public int DeviceNumber
        {
            get => AppUserSettings.Default.DeviceNumber;
            set
            {
                AppUserSettings.Default.DeviceNumber = value;
                AppUserSettings.Default.Save();
            }
        }
        public int Timeout
        {
            get => AppUserSettings.Default.Timeout;
            set
            {
                AppUserSettings.Default.Timeout = value;
                AppUserSettings.Default.Save();
            }
        }
        public int ReconnectionCount
        {
            get => AppUserSettings.Default.ReconnectionCount;
            set
            {
                AppUserSettings.Default.ReconnectionCount = value;
                AppUserSettings.Default.Save();
            }
        }
        public int MaxCurrentLimit
        {
            get => AppUserSettings.Default.MaxCurrentLimit;
            set
            {
                AppUserSettings.Default.MaxCurrentLimit = value;
                AppUserSettings.Default.Save();
            }
        }
        public int MachineType
        {
            get => AppUserSettings.Default.MachineType;
            set
            {
                AppUserSettings.Default.MachineType = value;
                AppUserSettings.Default.Save();
            }
        }
        public int ScaleFactor
        {
            get => AppUserSettings.Default.ScaleFactor;
            set
            {
                AppUserSettings.Default.ScaleFactor = value;
                AppUserSettings.Default.Save();
            }
        }

        public int SafetyXCoordinate
        {
            get => AppUserSettings.Default.SafetyXCoordinate;
            set
            {
                AppUserSettings.Default.SafetyXCoordinate = value;
                AppUserSettings.Default.Save();
            }
        }
        public int SafetyYCoordinate
        {
            get => AppUserSettings.Default.SafetyYCoordinate;
            set
            {
                AppUserSettings.Default.SafetyYCoordinate = value;
                AppUserSettings.Default.Save();
            }
        }
        public int SafetyZCoordinate
        {
            get => AppUserSettings.Default.SafetyZCoordinate;
            set
            {
                AppUserSettings.Default.SafetyZCoordinate = value;
                AppUserSettings.Default.Save();
            }
        }

        public List<IOffsetData> Offsets { get; } = new List<IOffsetData>
        {
            new OffsetData("G54"), new OffsetData("G55"), new OffsetData("G56"),
           new OffsetData("G57"), new OffsetData("G58"), new OffsetData("G59")
        };
        public List<IToolOffsetData> ToolOffsets { get; } = new List<IToolOffsetData>
        {
            new ToolOffsetData(1), new ToolOffsetData(2),new ToolOffsetData(3),new ToolOffsetData(4),new ToolOffsetData(5),new ToolOffsetData(6),
            new ToolOffsetData(7),new ToolOffsetData(8),new ToolOffsetData(9),new ToolOffsetData(10),new ToolOffsetData(11),new ToolOffsetData(12)
        };
        public int CSNumber
        {
            get => AppUserSettings.Default.CSNumber;
            set
            {
                AppUserSettings.Default.CSNumber = value;
                AppUserSettings.Default.Save();
            }
        }
    }
}
