using Infrastructure.AppEventArgs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IControllerConnectionManager
    {
        bool Connected { get; }
        bool IsReconnectionStarted { get; }

        event EventHandler OnConnect;
        event EventHandler OnDisconnected;
        event EventHandler<ControllerErrorEventArgs> OnError;
        event EventHandler<ControllerLostConnectionEventArgs> OnLostConnection;

        void Connect();
        Task ConnectAsync(CancellationToken cancellationToken);
        void Disconnect();
        //bool SelectController(bool autoConnect = false);
        Task<bool> SelectControllerAsync(bool autoConnect = false);
    }
}
