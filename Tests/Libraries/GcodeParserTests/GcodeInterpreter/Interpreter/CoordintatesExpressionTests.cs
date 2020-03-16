using NUnit.Framework;

namespace GcodeParser.GcodeInterpreter.Interpreter.Tests
{
    [TestFixture]
    [Category("GCodeParser.Interpreter")]
    public class CoordintatesExpressionTests
    {
        [Test]
        public void CoordintatesExpressionInterpretTest()
        {
            var exp = new CoordintatesExpression();
            var context = new Context { InputString = "X100 Y10.552 Z-2.334 A0.123", OutputData = new GFrame() };
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.Coordinate.X, 100f);
            Assert.AreEqual(context.OutputData.Coordinate.Y, 10.552f);
            Assert.AreEqual(context.OutputData.Coordinate.Z, -2.334f);
            Assert.AreEqual(context.OutputData.Coordinate.A, 0.123f);
        }
    }
}