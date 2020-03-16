using ControllerService.Gcode.OffsetCalculators;
using NUnit.Framework;
using System.Diagnostics;
using System.Linq;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.GCode.OffsetCalculators
{
    [TestFixture]
    [Category("TurboPMACService")]
    public class GlobalOffsetCalculatorTests
    {
        [Test]
        public void ApplyOffsetsTest()
        {
            GlobalOffsetCalculator globalOffsetCalculator = new GlobalOffsetCalculator();
            var userSettings = new FakeUserSettingsService();
            userSettings.Offsets[0].X = 25.6;
            userSettings.Offsets[0].Y = 25.6;
            userSettings.Offsets[0].Z = 25.6;
            string text = "G1 X-0.000 Y-0.000";
            globalOffsetCalculator.ApplyOffsets(ref text, userSettings.Offsets[0]);
            Debug.WriteLine(text);
        }

        [Test]
        public void ApplyToolOffsetsTest()
        {
            LaserToolOffsetCalculator globalOffsetCalculator = new LaserToolOffsetCalculator();
            var userSettings = new FakeUserSettingsService();
            userSettings.ToolOffsets[0].ToolDiameter = 25.6;
            userSettings.ToolOffsets[0].ToolLength = 21.6;
            string text = "G1 X-0.000 Y-0.000";
            text = globalOffsetCalculator.ApplyOffsets(text, userSettings.ToolOffsets[0]);
            Debug.WriteLine(text);
        }
    }
}
