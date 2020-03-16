using ControllerService.Modules;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.Modules
{
    [TestFixture]
    [Category("TurboPMACService")]
    public class PmacJogManagerTests
    {
        [Test]
        public void PmacJogManagerTest()
        {
            //public PmacJogManager(IController controller, IControllerInformation controllerInformation, IControllerConfigurator controllerConfigurator)
            var pmac = new PMACFake();
            var logger = new FakeLogger();
            var settings = new FakeUserSettingsService();
            var information = new PmacInformation(pmac, settings, logger);
            var configurator = new PmacConfigurator(pmac, logger);
            var jogManager = new PmacJogManager(pmac, information, configurator);
        }

        [Test]
        public void JogIncrementallyTest()
        {
            PrepareToJotTest(out Random random, out PmacJogManager jogManager);

            foreach (var letter in "XYZABCUVW")
            {
                jogManager.JogIncrementally(letter.ToString(CultureInfo.InvariantCulture), random.Next(0, int.MaxValue), false, random.Next(1, 5));
                jogManager.JogIncrementally(letter.ToString(CultureInfo.InvariantCulture), random.Next(0, int.MaxValue), true, random.Next(1, 5));
            }
        }

        [Test]
        public void StopJogTest()
        {
            var pmac = new PMACFake();
            var logger = new FakeLogger();
            var settings = new FakeUserSettingsService();
            var information = new PmacInformation(pmac, settings, logger);
            var configurator = new PmacConfigurator(pmac, logger);
            var jogManager = new PmacJogManager(pmac, information, configurator);

            pmac.Connect(0, 10);
            pmac.CanAnswer = true;

            foreach (var letter in "XYZABCUVW")
            {
                jogManager.StopJog();
            }
        }

        [Test]
        public void TryJogTest()
        {
            Random random;
            PmacJogManager jogManager;
            PrepareToJotTest(out random, out jogManager);

            foreach (var letter in "XYZABCUVW")
            {
                jogManager.TryJog(letter.ToString(CultureInfo.InvariantCulture), false, random.Next(1, 5));
                jogManager.TryJog(letter.ToString(CultureInfo.InvariantCulture), true, random.Next(1, 5));
            }
        }

        private static void PrepareToJotTest(out Random random, out PmacJogManager jogManager)
        {
            random = new Random();
            var pmac = new PMACFake();
            var logger = new FakeLogger();
            var settings = new FakeUserSettingsService();
            var information = new PmacInformation(pmac, settings, logger);
            var configurator = new PmacConfigurator(pmac, logger);
            jogManager = new PmacJogManager(pmac, information, configurator);
            pmac.Connect(0, 10);
            pmac.CanAnswer = true;
        }
    }
}
