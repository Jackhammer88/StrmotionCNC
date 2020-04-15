using GeneralComponents.Models;
using Infrastructure.Abstract;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Infrastructure.SharedClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneralComponents.ViewModels
{
    public class StatusesViewModel : ViewModelBase
    {
        private readonly IControllerInformation _controllerInformation;
        private readonly IControllerErrorHandler _errorHandler;
        private int _motorsCount;
        private GlobalStatuses _global;

        public StatusesViewModel(IControllerInformation controllerInformation, IControllerErrorHandler errorHandler)
        {
            _controllerInformation = controllerInformation ?? throw new ArgumentNullException(nameof(controllerInformation));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler)); ;
            var nClient = new StatusesNotificationClient();
            NotificationClient = nClient;
            MotorsStatuses = new List<MotorStatuses>();

            Title = GeneralComponentsStrings.StatusString;

            nClient.OnStatusesUpdated += NClient_OnStatusesUpdated;
            _controllerInformation.StatusManager.Subscribe(nClient);
        }

        private void NClient_OnStatusesUpdated(object sender, MachineStatuses e)
        {
            Global = new GlobalStatuses(e.GlobalX, e.GlobalY);
            if (MotorsCount != e.MotorsStatuses.Count)
                MotorsCount = e.MotorsStatuses.Count;
            MotorsStatuses.Clear();
            foreach (var motorStatus in e.MotorsStatuses)
            {
                MotorsStatuses.Add(new MotorStatuses(motorStatus.Item1, motorStatus.Item2));
            }
            RaisePropertyChanged(nameof(MotorsStatuses));
        }
        private void _controllerInformation_MotorCountChanged(object sender, EventArgs e)
        {
            MotorsCount = _controllerInformation.Motors.Count;
        }

        public int MotorsCount
        {
            get => _motorsCount;
            set => SetProperty(ref _motorsCount, value);
        }
        public GlobalStatuses Global
        {
            get => _global;
            set => SetProperty(ref _global, value);
        }
        public List<MotorStatuses> MotorsStatuses { get; }
        public IObserver<MachineStatuses> NotificationClient { get; }
    }
}
