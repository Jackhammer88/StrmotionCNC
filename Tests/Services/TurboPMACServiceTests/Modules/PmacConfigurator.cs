using ControllerService.Modules;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading.Tasks;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.Modules
{
    [TestFixture]
    [Category("TurboPMACService")]
    public class PmacConfiguratorTests
    {

        [Test]
        public void SetVariableTest()
        {
            PrepareToGetSetVariables(out Random random, out PMACFake pmac, out PmacConfigurator configurator, out string responseBody);
            pmac.ResponseSended += (s, e) => responseBody = e;

            var randomVariableAddress = $"I{random.Next(1, 100000)}";
            var randomInt = random.Next(int.MinValue, int.MaxValue);
            configurator.SetVariable(randomVariableAddress, randomInt);

            Assert.AreEqual($"{randomVariableAddress}={randomInt}", responseBody);

            responseBody = "NO";
            randomVariableAddress = $"I{random.Next(1, 100000)}";
            var randomDouble = random.Next(int.MinValue, int.MaxValue) + random.NextDouble();
            configurator.SetVariable(randomVariableAddress, randomDouble);

            Assert.AreEqual($"{randomVariableAddress}={randomDouble}", responseBody);

            Assert.AreNotEqual("NO", responseBody);
        }

        private static void PrepareToGetSetVariables(out Random random, out PMACFake pmac, out PmacConfigurator configurator, out string responseBody)
        {
            random = new Random();
            pmac = new PMACFake();
            var logger = new FakeLogger();
            configurator = new PmacConfigurator(pmac, logger);
            pmac.SelectController(out int devNumber);
            pmac.Connect(devNumber, 10);
            pmac.CanAnswer = true;

            responseBody = "NO";
        }

        [Test]
        public void GetVariableTest()
        {
            PrepareToGetSetVariables(out Random random, out PMACFake pmac, out PmacConfigurator configurator, out string responseBody);

            pmac.ResponseSended += (s,e) => responseBody = e;
            //INT
            responseBody = "NO";
            var randomInt = random.Next(1, 1999);
            var randomVariableAddress = $"I{randomInt}";

            configurator.GetVariable(randomVariableAddress, out int result);
            Assert.AreEqual(randomInt, result);

            responseBody = "NO";
            randomInt = random.Next(2000, 9999);
            randomVariableAddress = $"I{randomInt}";

            configurator.GetVariable(randomVariableAddress, out result);
            randomInt = int.Parse(randomInt.ToString(CultureInfo.InvariantCulture), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(randomInt, result);

            //Double - дописать
        }
    }
}
