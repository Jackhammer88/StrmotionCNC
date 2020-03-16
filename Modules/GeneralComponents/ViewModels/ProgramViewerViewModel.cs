using Infrastructure;
using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralComponents.ViewModels
{
    public class ProgramViewerViewModel : AutoViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private bool _isAutoStateSelected;
        private bool _isProgramNotRunning = true;
        private readonly ILoggerFacade _logger;
        private Dictionary<int, string> _programListing;
        private string _programText;
        private readonly IProgramLoader _programLoader;
        private readonly IRegionManager _regionManager;
        private int _stringNumber;
        private bool _canEditProgram;
        private bool _highlightLine;

        public ProgramViewerViewModel(IEventAggregator eventAggregator, IProgramLoader programLoader, IDialogService dialogService,
            IRegionManager regionManager, ILoggerFacade logger)
        {
            _logger = logger;
            _regionManager = regionManager;
            _dialogService = dialogService;
            _programLoader = programLoader ?? throw new ArgumentNullException(nameof(programLoader));
            CanEditProgram = true;

            Title = GeneralComponentsStrings.Auto;

            LoadCommand = new DelegateCommand<string>(LoadCommandExecute);
            EditCommand = new DelegateCommand(EditCommandExecute, EditCommandCanExecute);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _eventAggregator.GetEvent<MachineState>().Subscribe(ChangeStateCallback);
            _eventAggregator.GetEvent<ProgramDoneToRunEvent>().Subscribe(ProgramDoneToRunEventExecute);

            ProgramListing = new Dictionary<int, string>();

            _programLoader.PropertyChanged += ProgramLoader_PropertyChanged;
            _programLoader.OnProgramReseted += ProgramLoader_OnProgramReseted;
        }

        private void ProgramDoneToRunEventExecute()
        {
            var temp = _programLoader.LoadedProgram.Take(50).Select(p => p.Value).Aggregate((s1, s2) => $"{s1}\r\n{s2}");
            ProgramText = temp;
        }

        private void ProgramLoader_OnProgramReseted(object sender, EventArgs e)
        {
            ProgramListing = new Dictionary<int, string>();
            ProgramText = string.Empty;
            CanBeEdited = false;
            EditCommand.RaiseCanExecuteChanged();
            FileNameVisibility = System.Windows.Visibility.Collapsed;
        }
        private void ChangeStateCallback(MachineStateType state)
        {
            IsAutoStateSelected = state == MachineStateType.Auto;
            CanEditProgram = state != MachineStateType.Auto;
        }
        private bool EditCommandCanExecute()
        {
            return IsProgramNotRunning && CanBeEdited;
        }
        private void EditCommandExecute()
        {
            try
            {
                File.OpenWrite(FileName).Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Log($"Ошибка при открытии файла. Нет доступа. {ex.Message}", Category.Exception, Priority.High);
                return;
            }
            var navigationParameters = new NavigationParameters { { "filename", FileName } };
            _regionManager.RequestNavigate(RegionNames.AutoChildRegion, ViewNames.ProgramEditor, navigationParameters);
        }
        private void LoadCommandExecute(string arg)
        {
            try
            {
                _dialogService.ShowDialog("Open", null, (r) =>
                {
                    if (r.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    var path = r.Parameters.GetValue<string>("path");
                    OpenFile(path);
                    _eventAggregator.GetEvent<CncProgramLoadedEvent>().Publish(path);
                    CanBeEdited = true;
                    FileNameVisibility = System.Windows.Visibility.Visible;
                });
            }
            catch { throw; }
            EditCommand.RaiseCanExecuteChanged();
            //--Temporary code
        }
        /// <summary>
        /// Открывает программу сначала
        /// </summary>
        /// <param name="path"></param>
        private async void OpenFile(string path)
        {
            await OpenFileCommon(path).ConfigureAwait(false);
        }
        /// <summary>
        /// Открывает программу с выбранной строки
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedLine"></param>
        private async void OpenFile(string path, int selectedLine)
        {
            await OpenFileCommon(path, selectedLine).ConfigureAwait(false);
            _programLoader.StartLine = selectedLine;
        }

        private async Task OpenFileCommon(string path, int selectedLine = 0)
        {
            IsBusy = true;
            string[] programText;
            if (selectedLine == 0)
                programText = await _programLoader.OpenProgramFileAsync(path, selectedLine, CancellationToken.None).ConfigureAwait(false);
            else
                programText = await _programLoader.OpenProgramFileAsync(path, CancellationToken.None).ConfigureAwait(false);
            IsBusy = false;
            int pos = 0;
            var dict = programText.Where(s => !string.IsNullOrWhiteSpace(s)).ToDictionary(s => pos++, s => $"N{pos} {s}");
            ProgramListing = dict;
            FileName = path;

            //temporary
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var line in programText)
            {
                stringBuilder.AppendLine(line);
            }
            ProgramText = stringBuilder.ToString();
        }

        private void ProgramLoader_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(_programLoader.ProgramStringNumber), StringComparison.Ordinal))
            {
                RaisePropertyChanged(nameof(ProgramListing));
                RaisePropertyChanged(nameof(CurrentIndex));

                //Временно
                UpdateProgramText();
            }
            if (e.PropertyName.Equals(nameof(_programLoader.IsProgramRunning), StringComparison.Ordinal))
            {
                IsProgramNotRunning = !_programLoader.IsProgramRunning;
                EditCommand.RaiseCanExecuteChanged();

                //Временно
                HighlightLine = _programLoader.IsProgramRunning;
                UpdateProgramText();
            }
        }
        private void UpdateProgramText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var pair in ProgramListing)
            {
                stringBuilder.AppendLine(pair.Value);
            }
            ProgramText = stringBuilder.ToString();
            StringNumber = _programLoader.ProgramStringNumber;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext == null || navigationContext.Parameters == null || !navigationContext.Parameters.ContainsKey("file-changed"))
            {
                return;
            }

            var fileChanged = navigationContext.Parameters.GetValue<bool>("file-changed");

            if (navigationContext.Parameters.ContainsKey("selectedLine"))
                OpenFile(FileName, navigationContext.Parameters.GetValue<int>("selectedLine"));
            else if (fileChanged)
                OpenFile(FileName);
        }
        public void RemovePreviousStrings(int index)
        {
            Task.Run((Action)(() =>
            {
                var range = Enumerable.Where<KeyValuePair<int, string>>(this._programLoader.LoadedProgram, (Func<KeyValuePair<int, string>, bool>)(p => (p.Key < index)));
                Parallel.ForEach(range, (Action<KeyValuePair<int, string>>)((s) => this._programLoader.LoadedProgram.TryRemove(s.Key, out _)));
            }));
        }

        public int CurrentIndex
        {
            get
            {
                return _programLoader.ProgramStringNumber;
            }
        }
        public bool IsAutoStateSelected
        {
            get { return _isAutoStateSelected; }
            set { SetProperty(ref _isAutoStateSelected, value); }
        }
        public bool CanEditProgram
        {
            get => _canEditProgram;
            private set => SetProperty(ref _canEditProgram, value);
        }
        public string ProgramText
        {
            get => _programText;
            set
            {
                _programText = value;
                RaisePropertyChanged(nameof(ProgramText));
            }
        }
        public int StringNumber
        {
            get => _stringNumber;
            set => SetProperty(ref _stringNumber, value);
        }
        public bool IsProgramNotRunning
        {
            get { return _isProgramNotRunning; }
            set => SetProperty(ref _isProgramNotRunning, value);
        }
        public bool CanBeEdited { get; private set; }
        public bool HighlightLine
        {
            get => _highlightLine;
            set => SetProperty(ref _highlightLine, value);
        }
        public override bool KeepAlive => true;

        public DelegateCommand<string> LoadCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Свойства коллекций должны быть доступны только для чтения", Justification = "<Ожидание>")]
        public Dictionary<int, string> ProgramListing
        {
            get
            {
                if (_programLoader.IsProgramRunning)
                {
                    RemovePreviousStrings(_programLoader.ProgramStringNumber - 5);
                    _programListing = _programLoader.LoadedProgram.Where(p => p.Key >= _programLoader.ProgramStringNumber - 5 && p.Key < _programLoader.ProgramStringNumber + 50).ToDictionary(p => p.Key, p => p.Value);
                    return _programListing;
                }
                else
                {
                    return _programListing;
                }
            }
            set => SetProperty(ref _programListing, value);
        }
    }
}
