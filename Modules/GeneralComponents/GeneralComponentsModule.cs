using CommonServiceLocator;
using GeneralComponents.Views;
using Infrastructure;
using Infrastructure.Constants;
using Infrastructure.Interfaces.Logger;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace GeneralComponents
{
    [Module(ModuleName = ModuleNames.GeneralComponentsModule)]
    public class GeneralComponentsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.Auto);
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.MDI);
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.ManualExt);
            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.Statuses);

            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Manual);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.ToolOffsets);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Offset);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Plot);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.LaserTuning);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.Terminal);

            regionManager.RequestNavigate(RegionNames.LeftCenterRegion, ViewNames.Auto);
            regionManager.RequestNavigate(RegionNames.RightCenterRegion, ViewNames.ToolOffsets);


            var autoView = ServiceLocator.Current.GetInstance<Auto>();

            containerProvider.Resolve<Auto>();
            var srm = regionManager.CreateRegionManager();
            RegionManager.SetRegionManager(autoView, srm);
        }

        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    var logger = ServiceLocator.Current.GetInstance<ILoggerExtended>();
        //    var exception = e.ExceptionObject as Exception;
        //    logger.Fatal(exception.ToString());
        //}

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Auto>();
            containerRegistry.RegisterForNavigation<MDI>();
            containerRegistry.RegisterForNavigation<Manual>();
            containerRegistry.RegisterForNavigation<ManualExt>();
            containerRegistry.RegisterForNavigation<Offset>();
            containerRegistry.RegisterForNavigation<LaserTuning>();
            containerRegistry.RegisterForNavigation<ToolOffsets>();
            containerRegistry.RegisterForNavigation<Plot>();
            containerRegistry.RegisterForNavigation<Terminal>();
            containerRegistry.RegisterForNavigation<Statuses>();

            containerRegistry.Register<Auto>();
            containerRegistry.RegisterForNavigation<ProgramViewer>();
            containerRegistry.RegisterForNavigation<ProgramEditor>();
        }
    }
}
