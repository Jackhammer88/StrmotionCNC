using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.LaserService
{
    public interface ILaserService
    {
        
        void TryToStart();
        void TryToStop();
        bool TestRead(ushort address);
        bool IsConnected { get; }
        event EventHandler Connected;
        event EventHandler Disconnected;
        ILaserInfo LaserInfoModel { get; }
    }
}
