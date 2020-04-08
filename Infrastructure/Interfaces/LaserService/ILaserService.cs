using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.LaserService
{
    public interface ILaserService : INotifyPropertyChanged
    {
        
        void TryToStart();
        void TryToStop();
        Task<bool> ChangeLensFocus(int value);
        bool IsConnected { get; }
        ILaserInfo LaserInfoModel { get; }
        event EventHandler Connected;
        event EventHandler Disconnected;
        event EventHandler FocusChangingRequested;
    }
}
