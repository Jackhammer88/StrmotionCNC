﻿using ActualCodes;
using BottomButtons;
using CNC.ViewModels;
using CNC.Views;
using CNCDialogService.ViewModels;
using CNCDialogService.Views;
using ControllerService;
using GeneralComponents;
using Infrastructure;
using LoggerService;
using Messages;
using MotorInfo;
using NLog;
using OpenDialog.Views;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Unity;
using SharpDX.Direct3D11;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TopButtons;
using UserSettingService;

namespace CNC
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private Views.SplashScreen _tempWindow;

        public Logger GlobalLogger { get; }

        public App()
        {
            GlobalLogger = LogManager.GetCurrentClassLogger();
        }
        protected override Window CreateShell()
        {
            CatchUnhandledExceptions();

            //SplashScreen splash = new SplashScreen("splash.jpg");
            //splash.Show(true);
            var shell = Container.Resolve<Shell>();
            return shell;
        }

        protected override void InitializeModules()
        {
            bool wasException = false;
            try
            {
                NewWindowHandler();
                base.InitializeModules();
            }
            catch(Exception)
            {
                SetLoadingMessage($"An error ocurrs during the app was loading.");
                wasException = true;
            }
            SetFinishLoadingProgress();
            Thread.Sleep(500);
            if (wasException)
                Thread.Sleep(5000);
            CloseWindowSafe();
        }

        private void CatchUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<Shell>();
            containerRegistry.RegisterDialog<GotoLine, GotoLineViewModel>();
            containerRegistry.RegisterDialog<Open, OpenViewModel>();
            containerRegistry.RegisterDialog<Save, SaveViewModel>();
            containerRegistry.RegisterDialog<Confirmation, ConfirmationViewModel>();
            containerRegistry.RegisterDialog<UserSettings, UserSettingsViewModel>();
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            if (moduleCatalog == null) throw new ArgumentNullException(nameof(moduleCatalog));
            base.ConfigureModuleCatalog(moduleCatalog);
            LoadServices(moduleCatalog);
            LoadModules(moduleCatalog);
        }

        private void LoadModules(IModuleCatalog moduleCatalog)
        {
            var codesModule = typeof(ActualCodesModule);
            var messagesModule = typeof(MessagesModule);
            var bottomButtonsModule = typeof(BottomButtonsModule);
            var generalComponentsModule = typeof(GeneralComponentsModule);
            var motorInfoModule = typeof(MotorInfoModule);
            var topButtonsModule = typeof(TopButtonsModule);

            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = codesModule.Name,
                ModuleType = codesModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.ControllerServiceModule }
            });
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = messagesModule.Name,
                ModuleType = messagesModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.LoggerServiceModule, ModuleNames.ActualCodesModule }
            });
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = bottomButtonsModule.Name,
                ModuleType = bottomButtonsModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.MessagesModule, /*ModuleNames.ControlPanelServiceModule,*/ ModuleNames.ControllerServiceModule }
            });
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = generalComponentsModule.Name,
                ModuleType = generalComponentsModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.BottomButtonsModule, ModuleNames.ControllerServiceModule, ModuleNames.LoggerServiceModule }
            });
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = motorInfoModule.Name,
                ModuleType = motorInfoModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.GeneralComponentsModule, ModuleNames.ControllerServiceModule }
            });
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = topButtonsModule.Name,
                ModuleType = topButtonsModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.MotorInfoModule, ModuleNames.ControllerServiceModule }
            });
        }
        private void LoadServices(IModuleCatalog moduleCatalog)
        {
            var userSettingServiceModule = typeof(UserSettingServiceModule);

            moduleCatalog.AddModule<LoggerServiceModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = userSettingServiceModule.Name,
                ModuleType = userSettingServiceModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string> { ModuleNames.LoggerServiceModule }
            });
            //moduleCatalog.AddModule<ControlPanelServiceModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<ControllerServiceModule>(InitializationMode.WhenAvailable);

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = Container.Resolve<ILoggerFacade>();
            GlobalLogger.Fatal(e);
            logger.Log(e.ExceptionObject.ToString(), Category.Exception, Priority.High);
            if (e.IsTerminating)
            {
                logger.Log("Application is shutting down.", Category.Exception, Priority.High);
                App.Current.Shutdown();
            }
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var logger = Container.Resolve<ILoggerFacade>();
            GlobalLogger.Fatal(e);
            logger.Log(e.Exception.ToString(), Category.Exception, Priority.High);
        }
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = Container.Resolve<ILoggerFacade>();
            var exception = e.Exception;
            while (exception != null)
            {
                logger.Log(exception.Message, Category.Exception, Priority.High);
                GlobalLogger.Fatal(exception);
                exception = exception.InnerException;
            }
            e.Handled = true;
        }

        private void NewWindowHandler()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                _tempWindow = new Views.SplashScreen();
                // When the window closes, shut down the dispatcher
                _tempWindow.Closed += (s, e) =>
                   Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                _tempWindow.Show();
                // Start the Dispatcher Processing
                System.Windows.Threading.Dispatcher.Run();
            }));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();
        }
        void SetLoadingMessage(string message)
        {
            RunThroughThread(() => _tempWindow.loadingMessage.Text = message);
        }
        void SetFinishLoadingProgress()
        {
            RunThroughThread(() => _tempWindow.loadingBar.Maximum = 1);
        }
        void CloseWindowSafe()
        {
            RunThroughThread(() => _tempWindow.Close());
        }
        void RunThroughThread(Action runner)
        {
            if (_tempWindow.Dispatcher.CheckAccess())
                runner();
            else
                _tempWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(runner));
        }
    }


}
