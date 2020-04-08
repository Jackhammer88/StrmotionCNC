using ControllerService.Models;
using Infrastructure.AppEventArgs;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Infrastructure.SharedClasses;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerService.Modules
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Типы, владеющие высвобождаемыми полями, должны быть высвобождаемыми", Justification = "<Ожидание>")]
    public class PmacInformation : IControllerInformation
    {
        readonly IController _controller;
        readonly IUserSettingsService _settings;
        readonly ILoggerFacade _logger;
        private int _motorCount;
        private CancellationTokenSource _cancToken;
        private Task _motorCheck;
        private bool _pollingStarted;
        private GlobalStatusXs _statusX;
        private GlobalStatusYs _statusY;

        public PmacInformation(IController controller, IUserSettingsService settings, ILoggerFacade logger)
        {
            _logger = logger;
            _settings = settings;
            _controller = controller ?? throw new NullReferenceException();

            StatusManager = new StatusHandler();
            _controller.OnConnect += _controller_OnConnect;
            _controller.OnDisconnected += _controller_OnDisconnected;
            if (_controller.Connected && !_pollingStarted)
                StartMotorPolling();
        }

        private void _controller_OnDisconnected(object sender, EventArgs e)
        {
            StopMotorPolling();
        }
        private void _controller_OnConnect(object sender, EventArgs e)
        {
            StartMotorPolling();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2008:Не создавайте задачи без передачи TaskScheduler", Justification = "<Ожидание>")]
        private void StartMotorPolling()
        {
            _pollingStarted = true;
            GetMotorsCount();
            if (_cancToken != null)
                _cancToken.Cancel();
            _cancToken = new CancellationTokenSource();
            _motorCheck = new Task(DoPollMotors, _cancToken.Token);
            _motorCheck.ContinueWith(WhenMotorCheckingFall, TaskContinuationOptions.OnlyOnFaulted);
            _motorCheck.Start();
        }
        private void StopMotorPolling()
        {
            if (_cancToken != null)
                _cancToken.Cancel();
            _pollingStarted = false;
        }
        private void DoPollMotors()
        {
            while (!_cancToken.IsCancellationRequested)
            {
                //Global statuses
                if (_controller.GetGlobalStatuses(out GlobalStatusXs globalStatusX, out GlobalStatusYs globalStatusY))
                {
                    StatusX = globalStatusX;
                    StatusY = globalStatusY;
                }
                //Опрос моторов
                for (int i = 0; i < _motorCount; i++)
                {
                    var motor = (Motor)Motors[i];
                    double axisScale = 1;
                    //Motors statuses
                    if (_controller.GetMotorStatus(i, out MotorXStatuses statusesX, out MotorYStatuses statusesY)) SetMotorBits(ref motor, statusesX, statusesY);
                    //Current
                    if (_controller.GetMotorCurrent(i, out int phaseA, out int phaseB)) SetMotorCurrent(ref motor, phaseA, phaseB);
                    //Letter
                    if (_controller.GetResponse($"#{i + 1}->", out string motorBinding)) GetMotorLetterAndScale(motor, ref axisScale, motorBinding);
                    //Position
                    if (!string.IsNullOrEmpty(motor.Letter))
                    {
                        var csOffset = GetCSOffsets(motor.Letter);
                        _controller.GetMotorPosition(i, out double rawPosition, PositionType.ActualPosition);
                        motor.RawPosition = rawPosition / axisScale;
                        motor.PlotPosition = (rawPosition / axisScale) - csOffset;
                        _controller.GetMotorPosition(i, out double position, SelectedPositionType);
                        motor.Position = (position / axisScale) - ((SelectedPositionType == PositionType.ActualPosition) ? csOffset : 0);
                    }
                    //Following error
                    if (_controller.GetResponse($"f{i + 1}", out string followingErrorAnswer)) motor.FollowingError = double.Parse(followingErrorAnswer, CultureInfo.InvariantCulture);
                }

                var statusHandler = (StatusHandler)StatusManager;
                statusHandler.StatusesUpdated(StatusX, StatusY, Motors.Select(m => new Tuple<MotorXStatuses, MotorYStatuses>(m.XStatuses, m.YStatuses)));

                //Ожидаем заданное время
                Task.Delay(_settings.Timeout).Wait();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void HasTroubles(GlobalStatusXs globalStatusX)
        {
            if (globalStatusX.HasFlag(GlobalStatusXs.DPRamError))
            {
                _logger.Log($"Dpram error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.EAROMError))
            {
                _logger.Log($"EAROM error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.MacroCommunicationError))
            {
                _logger.Log($"Macro communication error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.MacroRingError))
            {
                _logger.Log($"Macro ring error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.MainError))
            {
                _logger.Log($"Main error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.PhaseClockMissing))
            {
                _logger.Log($"Phase clock missing", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.RTIReEntryError))
            {
                _logger.Log($"RTI reentry error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.ServoMacroICConfigError))
            {
                _logger.Log($"Servo macro IC config error", Category.Warn, Priority.High);
            }
            if (globalStatusX.HasFlag(GlobalStatusXs.TWSVariableParityError))
            {
                _logger.Log($"TWS variable parity error ", Category.Warn, Priority.High);
            }
        }

        private double GetCSOffsets(string letter)
        {
            var props = _settings.Offsets[_settings.CSNumber].GetType().GetProperties();
            var prop = props.First(o => o.Name.Contains(letter));
            return (double)prop.GetValue(_settings.Offsets[_settings.CSNumber]);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void GetMotorLetterAndScale(IMotor motor, ref double axisScale, string motorBinding)
        {
            if (!motorBinding.Equals("0", StringComparison.Ordinal))
            {
                var strAxisScale = new string(motorBinding.Where(c => char.IsDigit(c) || c == '.').ToArray());
                axisScale = double.Parse(string.IsNullOrEmpty(strAxisScale) ? "1" : strAxisScale, CultureInfo.InvariantCulture);
            }
            if (axisScale == 0) throw new ArgumentException("Ошибка при определении масштабирования оси.");
            string letter = new string(motorBinding.Where(c => char.IsLetter(c)).ToArray());
            if (motor.Letter == null || !motor.Letter.Equals(letter, StringComparison.Ordinal))
            {
                motor.Letter = letter;
                ActivatedMotorsCountChanged(this, new EventArgs());
            }
        }
        private void GetMotorsCount()
        {
            if (_controller.I7XX0[0] > 0 && _controller.I7XX0[1] > 0) //8 моторов доступно
                _motorCount = 8;
            else if (_controller.I7XX0[0] > 0 && _controller.I7XX0[1] == 0)
                _motorCount = 4;
            Motors.Clear();
            for (int i = 0; i < _motorCount; i++)
                Motors.Add(new Motor());
            MotorCountChanged?.Invoke(this, EventArgs.Empty);
        }
        private void WhenMotorCheckingFall(Task motorChekingTask)
        {
            _logger.Log($"Error. Motor checking task has crashed. {motorChekingTask.Exception}", Category.Exception, Priority.High);
        }
        private void SetMotorCurrent(ref Motor motor, int phaseA, int phaseB)
        {
            motor.PhaseA = phaseA;
            motor.PhaseB = phaseB;
            motor.CurrentPercentage = (double)(Math.Abs(phaseA)) / (double)_settings.MaxCurrentLimit * 100; //Вычисляем процент от максимального тока нагрузки                            
        }
        private void SetMotorBits(ref Motor motor, MotorXStatuses statusesX, MotorYStatuses statusesY)
        {
            var activatedMotorStatus = statusesX.HasFlag(MotorXStatuses.MotorActivated);
            if (motor.Activated != activatedMotorStatus)
                ActivatedMotorsCountChanged(this, new EventArgs());
            motor.Activated = activatedMotorStatus;
            motor.InPosition = statusesY.HasFlag(MotorYStatuses.InPositionTrue);
            motor.FatalFollowing = statusesY.HasFlag(MotorYStatuses.FatalFollowingErrorEx);
            motor.WarningFollowing = statusesY.HasFlag(MotorYStatuses.WarningFollowingErrorEx);
            motor.AmplifierFaultError = statusesY.HasFlag(MotorYStatuses.AmplifierFaultError);
            motor.PhasingSearchError = statusesY.HasFlag(MotorYStatuses.PhasingSearchError);
            motor.PhaseRequest = statusesY.HasFlag(MotorYStatuses.MotorPhaseRequest);
            motor.SetXStatuses(statusesX);
            motor.SetYStatuses(statusesY);
        }

        public GlobalStatusXs StatusX
        {
            get => _statusX;
            private set
            {
                if (_statusX != value)
                {
                    _statusX = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusX)));
                }
            }
        }
        public GlobalStatusYs StatusY
        {
            get => _statusY;
            private set
            {
                if (_statusY != value)
                {
                    _statusY = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusY)));
                }
            }
        }
        public int CoordinateSystemNumber { get; set; }
        public bool IsReconnectionStarted { get; set; }
        public int MachineType { get; set; }
        public List<IMotor> Motors { get; } = new List<IMotor>();
        public PositionType SelectedPositionType { get; set; }
        public IObservable<MachineStatuses> StatusManager { get; }

        public event EventHandler ActivatedMotorsCountChanged = delegate { };
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MotorCountChanged = delegate { };
    }
}
