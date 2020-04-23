using Light.GuardClauses;
using ControllerService.Gcode.OffsetCalculators;
using ControllerService.GCode;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Prism.Events;
using Prism.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerService.Modules
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Типы, владеющие высвобождаемыми полями, должны быть высвобождаемыми", Justification = "<Ожидание>")]
    public class PmacProgramLoader : IProgramLoader
    {
        readonly ICodeCalculator _codeCalculator;
        private readonly IEventAggregator _eventAggregator;
        readonly IController _controller;
        readonly IControllerConfigurator _controllerConfigurator;
        private bool _isProgramOpened;
        private bool _isProgramPaused;
        private bool _isProgramRunned;
        readonly ILoggerFacade _logger;
        private NCFile _programFile;
        private int _programStringNumber;
        private CancellationTokenSource _rotaryBufferCancellationToken;
        readonly IUserSettingsService _settings;
        private const int COUNT_OF_LINES_TO_LOAD = 600;

        public PmacProgramLoader(IController controller, IUserSettingsService settings, ILoggerFacade logger, IControllerConfigurator controllerConfigurator, ICodeCalculator codeCalculator, IEventAggregator eventAggregator)
        {
            _codeCalculator = codeCalculator;
            _eventAggregator = eventAggregator;
            _settings = settings;
            _controllerConfigurator = controllerConfigurator;
            _logger = logger;
            _controller = controller;
            CurrentState = ProgramLoaderState.NotRunning;
        }

        #region auto
        public async Task<string[]> OpenProgramFileAsync(string programPath, int selectedLine, CancellationToken cancellationToken)
        {
            if (_programFile != null)
                _programFile.Dispose();

            LoadedProgram.Clear();
            StartLine = selectedLine;
            await InitNewRotaryBuffer().ConfigureAwait(false);
            return await Task.Run(() =>
            {
                _programFile = new NCFile(programPath, true);
                IsProgramOpened = true;
                var startLine = selectedLine;
                var endLine = selectedLine + 16;
                if (_programFile.StringCount > 0)
                {
                    var test = _programFile.StringCount;
                    return _programFile.GetSomeString(selectedLine, _programFile.StringCount - endLine > 16 ? endLine : _programFile.StringCount).Replace("\r", string.Empty).Split('\n');
                }
                else
                    return null;
            }, cancellationToken).ConfigureAwait(false);
        }
        public async Task<string[]> OpenProgramFileAsync(string programPath, CancellationToken cancellationToken)
        {
            if (_programFile != null)
                _programFile.Dispose();

            LoadedProgram.Clear();
            await InitNewRotaryBuffer().ConfigureAwait(false);
            return await Task.Run(() =>
            {
                _programFile = new NCFile(programPath, true);
                IsProgramOpened = true;
                if (_programFile.StringCount > 0)
                {
                    var test = _programFile.StringCount;
                    return _programFile.GetSomeString(0, _programFile.StringCount > 16 ? 16 : _programFile.StringCount).Replace("\r", string.Empty).Split('\n');
                }
                else
                    return null;
            }, cancellationToken).ConfigureAwait(false);
        }
        public bool StartProgram(int line = 0)
        {
            CurrentState = ProgramLoaderState.Auto;
            return StartLoadProgramGeneral(DoCheckProgramStringNumber);
        }
        private bool StartLoadProgramGeneral(Action runner)
        {
            bool result = _controller.Connected && _controller.DPRAvailable();
            if (result)
            {
                if (_rotaryBufferCancellationToken != null)
                    _rotaryBufferCancellationToken.Cancel();
                _rotaryBufferCancellationToken = new CancellationTokenSource();
                StartLoadProgram(runner, _rotaryBufferCancellationToken.Token);
                result = true;
            }
            return result;
        }
        public async Task InitNewRotaryBuffer(bool mdi = false)
        {
            _eventAggregator.GetEvent<MachineLockedState>().Publish(true);
            IsProgramRunning = false;

            if (!mdi)
                ReadyToRun = false;

            await Task.Run(() =>
            {
                ClearStates();
                _controller.GetResponse("A", out string result);
                _controller.GetResponse($"{MVariables.ProgramLineNumberFirst}=0", out result);
                _controller.GetResponse($"{MVariables.ProgramLineNumberSecond}=0", out result);
                _controller.GetResponse("DELETE LOOKAHEAD", out result);
                _controller.GetResponse("DELETE ROT", out result);
                _controller.RotBufRemove(0);
                _controller.GetResponse("&1 DEFINE ROT 4096", out result);
                _controller.GetResponse("&1 DEFINE LOOKAHEAD 500,5", out result);
                _controller.RotBufClear(0);
                Task.Delay(100).Wait();
                _controller.RotBufSet(false);
                _controller.RotBufSet(true);
                _controller.RotBufInit();
                _settings.CSNumber = 0;

                if (!mdi)
                {
                    StartProgram();
                    while (!ReadyToRun)
                    {
                        Task.Delay(200).Wait();
                    }
                }

            }).ConfigureAwait(false);
            _eventAggregator.GetEvent<MachineLockedState>().Publish(false);
        }
        public async Task PrepareProgramAsync()
        {
            _eventAggregator.GetEvent<MachineLockedState>().Publish(true);
            IsProgramRunning = false;

            await Task.Run(() =>
            {
                ClearStates();
                _controller.GetResponse($"{MVariables.ProgramLineNumberFirst}=0", out string result);
                _controller.GetResponse($"{MVariables.ProgramLineNumberSecond}=0", out result);
                _settings.CSNumber = 0;
                _controller.GetResponse("ABR0", out result);

            }).ConfigureAwait(false);
            _eventAggregator.GetEvent<MachineLockedState>().Publish(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2008:Не создавайте задачи без передачи TaskScheduler", Justification = "<Ожидание>")]
        private void DoCheckProgramStringNumber()
        {
            var child = Task.Factory.StartNew(DoRotLoad, TaskCreationOptions.AttachedToParent, _rotaryBufferCancellationToken.Token);
            child.ContinueWith(WhenRotaryBufferLoadingIsFall, TaskContinuationOptions.OnlyOnFaulted);
            while (!child.IsCompleted)
            {
                _controllerConfigurator.GetVariable(MVariables.ProgramLineNumberFirst, out int num);
                ProgramStringNumber = num;
                Task.Delay(100).Wait();
            }
        }
        private void DoRotLoad(object obj)
        {
            CoordinateSystemNumber = 0;
            int currentStringNumber = 0; // Номер текущей выполняемой строки
            int loadedStringNumber = StartLine; // Номер загружаемой строки
            int currentToolNumber = 0;
            _programFile.MustNotBeNullReference(nameof(_programFile));
            _programFile.StringCount.MustNotBe(0, nameof(_programFile.StringCount));
            PrimaryLoadNStrings(ref loadedStringNumber, ref currentToolNumber, loadedStringCount: loadedStringNumber + COUNT_OF_LINES_TO_LOAD);
            ReadyToRun = true;
            _eventAggregator.GetEvent<ProgramDoneToRunEvent>().Publish();
            while (loadedStringNumber < _programFile.StringCount)
            {
                _controllerConfigurator.GetVariable(MVariables.ProgramLineNumberFirst, out currentStringNumber);
                SecondaryLoadNStrings(ref loadedStringNumber, currentStringNumber, currentToolNumber);
                if (loadedStringNumber == _programFile.StringCount) //Программа закончилась
                    break;
                if (CheckCancelProgramRequest())
                    return;

                Task.Delay(100).Wait();
            }
            int coordinateSystemInPosition;
            int isProgramRunning;
            do
            {
                _controllerConfigurator.GetVariable($"M5187", out coordinateSystemInPosition);
                _controllerConfigurator.GetVariable($"M5180", out isProgramRunning);
                _controllerConfigurator.GetVariable(MVariables.ProgramLineNumberFirst, out currentStringNumber);
                if (CheckCancelProgramRequest())
                    return;

                Task.Delay(100).Wait();
            }
            while (currentStringNumber < loadedStringNumber || coordinateSystemInPosition == 0 && isProgramRunning == 1); //Ожидание окончания программы
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            StartLine = 0;
            PrepareProgramAsync().Wait();
        }
        private void PrimaryLoadNStrings(ref int loadedString, ref int currentToolNumber, int loadedStringCount)
        {
            while (loadedString < loadedStringCount && loadedString < _programFile.StringCount)
            {
                string sourceString = GCodeNormalizer.RemoveComments(_programFile.GetClearSomeString(loadedString, 1).Replace(Environment.NewLine, string.Empty));
                FindCoordinateSystemChangingCode(sourceString);
                if (GetTCode(sourceString).HasValue)
                    currentToolNumber = GetTCode(sourceString).Value - 1;
                int frameNumber = loadedString + 1;
                LoadedProgram.AddOrUpdate(loadedString + 1, $"N{frameNumber} {sourceString}", (key, val) => $"N{frameNumber} {sourceString}");
                var tempStr = ApplyOffsets($"{MVariables.ProgramLineNumberFirst}=={frameNumber}{MVariables.ProgramLineNumberSecond}={frameNumber}{sourceString}", CoordinateSystemNumber, currentToolNumber, MachineType);
                if (!_controller.RotBufSendString(tempStr, 0))
                    return;
                loadedString++;
            }
        }
        private void SecondaryLoadNStrings(ref int loadedStringNumber, int currentStringNumber, int currentToolNumber)
        {
            while ((loadedStringNumber - currentStringNumber) < 30)
            {
                _controllerConfigurator.GetVariable(MVariables.ProgramLineNumberFirst, out currentStringNumber);
                if (loadedStringNumber == _programFile.StringCount) //Программа закончилась
                    break;
                string sourceString = GCodeNormalizer.RemoveComments(_programFile.GetClearSomeString(loadedStringNumber, 1).Replace(Environment.NewLine, string.Empty));
                FindCoordinateSystemChangingCode(sourceString);
                if (GetTCode(sourceString).HasValue)
                    currentToolNumber = GetTCode(sourceString).Value - 1;
                var frameNumber = loadedStringNumber + 1;
                LoadedProgram.AddOrUpdate(frameNumber, $"N{frameNumber} {sourceString}", (key, val) => $"N{frameNumber} {sourceString}");
                var tempStr = ApplyOffsets($"{MVariables.ProgramLineNumberFirst}=={frameNumber}{MVariables.ProgramLineNumberSecond}={frameNumber}{sourceString}", CoordinateSystemNumber, currentToolNumber, MachineType);
                _controller.RotBufSendString(tempStr, 0);
                loadedStringNumber++;
            }
        }
        public void CycleStart()
        {
            IsProgramRunning = true;
            _controllerConfigurator.SetVariable(PVariables.PBStart, 1);
        }
        public void AbortProgram()
        {
            ClearStates();
            _rotaryBufferCancellationToken?.Cancel();
            StartLine = 0;
            PrepareProgramAsync().Wait();
        }
        public async Task AbortProgramAsync()
        {
            ClearStates();
            _rotaryBufferCancellationToken?.Cancel();
            StartLine = 0;
            await PrepareProgramAsync();
        }
        private static void StartLoadProgram(Action action, CancellationToken token)
        {
            if (token != null)
                Task.Run(action, token);
            else
                Task.Run(action);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        public void PauseProgram()
        {
            var result = _controller.GetResponse("Q", out string res);
            if (result)
                IsProgramPaused = true;
            if (!result || res.ToUpper(CultureInfo.InvariantCulture).Contains("ERR"))
                _logger.Log("Pausing error", Category.Warn, Priority.High);
        }
        public void ResetProgram()
        {

            ClearStates();
            IsProgramOpened = false;
            OnProgramReseted?.Invoke(this, new EventArgs());
            _controller.GetResponse("A K", out _);
            if (IsProgramRunning)
                AbortProgram();
            _programFile?.Dispose();
        }
        public void ResumeProgram()
        {
            if (!IsProgramPaused)
                return;
            _controller.GetResponse("R", out _);
            IsProgramPaused = false;
        }
        public void ExitFromAuto()
        {
            ClearStates();
            _controller.GetResponse("A K", out _);
            _rotaryBufferCancellationToken?.Cancel();
            if (IsProgramRunning)
            {
                ClearStates();
                StartLine = 0;
            }
            CurrentState = ProgramLoaderState.NotRunning;
        }
        public string GetProgramLine(int line, int count) => _programFile.GetClearSomeString(line, count);
        #endregion

        #region mdi
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        public void LoadMDIProgram(IEnumerable<string> programStrings)
        {
            if (programStrings == null) throw new NullReferenceException();
            if (!(_controller.Connected && _controller.DPRAvailable()))
            {
                var errorMessage = _controller.Connected ? "Dpr is not available." : "Controller is not connected.";
                _logger.Log($"Program can't run. {errorMessage}", Category.Warn, Priority.High);
            }
            else
            {
                List<string> normalizedProgramStrings = new List<string>();
                foreach (string programString in programStrings)
                {
                    normalizedProgramStrings.Add(GCodeNormalizer.NormalizeString(programString));
                }
                StartMdiProgram(normalizedProgramStrings);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2008:Не создавайте задачи без передачи TaskScheduler", Justification = "<Ожидание>")]
        private void StartMdiProgram(List<string> normalizedProgramStrings)
        {
            if (_rotaryBufferCancellationToken != null)
                _rotaryBufferCancellationToken.Cancel();
            _rotaryBufferCancellationToken = new CancellationTokenSource();

            Task.Run(async () => await InitNewRotaryBuffer(true).ConfigureAwait(false), _rotaryBufferCancellationToken.Token)
                .ContinueWith((t) => DoMdiProgramLoad(normalizedProgramStrings), _rotaryBufferCancellationToken.Token);
        }
        //private async Task PrepareMdiProgram()
        //{
        //    _eventAggregator.GetEvent<MachineLockedState>().Publish(true);
        //    IsProgramRunning = false;
        //    await Task.Run(() =>
        //    {
        //        ClearStates();
        //        _controller.GetResponse("A", out string result);
        //        _controller.GetResponse($"{MVariables.ProgramLineNumberFirst}=0", out result);
        //        _controller.GetResponse($"{MVariables.ProgramLineNumberSecond}=0", out result);
        //        _controller.GetResponse("DELETE LOOKAHEAD", out result);
        //        _controller.GetResponse("DELETE ROT", out result);
        //        _controller.RotBufRemove(0);
        //        _controller.GetResponse("&1 DEFINE ROT 4096", out result);
        //        _controller.GetResponse("&1 DEFINE LOOKAHEAD 500,5", out result);
        //        _controller.RotBufClear(0);
        //        Task.Delay(100).Wait();
        //        _controller.RotBufSet(false);
        //        _controller.RotBufSet(true);
        //        _controller.RotBufInit();
        //        _settings.CSNumber = 0;
        //    }).ConfigureAwait(false);
        //    _eventAggregator.GetEvent<MachineLockedState>().Publish(false);
        //}
        private void DoMdiProgramLoad(IEnumerable<string> mdiProgram)
        {
            CurrentState = ProgramLoaderState.Mdi;
            IsProgramRunning = true;
            int stringCount = mdiProgram.Count();
            stringCount.MustNotBe(0, nameof(mdiProgram));
            int frameNumber = 1;
            int currentToolNumber = 0;
            foreach (var programString in mdiProgram)
            {
                FindCoordinateSystemChangingCode(programString);
                if (GetTCode(programString).HasValue)
                    currentToolNumber = GetTCode(programString).Value - 1;
                var tempStr = ApplyOffsets($"{MVariables.ProgramLineNumberFirst}=={frameNumber}{MVariables.ProgramLineNumberSecond}={frameNumber}{programString}", CoordinateSystemNumber, currentToolNumber, MachineType);
                _controller.RotBufSendString(tempStr, bufNum: 0);
                frameNumber++;
            }
            _controllerConfigurator.SetVariable(PVariables.PBStart, 1);
            do
            {
                if (_controller.GetResponse(MVariables.ProgramLineNumberFirst, out string strCurrentStringNumber))
                    ProgramStringNumber = int.Parse(strCurrentStringNumber, CultureInfo.InvariantCulture);
                CheckCancelProgramRequest();
                Task.Delay(200).Wait();
            }
            while (ProgramStringNumber < stringCount && !_rotaryBufferCancellationToken.IsCancellationRequested); //Ожидание окончания программы
            Task.Delay(4000).Wait();
            InitNewRotaryBuffer(true).Wait(); // вместо ClearBuffer
            IsProgramRunning = false;
            CurrentState = ProgramLoaderState.NotRunning;
        }
        public void AbortMdiProgram()
        {
            _rotaryBufferCancellationToken.Cancel();
            _controllerConfigurator.SetVariable(PVariables.PBStop, 1);
        }
        //private async Task CloseBuffer()
        //{
        //    _eventAggregator.GetEvent<MachineLockedState>().Publish(true);
        //    await Task.Run(() =>
        //    {
        //        ClearStates();
        //        _controller.GetResponse("A", out string result);
        //        _controller.GetResponse($"{MVariables.ProgramLineNumberFirst}=0", out result);
        //        _controller.GetResponse($"{MVariables.ProgramLineNumberSecond}=0", out result);
        //        _controller.GetResponse("DELETE LOOKAHEAD", out result);
        //        _controller.GetResponse("DELETE ROT", out result);
        //        _controller.RotBufRemove(0);
        //        _controller.GetResponse("&1 DEFINE ROT 4096", out result);
        //        _controller.GetResponse("&1 DEFINE LOOKAHEAD 500,5", out result);
        //        _controller.RotBufClear(0);
        //        Task.Delay(100).Wait();
        //        _controller.RotBufSet(false);
        //    }).ConfigureAwait(false);
        //    _eventAggregator.GetEvent<MachineLockedState>().Publish(false);
        //}

        #endregion

        private string ApplyOffsets(string programString, int coordinateSystemNumber, int toolNumber, int machineType)
        {
            return MachineOffsetCalculator.ApplyOffsets(programString, coordinateSystemNumber, toolNumber, (MachineType)machineType, _settings);
        }
        private bool CheckCancelProgramRequest()
        {
            if (!_rotaryBufferCancellationToken.IsCancellationRequested)
                return false;
            bool canceledOk;
            //string abortResult;
            do
            {
                canceledOk = _controllerConfigurator.SetVariable(PVariables.PBStop, 1);
                //canceledOk = _controller.GetResponse("A K", out abortResult);
            } while (!canceledOk /*&& !string.IsNullOrEmpty(abortResult)*/);
            IsProgramRunning = false;
            return true;
        }
        private void ClearStates()
        {
            IsProgramPaused = false;
            IsProgramRunning = false;
        }
        private void FindCoordinateSystemChangingCode(string sourceString)
        {
            if (sourceString.Contains("G54"))
                CoordinateSystemNumber = 0;
            if (sourceString.Contains("G55"))
                CoordinateSystemNumber = 1;
            if (sourceString.Contains("G56"))
                CoordinateSystemNumber = 2;
            if (sourceString.Contains("G57"))
                CoordinateSystemNumber = 3;
            if (sourceString.Contains("G58"))
                CoordinateSystemNumber = 4;
            if (sourceString.Contains("G59"))
                CoordinateSystemNumber = 5;
        }
        private void WhenRotaryBufferLoadingIsFall(Task task)
        {
            IsProgramRunning = false;
            foreach (var item in task.Exception.InnerExceptions)
            {
                _logger.Log($"Exception: {item.GetType().Name} Message: {item.Message}. Stack: {item.StackTrace}", Category.Exception, Priority.High);
            }
        }
        public int? GetTCode(string programString)
        {
            return _codeCalculator.GetTCode(programString);
        }
        private bool SetProperty<T>(ref T field, T value, [CallerMemberName]string callerName = "")
        {
            if (EqualityComparer<T>.Default.Equals(value, field))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));
            return true;
        }

        public int CoordinateSystemNumber
        {
            get => _settings.CSNumber;
            set
            {
                _settings.CSNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoordinateSystemNumber)));
            }
        }
        public bool IsProgramOpened
        {
            get { return _isProgramOpened; }
            set { SetProperty(ref _isProgramOpened, value); }
        }
        public bool IsProgramPaused
        {
            get => _isProgramPaused;
            set => SetProperty(ref _isProgramPaused, value);
        }
        public bool IsProgramRunning
        {
            get => _isProgramRunned;
            private set => SetProperty(ref _isProgramRunned, value);
        }
        public int StartLine
        {
            get; set;
        }
        public ConcurrentDictionary<int, string> LoadedProgram { get; } = new ConcurrentDictionary<int, string>();
        public int MachineType
        {
            get => _settings.MachineType;
            set => _settings.MachineType = value;
        }
        public int ProgramStringNumber
        {
            get => _programStringNumber;
            private set => SetProperty(ref _programStringNumber, value);
        }
        public bool ReadyToRun { get; private set; }
        public ProgramLoaderState CurrentState { get; private set; }

        public event EventHandler OnProgramReseted = delegate { };
        public event PropertyChangedEventHandler PropertyChanged;
    }
}