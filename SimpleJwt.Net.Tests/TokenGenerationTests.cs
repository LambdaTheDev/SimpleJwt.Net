using System;
using System.Text.Json;
using LambdaTheDev.SimpleJwt.Net;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Claims;
using LambdaTheDev.SimpleJwt.Net.Extensions;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests;

[TestFixture]
public class TokenGenerationTests
{
    private readonly JwtTokenOptions _options = new JwtTokenOptions(new JwtSha256Algorithm(), new JsonSerializerOptions());

    [Test]
    public void GenerateTest()
    {
        ExampleToken tokenObj = new ExampleToken
        {
            Sub = "TestToken",
            Iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds(),
            Aud = "TestAudience",
        };

        string token = _options.Generate(tokenObj);
        Console.WriteLine(token);
    }
        
        
    private class ExampleToken : IIssuedAtClaim, IExpirationClaim, ISubjectClaim<string>, IAudienceClaim<string>
    {
        public string Sub { get; set; }
        public string Aud { get; set; }
        public long Iat { get; set; }
        public long Exp { get; set; }
    }
}