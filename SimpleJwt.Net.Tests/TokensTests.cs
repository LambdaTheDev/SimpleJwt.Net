using System;
using LambdaTheDev.SimpleJwt.Net;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Claims;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class TokensTests
    {
        private SimpleJsonWebTokens _jwt = new SimpleJsonWebTokens(new Hmac256(null));

        [Test]
        public void GenerateTest()
        {
            ExampleToken tokenObj = new ExampleToken
            {
                Sub = "TestToken",
                Iat = DateTime.UtcNow,
                Exp = DateTimeOffset.UtcNow.AddDays(1).DateTime,
                Aud = "TestAudience",
            };

            string token = _jwt.Generate(tokenObj);
            Console.WriteLine(token);
        }
        
        
        private class ExampleToken : IIssuedAtClaim, IExpirationClaim, ISubjectClaim<string>, IAudienceClaim<string>
        {
            public string Sub { get; set; }
            public string Aud { get; set; }
            public DateTime Iat { get; set; }
            public DateTime Exp { get; set; }
        }
    }
}