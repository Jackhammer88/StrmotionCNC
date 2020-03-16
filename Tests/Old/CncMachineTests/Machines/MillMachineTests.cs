using System;
using System.Linq;
using System.Diagnostics;
using NUnit.Framework;
using System.IO;

namespace CncMachine.Machines.Tests
{
    [TestFixture]
    [Category("MillMachine.Test")]
    public class MillMachineTests
    {
        private static string TestProgramPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testprog.nc");
        [Test]
        public void MillMachineLoadProgramTest()
        {
            var machine = new MillMachine();
            machine.LoadProgram(TestProgramPath);
            machine.NextFrame();
        }

        [Test]
        public void MillMachineLoadProgramAsyncTest()
        {
            var machine = new MillMachine();
            machine.LoadProgramAsync(TestProgramPath).Wait();
            machine.NextFrame();
        }
        [Test]
        public void RewindTest()
        {
            //TODO: доделать тест перемотки программы.
            var machine = new MillMachine();
            machine.LoadProgramAsync(TestProgramPath).Wait();
            for (int i = 0; i < machine.Program.Count / 2; i++)
                machine.NextFrame();
            machine.Rewind(0);
            Assert.AreEqual(0, machine.ToolNumber);
            machine.NextFrame();
            Assert.AreEqual(2, machine.ToolNumber);
        }
        [Test]
        public void MillMachineFullTest()
        {
            var machine = new MillMachine();
            machine.LoadProgramAsync(TestProgramPath).Wait();
            for (int i = 0; i < machine.Program.Count; i++)
                machine.NextFrame();
        }
    }
}