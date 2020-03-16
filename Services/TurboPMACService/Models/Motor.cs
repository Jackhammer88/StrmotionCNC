using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Prism.Mvvm;

namespace ControllerService.Models
{
    public class Motor : BindableBase, IMotor
    {

        private bool _Activated;
        private bool _AmplifierFaultError;
        private string _axisName;
        private double _currentPercentage;
        private bool _fatalFollowing;
        private bool _inPosition;
        private string _letter;
        private int _letterFactor;
        private int _phaseA;
        private int _phaseB;
        private bool _phaseRequest;
        private bool _phasingSearchError;
        private double _position;
        private double _plotPosition;
        private double _followingError;
        private bool _warningFollowing;
        private double _rawPosition;

        public void SetXStatuses(MotorXStatuses statuses)
        {
            XStatuses = statuses;
        }
        public void SetYStatuses(MotorYStatuses statuses)
        {
            YStatuses = statuses;
        }

        public bool Activated
        {
            get => _Activated;
            set => SetProperty(ref _Activated, value);
        }
        public bool AmplifierFaultError
        {
            get => _AmplifierFaultError;
            set => SetProperty(ref _AmplifierFaultError, value);
        }
        public string AxisName
        {
            get { return _axisName; }
            set { SetProperty(ref _axisName, value); }
        }
        public double CurrentPercentage
        {
            get => _currentPercentage;
            set => SetProperty(ref _currentPercentage, value);
        }
        public bool FatalFollowing
        {
            get => _fatalFollowing;
            set => SetProperty(ref _fatalFollowing, value);
        }
        public bool InPosition
        {
            get => _inPosition;
            set => SetProperty(ref _inPosition, value);
        }
        public string Letter
        {
            get => _letter;
            set => SetProperty(ref _letter, value);
        }
        public int LetterFactor
        {
            get => _letterFactor;
            set => SetProperty(ref _letterFactor, value);
        }
        public int PhaseA
        {
            get => _phaseA;
            set => SetProperty(ref _phaseA, value);
        }
        public int PhaseB
        {
            get => _phaseB;
            set => SetProperty(ref _phaseB, value);
        }
        public bool PhaseRequest
        {
            get => _phaseRequest;
            set => SetProperty(ref _phaseRequest, value);
        }
        public bool PhasingSearchError
        {
            get => _phasingSearchError;
            set => SetProperty(ref _phasingSearchError, value);
        }
        public double Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }
        public double PlotPosition
        {
            get => _plotPosition;
            set => SetProperty(ref _plotPosition, value);
        }
        public bool WarningFollowing
        {
            get => _warningFollowing;
            set => SetProperty(ref _warningFollowing, value);
        }

        public double FollowingError
        {
            get { return _followingError; }
            set { SetProperty(ref _followingError, value); }
        }

        public double RawPosition
        {
            get => _rawPosition;
            set => SetProperty(ref _rawPosition, value);
        }

        public MotorXStatuses XStatuses { get; private set; }

        public MotorYStatuses YStatuses { get; private set; }
    }
}