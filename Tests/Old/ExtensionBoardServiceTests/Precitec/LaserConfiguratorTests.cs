using NUnit.Framework;
using ExtensionBoardService.Precitec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExtensionBoardService.Precitec.Tests
{
    [TestFixture()]
    public class LaserConfiguratorTests
    {
        [Test()]
        public void LaserConfiguratorTest()
        {
            ExtensionBoard board = new ExtensionBoard();
            board.ConnectAsync("192.168.0.110", 502).Wait();
            //board.ResetAllDiscreteOutput();
            for (int i = 0; i < 10; i++)
            {
                board.SetDiscreteOutput(0, false);
                Task.Delay(200).Wait();
                board.SetDiscreteOutput(0, true);
                Task.Delay(1200).Wait();
            }
        }
    }
}