using NUnit.Framework;
using ExtensionBoardService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ExtensionBoardService.Tests
{
    [TestFixture()]
    public class ExtensionBoardTests
    {
        //[Test()]
        //public void AnalogTestTest()
        //{
        //    var ext = new ExtensionBoard();
        //    if (ext.ConnectAsync("192.168.0.110", 502).Result)
        //    {
        //        Random random = new Random(DateTime.Now.Millisecond);

        //        byte[] value = new byte[] { 0 };
        //        random.NextBytes(value);
        //        Assert.IsTrue(ext.SetAnalogOutputLevel(0, value[0]));
        //        Assert.IsTrue(ext.GetAnalogOutputLevel(0, out byte newResult));
        //        Assert.AreEqual(value[0], newResult);
        //        Task.Delay(100).Wait();
        //        Assert.IsTrue(ext.SetAnalogOutputLevel(1, value[0]));
        //        Assert.IsTrue(ext.GetAnalogOutputLevel(1, out newResult));
        //        Assert.AreEqual(value[0], newResult);
        //        Task.Delay(100).Wait();
        //        Debug.WriteLine(value[0]);

        //        random.NextBytes(value);
        //        Assert.IsTrue(ext.SetAnalogOutputLevel(0, value[0]));
        //        Task.Delay(100).Wait();
        //        Assert.IsTrue(ext.SetAnalogOutputLevel(1, value[0]));
        //        Task.Delay(100).Wait();
        //        Debug.WriteLine(value[0]);

        //        ext.DisconnectAsync().Wait();
        //    }
        //    else
        //        Assert.Fail();
        //}
        //[Test]
        //public void DiscreteTest()
        //{
        //    var ext = new ExtensionBoard();
        //    if (ext.ConnectAsync("192.168.0.110", 502).Result)
        //    {
        //        for (ushort i = 0; i < 16; i++)
        //        {
        //            Assert.IsTrue(ext.GetDiscreteOutput(i, out bool oldValue));
        //            Task.Delay(100).Wait();
        //            Assert.IsTrue(ext.SetDiscreteOutput(i, !oldValue));
        //            Task.Delay(100).Wait();
        //            Assert.IsTrue(ext.GetDiscreteOutput(i, out bool newValue));
        //            Assert.AreNotEqual(oldValue, newValue);
        //        }

        //        ext.DisconnectAsync().Wait();
        //    }
        //    else
        //        Assert.Fail();
        //}
        //[Test]
        //public void ResetDiscreteTest()
        //{
        //    var ext = new ExtensionBoard();
        //    if (ext.ConnectAsync("192.168.0.110", 502).Result)
        //    {
        //        Random random = new Random(DateTime.Now.Millisecond);
        //        for (ushort i = 0; i < 16; i++)
        //        {
        //            Assert.IsTrue(ext.SetDiscreteOutput(i, random.Next(0, 1) == 1));
        //            Task.Delay(100).Wait();
        //        }

        //        Assert.IsTrue(ext.ResetAllDiscreteOutput());

        //        for (ushort i = 0; i < 16; i++)
        //        {
        //            Assert.IsTrue(ext.GetDiscreteOutput(i, out bool result));
        //            Task.Delay(100).Wait();
        //            Assert.IsFalse(result);
        //        }

        //        ext.DisconnectAsync().Wait();
        //    }
        //    else
        //        Assert.Fail();
        //}
        //[Test]
        //public void SetDiscreteTest()
        //{
        //    var ext = new ExtensionBoard();
        //    if (ext.ConnectAsync("192.168.0.110", 502).Result)
        //    {
        //        Random random = new Random(DateTime.Now.Millisecond);
        //        for (ushort i = 0; i < 16; i++)
        //        {
        //            Assert.IsTrue(ext.SetDiscreteOutput(i, random.Next(0, 1) == 1));
        //            Task.Delay(100).Wait();
        //        }

        //        Assert.IsTrue(ext.SetAllDiscreteOutput());

        //        for (ushort i = 0; i < 16; i++)
        //        {
        //            Assert.IsTrue(ext.GetDiscreteOutput(i, out bool result));
        //            Task.Delay(100).Wait();
        //            Assert.IsTrue(result);
        //        }

        //        ext.DisconnectAsync().Wait();
        //    }
        //    else
        //        Assert.Fail();
        //}
        //[Test]
        //public void StartupTest()
        //{
        //    var ext = new ExtensionBoard();
        //    if (ext.ConnectAsync("192.168.0.110", 502).Result)
        //    {
        //        for (int i = 0; i < 300; i++)
        //        //if (ext.ConnectAsync("192.168.0.110", 502).Result)
        //        {
        //            ext.ResetAllDiscreteOutput();
        //            ext.SetAnalogOutputLevel(0, 127);
        //            ext.SetAnalogOutputLevel(1, 127);
        //            ext.SetAnalogOutputLevel(0, 150);
        //            ext.SetDiscreteOutput(2, false);
        //            ext.GetAnalogInputLevel(0, out byte value);
        //            ext.GetDiscreteOutput(0, out bool result1);
        //            ext.GetDiscreteOutput(1, out bool result2);
        //            ext.DisconnectAsync().Wait();
        //            Debug.WriteLine(i);
        //        }
        //    }

        //}
    }
}