using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Infrastructure.Interfaces.LaserService
{
    public interface ILaserInfo : INotifyPropertyChanged
    {
        float ColimLensTemp { get; set; }
        int DirtyLaserRayCoefficient { get; set; }
        int FocalConfiguration { get; set; }
        float FocusingLensTemp { get; set; }
        float LensPosition { get; set; }
        float MainTemperature { get; set; }
        float ProtectiveWindowTemp { get; set; }
        float ZPosition { get; set; }
        float InternalPressure { get; set; }
        int CurrentErrorCode { get; set; }
        float WorkingGasPressure { get; set; }
    }
}
