namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IAxesPos
    {
        double A { get; set; }
        double B { get; set; }
        double C { get; set; }
        double U { get; set; }
        double V { get; set; }
        double W { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }
}