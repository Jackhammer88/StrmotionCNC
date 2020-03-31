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

namespace ModbusLaserService
{

    public class ModbusLaserService : ILaserService
    {
        private const string boardIpAddress = "17.172.68.110";
        public const byte DeviceID = 1;
        //private const string boardIpAddress = "127.0.0.1";
        private readonly ILoggerExtended _loggerExtended;


        private ModbusClient _client;
        private CancellationTokenSource _cancellationToken;

        public ModbusLaserService(ILoggerExtended loggerExtended)
        {
            _client = new ModbusClient(boardIpAddress);
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            _loggerExtended = loggerExtended ?? throw new ArgumentNullException(nameof(loggerExtended));

            LaserInfoModel = new LaserInfo();

            TryToStart();
        }


        private void _client_Connected(object sender, EventArgs e)
        {
            _loggerExtended.Info("board connected");
        }
        private void _client_Disconnected(object sender, EventArgs e)
        {
            _loggerExtended.Info("board disconnected");
        }

        ~ModbusLaserService()
        {
            if (_client.IsConnected)
                _client.Disconnect();
        }
        public void TryToStart()
        {
            Task.Run(ConnectToBoard, RechargeCancellationToken());
        }

        private async void ConnectToBoard()
        {
            while(!_client.IsConnected)
            {

                await _client.Connect().AwaitWithTimeout(10000, () =>
                {
                    _loggerExtended.Info("Board connected");
                    Task.Run(DoPolling, RechargeCancellationToken());
                },
                () =>
                {
                    _loggerExtended.Exception("Can't connect to laser board");
                });
            }
            
            
        }

        

        private async void DoPolling()
        {
            while(!_cancellationToken.IsCancellationRequested)
            {
                SetErrorCode(await _client.ReadInputRegisters(DeviceID, 0x7, 1)); //Регистр содержит значение кода ошибки лазерной головы; диапазон значений 0x000-0xFFF.

                SetProtectiveWindowTemp(await _client.ReadInputRegisters(DeviceID, 0xA, 1)); //Регистр содержит значение температуры защитного окна; диапазон значений от 0 до 200.
                SetWorkingGasPressure(await _client.ReadInputRegisters(DeviceID, 0xB, 1)); //Регистр содержит значение давления газа; диапазон значений от 0 до 25.
                SetInternalPressure(await _client.ReadInputRegisters(DeviceID, 0xC, 1)); //Регистр содержит значение внутреннего давления; диапазон значений от 0 до 100.
                SetColimLensTemp(await _client.ReadInputRegisters(DeviceID, 0xD, 1)); //Регистр содержит значение температуры коллиматорной линзы; диапазон значений от 0 до 100.
                SetFocusingLensTemp(await _client.ReadInputRegisters(DeviceID, 0xE, 1)); //Регистр содержит значение температуры фокусирующей линзы; диапазон значений от 0 до 100.
                SetMainTemp(await _client.ReadInputRegisters(DeviceID, 0xF, 1)); //Регистр содержит значение общей температуры лазерной головы; диапазон значений от 0 до 100.
                SetLensPosition(await _client.ReadInputRegisters(DeviceID, 0x10, 1)); //Регистр содержит значение положения линз; диапазон значений от 0 до 10.
                SetDirtyLaserRayCoefficient(await _client.ReadInputRegisters(DeviceID, 0x11, 1)); //Регистр содержит значение о рассеянии лазерного луча; диапазон значений от 0 до 1000.
                SetFocalConfiguration(await _client.ReadInputRegisters(DeviceID, 0x12, 1)); //Регистр содержит значение о фокусной конфигурации линз; диапазон значений от 0 до 100.
                SetZPosition(await _client.ReadInputRegisters(DeviceID, 0x13, 1)); //Регистр содержит значение Z позиции; диапазон значений от 0 до 100.
                await Task.Delay(1000);
            }
        }
        private ushort UnwrapRegisterList(List<Register> list)
        {
            if(list.Count > 0)
                return list.First().Value;
            else
            {
                _loggerExtended.Exception("Can't get value through modbus.");
                return 0;
            }
        }

        private ushort dc(ushort value)
        {
            Debug.WriteLine(value);
            return value;
        }

        private void SetErrorCode(List<Register> list)
        {
            LaserInfoModel.CurrentErrorCode = UnwrapRegisterList(list);
        }

        private void SetProtectiveWindowTemp(List<Register> list)
        {
            LaserInfoModel.ProtectiveWindowTemp = UnwrapRegisterList(list);
        }

        private void SetWorkingGasPressure(List<Register> list)
        {
            LaserInfoModel.WorkingGasPressure = UnwrapRegisterList(list);
        }

        private void SetInternalPressure(List<Register> list)
        {
            LaserInfoModel.InternalPressure = UnwrapRegisterList(list);
        }

        private void SetColimLensTemp(List<Register> list)
        {
            LaserInfoModel.ColimLensTemp = UnwrapRegisterList(list);
        }

        private void SetFocusingLensTemp(List<Register> list)
        {
            LaserInfoModel.FocusingLensTemp = UnwrapRegisterList(list);
        }

        private void SetMainTemp(List<Register> list)
        {
            LaserInfoModel.MainTemperature = UnwrapRegisterList(list);
        }

        private void SetLensPosition(List<Register> list)
        {
            LaserInfoModel.LensPosition = UnwrapRegisterList(list);
        }

        private void SetDirtyLaserRayCoefficient(List<Register> list)
        {
            LaserInfoModel.DirtyLaserRayCoefficient = UnwrapRegisterList(list);
        }

        private void SetFocalConfiguration(List<Register> list)
        {
            LaserInfoModel.FocalConfiguration = UnwrapRegisterList(list);
        }

        private void SetZPosition(List<Register> list)
        {
            LaserInfoModel.ZPosition = UnwrapRegisterList(list);
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
        public bool TestRead(ushort address)
        {
            _client.WriteCoils(1, new Collection<Coil> { new Coil { Address = 0, Value = true }, new Coil { Address = 1, Value = false }, new Coil { Address = 2, Value = true } } );
            return _client.ReadCoils(1, 0, 5).Result.First().Value;
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
    }
}
