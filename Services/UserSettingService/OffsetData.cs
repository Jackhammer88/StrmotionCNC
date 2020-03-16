using Infrastructure.Interfaces.UserSettingService;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UserSettingService
{
    public class OffsetData : INotifyPropertyChanged, IOffsetData
    {
        private double _a;
        private double _b;
        private double _c;
        private double _u;
        private double _v;
        private double _w;
        private double _x;
        private double _y;
        private double _z;

        public OffsetData(string offset)
        {
            Offset = offset;
        }
        public OffsetData(string offset, double x, double y, double z, double a, double b, double c, double u, double v, double w)
        {
            Offset = offset;
            X = x;
            Y = y;
            Z = z;
            A = a;
            B = b;
            C = c;
            U = u;
            V = v;
            W = w;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void Set(ref double field, double value, [CallerMemberName]string name = "")
        {
            if (value != field)
            {
                field = value;
                var settingsProperty = typeof(AppUserSettings).GetProperty($"{Offset}{name}");
                settingsProperty.SetValue(AppUserSettings.Default, value);
                AppUserSettings.Default.Save();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public double A
        {
            get => _a;
            set => Set(ref _a, value);
        }
        public double B
        {
            get => _b;
            set => Set(ref _b, value);
        }
        public double C
        {
            get => _c;
            set => Set(ref _c, value);
        }
        public string Offset { get; }
        public double U
        {
            get => _u;
            set => Set(ref _u, value);
        }
        public double V
        {
            get => _v;
            set => Set(ref _v, value);
        }
        public double W
        {
            get => _w;
            set => Set(ref _w, value);
        }
        public double X
        {
            get => _x;
            set => Set(ref _x, value);
        }
        public double Y
        {
            get => _y;
            set => Set(ref _y, value);
        }
        public double Z
        {
            get => _z;
            set => Set(ref _z, value);
        }
        public double I => X;
        public double J => Y;
        public double K => Z;
    }
}
