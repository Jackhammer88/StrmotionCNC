using ExtensionBoardService.Precitec;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionBoardServiceTests
{
    [TestFixture()]
    public class ErrorValidatorTests
    {
        [Test()]
        public void ParseErrorTest()
        {
            Error cerror = Error.Unknown;
            for (float i = 10; i > 5.625f; i-= 0.15625f)
            {
                var error = ErrorValidator.ParseError(i);
                if(cerror != error)
                {
                    cerror = error;
                    Debug.WriteLine(error.ToString());
                }
            }
            Assert.AreNotEqual(cerror, Error.Unknown);

            cerror = Error.Unknown;
            for (byte i = 0; i < 255; i++)
            {
                var error = ErrorValidator.ParseError(i);
                if (cerror != error)
                {
                    cerror = error;
                    Debug.WriteLine(error.ToString());
                }
            }
            Assert.AreNotEqual(cerror, Error.Unknown);
        }
    }
}
