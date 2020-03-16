using ControllerService.Modules;
using Infrastructure.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.Modules
{
    //[TestFixture]
    //[Category("TurboPMACService")]
    //public class PmacInformationTests
    //{
    //    [Test]
    //    public void PmacInformationTest()
    //    {
    //        var pmac = new PMACFake();
    //        var userSettings = new FakeUserSettingsService { MachineType = (int)MachineType.Milling, Timeout = 80  };
    //        var logger = new FakeLogger();
    //        var information = new PmacInformation(pmac, userSettings, logger);

    //        bool motorCountChanged = false;

    //        information.ActivatedMotorsCountChanged += (s, e)=> motorCountChanged = true;

    //        pmac.Connect(userSettings.DeviceNumber, userSettings.Timeout);

    //        while (!motorCountChanged) ;
    //        Assert.IsTrue(motorCountChanged);

    //        Task.Delay(100).Wait();

    //        foreach (var motor in information.Motors)
    //        {
    //            if (motor.Activated)
    //            {
    //                //Assert.IsTrue(motor.Position != 0);
    //                Assert.AreNotEqual(0, motor.CurrentPercentage);
    //                Assert.AreNotEqual(0, motor.PhaseA);
    //                Assert.AreNotEqual(0, motor.PhaseB);
    //                Assert.AreNotEqual(string.Empty, motor.Letter);
    //            }
    //        }
    //    }
    //}
}
