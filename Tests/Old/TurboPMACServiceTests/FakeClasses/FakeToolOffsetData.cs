using Infrastructure.Interfaces.CNCControllerService;
using System.ComponentModel;

namespace TurboPMACServiceTests.FakeClasses
{
    public class FakeToolOffsetData : IToolOffsetData
    {
        private double _toolDiameter;
        private double _toolLength;
        private string _toolNumber;

        public FakeToolOffsetData(string toolNumber)
        {
            ToolNumber = toolNumber;
        }

        public double ToolDiameter 
        {
            get => _toolDiameter;
            set
            {
                _toolDiameter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToolDiameter)));
            }
        }
        public double ToolLength
        {
            get => _toolLength;
            set
            {
                _toolLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToolLength)));
            }
        }
        public string ToolNumber
        {
            get => _toolNumber;
            set
            {
                _toolNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToolNumber)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
