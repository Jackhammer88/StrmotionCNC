using Infrastructure;
using Infrastructure.Interfaces.UserSettingService;
using Prism.Ioc;
using Prism.Modularity;

namespace UserSettingService
{
    [Module(ModuleName = ModuleNames.UserSettingServiceModule)]
    public class UserSettingServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IUserSettingsService, UserSettingsService>();
        }
    }
}
