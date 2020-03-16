using System;

namespace Infrastructure.Enums
{
    //Моторы
    [Flags]
    public enum MotorYStatuses
    {
        None = 0,
        InPositionTrue = 0x1,
        WarningFollowingErrorEx = 0x2,
        FatalFollowingErrorEx = 0x4,
        AmplifierFaultError = 0x8,
        BacklashDirectionFlag = 0x10,
        I2TAmplifierFaultError = 0x20,
        IntegratedFatalFollowingError = 0x40,
        TriggerMove = 0x80,
        PhasingSearchError = 0x100,
        MotorPhaseRequest = 0x200,
        HomeComplete = 0x400,
        StoppedOnPositionLimit = 0x800,
        DesiredPositionLimitStop = 0x1000,
        ForegroundInPosition = 0x2000,
        AssignedToCS = 0x8000,
        CSAxisDefinitionBit0 = 0x10000,
        CSAxisDefinitionBit1 = 0x20000,
        CSAxisDefinitionBit2 = 0x40000,
        CSAxisDefinitionBit3 = 0x80000,
        CS1Bit0 = 0x100000,
        CS1Bit1 = 0x200000,
        CS1Bit2 = 0x400000,
        CS1Bit3 = 0x800000
    }
}
