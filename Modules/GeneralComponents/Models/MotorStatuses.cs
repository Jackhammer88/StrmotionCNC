using Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralComponents.Models
{
    public class MotorStatuses
    {
        private MotorXStatuses _xs;
        private MotorYStatuses _ys;

        public MotorStatuses(MotorXStatuses item1, MotorYStatuses item2)
        {
            _xs = item1;
            _ys = item2;
            SetValues();
        }

        private void SetValues()
        {
            var xNames = Enum.GetNames(typeof(MotorXStatuses));
            var yNames = Enum.GetNames(typeof(MotorYStatuses));
            var props = GetType().GetProperties();
            var xProps = props.Where(p => xNames.Any(n => p.Name.Equals(n, StringComparison.Ordinal)));
            var yProps = props.Where(p => yNames.Any(n => p.Name.Equals(n, StringComparison.Ordinal)));
            foreach (var prop in xProps)
            {
                var value = _xs.HasFlag((MotorXStatuses)Enum.Parse(typeof(MotorXStatuses), prop.Name));
                prop.SetValue(this, value);
            }
            foreach (var prop in yProps)
            {
                var value = _ys.HasFlag((MotorYStatuses)Enum.Parse(typeof(MotorYStatuses), prop.Name));
                prop.SetValue(this, value);
            }
        }

        public bool RapidMaxVelocitySelect { get; set; }
        public bool SignMagnitudeServoEn { get; set; }
        public bool SoftwareCaptureEn { get; set; }
        public bool CaptureOnErrorEn { get; set; }
        public bool PosFollowEn { get; set; }
        public bool PosFollowOffsetMode { get; set; }
        public bool CommutationEnable { get; set; }
        public bool YaddrCommuteEnc { get; set; }
        public bool UserWrittenServoEn { get; set; }
        public bool UserWrittenPhaseEn { get; set; }
        public bool HomeSearchInProgress { get; set; }
        public bool BlockRequest { get; set; }
        public bool AbortDecelerationInProgress { get; set; }
        public bool DesiredVelocityNull { get; set; }
        public bool DataBlockError { get; set; }
        public bool DwellInProgress { get; set; }
        public bool IntegrationMode { get; set; }
        public bool MoveTimerActive { get; set; }
        public bool OpenLoopMode { get; set; }
        public bool AmplifierEn { get; set; }
        public bool ExtServoAlgoEn { get; set; }
        public bool PositiveEndLimitSet { get; set; }
        public bool NegativeEndLimitSet { get; set; }
        public bool MotorActivated { get; set; }
        public bool InPositionTrue { get; set; }
        public bool WarningFollowingErrorEx { get; set; }
        public bool FatalFollowingErrorEx { get; set; }
        public bool AmplifierFaultError { get; set; }
        public bool BacklashDirectionFlag { get; set; }
        public bool I2TAmplifierFaultError { get; set; }
        public bool IntegratedFatalFollowingError { get; set; }
        public bool TriggerMove { get; set; }
        public bool PhasingSearchError { get; set; }
        public bool MotorPhaseRequest { get; set; }
        public bool HomeComplete { get; set; }
        public bool StoppedOnPositionLimit { get; set; }
        public bool DesiredPositionLimitStop { get; set; }
        public bool ForegroundInPosition { get; set; }
        public bool AssignedToCS { get; set; }
        public bool CSAxisDefinitionBit0 { get; set; }
        public bool CSAxisDefinitionBit1 { get; set; }
        public bool CSAxisDefinitionBit2 { get; set; }
        public bool CSAxisDefinitionBit3 { get; set; }
        public bool CS1Bit0 { get; set; }
        public bool CS1Bit1 { get; set; }
        public bool CS1Bit2 { get; set; }
        public bool CS1Bit3 { get; set; }
    }
}
