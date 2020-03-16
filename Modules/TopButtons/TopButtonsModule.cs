using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TopButtons
{
    [Module(ModuleName = ModuleNames.TopButtonsModule)]
    public class TopButtonsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.TopRegion, typeof(Views.TopButtons));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
