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
        }

        public ILaserInfo InfoModel => _laserService.LaserInfoModel;
    }
}
