using NUnit.Framework;
using System.Collections.Generic;

namespace GcodeParser.GcodeInterpreter.Interpreter.Tests
{
    [TestFixture]
    [Category("GCodeParser.Interpreter")]
    public class TcodeExpressionTests
    {
        [Test]
        public void TCodeExpressionInterpretTest()
        {
            var context = new Context { InputString = "T2 G0 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            var exp = new TCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.ToolNumber == 2f);

            context = new Context { InputString = "T11 G0 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new TCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.ToolNumber == 11f);

            context = new Context { InputString = "T2.3 G0 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new TCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.ToolNumber == 2.3f);
        }
    }
}