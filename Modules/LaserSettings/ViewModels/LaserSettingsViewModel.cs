using Infrastructure.Abstract;
using Infrastructure.Constants;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.LaserService;
using Infrastructure.Interfaces.Logger;
using LaserSettings.Model;
using LaserSettings.ParamsLoader;
using LaserSettings.Resources;
using Prism.Commands;
using Prism.Logging;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LaserSettings.ViewModels
{
    public class LaserSettingsViewModel : ViewModelBase
    {
        private readonly ConfigurationLoader _configurationLoader;
        private readonly IDialogService _dialogService;
        private readonly ILoggerExtended _logger;
        private readonly ILaserService _laserService;
        private readonly IControllerInformation _controllerInformation;
        private readonly IControllerConfigurator _controllerConfigurator;
        private LaserConfiguration _dataItems;

        public LaserSettingsViewModel(IDialogService dialogService, ILoggerExtended logger, ILaserService laserService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _laserService = laserService ?? throw new ArgumentNullException(nameof(laserService));

            Title = GeneralStrings.LaserSettingsTitle;
            _configurationLoader = new ConfigurationLoader(_logger);
            DataItems = GenerateDataItems();

            LoadConfigurationCommand = new DelegateCommand(LoadConfigurationExecute);
            SaveConfigurationCommand = new DelegateCommand(SaveConfigurationExecute);
            ReorderCollectionCommand = new DelegateCommand(ReorderCollectionExecute);
        }

        private void SaveConfigurationExecute()
        {
            _dialogService.ShowDialog(DialogServiceWindowsNames.Save, null, (r) =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    r.Parameters.TryGetValue("path", out string path);

                    if (!_configurationLoader.Save(DataItems, path))
                        _logger.Info($"{GeneralStrings.SavingError} {path}");
                }
            });

        }

        private void LoadConfigurationExecute()
        {
            var extensions = new HashSet<Tuple<string, string>>
            {
                new Tuple<string, string>("Json", ".json"),
                new Tuple<string, string>("All files", string.Empty)
            };
            var parameters = new DialogParameters
            {
                { "extensions", extensions }
            };
            LaserConfiguration laserConfiguration = null;
            _dialogService.ShowDialog(DialogServiceWindowsNames.Open, parameters, (r) =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    r.Parameters.TryGetValue("path", out string path);
                    if (_configurationLoader.LoadFromDisk(path, out laserConfiguration))
                    {
                        laserConfiguration.Tables = laserConfiguration.Tables.OrderByDescending(t => t.IsFavourite).ThenBy(t => ((ILaserTableBase)t).TableName).ToList();
                    }
                    else
                        _logger.Warn($"{GeneralStrings.OpeningError} {path}");
                }
            });
            if (laserConfiguration != null)
                DataItems = laserConfiguration;
        }

        public LaserConfiguration DataItems
        {
            get => _dataItems;
            private set => SetProperty(ref _dataItems, value);
        }
        public DelegateCommand LoadParametersCommand { get; }
        public DelegateCommand LoadConfigurationCommand { get; }
        public DelegateCommand SaveConfigurationCommand { get; }
        public DelegateCommand ReorderCollectionCommand { get; }

        private LaserConfiguration GenerateDataItems()
        {
            //var toSerialize = new LaserSettings.Model.LaserConfiguration
            //{
            //    Comments = "There is a comment.",
            //    MaterialThickness = 0.3f,
            //    MaterialType = Material.Cooper,
            //    Tables = new List<LaserParameterTableBase>()
            //    {
            //        new Punching { IsFavourite = true }, new Model.BurnNormal(), new Model.BurnSoft(),
            //        new Model.Evaporation(), new MicroweldSoft(), new SmallContour(),
            //        new ApproachNormal(), new ApproachPrecut(), new Model.Engraving(),
            //        new Model.Cooling(), new ApproachRedAcc { IsFavourite = true }, new Model.MiddleContour(),
            //        new ApproachRedSpeed(), new Model.PreBurn(), new ApproachRedSpeed(),
            //        new Model.MicroweldHard(), new Model.LargeContour()
            //    }
            //};

            //ConfigurationLoader.Save(toSerialize, @"D:\test.json");
            //return null;
            var test = _configurationLoader.LoadLastOrEmpty();
            test.Tables = test.Tables.OrderByDescending(t => t.IsFavourite).ThenBy(t => ((ILaserTableBase)t).TableName).ToList();
            return test;
        }

        public void ReorderCollectionExecute()
        {
            DataItems.Tables = DataItems.Tables.OrderByDescending(t => t.IsFavourite).ThenBy(t => ((ILaserTableBase)t).TableName).ToList();
            DataItems.RaiseTablesPropertyChanged();
        }
    }
}
