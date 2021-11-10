﻿using System;
using System.Text;
using System.Text.Json;
using Base64Url;
using SimpleJwt.Net.Exceptions;

namespace SimpleJwt.Net
{
    // Used to generate JWT tokens
    public sealed class JwtGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder(1024);

        private readonly IJwtAlgorithm _algorithm; // Algorithm used by this generator
        private readonly string _issuerName; // Issuer name for generator
        private readonly string _encodedHeader; // One-time cached header, to avoid allocations

        public JwtGenerator(IJwtAlgorithm algo, string issuerName)
        {
            _algorithm = algo;
            _issuerName = issuerName;
            
            _encodedHeader = JsonSerializer.Serialize(new JwtHeader { Typ = "JWT", Alg = _algorithm.Code });
            _encodedHeader = Base64.GetBase64(_encodedHeader);
        }
        
        // Generates JWT for provided type
        public string Generate<T>(T payload, DateTime exp, string sub) where T : struct, IJwtBasicPayload
        {
            if (DateTime.UtcNow >= exp)
                throw new JwtException(JwtFailureCause.TooShortExpiryTime);

            payload.Iss = _issuerName;
            payload.Sub = sub;
            payload.Iat = DateTime.UtcNow;
            payload.Exp = exp;

            string serializedPayload = Base64.GetBase64(JsonSerializer.Serialize(payload));

            _builder.Clear();
            _builder.Append(_encodedHeader).Append('.').Append(serializedPayload);
            
            string combinedHeaderPayload = _builder.ToString();
            string hash = _algorithm.Hash(combinedHeaderPayload);
            _builder.Append('.').Append(hash);

            return _builder.ToString();
        }

        // Used on testing proposes, to test against expired tokens
        internal string GenerateTest<T>(T payload, DateTime exp, string sub) where T : struct, IJwtBasicPayload
        {
            // if (DateTime.UtcNow >= exp)
                // throw new JwtException(JwtFailureCause.TooShortExpiryTime);

            payload.Iss = _issuerName;
            payload.Sub = sub;
            payload.Iat = DateTime.UtcNow;
            payload.Exp = exp;

            string serializedPayload = Base64.GetBase64(JsonSerializer.Serialize(payload));

            _builder.Clear();
            _builder.Append(_encodedHeader).Append('.').Append(serializedPayload);
            
            string combinedHeaderPayload = _builder.ToString();
            string hash = _algorithm.Hash(combinedHeaderPayload);
            _builder.Append('.').Append(hash);

            return _builder.ToString();
        }
    }
}