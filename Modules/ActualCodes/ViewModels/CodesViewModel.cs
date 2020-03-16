using ActualCodes.CodeCreator;
using Infrastructure.AggregatorEvents;
using Infrastructure.Interfaces.CNCControllerService;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ActualCodes.ViewModels
{
    public class CodesViewModel : BindableBase
    {
        private IEventAggregator _aggregator;
        readonly IProgramLoader _programLoader;
        readonly ICodeCalculator _codeCalculator;
        private int _lastRunnedString;
        private Dictionary<int, double?> _gGroups = GGroupCreator.Create();
        private Dictionary<int, int?> _mGroup = MGroupCreator.Create();
        private string _tCode;
        private string _mCode;
        private string _gCode1;
        private string _gCode2;
        private string _gCode3;
        private bool _connectedToController;

        public CodesViewModel(IEventAggregator aggregator, IProgramLoader programLoader, ICodeCalculator codeCalculator)
        {
            _codeCalculator = codeCalculator;
            _programLoader = programLoader == null ? throw new NullReferenceException() : programLoader;
            _aggregator = aggregator;
            _aggregator?.GetEvent<ConnectionEvent>().Subscribe(ChangeConnectionState);

            _programLoader.PropertyChanged += OnPropertyChanged;
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null) throw new NullReferenceException();
            if (e.PropertyName.Equals(nameof(_programLoader.IsProgramRunning), StringComparison.Ordinal) && _programLoader.IsProgramRunning)
            {
                ResetCodes();
                _lastRunnedString = 1;
            }
            if (e.PropertyName.Equals(nameof(_programLoader.ProgramStringNumber), StringComparison.Ordinal))
            {
                GetAroundEachCode();
                SetGcodesToUI();
                SetMCodeToUI();
            }
        }
        private void GetAroundEachCode()
        {
            for (int i = _lastRunnedString; i <= _programLoader.ProgramStringNumber; i++, _lastRunnedString++)
            {
                if (_programLoader.LoadedProgram.TryGetValue(i, out string str))
                {
                    var tCode = _codeCalculator.GetTCode(str);
                    if (tCode.HasValue)
                        TCode = $"T{tCode}";
                    _codeCalculator.GetGCodes(str, _gGroups);
                    _codeCalculator.GetMCodes(str, _mGroup);
                }
            }
        }
        private void SetMCodeToUI()
        {
            MCode = string.Empty;
            foreach (var item in _mGroup)
            {
                if (item.Value.HasValue)
                    MCode += $"M{item.Value.Value} ";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void SetGcodesToUI()
        {
            int gCodesCount = 0;
            GCode1 = GCode2 = GCode3 = string.Empty;
            foreach (var item in _gGroups)
            {
                if (item.Value.HasValue)
                    switch (gCodesCount)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            GCode1 += $"G{item.Value.Value} ";
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            GCode2 += $"G{item.Value.Value} ";
                            break;
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            GCode3 += $"G{item.Value.Value} ";
                            break;
                        default:
                            throw new Exception("G коды не поместились");
                    }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void SetGcodesToUI2()
        {
            int gCodesCount = 0;
            GCode1 = GCode2 = GCode3 = string.Empty;
            foreach (var item in _gGroups)
            {
                if (!item.Value.HasValue) continue;
                switch (gCodesCount)
                {
                    case int gc when gc >= 0 && gc <= 3:
                        GCode1 += $"G{item.Value.Value} ";
                        break;
                    case int gc when gc >= 4 && gc >= 7:
                        GCode2 += $"G{item.Value.Value} ";
                        break;
                    case int gc when gc >= 8 && gc <= 11:
                        GCode3 += $"G{item.Value.Value} ";
                        break;
                    default:
                        throw new Exception("G коды не поместились");
                }
                gCodesCount++;
            }
        }
        private void ChangeConnectionState(bool isConnected)
        {
            ConnectedToController = isConnected;
        }
        private void ResetCodes()
        {
            _gGroups = GGroupCreator.Create();
            _mGroup = MGroupCreator.Create();
            TCode = string.Empty;
        }

        public string TCode
        {
            get => _tCode;
            set => SetProperty(ref _tCode, value);
        }
        public string MCode
        {
            get => _mCode;
            set => SetProperty(ref _mCode, value);
        }
        public string GCode1
        {
            get => _gCode1;
            set => SetProperty(ref _gCode1, value);
        }
        public string GCode2
        {
            get => _gCode2;
            set => SetProperty(ref _gCode2, value);
        }
        public string GCode3
        {
            get => _gCode3;
            set => SetProperty(ref _gCode3, value);
        }

        public bool ConnectedToController
        {
            get => _connectedToController;
            set => SetProperty(ref _connectedToController, value);
        }
    }
}
