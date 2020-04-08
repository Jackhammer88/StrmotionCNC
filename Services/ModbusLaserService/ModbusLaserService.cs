using AMWD.Modbus.Common.Structures;
using AMWD.Modbus.Tcp.Client;
using Infrastructure.Interfaces.LaserService;
using Infrastructure.Interfaces.Logger;
using ModbusLaserService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModbusLaserService.TaskExtension;
using System.Diagnostics;
using Infrastructure.Interfaces.UserSettingService;
using System.ComponentModel;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Constants;
using Infrastructure.AppEventArgs;

namespace ModbusLaserService
{

    public class ModbusLaserService : ILaserService
    {
        private const string boardIpAddress = "17.172.68.110";
        public const byte DeviceID = 1;
        //private const string boardIpAddress = "127.0.0.1";
        private readonly ILoggerExtended _loggerExtended;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IControllerConfigurator _controllerConfigurator;
        private ModbusClient _client;
        private CancellationTokenSource _cancellationToken;
        private int _debug = 50;

        public ModbusLaserService(ILoggerExtended loggerExtended, IUserSettingsService userSettingsService, IControllerConfigurator controllerConfigurator)
        {
            _loggerExtended = loggerExtended ?? throw new ArgumentNullException(nameof(loggerExtended));
            _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
            _controllerConfigurator = controllerConfigurator ?? throw new ArgumentNullException(nameof(controllerConfigurator));

            _client = new ModbusClient(boardIpAddress);
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            LaserInfoModel = new LaserInfo();

            TryToStart();
        }


        private void _client_Connected(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
            _loggerExtended.Info("board connected");
        }
        private void _client_Disconnected(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
            _loggerExtended.Info("board disconnected");
            if (_userSettingsService.ReconnectionCount > 0)
                Task.Run(ReconnectToBoard, RechargeCancellationToken());
        }

        private async void ReconnectToBoard()
        {
            bool alreadyConnected = false;
            for (int i = 0; i < _userSettingsService.ReconnectionCount && !alreadyConnected; i++)
            {
                await Task.Delay(2000);
                await _client.Connect().AwaitWithTimeout(10000, () =>
                {
                    if (_client.IsConnected)
                    {
                        alreadyConnected = true;
                        Task.Run(DoPolling, RechargeCancellationToken());
                    }
                }, () => _loggerExtended.Exception("Can't connect to laser board"));
            }
        }

        ~ModbusLaserService()
        {
            if (_client != null && _client.IsConnected)
                _client.Disconnect();
        }
        public void TryToStart()
        {
            Task.Run(ConnectToBoard, RechargeCancellationToken());
        }

        private async void ConnectToBoard()
        {
            while (!_client.IsConnected)
            {
                await Task.Delay(2000);
                await _client.Connect().AwaitWithTimeout(10000, () => Task.Run(DoPolling, RechargeCancellationToken()),
                    () => _loggerExtended.Info("Can't connect to laser board"));
            }
        }

        private async void DoPolling()
        {
            while (_client.IsConnected)
            {
                try
                {
                    //if (_debug % 5 == 0)
                    //{
                    //    var fResult = await ChangeLensFocus(_debug);
                    //    Debug.WriteLine($"Focal distance = {_debug}\nFocusing result: {fResult}");
                    //}
                    //if (_debug > 255)
                    //    _debug = 5;
                    //_debug++;

                    if (_controllerConfigurator.GetVariable(PVariables.LaserFocusChangingRequired, out double needToChangeFocus) && needToChangeFocus == 1
                        && _controllerConfigurator.GetVariable(PVariables.LaserCurrentFocus, out int focus))
                    {
                        if (!await ChangeLensFocus(focus))
                            _loggerExtended.Fatal("Can't change laser focus");
                    }
                    var result = await _client.ReadInputRegisters(DeviceID, 0x7, 13);
                    if (result != null)
                    {
                        SetErrorCode(result.Single(d => d.Address == 0x7)); //Регистр содержит значение кода ошибки лазерной головы; диапазон значений 0x000-0xFFF.

                        SetProtectiveWindowTemp(result.Single(d => d.Address == 0xA)); //Регистр содержит значение температуры защитного окна; диапазон значений от 0 до 200.
                        SetWorkingGasPressure(result.Single(d => d.Address == 0xB)); //Регистр содержит значение давления газа; диапазон значений от 0 до 25.
                        SetInternalPressure(result.Single(d => d.Address == 0xC)); //Регистр содержит значение внутреннего давления; диапазон значений от 0 до 100.
                        SetColimLensTemp(result.Single(d => d.Address == 0xD)); //Регистр содержит значение температуры коллиматорной линзы; диапазон значений от 0 до 100.
                        SetFocusingLensTemp(result.Single(d => d.Address == 0xE)); //Регистр содержит значение температуры фокусирующей линзы; диапазон значений от 0 до 100.
                        SetMainTemp(result.Single(d => d.Address == 0xF)); //Регистр содержит значение общей температуры лазерной головы; диапазон значений от 0 до 100.
                        SetLensPosition(result.Single(d => d.Address == 0x10)); //Регистр содержит значение положения линз; диапазон значений от 0 до 10.
                        SetDirtyLaserRayCoefficient(result.Single(d => d.Address == 0x11)); //Регистр содержит значение о рассеянии лазерного луча; диапазон значений от 0 до 1000.
                        SetFocalConfiguration(result.Single(d => d.Address == 0x12)); //Регистр содержит значение о фокусной конфигурации линз; диапазон значений от 0 до 100.
                        SetZPosition(result.Single(d => d.Address == 0x13)); //Регистр содержит значение Z позиции; диапазон значений от 0 до 100.
                    }
                    await Task.Delay(1000);
                }
                catch (InvalidOperationException)
                {
                    await _client.Disconnect();
                    return;
                }
            }
        }
        public async Task<bool> ChangeLensFocus(int value)
        {
            if(_client.IsConnected)
            {
                //TODO: загрузить параметры фокусировки
                await _client.WriteSingleRegister(1, new Register { Address = 0x7, Value = (ushort)value });
                await _client.WriteSingleCoil(1, new Coil { Address = 0x7, Value = true });
                await Task.Delay(50);
                List<Coil> result = await _client.ReadCoils(1, 0x7, 1);
                for(int i = 0; result != null && result[0].Value && i < 10; i++)
                {
                    await Task.Delay(200);
                    result = await _client.ReadCoils(1, 0x7, 1);
                }
                Debug.WriteLine(result == null ? false : result[0].Value);
                if (result == null ? false : result[0].Value)
                {
                    _loggerExtended.Fatal("can't change a focal distance. Board going to reboot.");
                    await _client.WriteSingleCoil(DeviceID, new Coil { Address = 0xB, Value = true });
                }
                return result != null && result.Count == 1 && result.First().Value == false;
            }
            return false;
        }

        private void SetErrorCode(Register list)
        {
            LaserInfoModel.CurrentErrorCode = list.Value;
        }

        private void SetProtectiveWindowTemp(Register list)
        {
            LaserInfoModel.ProtectiveWindowTemp = list.Value;
        }

        private void SetWorkingGasPressure(Register list)
        {
            LaserInfoModel.WorkingGasPressure = list.Value;
        }

        private void SetInternalPressure(Register list)
        {
            LaserInfoModel.InternalPressure = list.Value;
        }

        private void SetColimLensTemp(Register list)
        {
            LaserInfoModel.ColimLensTemp = list.Value;
        }

        private void SetFocusingLensTemp(Register list)
        {
            LaserInfoModel.FocusingLensTemp = list.Value;
        }

        private void SetMainTemp(Register list)
        {
            LaserInfoModel.MainTemperature = list.Value;
        }

        private void SetLensPosition(Register list)
        {
            LaserInfoModel.LensPosition = list.Value;
        }

        private void SetDirtyLaserRayCoefficient(Register list)
        {
            LaserInfoModel.DirtyLaserRayCoefficient = list.Value;
        }

        private void SetFocalConfiguration(Register list)
        {
            LaserInfoModel.FocalConfiguration = list.Value;
        }

        private void SetZPosition(Register list)
        {
            LaserInfoModel.ZPosition = list.Value;
        }

        private CancellationToken RechargeCancellationToken()
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();
            _cancellationToken = new CancellationTokenSource();
            return _cancellationToken.Token;
        }

        public void TryToStop()
        {
            _client.Disconnect();
        }

        public ILaserInfo LaserInfoModel { get; }
        public bool IsConnected => _client.IsConnected;

        public event EventHandler Connected
        {
            add { _client.Connected += value; }
            remove { _client.Connected -= value; }
        }
        public event EventHandler Disconnected
        {
            add { _client.Disconnected += value; }
            remove { _client.Disconnected -= value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler FocusChangingRequested;
    }
}
