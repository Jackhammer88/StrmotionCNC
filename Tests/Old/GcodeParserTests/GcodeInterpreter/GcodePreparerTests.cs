using NUnit.Framework;
using System.IO;

namespace GcodeParser.GcodeInterpreter.Tests
{
    [TestFixture]
    [Category("GCode.Preparer")]
    public class GcodePreparerTests
    {
        [Test]
        public void OpenFileTest()
        {
            GCodePreparer preparer = new GCodePreparer();
            var currentDirectory = TestContext.CurrentContext.TestDirectory;
            preparer.OpenFile(Path.Combine(currentDirectory, "testprog.nc"));
            Assert.IsTrue(!string.IsNullOrEmpty(preparer.FileName));
        }

        [Test]
        public void PrepareStringsTest()
        {
            GCodePreparer preparer = new GCodePreparer();
            var currentDirectory = TestContext.CurrentContext.TestDirectory;
            preparer.OpenFile(Path.Combine(currentDirectory, "testprog.nc"));
            preparer.PrepareStrings();
            Assert.IsTrue(preparer.Strings.Count > 0);
            Assert.IsTrue(preparer.StringsPrepared);
        }
    }
}