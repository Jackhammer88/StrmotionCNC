using Infrastructure.Enums;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralComponents.Models
{
    public class GlobalStatuses : BindableBase
    {
        private bool _allCardsAddressedSerially;
        private bool _phaseClockMissing;
        private bool _macroRingError;
        private bool _macroCommunicationError;
        private bool _tWSVariableParityError;
        private bool _servoMacroICConfigError;
        private bool _illegalLVariableDefinition;
        private bool _realTimeInterruptWarning;
        private bool _eAROMError;
        private bool _dPRamError;
        private bool _firmwareChecksumError;
        private bool _generalChecksumError;
        private bool _compensateTableOn;
        private bool _smallMemoryTurboPMAC;
        private bool _gatherOnExternalTrig;
        private bool _dataGatheringEnabled;
        private bool _servoError;
        private bool _rTIReEntryError;
        private bool _mainError;
        private bool _binaryRotaryBufferOpen;
        private bool _motionBufferOpen;
        private bool _aSCIIRotaryBufferOpen;
        private bool _pLCBufferOpen;
        private bool _macroAuxiliaryMode;
        private bool _fixedBufferFull;
        private bool _macroRingTestEnable;
        private bool _ringActive;
        private bool _modbusActive;
        private bool _macroRingRcvdBreakMsg;
        private bool _macroRingBreak;
        private bool _macroRingSynchPacketFault;
        private bool _abortInput;
        private bool _thisCardAddressedSerially;
        private bool _macroServoClockSync;
        private GlobalStatusXs _globalX;
        private GlobalStatusYs _globalY;

        public GlobalStatuses(GlobalStatusXs globalX, GlobalStatusYs globalY)
        {
            _globalX = globalX;
            _globalY = globalY;
            var xNames = Enum.GetNames(typeof(GlobalStatusXs));
            var yNames = Enum.GetNames(typeof(GlobalStatusYs));
            var props = GetType().GetProperties();
            var xProps = props.Where(p =>  xNames.Any(n => n.Equals(p.Name, StringComparison.Ordinal)));
            var yProps = props.Where(p =>  yNames.Any(n => n.Equals(p.Name, StringComparison.Ordinal)));

            foreach (var prop in xProps)
            {
                var value = _globalX.HasFlag((GlobalStatusXs)Enum.Parse(typeof(GlobalStatusXs), prop.Name));
                prop.SetValue(this, value);
            }

            foreach (var prop in yProps)
            {
                var value = _globalY.HasFlag((GlobalStatusYs)Enum.Parse(typeof(GlobalStatusYs), prop.Name));
                prop.SetValue(this, value);
            }
        }

        public bool ThisCardAddressedSerially
        {
            get => _thisCardAddressedSerially;
            set => SetProperty(ref _thisCardAddressedSerially, value);
        }
        public bool AllCardsAddressedSerially
        {
            get => _allCardsAddressedSerially;
            set => SetProperty(ref _allCardsAddressedSerially, value);
        }
        public bool PhaseClockMissing
        {
            get => _phaseClockMissing;
            set => SetProperty(ref _phaseClockMissing, value);
        }
        public bool MacroRingError
        {
            get => _macroRingError;
            set => SetProperty(ref _macroRingError, value);
        }
        public bool MacroCommunicationError
        {
            get => _macroCommunicationError;
            set => SetProperty(ref _macroCommunicationError, value);
        }
        public bool TWSVariableParityError
        {
            get => _tWSVariableParityError;
            set => SetProperty(ref _tWSVariableParityError, value);
        }
        public bool ServoMacroICConfigError
        {
            get => _servoMacroICConfigError;
            set => SetProperty(ref _servoMacroICConfigError, value);
        }
        public bool IllegalLVariableDefinition
        {
            get => _illegalLVariableDefinition;
            set => SetProperty(ref _illegalLVariableDefinition, value);
        }
        public bool RealTimeInterruptWarning
        {
            get => _realTimeInterruptWarning;
            set => SetProperty(ref _realTimeInterruptWarning, value);
        }
        public bool EAROMError
        {
            get => _eAROMError;
            set => SetProperty(ref _eAROMError, value);
        }
        public bool DPRamError
        {
            get => _dPRamError;
            set => SetProperty(ref _dPRamError, value);
        }
        public bool FirmwareChecksumError
        {
            get => _firmwareChecksumError;
            set => SetProperty(ref _firmwareChecksumError, value);
        }
        public bool GeneralChecksumError
        {
            get => _generalChecksumError;
            set => SetProperty(ref _generalChecksumError, value);
        }
        public bool CompensateTableOn
        {
            get => _compensateTableOn;
            set => SetProperty(ref _compensateTableOn, value);
        }
        public bool SmallMemoryTurboPMAC
        {
            get => _smallMemoryTurboPMAC;
            set => SetProperty(ref _smallMemoryTurboPMAC, value);
        }
        public bool GatherOnExternalTrig
        {
            get => _gatherOnExternalTrig;
            set => SetProperty(ref _gatherOnExternalTrig, value);
        }
        public bool DataGatheringEnabled
        {
            get => _dataGatheringEnabled;
            set => SetProperty(ref _dataGatheringEnabled, value);
        }
        public bool ServoError
        {
            get => _servoError;
            set => SetProperty(ref _servoError, value);
        }
        public bool RTIReEntryError
        {
            get => _rTIReEntryError;
            set => SetProperty(ref _rTIReEntryError, value);
        }
        public bool MainError
        {
            get => _mainError;
            set => SetProperty(ref _mainError, value);
        }

        public bool BinaryRotaryBufferOpen
        {
            get => _binaryRotaryBufferOpen;
            set => SetProperty(ref _binaryRotaryBufferOpen, value);
        }
        public bool MotionBufferOpen
        {
            get => _motionBufferOpen;
            set => SetProperty(ref _motionBufferOpen, value);
        }
        public bool ASCIIRotaryBufferOpen
        {
            get => _aSCIIRotaryBufferOpen;
            set => SetProperty(ref _aSCIIRotaryBufferOpen, value);
        }
        public bool PLCBufferOpen
        {
            get => _pLCBufferOpen;
            set => SetProperty(ref _pLCBufferOpen, value);
        }
        public bool MacroServoClockSync
        {
            get => _macroServoClockSync;
            set => SetProperty(ref _macroServoClockSync, value);
        }
        public bool MacroAuxiliaryMode
        {
            get => _macroAuxiliaryMode;
            set => SetProperty(ref _macroAuxiliaryMode, value);
        }
        public bool FixedBufferFull
        {
            get => _fixedBufferFull;
            set => SetProperty(ref _fixedBufferFull, value);
        }
        public bool MacroRingTestEnable
        {
            get => _macroRingTestEnable;
            set => SetProperty(ref _macroRingTestEnable, value);
        }
        public bool RingActive
        {
            get => _ringActive;
            set => SetProperty(ref _ringActive, value);
        }
        public bool ModbusActive
        {
            get => _modbusActive;
            set => SetProperty(ref _modbusActive, value);
        }
        public bool MacroRingRcvdBreakMsg
        {
            get => _macroRingRcvdBreakMsg;
            set => SetProperty(ref _macroRingRcvdBreakMsg, value);
        }
        public bool MacroRingBreak
        {
            get => _macroRingBreak;
            set => SetProperty(ref _macroRingBreak, value);
        }
        public bool MacroRingSynchPacketFault
        {
            get => _macroRingSynchPacketFault;
            set => SetProperty(ref _macroRingSynchPacketFault, value);
        }
        public bool AbortInput
        {
            get => _abortInput;
            set => SetProperty(ref _abortInput, value);
        }
    }
}
