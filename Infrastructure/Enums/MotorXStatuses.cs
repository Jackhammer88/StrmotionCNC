using System;

namespace Infrastructure.Enums
{
    //Моторы
    [Flags]
    public enum MotorXStatuses
    {
        None = -1,
        RapidMaxVelocitySelect = 0x1,
        SignMagnitudeServoEn = 0x2,
        SoftwareCaptureEn = 0x4,
        CaptureOnErrorEn = 0x8,
        PosFollowEn = 0x10,
        PosFollowOffsetMode = 0x20,
        CommutationEnable = 0x40,
        YaddrCommuteEnc = 0x80,
        UserWrittenServoEn = 0x100,
        UserWrittenPhaseEn = 0x200,
        HomeSearchInProgress = 0x400,
        BlockRequest = 0x800,
        AbortDecelerationInProgress = 0x1000,
        DesiredVelocityNull = 0x2000,
        DataBlockError = 0x4000,
        DwellInProgress = 0x8000,
        IntegrationMode = 0x10000,
        MoveTimerActive = 0x20000,
        OpenLoopMode = 0x40000,
        AmplifierEn = 0x80000,
        ExtServoAlgoEn = 0x100000,
        PositiveEndLimitSet = 0x200000,
        NegativeEndLimitSet = 0x400000,
        MotorActivated = 0x800000
    }
}
