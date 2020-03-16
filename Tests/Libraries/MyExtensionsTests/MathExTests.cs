using NUnit.Framework;
using MyExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyExtensions.Tests
{
    [TestFixture()]
    public class MathExTests
    {
        [Test()]
        public void DegreeToRadianTest()
        {
            Assert.AreEqual(MathEx.DegreeToRadian(90), Math.PI / 2);
            Assert.AreEqual(MathEx.DegreeToRadian(180), Math.PI);
            Assert.AreEqual(MathEx.DegreeToRadian(270), 3 * Math.PI / 2);
            Assert.AreEqual(MathEx.DegreeToRadian(360), 2 * Math.PI);
        }
    }
}