using Infrastructure.AppEventArgs;
using Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IController
    {
        bool Connected { get; set; }
        int DPRSize { get; }
        bool ProgramRunning { get; }
        List<int> I7XX0 { get; }

        event EventHandler OnCheckConnection;
        event EventHandler OnConnect;
        event EventHandler OnDisconnected;
        event EventHandler<ControllerErrorEventArgs> OnError;
        event EventHandler<ControllerLostConnectionEventArgs> OnLostConnection;
        event PropertyChangedEventHandler PropertyChanged;

        bool Connect(int deviceNumber = -1, int timeout = 100);
        void Disconnect();
        void Dispose();
        bool DPRAvailable();
        bool GetMotorCurrent(int motorNumber, out int phaseA, out int phaseB);
        void GetMotorPosition(int motorNumber, out double position, PositionType positionType);
        bool GetGlobalStatuses(out GlobalStatusXs nX, out GlobalStatusYs nY);
        bool GetMotorStatus(int motorNum, out MotorXStatuses nX, out MotorYStatuses nY);
        bool GetResponse(string query, out string result);
        void RotBufClear(int bufNum);
        bool RotBufInit();
        void RotBufRemove(int bufNum);
        bool RotBufSendString(string str, int bufNum);
        void RotBufSet(bool enabled);
        bool SelectController(out int devNumber);
    }
}