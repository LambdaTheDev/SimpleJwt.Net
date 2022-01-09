using System;
using System.Security.Cryptography;

namespace LambdaTheDev.SimpleJwt.Net.Algorithms
{
    // HMAC SHA 256 wrapper. I will try to implement a pre-alloc solution when I have more time
    public sealed class Hmac256 : IJwtAlgorithm
    {
        // Public, to let user change secret key
        public readonly HMACSHA256 Hmac = new HMACSHA256();
        public string Name => "HS256";


        public Hmac256(byte[] secret = null)
        {
            if (secret != null)
                Hmac.Key = secret;
            else
                CryptoSafeRng.FillBytes(new ArraySegment<byte>(Hmac.Key));
        }

        // Just computes HMAC256 hash.
        public byte[] Hash(ArraySegment<byte> data)
        {
            return Hmac.ComputeHash(data.Array, data.Offset, data.Count);
        }
    }
}