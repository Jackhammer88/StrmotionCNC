using System.ComponentModel;

namespace Infrastructure.Interfaces.CNCControllerService
{
    public interface IToolOffsetData
    {
        double ToolDiameter { get; set; }
        double ToolLength { get; set; }
        string ToolNumber { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
