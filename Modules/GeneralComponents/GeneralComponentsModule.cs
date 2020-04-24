using CommonServiceLocator;
using GeneralComponents.Views;
using Infrastructure;
using Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace GeneralComponents
{
    [Module(ModuleName = ModuleNames.GeneralComponentsModule)]
    public class GeneralComponentsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.Auto);
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.MDI);
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.ManualExt);
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.Statuses);

            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Manual);
            //regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.ToolOffsets);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Offset);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Plot);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.LaserTuning);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Terminal);
            
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.Auto);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Offset);
            //regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.ToolOffsets);


            var autoView = ServiceLocator.Current.GetInstance<Auto>();

            containerProvider.Resolve<Auto>();
            var srm = regionManager.CreateRegionManager();
            RegionManager.SetRegionManager(autoView, srm);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Auto>();
            containerRegistry.RegisterForNavigation<MDI>();
            containerRegistry.RegisterForNavigation<Manual>();
            containerRegistry.RegisterForNavigation<ManualExt>();
            containerRegistry.RegisterForNavigation<Offset>();
            containerRegistry.RegisterForNavigation<LaserTuning>();
            //containerRegistry.RegisterForNavigation<ToolOffsets>();
            containerRegistry.RegisterForNavigation<Plot>();
            containerRegistry.RegisterForNavigation<Terminal>();
            containerRegistry.RegisterForNavigation<Statuses>();

            containerRegistry.Register<Auto>();
            containerRegistry.RegisterForNavigation<ProgramViewer>();
            containerRegistry.RegisterForNavigation<ProgramEditor>();
        }
    }
}
