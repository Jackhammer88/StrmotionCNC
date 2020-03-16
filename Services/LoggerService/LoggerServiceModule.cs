using Infrastructure;
using Infrastructure.Interfaces.Logger;
using LoggerService.Resources;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;

namespace LoggerService
{
    [Module(ModuleName = ModuleNames.LoggerServiceModule)]
    public class LoggerServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<ILoggerFacade>().Log(CommonResources.LoggerLoaded, Category.Debug, Priority.None);

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<LoggerExtended>();
            containerRegistry.Register<ILoggerFacade, LoggerExtended>();
            containerRegistry.Register<ILoggerCollection, LoggerExtended>();
            containerRegistry.Register<ILoggerExtended, LoggerExtended>();
        }
    }
}
