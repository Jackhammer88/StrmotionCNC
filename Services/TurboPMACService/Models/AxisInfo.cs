using Infrastructure.Interfaces.CNCControllerService;
using Prism.Mvvm;

namespace ControllerService.Models
{
    public class AxisInfo : BindableBase, IAxisInfo
    {

        private string _AxisName;
        public string AxisName
        {
            get => _AxisName;
            set => SetProperty(ref _AxisName, value);
        }

        private int _motorNumber;
        public int MotorNumber
        {
            get => _motorNumber;
            set => SetProperty(ref _motorNumber, value);
        }


        private double _AxisPosition;
        public double AxisPosition
        {
            get => _AxisPosition;
            set => SetProperty(ref _AxisPosition, value);
        }


        private double _AxisLoadPercentage;
        public double AxisLoadPercentage
        {
            get => _AxisLoadPercentage;
            set => SetProperty(ref _AxisLoadPercentage, value);
        }


        private bool _InPositionTrue;
        public bool InPositionTrue
        {
            get => _InPositionTrue;
            set => SetProperty(ref _InPositionTrue, value);
        }
    }
}