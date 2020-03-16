using Infrastructure.AppEventArgs;
using System;
using System.ComponentModel;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IControllerEvents : INotifyPropertyChanged
    {
        event EventHandler OnConnect;
        event EventHandler OnDisconnected;
        event EventHandler<ControllerErrorEventArgs> OnError;
        event EventHandler<ControllerConfiguratorEventArgs> OnInvalidInputFormat;
        event EventHandler<ControllerLostConnectionEventArgs> OnLostConnection;
    }
}
