using Infrastructure.Interfaces.UserSettingService;
using System.ComponentModel;

namespace TurboPMACServiceTests.FakeClasses
{
    public class FakeOffsetData : IOffsetData
    {
        private double _a;
        private double _z;
        private double _y;
        private double _x;
        private double _w;
        private double _v;
        private double _u;
        private double _c;
        private double _b;

        public FakeOffsetData(string offsetName)
        {
            Offset = offsetName;
        }

        public string Offset { get; }
        public double A
        {
            get => _a;
            set
            {
                _a = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(A)));
            }
        }
        public double B
        {
            get => _b;
            set
            {
                _b = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(B)));
            }
        }
        public double C
        {
            get => _c;
            set
            {
                _c = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(C)));
            }
        }
        public double U
        {
            get => _u;
            set
            {
                _u = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(U)));
            }
        }
        public double V
        {
            get => _v;
            set
            {
                _v = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(V)));
            }
        }
        public double W
        {
            get => _w;
            set
            {
                _w = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(W)));
            }
        }
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }
        public double Z
        {
            get => _z;
            set
            {
                _z = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Z)));
            }
        }
        public double I => X;
        public double J => Y;
        public double K => Z;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
