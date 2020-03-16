using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.Logger;
using Infrastructure.Resources.Strings;
using Infrastructure.SharedClasses;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerService.Modules
{
    public class PmacErrorHandler : IControllerErrorHandler
    {
        private readonly IControllerInformation _information;
        private readonly ILoggerFacade _logger;
        private readonly HashSet<string> _globalErrorList;
        private readonly HashSet<Tuple<int, string>> _motorsErrorList;
        private readonly IDisposable _unsubscriber;

        public PmacErrorHandler(IControllerInformation information, ILoggerFacade logger)
        {
            _information = information ?? throw new ArgumentNullException(nameof(information));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _globalErrorList = new HashSet<string>();
            _motorsErrorList = new HashSet<Tuple<int, string>>();

            _unsubscriber = _information.StatusManager.Subscribe(this);
        }

        ~PmacErrorHandler()
        {
            _unsubscriber?.Dispose();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(MachineStatuses value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            CheckGlobalFlagsForErrors(value.GlobalX);
            CheckMotorsFlags(value.MotorsStatuses, value.NumberOfMotors);
        }
        private void CheckGlobalFlagsForErrors(GlobalStatusXs globalX)
        {
            foreach (var error in _globalErrorList)
            {
                if (!globalX.HasFlag((GlobalStatusXs)Enum.Parse(typeof(GlobalStatusXs), error)))
                    _globalErrorList.Remove(error);
            }

            var errorList = new HashSet<string>();
            switch (globalX)
            {
                case GlobalStatusXs.PhaseClockMissing:
                    errorList.Add(nameof(GlobalStatusXs.PhaseClockMissing));
                    break;
                case GlobalStatusXs.MacroRingError:
                    errorList.Add(nameof(GlobalStatusXs.MacroRingError));
                    break;
                case GlobalStatusXs.MacroCommunicationError:
                    errorList.Add(nameof(GlobalStatusXs.MacroCommunicationError));
                    break;
                case GlobalStatusXs.TWSVariableParityError:
                    errorList.Add(nameof(GlobalStatusXs.TWSVariableParityError));
                    break;
                case GlobalStatusXs.ServoMacroICConfigError:
                    errorList.Add(nameof(GlobalStatusXs.TWSVariableParityError));
                    break;
                case GlobalStatusXs.IllegalLVariableDefinition:
                    errorList.Add(nameof(GlobalStatusXs.IllegalLVariableDefinition));
                    break;
                case GlobalStatusXs.EAROMError:
                    errorList.Add(nameof(GlobalStatusXs.EAROMError));
                    break;
                case GlobalStatusXs.DPRamError:
                    errorList.Add(nameof(GlobalStatusXs.DPRamError));
                    break;
                case GlobalStatusXs.FirmwareChecksumError:
                    errorList.Add(nameof(GlobalStatusXs.FirmwareChecksumError));
                    break;
                case GlobalStatusXs.GeneralChecksumError:
                    errorList.Add(nameof(GlobalStatusXs.GeneralChecksumError));
                    break;
                case GlobalStatusXs.ServoError:
                    errorList.Add(nameof(GlobalStatusXs.ServoError));
                    break;
                case GlobalStatusXs.RTIReEntryError:
                    errorList.Add(nameof(GlobalStatusXs.RTIReEntryError));
                    break;
                case GlobalStatusXs.MainError:
                    errorList.Add(nameof(GlobalStatusXs.MainError));
                    break;
                default:
                    break;
            }
            foreach (var error in errorList.Except(_globalErrorList))
            {
                var prop = typeof(GeneralComponentsStrings).GetProperty(error);
                var errorString = prop.GetValue(null, null);
                _logger.Log($"{errorString}", Category.Warn, Priority.High);
            }
            _globalErrorList.UnionWith(errorList);
        }

        private void CheckMotorsFlags(List<Tuple<MotorXStatuses, MotorYStatuses>> motorsStatuses, int motorCount)
        {
            var collectionToRemove = new HashSet<Tuple<int, string>>();
            foreach (var error in _motorsErrorList)
            {
                if (Enum.TryParse(error.Item2, out MotorXStatuses xs) && !motorsStatuses[error.Item1].Item1.HasFlag(xs))
                {
                    collectionToRemove.Add(error);
                }
                if (Enum.TryParse(error.Item2, out MotorYStatuses ys) && !motorsStatuses[error.Item1].Item2.HasFlag(ys))
                {
                    collectionToRemove.Add(error);
                }
            }
            _motorsErrorList.ExceptWith(collectionToRemove);

            var motorsErrorList = new HashSet<Tuple<int, string>>();
            for (int i = 0; i < motorCount; i++)
            {
                if (!motorsStatuses[i].Item1.HasFlag(MotorXStatuses.MotorActivated))
                    continue;
                switch (motorsStatuses[i].Item1)
                {
                    case MotorXStatuses.DataBlockError:
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorXStatuses.DataBlockError)));
                        break;
                    default:
                        break;
                }
                switch (motorsStatuses[i].Item2)
                {
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.WarningFollowingErrorEx):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.WarningFollowingErrorEx)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.FatalFollowingErrorEx):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.FatalFollowingErrorEx)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.AmplifierFaultError):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.AmplifierFaultError)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.I2TAmplifierFaultError):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.I2TAmplifierFaultError)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.IntegratedFatalFollowingError):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.IntegratedFatalFollowingError)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.PhasingSearchError):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.PhasingSearchError)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.StoppedOnPositionLimit):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.StoppedOnPositionLimit)));
                        break;
                    case MotorYStatuses s when s.HasFlag(MotorYStatuses.DesiredPositionLimitStop):
                        motorsErrorList.Add(new Tuple<int, string>(i, nameof(MotorYStatuses.DesiredPositionLimitStop)));
                        break;
                    default:
                        break;
                }

            }

            foreach (var error in motorsErrorList.Except(_motorsErrorList))
            {
                var prop = typeof(GeneralComponentsStrings).GetProperty(error.Item2);
                var errorString = prop.GetValue(null, null);
                _logger.Log($"Motor {error.Item1 + 1}: {errorString}", Category.Warn, Priority.High);
            }
            _motorsErrorList.UnionWith(motorsErrorList);
        }

    }
}
