using ControllerService.Modules;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.Modules
{
    [TestFixture]
    [Category("TurboPMACService")]
    public class PmacTerminalClientTests
    {
        [Test]
        public void TerminalResponseTest()
        {
            var fakePmac = new PMACFake();
            var client = new PmacTerminalClient(fakePmac);

            Assert.IsFalse(client.TerminalResponse("ver", out string answer));

            fakePmac.SelectController(out int devNumber);
            fakePmac.Connect(devNumber);
            fakePmac.CanAnswer = true;
            Assert.IsTrue(client.TerminalResponse("ver", out answer));
            Assert.AreEqual("ver", answer);

            fakePmac.CanAnswer = false;
            Assert.IsFalse(client.TerminalResponse("ver", out answer));
            Assert.AreEqual(string.Empty, answer);
        }
    }
}
