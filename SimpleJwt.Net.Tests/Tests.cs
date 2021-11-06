using System;
using NUnit.Framework;
using SimpleJwt.Net.Algorithms;
using SimpleJwt.Net.Exception;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class Tests
    {
        private readonly IJwtAlgorithm _algorithm = new Hs256("cn8wynfc7tn7c8sfwf");
        private JwtGenerator _generator;
        private JwtValidator _validator;
        private JwtValidator _invalidValidator;

        private string _token;
        
        [SetUp]
        public void Setup()
        {
            _generator = new JwtGenerator(_algorithm, "test");
            _validator = new JwtValidator(_algorithm, "test");
            _invalidValidator = new JwtValidator(_algorithm, "nope");
            
            DateTimeOffset offset = DateTimeOffset.UtcNow.AddSeconds(10);
            DateTime exp = offset.DateTime;

            _token = _generator.Generate<SimpleJwt>(exp, "user");
        }

        [Test]
        public void TestGenerator()
        {
            Console.WriteLine(_token);
        }

        [Test]
        public void SimpleValidation()
        {
            SimpleJwt jwt = _validator.Validate<SimpleJwt>(_token);
        }

        [Test]
        public void TestInvalidValidator()
        {
            Assert.Catch<JwtException>((() =>
            {
                _invalidValidator.Validate<SimpleJwt>(_token);
            }));
        }

        [Test]
        public void TestExpired()
        {
            Assert.Catch<JwtException>((() =>
            {
                DateTimeOffset offset = DateTimeOffset.UtcNow.AddSeconds(-10);
                DateTime exp = offset.DateTime;

                string token = _generator.Generate<SimpleJwt>(exp, "user");
                _validator.Validate<SimpleJwt>(token);
            }));
        }
    }

    internal struct SimpleJwt : IJwtPayload
    {
        public string Iss { get; set; }
        public string Sub { get; set; }
        public DateTime Iat { get; set; }
        public DateTime Exp { get; set; }
    }
}