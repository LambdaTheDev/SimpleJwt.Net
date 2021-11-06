using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleJwt.Net.Algorithms
{
    public class Hs256 : IJwtAlgorithm
    {
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();

        private readonly HMACSHA256 _hmac;
        private readonly byte[] _key = new byte[32];
        public string Code => "HS256";

        private byte[] _dataBytes = new byte[512];
        private int _dataLength;
        

        public Hs256(string secret)
        {
            Encoding.GetBytes(secret, _key);
            _hmac = new HMACSHA256(_dataBytes);
        }
        
        public string Hash(string payload)
        {
            _dataLength = Encoding.GetByteCount(payload);
            if (_dataLength > _dataBytes.Length)
                _dataBytes = new byte[_dataLength + 64];

            byte[] hashBytes = _hmac.ComputeHash(_dataBytes, 0, _dataLength);
            return BitConverter.ToString(hashBytes, 0, hashBytes.Length).Replace("-", "");
        }
    }
}