using System;
using System.Security.Cryptography;

namespace LambdaTheDev.SimpleJwt.Net.Algorithms
{
    // SHA256 wrapper for SimpleJwt
    public sealed class JwtSha256Algorithm : IJwtTokenAlgorithm
    {
        public HMACSHA256 Hmac { get; }
        public string Alg => "HS256";
        public int SignatureSize => 256 / 8;


        public JwtSha256Algorithm(byte[] key = null)
        {
            if (key == null) Hmac = new HMACSHA256();
            else Hmac = new HMACSHA256(key);
        }

        public ArraySegment<byte> ComputeSignature(ArraySegment<byte> data)
        {
            if (data.Array == null) return new ArraySegment<byte>();
            byte[] hash = Hmac.ComputeHash(data.Array, data.Offset, data.Count);
            return new ArraySegment<byte>(hash);
        }
    }
}