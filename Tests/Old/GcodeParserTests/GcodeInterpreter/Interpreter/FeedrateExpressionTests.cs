using NUnit.Framework;

namespace GcodeParser.GcodeInterpreter.Interpreter.Tests
{
    [TestFixture]
    [Category("GCodeParser.Interpreter")]
    public class FeedrateExpressionTests
    {
        [Test]
        public void FeedrateExpressionInterpretTest()
        {
            var exp = new FeedrateExpression();
            var context = new Context { InputString = "X100 Y10.552 Z-2.334 A0.123 F0.35", OutputData = new GFrame() };
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.Feedrate.Value, 0.35f);

            exp = new FeedrateExpression();
            context = new Context { InputString = "X100 Y10.552 Z-2.334 A0.123 F.35", OutputData = new GFrame() };
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.Feedrate.Value, 0.35f);

            exp = new FeedrateExpression();
            context = new Context { InputString = "X100 Y10.552 Z-2.334 A0.123", OutputData = new GFrame() };
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.Feedrate, null);

            exp = new FeedrateExpression();
            context = new Context { InputString = "X100 Y10.552 Z-2.334 A0.123 F35", OutputData = new GFrame() };
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.Feedrate.Value, 35);
        }
    }
}