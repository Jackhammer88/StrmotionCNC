using GcodeParser.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GcodeParser.GcodeInterpreter.Interpreter.Tests
{
    [TestFixture]
    [Category("GCodeParser.Interpreter")]
    public class MCodeExpressionTests
    {
        [Test]
        public void MCodeExpressionInterpretTest()
        {
            var context = new Context { InputString = "G0 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            var exp = new MCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 3) == 1);

            context = new Context { InputString = "G0 M3 M15 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new MCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 3) == 1);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 15) == 1);

            context = new Context { InputString = "G0 M3 M15 M0 M2.3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            exp = new MCodeExpression();
            exp.Interpret(context);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 3f) == 1);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 15f) == 1);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 0f) == 1);
            Assert.IsTrue(context.OutputData.MCodes.Count(f => f == 2.3f) == 1);
        }
        [Test]
        public void NotValidGCodesTest()
        {
            var context = new Context { InputString = "G0 M3 M4 S200 X20 Y0 Z1", OutputData = new GFrame() };
            var gExp = new MCodeExpression();
            Assert.Throws<MutuallyExclusiveException>(() => gExp.Interpret(context));

            context = new Context { InputString = "G0 G1 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            var mExp = new GCodeExpression();
            Assert.Throws<MutuallyExclusiveException>(() => mExp.Interpret(context));

            context = new Context { InputString = "G0 M3 S200 X20 Y0 Z1", OutputData = new GFrame() };
            gExp = new MCodeExpression();
            gExp.Interpret(context);
            Assert.IsTrue(context.OutputData.MCodes.Contains(3));
        }
    }
}