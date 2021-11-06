using System;
using System.Text.Json;
using Base64Url;

namespace SimpleJwt.Net
{
    // Class used to generate JWT string
    public class JwtGenerator
    {
        private readonly IJwtAlgorithm _algorithm; // JWT Algorithm
        private readonly string _issuerName; // Used in Iss clause
        private readonly string _encodedHeader; // Encoded header

        public JwtGenerator(IJwtAlgorithm algorithm, string issuerName)
        {
            _algorithm = algorithm;
            
            _issuerName = issuerName;

            _encodedHeader = JsonSerializer.Serialize(new Header { Alg = algorithm.Code, Typ = "JWT" });
            _encodedHeader = Base64.GetBase64(_encodedHeader);
        }

        // Generates JWT for 
        public string Generate<T>(DateTime exp, string sub) where T : IJwtPayload, new()
        {
            T payload = new T();

            payload.Iss = _issuerName;
            payload.Sub = sub;
            payload.Iat = DateTime.UtcNow;
            payload.Exp = exp;

            string combined =  _encodedHeader + '.' + Base64.GetBase64(JsonSerializer.Serialize(payload));
            string signature = _algorithm.Hash(combined);

            return combined + '.' + signature;
        }


        // QOL header wrapper
        private struct Header
        {
            public string Alg { get; set; }
            public string Typ { get; set; }
        }
    }
}