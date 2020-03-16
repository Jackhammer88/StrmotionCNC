using ICSharpCode.AvalonEdit.Document;
using Infrastructure;
using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GeneralComponents.ViewModels
{
    public class ProgramEditorViewModel : AutoViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        readonly IDialogService _dialogService;
        readonly IRegionManager _regionManager;

        public ProgramEditorViewModel(IEventAggregator eventAggregator, IDialogService dialogService, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            Title = GeneralComponentsStrings.Auto;

            _eventAggregator.GetEvent<ResetEvent>().Subscribe(OnReseted);
            _eventAggregator.GetEvent<MachineState>().Subscribe(OnMachineStateChanged);

            GotoCommand = new DelegateCommand(OnGotoExecute);
            ExitEditorCommand = new DelegateCommand(ExitEditorCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);
            RunFromLineCommand = new DelegateCommand(RunFromLineExecute);
            Document = new TextDocument();
        }

        private async void RunFromLineExecute()
        {
            if (DocumentChanged && AskAboutSaving())
                await SaveFile().ConfigureAwait(true);

            var navigationParameters = new NavigationParameters { { "file-changed", FileChanged }, { "selectedLine", SelectedLine } };
            _regionManager.RequestNavigate(RegionNames.AutoChildRegion, ViewNames.ProgramViewer, navigationParameters);
        }

        private void OnMachineStateChanged(MachineStateType state)
        {
            CloseEditorAndDiscardChanges();
        }
        private void OnReseted()
        {
            CloseEditorAndDiscardChanges();
        }
        private void CloseEditorAndDiscardChanges()
        {
            var navigationParameters = new NavigationParameters();
            _regionManager.RequestNavigate(RegionNames.AutoChildRegion, ViewNames.ProgramViewer, navigationParameters);
        }
        void OnGotoExecute()
        {
            _dialogService.ShowDialog("GotoLine", new DialogParameters(), GotoCallback);
        }
        void GotoCallback(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                //TODO: go to line number n...
                if (result.Parameters.TryGetValue("line", out uint line))
                    LineNumber = line == 0 ? 0 : line - 1;
            }
        }
        async void ExitEditorCommandExecute()
        {
            if (DocumentChanged && AskAboutSaving())
                await SaveFile().ConfigureAwait(true);

            var navigationParameters = new NavigationParameters { { "file-changed", FileChanged } };
            _regionManager.RequestNavigate(RegionNames.AutoChildRegion, ViewNames.ProgramViewer, navigationParameters);
        }
        bool SaveCommandCanExecute()
        {
            return DocumentChanged;
        }
        async void SaveCommandExecute()
        {
            await SaveFile().ConfigureAwait(true);
            SaveCommand.RaiseCanExecuteChanged();
        }
        private async Task LoadFile()
        {
            IsBusy = true;
            using (var reader = new StreamReader(FileName))
            {
                Document = new TextDocument(await reader.ReadToEndAsync().ConfigureAwait(true));
                Document.Changed += (s, e) =>
                {
                    DocumentChanged = true;
                    SaveCommand.RaiseCanExecuteChanged();
                };
                reader.Close();
                RaisePropertyChanged(nameof(Document));
            }
            IsBusy = false;
        }
        async Task SaveFile()
        {
            FileChanged = true;
            IsBusy = true;
            using (var writer = new StreamWriter(FileName, false))
            {
                await writer.WriteAsync(Document.Text).ConfigureAwait(true);
                writer.Close();
            }
            DocumentChanged = false;
            IsBusy = false;
        }
        private bool AskAboutSaving()
        {
            bool result = false;
            var parameters = new DialogParameters
            {
                { "title", GeneralComponentsStrings.EditExitTitle },
                { "message", GeneralComponentsStrings.EditExitMessage }
            };
            if (DocumentChanged)
                _dialogService.ShowDialog("Confirmation", parameters, (r) => result = r.Result == ButtonResult.Yes);
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters == null || !navigationContext.Parameters.ContainsKey("filename"))
                return;

            var parameters = navigationContext.Parameters;
            FileName = (string)parameters["filename"];
            await LoadFile().ConfigureAwait(true);
        }

        public TextDocument Document { get; private set; }
        public bool DocumentChanged { get; private set; }
        public bool FileChanged { get; private set; }
        private uint _lineNumber;
        private int _selectedLine;

        public uint LineNumber
        {
            get => _lineNumber;
            set => SetProperty(ref _lineNumber, value);
        }
        public int SelectedLine
        {
            get => _selectedLine;
            set => SetProperty(ref _selectedLine, value);
        }

        public DelegateCommand GotoCommand { get; set; }
        public DelegateCommand ExitEditorCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand RunFromLineCommand { get; }
    }
}
