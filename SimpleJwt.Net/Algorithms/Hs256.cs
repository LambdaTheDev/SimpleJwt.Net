using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleJwt.Net.Algorithms
{
    // HMAC256 JWT algorithm implementation
    public sealed class Hs256 : IJwtAlgorithm
    {
        private const int DefaultInputBytesSize = 128;
        
        private static readonly UTF8Encoding Encoding = new UTF8Encoding(false, true); // ENcoding
        private readonly HMACSHA256 _hmac; // HS256 instance
        private byte[] _inputBytes;
        
        public string Code => "HS256";


        // Initializes this algorithm with a secret key
        public Hs256(string secretKey)
        {
            byte[] keyBytes = Encoding.GetBytes(secretKey);
            _hmac = new HMACSHA256(keyBytes);

            _inputBytes = new byte[DefaultInputBytesSize];
        }

        public string Hash(string input)
        {
            int neededBytes = Encoding.GetByteCount(input);
            if(neededBytes > _inputBytes.Length)
                Array.Resize(ref _inputBytes, (int) (neededBytes * 1.5f));

            int usedBytes = Encoding.GetBytes(input, 0, input.Length, _inputBytes, 0);
            byte[] computedBytes = _hmac.ComputeHash(_inputBytes, 0, usedBytes);
            
            return Encoding.GetString(computedBytes);
        }
    }
}