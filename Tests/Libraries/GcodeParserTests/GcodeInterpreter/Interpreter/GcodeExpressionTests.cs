using System.Linq;
using GcodeParser.GcodeInterpreter.Interpreter;
using GcodeParser;
using NUnit.Framework;

namespace GcodeParserTests.GcodeInterpreter.Interpreter
{
    [TestFixture]
    [Category("GCodeParser.Interpreter")]
    public class GcodeExpressionTests
    {
        [Test]
        public void GCodeExpressionInterpretTest()
        {
            var context = new Context { InputString = "T2 G55 G17 G2 R-10.234 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            var exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.GCodes.Any(c => c == 55));
            Assert.IsTrue(context.OutputData.GCodes.Any(c => c == 17));
            Assert.IsTrue(context.OutputData.GCodes.Any(c => c == 2));
            Assert.AreEqual(context.OutputData.RValue, -10.234f);

            context = new Context { InputString = "T2 G0 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.GCodes.First(), 0);

            context = new Context { InputString = "T2 G2 I0 J2 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.GCodes.First(), 2);
            Assert.AreEqual(context.OutputData.IValue, 0);
            Assert.AreEqual(context.OutputData.JValue, 2);

            context = new Context { InputString = "T2 G3 I1.5 J-2.333 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.GCodes.First(), 3);
            Assert.AreEqual(context.OutputData.IValue, 1.5f);
            Assert.AreEqual(context.OutputData.JValue, -2.333f);

            context = new Context { InputString = "T2 G3 I1.5 K-2.333 M3 S200 X20 Z1", OutputData = new GFrame() };
            exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.GCodes.First(), 3);
            Assert.AreEqual(context.OutputData.IValue, 1.5f);
            Assert.AreEqual(context.OutputData.KValue, -2.333f);

            context = new Context { InputString = "T2 G3 R10 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.GCodes.First(), 3);
            Assert.AreEqual(context.OutputData.RValue, 10f);


            context = new Context { InputString = "T2 G2 R-10.234 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new GCodeExpression();
            exp.Interpret(context);
            Assert.AreEqual(context.OutputData.GCodes.First(), 2);
            Assert.AreEqual(context.OutputData.RValue, -10.234f);
        }
    }
}