using System;
using System.Text.Json;
using Base64Url;
using SimpleJwt.Net.Exception;

namespace SimpleJwt.Net
{
    // Class used to validate JWT token
    public class JwtValidator
    {
        private readonly IJwtAlgorithm _algorithm; // JWT Algorithm
        private readonly string _issuerName; // Used in Iss clause

        public JwtValidator(IJwtAlgorithm algorithm, string issuerName)
        {
            _algorithm = algorithm;
            _issuerName = issuerName;
        }

        public T Validate<T>(string token) where T : IJwtPayload
        {
            // todo: Use spans

            string[] splitToken = token.Split('.');
            if (splitToken.Length != 3)
                throw new JwtException(JwtFailureCause.InvalidToken);

            try
            {
                Header header = JsonSerializer.Deserialize<Header>(Base64.ToString(splitToken[0]));
                if (header.Typ != "JWT")
                    throw new JwtException(JwtFailureCause.InvalidType);

                if (header.Alg != _algorithm.Code)
                    throw new JwtException(JwtFailureCause.InvalidAlgorithm);

                T payload = JsonSerializer.Deserialize<T>(Base64.ToString(splitToken[1]));
                if (payload == null)
                    throw new JwtException(JwtFailureCause.InvalidToken);

                if (payload.Iss != _issuerName)
                    throw new JwtException(JwtFailureCause.InvalidIssuer);

                if (payload.Iat > DateTime.UtcNow)
                    throw new JwtException(JwtFailureCause.TooNew);

                if (payload.Exp < DateTime.UtcNow)
                    throw new JwtException(JwtFailureCause.Expired);

                return payload;
            }
            catch (System.Exception)
            {
                throw new JwtException(JwtFailureCause.InvalidToken);
            }
        }
        
        
        // QOL header wrapper
        private struct Header
        {
            public string Alg { get; set; }
            public string Typ { get; set; }
        }
    }
}