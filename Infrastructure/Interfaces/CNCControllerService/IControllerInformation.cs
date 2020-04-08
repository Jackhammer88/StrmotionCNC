using Infrastructure.AppEventArgs;
using Infrastructure.Enums;
using Infrastructure.SharedClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IControllerInformation : INotifyPropertyChanged
    {
        int CoordinateSystemNumber { get; set; }
        bool IsReconnectionStarted { get; set; }
        int MachineType { get; }
        PositionType SelectedPositionType { get; set; }
        List<IMotor> Motors { get; }
        IObservable<MachineStatuses> StatusManager { get; }
        GlobalStatusXs StatusX { get; }
        GlobalStatusYs StatusY { get; }

        event EventHandler ActivatedMotorsCountChanged;
        event EventHandler MotorCountChanged;
    }
}