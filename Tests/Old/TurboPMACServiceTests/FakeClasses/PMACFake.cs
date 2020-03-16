using Infrastructure.AppEventArgs;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using PCOMMSERVERLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TurboPMACServiceTests.FakeClasses
{
    public class PMACFake : IController
    {
        private int _dprSize;

        public PMACFake()
        {
            I7XX0 = new List<int> { 7100, 7400 };
        }

        public bool Connected { get; set; }
        public bool CanAnswer { get; set; }

        public int DPRSize
        {
            get => _dprSize;
            set
            {
                _dprSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DPRSize)));
            }
        }

        public List<int> I7XX0 { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1065:Не вызывайте исключения в непредвиденных местах", Justification = "<Ожидание>")]
        public bool ProgramRunning { get; set; }

        public event EventHandler OnCheckConnection;
        public event EventHandler OnConnect;
        public event EventHandler OnDisconnected;
        public event EventHandler<ControllerErrorEventArgs> OnError;
        public event EventHandler<ControllerLostConnectionEventArgs> OnLostConnection;
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<string> ResponseSended;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        public bool Connect(int deviceNumber = -1, int timeout = 100)
        {
            if (deviceNumber >= 0)
            {
                OnConnect?.Invoke(this, new EventArgs());
                Connected = true;
                return true;
            }
            else
            {
                OnError.Invoke(this, new ControllerErrorEventArgs(100, "Can't connect. Bad device number"));
                return false;
            }
        }
        public void Disconnect()
        {
            OnDisconnected.Invoke(this, new EventArgs());
            Connected = false;
        }
        public void Dispose()
        {

        }
        public bool DPRAvailable()
        {
            return Connected;
        }
        public bool GetMotorCurrent(int motorNumber, out int phaseA, out int phaseB)
        {
            Random random = new Random();
            phaseA = random.Next(250, 4000);
            phaseB = random.Next(phaseA, 4000);

            return true;
        }
        public void GetMotorPosition(int motorNumber, out double position, PositionType positionType)
        {
            Random random = new Random();
            var randomPosition = random.NextDouble() + random.Next(-1000, 1000);
            position = randomPosition == 0 ? 123 : randomPosition;
        }
        public bool GetMotorStatus(int motorNum, out MotorXStatuses nX, out MotorYStatuses nY)
        {
            nX = MotorXStatuses.None;
            nY = MotorYStatuses.None;

            if (motorNum < 0 || motorNum > 8) return false;

            nX = MotorXStatuses.MotorActivated;
            nY = MotorYStatuses.InPositionTrue;
            return true;
        }
        public bool GetResponse(string query, out string result)
        {
            string localResult = result = string.Empty;
            switch (query)
            {
                case string motorBinding when Regex.IsMatch(motorBinding, @"^#(\d+)->$"):
                    localResult = GenerateMotorLetter(int.Parse(motorBinding.Replace("->", "").Replace("#", ""), CultureInfo.InvariantCulture));
                    break;
                case "ver":
                    localResult = "ver";
                    break;
                case string intValueGet when Regex.Match(intValueGet, @"^[IMPimp]\d+$").Success:
                    {
                        int variableValue;
                        intValueGet = intValueGet.Remove(0, 1);
                        if (intValueGet.Contains("$"))
                            variableValue = int.Parse(intValueGet.Trim('$'), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        else
                            variableValue = int.Parse(intValueGet, NumberStyles.Any, CultureInfo.InvariantCulture);
                        localResult = variableValue >= 2000 ? $"${variableValue.ToString(CultureInfo.InvariantCulture)}" : variableValue.ToString(CultureInfo.InvariantCulture);
                    }
                    break;
                case string followingError when Regex.Match(followingError, @"^[fF]\d+$").Success:
                    localResult = "1234";
                    break;

            }
            if (Connected && CanAnswer)
            {
                result = localResult;
                ResponseSended?.Invoke(this, query);
                return true;
            }
            else
                return false;
        }

        private string GenerateMotorLetter(int motorNumber)
        {
            switch (motorNumber)
            {
                case 1:
                    return "X";
                case 2:
                    return "Y";
                case 3:
                    return "Z";
                default:
                    return string.Empty;
            }
        }

        public void RotBufClear(int bufNum)
        {

        }
        public bool RotBufInit()
        {
            return Connected;
        }
        public void RotBufRemove(int bufNum)
        {

        }
        public bool RotBufSendString(string str, int bufNum)
        {
            return bufNum > 0 && bufNum < 8;
        }
        public void RotBufSet(bool on)
        {

        }
        public bool SelectController(out int devNumber)
        {
            devNumber = 0;
            return true;
        }

        private void GenerateConnectionError()
        {
            OnLostConnection?.Invoke(this, new ControllerLostConnectionEventArgs(200));
        }
        private void OnCheckConnectionCall()
        {
            OnCheckConnection?.Invoke(this, new EventArgs());
        }
        public void DoError(long code, string message)
        {
            OnError?.Invoke(this, new ControllerErrorEventArgs(code, message));
        }
        public void DoLostConnection()
        {
            OnLostConnection?.Invoke(this, new ControllerLostConnectionEventArgs(2019));
        }

        public bool GetGlobalStatuses(out GlobalStatusXs nX, out GlobalStatusYs nY)
        {
            throw new NotImplementedException();
        }
    }
}
