using System.ComponentModel;

namespace Infrastructure.Interfaces.UserSettingService
{
    public interface IOffsetData
    {
        double A { get; set; }
        double B { get; set; }
        double C { get; set; }
        string Offset { get; }
        double U { get; set; }
        double V { get; set; }
        double W { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        double I { get; }
        double J { get; }
        double K { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
