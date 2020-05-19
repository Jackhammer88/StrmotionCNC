using Infrastructure.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IProgramLoader
    {
        int CoordinateSystemNumber { get; }
        bool IsProgramOpened { get; set; }
        bool IsProgramPaused { get; set; }
        bool IsProgramRunning { get; }
        ConcurrentDictionary<int, string> LoadedProgram { get; }
        int MachineType { get; set; }
        int ProgramStringNumber { get; }
        int ProgramLinesCount { get; }
        int StartLine { get; set; }
        ProgramLoaderState CurrentState { get; }

        event EventHandler OnProgramReseted;
        event PropertyChangedEventHandler PropertyChanged;

        Task PrepareProgramAsync();
        Task InitNewRotaryBuffer(bool mdi = false);
        void AbortProgram();
        Task AbortProgramAsync();
        Task<IEnumerable<string>> OpenProgramFileNextAsync(string programPath, CancellationToken cancellationToken);
        Task<string[]> OpenProgramFileAsync(string programPath, CancellationToken cancellationToken);
        Task<string[]> OpenProgramFileAsync(string programPath, int selectedLine, CancellationToken cancellationToken);
        void LoadMDIProgram(IEnumerable<string> programStrings);
        void CycleStart();
        void PauseProgram();
        void ResetProgram();
        void ResumeProgram();
        void ExitFromAuto();
        bool StartProgram(int line = 0);
        string GetProgramLine(int line, int count);
        void AbortMdiProgram();
    }
}