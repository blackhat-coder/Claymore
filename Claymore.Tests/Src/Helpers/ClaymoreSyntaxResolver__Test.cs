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

        [TestMethod]
        public void ParseBooleanToken_WithValidInput_ReturnsToken()
        {
            string input = "cache:$bool";

            var result = _syntaxValidator.ParseBooleanToken(input);
            Assert.IsNotNull(result);
            Assert.AreEqual("$bool", result);
        }

        [TestMethod]
        public void ParseBooleanToken_WithInvalidInput_ReturnsNull()
        {
            string input = "cache:$#bool";

            var result = _syntaxValidator.ParseBooleanToken(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseStringToken_WithValidInput_ReturnsToken()
        {
            string input = "Description: $string[15]";

            var result = _syntaxValidator.ParseStringToken(input);
            Assert.IsNotNull(result);
            Assert.AreEqual("$string", result.Value.token);
            Assert.AreEqual("15", result.Value.length.ToString());
        }

        [TestMethod]
        public void ParseStringToken_WithInValidInputNoLen_ReturnsNull()
        {
            string input = "Description: $string";

            var result = _syntaxValidator.ParseStringToken(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseStringToken_WithInValidInputLen_ReturnsNull()
        {
            string input = "Description: $string[]";

            var result = _syntaxValidator.ParseStringToken(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseNameToken_WithValidInput_ReturnsToken()
        {
            string input = "firstName: $name";

            var result = _syntaxValidator.ParseNameToken(input);
            Assert.IsNotNull(result);
            Assert.AreEqual("$name", result);
        }

        [TestMethod]
        public void ParseNameToken_WithInValidInput_ReturnsNull()
        {
            string input = "firstName: $namex";

            var result = _syntaxValidator.ParseNameToken(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseNumberToken_WithValidInput_ReturnsTokens()
        {
            string input = "phoneNumber: $number[9]";

            var result = _syntaxValidator.ParseNumberToken(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("$number", result.Value.token);
            Assert.AreEqual("9", result.Value.length.ToString());
        }

        [TestMethod]
        public void ParseNumberToken_WithInValidInput_ReturnsNull()
        {
            string input = "phoneNumber: $number";

            var result = _syntaxValidator.ParseNumberToken(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseNumberToken_WithInValidNumberLength_ReturnsNull()
        {
            string input = "phoneNumber: $number[]";

            var result = _syntaxValidator.ParseNumberToken(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseEmailToken_WithValidInput_ReturnsToken()
        {
            string input = "email: $email";

            var result = _syntaxValidator.ParseEmailToken(input);
            Assert.IsNotNull(result);
            Assert.AreEqual("$email", result);
        }

        [TestMethod]
        public void ParseEmailToken_WithInValidInputOne_ReturnsNull()
        {
            string input = "email: $emailsufixthebread";

            var result = _syntaxValidator.ParseEmailToken(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParseEmailToken_WithInValidInputTwo_ReturnsNull()
        {
            string input = "email: ss$email";

            var result = _syntaxValidator.ParseEmailToken(input);
            Assert.IsNull(result);
        }
    }
}