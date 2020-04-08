using Infrastructure.Interfaces.LaserService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace ModbusLaserService.Model
{
    public class LaserInfo : ILaserInfo
    {
        private float _mainTemperature;
        private float _protectiveWindowTemp;
        private float _colimLensTemp;
        private float _focusingLensTemp;
        private int _dirtyLaserRayCoefficient;
        private int _focalConfiguration;
        private float _ZPosition;
        private int _currentErrorCode;
        private float _lensPosition;
        private float _workingGasPressure;
        private float _internalPressure;

        public float MainTemperature
        {
            get => _mainTemperature;
            set => SetProperty(ref _mainTemperature, value);
        }
        public float ProtectiveWindowTemp
        {
            get => _protectiveWindowTemp;
            set => SetProperty(ref _protectiveWindowTemp, value);
        }
        public float WorkingGasPressure
        {
            get  => _workingGasPressure;
            set => SetProperty(ref _workingGasPressure, value);
        }
        public float InternalPressure
        {
            get => _internalPressure;
            set => SetProperty(ref _internalPressure, value);
        }
        public float ColimLensTemp
        {
            get => _colimLensTemp;
            set => SetProperty(ref _colimLensTemp, value);
        }
        public float FocusingLensTemp
        {
            get => _focusingLensTemp;
            set => SetProperty(ref _focusingLensTemp, value);
        }
        public float LensPosition
        {
            get => _lensPosition;
            set => SetProperty(ref _lensPosition, value);
        }
        public int DirtyLaserRayCoefficient
        {
            get => _dirtyLaserRayCoefficient;
            set => SetProperty(ref _dirtyLaserRayCoefficient, value);
        }
        public int FocalConfiguration
        {
            get => _focalConfiguration;
            set => SetProperty(ref _focalConfiguration, value);
        }
        public float ZPosition
        {
            get => _ZPosition;
            set => SetProperty(ref _ZPosition, value);
        }
        public int CurrentErrorCode
        {
            get => _currentErrorCode;
            set => SetProperty(ref _currentErrorCode, value);
        }
        public void SetProperty<T>(ref T source, T value, [CallerMemberName]string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(source, value))
            {
                source = value;
                RaisePropertyChanged(propertyName);
            }
        }
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    }
