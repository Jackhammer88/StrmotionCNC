using Infrastructure.Interfaces.UserSettingService;
using Prism.Commands;

namespace CNCDialogService.ViewModels
{
    public class UserSettingsViewModel : DialogBase
    {
        private readonly IUserSettingsService _settingsService;
        private bool _alertVisibility;

        public DelegateCommand ExitCommand { get; }

        public UserSettingsViewModel(IUserSettingsService settingsService)
        {
            _settingsService = settingsService;

            ExitCommand = new DelegateCommand(ExitExecute);

            AlertVisibility = false;
        }

        private void ExitExecute()
        {
            RaiseRequestClose(null);
        }

        public int Timeout
        {
            get => _settingsService.Timeout;
            set
            {
                _settingsService.Timeout = value;
                AlertVisibility = true;
                RaisePropertyChanged();
            }
        }
        public int ReconnectionCount
        {
            get => _settingsService.ReconnectionCount;
            set
            {
                _settingsService.ReconnectionCount = value;
                AlertVisibility = true;
                RaisePropertyChanged();
            }
        }
        public int MaxCurrentLimit
        {
            get => _settingsService.MaxCurrentLimit;
            set
            {
                _settingsService.MaxCurrentLimit = value;
                AlertVisibility = true;
                RaisePropertyChanged();
            }
        }
        public int ScaleFactor
        {
            get => _settingsService.ScaleFactor;
            set
            {
                _settingsService.ScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        public bool AlertVisibility
        {
            get => _alertVisibility;
            set => SetProperty(ref _alertVisibility, value);
        }
    }
}
