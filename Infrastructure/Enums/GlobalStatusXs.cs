using System;

namespace Infrastructure.Enums
{
    [Flags]
    public enum GlobalStatusXs
    {
        None = -1,
        ThisCardAddressedSerially = 1, //Bit 0
        AllCardsAddressedSerially = 0x2, //Bit 1
        Reserved1 = 0x4, //Bit 2
        PhaseClockMissing = 0x8, //Bit 3
        MacroRingError = 0x10, //Bit 4
        MacroCommunicationError = 0x20, //Bit 5
        TWSVariableParityError = 0x40, //Bit 6
        ServoMacroICConfigError = 0x80, //Bit 7
        IllegalLVariableDefinition = 0x100, //Bit 8
        RealTimeInterruptWarning = 0x200, //Bit 9
        EAROMError = 0x400, //Bit 10
        DPRamError = 0x800, //Bit 11
        FirmwareChecksumError = 0x1000, //Bit 12
        GeneralChecksumError = 0x2000, //Bit 13
        CompensateTableOn = 0x4000, //Bit 14
        Internal = 0x8000, //Bit 15
        SmallMemoryTurboPMAC = 0x10000, //Bit 16
        GatherOnExternalTrig = 0x20000, //Bit 17
        Reserver2 = 0x40000, //Bit 18
        DataGatheringEnabled = 0x80000, //Bit 19
        ServoError = 0x100000, //Bit 20
        CPUType1 = 0x200000, //Bit 21
        RTIReEntryError = 0x400000, //Bit 22
        MainError = 0x800000 //Bit 23
    }
}
