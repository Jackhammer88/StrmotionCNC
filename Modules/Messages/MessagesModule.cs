using Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Messages
{
    [Module(ModuleName = ModuleNames.MessagesModule)]
    public class MessagesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MessageRegion, typeof(Views.Messages));
            regionManager.RegisterViewWithRegion(RegionNames.StatusMessageRegion, typeof(Views.StatusBar));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
