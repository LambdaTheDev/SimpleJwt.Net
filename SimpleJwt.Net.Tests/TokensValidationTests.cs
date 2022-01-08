using System;
using LambdaTheDev.SimpleJwt.Net;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Claims;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests
{
    [TestFixture]
    public class TokensValidationTests
    {
        private SimpleJsonWebTokens _jwt = new SimpleJsonWebTokens(new Hmac256());

        private string _validToken;
        private string _issuedInFutureToken;
        private string _tooEarlyToken;
        private string _expiredToken;
        private string _invalidSignatureToken;
        private string _tooLongToken;
        private string _tooShortToken;
        private string _invalidToken;
        
        
        [SetUp]
        public void Setup()
        {
            ExampleToken validTokenObj = new ExampleToken
            {
                Iat = DateTimeOffset.UtcNow.AddDays(-1).DateTime,
                Nbf = DateTimeOffset.UtcNow.AddMinutes(-10).DateTime,
                Exp = DateTimeOffset.UtcNow.AddDays(1).DateTime,
                CustomClaim = "abcd"
            };
            
            ExampleToken futureTokenObj = new ExampleToken
            {
                Iat = DateTimeOffset.UtcNow.AddDays(1).DateTime,
                Nbf = DateTimeOffset.UtcNow.AddMinutes(-10).DateTime,
                Exp = DateTimeOffset.UtcNow.AddDays(1).DateTime,
                CustomClaim = "abcd"
            };
            
            ExampleToken tooEarlyTokenObj = new ExampleToken
            {
                Iat = DateTimeOffset.UtcNow.AddDays(-1).DateTime,
                Nbf = DateTimeOffset.UtcNow.AddMinutes(10).DateTime,
                Exp = DateTimeOffset.UtcNow.AddDays(1).DateTime,
                CustomClaim = "abcd"
            };
            
            ExampleToken expiredTokenObj = new ExampleToken
            {
                Iat = DateTimeOffset.UtcNow.AddDays(-1).DateTime,
                Nbf = DateTimeOffset.UtcNow.AddMinutes(-10).DateTime,
                Exp = DateTimeOffset.UtcNow.AddDays(-1).DateTime,
                CustomClaim = "abcd"
            };

            _validToken = _jwt.Generate(validTokenObj);
            _issuedInFutureToken = _jwt.Generate(futureTokenObj);
            _tooEarlyToken = _jwt.Generate(tooEarlyTokenObj);
            _expiredToken = _jwt.Generate(expiredTokenObj);

            _invalidSignatureToken = _validToken.Substring(0, _validToken.Length - 10);
            _tooShortToken = _jwt.EncodedHeader + ".du8y89yn7fy78neg78fcmw78q78784m32mr9f9k";
            _tooLongToken = _jwt.EncodedHeader + ".fjiwhjf8w89ny72yn7y329rm2891ur3982u09.9fj890f829k890f2j83f92.dj928j90jr78j22";
            _invalidToken = "mojfimwf9wmci90sicjmwifwf9u9u.ui0f9m9nu9n2uf9me8m.f2WNU09NU898f92j7f";
        }

        [Test]
        public void ValidTokenTest()
        {
            ExampleToken token = _jwt.Validate<ExampleToken>(_validToken);
            Assert.True(token.CustomClaim == "abcd");
        }

        [Test]
        public void FutureTokenTest()
        {
            Assert.Catch(() =>
            {
                _jwt.Validate<ExampleToken>(_issuedInFutureToken);
            });
        }

        [Test]
        public void TooEarlyTokenTest()
        {
            Assert.Catch<InvalidTokenException>(() =>
            {
                _jwt.Validate<ExampleToken>(_tooEarlyToken);
            });
        }
        
        [Test]
        public void ExpiredTokenTest()
        {
            Assert.Catch<InvalidTokenException>(() =>
            {
                _jwt.Validate<ExampleToken>(_expiredToken);
            });
        }
        
        [Test]
        public void InvalidSignatureTokenTest()
        {
            Assert.Catch<InvalidTokenException>(() =>
            {
                _jwt.Validate<ExampleToken>(_invalidSignatureToken);
            });
        }
        
        [Test]
        public void TooLongTokenTest()
        {
            Assert.Catch<InvalidTokenException>(() =>
            {
                _jwt.Validate<ExampleToken>(_tooLongToken);
            });
        }
        
        [Test]
        public void TooShortTokenTest()
        {
            Assert.Catch<InvalidTokenException>(() =>
            {
                _jwt.Validate<ExampleToken>(_tooShortToken);
            });
        }
        
        [Test]
        public void InvalidTokenTest()
        {
            Assert.Catch<InvalidTokenException>(() =>
            {
                _jwt.Validate<ExampleToken>(_invalidToken);
            });
        }
        
        public class ExampleToken : IIssuedAtClaim, INotBeforeClaim, IExpirationClaim
        {
            public DateTime Iat { get; set; }
            public DateTime Nbf { get; set; }
            public DateTime Exp { get; set; }
            public string CustomClaim { get; set; }
        }
    }
}