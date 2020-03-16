namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IAxisInfo
    {
        double AxisLoadPercentage { get; set; }
        string AxisName { get; set; }
        double AxisPosition { get; set; }
        bool InPositionTrue { get; set; }
        int MotorNumber { get; set; }
    }

}
