using System;
using System.Text.Json;
using LambdaTheDev.SimpleJwt.Net;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Claims;
using LambdaTheDev.SimpleJwt.Net.Extensions;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests;

[TestFixture]
public class TokenValidationTests
{
    private readonly JwtTokenOptions _options = new JwtTokenOptions(new JwtSha256Algorithm(), new JsonSerializerOptions());

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
            Iat = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
            Nbf = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
            CustomClaim = "abcd"
        };

        ExampleToken futureTokenObj = new ExampleToken
        {
            Iat = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
            Nbf = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
            CustomClaim = "abcd"
        };

        ExampleToken tooEarlyTokenObj = new ExampleToken
        {
            Iat = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
            Nbf = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
            CustomClaim = "abcd"
        };

        ExampleToken expiredTokenObj = new ExampleToken
        {
            Iat = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
            Nbf = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds(),
            Exp = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
            CustomClaim = "abcd"
        };

        _validToken = _options.Generate(validTokenObj);
        _issuedInFutureToken = _options.Generate(futureTokenObj);
        _tooEarlyToken = _options.Generate(tooEarlyTokenObj);
        _expiredToken = _options.Generate(expiredTokenObj);

        _invalidSignatureToken = _validToken.Substring(0, _validToken.Length - 10);
        _tooShortToken = _options.PreComputedHeader + ".du8y89yn7fy78neg78fcmw78q78784m32mr9f9k";
        _tooLongToken = _options.PreComputedHeader +
                        ".fjiwhjf8w89ny72yn7y329rm2891ur3982u09.9fj890f829k890f2j83f92.dj928j90jr78j22";
        _invalidToken = "mojfimwf9wmci90sicjmwifwf9u9u.ui0f9m9nu9n2uf9me8m.f2WNU09NU898f92j7f";
    }

    [Test]
    public void ValidTokenTest()
    {
        var result = _options.Validate(_validToken, out ExampleToken token);
        Assert.That(result == JwtValidationResult.Success && token.CustomClaim == "abcd");
    }

    [Test]
    public void FutureTokenTest()
    {
        var result = _options.Validate(_issuedInFutureToken, out ExampleToken _);
        Assert.That(result == JwtValidationResult.IssuedInFuture);
    }

    [Test]
    public void TooEarlyTokenTest()
    {
        var result = _options.Validate(_tooEarlyToken, out ExampleToken _);
        Assert.That(result == JwtValidationResult.UsedTooEarly);
    }

    [Test]
    public void ExpiredTokenTest()
    {
        var result = _options.Validate(_expiredToken, out ExampleToken token);
        Assert.That(result == JwtValidationResult.Expired);
    }

    [Test]
    public void InvalidSignatureTokenTest()
    {
        var result = _options.Validate(_invalidSignatureToken, out ExampleToken _);
        // CONSTANT SIZE SIGNATURE ALGORITHMS WILL RETURN INVALID TOKEN INSTEAD OF INVALID SIGNATURE HERE!!!
        Assert.That(result is JwtValidationResult.InvalidSignature or JwtValidationResult.InvalidToken);
    }

    [Test]
    public void TooLongTokenTest()
    {
        var result = _options.Validate(_tooLongToken, out ExampleToken _);
        Assert.That(result == JwtValidationResult.InvalidToken);
    }

    [Test]
    public void TooShortTokenTest()
    {
        var result = _options.Validate(_tooShortToken, out ExampleToken _);
        Assert.That(result == JwtValidationResult.InvalidToken);
    }

    [Test]
    public void InvalidTokenTest()
    {
        var result = _options.Validate(_invalidToken, out ExampleToken _);
        Assert.That(result == JwtValidationResult.InvalidToken);
    }

    public class ExampleToken : IIssuedAtClaim, INotBeforeClaim, IExpirationClaim
    {
        public long Iat { get; set; }
        public long Nbf { get; set; }
        public long Exp { get; set; }
        public string CustomClaim { get; set; }
    }
}