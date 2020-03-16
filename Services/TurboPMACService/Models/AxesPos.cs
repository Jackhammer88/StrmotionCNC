using Infrastructure.Interfaces.CNCControllerService;
using Prism.Mvvm;

namespace ControllerService.Models
{
    public class AxesPos : BindableBase, IAxesPos
    {

        private double _X;
        public double X
        {
            get => _X;
            set => SetProperty(ref _X, value);
        }

        private double _Y;
        public double Y
        {
            get => _Y;
            set => SetProperty(ref _Y, value);
        }

        private double _Z;
        public double Z
        {
            get => _Z;
            set => SetProperty(ref _Z, value);
        }

        private double _A;
        public double A
        {
            get => _A;
            set => SetProperty(ref _A, value);
        }

        private double _B;
        public double B
        {
            get => _B;
            set => SetProperty(ref _B, value);
        }

        private double _C;
        public double C
        {
            get => _C;
            set => SetProperty(ref _C, value);
        }

        private double _U;
        public double U
        {
            get => _U;
            set => SetProperty(ref _U, value);
        }

        private double _V;
        public double V
        {
            get => _V;
            set => SetProperty(ref _V, value);
        }

        private double _W;
        public double W
        {
            get => _W;
            set => SetProperty(ref _W, value);
        }
    }
}
