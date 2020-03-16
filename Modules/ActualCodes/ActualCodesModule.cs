using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ActualCodes
{
    [Module(ModuleName = ModuleNames.ActualCodesModule)]
    public class ActualCodesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.CodesRegion, typeof(Views.Codes));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
