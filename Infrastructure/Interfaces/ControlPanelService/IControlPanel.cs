using Infrastructure.AppEventArgs;
using System;

namespace Infrastructure.Interfaces.ControlPanelService
{
    public interface IControlPanel
    {
        bool Restart { get; set; }
        long Support { get; }

        event EventHandler<ControlPanelDataRecievedEventArgs> DataReceived;
        event EventHandler KeyboardConnected;
        event EventHandler KeyboardDisconnected;
        event EventHandler KeyboardRestarted;
        event EventHandler LedConnected;
        event EventHandler LedDisconnected;

        void BeginKeyboardConnect();
        void BeginLedConnect();
        void ClearSupport();
        bool ConvertData(byte[] Mas);
        bool ConvertData(byte[] Mas, int StartingAddress);
        void KeyboardDisconnect();
        void LedDisconnect();
        bool ReadButton(int aNumber = 0);
        short ReadEncoder(int aNumber = 0);
        bool SetLed(int aNumber = 0, bool aValue = false);
    }
}
