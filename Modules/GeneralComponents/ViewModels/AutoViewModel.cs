using Infrastructure;
using Infrastructure.Abstract;
using Infrastructure.Constants;
using Infrastructure.Resources.Strings;
using Prism.Regions;
using System;

namespace GeneralComponents.ViewModels
{
    public class AutoViewModel : ViewModelBase
    {
        readonly IRegionManager _regionManager;
        public AutoViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager ?? throw new NullReferenceException();
            Title = GeneralComponentsStrings.Auto;

            regionManager.RequestNavigate(RegionNames.AutoChildRegion, ViewNames.ProgramViewer);
        }

    }
}
