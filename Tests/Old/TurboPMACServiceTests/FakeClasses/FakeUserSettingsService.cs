using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboPMACServiceTests.FakeClasses
{
    public class FakeUserSettingsService : IUserSettingsService
    {
        public FakeUserSettingsService()
        {
            DeviceNumber = 0;
            MaxCurrentLimit = 16;
            MachineType = (int)Infrastructure.Enums.MachineType.Milling;
            ReconnectionCount = 10;
            Timeout = 100;
            Offsets = new List<IOffsetData> { new FakeOffsetData("G54"), new FakeOffsetData("G55"), new FakeOffsetData("G56"), new FakeOffsetData("G57"), new FakeOffsetData("G58"),
                new FakeOffsetData("G59") };
            ToolOffsets = new List<IToolOffsetData> { new FakeToolOffsetData("T1"), new FakeToolOffsetData("T2"), new FakeToolOffsetData("T3"), new FakeToolOffsetData("T4"),
                    new FakeToolOffsetData("T5"), new FakeToolOffsetData("T6"), new FakeToolOffsetData("T7"), new FakeToolOffsetData("T8"), new FakeToolOffsetData("T9"), new FakeToolOffsetData("T10"),
                        new FakeToolOffsetData("T11"), new FakeToolOffsetData("T12") };
        }

        public int DeviceNumber { get; set; }
        public int MaxCurrentLimit { get; set; }
        public int MachineType { get; set; }
        public int ReconnectionCount { get; set; }
        public int Timeout { get; set; }

        public List<IOffsetData> Offsets { get; }
        public List<IToolOffsetData> ToolOffsets { get; }
        public int ScaleFactor { get; set; }
        public int SafetyXCoordinate { get; set; }
        public int SafetyYCoordinate { get; set; }
        public int SafetyZCoordinate { get; set; }
        public int CSNumber { get; set; }
    }
}
