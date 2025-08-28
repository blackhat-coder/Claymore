using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Claymore.Src.Helpers;
using Claymore.Src.Services.TextGeneration;

namespace Claymore.Tests.Src.Helpers
{
    [TestClass]
    public class SyntaxValidator_Test
    {
        private SyntaxValidator _syntaxValidator { get; set; }

        [TestInitialize]
        public void Setup()
        {
            _syntaxValidator = new SyntaxValidator();
        }


        [TestMethod]
        public void ParseReplacementToken_WithRegexStringResponseBody_ReturnsCorrectTuple()
        {
            string input = ":$auth.ResponseBody.payload";

            var result = _syntaxValidator.ParseReplacementToken(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("auth", result.Value.name);
            Assert.AreEqual("ResponseBody", result.Value.part);
            Assert.AreEqual("payload", result.Value.property);
        }

        [TestMethod]
        public void ParseReplacementToken_WithRegexStringResponseHeader_ReturnsCorrectTuple()
        {
            string input = "$auth.ResponseHeader.token";

            var result = _syntaxValidator.ParseReplacementToken(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("auth", result.Value.name);
            Assert.AreEqual("ResponseHeader", result.Value.part);
            Assert.AreEqual("token", result.Value.property);
        }

        [TestMethod]
        public void ParseReplacementToken_WithNonRegexString_ReturnsNull()
        {
            string input = "auth.ResponseBody.token";

            var result = _syntaxValidator.ParseReplacementToken(input);

            Assert.IsNull(result);
        }
    }
}