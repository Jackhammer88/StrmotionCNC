using Infrastructure.Abstract;
using Infrastructure.Interfaces.LaserService;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaserSettings.ViewModels
{
    public class LaserInformationViewModel : ViewModelBase
    {
        private readonly ILaserService _laserService;

        public LaserInformationViewModel(ILaserService laserService)
        {
            _laserService = laserService ?? throw new ArgumentNullException(nameof(laserService));
            Title = "Laser Info";

            _laserService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals(nameof(_laserService.IsConnected), StringComparison.Ordinal))
                    RaisePropertyChanged(nameof(IsConnected));
            };
        }

        public bool IsConnected => _laserService.IsConnected;
        public ILaserInfo InfoModel => _laserService.LaserInfoModel;
    }
}
