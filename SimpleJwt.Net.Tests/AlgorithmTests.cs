using System;
using System.Security.Cryptography;
using System.Text;
using LambdaTheDev.Exyll.Base64EncoderNonAlloc;
using LambdaTheDev.SimpleJwt.Net.Algorithms;
using LambdaTheDev.SimpleJwt.Net.Extensions;
using NUnit.Framework;

namespace SimpleJwt.Net.Tests;

[TestFixture]
public class AlgorithmTests
{
    private readonly JwtSha256Algorithm _sha256 = new JwtSha256Algorithm();


    [Test]
    public void TestBase64ShaHashLength()
    {
        string test = "zfdwdifc9ijwqey98fn89eqwdbyfne7m8neyw";

        ArraySegment<byte> hash = _sha256.ComputeSignature(Encoding.UTF8.GetBytes(test));
        ArraySegment<char> base64 = Base64Encoder.UrlEncoding.ToBaseNonAlloc(hash);
        Assert.That(base64.Count == _sha256.GetSignatureChars());
    }
    
    [Test]
    public void TestBase64TestHashLength()
    {
        string test = "zfdwdifc9ijwqey98fn89eqwdbyfne7m8neyw";

        TestAlgorithm algo = new TestAlgorithm(12);
        ArraySegment<byte> hash = algo.ComputeSignature(Encoding.UTF8.GetBytes(test));
        ArraySegment<char> base64 = Base64Encoder.UrlEncoding.ToBaseNonAlloc(hash);
        
        Console.WriteLine(base64.Count);
        Console.WriteLine(algo.GetSignatureChars());
        Assert.That(base64.Count == algo.GetSignatureChars());
    }
    
    public sealed class TestAlgorithm : IJwtTokenAlgorithm
    {
        public HMACSHA256 Hmac { get; }
        public string Alg => "HS256";
        public int SignatureSize { get; }


        public TestAlgorithm(int signatureSize)
        {
            Hmac = new HMACSHA256();
            SignatureSize = signatureSize;
        }

        public ArraySegment<byte> ComputeSignature(ArraySegment<byte> data)
        {
            if (data.Array == null) return new ArraySegment<byte>();
            byte[] hash = Hmac.ComputeHash(data.Array, data.Offset, data.Count);
            return new ArraySegment<byte>(hash, 0, SignatureSize);
        }
    }
}