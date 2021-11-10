using System;
using System.Text.Json;
using Base64Url;
using SimpleJwt.Net.Exceptions;

namespace SimpleJwt.Net
{
    // Used to validate JWT tokens
    public class JwtValidator
    {
        private readonly IJwtAlgorithm _algorithm; // Algorithm used by this validator
        private readonly string _issuerName; // Issuer name for validator
        private readonly string _encodedHeader; // One-time cached header, to avoid allocations

        public JwtValidator(IJwtAlgorithm algo, string issuerName)
        {
            _algorithm = algo;
            _issuerName = issuerName;
            
            _encodedHeader = JsonSerializer.Serialize(new JwtHeader { Typ = "JWT", Alg = _algorithm.Code });
            _encodedHeader = Base64.GetBase64(_encodedHeader);
        }
        
        // Returns T type of JWT or throws exception on failure
        public T Validate<T>(string token) where T : struct, IJwtBasicPayload
        {
            // Check if first dot is in place
            // (token template is header.payload.signature)
            if (token[_encodedHeader.Length] != '.')
                throw new JwtException(JwtFailureCause.TokenInvalid);
            
            // Check if second dot is in place
            // This time using for loop, due to not all algos used in JWT
            // have a fixed size
            int secondDotIndex = -1;
            for (int i = token.Length; i >= _encodedHeader.Length; i--)
            {
                if (token[i] == '.')
                {
                    secondDotIndex = i;
                    break;
                }
            }

            if (secondDotIndex == -1)
                throw new JwtException(JwtFailureCause.TokenInvalid);
            
            // Split token into parts
            ReadOnlySpan<char> tokenSpan = token;
            ReadOnlySpan<char> combined = tokenSpan.Slice(0, secondDotIndex - 1);
            ReadOnlySpan<char> payload = tokenSpan.Slice(_encodedHeader.Length, secondDotIndex - 1);
            ReadOnlySpan<char> hash = tokenSpan.Slice(secondDotIndex, token.Length);

            // Wrap validation into try-catch.
            // If anything wrong happens inside, then token is just invalid
            try
            {
                // Parse JWT into an T object
                string payloadStr = payload.ToString();
                T payloadObj = JsonSerializer.Deserialize<T>(payloadStr);
            
                // Validate payload
                if (DateTime.UtcNow >= payloadObj.Exp)
                    throw new JwtException(JwtFailureCause.ExpirationClaimFailed);

                if (DateTime.UtcNow < payloadObj.Iat)
                    throw new JwtException(JwtFailureCause.IssuedAtClaimFailed);

                if (payloadObj.Iss != _issuerName)
                    throw new JwtException(JwtFailureCause.IssuerClaimFailed);
            
                // todo: Implement Advanced payload checks

                // Generate hash for payload & compare them
                ReadOnlySpan<char> comparisonHash = _algorithm.Hash(combined);
                if (!hash.Equals(comparisonHash, StringComparison.Ordinal))
                    throw new JwtException(JwtFailureCause.SignatureInvalid);
                
                // If everything is alright, return object
                return payloadObj;
            }
            catch (System.Exception)
            {
                throw new JwtException(JwtFailureCause.TokenInvalid);
            }
        }
    }
}