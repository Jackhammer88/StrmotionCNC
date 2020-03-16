using ControllerService.Modules;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TurboPMACServiceTests.FakeClasses;

namespace TurboPMACServiceTests.Modules
{
    [TestFixture]
    [Category("TurboPMACService")]
    public class PmacConnectionManagerTests
    {
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        public void PmacConnectionManagerTest()
        {
            var pmac = new PMACFake();
            var userSettings = new FakeUserSettingsService();
            userSettings.ReconnectionCount = 1;
            var logger = new FakeLogger();
            var connectionManager = new PmacConnectionManager(pmac, userSettings, logger);

            bool wasConnected = false;
            bool wasDisconnected = false;
            bool wasLostConnection = false;
            bool wasError = false;

            connectionManager.OnConnect += (s1,e1) => wasConnected = true;
            connectionManager.OnDisconnected += (s2,e2) => wasDisconnected = true;
            connectionManager.OnError += (arg1,arg2) => wasError = true;
            connectionManager.OnLostConnection += (s, arg3) => wasLostConnection = true;

            connectionManager.Connect();
            connectionManager.Disconnect();

            wasConnected = false;
            connectionManager.ConnectAsync(CancellationToken.None).Wait();

            pmac.DoError(100, "Lost connection");
            pmac.DoLostConnection();

            Assert.IsTrue(connectionManager.SelectControllerAsync().Result);

            Assert.IsTrue(wasConnected && wasDisconnected && wasError && wasLostConnection);
        }
    }
}
