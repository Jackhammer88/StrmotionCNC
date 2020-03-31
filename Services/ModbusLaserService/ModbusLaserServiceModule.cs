using Infrastructure;
using Infrastructure.Interfaces.LaserService;
using Infrastructure.Interfaces.Logger;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Threading.Tasks;

namespace ModbusLaserService
{
    [Module(ModuleName = ModuleNames.ModbusLaserServiceModule)]
    public class ModbusLaserServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var lasers = containerProvider.Resolve<ILaserService>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILaserService, ModbusLaserService>();
        }
    }
}
