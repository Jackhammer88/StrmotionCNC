using BottomButtons.Views;
using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace BottomButtons
{
    [Module(ModuleName = ModuleNames.BottomButtonsModule)]
    public class BottomButtonsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(Bottom));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
