using Infrastructure.Interfaces.CNCControllerService;
using System.Collections.Generic;

namespace Infrastructure.Interfaces.UserSettingService
{
    public interface IUserSettingsService
    {
        int DeviceNumber { get; set; }
        int Timeout { get; set; }
        int ReconnectionCount { get; set; }
        int CSNumber { get; set; }
        List<IOffsetData> Offsets { get; }
        List<IToolOffsetData> ToolOffsets { get; }
        int MaxCurrentLimit { get; set; }
        int MachineType { get; set; }
        int ScaleFactor { get; set; }
        int SafetyXCoordinate { get; set; }
        int SafetyYCoordinate { get; set; }
        int SafetyZCoordinate { get; set; }
    }
}
