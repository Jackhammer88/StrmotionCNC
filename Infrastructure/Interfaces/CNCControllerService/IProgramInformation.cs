using System.Collections.Concurrent;
using System.ComponentModel;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IProgramInformation : INotifyPropertyChanged
    {
        ConcurrentDictionary<int, string> LoadedProgram { get; }
        int ProgramStringNumber { get; }
        string[] GetProgramString(int strNumber, int count = 1);
        bool IsProgramRunning { get; }
        bool IsProgramPaused { get; }
    }
}
