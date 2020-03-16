using ControllerService.Gcode.OffsetCalculators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.Modules
{
    [TestFixture]
    [Category("TurboPMACService")]
    public class PmacProgramLoaderTests
    {
        [Test]
        public void OffsetCalculatorsTest()
        {
            var offsetData = new FakeOffsetData("")
            {
                X = 1.1,
                Y = 2.2,
                Z = 3.3
            };
            string[] testStrings =
                {
                    "G0 X100 Y10 Z0",
                    "G1 X-10 Y-100 Z-1000",
                    "G1 X-10.253 Y-88.123 Z-0.245",
                    "G2 X10 Y0 I-5 J-5",
                    "G2 X-10 Y-10 I-5 J-5",
                    "G2 X10 Y10 I5 J5",
                    "G2 X10.234 Y0.234 I5.234 J5.678",
                    "G3 X10 Y0 I-5 J-5",
                    "G3 X-10 Y-10 I-5 J-5",
                    "G3 X10 Y10 I5 J5",
                    "G3 X10.234 Y0.234 I5.234 J5.678"
                };

            GlobalOffsetCalculator calculator = new GlobalOffsetCalculator();
            calculator.ApplyOffsets(ref testStrings[0], offsetData);
            Assert.AreEqual(testStrings[0], "G0 X101.1 Y12.2 Z3.3");

            calculator.ApplyOffsets(ref testStrings[1], offsetData);
            Assert.AreEqual(testStrings[1], "G1 X-8.9 Y-97.8 Z-996.7");

            calculator.ApplyOffsets(ref testStrings[2], offsetData);
            Debug.WriteLine($"{testStrings[2]}\nG1 X‭-9.153‬ Y-85.923 Z3.055");
            Assert.IsTrue(string.Equals(testStrings[2], "G1 X‭-9.153‬ Y-85.923 Z3.055", StringComparison.InvariantCultureIgnoreCase));

            calculator.ApplyOffsets(ref testStrings[3], offsetData);
            Assert.IsTrue(string.Equals(testStrings[3], "G2 X11.1 Y2.2 I-3.9 J-2.8", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[3]} but {"G2 X11.1 Y2.2 I-3.9 J-2.8"}");

            calculator.ApplyOffsets(ref testStrings[4], offsetData);
            Assert.IsTrue(string.Equals(testStrings[4], "G2 X-8.9 Y-7.8 I-3.9 J-2.8", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[4]} but {"G2 X-8.9 Y-7.8 I-3.9 J-2.8"}");

            calculator.ApplyOffsets(ref testStrings[5], offsetData);
            Assert.IsTrue(string.Equals(testStrings[5], "G2 X11.1 Y12.2 I6.1 J7.2", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[5]} but {"G2 X11.1 Y12.2 I6.1 J7.2"}");

            calculator.ApplyOffsets(ref testStrings[6], offsetData);
            Assert.IsTrue(string.Equals(testStrings[6], "G2 X11.334 Y2.434 I6.334 J7.878", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[6]} but {"G2 X11.334 Y2.434 I6.334 J7.878"}");

            calculator.ApplyOffsets(ref testStrings[7], offsetData);
            Assert.IsTrue(string.Equals(testStrings[7], "G3 X11.1 Y2.2 I-3.9 J-2.8", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[7]} but { "G3 X11.1 Y2.2 I-3.9 J-2.8" }");

            calculator.ApplyOffsets(ref testStrings[8], offsetData);
            Assert.IsTrue(string.Equals(testStrings[8], "G3 X-8.9 Y-7.8 I-3.9 J-2.8", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[8]} but { "G3 X-8.9 Y-7.8 I-3.9 J-2.8" }");

            calculator.ApplyOffsets(ref testStrings[9], offsetData);
            Assert.IsTrue(string.Equals(testStrings[9], "G3 X11.1 Y12.2 I6.1 J7.2", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[9]} but { "G3 X11.1 Y12.2 I6.1 J7.2" }");
            
            calculator.ApplyOffsets(ref testStrings[10], offsetData);
            Assert.IsTrue(string.Equals(testStrings[10], "G3 X11.334 Y2.434 I6.334 J7.878", StringComparison.InvariantCultureIgnoreCase), $"{testStrings[10]} but { "G3 X11.334 Y2.434 I6.334 J7.878" }");
            
        }
    }
}
