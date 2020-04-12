using ControllerService.Gcode;
using ControllerService.Modules;
using Infrastructure;
using Infrastructure.Interfaces.CNCControllerService;
using Prism.Ioc;
using Prism.Modularity;
using System;

namespace ControllerService
{
    [Module(ModuleName = ModuleNames.ControllerServiceModule)]
    public class ControllerServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterControllerServiceModules(containerRegistry);
        }

        private static void RegisterControllerServiceModules(IContainerRegistry container)
        {
            container.RegisterSingleton<IController, PMAC>();
            container.RegisterSingleton<IControllerConnectionManager, PmacConnectionManager>();
            container.RegisterSingleton<IControllerConfigurator, PmacConfigurator>();
            container.RegisterSingleton<IControllerInformation, PmacInformation>();
            container.RegisterSingleton<IManualMachineControl, PmacJogManager>();
            container.RegisterSingleton<IProgramLoader, PmacProgramLoader>();
            container.RegisterSingleton<ITerminalClient, PmacTerminalClient>();
            container.RegisterSingleton<ICodeCalculator, CodeProcessor>();
            container.RegisterSingleton<IControllerErrorHandler, PmacErrorHandler>();
        }
    }
}
