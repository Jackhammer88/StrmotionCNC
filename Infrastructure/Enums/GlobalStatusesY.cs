using System;

namespace Infrastructure.Enums
{
    [Flags]
    public enum GlobalStatusYs
    {
        None = 0,
        AbortInput = 1, //Bit 0
        Reserved1 = 0x2, //Bit 1
        Reserved2 = 0x4, //Bit 2
        MacroRingSynchPacketFault = 0x8, //Bit 3
        MacroRingBreak = 0x10, //Bit 4
        MacroRingRcvdBreakMsg = 0x20, //Bit 5
        Reserved3 = 0x40, //Bit 6
        Reserved4 = 0x80, //Bit 7
        ModbusActive = 0x100, //Bit 8
        RingActive = 0x200, //Bit 9
        MacroRingTestEnable = 0x400, //Bit 10
        FixedBufferFull = 0x800, //Bit 11
        MacroAuxiliaryMode = 0x1000, //Bit 12
        MacroServoClockSync = 0x2000, //Bit 13
        Internal1 = 0x4000, //Bit 14
        Internal2 = 0x8000, //Bit 15
        UMACTurbo = 0x10000, //Bit 16
        PLCBufferOpen = 0x20000, //Bit 17
        ASCIIRotaryBufferOpen = 0x40000, //Bit 18
        MotionBufferOpen = 0x80000, //Bit 19
        BinaryRotaryBufferOpen = 0x100000, //Bit 20
        CPUType = 0x200000, //Bit 21
        TurboVME = 0x400000, //Bit 22
        TurboUltralite = 0x800000 //Bit 23
    }
}
