using Infrastructure.Enums;
using System.ComponentModel;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IMotor : INotifyPropertyChanged
    {
        bool Activated { get; set; }
        bool AmplifierFaultError { get; set; }
        string AxisName { get; set; }
        double CurrentPercentage { get; set; }
        bool FatalFollowing { get; set; }
        bool InPosition { get; set; }
        string Letter { get; set; }
        int LetterFactor { get; set; }
        int PhaseA { get; set; }
        int PhaseB { get; set; }
        bool PhaseRequest { get; set; }
        bool PhasingSearchError { get; set; }
        double Position { get; set; }
        double RawPosition { get; set; }
        double PlotPosition { get; set; }
        bool WarningFollowing { get; set; }
        double FollowingError { get; set; }
        MotorXStatuses XStatuses { get; }
        MotorYStatuses YStatuses { get; }
}
}
