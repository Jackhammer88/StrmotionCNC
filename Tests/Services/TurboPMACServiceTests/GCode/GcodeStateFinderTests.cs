using ControllerService;
using ControllerService.GCode;
using Infrastructure.Interfaces.UserSettingService;
using Moq;
using NUnit.Framework;
using System.Diagnostics;

namespace TurboPMACServiceTests.GCode
{
    //[TestFixture]
    //[Category("TurboPMACService")]
    //public class GcodeStateFinderTests
    //{
    //    [Test]
    //    public void GetPreviousCodesTest()
    //    {
    //        var ncFile = new Mock<NCFile>();
    //        string[] ncProgram = new string[] 
    //        {
    //            "G0 X0 Y0 Z0",  //1
    //            "X20 Y10 Z5",   //2
    //            "G1 X40 Y20",   //3
    //            "X45 Y25",      //4
    //            "X50 Y30",      //5
    //            "G0 X10 Y10 Z0",//6
    //            "X0 Y0 Z0",     //7
    //            "G1 X100 Y2000 Z0",//8
    //            "X23 Y43 Z53",     //9
    //            "X33 Y66 Z99",     //10
    //            "X0 Y0 Z0",     //11
    //            "X50 Y30",     //12
    //            "X70 Y30",     //13
    //            "X0 Y0 Z8",     //14
    //            "G2 X-530 Y555 I-25 J0",//15
    //            "X0 Y0",     //16
    //            "G3 X-480 Y590 I25 J0"//17
    //        };
    //        ncFile.Setup(f => f.GetClearSomeString(It.IsAny<int>(), It.IsAny<int>())).Returns((int int1, int int2) => ncProgram[int1]);
    //        var userSettings = new Mock<IUserSettingsService>();
    //        userSettings.Setup(s => s.SafetyXCoordinate).Returns(40);
    //        userSettings.Setup(s => s.SafetyYCoordinate).Returns(50);
    //        userSettings.Setup(s => s.SafetyZCoordinate).Returns(60);
    //        var frames = GcodeStateFinder.GetPreviousCodes(ncFile.Object, 4, userSettings.Object);

    //        Assert.AreEqual("G0 Z60 ", frames[0].ToString());
    //        Assert.AreEqual("G0 X40 ", frames[1].ToString());
    //        Assert.AreEqual("G0 Y50 ", frames[2].ToString());
    //        Assert.AreEqual("G0 Z5 ", frames[3].ToString());
    //        Assert.AreEqual("G1 X45 Y25 ", frames[4].ToString());


    //        frames = GcodeStateFinder.GetPreviousCodes(ncFile.Object, 5, userSettings.Object);
    //        Assert.AreEqual("G0 Z60 ", frames[0].ToString());
    //        Assert.AreEqual("G0 X40 ", frames[1].ToString());
    //        Assert.AreEqual("G0 Y50 ", frames[2].ToString());
    //        Assert.AreEqual("G0 Z5 ", frames[3].ToString());
    //        Assert.AreEqual("G1 X50 Y30 ", frames[4].ToString());


    //        frames = GcodeStateFinder.GetPreviousCodes(ncFile.Object, 15, userSettings.Object);
    //        Assert.AreEqual("G0 Z60 ", frames[0].ToString());
    //        Assert.AreEqual("G0 X40 ", frames[1].ToString());
    //        Assert.AreEqual("G0 Y50 ", frames[2].ToString());
    //        Assert.AreEqual("G0 Z8 ", frames[3].ToString());
    //        Assert.AreEqual("G2 X-530 I-25 Y555 J0 ", frames[4].ToString());

    //        frames = GcodeStateFinder.GetPreviousCodes(ncFile.Object, 17, userSettings.Object);
    //        Assert.AreEqual("G0 Z60 ", frames[0].ToString());
    //        Assert.AreEqual("G0 X40 ", frames[1].ToString());
    //        Assert.AreEqual("G0 Y50 ", frames[2].ToString());
    //        Assert.AreEqual("G0 Z8 ", frames[3].ToString());
    //        Assert.AreEqual("G3 X-480 I25 Y590 J0 ", frames[4].ToString());
    //    }
    //}
}
