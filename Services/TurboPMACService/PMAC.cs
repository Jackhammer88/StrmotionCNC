//-----------------------------------------------------------------------
// <copyright file="D:\git\Cnc_prism\Services\ControllerService\PMAC.cs" company="Alexey Morkovin">
//     Author:  Alexey Morkovin
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using ControllerService.PmacHelpers;
using Infrastructure.AppEventArgs;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using PCOMMSERVERLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TurboPMACService.Resources;

namespace ControllerService
{

    public sealed class PMAC : IDisposable, INotifyPropertyChanged, IController
    {
        private CancellationTokenSource _canncellationTokenS;
        private int _checkConTimeout = 100;
        private bool _connected;
        private readonly PmacDeviceClass _controller;
        private int _deviceNumber = -1;
        private Lazy<List<int>> _i7000;
        private object _locker = new object();

        private Task _tCheckCon;

        #region Список кодов ошибок
        /*
         * 100 - Ошибка возникла при попытке подключения. Невозможно подключиться к контроллеру.
         * 200 - Отмена подключения.
         * 300 - Неверно указан код устройства
         * 400 - Возникла ошибка при отправке запроса
         */
        #endregion

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        public List<int> I7XX0 => _i7000.Value;

        #region Конструкторы и деструктор
        public PMAC()
        {
            _i7000 = new Lazy<List<int>>(() => GetI7000_Values());
            _controller = new PmacDeviceClass();
            _canncellationTokenS = new CancellationTokenSource();
        }

        ~PMAC()
        {
            if (_deviceNumber != -1)
                _controller.Close(_deviceNumber);
            Dispose();
        }
        #endregion
        #region Внутренние методы
        /// <summary>
        /// Асинхронный метод проверки состояния подключения
        /// </summary>
        private void DoConnectionCheck()
        {
            int status = -1;
            string result = string.Empty;
            try
            {
                while (!_canncellationTokenS.IsCancellationRequested)
                {

                    lock (_locker)
                    {
                        try
                        {
                            _controller.GetResponseEx(_deviceNumber, "ver", false, out result, out status);
                        }
                        catch (COMException) { }
                    }
                    byte responseStatus = (byte)((status & 0xF0000000) >> 28);
                    int receivedLength = status & 0x0FFFFFFF;

                    switch (responseStatus)
                    {
                        case 0x8: //COMM_EOT - OK
                            if (result.Length != receivedLength)
                                OnError(this, new ControllerErrorEventArgs(400, TurbopmacStrings.PollingError));
                            break;
                        case 0xC: //COMM_TIMEOUT
                        case 0xD: //COMM_BADCKSUM
                        case 0xE: //COMM_ERROR
                        case 0xF: //COMM FAIL
                        case 0x70: //COMM_ANYERR
                        case 0x10:
                        default:
                            OnError(this, new ControllerErrorEventArgs(400, TurbopmacStrings.PollingError));
                            break;
                    }

                    Task.Delay(_checkConTimeout).Wait();
                }
            }
            catch (InvalidOperationException)
            {
                Disconnect(true);
                return;
            }
        }
        private void Disconnect(bool lostConnection)
        {
            if (Connected)
            {
                _connected = false;
                _controller.Close(_deviceNumber);
                if (_canncellationTokenS != null && !_canncellationTokenS.IsCancellationRequested)
                    _canncellationTokenS.Cancel();
            }
            if (lostConnection)
                OnLostConnection(this, new ControllerLostConnectionEventArgs(2019));
            else
                OnDisconnected(this, new EventArgs());
        }
        /// <summary>
        /// Возвращает значения переменных I7XX0.
        /// </summary>
        /// <param name="i7000">Значения I7XX0.</param>
        /// <returns>Успешность операции.</returns>
        private List<int> GetI7000_Values()
        {
            var result = new List<int> { 0, 0 };
            if (Connected && GetResponse("I4900", out string sI4900))
                result = I7xx0AddressChecker.ParseAddresses(sI4900);

            return result;
        }

        #endregion
        #region Внешние методы
        /// <summary>
        /// Метод подключения к контроллеру
        /// </summary>
        /// <param name="timeout">Таймаут проверки соединения</param>
        /// <returns></returns>
        public bool Connect(int deviceNumber = -1, int timeout = 100)
        {
            if (Connected)
                return true;

            _deviceNumber = deviceNumber;

            _checkConTimeout = timeout;
            if (_deviceNumber == -1)
                return false;

            bool result = false;
            try
            {
                _controller.Open(_deviceNumber, out result);
            }
            catch (ArgumentException)
            { // Если выбран неверный номер устройства
                OnError.Invoke(this, new ControllerErrorEventArgs(300, TurbopmacStrings.InvalidDeviceNumber));
                return false;
            }

            if (result) ///Подключение успешно?
            {
                _canncellationTokenS = new CancellationTokenSource();
                _tCheckCon = new Task(DoConnectionCheck, _canncellationTokenS.Token, TaskCreationOptions.LongRunning);

                _tCheckCon.Start();
                _connected = result;
                _i7000 = new Lazy<List<int>>(() => GetI7000_Values());
                //if (GetI7000_Values(out List<int> rI7000))
                //    _i7000 = rI7000;
                OnConnect(this, new EventArgs());
                return true;
            }
            OnError.Invoke(this, new ControllerErrorEventArgs(100, TurbopmacStrings.ConnectionError));
            return false;
        }

        /// <summary>
        /// Разрывает соединение
        /// </summary>
        public void Disconnect()
        {
            Disconnect(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="devNumber"></param>
        /// <returns></returns>
        public bool SelectController(out int devNumber)
        {
            _controller.SelectDevice(0, out int dNum, out bool selectResult);
            devNumber = dNum;
            return selectResult;
        }

        /// <summary>
        /// Посылает запрос в контроллер и возвращает успешность выполнения операции.
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <param name="result">Ответ</param>
        /// <returns></returns>
        public bool GetResponse(string query, out string result)
        {
            result = string.Empty;
            if (!Connected)
                return false;

            int status = 0x70000000;
            lock (_locker)
            {
                try
                {
                    _controller.GetResponseEx(_deviceNumber, query, false, out result, out status);
                }
                catch (COMException) { }
            }
            byte responseStatus = (byte)((status & 0xF0000000) >> 28);
            int receivedLength = status & 0x0FFFFFFF;
            if (responseStatus == 0x8 && result.Length == receivedLength)
            {
                result = result.Trim('\n', '\r');
                return true;
            }

            switch (responseStatus)
            {
                case 0x8: //COMM_EOT - OK
                    if (result.Length != receivedLength)
                        OnError(this, new ControllerErrorEventArgs(400, $"The received length wasn't equal to an answer. Command: {query}"));
                    break;
                case 0xC: //COMM_TIMEOUT
                    OnError(this, new ControllerErrorEventArgs(400, $"Communication error - timeout. Command: {query}"));
                    break;
                case 0xD: //COMM_BADCKSUM
                    break;
                case 0xE: //COMM_ERROR
                    OnError(this, new ControllerErrorEventArgs(400, $"Communication error. Command: {query}"));
                    break;
                case 0xF: //COMM FAIL
                    OnError(this, new ControllerErrorEventArgs(400, $"Communication fail. Command: {query}"));
                    break;
                case 0x70: //COMM_ANYERR
                    OnError(this, new ControllerErrorEventArgs(400, $"Another error. Command: {query}"));
                    break;
                case 0x10:
                    OnError(this, new ControllerErrorEventArgs(400, $"Communication UNSOLICITED message. Command: {query}"));
                    break; //COM_UNSOLICITED
                default:
                    OnError(this, new ControllerErrorEventArgs(400, $"Unknown error during requesting to the controller. Command: {query}"));
                    break;
            }
            return false;
        }
        /// <summary>
        /// Очищает указанный rotary буфер.
        /// </summary>
        /// <param name="bufNum">Номер буфера</param>
        public void RotBufClear(int bufNum)
        {
            if (Connected) _controller.DPRRotBufClr(_deviceNumber, bufNum);
        }
        /// <summary>
        /// Удаляет указанный rotary буфер.
        /// </summary>
        /// <param name="bufNum"></param>
        public void RotBufRemove(int bufNum)
        {
            if (Connected) _controller.DPRRotBufRemove(_deviceNumber, bufNum);
        }
        /// <summary>
        /// Инициализирует rotary буфер.
        /// </summary>
        /// <returns>Успешность операции</returns>
        public bool RotBufInit()
        {
            if (!Connected)
                return false;
            _controller.DPRRotBufInit(_deviceNumber, out bool result);
            return result;
        }

        public void RotBufSet(bool on)
        {
            if (Connected) _controller.DPRSetRotBuf(_deviceNumber, on);
        }

        /// <summary>
        /// Записывает в rotary буфер строку.
        /// </summary>
        /// <param name="str">Строка для записи</param>
        /// <param name="bufNum">Номер буфера</param>
        public bool RotBufSendString(string str, int bufNum)
        {
            if (Connected)
            {
                _controller.DPRAsciiStrToRot(_deviceNumber, str, bufNum, out int status);
                if (status == 0)
                    return true;
            }
            return false;
        }

        public bool DPRAvailable()
        {
            return _controller.DPRAvailable[_deviceNumber];
        }
        /// <summary>
        /// Получает флаги состояния указанного мотора.
        /// </summary>
        /// <param name="motorNum">Номер мотора</param>
        /// <param name="nX">Выходные флаги X.</param>
        /// <param name="nY">Выходные флаги Y.</param>
        /// <returns></returns>
        public bool GetMotorStatus(int motorNum, out MotorXStatuses nX, out MotorYStatuses nY)
        {
            nX = MotorXStatuses.None;
            nY = MotorYStatuses.None;
            if (!Connected)
                return false;

            bool succes;
            try
            {
                _controller.GetMotorStatus(_deviceNumber, motorNum, out int sx, out int sy, out succes);
                nX = (MotorXStatuses)sx;
                nY = (MotorYStatuses)sy;
            }
            catch (COMException)
            {
                OnError(this, new ControllerErrorEventArgs(0, TurbopmacStrings.MotorStatusPollingError));
                return false;
            }
            return succes;
        }
        /// <summary>
        /// Возвращает позицию выбранного мотора.
        /// </summary>
        /// <param name="motorNumber">Номер мотора.</param>
        /// <param name="position">Позиция мотора.</param>
        public void GetMotorPosition(int motorNumber, out double position, PositionType positionType)
        {
            position = 0;
            if (!Connected)
                return;

            try
            {
                switch (positionType)
                {
                    case PositionType.ActualPosition:
                        _controller.GetPosition(_deviceNumber, motorNumber, 1, out position);
                        break;
                    case PositionType.CommandedPosition:
                        _controller.GetCommandedPos(_deviceNumber, motorNumber, 1, out position);
                        //GetComandedPosition(motorNumber, 1, out position);
                        break;
                    case PositionType.TargetPosition:
                        GetTargetPosition(motorNumber, 1, out position);
                        break;
                    default:
                        break;
                }
            }
            catch (COMException)
            {
                OnError(this, new ControllerErrorEventArgs(0, TurbopmacStrings.MotorPositionPollingError));
            }
        }

        private void GetTargetPosition(int motorNumber, int scale, out double position)
        {
            position = 0;
            if (GetResponse($"M{motorNumber + 1}63", out string targetPositionAnswer) && GetResponse($"I{motorNumber + 1}08", out string ix08Answer))
            {
                double targetPositionBeforeCalculating = double.Parse(targetPositionAnswer, CultureInfo.InvariantCulture);
                int ix08 = int.Parse(ix08Answer, CultureInfo.InvariantCulture);
                position = targetPositionBeforeCalculating / (ix08 * 32) / scale;
            }
        }

        void GetComandedPosition(int motorNumber, int scale, out double position)
        {
            position = 0;
            if (GetResponse($"M{motorNumber + 1}61", out string targetPositionAnswer) && GetResponse($"I{motorNumber + 1}08", out string ix08Answer))
            {
                double targetPositionBeforeCalculating = double.Parse(targetPositionAnswer, CultureInfo.InvariantCulture);
                int ix08 = int.Parse(ix08Answer, CultureInfo.InvariantCulture);
                position = targetPositionBeforeCalculating / (ix08 * 32) / scale;
            }
        }

        /// <summary>
        /// Возвращает токи фаз выбранного мотора.
        /// </summary>
        /// <param name="motorNumber">Номер мотора.</param>
        /// <param name="phaseA">Тока фазы А.</param>
        /// <param name="phaseB">Ток фазы Б.</param>
        /// <returns></returns>
        public bool GetMotorCurrent(int motorNumber, out int phaseA, out int phaseB)
        {
            phaseA = phaseB = 0;
            if (!Connected)
                return false;
            bool success1 = false, success2 = false;
            if (GetResponse($"M{motorNumber + 1}05", out string result))
                success1 = int.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out phaseA);
            if (GetResponse($"M{motorNumber + 1}06", out result))
                success2 = int.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out phaseB);
            return success1 && success2;
        }
        public bool GetGlobalStatuses(out GlobalStatusXs nX, out GlobalStatusYs nY)
        {


            nX = GlobalStatusXs.None;
            nY = GlobalStatusYs.None;
            if (!Connected)
                return false;

            bool success;
            try
            {
                _controller.GetGlobalStatus(_deviceNumber, out int sx, out int sy, out success);
                nX = (GlobalStatusXs)sx;
                nY = (GlobalStatusYs)sy;
            }
            catch (COMException)
            {
                OnError(this, new ControllerErrorEventArgs(0, TurbopmacStrings.DuringGlobalStatusGetting));
                return false;
            }
            return success;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Методы Dispose должны вызывать SuppressFinalize", Justification = "<Ожидание>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public void Dispose()
        {
            try { _controller.Close(_deviceNumber); } catch { }
            _canncellationTokenS?.Cancel();
            if (_tCheckCon != null && _tCheckCon.IsCompleted)
                _tCheckCon.Dispose();
            _canncellationTokenS.Dispose();
        }

        #endregion
        #region Свойства
        public bool Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, value);
        }
        public int DPRSize => Connected ? _controller.DPRSize[_deviceNumber] : -1;
        public bool ProgramRunning => _controller.ProgramRunning[_deviceNumber, 1];

        #endregion
        public event EventHandler OnCheckConnection = delegate { };
        public event EventHandler OnConnect = delegate { };
        public event EventHandler OnDisconnected = delegate { };
        public event EventHandler<ControllerErrorEventArgs> OnError = delegate { };
        public event EventHandler<ControllerLostConnectionEventArgs> OnLostConnection = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

    }
}
