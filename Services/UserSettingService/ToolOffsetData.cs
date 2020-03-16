using Infrastructure.Interfaces.CNCControllerService;
using Prism.Mvvm;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UserSettingService
{
    public class ToolOffsetData : BindableBase, IToolOffsetData
    {
        public ToolOffsetData(int toolNumber) => _toolNumber = toolNumber.ToString(CultureInfo.InvariantCulture);

        private string _toolNumber;
        public string ToolNumber
        {
            get => _toolNumber;
            set => SetProperty(ref _toolNumber, value);
        }


        private double _toolLength;
        public double ToolLength
        {
            get => _toolLength;
            set
            {
                SetProperty(ref _toolLength, value);
                SaveSettings(_toolLength);
            }
        }


        private double _toolDiameter;
        public double ToolDiameter
        {
            get => _toolDiameter;
            set
            {
                SetProperty(ref _toolDiameter, value);
                SaveSettings(_toolDiameter);
            }
        }

        private void SaveSettings(double value, [CallerMemberName]string name = "")
        {
            PropertyInfo prop;
            switch (name)
            {
                case nameof(ToolLength):
                    prop = typeof(AppUserSettings).GetProperty($"T{ToolNumber}L");
                    break;
                case nameof(ToolDiameter):
                    prop = typeof(AppUserSettings).GetProperty($"T{ToolNumber}D");
                    break;
                default:
                    return;
            }
            prop.SetValue(AppUserSettings.Default, value);
            AppUserSettings.Default.Save();
        }
    }
}
