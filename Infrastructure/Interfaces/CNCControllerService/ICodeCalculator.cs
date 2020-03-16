using System.Collections.Generic;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface ICodeCalculator
    {
        void GetGCodes(string progStr, Dictionary<int, double?> gGroups);
        void GetMCodes(string progStr, Dictionary<int, int?> mGroup);
        int GetGGroup(double gcode);
        int GetMGroup(int mcode);
        int? GetTCode(string progStr);
    }
}
