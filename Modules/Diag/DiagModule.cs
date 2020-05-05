using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Diag.Views;
using Diag.ViewModels;


namespace Diag
{
    public class DiagModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, "Diagnostics");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Diagnostics, DiagnosticsViewModel>();
        }
    }
}
