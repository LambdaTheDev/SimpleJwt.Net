using System;
using System.Security.Cryptography;

namespace LambdaTheDev.SimpleJwt.Net.Algorithms
{
    // Helper class for RNCCrypto... implementations
    public static class CryptoSafeRng
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        public static void FillBytes(ArraySegment<byte> bytes)
        {
            Rng.GetBytes(bytes.Array, bytes.Offset, bytes.Count);
        }
    }
}