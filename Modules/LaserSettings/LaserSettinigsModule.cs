﻿using Infrastructure;
using Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace LaserSettings
{
    [Module(ModuleName = ModuleNames.LasserSettingsModule)]
    public class LaserSettinigsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.LaserSettings);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.LaserInformation);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.LaserSettings>();
            containerRegistry.RegisterForNavigation<Views.LaserInformation>();
        }
    }
}
