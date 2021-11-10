using System;
using NUnit.Framework;
using SimpleJwt.Net.Algorithms;
using SimpleJwt.Net.Exceptions;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class BasicTests
    {
        private static readonly IJwtAlgorithm Algo = new Hs256("secret123");
        private readonly JwtGenerator _generator = new JwtGenerator(Algo, "test");
        private readonly JwtValidator _validator = new JwtValidator(Algo, "test");

        private string _token;
        

        [SetUp]
        public void SetUp()
        {
            TestJwt jwt = new TestJwt
            {
                Secret = "password123",
            };
            
            _token = _generator.Generate<TestJwt>(jwt, new DateTime(2030, 1, 1), "1234");
            Console.WriteLine(_token);
        }

        [Test]
        public void ValidationTest()
        {
            try
            {
                TestJwt token = _validator.Validate<TestJwt>(_token);
                if(token.Secret != "password123")
                    Assert.Fail();
            }
            catch (JwtException e)
            {
                Console.WriteLine("JwtException: " + e.Cause);
                throw;
            }
        }

        [Test]
        public void ExpirationTest()
        {
            Assert.Catch<JwtException>(() =>
            {
                TestJwt token = new TestJwt
                {
                    Iss = "test",
                    Sub = "1234",
                    Iat = DateTime.UtcNow,
                    Exp = new DateTime(2000, 1, 1),
                    Secret = "password"
                };

                string jwt = _generator.GenerateTest(token, new DateTime(2000, 1, 1), "1234");
                _validator.Validate<TestJwt>(jwt);
            });
        }
    }
}