using Infrastructure;
using MotorInfo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MotorInfo
{
    [Module(ModuleName = ModuleNames.MotorInfoModule)]
    public class MotorInfoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.InformationRegion, typeof(Info));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
