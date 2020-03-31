using Infrastructure;
using Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace LaserSettings
{
    public class LaserSettinigsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.LaserSettingsView);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.LaserSettingsView>();
        }
    }
}
